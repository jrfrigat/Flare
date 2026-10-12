// Browser-only focus mechanics. Data, geometry and tooltip contents stay in the component.
import { registry, listen, all } from './flare-dom.js';
import { startDrag } from './flare-drag.js';

const plots = registry();
const selector = '[data-flare-chart-hit]';

export function observePlot(plot, receiver) {
    if (!plot) return;
    let active = null;
    const hits = () => [...plot.querySelectorAll(selector)];
    const send = (method, ...args) => receiver.invokeMethodAsync(method, ...args).catch(() => { });
    const sync = () => {
        const marks = hits();
        const chosen = marks.find(el => el.dataset.flareChartHit === active) || marks[0];
        active = chosen?.dataset.flareChartHit || null;
        for (const mark of marks) mark.tabIndex = mark === chosen ? 0 : -1;
    };
    const focus = event => {
        const mark = event.target.closest?.(selector);
        if (!mark || !plot.contains(mark)) return;
        active = mark.dataset.flareChartHit;
        sync();
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
    const observer = new MutationObserver(sync);
    observer.observe(plot, { childList: true, subtree: true });
    plots.keep(plot, all(
        bindPointer(plot, send),
        bindZoom(plot, send),
        listen(plot, 'focusin', focus),
        listen(plot, 'focusout', event => { if (!plot.contains(event.relatedTarget)) send('ChartDetailDismissed'); }),
        listen(plot, 'keydown', key),
        () => observer.disconnect()));
    sync();
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
            onMove: (_dx, _dy, event) => gesture('move', event),
            onEnd: event => { suppressClick = true; gesture('end', event); },
            onCancel: event => { suppressClick = false; gesture('cancel', event); }
        }),
        listen(plot, 'click', event => {
            if (!suppressClick) return;
            suppressClick = false;
            event.preventDefault();
            event.stopImmediatePropagation();
        }, true),
        listen(plot, 'wheel', wheel, { passive: false }));
}
