// Flare unified pointer-drag module.
//
// One gesture primitive (startDrag) plus the component behaviours that build on it: the resizable
// handle, the sibling splitter, and the colour-picker canvas. Before this module each of these
// re-implemented the same pointerdown -> track delta -> apply -> notify .NET skeleton (with subtly
// different mouse-vs-pointer and capture handling). Centralising it here means one touch-capable,
// pointer-captured implementation. Pure geometry helpers used by drag-and-drop reorder
// (tree drop-zone, kanban column hit-test) live here too, next to the gestures they serve.

// -- Gesture primitive --------------------------------------------------------
// Attach a drag gesture to `handle`. On press it captures the pointer (so tracking continues even
// when the cursor leaves the element and works for mouse/pen/touch alike) and reports the delta from
// the press point. Returns an unsubscribe function.
//   opts: { onStart(e)?, onMove(dx, dy, e)?, onEnd(e)?, cursor?: string, button?: number }
// `cursor` (when set) is applied to <body> for the duration; `button` (default 0) filters mouse button.
export function startDrag(handle, opts) {
    if (!handle) return () => { };
    const o = opts || {};
    const button = o.button ?? 0;
    let active = false, startX = 0, startY = 0, pid = null, prevCursor = '', prevSelect = '';

    // A drag handle should not also pan/scroll the page on touch.
    handle.style.touchAction = 'none';

    function down(e) {
        if (e.pointerType === 'mouse' && e.button !== button) return;
        active = true; startX = e.clientX; startY = e.clientY; pid = e.pointerId;
        try { handle.setPointerCapture(e.pointerId); } catch (_) { }
        if (o.cursor) { prevCursor = document.body.style.cursor; document.body.style.cursor = o.cursor; }
        prevSelect = document.body.style.userSelect; document.body.style.userSelect = 'none';
        o.onStart && o.onStart(e);
        e.preventDefault();
    }
    function move(e) {
        if (!active || e.pointerId !== pid) return;
        o.onMove && o.onMove(e.clientX - startX, e.clientY - startY, e);
        e.preventDefault();
    }
    function end(e) {
        if (!active || e.pointerId !== pid) return;
        active = false;
        try { handle.releasePointerCapture(pid); } catch (_) { }
        if (o.cursor) document.body.style.cursor = prevCursor;
        document.body.style.userSelect = prevSelect;
        o.onEnd && o.onEnd(e);
    }

    handle.addEventListener('pointerdown', down);
    handle.addEventListener('pointermove', move);
    handle.addEventListener('pointerup', end);
    handle.addEventListener('pointercancel', end);
    return () => {
        handle.removeEventListener('pointerdown', down);
        handle.removeEventListener('pointermove', move);
        handle.removeEventListener('pointerup', end);
        handle.removeEventListener('pointercancel', end);
    };
}

// -- FlareResizable (drag one edge to resize a single container) --------------
const _resizeHandles = new Map();

export function registerResizeHandle(container, handle, edge, minSize, maxSize, dotNetRef) {
    if (!handle || !container) return;
    const horiz = edge === 'right' || edge === 'left';
    const dim = horiz ? 'width' : 'height';
    let startSize = 0;
    const off = startDrag(handle, {
        onStart() {
            const rect = container.getBoundingClientRect();
            startSize = horiz ? rect.width : rect.height;
        },
        onMove(dx, dy) {
            const along = horiz ? dx : dy;
            const delta = edge === 'right' || edge === 'bottom' ? along : -along;
            let newSize = Math.max(0, startSize + delta);
            if (minSize) newSize = Math.max(parseFloat(minSize), newSize);
            if (maxSize) newSize = Math.min(parseFloat(maxSize), newSize);
            container.style[dim] = newSize + 'px';
        },
        onEnd() {
            if (dotNetRef) dotNetRef.invokeMethodAsync('OnResizedCallback', container.style[dim]);
        },
    });
    _resizeHandles.set(handle, off);
}

export function removeResizeHandle(handle) {
    const off = _resizeHandles.get(handle);
    if (off) { off(); _resizeHandles.delete(handle); }
}

// -- FlareDialog drag (move the panel by its header) + corner resize ----------
// Both reuse the shared startDrag gesture so there is no bespoke pointer plumbing here. Drag applies a
// CSS translate (preserving the centering transform's starting offset); resize grows width/height from
// the bottom-right gripper down to the given minimums.
const _dialogDrags = new Map();

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
    _dialogDrags.set(handle, off);
}

export function removeDialogDrag(handle) {
    const off = _dialogDrags.get(handle);
    if (off) { off(); _dialogDrags.delete(handle); }
}

const _dialogResizes = new Map();

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
        },
        onMove(dx, dy) {
            panel.style.width = Math.max(minW, sw + dx) + 'px';
            panel.style.height = Math.max(minH, sh + dy) + 'px';
        },
    });
    _dialogResizes.set(handle, off);
}

export function removeDialogResize(handle) {
    const off = _dialogResizes.get(handle);
    if (off) { off(); _dialogResizes.delete(handle); }
}

// -- FlareSplitter (handle that resizes its two flex siblings) ----------------
const _splitters = new Map();

function _flareParsePx(v) {
    if (!v) return 0;
    const n = parseFloat(String(v));
    return isNaN(n) ? 0 : n;
}

// Resolve the resize axis: explicit 'horizontal'/'vertical', or 'auto' from the parent flex direction.
function _flareSplitterHoriz(gutter, orientation) {
    if (orientation === 'horizontal') return true;
    if (orientation === 'vertical') return false;
    const dir = gutter.parentElement ? getComputedStyle(gutter.parentElement).flexDirection : 'row';
    return dir !== 'column' && dir !== 'column-reverse';
}

// Reflect the resolved axis on the element so the CSS handle (size/cursor) and aria are correct.
function _flareSplitterReflect(gutter, horiz) {
    gutter.classList.toggle('flare-splitter--vertical', !horiz);
    gutter.classList.toggle('flare-splitter--horizontal', horiz);
    gutter.setAttribute('aria-orientation', horiz ? 'vertical' : 'horizontal');
}

// Apply new sizes to the previous/next siblings, keeping their combined size constant.
function _flareApplySiblingSize(prev, next, horiz, newPrev, total, minPx, maxPx, dotNetRef) {
    const min = minPx || 0;
    let p = Math.max(min, Math.min(newPrev, total - min));
    if (maxPx) p = Math.min(p, maxPx);
    const n = total - p;
    const dim = horiz ? 'width' : 'height';
    prev.style.flex = '0 0 ' + p + 'px';
    prev.style[dim] = p + 'px';
    next.style.flex = '0 0 ' + n + 'px';
    next.style[dim] = n + 'px';
    if (dotNetRef) dotNetRef.invokeMethodAsync('OnSplitChanged', Math.round(p) + 'px');
}

export function registerSiblingSplitter(gutter, orientation, minSize, maxSize, dotNetRef) {
    if (!gutter) return;
    // Reflect the (possibly auto-detected) axis on the element so the handle renders correctly.
    if (orientation === 'auto') _flareSplitterReflect(gutter, _flareSplitterHoriz(gutter, 'auto'));

    let horiz = true, prev = null, next = null, prevSize = 0, total = 0;
    const off = startDrag(gutter, {
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
            _flareApplySiblingSize(prev, next, horiz, prevSize + along, total,
                _flareParsePx(minSize), _flareParsePx(maxSize), dotNetRef);
        },
        onEnd() { document.body.style.cursor = ''; },
    });
    _splitters.set(gutter, off);
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
    _flareApplySiblingSize(prev, next, horiz, prevSize + deltaPx, total,
        _flareParsePx(minSize), _flareParsePx(maxSize), dotNetRef);
}

export function removeSiblingSplitter(gutter) {
    const off = _splitters.get(gutter);
    if (off) { off(); _splitters.delete(gutter); }
}

// -- FlareColorPicker (drag the saturation/lightness canvas) ------------------
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

// -- Drag-and-drop reorder geometry helpers ----------------------------------
// These serve native HTML5 drag-and-drop (driven in C#), which reports the cursor position but not
// the target's bounds -- so the "which zone / which column" decision must be made here.

// FlareTreeItem: given a row element and the cursor's clientY, return which third of the row the
// cursor is over: top ~25% -> 'before', bottom ~25% -> 'after', middle -> 'inside'.
export function getDropZone(rowEl, clientY) {
    if (!rowEl) return 'inside';
    const rect = rowEl.getBoundingClientRect();
    if (rect.height <= 0) return 'inside';
    const offset = clientY - rect.top;
    if (offset < rect.height * 0.25) return 'before';
    if (offset > rect.height * 0.75) return 'after';
    return 'inside';
}

// FlareKanban touch drop: native DnD does not fire on touch, so the board hit-tests the column under
// the lifted finger itself. Returns the data-kanban-column id at the given point, or null.
export function getKanbanColumnAtPoint(x, y) {
    const el = document.elementFromPoint(x, y);
    return el ? (el.closest('[data-kanban-column]')?.dataset?.kanbanColumn ?? null) : null;
}
