const workspaces = new WeakMap();

export function register(canvas, callback) {
    unregister(canvas);
    if (!canvas || !callback) return;
    const state = { resize: null, keyboard: null, focus: null, direction: [] };
    const report = () => {
        const rtl = getComputedStyle(canvas).direction === 'rtl';
        callback.invokeMethodAsync('UpdateDockViewport', canvas.clientWidth, canvas.clientHeight, rtl).catch(() => {});
    };
    state.resize = new ResizeObserver(report);
    state.resize.observe(canvas);
    // A direction change does not resize the canvas, so the ResizeObserver never sees it. Watch the
    // elements that can carry dir/class/style, including the document, for a live theme or locale flip.
    if (typeof MutationObserver !== 'undefined') {
        const watch = element => {
            if (!element) return;
            const observer = new MutationObserver(report);
            observer.observe(element, { attributes: true, attributeFilter: ['dir', 'class', 'style'] });
            state.direction.push(observer);
        };
        watch(canvas);
        watch(canvas.parentElement);
        watch(document.body);
        watch(document.documentElement);
    }
    state.keyboard = event => {
        if (event.defaultPrevented || !event.target.closest?.('[data-flare-dock-tabs] [role="tab"]')) return;
        if (['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) event.preventDefault();
    };
    canvas.addEventListener('keydown', state.keyboard);
    workspaces.set(canvas, state);
}

export function unregister(canvas) {
    const state = canvas && workspaces.get(canvas);
    if (!state) return;
    state.resize.disconnect();
    for (const observer of state.direction) observer.disconnect();
    canvas.removeEventListener('keydown', state.keyboard);
    workspaces.delete(canvas);
}

export function clearResize(canvas, property) {
    canvas?.style.removeProperty(property);
}

export function focusTab(canvas, id) {
    const tab = document.getElementById(id);
    if (!tab || !canvas?.contains(tab)) return;
    for (const entry of tab.closest('[role="tablist"]').querySelectorAll('[role="tab"]'))
        entry.tabIndex = entry === tab ? 0 : -1;
    tab.focus();
}

export function captureFocus(canvas) {
    const state = workspaces.get(canvas);
    if (!state) return;
    state.focus = null;
    const target = document.activeElement;
    if (!canvas.contains(target) || !target.closest('[data-flare-dock-pane]')) return;
    const selection = target.isContentEditable ? window.getSelection() : null;
    state.focus = {
        target,
        start: target.selectionStart,
        end: target.selectionEnd,
        direction: target.selectionDirection,
        scrollTop: target.scrollTop,
        scrollLeft: target.scrollLeft,
        anchor: target.contains(selection?.anchorNode) ? selection.anchorNode : null,
        anchorOffset: selection?.anchorOffset,
        focus: selection?.focusNode,
        focusOffset: selection?.focusOffset
    };
}

export function restoreFocus(canvas, fallbackId) {
    const state = workspaces.get(canvas);
    if (!state) return;
    const saved = state.focus;
    state.focus = null;
    if (saved && canvas.contains(saved.target) && !saved.target.closest('[hidden], [inert]')) {
        const { target, start, end, direction, scrollTop, scrollLeft, anchor, anchorOffset, focus, focusOffset } = saved;
        target.focus({ preventScroll: true });
        if (typeof start === 'number') target.setSelectionRange(start, end, direction);
        if (anchor && target.contains(anchor) && target.contains(focus)) {
            const anchorLength = anchor.nodeType === Node.TEXT_NODE ? anchor.length : anchor.childNodes.length;
            const focusLength = focus.nodeType === Node.TEXT_NODE ? focus.length : focus.childNodes.length;
            window.getSelection().setBaseAndExtent(anchor, Math.min(anchorOffset, anchorLength),
                focus, Math.min(focusOffset, focusLength));
        }
        target.scrollTop = scrollTop;
        target.scrollLeft = scrollLeft;
        return;
    }

    const fallback = fallbackId && document.getElementById(fallbackId);
    if (fallback && canvas.contains(fallback)) {
        if (fallback.matches('[role="tab"]')) focusTab(canvas, fallbackId);
        else fallback.focus();
    }
}
