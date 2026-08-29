// Flare shared DOM plumbing. The pieces every other module in this folder needs: teardown registries,
// listener attachment that hands back its own removal, and the "which object do I measure and listen
// on" resolution.
//
// None of this is a component behaviour. It is here because it was previously written out once per
// feature - fifteen near-identical Map registries and three separate answers to element resolution -
// and each copy was a place to forget a removeEventListener. One dead code path did exactly that and
// leaked two window listeners per call before it was noticed.

// -- Teardown registries ------------------------------------------------------
// A registry holds one teardown closure per id. `keep` replaces any previous entry by running its
// teardown first, so registering twice cannot leak the first registration; `drop` runs and forgets.
// Teardown is structural rather than remembered: the only way to put something in is to have already
// produced the way to take it back out.
export function registry() {
    const entries = new Map();
    return {
        keep(id, teardown) {
            const prev = entries.get(id);
            if (prev) { entries.delete(id); prev(); }
            entries.set(id, teardown);
        },
        drop(id) {
            const teardown = entries.get(id);
            if (!teardown) return false;
            entries.delete(id);
            teardown();
            return true;
        },
        has(id) { return entries.has(id); },
        keys() { return [...entries.keys()]; },
        clear() { for (const id of [...entries.keys()]) this.drop(id); },
        get size() { return entries.size; },
    };
}

// Attach a listener and return the function that removes it, with the same target, type, handler and
// options captured - the four things a hand-written removal has to repeat and can get wrong.
export function listen(target, type, handler, options) {
    if (!target) return () => { };
    target.addEventListener(type, handler, options);
    return () => target.removeEventListener(type, handler, options);
}

// Combine teardowns into one, run in reverse order of registration.
export function all(...teardowns) {
    return () => { for (let i = teardowns.length - 1; i >= 0; i--) teardowns[i]?.(); };
}

// -- Element resolution -------------------------------------------------------

// The page's scroll state lives on the scrolling element, not on window: window carries the offsets
// but not the extents, and the two must come from the same object or a progress reading lands off by a
// viewport.
export function pageTarget() {
    return document.scrollingElement || document.documentElement;
}

// The page's scroll event fires on document, not on the scrolling element, so the listen target and
// the measure target are deliberately different objects.
export function listenTarget(element) {
    return element || window;
}

// A target arrives as an element reference OR a CSS selector; a selector is resolved at call time, so
// a panel that mounts after the caller did still resolves on the next call rather than never.
export function resolve(element, selector) {
    if (element) return element;
    if (selector) return document.querySelector(selector);
    return null;
}

// Nearest scrollable ancestor of el on the given axis, or null when the page itself scrolls. An
// element that merely declares overflow:auto does not count until it actually overflows, or a panel
// sized to its content would swallow the scroll the caller meant to watch.
export function scrollParent(el, axis) {
    const horizontal = axis === 'x';
    for (let p = el && el.parentElement; p; p = p.parentElement) {
        const style = getComputedStyle(p);
        const overflow = horizontal ? style.overflowX : style.overflowY;
        if (overflow !== 'auto' && overflow !== 'scroll') continue;
        if (horizontal ? p.scrollWidth > p.clientWidth : p.scrollHeight > p.clientHeight) return p;
    }
    return null;
}
