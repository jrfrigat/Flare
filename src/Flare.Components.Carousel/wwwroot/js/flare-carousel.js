// Keyboard navigation for the carousel.
//
// The listener sits on the carousel root, but a key only belongs to the carousel when it came from the
// root itself or from one of the carousel's own controls. A text field, an editor or any other widget
// inside a slide owns its arrow keys: pressing an arrow there must move the caret, not the slide. Blazor
// cannot tell those apart - its keyboard event carries no target - so the check lives here and the
// component hears about a key only once it has been claimed.

export function registerKeyboard(root, dotNetRef) {
    if (!root) return;
    const onKeyDown = (event) => {
        if (event.altKey || event.ctrlKey || event.metaKey || event.isComposing) return;
        if (event.key !== 'ArrowLeft' && event.key !== 'ArrowRight') return;
        const target = event.target;
        const fromControl = target instanceof Element
            && root.contains(target)
            && target.closest('[data-flare-carousel-control]') !== null;
        if (target !== root && !fromControl) return;
        event.preventDefault();
        dotNetRef.invokeMethodAsync('OnCarouselKey', event.key).catch(() => { });
    };
    root.addEventListener('keydown', onKeyDown);
    root.__flareCarouselKeyboard = onKeyDown;
}

export function unregisterKeyboard(root) {
    const onKeyDown = root?.__flareCarouselKeyboard;
    if (!onKeyDown) return;
    root.removeEventListener('keydown', onKeyDown);
    delete root.__flareCarouselKeyboard;
}
