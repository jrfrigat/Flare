// Kanban pointer-event drag support for touch and mouse
const state = { dragging: null, dotNetRef: null, startX: 0, startY: 0 };

export function init(dotNetRef) {
    state.dotNetRef = dotNetRef;
}

export function enableCard(el, cardId, columnId) {
    el.setAttribute('data-card-id', cardId);
    el.setAttribute('data-column-id', columnId);

    el.addEventListener('pointerdown', (e) => {
        if (e.button !== 0 && e.pointerType === 'mouse') return;
        state.dragging = { el, cardId, columnId, origTransform: el.style.transform };
        state.startX = e.clientX;
        state.startY = e.clientY;
        try { el.setPointerCapture(e.pointerId); } catch (_) {}
        el.classList.add('flare-kanban__card--dragging');
        e.preventDefault();
    });

    el.addEventListener('pointermove', (e) => {
        if (!state.dragging || state.dragging.el !== el) return;
        el.style.transform = `translate(${e.clientX - state.startX}px,${e.clientY - state.startY}px)`;
        e.preventDefault();
    });

    el.addEventListener('pointerup', async (e) => {
        if (!state.dragging || state.dragging.el !== el) return;
        el.classList.remove('flare-kanban__card--dragging');
        el.style.transform = state.dragging.origTransform || '';

        el.style.pointerEvents = 'none';
        const under = document.elementFromPoint(e.clientX, e.clientY);
        el.style.pointerEvents = '';

        const col = under?.closest('[data-kanban-column]');
        const targetCol = col?.dataset.kanbanColumn;
        if (targetCol && targetCol !== state.dragging.columnId && state.dotNetRef) {
            await state.dotNetRef.invokeMethodAsync('OnCardDropped', state.dragging.cardId, targetCol);
        }
        state.dragging = null;
    });

    el.addEventListener('pointercancel', () => {
        if (!state.dragging || state.dragging.el !== el) return;
        el.classList.remove('flare-kanban__card--dragging');
        el.style.transform = state.dragging.origTransform || '';
        state.dragging = null;
    });
}

export function destroy() {
    state.dotNetRef = null;
    state.dragging = null;
}
