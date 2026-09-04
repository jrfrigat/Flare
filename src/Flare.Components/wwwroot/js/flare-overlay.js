// Flare overlay/popup behaviours: dialog Esc handling, focus trap, outside-click dismiss and
// fixed-position anchored panels. Extracted from the former flare-theme.js god-module so dialogs,
// drawers, selects and pickers import only what they use.
//
// The body scroll-lock moved to flare-scroll.js (Flare.Components.IScrollService): one counter has to
// own body.style.overflow, or two modules take turns restoring each other's saved value.

import { all, listen, registry } from './flare-dom.js';

// --- One document listener per event type, not per widget ---
// Every open dialog used to attach its own keydown, and every open popup its own capture-phase
// pointerdown, so one keystroke or one click walked N independent handlers that each did the same test.
// A bus keeps ONE listener alive while its registry is non-empty and hands the event to the entries, so
// the cost of a second open overlay is a map entry rather than a listener. Attaching and detaching are
// tied to the registry being non-empty, which also means a caller cannot leak a listener by forgetting
// to remove one - dropping the last entry takes it down.
function documentBus(type, options) {
    const entries = new Map();
    let attached = false;

    const dispatch = (e) => {
        // Snapshot: a handler is allowed to remove itself (a dismiss closes its own popup).
        for (const fn of [...entries.values()]) fn(e);
    };

    return {
        set(id, fn) {
            entries.set(id, fn);
            if (!attached) { document.addEventListener(type, dispatch, options); attached = true; }
        },
        delete(id) {
            if (!entries.delete(id)) return;
            if (entries.size === 0 && attached) { document.removeEventListener(type, dispatch, options); attached = false; }
        },
        get size() { return entries.size; },
    };
}

// --- Dialog Esc handlers ---
const _escHandlers = documentBus('keydown');

export function registerDialogEscHandler(id, dotNetRef) {
    _escHandlers.set(id, (e) => {
        if (e.key === 'Escape') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('CloseFromEsc').catch(() => { });
        }
    });
}

export function removeDialogEscHandler(id) {
    _escHandlers.delete(id);
}

// --- Focus trap for dialogs ---
const FOCUSABLE_SELECTORS =
    'a[href]:not([disabled]), button:not([disabled]), input:not([disabled]), ' +
    'select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

const _focusTraps = registry();

export function trapFocus(id, dialogEl) {
    releaseFocusTrap(id);

    const focusable = () => Array.from(dialogEl.querySelectorAll(FOCUSABLE_SELECTORS))
        .filter(el => !el.closest('[hidden]') && el.offsetParent !== null);

    const previouslyFocused = document.activeElement;

    const handler = (e) => {
        if (e.key !== 'Tab') return;
        const els = focusable();
        if (els.length === 0) { e.preventDefault(); return; }
        const first = els[0];
        const last = els[els.length - 1];
        if (e.shiftKey) {
            if (document.activeElement === first) { e.preventDefault(); last.focus(); }
        } else {
            if (document.activeElement === last) { e.preventDefault(); first.focus(); }
        }
    };

    const off = listen(dialogEl, 'keydown', handler);
    _focusTraps.keep(id, () => {
        off();
        // Restore focus to whatever was active before the dialog opened.
        try { previouslyFocused?.focus(); } catch { }
    });

    // Focus the first focusable element
    const els = focusable();
    if (els.length > 0) els[0].focus();
}

export function releaseFocusTrap(id) {
    _focusTraps.drop(id);
}

export function focusFirstInDialog(dialogEl) {
    const focusable = dialogEl?.querySelector(FOCUSABLE_SELECTORS);
    focusable?.focus();
}

// --- The browser's top layer ---
// A floating panel has to be readable wherever its component was placed - in a Card, in a scrolling
// grid, in a dialog. `position: fixed` is only half of that: it escapes an ancestor's `overflow`, and
// it still loses to a later stacking context. The top layer is the other half - it paints above every
// stacking context on the page and is clipped by nothing - and the `popover` attribute is how a plain
// element is put in it.
//
// The attribute is added HERE and not in the markup on purpose. An element carrying `popover` is
// `display: none` until something shows it, so a page whose JS never ran would render no panel at all;
// adding it from the code that immediately shows it means a failed interop degrades to the plain fixed
// panel, which is what the select family shipped before this. `manual` rather than `auto`: light
// dismiss and the Escape handling stay with the component, which already owns both.
function promote(panel) {
    if (typeof panel.showPopover !== 'function') return;
    if (!panel.hasAttribute('popover')) panel.setAttribute('popover', 'manual');
    // Showing an already-shown popover throws, and a re-position on an open panel hits that every time.
    try { if (!panel.matches(':popover-open')) panel.showPopover(); } catch { }
}

function demote(panel) {
    if (!panel || !panel.hasAttribute('popover')) return;
    // A panel removed from the DOM leaves the top layer on its own; one still connected has to be told.
    try { if (panel.isConnected && panel.matches(':popover-open')) panel.hidePopover(); } catch { }
    panel.removeAttribute('popover');
}

// -- Anchored fixed-position panel (every floating surface in the library) --
// Positions a popup panel as position:fixed against its anchor so it escapes any ancestor clipping
// context -- a Card's overflow:hidden, a grid's scroll container, a dialog -- and promotes it to the
// top layer so nothing paints over it. Re-positions on scroll (capture phase, so nested scrollers
// count) and resize until removeAnchoredPanel(id) is called.
//
// options:
//   placement   'bottom-start' (default) | 'bottom' | 'bottom-end' | the same for top/left/right.
//               The side flips when the panel does not fit and the opposite side has more room;
//               the alignment does not flip, it is clamped to the viewport like everything else.
//   gap         distance from the anchor in px (default 4).
//   matchWidth  keep the panel at least as wide as the anchor.
//   anchorPoint {xPct, yPct} - anchor to a POINT inside the anchor element rather than to its box,
//               given as a percentage of that box. This is how a chart tooltip anchors to a data
//               point: the percentage is what the component already knows, and re-measuring the
//               element on scroll keeps the point correct without the caller re-sending it.
//   anchorRect  {x, y, width, height} in viewport coordinates - an anchor with no element at all
//               (a context menu pinned to the pointer). Static by nature, so it does not follow
//               scrolling; nothing that uses it did before either.
//   topLayer    false to keep the panel out of the top layer (default true).
const _anchoredPanels = registry();
// id -> the element currently held in the top layer. Separate from the listener registry because the
// two have different lifetimes: a panel that follows a moving anchor (a chart tooltip tracking the
// pointer) re-registers its listeners on every move, and hiding and re-showing the popover each time
// would restart its animation and flicker. The top layer is released when the panel CHANGES or when
// the caller removes it, not on every re-position.
const _topLayer = new Map();

const SIDES = ['bottom', 'top', 'left', 'right'];

export function positionAnchoredPanel(id, anchor, panel, options) {
    _anchoredPanels.drop(id);
    const held = _topLayer.get(id);
    if (held && held !== panel) { _topLayer.delete(id); demote(held); }
    if (!panel) return;
    const opts = options || {};
    const fixedRect = opts.anchorRect;
    if (!anchor && !fixedRect) return;
    const gap = opts.gap ?? 4;
    const margin = 4; // keep this far from the viewport edge
    const point = opts.anchorPoint;
    const offset = opts.anchorOffset;
    const parts = String(opts.placement || 'bottom-start').split('-');
    const wantSide = SIDES.includes(parts[0]) ? parts[0] : 'bottom';
    const align = parts[1] === 'start' || parts[1] === 'end' ? parts[1] : (parts.length > 1 ? 'start' : 'center');

    const anchorBox = () => {
        if (fixedRect) {
            const w = fixedRect.width ?? 0, h = fixedRect.height ?? 0;
            return { left: fixedRect.x, top: fixedRect.y, right: fixedRect.x + w, bottom: fixedRect.y + h, width: w, height: h };
        }
        const r = anchor.getBoundingClientRect();
        if (!point && !offset) return r;
        // Collapse the anchor to the requested point inside it - the panel then treats that point as a
        // zero-size anchor, so every placement and flip rule below applies to it unchanged. Percentages
        // suit a caller that thinks in the element's own coordinate space (a chart's viewBox); pixels
        // suit one that has already measured (a code editor's caret).
        const x = offset ? r.left + (offset.x ?? 0) : r.left + r.width * ((point.xPct ?? 0) / 100);
        const y = offset ? r.top + (offset.y ?? 0) : r.top + r.height * ((point.yPct ?? 0) / 100);
        return { left: x, top: y, right: x, bottom: y, width: 0, height: 0 };
    };

    const place = () => {
        const a = anchorBox();
        const vh = window.innerHeight, vw = window.innerWidth;
        panel.style.position = 'fixed';
        panel.style.margin = '0';
        if (opts.matchWidth) {
            // At least as wide as the field, and wider when an option needs it. Pinning the panel to
            // the field's width made the list clip the very values it exists to show - a name only
            // slightly longer than its box became unreadable at the point of choosing it. Capped so
            // a long option widens the list without pushing it off the screen.
            panel.style.minWidth = `${a.width}px`;
            panel.style.width = 'max-content';
            panel.style.maxWidth = `${vw - 2 * margin}px`;
        }
        const p = panel.getBoundingClientRect();
        const room = { top: a.top, bottom: vh - a.bottom, left: a.left, right: vw - a.right };
        const opposite = { top: 'bottom', bottom: 'top', left: 'right', right: 'left' };
        const need = (s) => (s === 'top' || s === 'bottom' ? p.height : p.width) + gap;
        // Flip only when the preferred side is short AND the opposite one is roomier: a panel taller
        // than the viewport fits nowhere, and flipping it would just move the clipped half.
        const side = room[wantSide] >= need(wantSide) || room[wantSide] >= room[opposite[wantSide]]
            ? wantSide
            : opposite[wantSide];

        let top, left;
        if (side === 'bottom' || side === 'top') {
            top = side === 'bottom' ? a.bottom + gap : a.top - p.height - gap;
            left = align === 'end' ? a.right - p.width
                : align === 'center' ? a.left + a.width / 2 - p.width / 2
                    : a.left;
        } else {
            left = side === 'right' ? a.right + gap : a.left - p.width - gap;
            top = align === 'end' ? a.bottom - p.height
                : align === 'center' ? a.top + a.height / 2 - p.height / 2
                    : a.top;
        }
        panel.style.top = `${Math.max(margin, Math.min(top, vh - p.height - margin))}px`;
        panel.style.left = `${Math.max(margin, Math.min(left, vw - p.width - margin))}px`;
        // Two facts the stylesheet needs back. `flarePlaced` switches a panel's resting CSS off - the
        // edges and centring transforms that put it under its anchor without script, and that would
        // otherwise fight these coordinates. `flareSide` is where it ACTUALLY landed, which is what an
        // arrow has to point away from; the component's own placement parameter is only a preference.
        panel.dataset.flarePlaced = '';
        panel.dataset.flareSide = side;
    };

    if (opts.topLayer !== false) { promote(panel); _topLayer.set(id, panel); }
    place();

    _anchoredPanels.keep(id, all(
        listen(window, 'scroll', place, { passive: true, capture: true }),
        listen(window, 'resize', place, { passive: true }),
    ));
}

// The anchor named by its DOM id, for a panel whose anchor is one of many rendered in a loop: a single
// captured element reference there holds whichever one rendered last, which is not the one that opened.
export function positionAnchoredPanelById(id, anchorElementId, panel, options) {
    positionAnchoredPanel(id, document.getElementById(anchorElementId), panel, options);
}

export function removeAnchoredPanel(id) {
    _anchoredPanels.drop(id);
    const held = _topLayer.get(id);
    if (held) { _topLayer.delete(id); demote(held); }
}

// The top layer on its own, for a panel that computes its own coordinates.
export function raiseToTopLayer(panel) {
    if (panel) promote(panel);
}

export function dropFromTopLayer(panel) {
    demote(panel);
}

// -- Tooltips: two delegated listeners for the whole page --
// A tooltip is revealed by CSS (:hover / :focus-within), which is why it costs nothing until somebody
// uses it - and also why it cannot leave the box it was declared in. Placing it needs script, but not a
// registration per instance: one delegated listener finds the bubble from the event's target, so a page
// with two hundred tooltips pays for two listeners and one interop call in total.
//
// There is no matching leave handler on purpose. The panel keeps its coordinates while it fades out -
// releasing it there would drop it back to its resting position mid-fade, in full view - and the next
// hover simply places it again.
let _tooltipsReady = false;

const TOOLTIP_SIDES = [
    ['flare-tooltip--bottom', 'bottom'],
    ['flare-tooltip--left', 'left'],
    ['flare-tooltip--right', 'right'],
];

// A CSS length resolved to pixels, so a theme's token keeps deciding the distance from the trigger
// rather than a number baked into this file. px, rem and em cover every unit the tokens use.
function cssLengthPx(el, name, fallback) {
    const raw = getComputedStyle(el).getPropertyValue(name).trim();
    const n = parseFloat(raw);
    if (!Number.isFinite(n)) return fallback;
    if (raw.endsWith('rem')) return n * parseFloat(getComputedStyle(document.documentElement).fontSize);
    if (raw.endsWith('em')) return n * parseFloat(getComputedStyle(el).fontSize);
    return n;
}

function placeTooltip(e) {
    const target = e.target;
    if (!(target instanceof Element)) return;
    const root = target.closest('.flare-tooltip');
    if (!root || root.classList.contains('flare-tooltip--disabled')) return;
    const bubble = root.querySelector(':scope > .flare-tooltip__content');
    if (!bubble?.id) return;
    let side = 'top';
    for (const [cls, name] of TOOLTIP_SIDES) { if (root.classList.contains(cls)) { side = name; break; } }

    // A bubble at rest is `content-visibility: hidden`, so it measures as if it had no contents: 24px
    // wide instead of 181 on the Gallery's own tooltip. Placed from that measurement, a centred bubble
    // lands half its width out and a side-placed one lands on top of the trigger it was meant to sit
    // beside. Both lines below are needed and were measured: clearing the containment alone changes
    // nothing, because the stylesheet TRANSITIONS `content-visibility` (with `allow-discrete`, so it can
    // fade rather than vanish), and a transitioned property does not take its new value until the
    // transition starts a frame or two later. Dropping it from the transition list makes the change
    // discrete and immediate.
    //
    // Both stay set for the life of the bubble, which is correct: the containment exists to stop a
    // HIDDEN bubble giving its scroll container overflow (see HiddenOverlayFootprintTests), and the next
    // line takes this one out of flow and into the top layer, where it contributes to no container at
    // all. The fade still runs - opacity and visibility keep their transitions.
    bubble.style.transitionProperty = 'opacity, visibility';
    bubble.style.contentVisibility = 'visible';
    positionAnchoredPanel(bubble.id, root, bubble, {
        placement: side,
        gap: cssLengthPx(bubble, '--flare-tooltip-offset', 8),
    });
}

export function initFloatingTooltips() {
    if (_tooltipsReady) return;
    _tooltipsReady = true;
    // Capture phase: pointerover does not bubble out of a shadow root or a disabled control, and the
    // trigger may be either.
    document.addEventListener('pointerover', placeTooltip, true);
    document.addEventListener('focusin', placeTooltip, true);
}

// -- Scroll the keyboard-highlighted option into view within its listbox scroll container. --
export function scrollOptionIntoView(optionId, block) {
    const el = document.getElementById(optionId);
    if (el) el.scrollIntoView({ block: block || 'nearest', inline: 'nearest' });
}

// -- Unified popup dismissal (every dismissible popup) --
// One handler pair per open popup: a capture-phase pointerdown outside the widget, plus a focusout that
// escapes the widget (Tab away). Replaces the per-component blur timer + separate outside-click, so there
// is no SignalR blur race and no two mechanisms fighting.
//
// A `registerOutsideClick` pair used to sit above this one with the identical pointerdown handler and no
// focusout, leaving its one caller (the colour picker) open when focus tabbed out of it. There is one
// mechanism now.
// The pointerdown listener is on document rather than on a backdrop element, so the page keeps scrolling
// normally while the popup is open -- a fixed full-screen backdrop would trap the wheel.
// The pointerdown half goes on the shared bus; the focusout half stays on the widget's own element,
// because it has to be scoped to that subtree to mean anything.
const _dismissPointer = documentBus('pointerdown', true);
const _dismissFocus = registry();

export function registerDismiss(id, element, dotNetRef, method) {
    removeDismiss(id);
    _dismissPointer.set(id, (e) => {
        if (element && !element.contains(e.target)) dotNetRef.invokeMethodAsync(method).catch(() => { });
    });

    if (!element) return;
    const onFocusOut = (e) => {
        const to = e.relatedTarget;
        // Dismiss only when focus actually leaves the widget (ignore moves between its own children).
        if (to && !element.contains(to)) dotNetRef.invokeMethodAsync(method).catch(() => { });
    };
    _dismissFocus.keep(id, listen(element, 'focusout', onFocusOut));
}

export function removeDismiss(id) {
    _dismissPointer.delete(id);
    _dismissFocus.drop(id);
}
