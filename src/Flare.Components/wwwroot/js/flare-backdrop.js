// A click can target the common ancestor of different press/release targets. Only a complete
// gesture on a marked backdrop may dismiss it; clicks on content keep their normal bubbling.
if (!globalThis.__flareBackdropGestures) {
    globalThis.__flareBackdropGestures = true;
    let gesture = null;
    const backdrop = target => target instanceof Element && target.hasAttribute('data-flare-backdrop')
        ? target : null;

    document.addEventListener('pointerdown', e => {
        gesture = e.isPrimary && e.button === 0
            ? { id: e.pointerId, start: backdrop(e.target), end: null } : null;
    }, true);
    document.addEventListener('pointerup', e => {
        if (gesture?.id !== e.pointerId) return;
        // Pointer capture retargets pointerup, including the implicit capture used by touch.
        gesture.end = backdrop(document.elementFromPoint(e.clientX, e.clientY));
    }, true);
    document.addEventListener('pointercancel', () => { gesture = null; }, true);
    window.addEventListener('blur', () => { gesture = null; });
    document.addEventListener('click', e => {
        const completed = gesture;
        gesture = null;
        const target = backdrop(e.target);
        if (!target || e.detail === 0) return;
        if (completed?.start === target && completed.end === target
            && (e.pointerId === undefined || e.pointerId === completed.id)) return;
        e.stopImmediatePropagation();
    }, true);
}
