// Fits the circular progress fallback to browser-resolved CSS lengths.
const observed = new WeakMap();
const number = value => Number(value.toFixed(4)).toString();
const nonnegative = value => Number.isFinite(value) ? Math.max(0, value) : 0;

function attribute(element, name, value) {
    if (value === null) element.removeAttribute(name);
    else if (element.getAttribute(name) !== value) element.setAttribute(name, value);
}

function fit(state) {
    const { element, svg, measures } = state;
    if (!element.isConnected) return;

    const style = getComputedStyle(svg);
    const width = parseFloat(style.width);
    const height = parseFloat(style.height);
    if (!(width > 0 && height > 0)) return;

    const track = svg.querySelector('circle.flare-progress__track');
    const indicator = svg.querySelector('circle.flare-progress__indicator');
    if (!track || !indicator) return;

    const [stroke, gap] = measures.map(box => nonnegative(box.getBBox().width));
    const radius = Math.max(0, (Math.min(width, height) - stroke) / 2);
    const cx = width / 2;
    const cy = height / 2;

    attribute(svg, 'viewBox', `0 0 ${number(width)} ${number(height)}`);
    attribute(track.parentElement, 'transform', `rotate(-90 ${number(cx)} ${number(cy)})`);
    for (const circle of [track, indicator]) {
        attribute(circle, 'cx', number(cx));
        attribute(circle, 'cy', number(cy));
        attribute(circle, 'r', number(radius));
    }

    const rawValue = svg.getAttribute('data-value');
    const value = rawValue === null ? null : Math.min(100, nonnegative(Number(rawValue)));
    const gapPercent = radius > 0 && (value === null || value > 0 && value < 100)
        ? Math.min(100, gap / (2 * Math.PI * radius) * 100)
        : 0;
    const arc = value === null ? 100 : Math.max(0, value - gapPercent);
    const rest = value === null ? 100 : Math.max(0, 100 - value - gapPercent);

    // Publish the two numbers a theme cannot work out for itself: turning a CSS gap length into a
    // share of the circumference needs the radius, which only exists once the browser has laid the
    // ring out. Plain geometry, no opinion about how it should look.
    svg.style.setProperty('--_ring-arc', number(arc));
    svg.style.setProperty('--_ring-lead', number(gapPercent / 2));

    indicator.style.visibility = arc > 0 && radius > 0 ? '' : 'hidden';
    track.style.visibility = rest > 0 && radius > 0 ? '' : 'hidden';
    attribute(track, 'stroke-dasharray', value === null
        ? null
        : `0 ${number(value + gapPercent / 2)} ${number(rest)} 100`);
    attribute(indicator, 'stroke-dasharray', value === null
        ? null
        : `0 ${number(gapPercent / 2)} ${number(arc)} 100`);
    if (value === null) track.style.setProperty('--_ring-gap', number(gapPercent));
    else track.style.removeProperty('--_ring-gap');

    state.mutations.takeRecords();
}

export function observe(element) {
    if (!element || observed.has(element)) return;
    const svg = element.querySelector('.flare-progress__svg');
    const measures = Array.from(element.querySelectorAll('.flare-progress__measure'));
    if (!svg || measures.length !== 2) return;

    const state = { element, svg, measures };
    state.mutations = new MutationObserver(() => fit(state));
    state.resize = new ResizeObserver(() => fit(state));
    observed.set(element, state);
    state.mutations.observe(svg, {
        subtree: true,
        attributes: true,
        attributeFilter: ['data-value']
    });
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
