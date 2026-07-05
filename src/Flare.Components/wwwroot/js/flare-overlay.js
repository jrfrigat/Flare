// Flare overlay/popup behaviours: body scroll-lock, dialog Esc handling, focus trap, outside-click
// dismiss and fixed-position anchored panels. Extracted from the former flare-theme.js god-module so
// dialogs, drawers, selects and pickers import only what they use.

// --- Body scroll-lock ---
let _scrollLockCount = 0;
let _savedOverflow = '';

export function lockBodyScroll() {
    if (_scrollLockCount === 0) {
        _savedOverflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';
    }
    _scrollLockCount++;
}

export function unlockBodyScroll() {
    if (_scrollLockCount > 0) _scrollLockCount--;
    if (_scrollLockCount === 0) {
        document.body.style.overflow = _savedOverflow;
        _savedOverflow = '';
    }
}

// --- Dialog Esc handlers ---
const _escHandlers = new Map();

export function registerDialogEscHandler(id, dotNetRef) {
    _removeEsc(id);
    const handler = (e) => {
        if (e.key === 'Escape') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('CloseFromEsc');
        }
    };
    _escHandlers.set(id, handler);
    document.addEventListener('keydown', handler);
}

export function removeDialogEscHandler(id) {
    _removeEsc(id);
}

function _removeEsc(id) {
    const h = _escHandlers.get(id);
    if (h) {
        document.removeEventListener('keydown', h);
        _escHandlers.delete(id);
    }
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
// A document-level listener (no overlay element) so the page keeps scrolling normally
// while the popup is open -- a fixed full-screen backdrop would trap the wheel.
const _outsideClick = new Map();

export function registerOutsideClick(id, element, dotNetRef, method) {
    removeOutsideClick(id);
    const handler = (e) => {
        if (element && !element.contains(e.target)) dotNetRef.invokeMethodAsync(method);
    };
    _outsideClick.set(id, handler);
    // Capture phase + pointerdown: fires before the click lands, for any pointer type.
    document.addEventListener('pointerdown', handler, true);
}

export function removeOutsideClick(id) {
    const h = _outsideClick.get(id);
    if (h) { document.removeEventListener('pointerdown', h, true); _outsideClick.delete(id); }
}

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
        panel.style.position = 'fixed';
        panel.style.margin = '0';
        if (opts.matchWidth) panel.style.width = `${a.width}px`;
        const p = panel.getBoundingClientRect();
        const vh = window.innerHeight, vw = window.innerWidth;
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

// -- Unified popup dismissal (combobox family) --
// One handler pair per open popup: a capture-phase pointerdown outside the widget, plus a focusout that
// escapes the widget (Tab away). Replaces the per-component blur timer + separate outside-click, so there
// is no SignalR blur race and no two mechanisms fighting.
const _dismiss = new Map();

export function registerDismiss(id, element, dotNetRef, method) {
    removeDismiss(id);
    const onPointerDown = (e) => {
        if (element && !element.contains(e.target)) dotNetRef.invokeMethodAsync(method);
    };
    const onFocusOut = (e) => {
        const to = e.relatedTarget;
        // Dismiss only when focus actually leaves the widget (ignore moves between its own children).
        if (element && to && !element.contains(to)) dotNetRef.invokeMethodAsync(method);
    };
    _dismiss.set(id, { onPointerDown, onFocusOut, element });
    document.addEventListener('pointerdown', onPointerDown, true);
    if (element) element.addEventListener('focusout', onFocusOut);
}

export function removeDismiss(id) {
    const h = _dismiss.get(id);
    if (h) {
        document.removeEventListener('pointerdown', h.onPointerDown, true);
        if (h.element) h.element.removeEventListener('focusout', h.onFocusOut);
        _dismiss.delete(id);
    }
}
