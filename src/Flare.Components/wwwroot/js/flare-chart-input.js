// Browser-only focus mechanics. Data, geometry and tooltip contents stay in the component.
import { registry, listen, all } from './flare-dom.js';
import { startDrag } from './flare-drag.js';

const plots = registry();
const selector = '[data-flare-chart-hit]';

export function observePlot(plot, receiver) {
    if (!plot) return;
    let active = null, marks = [], byKey = new Map(), chosen = null, described = null, focusedKey = null;
    const hits = () => marks;
    const send = (method, ...args) => receiver.invokeMethodAsync(method, ...args).catch(() => { });
    const sync = () => {
        marks = [...plot.querySelectorAll(selector)];
        byKey = new Map(marks.map(el => [el.dataset.flareChartHit, el]));
        chosen = byKey.get(active) || marks[0];
        active = chosen?.dataset.flareChartHit || null;
        for (const mark of marks) {
            const index = mark === chosen ? 0 : -1;
            if (mark.tabIndex !== index) mark.tabIndex = index;
        }
    };
    const detail = () => {
        const key = plot.dataset.flareChartActive;
        const next = byKey.get(key) || null;
        if (next === described) return;
        described?.removeAttribute('aria-describedby');
        described = next;
        if (described) described.setAttribute('aria-describedby', plot.dataset.flareChartTooltip);
    };
    const focus = event => {
        const mark = event.target.closest?.(selector);
        if (!mark || !plot.contains(mark)) return;
        active = mark.dataset.flareChartHit;
        focusedKey = active;
        if (chosen !== mark) {
            if (chosen) chosen.tabIndex = -1;
            chosen = mark;
            chosen.tabIndex = 0;
        }
        const rect = mark.getBoundingClientRect();
        send('ChartHitFocused', active, rect.left + rect.width / 2, rect.top + rect.height / 2);
    };
    const key = event => {
        const mark = event.target.closest?.(selector);
        if (!mark || !plot.contains(mark) || event.altKey || event.ctrlKey || event.metaKey) return;
        if (event.key === 'Escape') { event.preventDefault(); send('ChartDetailDismissed'); return; }
        if (event.key === 'Enter' || event.key === ' ') {
            event.preventDefault();
            if (event.repeat) return;
            const rect = mark.getBoundingClientRect();
            mark.dispatchEvent(new MouseEvent('click', { bubbles: true, clientX: rect.left + rect.width / 2, clientY: rect.top + rect.height / 2 }));
            return;
        }
        const marks = hits();
        let index = marks.indexOf(mark);
        switch (event.key) {
            case 'ArrowRight': case 'ArrowDown': index++; break;
            case 'ArrowLeft': case 'ArrowUp': index--; break;
            case 'Home': index = 0; break;
            case 'End': index = marks.length - 1; break;
            default: return;
        }
        event.preventDefault();
        marks[Math.max(0, Math.min(marks.length - 1, index))]?.focus({ preventScroll: true });
    };
    const observer = new MutationObserver(records => {
        // A zoom can remove the mark that held focus and leave focus on the body; when that happened,
        // hand it to the first mark that is still there instead of dropping the reader out of the chart.
        const lostFocus = focusedKey !== null
            && !plot.querySelector(`${selector}[data-flare-chart-hit="${CSS.escape(focusedKey)}"]`)
            && document.activeElement === document.body;
        if (records.some(r => r.type === 'childList' &&
            (r.target.closest?.('svg') || [...r.addedNodes, ...r.removedNodes].some(n => n.nodeName === 'svg')))) sync();
        if (lostFocus && marks.length) marks[0].focus({ preventScroll: true });
        detail();
    });
    observer.observe(plot, { childList: true, subtree: true, attributes: true, attributeFilter: ['data-flare-chart-active'] });
    plots.keep(plot, all(
        bindPointer(plot, send),
        bindZoom(plot, send),
        listen(plot, 'focusin', focus),
        listen(plot, 'focusout', event => {
            if (plot.contains(event.relatedTarget)) return;
            // A null relatedTarget means the focused mark was removed, not that the reader left the plot.
            if (event.relatedTarget) focusedKey = null;
            send('ChartDetailDismissed');
        }),
        listen(plot, 'keydown', key),
        () => observer.disconnect()));
    sync();
    detail();
}

export function unobservePlot(plot) { plots.drop(plot); }

function bindPointer(plot, send) {
    let latest = null, frame = 0, sending = false;
    const queue = () => {
        if (!frame && !sending && latest) frame = requestAnimationFrame(deliver);
    };
    const deliver = () => {
        frame = 0;
        if (!latest || !plot.querySelector('[role="tooltip"]')) { latest = null; return; }
        const point = latest;
        latest = null;
        sending = true;
        send('ChartPointerMoved', point.x, point.y).finally(() => {
            sending = false;
            queue();
        });
    };
    const clear = () => {
        latest = null;
        if (frame) cancelAnimationFrame(frame);
        frame = 0;
    };
    return all(
        listen(plot, 'pointermove', event => {
            if (event.pointerType === 'touch' || !plot.querySelector('[role="tooltip"]')) return;
            latest = { x: event.clientX, y: event.clientY };
            queue();
        }),
        listen(plot, 'pointerleave', clear),
        clear);
}

function bindZoom(plot, send) {
    let suppressClick = false;
    const enabled = () => plot.dataset.flareChartZoom === 'true';
    const position = event => {
        const matrix = plot.querySelector('svg')?.getScreenCTM();
        if (!matrix) return null;
        const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());
        return Number.isFinite(point.x) && Number.isFinite(point.y) ? point : null;
    };
    const inside = point => point && point.x >= +plot.dataset.plotLeft
        && point.x <= +plot.dataset.plotLeft + +plot.dataset.plotWidth
        && point.y >= +plot.dataset.plotTop && point.y <= +plot.dataset.plotTop + +plot.dataset.plotHeight;
    const gesture = (phase, event) => {
        const point = position(event);
        send('ChartZoomGesture', point ? phase : 'cancel', point?.x ?? 0);
    };
    // One move per frame: a pointer can report several times between paints, and each move now
    // repaints only the selection overlay, so coalescing is what keeps the rate bounded.
    let moveFrame = 0, moveEvent = null;
    const flushMove = () => {
        moveFrame = 0;
        const event = moveEvent;
        moveEvent = null;
        if (event) gesture('move', event);
    };
    const queueMove = event => {
        moveEvent = event;
        if (!moveFrame) moveFrame = requestAnimationFrame(flushMove);
    };
    const cancelMove = () => {
        moveEvent = null;
        if (moveFrame) cancelAnimationFrame(moveFrame);
        moveFrame = 0;
    };
    const wheel = event => {
        if (!enabled() || !event.ctrlKey || event.deltaY === 0) return;
        const point = position(event);
        if (!inside(point)) return;
        event.preventDefault();
        send('ChartZoomWheel', event.deltaY, point.x);
    };
    return all(
        startDrag(plot, {
            threshold: 2, touchAction: null, trackWindow: true,
            filter: event => {
                if (!enabled() || event.button !== 0 || !event.isPrimary || event.pointerType === 'touch'
                    || !inside(position(event))) return false;
                suppressClick = false;
                return true;
            },
            onStart: event => gesture('start', event),
            onMove: (_dx, _dy, event) => queueMove(event),
            onEnd: event => { cancelMove(); suppressClick = true; gesture('end', event); },
            onCancel: event => { cancelMove(); suppressClick = false; gesture('cancel', event); }
        }),
        listen(plot, 'click', event => {
            if (!suppressClick) return;
            suppressClick = false;
            event.preventDefault();
            event.stopImmediatePropagation();
        }, true),
        listen(plot, 'wheel', wheel, { passive: false }),
        cancelMove);
}
