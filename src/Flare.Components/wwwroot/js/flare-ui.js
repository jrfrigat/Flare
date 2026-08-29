// Flare misc UI utilities: tab-bar overflow scroller, global keyboard shortcuts and the EyeDropper
// API. Extracted from the former flare-theme.js god-module.

// Scroll watching and scroll-to-top moved to flare-scroll.js (Flare.Components.IScrollService).
// Breakpoint / viewport detection moved to flare-viewport.js (Flare.Components.IBrowserViewportService).

// -- FlareTabs overflow scroller --------------------------------------------
const _tabScrollers = new Map();

// The bar reports three booleans, so the only scroll events worth crossing interop for are the ones
// that flip one of them - a drag from one end to the other has two interesting frames out of a
// hundred. Coalescing to a frame bounds the work per gesture; comparing against the last state
// removes the rest.
export function registerTabScroller(bar, dotNetRef) {
    if (!bar) return;
    let last = null, ticking = false;
    function update() {
        ticking = false;
        const overflowing = bar.scrollWidth > bar.clientWidth + 1;
        const atStart = bar.scrollLeft <= 1;
        const atEnd = bar.scrollLeft + bar.clientWidth >= bar.scrollWidth - 1;
        if (last && last[0] === overflowing && last[1] === atStart && last[2] === atEnd) return;
        last = [overflowing, atStart, atEnd];
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnTabScrollState', overflowing, atStart, atEnd);
    }
    const onScroll = () => { if (!ticking) { ticking = true; requestAnimationFrame(update); } };
    const ro = new ResizeObserver(onScroll);
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

// -- FlareButtonGroup collapse ----------------------------------------------
// A button group that runs out of room folds its trailing segments into an overflow menu. Only the
// browser knows whether they fit, so the decision is made here and the component is told the count.
//
// The hiding is a data attribute rather than a class or an inline style, and that is not cosmetic:
// `class` and `style` are attributes Blazor renders and will rewrite on its next pass, so a decision
// written into either would be silently undone. Blazor never emits this attribute, so the two writers
// never collide - and the component re-applies after each of its renders anyway, which covers a
// segment being replaced outright.
const HIDDEN_ATTR = 'data-flare-bg-hidden';
const _groupCollapsers = new Map();

function _segments(root) {
    return Array.from(root.children).filter(el =>
        (el.classList.contains('flare-btn') || el.classList.contains('flare-toggle-btn')) &&
        !el.classList.contains('flare-btn-group__more'));
}

export function applyButtonGroupOverflow(root, dotNetRef) {
    if (!root || !root.isConnected) return 0;
    const vertical = root.classList.contains('flare-btn-group--vertical');
    const more = root.querySelector(':scope > .flare-btn-group__more');
    const segs = _segments(root);
    if (segs.length === 0) return 0;

    // Measure with everything shown, or a segment hidden on the last pass would measure as zero and
    // never come back when the group grows again. The overflow control is un-hidden for the same
    // reason: its width is what the fold reserves, and a display:none element measures zero.
    segs.forEach(s => s.removeAttribute(HIDDEN_ATTR));
    if (more) more.removeAttribute(HIDDEN_ATTR);
    const size = el => vertical ? el.offsetHeight : el.offsetWidth;
    const available = vertical ? root.clientHeight : root.clientWidth;
    const styles = getComputedStyle(root);
    const gap = parseFloat(vertical ? styles.rowGap : styles.columnGap) || 0;

    const natural = segs.map(size);
    const total = natural.reduce((a, b) => a + b, 0) + gap * (segs.length - 1);

    let visible = segs.length;
    if (total > available + 0.5) {
        // The overflow control has to fit too, so it is reserved before anything is placed.
        const reserve = more ? size(more) + gap : 0;
        let used = 0;
        visible = 0;
        for (let i = 0; i < segs.length; i++) {
            const step = natural[i] + (i > 0 ? gap : 0);
            if (used + step + reserve > available + 0.5) break;
            used += step;
            visible++;
        }
        // Folding everything away leaves a lone "..." with no way back; keep one real segment.
        if (visible === 0) visible = 1;
    }

    for (let i = visible; i < segs.length; i++) segs[i].setAttribute(HIDDEN_ATTR, '');

    // The panel holds a second copy of the same content: it shows exactly what the bar hid.
    const panel = root.querySelector(':scope > .flare-btn-group__more .flare-btn-group__overflow-list');
    if (panel) {
        const copies = Array.from(panel.children);
        copies.forEach((el, i) => {
            if (i < visible) el.setAttribute(HIDDEN_ATTR, '');
            else el.removeAttribute(HIDDEN_ATTR);
        });
    }

    const hidden = segs.length - visible;
    if (more && hidden === 0) more.setAttribute(HIDDEN_ATTR, '');
    if (dotNetRef) dotNetRef.invokeMethodAsync('OnOverflowChanged', hidden).catch(() => {});
    return hidden;
}

export function registerButtonGroupCollapse(root, dotNetRef) {
    if (!root) return;
    const run = () => applyButtonGroupOverflow(root, dotNetRef);
    const ro = new ResizeObserver(run);
    ro.observe(root);
    if (root.parentElement) ro.observe(root.parentElement);
    _groupCollapsers.set(root, ro);
    // No first pass from here: observe() already schedules one, and the component asks for another as
    // soon as this call returns. A third would only measure the same layout again.
}

export function removeButtonGroupCollapse(root) {
    const ro = _groupCollapsers.get(root);
    if (ro) { ro.disconnect(); _groupCollapsers.delete(root); }
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
