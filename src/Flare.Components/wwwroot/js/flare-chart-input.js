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
        listen(plot, 'focusin', focus),
        listen(plot, 'focusout', event => { if (!plot.contains(event.relatedTarget)) send('ChartDetailDismissed'); }),
        listen(plot, 'keydown', key),
        () => observer.disconnect()));
    sync();
}

export function unobservePlot(plot) { plots.drop(plot); }
