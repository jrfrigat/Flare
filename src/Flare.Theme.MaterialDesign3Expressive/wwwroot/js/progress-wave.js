// MD3 Expressive progress decoration. The public Wave class is the whole opt-in contract; all
// generated SVG stays inside the neutral hosts' shadow roots so Blazor continues to own light DOM.
const themeClass = 'flare-theme-md3-expressive';
const waveClass = 'flare-progress--md3e-wave';
const linearClass = 'flare-progress--linear';
const circularClass = 'flare-progress--circular';
const indeterminateClass = 'flare-progress--indeterminate';
const decorationClass = 'flare-progress__decoration';
const barClass = 'flare-progress__bar';
const svgClass = 'flare-progress__svg';
const trackClass = 'flare-progress__track';
const indicatorClass = 'flare-progress__indicator';
const readyAttribute = 'data-flare-md3e-wave-ready';
const svgNamespace = 'http://www.w3.org/2000/svg';

const token = {
    height: '--flare-progress-wavy-height',
    length: '--flare-progress-wave-length',
    indeterminateLength: '--flare-progress-indeterminate-wave-length',
    amplitude: '--flare-progress-wave-amplitude',
    speed: '--flare-progress-wave-speed',
    ringLength: '--flare-progress-ring-wave-length',
    ringAmplitude: '--flare-progress-ring-wave-amplitude'
};

const states = new Map();
let patternSequence = 0;
let scanPending = false;

const number = value => Number(value.toFixed(4)).toString();
const nonnegative = value => Number.isFinite(value) ? Math.max(0, value) : 0;

function attribute(element, name, value) {
    if (value === null) element.removeAttribute(name);
    else if (element.getAttribute(name) !== value) element.setAttribute(name, value);
}

function svgElement(name, attributes = {}) {
    const element = document.createElementNS(svgNamespace, name);
    for (const [key, value] of Object.entries(attributes)) attribute(element, key, value);
    return element;
}

function ensureShadow(host) {
    const shadow = host.shadowRoot ?? host.attachShadow({ mode: 'open' });
    for (const node of shadow.querySelectorAll('[data-flare-md3e-owned]')) node.remove();
    return shadow;
}

function measureLength(root, shadow, property, fallback) {
    const raw = getComputedStyle(root).getPropertyValue(property).trim();
    if (/^[+-]?(?:\d+\.?\d*|\.\d+)$/.test(raw)) return nonnegative(Number(raw));
    if (!shadow) return fallback;

    let probe = shadow.querySelector('[data-flare-md3e-measure]');
    if (!probe) {
        probe = document.createElement('span');
        probe.dataset.flareMd3eMeasure = '';
        probe.dataset.flareMd3eOwned = '';
        probe.style.cssText = 'position:fixed;left:0;top:0;display:block;height:0;visibility:hidden;pointer-events:none;';
        shadow.append(probe);
    }
    probe.style.width = `var(${property})`;
    const measured = probe.getBoundingClientRect().width;
    return measured > 0 && Number.isFinite(measured) ? measured : fallback;
}

function parseTime(root, property, fallback) {
    const raw = getComputedStyle(root).getPropertyValue(property).trim().toLowerCase();
    const match = /^([+-]?(?:\d+\.?\d*|\.\d+))(ms|s)$/.exec(raw);
    if (!match) return fallback;
    const value = Number(match[1]);
    return nonnegative(match[2] === 's' ? value * 1000 : value);
}

function linearPath(period, amplitude, middle) {
    const quarter = period / 4;
    const tangent = amplitude * Math.PI / 6;
    const parts = [`M${number(-period)} ${number(middle)}`];
    for (let i = -4; i < 8; i++) {
        const angle = i * Math.PI / 2;
        const nextAngle = (i + 1) * Math.PI / 2;
        const x = i * quarter;
        const nextX = (i + 1) * quarter;
        const y = middle - amplitude * Math.sin(angle);
        const nextY = middle - amplitude * Math.sin(nextAngle);
        parts.push(
            `C${number(x + quarter / 3)} ${number(y - tangent * Math.cos(angle))}`,
            `${number(nextX - quarter / 3)} ${number(nextY + tangent * Math.cos(nextAngle))}`,
            `${number(nextX)} ${number(nextY)}`);
    }
    return parts.join(' ');
}

function circularPath(cx, cy, radius, amplitude, waves) {
    const segments = Math.max(120, waves * 16);
    const parts = [];
    for (let i = 0; i <= segments; i++) {
        const angle = 2 * Math.PI * i / segments;
        const currentRadius = radius + amplitude * Math.sin(waves * angle);
        const x = cx + currentRadius * Math.cos(angle);
        const y = cy + currentRadius * Math.sin(angle);
        parts.push(`${i ? 'L' : 'M'}${number(x)} ${number(y)}`);
    }
    parts.push('Z');
    return parts.join(' ');
}

function createLinearRenderer(host) {
    const shadow = ensureShadow(host);
    const style = document.createElement('style');
    style.dataset.flareMd3eOwned = '';
    style.textContent = `
        :host { display:block; overflow:hidden; }
        svg { display:block; max-width:none; width:calc(100% + var(--_wave-period)); height:100%; }
        path { fill:none; stroke:currentColor; stroke-width:var(--_wave-stroke); stroke-linecap:round; }
        svg[data-indeterminate] {
            animation:flare-md3e-linear-indeterminate-wave
                var(--flare-progress-linear-indeterminate-duration)
                var(--flare-progress-linear-indeterminate-easing) infinite;
        }
        svg:not([data-indeterminate]) {
            animation:flare-md3e-linear-wave var(--flare-progress-wave-speed) linear infinite;
        }
        @keyframes flare-md3e-linear-wave {
            to { transform:translateX(calc(-1 * var(--_wave-period))); }
        }
        @keyframes flare-md3e-linear-indeterminate-wave {
            to { transform:translateX(calc(-3 * var(--_wave-period))); }
        }
        @media (prefers-reduced-motion:reduce) { svg { animation:none !important; } }
    `;

    const svg = svgElement('svg', { 'aria-hidden': 'true' });
    svg.dataset.flareMd3eOwned = '';
    const defs = svgElement('defs');
    const patternId = `flare-md3e-progress-wave-${++patternSequence}`;
    const pattern = svgElement('pattern', { id: patternId, patternUnits: 'userSpaceOnUse' });
    const path = svgElement('path', { stroke: 'currentColor' });
    const rect = svgElement('rect', { width: '100%', height: '100%', fill: `url(#${patternId})` });
    pattern.append(path);
    defs.append(pattern);
    svg.append(defs, rect);
    shadow.append(style, svg);
    return { host, shadow, svg, pattern, path };
}

function updateLinear(root, renderer, indeterminate) {
    const { shadow, svg, pattern, path } = renderer;
    const stroke = measureLength(root, shadow, '--_lin-height', 4);
    const amplitude = measureLength(root, shadow, token.amplitude, 3);
    const requestedHeight = measureLength(root, shadow, token.height, 10);
    const height = Math.max(requestedHeight, stroke + 2 * amplitude);
    const periodToken = indeterminate ? token.indeterminateLength : token.length;
    const period = Math.max(1, measureLength(root, shadow, periodToken, indeterminate ? 20 : 40));

    svg.toggleAttribute('data-indeterminate', indeterminate);
    svg.style.setProperty('--_wave-period', `${number(period)}px`);
    svg.style.setProperty('--_wave-stroke', `${number(stroke)}px`);
    attribute(svg, 'height', number(height));
    attribute(pattern, 'width', number(period));
    attribute(pattern, 'height', number(height));
    attribute(path, 'd', linearPath(period, amplitude, height / 2));
}

function createCircularRenderer(host) {
    const shadow = ensureShadow(host);
    const style = document.createElement('style');
    style.dataset.flareMd3eOwned = '';
    style.textContent = `
        :host { display:block; overflow:visible; }
        svg { display:block; width:100%; height:100%; overflow:visible; }
        path {
            fill:none;
            stroke:var(--fc-main, var(--flare-color-primary));
            stroke-width:var(--_circ-width);
            stroke-linecap:var(--flare-progress-circular-cap);
            stroke-linejoin:round;
            transform-box:view-box;
            transform-origin:50% 50%;
            stroke-dashoffset:calc(var(--_ring-lead, 0) + var(--_ring-sweep, 0));
            animation:flare-md3e-ring-wave var(--_ring-duration) linear infinite;
        }
        path[data-indeterminate] {
            stroke-dasharray:var(--_ring-progress) calc(100 - var(--_ring-progress));
            stroke-dashoffset:0;
            animation:none;
        }
        @keyframes flare-md3e-ring-wave {
            from { rotate:0deg; --_ring-sweep:0; }
            to { rotate:360deg; --_ring-sweep:100; }
        }
        @media (prefers-reduced-motion:reduce) { path { animation:none !important; } }
    `;
    const svg = svgElement('svg', { 'aria-hidden': 'true' });
    svg.dataset.flareMd3eOwned = '';
    const group = svgElement('g');
    const path = svgElement('path', { pathLength: '100' });
    group.append(path);
    svg.append(group);
    shadow.append(style, svg);
    return { host, shadow, svg, group, path };
}

function setCircleGeometry(circle, cx, cy, radius) {
    attribute(circle, 'cx', number(cx));
    attribute(circle, 'cy', number(cy));
    attribute(circle, 'r', number(radius));
}

function updateCircular(root, renderer) {
    const coreSvg = root.querySelector(`.${svgClass}`);
    const track = coreSvg?.querySelector(`circle.${trackClass}`);
    const fallback = coreSvg?.querySelector(`circle.${indicatorClass}`);
    if (!coreSvg || !track || !fallback) return false;

    const bounds = coreSvg.getBoundingClientRect();
    const width = bounds.width;
    const height = bounds.height;
    if (!(width > 0 && height > 0)) return false;

    const stroke = nonnegative(parseFloat(getComputedStyle(fallback).strokeWidth)) || 4;
    const requestedAmplitude = measureLength(root, renderer.shadow, token.ringAmplitude, 1.6);
    const ringLength = measureLength(root, renderer.shadow, token.ringLength, 15);
    const measures = coreSvg.querySelectorAll('.flare-progress__measure');
    const gap = nonnegative(measures[1]?.getBBox().width) || 0;
    const available = Math.max(0, (Math.min(width, height) - stroke) / 2);
    const amplitude = Math.min(requestedAmplitude, available / 2);
    const radius = Math.max(0, available - amplitude);
    const cx = width / 2;
    const cy = height / 2;
    const circumference = 2 * Math.PI * radius;
    const requestedWaves = ringLength > 0 ? Math.floor(circumference / ringLength) : 0;
    const waves = Math.max(3, Math.min(requestedWaves || 3, 128));
    const gapPercent = radius > 0 ? Math.min(100, gap / circumference * 100) : 0;
    const rawValue = coreSvg.getAttribute('data-value');
    const value = rawValue === null ? null : Math.min(100, nonnegative(Number(rawValue)));

    attribute(renderer.svg, 'viewBox', `0 0 ${number(width)} ${number(height)}`);
    attribute(renderer.group, 'transform', `rotate(-90 ${number(cx)} ${number(cy)})`);
    attribute(renderer.path, 'd', circularPath(cx, cy, radius, amplitude, waves));
    renderer.path.toggleAttribute('data-indeterminate', value === null);
    renderer.path.style.setProperty('--_ring-duration', `${number(parseTime(root, token.speed, 1000) * waves)}ms`);

    setCircleGeometry(track, cx, cy, radius);
    setCircleGeometry(fallback, cx, cy, radius);
    if (track.parentElement) attribute(track.parentElement, 'transform', `rotate(-90 ${number(cx)} ${number(cy)})`);

    if (value === null) {
        attribute(renderer.path, 'stroke-dasharray', null);
        renderer.path.style.removeProperty('--_ring-lead');
        renderer.path.style.visibility = radius > 0 ? '' : 'hidden';
        track.style.setProperty('--_ring-gap', number(gapPercent));
        track.style.visibility = radius > 0 ? '' : 'hidden';
    } else {
        const arc = Math.max(0, value - gapPercent);
        const rest = Math.max(0, 100 - value - gapPercent);
        attribute(renderer.path, 'stroke-dasharray', `${number(arc)} ${number(100 - arc)}`);
        renderer.path.style.setProperty('--_ring-lead', number(-gapPercent / 2));
        renderer.path.style.visibility = arc > 0 && radius > 0 ? '' : 'hidden';
        attribute(track, 'stroke-dasharray', `0 ${number(value + gapPercent / 2)} ${number(rest)} 100`);
        track.style.removeProperty('--_ring-gap');
        track.style.visibility = rest > 0 && radius > 0 ? '' : 'hidden';
    }
    return true;
}

function restoreCircular(root) {
    const svg = root.querySelector(`.${svgClass}`);
    const track = svg?.querySelector(`circle.${trackClass}`);
    const indicator = svg?.querySelector(`circle.${indicatorClass}`);
    if (!svg || !track || !indicator) return;

    const bounds = svg.getBoundingClientRect();
    const width = bounds.width;
    const height = bounds.height;
    if (!(width > 0 && height > 0)) return;

    const stroke = nonnegative(parseFloat(getComputedStyle(indicator).strokeWidth)) || 4;
    const radius = Math.max(0, (Math.min(width, height) - stroke) / 2);
    const cx = width / 2;
    const cy = height / 2;
    const measures = svg.querySelectorAll('.flare-progress__measure');
    const gap = nonnegative(measures[1]?.getBBox().width) || 0;
    const rawValue = svg.getAttribute('data-value');
    const value = rawValue === null ? null : Math.min(100, nonnegative(Number(rawValue)));
    const gapPercent = radius > 0 && (value === null || value > 0 && value < 100)
        ? Math.min(100, gap / (2 * Math.PI * radius) * 100)
        : 0;

    attribute(svg, 'viewBox', `0 0 ${number(width)} ${number(height)}`);
    if (track.parentElement) attribute(track.parentElement, 'transform', `rotate(-90 ${number(cx)} ${number(cy)})`);
    setCircleGeometry(track, cx, cy, radius);
    setCircleGeometry(indicator, cx, cy, radius);

    if (value === null) {
        attribute(track, 'stroke-dasharray', null);
        attribute(indicator, 'stroke-dasharray', null);
        indicator.style.visibility = radius > 0 ? '' : 'hidden';
        track.style.visibility = radius > 0 ? '' : 'hidden';
        track.style.setProperty('--_ring-gap', number(gapPercent));
    } else {
        const arc = Math.max(0, value - gapPercent);
        const rest = Math.max(0, 100 - value - gapPercent);
        attribute(track, 'stroke-dasharray', `0 ${number(value + gapPercent / 2)} ${number(rest)} 100`);
        attribute(indicator, 'stroke-dasharray', `0 ${number(gapPercent / 2)} ${number(arc)} 100`);
        track.style.removeProperty('--_ring-gap');
        indicator.style.visibility = arc > 0 && radius > 0 ? '' : 'hidden';
        track.style.visibility = rest > 0 && radius > 0 ? '' : 'hidden';
    }
}

function update(state) {
    state.pending = false;
    const { root } = state;
    if (!root.isConnected || !isActive(root)) return;

    const indeterminate = root.classList.contains(indeterminateClass);
    if (state.kind === 'linear') {
        for (const renderer of state.renderers) updateLinear(root, renderer, indeterminate);
        root.toggleAttribute(readyAttribute, state.renderers.length > 0);
    } else {
        root.toggleAttribute(readyAttribute, updateCircular(root, state.renderers[0]));
    }
}

function scheduleUpdate(state) {
    if (state.pending) return;
    state.pending = true;
    queueMicrotask(() => update(state));
}

function activate(root) {
    const linear = root.classList.contains(linearClass);
    const circular = root.classList.contains(circularClass);
    if (!linear && !circular) return;

    const hosts = linear
        ? Array.from(root.querySelectorAll(`.${barClass} > .${decorationClass}`))
        : Array.from(root.querySelectorAll(`:scope > .${decorationClass}`));
    if (!hosts.length) return;

    const state = {
        root,
        kind: linear ? 'linear' : 'circular',
        renderers: hosts.map(host => linear ? createLinearRenderer(host) : createCircularRenderer(host)),
        pending: false
    };
    state.resize = new ResizeObserver(() => scheduleUpdate(state));
    state.resize.observe(root);
    for (const host of hosts) state.resize.observe(host);
    states.set(root, state);
    update(state);
}

function deactivate(root, state) {
    state.resize.disconnect();
    root.removeAttribute(readyAttribute);
    if (state.kind === 'circular') restoreCircular(root);
    for (const renderer of state.renderers) {
        for (const node of renderer.shadow.querySelectorAll('[data-flare-md3e-owned]')) node.remove();
    }
    states.delete(root);
}

function isActive(root) {
    return document.documentElement.classList.contains(themeClass)
        && root.classList.contains(waveClass);
}

function scan() {
    scanPending = false;
    const activeRoots = new Set(document.querySelectorAll(`.${waveClass}`));
    for (const [root, state] of states) {
        if (!activeRoots.has(root) || !isActive(root)) deactivate(root, state);
    }
    if (!document.documentElement.classList.contains(themeClass)) return;
    for (const root of activeRoots) {
        const state = states.get(root);
        const linear = root.classList.contains(linearClass);
        const hosts = linear
            ? Array.from(root.querySelectorAll(`.${barClass} > .${decorationClass}`))
            : Array.from(root.querySelectorAll(`:scope > .${decorationClass}`));
        const stale = state && (state.kind !== (linear ? 'linear' : 'circular')
            || hosts.length !== state.renderers.length
            || hosts.some((host, index) => host !== state.renderers[index].host));
        if (stale) deactivate(root, state);
        if (!states.has(root)) activate(root);
        else scheduleUpdate(states.get(root));
    }
}

function scheduleScan() {
    if (scanPending) return;
    scanPending = true;
    queueMicrotask(scan);
}

const mutations = new MutationObserver(records => {
    const onlyCircularDrawingStyles = records.every(record => record.attributeName === 'style'
        && (record.target.classList.contains(trackClass) || record.target.classList.contains(indicatorClass)));
    if (!onlyCircularDrawingStyles) scheduleScan();
});
mutations.observe(document.documentElement, {
    subtree: true,
    childList: true,
    attributes: true,
    attributeFilter: ['class', 'style', 'data-value']
});

window.addEventListener('resize', scheduleScan, { passive: true });
scan();
