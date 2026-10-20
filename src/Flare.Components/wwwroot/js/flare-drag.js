// Flare unified pointer-drag module.
//
// One gesture primitive (startDrag) plus the component behaviours that build on it: the resizable
// handle, the sibling splitter, and the colour-picker canvas. Before this module each of these
// re-implemented the same pointerdown -> track delta -> apply -> notify .NET skeleton (with subtly
// different mouse-vs-pointer and capture handling). Centralising it here means one touch-capable,
// pointer-captured implementation. The drag-and-drop TRANSFER layer that rides on startDrag lives in
// flare-dragdrop.js.

import { registry } from './flare-dom.js';

// Attach a drag gesture to `handle`. On press it captures the pointer (so tracking continues even
// when the cursor leaves the element and works for mouse/pen/touch alike) and reports the delta from
// the press point. Returns an unsubscribe function.
//   opts: { onStart(e)?, onMove(dx, dy, e)?, onEnd(e)?, cursor?: string, button?: number,
//           filter?(e): boolean, threshold?: number, touchAction?: string | null,
//           trackWindow?: boolean, onCancel?(e) }
// `cursor` (when set) is applied to <body> for the duration; `button` (default 0) filters mouse button.
// `filter` decides whether a press starts a gesture at all - the drag-and-drop layer binds ONE gesture
// to a container and uses it to accept only presses that landed on a draggable descendant.
// `threshold` (px, default 0) delays the gesture until the pointer has actually travelled: below it the
// press is left alone, so a click on a draggable is still a click. A gesture with a threshold neither
// preventDefaults nor captures the press, for the same reason.
// `touchAction` is written to the handle so a drag does not also pan the page; pass null when the
// handle is a container whose scrolling must survive and the draggable children declare it themselves.
export function startDrag(handle, opts) {
    if (!handle) return () => { };
    const o = opts || {};
    const button = o.button ?? 0;
    const threshold = o.threshold ?? 0;
    const track = o.trackWindow ? window : handle;
    let armed = false, active = false, startX = 0, startY = 0, pid = null, prevCursor = '', prevSelect = '';
    let downEvent = null;

    // A drag handle should not also pan/scroll the page on touch.
    if (o.touchAction !== null) handle.style.touchAction = o.touchAction || 'none';

    // onStart is handed the PRESS, never the move that crossed the threshold. The move's `target` is
    // whatever the pointer has travelled onto (or the handle, once captured), not what was pressed, so a
    // thresholded gesture that read it in onStart would pick up the wrong element or none. The press's
    // clientX/clientY are the press point too, which is what a grab offset is measured from.
    function begin() {
        active = true;
        // A thresholded gesture captures only now. Captured on the press, the release is retargeted to
        // the handle and the browser sends the click to it as well, so the pressed element never gets
        // its click - the very thing the threshold exists to keep.
        if (threshold) { try { handle.setPointerCapture(pid); } catch (_) { } }
        if (o.cursor) { prevCursor = document.body.style.cursor; document.body.style.cursor = o.cursor; }
        prevSelect = document.body.style.userSelect; document.body.style.userSelect = 'none';
        o.onStart && o.onStart(downEvent);
    }
    function down(e) {
        if (e.pointerType === 'mouse' && e.button !== button) return;
        if (o.filter && !o.filter(e)) return;
        armed = true; startX = e.clientX; startY = e.clientY; pid = e.pointerId; downEvent = e;
        if (threshold) return;   // the press stays a press until the pointer travels
        try { handle.setPointerCapture(e.pointerId); } catch (_) { }
        begin();
        e.preventDefault();
    }
    function move(e) {
        if (!armed || e.pointerId !== pid) return;
        const dx = e.clientX - startX, dy = e.clientY - startY;
        if (!active) {
            // Uncaptured, a release outside the handle never reaches `end`; a pointer that moves with no
            // button held has already let go, and must not start a drag nobody is holding. (A touch in
            // contact always reports a button.)
            if (e.buttons === 0) { armed = false; downEvent = null; return; }
            if (Math.abs(dx) < threshold && Math.abs(dy) < threshold) return;
            begin();
        }
        o.onMove && o.onMove(dx, dy, e);
        e.preventDefault();
    }
    function end(e) {
        if (!armed || e.pointerId !== pid) return;
        armed = false;
        downEvent = null;
        try { handle.releasePointerCapture(pid); } catch (_) { }
        if (!active) return;     // never passed the threshold: nothing was started, nothing to end
        active = false;
        if (o.cursor) document.body.style.cursor = prevCursor;
        document.body.style.userSelect = prevSelect;
        o.onEnd && o.onEnd(e);
    }

    function cancel(e) {
        if (!o.onCancel) { end(e); return; }
        if (!armed || e.pointerId !== pid) return;
        armed = false;
        downEvent = null;
        try { handle.releasePointerCapture(pid); } catch (_) { }
        if (!active) return;
        active = false;
        if (o.cursor) document.body.style.cursor = prevCursor;
        document.body.style.userSelect = prevSelect;
        o.onCancel(e);
    }

    handle.addEventListener('pointerdown', down);
    track.addEventListener('pointermove', move);
    track.addEventListener('pointerup', end);
    track.addEventListener('pointercancel', cancel);
    if (o.onCancel) handle.addEventListener('lostpointercapture', cancel);
    return () => {
        handle.removeEventListener('pointerdown', down);
        track.removeEventListener('pointermove', move);
        track.removeEventListener('pointerup', end);
        track.removeEventListener('pointercancel', cancel);
        if (o.onCancel) {
            handle.removeEventListener('lostpointercapture', cancel);
            cancel({ pointerId: pid });
        }
    };
}

const _resizeHandles = registry();

export function registerResizeHandle(container, handle, edge, minSize, maxSize, dotNetRef) {
    if (!handle || !container) return;
    const horiz = edge === 'right' || edge === 'left';
    const dim = horiz ? 'width' : 'height';
    const buttons = Array.from(container.querySelectorAll(':scope > [data-flare-resize-controls] > button'));
    const controls = buttons[0]?.parentElement;
    const footprint = () => {
        const grip = handle.getBoundingClientRect();
        const actions = controls?.getBoundingClientRect();
        const inset = controls ? parseFloat(getComputedStyle(controls)[horiz ? 'insetInlineEnd' : 'insetBlockEnd']) || 0 : 0;
        return Math.max(horiz ? grip.width : grip.height,
            actions ? (horiz ? actions.width : actions.height) + 2 * inset : 0);
    };
    let startSize = 0;
    const size = () => {
        const rect = container.getBoundingClientRect();
        return horiz ? rect.width : rect.height;
    };
    // A limit is a CSS length, not a number. parseFloat turned "10rem" into 10px and "50%" into 50px;
    // % is resolved against the parent box - the space the container can actually occupy - and the
    // font-relative and viewport units against what the browser computes for them. Anything that is
    // not a supported unit is ignored instead of being read as pixels.
    const lengthPx = (raw, fallback) => {
        if (raw == null || raw === '') return fallback;
        const text = String(raw).trim();
        if (!text) return fallback;
        const n = parseFloat(text);
        if (!Number.isFinite(n) || n < 0) return fallback;
        if (text.endsWith('%')) {
            const parent = container.parentElement?.getBoundingClientRect();
            const basis = parent ? (horiz ? parent.width : parent.height) : NaN;
            return Number.isFinite(basis) ? basis * n / 100 : fallback;
        }
        // 'rem' has to be tested before 'em', which it also ends with.
        const units = [
            ['px', 1],
            ['rem', parseFloat(getComputedStyle(document.documentElement).fontSize)],
            ['em', parseFloat(getComputedStyle(container).fontSize)],
            ['vw', window.innerWidth / 100],
            ['vh', window.innerHeight / 100],
        ];
        for (const [unit, base] of units) {
            if (!text.endsWith(unit)) continue;
            return Number.isFinite(base) ? n * base : fallback;
        }
        return fallback;
    };
    const bounds = () => {
        const parent = container.parentElement?.getBoundingClientRect();
        const available = parent ? (horiz ? parent.width : parent.height) : size();
        const min = Math.max(0, footprint(), lengthPx(minSize, 0));
        const configuredMax = lengthPx(maxSize, NaN);
        // An inverted range is raised to its minimum rather than left as min > max; without a limit the
        // current size and the parent's room are the ceiling the handle may reach.
        const max = Number.isFinite(configuredMax)
            ? Math.max(min, configuredMax)
            : Math.max(min, size(), available);
        return { min, max: Math.max(min, max) };
    };
    const reflect = value => {
        const { min, max } = bounds();
        handle.setAttribute('aria-orientation', horiz ? 'vertical' : 'horizontal');
        handle.setAttribute('aria-valuemin', String(Math.round(min)));
        handle.setAttribute('aria-valuemax', String(Math.round(max)));
        handle.setAttribute('aria-valuenow', String(Math.round(value)));
        handle.setAttribute('aria-valuetext', Math.round(value) + 'px');
        for (const button of buttons) button.disabled = Number(button.dataset.flareResizeStep) < 0 ? value <= min : value >= max;
    };
    const apply = value => {
        const { min, max } = bounds();
        const clamped = Math.max(min, Math.min(value, max));
        container.style[dim] = clamped + 'px';
        reflect(clamped);
    };
    const notify = () => {
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnResizedCallback', container.style[dim]).catch(() => { });
    };
    const keyboard = e => {
        if (e.target !== handle) return;
        const positive = horiz ? 'ArrowRight' : 'ArrowDown';
        const negative = horiz ? 'ArrowLeft' : 'ArrowUp';
        const sign = edge === 'left' || edge === 'top' ? -1 : 1;
        if (e.key === 'Home') apply(bounds().min);
        else if (e.key === 'End') apply(bounds().max);
        else if (e.key === positive) apply(size() + sign * 16);
        else if (e.key === negative) apply(size() - sign * 16);
        else return;
        e.preventDefault();
        notify();
    };
    const click = e => {
        const button = e.currentTarget;
        apply(size() + Number(button.dataset.flareResizeStep) * 16);
        e.preventDefault();
        notify();
    };
    handle.addEventListener('keydown', keyboard);
    for (const button of buttons) button.addEventListener('click', click);
    reflect(size());
    const off = startDrag(handle, {
        onStart() {
            startSize = size();
        },
        onMove(dx, dy) {
            const along = horiz ? dx : dy;
            const delta = edge === 'right' || edge === 'bottom' ? along : -along;
            apply(startSize + delta);
        },
        onEnd: notify,
    });
    _resizeHandles.keep(handle, () => {
        off();
        handle.removeEventListener('keydown', keyboard);
        for (const button of buttons) button.removeEventListener('click', click);
    });
}

export function removeResizeHandle(handle) {
    _resizeHandles.drop(handle);
}

// Both reuse the shared startDrag gesture so there is no bespoke pointer plumbing here. Drag applies a
// CSS translate (preserving the centering transform's starting offset); resize grows width/height from
// the bottom-right gripper down to the given minimums.
const _dialogDrags = registry();

export function registerDialogDrag(handle, panel) {
    if (!handle || !panel) return;
    let startX = 0, startY = 0;
    const off = startDrag(handle, {
        cursor: 'move',
        onStart() {
            const m = new DOMMatrixReadOnly(getComputedStyle(panel).transform);
            startX = m.m41;
            startY = m.m42;
        },
        onMove(dx, dy) {
            panel.style.transform = `translate(${startX + dx}px, ${startY + dy}px)`;
        },
    });
    _dialogDrags.keep(handle, off);
}

export function removeDialogDrag(handle) {
    _dialogDrags.drop(handle);
}

const _dialogResizes = registry();

export function registerDialogResize(handle, panel, minWidth, minHeight) {
    if (!handle || !panel) return;
    const minW = parseFloat(minWidth) || 0;
    const minH = parseFloat(minHeight) || 0;
    let sw = 0, sh = 0;
    const off = startDrag(handle, {
        cursor: 'nwse-resize',
        onStart() {
            const r = panel.getBoundingClientRect();
            sw = r.width;
            sh = r.height;
            // The size preset caps max-width/height; lift the caps so the dragged size takes effect
            // (only once the user actually resizes, so the initial size is untouched).
            panel.style.maxWidth = 'none';
            panel.style.maxHeight = 'none';
        },
        onMove(dx, dy) {
            panel.style.width = Math.max(minW, sw + dx) + 'px';
            panel.style.height = Math.max(minH, sh + dy) + 'px';
        },
    });
    _dialogResizes.keep(handle, off);
}

export function removeDialogResize(handle) {
    _dialogResizes.drop(handle);
}

const _splitters = registry();

function _flareParsePx(v) {
    if (!v) return 0;
    const n = parseFloat(String(v));
    return isNaN(n) ? 0 : n;
}

// The orientation travels on the element as well as into the registration, so a parameter change
// reaches an open page without a second registration and cannot leave a stale closure behind.
const _flareSplitterAxis = (gutter, orientation) => gutter.dataset.flareSplitterOrientation || orientation;

// Resolve the resize axis: explicit 'horizontal'/'vertical', or 'auto' from the parent flex direction.
function _flareSplitterHoriz(gutter, orientation) {
    orientation = _flareSplitterAxis(gutter, orientation);
    if (orientation === 'horizontal') return true;
    if (orientation === 'vertical') return false;
    const dir = gutter.parentElement ? getComputedStyle(gutter.parentElement).flexDirection : 'row';
    return dir !== 'column' && dir !== 'column-reverse';
}

// +1 when the previous sibling sits on the visually leading side of the gutter, so a positive arrow
// or drag grows it; -1 in RTL or a reversed flex direction, where that sibling is on the trailing
// side and the same pointer motion has to shrink it.
function _flareVisualSign(gutter, horiz) {
    const parent = gutter.parentElement;
    if (!parent) return 1;
    const style = getComputedStyle(parent);
    if (horiz) return (style.flexDirection === 'row-reverse') !== (style.direction === 'rtl') ? -1 : 1;
    return style.flexDirection === 'column-reverse' ? -1 : 1;
}

// Keeps the class/aria axis in step with the layout before the user interacts: a parent that becomes
// column (a class change, a media query, a resize) or flips its writing direction has to move the
// grip and the arrow meaning with it.
function _flareWatchAxis(gutter, orientation, teardown) {
    const reflect = () => _flareSplitterReflect(gutter, _flareSplitterHoriz(gutter, orientation));
    const observers = [];
    const watch = (el, filter) => {
        if (!el || typeof MutationObserver === 'undefined') return;
        const observer = new MutationObserver(reflect);
        observer.observe(el, { attributes: true, attributeFilter: filter });
        observers.push(observer);
    };
    watch(gutter, ['data-flare-splitter-orientation']);
    watch(gutter.parentElement, ['style', 'class', 'dir']);
    watch(document.documentElement, ['style', 'class', 'dir']);
    window.addEventListener('resize', reflect, { passive: true });
    return () => {
        for (const observer of observers) observer.disconnect();
        window.removeEventListener('resize', reflect);
        teardown();
    };
}

// Reflect the resolved axis on the element so the CSS handle (size/cursor) and aria are correct.
function _flareSplitterReflect(gutter, horiz) {
    gutter.classList.toggle('flare-splitter--vertical', !horiz);
    gutter.classList.toggle('flare-splitter--horizontal', horiz);
    gutter.setAttribute('aria-orientation', horiz ? 'vertical' : 'horizontal');
}

// Apply new sizes to the previous/next siblings, keeping their combined size constant.
function _flareSplitterRange(gutter, value, total, minPx, maxPx) {
    const min = Math.min(Math.max(0, minPx || 0), total / 2);
    const max = Math.max(min, Math.min(maxPx || total, total - min));
    gutter.setAttribute('aria-valuemin', String(Math.round(min)));
    gutter.setAttribute('aria-valuemax', String(Math.round(max)));
    gutter.setAttribute('aria-valuenow', String(Math.round(value)));
    gutter.setAttribute('aria-valuetext', Math.round(value) + 'px');
    return { min, max };
}
function _flareApplySiblingSize(gutter, prev, next, horiz, newPrev, total, minPx, maxPx, dotNetRef) {
    const { min, max } = _flareSplitterRange(gutter, newPrev, total, minPx, maxPx);
    const p = Math.max(min, Math.min(newPrev, max));
    const n = total - p;
    const dim = horiz ? 'width' : 'height';
    prev.style.flex = '0 0 ' + p + 'px';
    prev.style[dim] = p + 'px';
    next.style.flex = '0 0 ' + n + 'px';
    next.style[dim] = n + 'px';
    _flareSplitterRange(gutter, p, total, minPx, maxPx);
    if (dotNetRef) dotNetRef.invokeMethodAsync('OnSplitChanged', Math.round(p) + 'px').catch(() => { });
}

// One length instead of a split between two neighbours (FlareSplitter.Value): the size lives in a CSS custom
// property on an ancestor, which every element that depends on it reads. The pointer moves it in the browser;
// .NET hears the new size once, when the drag, the key or the step is over.
function registerValueSplitter(gutter, orientation, dotNetRef) {
    let horiz = _flareSplitterHoriz(gutter, orientation);
    _flareSplitterReflect(gutter, horiz);
    // Re-read on every use: Orientation, the parent flex direction and RTL can all change after mount.
    const axis = () => (horiz = _flareSplitterHoriz(gutter, orientation));
    const sign = () => _flareVisualSign(gutter, horiz);
    const d = gutter.dataset;
    const scope = () => (d.flareSplitterScope && gutter.closest(d.flareSplitterScope)) || gutter.parentElement;
    const min = () => _flareParsePx(d.flareSplitterMin);
    const max = () => _flareParsePx(d.flareSplitterMax) || Infinity;
    const current = () => Number(d.flareSplitterValue) || 0;
    const clamp = v => Math.round(Math.max(min(), Math.min(v, max())));
    const step = () => Math.max(1, Number(d.flareSplitterKeyboardStep) || 16);
    const show = v => {
        scope()?.style.setProperty(d.flareSplitterVariable, v + 'px');
        gutter.setAttribute('aria-valuenow', String(v));
    };
    const commit = v => {
        show(v);
        d.flareSplitterValue = String(v);
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnSplitChanged', v + 'px').catch(() => { });
    };
    const reset = () => commit(clamp(d.flareSplitterDefault ? Number(d.flareSplitterDefault) : min()));
    const keyboard = e => {
        if (e.target !== gutter) return;
        axis();
        if (e.key === 'Home') reset();
        else if (e.key === 'End') commit(clamp(max() === Infinity ? current() : max()));
        else if (e.key === (horiz ? 'ArrowRight' : 'ArrowDown')) commit(clamp(current() + sign() * step()));
        else if (e.key === (horiz ? 'ArrowLeft' : 'ArrowUp')) commit(clamp(current() - sign() * step()));
        else return;
        e.preventDefault();
    };
    const click = e => {
        const target = e.target.closest('[data-flare-splitter-step]');
        if (!target || target.parentElement !== gutter) return;
        commit(clamp(current() + Number(target.dataset.flareSplitterStep) * step()));
        e.preventDefault();
    };
    const dblclick = e => { if (!e.target.closest('[data-flare-splitter-step]')) reset(); };
    gutter.addEventListener('keydown', keyboard);
    gutter.addEventListener('click', click);
    gutter.addEventListener('dblclick', dblclick);
    let start = 0, last = null;
    const off = startDrag(gutter, {
        filter: e => !e.target.closest('[data-flare-splitter-step]'),
        onStart() {
            axis();
            start = current();
            last = null;
            document.body.style.cursor = horiz ? 'col-resize' : 'row-resize';
        },
        onMove(dx, dy) {
            axis();
            last = clamp(start + sign() * (horiz ? dx : dy));
            show(last);
        },
        onEnd() { document.body.style.cursor = ''; if (last !== null && last !== start) commit(last); },
    });
    _splitters.keep(gutter, _flareWatchAxis(gutter, orientation, () => {
        off();
        gutter.removeEventListener('keydown', keyboard);
        gutter.removeEventListener('click', click);
        gutter.removeEventListener('dblclick', dblclick);
    }));
}

export function registerSiblingSplitter(gutter, orientation, minSize, maxSize, dotNetRef) {
    if (!gutter) return;
    if (gutter.dataset.flareSplitterVariable) { registerValueSplitter(gutter, orientation, dotNetRef); return; }
    // The limits are read when they are used, so new MinSize/MaxSize parameters apply without re-registering.
    const limit = (name, fallback) => gutter.dataset[name] ?? fallback;
    // Reflect the (possibly auto-detected) axis on the element so the handle renders correctly.
    _flareSplitterReflect(gutter, _flareSplitterHoriz(gutter, orientation));

    let horiz = true, prev = null, next = null, prevSize = 0, total = 0;
    const measure = () => {
        prev = gutter.previousElementSibling; next = gutter.nextElementSibling;
        if (!prev || !next) return false;
        horiz = _flareSplitterHoriz(gutter, orientation);
        _flareSplitterReflect(gutter, horiz);
        prevSize = horiz ? prev.getBoundingClientRect().width : prev.getBoundingClientRect().height;
        total = prevSize + (horiz ? next.getBoundingClientRect().width : next.getBoundingClientRect().height);
        return true;
    };
    const apply = value => _flareApplySiblingSize(gutter, prev, next, horiz, value, total,
        _flareParsePx(limit('flareSplitterMin', minSize)), _flareParsePx(limit('flareSplitterMax', maxSize)), dotNetRef);
    const step = () => Math.max(1, Number(gutter.dataset.flareSplitterKeyboardStep) || 16);
    const keyboard = e => {
        if (e.target !== gutter || !measure()) return;
        const sign = _flareVisualSign(gutter, horiz);
        if (e.key === 'Home') apply(0);
        else if (e.key === 'End') apply(total);
        else if (e.key === (horiz ? 'ArrowRight' : 'ArrowDown')) apply(prevSize + sign * step());
        else if (e.key === (horiz ? 'ArrowLeft' : 'ArrowUp')) apply(prevSize - sign * step());
        else return;
        e.preventDefault();
    };
    const click = e => {
        const target = e.target.closest('[data-flare-splitter-step]');
        if (!target || target.parentElement !== gutter || !measure()) return;
        apply(prevSize + Number(target.dataset.flareSplitterStep) * _flareVisualSign(gutter, horiz) * step());
        e.preventDefault();
    };
    gutter.addEventListener('keydown', keyboard); gutter.addEventListener('click', click);
    if (measure()) _flareSplitterRange(gutter, prevSize, total, _flareParsePx(limit('flareSplitterMin', minSize)), _flareParsePx(limit('flareSplitterMax', maxSize)));
    const off = startDrag(gutter, {
        filter: e => !e.target.closest('[data-flare-splitter-step]'),
        onStart() {
            prev = gutter.previousElementSibling;
            next = gutter.nextElementSibling;
            if (!prev || !next) return;
            horiz = _flareSplitterHoriz(gutter, orientation); // re-resolve in case the layout changed
            prevSize = horiz ? prev.getBoundingClientRect().width : prev.getBoundingClientRect().height;
            const nextSize = horiz ? next.getBoundingClientRect().width : next.getBoundingClientRect().height;
            total = prevSize + nextSize;
            document.body.style.cursor = horiz ? 'col-resize' : 'row-resize';
        },
        onMove(dx, dy) {
            if (!prev || !next) return;
            const along = horiz ? dx : dy;
            _flareApplySiblingSize(gutter, prev, next, horiz, prevSize + _flareVisualSign(gutter, horiz) * along, total,
                _flareParsePx(limit('flareSplitterMin', minSize)), _flareParsePx(limit('flareSplitterMax', maxSize)), dotNetRef);
        },
        onEnd() { document.body.style.cursor = ''; },
    });
    _splitters.keep(gutter, _flareWatchAxis(gutter, orientation, () => {
        off(); gutter.removeEventListener('keydown', keyboard); gutter.removeEventListener('click', click);
    }));
}

export function nudgeSiblingSplitter(gutter, orientation, deltaPx, keyAxis, minSize, maxSize, dotNetRef) {
    if (!gutter) return;
    const horiz = _flareSplitterHoriz(gutter, orientation);
    // Ignore arrow keys perpendicular to the resize axis.
    if ((horiz && keyAxis !== 'x') || (!horiz && keyAxis !== 'y')) return;
    const prev = gutter.previousElementSibling;
    const next = gutter.nextElementSibling;
    if (!prev || !next) return;
    const prevSize = horiz ? prev.getBoundingClientRect().width : prev.getBoundingClientRect().height;
    const nextSize = horiz ? next.getBoundingClientRect().width : next.getBoundingClientRect().height;
    const total = prevSize + nextSize;
    _flareApplySiblingSize(gutter, prev, next, horiz, prevSize + deltaPx, total,
        _flareParsePx(minSize), _flareParsePx(maxSize), dotNetRef);
}

export function removeSiblingSplitter(gutter) {
    _splitters.drop(gutter);
}

export const flareColorPicker = (() => {
    const _state = new Map(); // canvas el -> { dotNetRef, hue, sat, l, _cleanup }

    function _drawCanvas(canvas, hue) {
        const ctx = canvas.getContext('2d');
        const w = canvas.width, h = canvas.height;
        const hueColor = `hsl(${hue},100%,50%)`;
        const gradH = ctx.createLinearGradient(0, 0, w, 0);
        gradH.addColorStop(0, '#fff');
        gradH.addColorStop(1, hueColor);
        ctx.fillStyle = gradH;
        ctx.fillRect(0, 0, w, h);
        const gradV = ctx.createLinearGradient(0, 0, 0, h);
        gradV.addColorStop(0, 'rgba(0,0,0,0)');
        gradV.addColorStop(1, '#000');
        ctx.fillStyle = gradV;
        ctx.fillRect(0, 0, w, h);
    }

    function _drawCrosshair(canvas, sat, l) {
        const ctx = canvas.getContext('2d');
        const w = canvas.width, h = canvas.height;
        const x = (sat / 100) * w;
        const y = (1 - l / 100) * h;
        ctx.save();
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.strokeStyle = '#fff';
        ctx.lineWidth = 2;
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.strokeStyle = 'rgba(0,0,0,0.4)';
        ctx.lineWidth = 1;
        ctx.stroke();
        ctx.restore();
    }

    function _render(canvas) {
        const s = _state.get(canvas);
        if (!s) return;
        _drawCanvas(canvas, s.hue);
        _drawCrosshair(canvas, s.sat, s.l);
    }

    // Pointer events carry clientX/clientY directly (mouse, pen and touch), so no touch branching.
    function _pickFromEvent(canvas, e) {
        const rect = canvas.getBoundingClientRect();
        const x = Math.max(0, Math.min(e.clientX - rect.left, rect.width));
        const y = Math.max(0, Math.min(e.clientY - rect.top, rect.height));
        return { sat: (x / rect.width) * 100, l: (1 - y / rect.height) * 100 };
    }

    return {
        init(canvas, dotNetRef, hue, sat, l) {
            if (!canvas) return;
            const existing = _state.get(canvas);
            if (existing) {
                existing.dotNetRef = dotNetRef;
                existing.hue = hue;
                existing.sat = sat;
                existing.l = l;
                _render(canvas);
                return;
            }

            const state = { dotNetRef, hue, sat, l };
            _state.set(canvas, state);

            function pick(e) {
                const { sat: ns, l: nl } = _pickFromEvent(canvas, e);
                state.sat = ns; state.l = nl;
                _render(canvas);
                dotNetRef.invokeMethodAsync('OnCanvasPick', ns, nl).catch(() => { });
            }

            state._cleanup = startDrag(canvas, { onStart: pick, onMove: (dx, dy, e) => pick(e) });
            _render(canvas);
        },

        setHue(canvas, hue) {
            const s = _state.get(canvas);
            if (!s) return;
            s.hue = hue;
            _render(canvas);
        },

        setSatL(canvas, sat, l) {
            const s = _state.get(canvas);
            if (!s) return;
            s.sat = sat; s.l = l;
            _render(canvas);
        },

        destroy(canvas) {
            const s = _state.get(canvas);
            if (s) { s._cleanup?.(); _state.delete(canvas); }
        }
    };
})();

