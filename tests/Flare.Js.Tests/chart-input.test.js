import { afterEach, expect, it, vi } from 'vitest';
import { observePlot, unobservePlot } from '../../src/Flare.Components/wwwroot/js/flare-chart-input.js';

let plot;
const flush = () => new Promise(resolve => setTimeout(resolve, 0));
afterEach(() => { if (plot) unobservePlot(plot); document.body.replaceChildren(); vi.unstubAllGlobals(); });

function setup() {
    plot = document.createElement('div');
    plot.dataset.flareChartTooltip = 'detail';
    plot.innerHTML = '<svg>' + Array.from({ length: 240 }, (_, i) =>
        `<rect data-flare-chart-hit="category:${i}" tabindex="${i === 0 ? 0 : -1}"></rect>`).join('') + '</svg>';
    document.body.append(plot);
    observePlot(plot, { invokeMethodAsync: vi.fn().mockResolvedValue() });
    return [...plot.querySelectorAll('[data-flare-chart-hit]')];
}

it('tooltip changes update only the two described hits without rewriting the plot tab order', async () => {
    const marks = setup(), records = [];
    const observer = new MutationObserver(rs => records.push(...rs));
    observer.observe(plot.querySelector('svg'), { attributes: true, subtree: true });
    plot.dataset.flareChartActive = 'category:100';
    const tooltip = document.createElement('div'); tooltip.id = 'detail'; plot.append(tooltip);
    await flush();
    expect(marks[100].getAttribute('aria-describedby')).toBe('detail');
    plot.dataset.flareChartActive = 'category:101'; tooltip.textContent = 'Next value';
    await flush();
    expect(marks[100].hasAttribute('aria-describedby')).toBe(false);
    expect(marks[101].getAttribute('aria-describedby')).toBe('detail');
    expect(records.filter(r => r.attributeName === 'tabindex')).toHaveLength(0);
    expect(records.filter(r => r.attributeName === 'aria-describedby')).toHaveLength(3);
    delete plot.dataset.flareChartActive; tooltip.remove(); await flush();
    expect(marks[101].hasAttribute('aria-describedby')).toBe(false);
    observer.disconnect();
});

it('rebuilds the roving tab entry when zoom replaces hit geometry', async () => {
    setup();
    plot.querySelector('svg').innerHTML = '<rect data-flare-chart-hit="category:200" tabindex="-1"></rect>';
    await flush();
    expect(plot.querySelector('[data-flare-chart-hit]').tabIndex).toBe(0);
});
