// Flare chart motion: walks a chart's geometry from one dataset to the next.
//
// A new Data replaces the plot's markup, so the new numbers are simply written into the SVG
// attributes and the browser repaints them instantly - twenty points teleport. This watches those
// attributes and moves them, so a value that CHANGED reads as a move rather than as a different
// picture. It is not the enter animation (`Animate`, `stroke-dashoffset` on the line): that one
// redraws the series from nothing, which on a chart updating every three seconds is worse than no
// animation at all.
//
// THE INTEROP COST OF AN ANIMATED UPDATE IS ZERO. A MutationObserver, not a call per update: the
// browser already knows precisely which attributes changed and what they were, so asking .NET would
// mean marshalling the geometry of every element twice per update - on a component whose whole claim
// is that it draws itself without JS.
//
// Not `transition: d` in CSS either. `d` as an animatable property is not carried by every engine,
// and a library cannot ship motion that works in two of the three. Attribute tweening works
// everywhere by construction, and it covers bars, slices and radar spokes as well as the line: all of
// a chart's geometry is numbers inside attributes.

import { registry } from './flare-dom.js';

// Every attribute a chart writes geometry into. `x`/`y` are here for the labels too, so a value label
// travels with the bar it belongs to instead of jumping ahead of it.
const ATTRS = ['d', 'points', 'x', 'y', 'width', 'height', 'cx', 'cy', 'r', 'x1', 'y1', 'x2', 'y2', 'transform'];

const NUMBER = /-?\d*\.?\d+(?:e[-+]?\d+)?/gi;

const _plots = registry();
const _observers = new Map();
// element -> Map(attribute -> tween). One map for every chart on the page, and one frame loop below.
const _tweens = new Map();
let _raf = 0;

// A value split into the text around its numbers and the numbers themselves. Two values can be
// walked between only when the text matches exactly: "the same path with different numbers" is a
// move, and anything else is a different drawing. A line that gained a point has a longer `d` and a
// different set of parts, so it jumps - pretending otherwise slides every point into its neighbour's
// place, which reads as the data changing when it did not.
function _shape(value) {
    const nums = [];
    const parts = [];
    let last = 0;
    NUMBER.lastIndex = 0;
    for (let m = NUMBER.exec(value); m !== null; m = NUMBER.exec(value)) {
        parts.push(value.slice(last, m.index));
        nums.push(parseFloat(m[0]));
        last = m.index + m[0].length;
    }
    parts.push(value.slice(last));
    return { nums, parts };
}

// Same drawing, different numbers. No sentinel to substitute into the text: a placeholder is a
// character that then must not appear in the value, and an invisible one in source is a trap.
function _sameShape(a, b) {
    if (a.parts.length !== b.parts.length) return false;
    for (let i = 0; i < a.parts.length; i++) if (a.parts[i] !== b.parts[i]) return false;
    return true;
}

function _write(parts, nums) {
    let out = parts[0];
    for (let i = 0; i < nums.length; i++) {
        const n = nums[i];
        out += (Number.isInteger(n) ? String(n) : n.toFixed(3)) + parts[i + 1];
    }
    return out;
}

// The duration is a theme's to set, so it is read from the motion scale rather than chosen here.
// A theme that says zero, or a reader who asked for less motion, gets the jump: the new value is
// already in the DOM by the time this runs, so doing nothing IS the old behaviour.
function _ms(value) {
    const v = (value || '').trim();
    if (v.endsWith('ms')) return parseFloat(v) || 0;
    if (v.endsWith('s')) return (parseFloat(v) || 0) * 1000;
    return 0;
}

const _bezA = (a, b) => 1 - 3 * b + 3 * a;
const _bezB = (a, b) => 3 * b - 6 * a;
const _bezC = a => 3 * a;
const _bezAt = (t, a, b) => ((_bezA(a, b) * t + _bezB(a, b)) * t + _bezC(a)) * t;
const _bezSlope = (t, a, b) => 3 * _bezA(a, b) * t * t + 2 * _bezB(a, b) * t + _bezC(a);

// The easing token is a CSS curve and has to be evaluated here, because this is not a CSS transition.
// Newton from x to t, then read y - the same solve a browser does for cubic-bezier().
function _easing(value) {
    const m = /cubic-bezier\(([^)]*)\)/.exec(value || '');
    if (!m) return t => 1 - Math.pow(1 - t, 3);
    const p = m[1].split(',').map(s => parseFloat(s));
    if (p.length !== 4 || p.some(n => !Number.isFinite(n))) return t => 1 - Math.pow(1 - t, 3);
    const [x1, y1, x2, y2] = p;
    if (x1 === y1 && x2 === y2) return t => t;
    return x => {
        let t = x;
        for (let i = 0; i < 8; i++) {
            const slope = _bezSlope(t, x1, x2);
            if (!slope) break;
            t -= (_bezAt(t, x1, x2) - x) / slope;
        }
        return _bezAt(Math.min(1, Math.max(0, t)), y1, y2);
    };
}

function _onMutations(plot, vars, records) {
    const style = getComputedStyle(plot);
    const duration = _ms(style.getPropertyValue(vars.duration));
    if (!duration || matchMedia('(prefers-reduced-motion: reduce)').matches) return;
    const ease = _easing(style.getPropertyValue(vars.easing));

    // The FIRST old value in the batch is where the element visually is; the last new value is where
    // it is going, and that one is simply what the DOM holds now. An attribute written twice in one
    // batch would otherwise start its walk from the halfway state.
    const from = new Map();
    for (const r of records) {
        let attrs = from.get(r.target);
        if (!attrs) from.set(r.target, attrs = new Map());
        if (!attrs.has(r.attributeName)) attrs.set(r.attributeName, r.oldValue);
    }

    const now = performance.now();
    for (const [el, attrs] of from) {
        for (const [attr, was] of attrs) {
            const is = el.getAttribute(attr);
            if (was == null || is == null || was === is) continue;

            const a = _shape(was);
            const b = _shape(is);
            if (!_sameShape(a, b)) continue;

            let running = _tweens.get(el);
            if (!running) _tweens.set(el, running = new Map());
            running.set(attr, { plot, parts: b.parts, from: a.nums, to: b.nums, start: now, duration, ease });
        }
    }

    if (_tweens.size && !_raf) _raf = requestAnimationFrame(_tick);
}

function _tick(now) {
    _raf = 0;
    for (const [el, attrs] of _tweens) {
        for (const [attr, tw] of attrs) {
            const p = Math.min(1, (now - tw.start) / tw.duration);
            const e = tw.ease(p);
            el.setAttribute(attr, _write(tw.parts, tw.from.map((v, i) => v + (tw.to[i] - v) * e)));
            if (p >= 1) attrs.delete(attr);
        }
        if (!attrs.size) _tweens.delete(el);
    }

    // Our own writes are mutations too. Draining them here, in the same task that made them, is what
    // keeps the loop from reading its own last frame back as a new destination - anything the chart
    // itself writes happens in another task and survives this.
    for (const observer of _observers.values()) observer.takeRecords();

    if (_tweens.size) _raf = requestAnimationFrame(_tick);
}

// Starts watching a chart's plot. One observer per chart, for the life of the component.
//
// The two token names are handed in rather than written here: a name spelled in a script is a name the
// CSS audit cannot read, and it exists already as a constant on the .NET side.
export function observePlot(plot, durationVar, easingVar) {
    if (!plot || !durationVar || !easingVar || _plots.has(plot)) return;

    const vars = { duration: durationVar, easing: easingVar };
    const observer = new MutationObserver(records => _onMutations(plot, vars, records));
    observer.observe(plot, { subtree: true, attributes: true, attributeOldValue: true, attributeFilter: ATTRS });
    _observers.set(plot, observer);

    _plots.keep(plot, () => {
        observer.disconnect();
        _observers.delete(plot);
        for (const [el, attrs] of _tweens) {
            for (const [attr, tw] of attrs) if (tw.plot === plot) attrs.delete(attr);
            if (!attrs.size) _tweens.delete(el);
        }
    });
}

// Stops watching, and abandons whatever this chart still had in flight.
export function unobservePlot(plot) {
    _plots.drop(plot);
}
