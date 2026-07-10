// Flare misc UI utilities: scroll-to-top button, responsive breakpoint detection, tab-bar overflow
// scroller, global keyboard shortcuts and the EyeDropper API. Extracted from the former
// flare-theme.js god-module.

// --- Scroll-top button ---
const _scrollHandlers = new Map();

// selector: a scroll container to watch; when null/missing the page (window) is used. This lets
// the button work in app-shell layouts where the scroll happens on an inner element, not the window.
export function registerScrollTopHandler(id, dotNetRef, threshold, selector) {
    _removeScrollHandler(id);
    const target = selector ? document.querySelector(selector) : window;
    if (!target) return; // selector did not match (yet) -- nothing to watch
    const scrolled = () => (target === window ? window.scrollY : target.scrollTop);
    const handler = () => dotNetRef.invokeMethodAsync('SetVisible', scrolled() > threshold);
    _scrollHandlers.set(id, { target, handler });
    target.addEventListener('scroll', handler, { passive: true });
    handler(); // initial check
}

export function removeScrollTopHandler(id) { _removeScrollHandler(id); }

function _removeScrollHandler(id) {
    const h = _scrollHandlers.get(id);
    if (h) { h.target.removeEventListener('scroll', h.handler); _scrollHandlers.delete(id); }
}

export function scrollToTop(selector) {
    const target = selector ? document.querySelector(selector) : window;
    if (target) target.scrollTo({ top: 0, behavior: 'smooth' });
}

// Breakpoint / viewport detection moved to flare-viewport.js (Flare.Components.IBrowserViewportService).

// -- FlareTabs overflow scroller --------------------------------------------
const _tabScrollers = new Map();

export function registerTabScroller(bar, dotNetRef) {
    if (!bar) return;
    function update() {
        const overflowing = bar.scrollWidth > bar.clientWidth + 1;
        const atStart = bar.scrollLeft <= 1;
        const atEnd = bar.scrollLeft + bar.clientWidth >= bar.scrollWidth - 1;
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnTabScrollState', overflowing, atStart, atEnd);
    }
    const onScroll = () => update();
    const ro = new ResizeObserver(() => update());
    bar.addEventListener('scroll', onScroll, { passive: true });
    ro.observe(bar);
    _tabScrollers.set(bar, { onScroll, ro });
    update();
}

export function scrollTabs(bar, dir) {
    if (!bar) return;
    bar.scrollBy({ left: dir * bar.clientWidth * 0.8, behavior: 'smooth' });
}

export function removeTabScroller(bar) {
    const handlers = _tabScrollers.get(bar);
    if (handlers) {
        bar.removeEventListener('scroll', handlers.onScroll);
        handlers.ro.disconnect();
        _tabScrollers.delete(bar);
    }
}

// -- FlareShortcuts ----------------------------------------------------------
let _shortcutDotNetRef = null;

export function registerShortcutListener(dotNetRef) {
    _shortcutDotNetRef = dotNetRef;
    document.addEventListener('keydown', _handleShortcutKeyDown);
}

export function removeShortcutListener() {
    document.removeEventListener('keydown', _handleShortcutKeyDown);
    _shortcutDotNetRef = null;
}

function _handleShortcutKeyDown(e) {
    if (!_shortcutDotNetRef) return;
    const parts = [];
    if (e.ctrlKey || e.metaKey) parts.push('ctrl');
    if (e.altKey) parts.push('alt');
    if (e.shiftKey) parts.push('shift');
    const key = e.key.toLowerCase();
    if (key !== 'control' && key !== 'alt' && key !== 'shift' && key !== 'meta') parts.push(key);
    const combo = parts.join('+');
    _shortcutDotNetRef.invokeMethodAsync('HandleKeyDown', combo).catch(() => {});
}

// -- EyeDropper API ----------------------------------------------------------

export function supportsEyeDropper() {
    return 'EyeDropper' in window;
}

export async function openEyeDropper() {
    if (!('EyeDropper' in window)) return null;
    try {
        const result = await new window.EyeDropper().open();
        return result.sRGBHex || null;
    } catch {
        return null;
    }
}
