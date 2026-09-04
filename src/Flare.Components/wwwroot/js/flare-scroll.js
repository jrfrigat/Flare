// Flare scroll service backend. Backs Flare.Components.IScrollService: one throttled listener PER
// SUBSCRIPTION, on the page or on a given element, plus programmatic scrolling and a reference-counted
// body lock. Direction, progress and the at-start/at-end flags are computed in C# from the position
// this module reports, so the wire payload stays one small object per throttle window per subscriber.
//
// Per subscription rather than per target, deliberately. Sharing one listener across the subscribers
// of a target would save exactly one passive listener registration per extra subscriber and nothing
// else: each subscription carries its own ThrottleMs, so the timers cannot be shared, and each already
// receives its own small payload. What it would cost is a refcounted registry between subscribe and
// unsubscribe, on the path where a disposed circuit and a live one have to unwind independently. The
// header used to claim the opposite of what the code does; the interface always documented this one.

import { listen, pageTarget, listenTarget, resolve } from './flare-dom.js';

// -- reading -------------------------------------------------------------------

function positionOf(el) {
    const t = el || pageTarget();
    return {
        top: t.scrollTop || 0,
        left: t.scrollLeft || 0,
        scrollHeight: t.scrollHeight || 0,
        clientHeight: t.clientHeight || 0,
        scrollWidth: t.scrollWidth || 0,
        clientWidth: t.clientWidth || 0,
    };
}

export function getPosition(element, selector) {
    return positionOf(resolve(element, selector));
}

// -- shared listeners ----------------------------------------------------------

// id -> { element (null = page), throttle, timer, dotNet, off }
const _subs = new Map();

function schedule(rec) {
    if (rec.timer >= 0) return;   // leading edge already claimed; the trailing fire will carry the latest
    rec.timer = window.setTimeout(() => {
        rec.timer = -1;
        if (!rec.dotNet) return;
        // Both arms are needed: a disposed reference throws synchronously, a gone circuit rejects the
        // promise, and a try/catch alone only sees the first of those.
        try { rec.dotNet.invokeMethodAsync('OnScrolled', rec.id, positionOf(rec.element)).catch(() => { }); }
        catch { /* circuit gone */ }
    }, rec.throttle);
}

export function subscribe(id, dotNetRef, element, selector, throttleMs) {
    unsubscribe(id);
    const rec = {
        id,
        element: resolve(element, selector),
        throttle: (typeof throttleMs === 'number' && throttleMs >= 0) ? throttleMs : 100,
        timer: -1,
        dotNet: dotNetRef,
        off: null,
    };
    rec.off = listen(listenTarget(rec.element), 'scroll', () => schedule(rec), { passive: true });
    _subs.set(id, rec);
    return positionOf(rec.element);
}

export function unsubscribe(id) {
    const rec = _subs.get(id);
    if (!rec) return;
    if (rec.timer >= 0) clearTimeout(rec.timer);
    rec.off?.();
    _subs.delete(id);
}

// -- driving -------------------------------------------------------------------

function behaviorOf(b) {
    return b === 'Smooth' ? 'smooth' : b === 'Instant' ? 'instant' : 'auto';
}

export function scrollTo(element, selector, top, behavior) {
    const t = resolve(element, selector) || pageTarget();
    try { t.scrollTo({ top, behavior: behaviorOf(behavior) }); }
    catch { t.scrollTop = top; }
}

export function scrollToEnd(element, selector, behavior) {
    const t = resolve(element, selector) || pageTarget();
    scrollTo(element, selector, Math.max(0, (t.scrollHeight || 0) - (t.clientHeight || 0)), behavior);
}

export function scrollIntoView(elementId, block, behavior) {
    const el = document.getElementById(elementId);
    if (!el) return;
    const b = (block || 'nearest').toLowerCase();
    try { el.scrollIntoView({ block: b, behavior: behaviorOf(behavior) }); }
    catch { el.scrollIntoView(); }
}

// -- body lock -----------------------------------------------------------------

// Counted, so nesting a dialog inside a drawer does not hand the page back when the inner one closes.
// Padding replaces the scrollbar's width to stop the fixed chrome jumping sideways as it disappears.
let _lockCount = 0;
let _savedOverflow = '';
let _savedPadding = '';

export function lock() {
    if (_lockCount === 0) {
        const body = document.body;
        const gap = window.innerWidth - (document.documentElement.clientWidth || window.innerWidth);
        _savedOverflow = body.style.overflow;
        _savedPadding = body.style.paddingRight;
        body.style.overflow = 'hidden';
        if (gap > 0) body.style.paddingRight = `${gap}px`;
    }
    _lockCount++;
    return _lockCount;
}

export function unlock() {
    if (_lockCount > 0) _lockCount--;
    if (_lockCount === 0) {
        document.body.style.overflow = _savedOverflow;
        document.body.style.paddingRight = _savedPadding;
        _savedOverflow = '';
        _savedPadding = '';
    }
    return _lockCount;
}

export function disposeAll() {
    for (const id of [..._subs.keys()]) unsubscribe(id);
    while (_lockCount > 0) unlock();
}
