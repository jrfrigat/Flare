// Model-order keyboard slots can outlive their rendered source item in a virtualized list.
const sessions = new Map();
const focusWaiters = new Map();
const bindings = new WeakMap();
const itemSelector = id => `[data-flare-drag="${CSS.escape(id)}"]`;

function stopFocusWait(root) {
    const pending = focusWaiters.get(root);
    if (!pending) return;
    pending.observer.disconnect();
    clearTimeout(pending.timer);
    pending.cleanup();
    focusWaiters.delete(root);
}

export function bindKeyboardDrag(root, dotNetRef) {
    const onKey = e => {
        const session = sessions.get(root);
        if (!session) return;
        if (e.key === 'Tab') {
            session.cancelled = true;
            session.skipFocus = true;
            dotNetRef.invokeMethodAsync('OnKeyboardReorderAsync', 'Escape').catch(() => { });
            return;
        }
        if (![' ', 'Spacebar', 'Escape', 'ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(e.key)) return;
        e.preventDefault();
        e.stopImmediatePropagation();
        session.cancelled = e.key === 'Escape';
        dotNetRef.invokeMethodAsync('OnKeyboardReorderAsync', e.key).catch(() => { });
    };
    bindings.set(root, onKey);
    return () => {
        document.removeEventListener('keydown', onKey, true);
        endKeyboardHint(root, false);
        stopFocusWait(root);
        if (bindings.get(root) === onKey) bindings.delete(root);
    };
}

export function showKeyboardHint(root, targetId, index, sourceId, overId, draw, hide) {
    let session = sessions.get(root);
    if (!session) {
        stopFocusWait(root);
        const source = root.querySelector(itemSelector(sourceId));
        const origin = source?.closest('[data-flare-virtual-count]');
        session = { sourceId, origin, originTop: origin?.scrollTop, cancelled: false };
        session.observer = new MutationObserver(() => session.redraw?.());
        session.observer.observe(root, { childList: true, subtree: true });
        sessions.set(root, session);
        const onKey = bindings.get(root);
        if (onKey) document.addEventListener('keydown', onKey, true);
        root.querySelector('[data-flare-keyboard-focus]')?.focus({ preventScroll: true });
    }
    session.redraw = () => {
        const zone = root.querySelector(`[data-flare-drop="${CSS.escape(targetId)}"]`);
        if (!zone) { hide(); return; }
        const list = zone.querySelector('[data-flare-virtual-count]');
        let anchor = overId ? zone.querySelector(itemSelector(overId)) : null;
        if (list && !anchor) {
            const count = Number(list.dataset.flareVirtualCount);
            list.scrollTop = overId ? index / Math.max(1, count) * list.scrollHeight : list.scrollHeight;
        }
        if (!overId) {
            const items = [...zone.querySelectorAll('[data-flare-drag]')]
                .filter(item => item.dataset.flareDrag !== sourceId && item.closest('[data-flare-drop]') === zone);
            anchor = items.at(-1);
        }
        if (!anchor) { hide(); return; }
        anchor.scrollIntoView({ block: 'nearest', inline: 'nearest' });
        draw(zone, anchor, !overId);
    };
    session.redraw();
}

export function endKeyboardHint(root, restoreFocus = true) {
    const session = sessions.get(root);
    if (!session) return;
    sessions.delete(root);
    session.observer.disconnect();
    const onKey = bindings.get(root);
    if (onKey) document.removeEventListener('keydown', onKey, true);
    if (!restoreFocus || session.skipFocus || !root.isConnected) return;
    if (session.cancelled && session.origin) session.origin.scrollTop = session.originTop;
    const focus = () => {
        const source = root.querySelector(itemSelector(session.sourceId));
        if (!source) return false;
        source.focus({ preventScroll: true });
        return true;
    };
    if (focus() && !session.origin) return;
    // A virtualizer can replace its first restored window once more after measuring card heights.
    // Keep focus on the source through those renders, but release it as soon as the user moves on.
    const stop = () => stopFocusWait(root);
    const onFocus = e => { if (e.target !== root.querySelector(itemSelector(session.sourceId))) stop(); };
    const observer = new MutationObserver(() => { if (root.isConnected) focus(); else stop(); });
    const timer = setTimeout(stop, 5000);
    const cleanup = () => {
        document.removeEventListener('keydown', stop, true);
        document.removeEventListener('pointerdown', stop, true);
        document.removeEventListener('focusin', onFocus, true);
    };
    focusWaiters.set(root, { observer, timer, cleanup });
    document.addEventListener('keydown', stop, true);
    document.addEventListener('pointerdown', stop, true);
    document.addEventListener('focusin', onFocus, true);
    observer.observe(root, { childList: true, subtree: true });
}
