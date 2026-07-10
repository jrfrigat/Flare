// Flare viewport service backend. Backs Flare.Components.IBrowserViewportService: a single shared,
// throttled window-resize listener that fans out to every C# subscriber (breakpoint logic lives in C#,
// so this module only reports raw pixel sizes), arbitrary media-query matching, and per-element
// observation via the browser ResizeObserver API. One listener is shared across all subscribers, so
// interop cost is one call per throttle window regardless of how many components subscribe.

// -- viewport size / media query ------------------------------------------------

// window.innerWidth/Height can read as 0 in an unsettled/very-early paint moment; fall back to the
// document element, which is generally populated before window dimensions settle.
function readSize() {
    let w = window.innerWidth;
    let h = window.innerHeight;
    if (!(w > 0)) w = (document.documentElement && document.documentElement.clientWidth) || 0;
    if (!(h > 0)) h = (document.documentElement && document.documentElement.clientHeight) || 0;
    return { width: w, height: h };
}

export function getViewportSize() {
    return readSize();
}

export function matchMedia(query) {
    try { return window.matchMedia(query).matches; }
    catch { return false; }
}

// -- shared window-resize listener ---------------------------------------------

let _winDotNet = null;
let _winThrottle = 100;
let _winTimer = -1;
let _winAttached = false;

function onWindowResize() {
    if (_winTimer >= 0) clearTimeout(_winTimer);
    _winTimer = window.setTimeout(fireWindowResize, _winThrottle);
}

function fireWindowResize() {
    _winTimer = -1;
    if (!_winDotNet) return;
    const s = readSize();
    try { _winDotNet.invokeMethodAsync('OnViewportResized', s.width, s.height); }
    catch { /* circuit gone */ }
}

// Idempotent: called by C# on every viewport subscription. Attaches the listener once and lowers the
// throttle if a more demanding subscription arrives, so the fastest subscriber is honored.
export function ensureViewportListener(dotNetRef, throttleMs) {
    _winDotNet = dotNetRef;
    if (typeof throttleMs === 'number' && throttleMs >= 0) {
        _winThrottle = _winAttached ? Math.min(_winThrottle, throttleMs) : throttleMs;
    }
    if (_winAttached) return;
    window.addEventListener('resize', onWindowResize, { passive: true });
    _winAttached = true;
}

export function stopViewportListener() {
    if (_winTimer >= 0) { clearTimeout(_winTimer); _winTimer = -1; }
    if (_winAttached) { window.removeEventListener('resize', onWindowResize); _winAttached = false; }
    _winDotNet = null;
}

// -- per-element ResizeObserver ------------------------------------------------

const _elMap = new Map();          // id -> { element, throttle, timer, dotNet, initialized }
const _elByTarget = new WeakMap(); // element -> id
let _ro = null;

function rectOf(el) {
    const r = el.getBoundingClientRect();
    return {
        top: r.top, left: r.left, width: r.width, height: r.height,
        windowWidth: window.innerWidth, windowHeight: window.innerHeight,
        scrollX: window.scrollX || window.pageXOffset || 0,
        scrollY: window.scrollY || window.pageYOffset || 0,
    };
}

function ensureRo() {
    if (_ro) return;
    _ro = new ResizeObserver((entries) => {
        for (const entry of entries) {
            const id = _elByTarget.get(entry.target);
            if (id === undefined) continue;
            const rec = _elMap.get(id);
            if (!rec) continue;
            // ResizeObserver fires once on observe() with the initial size; C# already has that from
            // the observeElement() return value, so always suppress this first synthetic emission.
            if (!rec.initialized) { rec.initialized = true; continue; }
            scheduleElement(rec);
        }
    });
}

function scheduleElement(rec) {
    if (rec.timer >= 0) clearTimeout(rec.timer);
    rec.timer = window.setTimeout(() => {
        rec.timer = -1;
        if (!rec.dotNet) return;
        try { rec.dotNet.invokeMethodAsync('OnElementResized', rec.id, rectOf(rec.element)); }
        catch { /* circuit gone */ }
    }, rec.throttle);
}

// Returns the element's initial geometry so C# can deliver the immediate notification without a
// second round-trip.
export function observeElement(id, dotNetRef, element, throttleMs) {
    if (!element) return null;
    ensureRo();
    const rec = {
        id, element, throttle: (throttleMs >= 0 ? throttleMs : 200), timer: -1,
        dotNet: dotNetRef, initialized: false,
    };
    _elMap.set(id, rec);
    _elByTarget.set(element, id);
    _ro.observe(element);
    return rectOf(element);
}

export function unobserveElement(id) {
    const rec = _elMap.get(id);
    if (!rec) return;
    if (rec.timer >= 0) clearTimeout(rec.timer);
    if (_ro && rec.element) _ro.unobserve(rec.element);
    _elByTarget.delete(rec.element);
    _elMap.delete(id);
}

export function disposeAll() {
    stopViewportListener();
    if (_ro) { _ro.disconnect(); _ro = null; }
    _elMap.clear();
}
