// Browser-only geometry and input. .NET owns the draft and validates every proposed value.
const roots = new WeakMap();

function bind(root, receiver, generation) {
    const controller = new AbortController();
    const states = new Map();
    let disposed = false;
    const locked = () => root.dataset.timeLocked === 'true' || !root.isConnected;
    const cells = col => [...col.querySelectorAll('[data-time-value]')];
    const enabled = col => cells(col).filter(c => !c.disabled);
    const center = (col, cell) => {
        if (!cell) return;
        const r = col.getBoundingClientRect(), c = cell.getBoundingClientRect();
        const target = col.scrollTop + c.top + c.height / 2 - r.top - col.clientHeight / 2;
        states.get(col).programmatic = target;
        col.scrollTop = target;
    };
    const nearest = col => {
        const mid = col.getBoundingClientRect().top + col.clientHeight / 2;
        return enabled(col).reduce((best, cell) => {
            const r = cell.getBoundingClientRect();
            const distance = Math.abs(r.top + r.height / 2 - mid);
            return !best || distance < best.distance ? { cell, distance } : best;
        }, null)?.cell;
    };
    const send = (col, state, cell) => {
        if (!cell || disposed || locked()) return;
        const value = Number(cell.dataset.timeValue);
        state.current = value;
        // Serialize callbacks so a server-hosted picker cannot apply older input after newer input.
        state.pending++;
        state.queue = state.queue.then(() => {
            if (!disposed && !locked())
                return receiver.invokeMethodAsync('SelectColumn', Number(col.dataset.timeColumn), value, generation);
        }).catch(() => { }).finally(() => {
            state.pending--;
            if (!disposed && !state.pending && !state.touching && !state.timer) update();
        });
    };
    const settle = (col, state) => {
        clearTimeout(state.timer);
        if (state.touching || disposed || locked()) return;
        const cell = nearest(col);
        if (cell && Number(cell.dataset.timeValue) !== state.current) send(col, state, cell);
        state.userScroll = false;
        center(col, cell);
    };
    const update = () => {
        for (const [col, state] of states) {
            const first = cells(col)[0];
            if (!first) continue;
            const pad = Math.max(0, (col.clientHeight - first.getBoundingClientRect().height) / 2);
            col.style.paddingTop = `${pad}px`;
            col.style.paddingBottom = `${pad}px`;
            if (state.touching || state.pending || state.timer) continue;
            const selected = col.querySelector('[aria-selected="true"]');
            state.current = Number(col.dataset.timeCurrent);
            center(col, selected);
        }
    };
    const observer = new ResizeObserver(update);
    root.addEventListener('keydown', e => {
        if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(e.key)) e.preventDefault();
    }, { signal: controller.signal });
    for (const col of root.querySelectorAll('[data-time-column]')) {
        const state = { current: null, timer: 0, touching: false, delta: 0, lastWheel: 0, queue: Promise.resolve(), pending: 0 };
        states.set(col, state);
        const on = (name, fn, options = {}) => col.addEventListener(name, fn, { ...options, signal: controller.signal });
        on('wheel', e => {
            if (locked() || e.ctrlKey || !e.deltaY || Math.abs(e.deltaX) > Math.abs(e.deltaY)) return;
            const options = enabled(col);
            if (!options.length) return;
            e.preventDefault();
            clearTimeout(state.timer); state.timer = 0;
            state.userScroll = false;
            const now = performance.now();
            if (now - state.lastWheel > 200 || Math.sign(state.delta) !== Math.sign(e.deltaY)) state.delta = 0;
            state.lastWheel = now;
            const pitch = Math.max(1, options[0].getBoundingClientRect().height);
            state.delta += e.deltaMode ? Math.sign(e.deltaY) * pitch : e.deltaY;
            const steps = Math.abs(state.delta) >= pitch ? Math.sign(state.delta) : 0;
            if (!steps) return;
            state.delta %= pitch;
            let index = steps > 0 ? options.findIndex(c => Number(c.dataset.timeValue) > state.current)
                : options.findLastIndex(c => Number(c.dataset.timeValue) < state.current);
            if (index < 0) index = steps > 0 ? options.length - 1 : 0;
            const cell = options[index];
            if (Number(cell.dataset.timeValue) !== state.current) send(col, state, cell);
            center(col, cell);
        }, { passive: false });
        on('scroll', () => {
            if (!state.userScroll) return; // Layout, focus and a neighbour's constraints are not a gesture.
            if (Math.abs(col.scrollTop - state.programmatic) < 1) return;
            clearTimeout(state.timer);
            state.timer = setTimeout(() => { state.timer = 0; settle(col, state); }, 160);
        }, { passive: true });
        on('touchstart', () => {
            state.touching = true;
            state.userScroll = true;
            state.delta = 0;
            clearTimeout(state.timer); state.timer = 0;
        }, { passive: true });
        const end = () => {
            state.touching = false;
            clearTimeout(state.timer);
            state.timer = setTimeout(() => { state.timer = 0; settle(col, state); }, 160);
        };
        on('touchend', end, { passive: true });
        on('touchcancel', end, { passive: true });
        on('click', () => { clearTimeout(state.timer); state.timer = 0; state.userScroll = false; });
        observer.observe(col);
    }
    update();
    return {
        generation, update,
        release() {
            disposed = true;
            controller.abort(); observer.disconnect();
            for (const [col, state] of states) {
                clearTimeout(state.timer);
                col.style.removeProperty('padding-top'); col.style.removeProperty('padding-bottom');
            }
        }
    };
}

export const flareTimeColumns = {
    sync(root, receiver, generation) {
        if (!root) return;
        const column = root.querySelector('[data-time-column]');
        if (!column || !['auto', 'scroll'].includes(getComputedStyle(column).overflowY)) {
            flareTimeColumns.release(root); // Without CSS, retain ordinary clickable columns and avoid resize feedback.
            return;
        }
        let binding = roots.get(root);
        if (binding && binding.generation !== generation) { binding.release(); binding = null; }
        if (!binding) { binding = bind(root, receiver, generation); roots.set(root, binding); }
        else binding.update();
    },
    release(root) {
        const binding = roots.get(root);
        if (binding) { binding.release(); roots.delete(root); }
    }
};
