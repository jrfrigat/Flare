const pads = new Map();

export function init(padId, canvasEl, dotNetRef) {
    const ctx = canvasEl.getContext('2d');
    const state = {
        ctx,
        canvasEl,
        dotNetRef,
        baseWidth: 2.0,
        pressureSensitive: true,
        drawing: false,
        lastX: 0,
        lastY: 0,
    };

    function getPos(e) {
        const rect = canvasEl.getBoundingClientRect();
        return { x: e.clientX - rect.left, y: e.clientY - rect.top };
    }

    // Pointer events unify mouse/pen/touch and expose `pressure` (0..1).
    // Mouse reports 0.5 while pressed; pens report true pressure.
    function effectiveWidth(e) {
        if (!state.pressureSensitive) return state.baseWidth;
        const p = e.pressure && e.pressure > 0 ? e.pressure : 0.5;
        return state.baseWidth * (0.4 + 1.2 * p);
    }

    function startDraw(e) {
        e.preventDefault();
        if (canvasEl.setPointerCapture) {
            try { canvasEl.setPointerCapture(e.pointerId); } catch { /* ignore */ }
        }
        state.drawing = true;
        const pos = getPos(e);
        state.lastX = pos.x;
        state.lastY = pos.y;
    }

    function draw(e) {
        if (!state.drawing) return;
        e.preventDefault();
        const pos = getPos(e);
        // Draw each segment separately so width can vary with pressure.
        ctx.beginPath();
        ctx.lineWidth = effectiveWidth(e);
        ctx.moveTo(state.lastX, state.lastY);
        ctx.lineTo(pos.x, pos.y);
        ctx.stroke();
        state.lastX = pos.x;
        state.lastY = pos.y;
    }

    function stopDraw(e) {
        if (!state.drawing) return;
        state.drawing = false;
        if (canvasEl.releasePointerCapture && e && e.pointerId !== undefined) {
            try { canvasEl.releasePointerCapture(e.pointerId); } catch { /* ignore */ }
        }
        dotNetRef.invokeMethodAsync('OnSignatureChanged');
    }

    state.handlers = { startDraw, draw, stopDraw };
    canvasEl.addEventListener('pointerdown', startDraw);
    canvasEl.addEventListener('pointermove', draw);
    canvasEl.addEventListener('pointerup', stopDraw);
    canvasEl.addEventListener('pointercancel', stopDraw);
    canvasEl.addEventListener('pointerleave', stopDraw);
    // Avoid touch scrolling while signing.
    canvasEl.style.touchAction = 'none';

    pads.set(padId, state);
}

export function clear(padId) {
    const pad = pads.get(padId);
    if (!pad) return;
    pad.ctx.clearRect(0, 0, pad.canvasEl.width, pad.canvasEl.height);
}

export function getDataUrl(padId) {
    const pad = pads.get(padId);
    return pad ? pad.canvasEl.toDataURL('image/png') : null;
}

export function setStrokeStyle(padId, color, lineWidth, pressureSensitive) {
    const pad = pads.get(padId);
    if (!pad) return;
    pad.ctx.strokeStyle = color;
    pad.baseWidth = lineWidth;
    pad.ctx.lineWidth = lineWidth;
    pad.ctx.lineCap = 'round';
    pad.ctx.lineJoin = 'round';
    if (pressureSensitive !== undefined && pressureSensitive !== null) {
        pad.pressureSensitive = pressureSensitive;
    }
}

export function destroy(padId) {
    const pad = pads.get(padId);
    if (pad && pad.handlers) {
        const { canvasEl, handlers } = pad;
        canvasEl.removeEventListener('pointerdown', handlers.startDraw);
        canvasEl.removeEventListener('pointermove', handlers.draw);
        canvasEl.removeEventListener('pointerup', handlers.stopDraw);
        canvasEl.removeEventListener('pointercancel', handlers.stopDraw);
        canvasEl.removeEventListener('pointerleave', handlers.stopDraw);
    }
    pads.delete(padId);
}
