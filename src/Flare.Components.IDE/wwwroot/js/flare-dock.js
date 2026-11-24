const workspaces = new WeakMap();

function observeTabFit(canvas) {
    const headers = new Map();
    const pending = new Set();
    const enabled = 'data-flare-dock-fit-enabled';
    let frame = 0;

    function update() {
        frame = 0;
        const rows = [];
        for (const strip of pending) {
            const bar = strip.querySelector('[data-flare-dock-tabs]');
            if (!strip.isConnected || !bar) continue;
            bar.removeAttribute('data-flare-dock-compact');
            rows.push({ strip, bar });
        }
        pending.clear();

        // The strip includes the width taken by any arrows. Compare against that width so an
        // arrow cannot keep itself visible, or keep labels hidden, once the tabs fit without it.
        for (const row of rows) {
            row.limit = row.strip.clientWidth + 1;
            row.compact = row.bar.scrollWidth > row.limit;
        }
        for (const { bar, compact } of rows)
            bar.toggleAttribute('data-flare-dock-compact', compact);
        for (const row of rows)
            row.fits = !row.compact || row.bar.scrollWidth <= row.limit;
        for (const { strip, fits } of rows)
            strip.toggleAttribute('data-flare-dock-fits', fits);
    }

    function queue(strip) {
        pending.add(strip);
        if (!frame) frame = requestAnimationFrame(update);
    }

    const resize = new ResizeObserver(entries => {
        for (const { target } of entries) queue(target);
    });
    const refresh = () => {
        for (const { strip } of headers.values()) queue(strip);
    };

    function clearFit(strip) {
        pending.delete(strip);
        strip.removeAttribute('data-flare-dock-fits');
        strip.querySelector('[data-flare-dock-tabs]')?.removeAttribute('data-flare-dock-compact');
    }

    function sync() {
        for (const [header, { strip, content }] of headers) {
            if (header.parentElement === canvas && header.hasAttribute(enabled)) continue;
            resize.unobserve(strip);
            content.disconnect();
            clearFit(strip);
            headers.delete(header);
        }
        for (const header of canvas.children) {
            if (!header.hasAttribute(enabled) || headers.has(header)) continue;
            const strip = header.querySelector('.flare-tabs__strip');
            if (!strip) continue;
            const content = new MutationObserver(() => queue(strip));
            content.observe(header, {
                subtree: true, childList: true, characterData: true,
                attributes: true, attributeFilter: ['class', 'style', 'dir', 'hidden']
            });
            headers.set(header, { strip, content });
            resize.observe(strip);
        }
    }

    const layout = new MutationObserver(sync);
    layout.observe(canvas, { childList: true });
    const policy = new MutationObserver(sync);
    policy.observe(canvas, { subtree: true, attributes: true, attributeFilter: [enabled] });
    document.fonts?.addEventListener('loadingdone', refresh);
    sync();
    return {
        refresh,
        dispose() {
            layout.disconnect();
            policy.disconnect();
            resize.disconnect();
            for (const { strip, content } of headers.values()) {
                content.disconnect();
                clearFit(strip);
            }
            document.fonts?.removeEventListener('loadingdone', refresh);
            cancelAnimationFrame(frame);
        }
    };
}

// The header is as tall as the theme's tabs; the body needs that height in CSS. Measure each header
// and publish it on the canvas, where the pane styles read it. Nothing here decides a height.
function syncHeaders(canvas, state) {
    if (!canvas) return;
    for (const header of canvas.querySelectorAll('[data-flare-dock-header]')) {
        const id = header.dataset.flareDockHeader;
        if (!id || !header.isConnected) continue;
        const height = header.getBoundingClientRect().height;
        if (height <= 0) continue;
        const property = '--flare-dock-' + id + '-header';
        const value = Math.round(height * 100) / 100 + 'px';
        if (canvas.style.getPropertyValue(property) !== value)
            canvas.style.setProperty(property, value);
    }
    if (state) {
        state.headerResize?.disconnect();
        for (const header of canvas.querySelectorAll('[data-flare-dock-header]'))
            state.headerResize?.observe(header);
    }
}

// Layout lengths are resolved by the browser, not by C#: the tokens may be var() aliases or rem, and
// only the cascade knows their pixels. Two probes read the effective gutter and group minimum.
function measureTokens(canvas) {
    const gutter = document.createElement('div');
    gutter.style.cssText = 'position:absolute;visibility:hidden;pointer-events:none;block-size:0;' +
        'inline-size:var(--flare-dock-gutter, var(--flare-splitter-gutter-size, 12px));';
    const groupMin = document.createElement('div');
    groupMin.style.cssText = 'position:absolute;visibility:hidden;pointer-events:none;inline-size:0;' +
        'block-size:var(--flare-dock-group-min, 96px);';
    canvas.append(gutter, groupMin);
    const width = gutter.getBoundingClientRect().width;
    const height = groupMin.getBoundingClientRect().height;
    gutter.remove();
    groupMin.remove();
    return { gutter: width > 0 ? width : 12, groupMin: height > 0 ? height : 96 };
}

export function register(canvas, callback) {
    unregister(canvas);
    if (!canvas || !callback) return;
    const state = { resize: null, keyboard: null, focus: null, direction: [], outside: null,
        headerResize: null, headerStyle: null, tabFit: null };
    const syncTabFit = () => {
        if (canvas.hasAttribute('data-flare-dock-adaptive-tabs')) {
            state.tabFit ??= observeTabFit(canvas);
        } else if (state.tabFit) {
            state.tabFit.dispose();
            state.tabFit = null;
        }
    };
    syncTabFit();
    let last = null;
    const report = () => {
        const width = canvas.clientWidth;
        const height = canvas.clientHeight;
        const rtl = getComputedStyle(canvas).direction === 'rtl';
        const tokens = measureTokens(canvas);
        const gutter = Math.max(1, Math.round(tokens.gutter));
        const groupMin = Math.max(1, Math.round(tokens.groupMin));
        syncHeaders(canvas, state);
        state.tabFit?.refresh();
        if (last && last.width === width && last.height === height && last.rtl === rtl &&
            last.gutter === gutter && last.groupMin === groupMin) return;
        last = { width, height, rtl, gutter, groupMin };
        callback.invokeMethodAsync('UpdateDockViewport', width, height, rtl, gutter, groupMin).catch(() => {});
    };
    state.resize = new ResizeObserver(report);
    state.resize.observe(canvas);
    // Blazor owns the canvas style attribute; a re-render replaces it and drops the measured header
    // variables. Measure again after such a change; the guard in syncHeaders prevents a write loop.
    if (typeof ResizeObserver !== 'undefined') {
        state.headerResize = new ResizeObserver(() => syncHeaders(canvas, state));
    }
    if (typeof MutationObserver !== 'undefined') {
        state.headerStyle = new MutationObserver(() => {
            syncTabFit();
            syncHeaders(canvas, state);
        });
        state.headerStyle.observe(canvas, {
            attributes: true, attributeFilter: ['style', 'data-flare-dock-adaptive-tabs']
        });
    }
    // A direction change does not resize the canvas, so the ResizeObserver never sees it. Watch every
    // ancestor that can carry dir/class/style, including the document, for a live theme or locale flip.
    // The canvas itself is watched for dir/class only: its style carries the splitter preview, and a
    // pointer move must not turn into an interop call.
    if (typeof MutationObserver !== 'undefined') {
        const watch = (element, filter) => {
            if (!element) return;
            const observer = new MutationObserver(report);
            observer.observe(element, { attributes: true, attributeFilter: filter });
            state.direction.push(observer);
        };
        watch(canvas, ['dir', 'class']);
        for (let element = canvas.parentElement; element; element = element.parentElement)
            watch(element, ['dir', 'class', 'style']);
    }
    state.keyboard = event => {
        const tab = event.target.closest?.('[data-flare-dock-tabs] [role="tab"]');
        if (event.defaultPrevented || !tab) return;
        if (tab.getAttribute('aria-haspopup') === 'menu' &&
            (event.key === 'ContextMenu' || (event.key === 'F10' && event.shiftKey))) {
            event.preventDefault();
            const rect = tab.getBoundingClientRect();
            const clientX = getComputedStyle(tab).direction === 'rtl' ? rect.right : rect.left;
            tab.dispatchEvent(new MouseEvent('contextmenu', {
                bubbles: true, cancelable: true, button: 2, clientX, clientY: rect.bottom
            }));
            return;
        }
        if (['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) event.preventDefault();
    };
    canvas.addEventListener('keydown', state.keyboard);
    // A pointer elsewhere dismisses the tab actions without another round trip to the trigger.
    state.outside = event => {
        if (!canvas.querySelector('[data-flare-dock-actions]')) return;
        const target = event.target;
        if (!(target instanceof Element)) return;
        if (target.closest('[data-flare-dock-actions], [data-flare-dock-actions-toggle]')) return;
        callback.invokeMethodAsync('CloseDockActions').catch(() => {});
    };
    document.addEventListener('pointerdown', state.outside, true);
    syncHeaders(canvas, state);
    workspaces.set(canvas, state);
}

export function unregister(canvas) {
    const state = canvas && workspaces.get(canvas);
    if (!state) return;
    state.resize.disconnect();
    state.headerResize?.disconnect();
    state.headerStyle?.disconnect();
    state.tabFit?.dispose();
    for (const observer of state.direction) observer.disconnect();
    document.removeEventListener('pointerdown', state.outside, true);
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

export function focusActions(canvas) {
    const first = canvas?.querySelector('[data-flare-dock-actions] button');
    if (first) first.focus();
}

export function focusElement(canvas, id) {
    const element = document.getElementById(id);
    if (element && canvas?.contains(element)) element.focus();
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
