// Flare overlay/popup behaviours: dialog Esc handling, focus trap, outside-click dismiss and
// fixed-position anchored panels. Extracted from the former flare-theme.js god-module so dialogs,
// drawers, selects and pickers import only what they use.
//
// The body scroll-lock moved to flare-scroll.js (Flare.Components.IScrollService): one counter has to
// own body.style.overflow, or two modules take turns restoring each other's saved value.

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

const _focusTraps = new Map();

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

    dialogEl.addEventListener('keydown', handler);
    _focusTraps.set(id, { handler, dialogEl, previouslyFocused });

    // Focus the first focusable element
    const els = focusable();
    if (els.length > 0) els[0].focus();
}

export function releaseFocusTrap(id) {
    const trap = _focusTraps.get(id);
    if (trap) {
        trap.dialogEl.removeEventListener('keydown', trap.handler);
        _focusTraps.delete(id);
        // Restore focus to the element that was active before the dialog opened
        try { trap.previouslyFocused?.focus(); } catch { }
    }
}

export function focusFirstInDialog(dialogEl) {
    const focusable = dialogEl?.querySelector(FOCUSABLE_SELECTORS);
    focusable?.focus();
}

// --- Outside-click dismiss (e.g. close an open Select when clicking elsewhere) ---
// -- Anchored fixed-position panel (Select / DatePicker / TimePicker / ColorPicker) --
// Positions a popup panel as position:fixed under (or above) its anchor element so it escapes
// any ancestor clipping context -- most notably a Card's overflow:hidden, which would otherwise
// crop the dropdown. Re-positions on scroll (capture phase, so nested scrollers count) and resize
// until removeAnchoredPanel(id) is called. Pass matchWidth:true to size the panel to the anchor.
const _anchoredPanels = new Map();

export function positionAnchoredPanel(id, anchor, panel, options) {
    removeAnchoredPanel(id);
    if (!anchor || !panel) return;
    const opts = options || {};
    const gap = opts.gap ?? 4;
    const margin = 4; // keep this far from the viewport edge

    const place = () => {
        const a = anchor.getBoundingClientRect();
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
        const below = vh - a.bottom, above = a.top;
        // Flip above only when there is not enough room below and more room above.
        let top = (below >= p.height + gap || below >= above)
            ? a.bottom + gap
            : a.top - p.height - gap;
        top = Math.max(margin, Math.min(top, vh - p.height - margin));
        const left = Math.max(margin, Math.min(a.left, vw - p.width - margin));
        panel.style.top = `${top}px`;
        panel.style.left = `${left}px`;
    };

    place();
    window.addEventListener('scroll', place, { passive: true, capture: true });
    window.addEventListener('resize', place, { passive: true });
    _anchoredPanels.set(id, () => {
        window.removeEventListener('scroll', place, { capture: true });
        window.removeEventListener('resize', place);
    });
}

export function removeAnchoredPanel(id) {
    const off = _anchoredPanels.get(id);
    if (off) { off(); _anchoredPanels.delete(id); }
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
const _dismissFocus = new Map();

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
    _dismissFocus.set(id, { onFocusOut, element });
    element.addEventListener('focusout', onFocusOut);
}

export function removeDismiss(id) {
    _dismissPointer.delete(id);
    const h = _dismissFocus.get(id);
    if (h) {
        h.element.removeEventListener('focusout', h.onFocusOut);
        _dismissFocus.delete(id);
    }
}
