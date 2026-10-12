// Browser-only focus mechanics. Data, geometry and tooltip contents stay in the component.
import { registry, listen, all } from './flare-dom.js';

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
        bindZoom(plot, send),
        listen(plot, 'focusin', focus),
        listen(plot, 'focusout', event => { if (!plot.contains(event.relatedTarget)) send('ChartDetailDismissed'); }),
        listen(plot, 'keydown', key),
        () => observer.disconnect()));
    sync();
}

export function unobservePlot(plot) { plots.drop(plot); }

function bindZoom(plot, send) {
    let drag = null, suppressClick = false;
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
    const down = event => {
        if (!enabled() || drag || event.button !== 0 || !event.isPrimary || event.pointerType === 'touch') return;
        suppressClick = false;
        const point = position(event);
        if (!inside(point)) return;
        drag = { id: event.pointerId, start: point.x, moved: false };
        send('ChartZoomGesture', 'start', point.x);
    };
    const move = event => {
        if (!drag || event.pointerId !== drag.id) return;
        const point = position(event);
        if (!point) return;
        if (!drag.moved && Math.abs(point.x - drag.start) > 2) {
            drag.moved = true;
            // Delay capture until a drag, so an ordinary click still targets its SVG hit zone.
            plot.setPointerCapture(drag.id);
        }
        if (drag.moved) send('ChartZoomGesture', 'move', point.x);
    };
    const finish = (event, cancel) => {
        if (!drag || event.pointerId !== drag.id) return;
        const current = drag;
        drag = null;
        const point = position(event);
        suppressClick = !cancel && current.moved;
        send('ChartZoomGesture', cancel || !point ? 'cancel' : 'end', point?.x ?? current.start);
        if (plot.hasPointerCapture(current.id)) plot.releasePointerCapture(current.id);
    };
    const wheel = event => {
        if (!enabled() || !event.ctrlKey || event.deltaY === 0) return;
        const point = position(event);
        if (!inside(point)) return;
        event.preventDefault();
        send('ChartZoomWheel', event.deltaY, point.x);
    };
    return all(
        listen(plot, 'pointerdown', down),
        listen(window, 'pointermove', move),
        listen(window, 'pointerup', event => finish(event, false)),
        listen(window, 'pointercancel', event => finish(event, true)),
        listen(plot, 'lostpointercapture', event => finish(event, true)),
        listen(plot, 'click', event => {
            if (!suppressClick) return;
            suppressClick = false;
            event.preventDefault();
            event.stopImmediatePropagation();
        }, true),
        listen(plot, 'wheel', wheel, { passive: false }),
        () => { if (drag && plot.hasPointerCapture(drag.id)) plot.releasePointerCapture(drag.id); drag = null; });
}
