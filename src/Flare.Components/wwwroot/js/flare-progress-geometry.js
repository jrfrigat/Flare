// SVG user units match CSS pixels. CSS resolves tokens (including rem/calc/var) in hidden boxes;
// observers update geometry only when those lengths, the viewport or the value change.
const observed = new WeakMap();
const number = value => Number(value.toFixed(4)).toString();
const nonnegative = value => Number.isFinite(value) ? Math.max(0, value) : 0;

function attribute(element, name, value) {
    if (value === null) element.removeAttribute(name);
    else if (element.getAttribute(name) !== value) element.setAttribute(name, value);
}

function wavePath(cx, cy, radius, amplitude, waves) {
    // Whole periods and equal angular steps keep rotation/dash animation periodic.
    const segments = Math.max(120, waves * 16);
    const points = [];
    for (let i = 0; i <= segments; i++) {
        const angle = 2 * Math.PI * i / segments;
        const r = radius + amplitude * Math.sin(waves * angle);
        points.push(`${i ? 'L' : 'M'}${number(cx + r * Math.cos(angle))} ${number(cy + r * Math.sin(angle))}`);
    }
    return points.join(' ') + 'Z';
}

function fit(state) {
    const { element, svg, measures } = state;
    if (!element.isConnected) return;
    const style = getComputedStyle(svg);
    const width = parseFloat(style.width), height = parseFloat(style.height);
    if (!(width > 0 && height > 0)) return;
    const track = svg.querySelector('.flare-progress__track');
    // A theme renderer can add a path after the fallback circle. Measure and update that active
    // path when it exists; querySelector used to keep selecting the hidden circle instead.
    const indicator = svg.querySelector('path.flare-progress__indicator')
        ?? svg.querySelector('.flare-progress__indicator');
    if (!track || !indicator) return;

    const [stroke, gap, waveAmplitude, waveLength, waveCount] = measures.map(box => nonnegative(box.getBBox().width));
    const wavy = indicator.localName === 'path';
    const available = Math.max(0, (Math.min(width, height) - stroke) / 2);
    const amplitude = wavy ? Math.min(waveAmplitude, available / 2) : 0;
    const radius = available - amplitude;
    const cx = width / 2, cy = height / 2;
    attribute(svg, 'viewBox', `0 0 ${number(width)} ${number(height)}`);
    attribute(track.parentElement, 'transform', `rotate(-90 ${number(cx)} ${number(cy)})`);
    for (const circle of [track, ...(wavy ? [] : [indicator])]) {
        attribute(circle, 'cx', number(cx));
        attribute(circle, 'cy', number(cy));
        attribute(circle, 'r', number(radius));
    }
    if (wavy) {
        // A whole wave count closes the path without a seam. The wavelength is preferred so larger
        // size steps keep the theme's frequency instead of stretching a fixed number of lobes.
        // RingWaves remains the fallback for existing custom themes.
        const requestedWaves = waveLength > 0 ? Math.floor(2 * Math.PI * radius / waveLength) : Math.floor(waveCount);
        const waves = Math.max(3, Math.min(requestedWaves, Math.ceil(2 * Math.PI * radius)));
        const key = [cx, cy, radius, amplitude, waves].join(',');
        if (key !== state.pathKey) {
            state.pathKey = key;
            state.path = wavePath(cx, cy, radius, amplitude, waves);
        }
        attribute(indicator, 'd', state.path);
    }

    const rawValue = svg.getAttribute('data-value');
    const value = rawValue === null ? null : Math.min(100, nonnegative(Number(rawValue)));
    // Gap measures distance along the unperturbed centerline, as it does for the smooth track.
    const gapPercent = radius > 0 && (value === null || value > 0 && value < 100)
        ? Math.min(100, gap / (2 * Math.PI * radius) * 100)
        : 0;
    const arc = value === null ? 100 : Math.max(0, value - gapPercent);
    const rest = value === null ? 100 : Math.max(0, 100 - value - gapPercent);
    indicator.style.visibility = arc > 0 && radius > 0 ? '' : 'hidden';
    track.style.visibility = rest > 0 && radius > 0 ? '' : 'hidden';
    attribute(track, 'stroke-dasharray', value === null ? null : `0 ${number(value + gapPercent / 2)} ${number(rest)} 100`);
    attribute(indicator, 'stroke-dasharray', value === null ? null : wavy
        ? `${number(arc)} ${number(100 - arc)}`
        : `0 ${number(gapPercent / 2)} ${number(arc)} 100`);
    if (wavy && value !== null) indicator.style.setProperty('--_ring-lead', number(-gapPercent / 2));
    else indicator.style.removeProperty('--_ring-lead');
    if (value === null) track.style.setProperty('--_ring-gap', number(gapPercent));
    else track.style.removeProperty('--_ring-gap');
    // Blazor can rewrite fallback d/dash/style attributes. Observe them, but discard our own writes.
    state.mutations.takeRecords();
}

export function observe(element) {
    if (!element || observed.has(element)) return;
    const svg = element.querySelector('.flare-progress__svg');
    const measures = Array.from(element.querySelectorAll('.flare-progress__measure'));
    if (!svg || measures.length !== 5) return;
    const state = { element, svg, measures };
    state.mutations = new MutationObserver(() => fit(state));
    state.resize = new ResizeObserver(() => fit(state));
    observed.set(element, state);
    state.mutations.observe(svg, {
        subtree: true, childList: true, attributes: true,
        attributeFilter: ['data-value', 'd', 'stroke-dasharray', 'style']
    });
    // Measuring boxes do not depend on the drawing's radius or dash, so fitting cannot resize them.
    state.resize.observe(svg);
    for (const box of measures) state.resize.observe(box);
    fit(state);
}

export function unobserve(element) {
    const state = observed.get(element);
    if (!state) return;
    state.mutations.disconnect();
    state.resize.disconnect();
    observed.delete(element);
}
