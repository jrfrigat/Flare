export function setCssVariables(vars) {
    const root = document.documentElement;
    for (const [name, value] of Object.entries(vars))
        root.style.setProperty(name, value);
}

// ClassToggle strategy: put the active theme/palette/mode classes on <html> so the
// generated CSS applies to everything, including overlays portaled to <body>.
export function setThemeClasses(themeId, paletteId, dark) {
    const r = document.documentElement;
    r.className = r.className
        .replace(/\bflare-theme-\S+/g, '')
        .replace(/\bflare-palette-\S+/g, '')
        .replace(/\bflare-mode-dark\b/g, '')
        .replace(/\s+/g, ' ')
        .trim();
    r.classList.add('flare-theme-' + themeId, 'flare-palette-' + paletteId);
    if (dark) r.classList.add('flare-mode-dark');
}

// Runtime safety net: make sure a stylesheet <link> is present (for themes/palettes
// registered after the initial <head> render). No-op if already loaded.
export function ensureStylesheet(href) {
    if (!href) return;
    if (!document.querySelector(`link[rel="stylesheet"][href="${CSS.escape(href)}"]`)) {
        const l = document.createElement('link');
        l.rel = 'stylesheet';
        l.href = href;
        document.head.appendChild(l);
    }
}

// ClassToggle strategy: keep the generated class-scoped theme CSS in a single <style>.
// Switching theme/palette/mode is then just a class change on the root element.
export function setStaticThemeCss(css) {
    let el = document.getElementById('flare-theme-static');
    if (!el) {
        el = document.createElement('style');
        el.id = 'flare-theme-static';
        document.head.appendChild(el);
    }
    el.textContent = css;
}

// Custom tokens must override the ClassToggle theme. The theme/palette/mode classes sit on BOTH
// <html> (for body-portaled overlays) and the FlareThemeProvider's .flare-root element, and that
// element re-declares the color/design tokens for its subtree -- so an inline custom value on <html>
// alone is shadowed inside the app. Write to both: <html> (covers overlays) and the outermost
// .flare-root (covers the in-app subtree); inline wins over the element's own class rules.
function flareTokenTargets() {
    const targets = [document.documentElement];
    const root = document.querySelector('.flare-root');
    if (root) targets.push(root);
    return targets;
}

export function setCustomTokens(tokens) {
    for (const el of flareTokenTargets())
        for (const [key, value] of Object.entries(tokens))
            el.style.setProperty(key, value);
}

export function clearCustomTokens(tokenNames) {
    for (const el of flareTokenTargets())
        for (const name of tokenNames)
            el.style.removeProperty(name);
}

export function getCssVariable(name) {
    return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
}

export function prefersColorSchemeDark() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
}

// --- Body scroll-lock ---
let _scrollLockCount = 0;
let _savedOverflow = '';

export function lockBodyScroll() {
    if (_scrollLockCount === 0) {
        _savedOverflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';
    }
    _scrollLockCount++;
}

export function unlockBodyScroll() {
    if (_scrollLockCount > 0) _scrollLockCount--;
    if (_scrollLockCount === 0) {
        document.body.style.overflow = _savedOverflow;
        _savedOverflow = '';
    }
}

// --- Dialog Esc handlers ---
const _escHandlers = new Map();

export function registerDialogEscHandler(id, dotNetRef) {
    _removeEsc(id);
    const handler = (e) => {
        if (e.key === 'Escape') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('CloseFromEsc');
        }
    };
    _escHandlers.set(id, handler);
    document.addEventListener('keydown', handler);
}

export function removeDialogEscHandler(id) {
    _removeEsc(id);
}

function _removeEsc(id) {
    const h = _escHandlers.get(id);
    if (h) {
        document.removeEventListener('keydown', h);
        _escHandlers.delete(id);
    }
}

// --- Focus trap for dialogs ---
const FOCUSABLE_SELECTORS =
    'a[href]:not([disabled]), button:not([disabled]), input:not([disabled]), ' +
    'select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

const _focusTraps = new Map();

export function trapFocus(id, dialogEl) {
    releaseFocusTrap(id);

    const focusable = () => Array.from(dialogEl.querySelectorAll(FOCUSABLE_SELECTORS))
        .filter(el => !el.closest('[hidden]') && el.offsetParent !== null);

    const previouslyFocused = document.activeElement;

    const handler = (e) => {
        if (e.key !== 'Tab') return;
        const els = focusable();
        if (els.length === 0) { e.preventDefault(); return; }
        const first = els[0];
        const last = els[els.length - 1];
        if (e.shiftKey) {
            if (document.activeElement === first) { e.preventDefault(); last.focus(); }
        } else {
            if (document.activeElement === last) { e.preventDefault(); first.focus(); }
        }
    };

    dialogEl.addEventListener('keydown', handler);
    _focusTraps.set(id, { handler, dialogEl, previouslyFocused });

    // Focus the first focusable element
    const els = focusable();
    if (els.length > 0) els[0].focus();
}

export function releaseFocusTrap(id) {
    const trap = _focusTraps.get(id);
    if (trap) {
        trap.dialogEl.removeEventListener('keydown', trap.handler);
        _focusTraps.delete(id);
        // Restore focus to the element that was active before the dialog opened
        try { trap.previouslyFocused?.focus(); } catch { }
    }
}

export function focusFirstInDialog(dialogEl) {
    const focusable = dialogEl?.querySelector(FOCUSABLE_SELECTORS);
    focusable?.focus();
}

// --- Scroll-top button ---
const _scrollHandlers = new Map();

// selector: a scroll container to watch; when null/missing the page (window) is used. This lets
// the button work in app-shell layouts where the scroll happens on an inner element, not the window.
export function registerScrollTopHandler(id, dotNetRef, threshold, selector) {
    _removeScrollHandler(id);
    const target = selector ? document.querySelector(selector) : window;
    if (!target) return; // selector did not match (yet) -- nothing to watch
    const scrolled = () => (target === window ? window.scrollY : target.scrollTop);
    const handler = () => dotNetRef.invokeMethodAsync('SetVisible', scrolled() > threshold);
    _scrollHandlers.set(id, { target, handler });
    target.addEventListener('scroll', handler, { passive: true });
    handler(); // initial check
}

export function removeScrollTopHandler(id) { _removeScrollHandler(id); }

function _removeScrollHandler(id) {
    const h = _scrollHandlers.get(id);
    if (h) { h.target.removeEventListener('scroll', h.handler); _scrollHandlers.delete(id); }
}

export function scrollToTop(selector) {
    const target = selector ? document.querySelector(selector) : window;
    if (target) target.scrollTo({ top: 0, behavior: 'smooth' });
}

// --- System color-scheme live subscription ---
const _schemeListeners = new Map();

export function subscribeColorScheme(id, dotNetRef) {
    unsubscribeColorScheme(id);
    const mq = window.matchMedia('(prefers-color-scheme: dark)');
    const handler = (e) => dotNetRef.invokeMethodAsync('OnSystemColorSchemeChanged', e.matches);
    _schemeListeners.set(id, { mq, handler });
    mq.addEventListener('change', handler);
}

export function unsubscribeColorScheme(id) {
    const entry = _schemeListeners.get(id);
    if (entry) {
        entry.mq.removeEventListener('change', entry.handler);
        _schemeListeners.delete(id);
    }
}

// --- Outside-click dismiss (e.g. close an open Select when clicking elsewhere) ---
// A document-level listener (no overlay element) so the page keeps scrolling normally
// while the popup is open -- a fixed full-screen backdrop would trap the wheel.
const _outsideClick = new Map();

export function registerOutsideClick(id, element, dotNetRef, method) {
    removeOutsideClick(id);
    const handler = (e) => {
        if (element && !element.contains(e.target)) dotNetRef.invokeMethodAsync(method);
    };
    _outsideClick.set(id, handler);
    // Capture phase + pointerdown: fires before the click lands, for any pointer type.
    document.addEventListener('pointerdown', handler, true);
}

export function removeOutsideClick(id) {
    const h = _outsideClick.get(id);
    if (h) { document.removeEventListener('pointerdown', h, true); _outsideClick.delete(id); }
}

// -- Anchored fixed-position panel (Select / DatePicker / TimePicker / ColorPicker) --
// Positions a popup panel as position:fixed under (or above) its anchor element so it escapes
// any ancestor clipping context -- most notably a Card's overflow:hidden, which would otherwise
// crop the dropdown. Re-positions on scroll (capture phase, so nested scrollers count) and resize
// until removeAnchoredPanel(id) is called. Pass matchWidth:true to size the panel to the anchor.
const _anchoredPanels = new Map();

export function positionAnchoredPanel(id, anchor, panel, options) {
    removeAnchoredPanel(id);
    if (!anchor || !panel) return;
    const opts = options || {};
    const gap = opts.gap ?? 4;
    const margin = 4; // keep this far from the viewport edge

    const place = () => {
        const a = anchor.getBoundingClientRect();
        panel.style.position = 'fixed';
        panel.style.margin = '0';
        if (opts.matchWidth) panel.style.width = `${a.width}px`;
        const p = panel.getBoundingClientRect();
        const vh = window.innerHeight, vw = window.innerWidth;
        const below = vh - a.bottom, above = a.top;
        // Flip above only when there is not enough room below and more room above.
        let top = (below >= p.height + gap || below >= above)
            ? a.bottom + gap
            : a.top - p.height - gap;
        top = Math.max(margin, Math.min(top, vh - p.height - margin));
        const left = Math.max(margin, Math.min(a.left, vw - p.width - margin));
        panel.style.top = `${top}px`;
        panel.style.left = `${left}px`;
    };

    place();
    window.addEventListener('scroll', place, { passive: true, capture: true });
    window.addEventListener('resize', place, { passive: true });
    _anchoredPanels.set(id, () => {
        window.removeEventListener('scroll', place, { capture: true });
        window.removeEventListener('resize', place);
    });
}

export function removeAnchoredPanel(id) {
    const off = _anchoredPanels.get(id);
    if (off) { off(); _anchoredPanels.delete(id); }
}

// -- Breakpoint detection (FlareMediaQuery) -----------------------------------
// Boundaries mirror responsive.css: xs<600, sm>=600, md>=960, lg>=1280, xl>=1920.
const _bpListeners = new Map();

function currentBreakpoint() {
    const w = window.innerWidth;
    if (w >= 1920) return 'Xl';
    if (w >= 1280) return 'Lg';
    if (w >= 960)  return 'Md';
    if (w >= 600)  return 'Sm';
    return 'Xs';
}

export function getBreakpoint() {
    return currentBreakpoint();
}

export function subscribeBreakpoint(id, dotNetRef) {
    unsubscribeBreakpoint(id);
    let last = currentBreakpoint();
    const handler = () => {
        const bp = currentBreakpoint();
        if (bp !== last) {
            last = bp;
            dotNetRef.invokeMethodAsync('OnBreakpointChanged', bp);
        }
    };
    window.addEventListener('resize', handler, { passive: true });
    _bpListeners.set(id, handler);
    return last;
}

export function unsubscribeBreakpoint(id) {
    const handler = _bpListeners.get(id);
    if (handler) {
        window.removeEventListener('resize', handler);
        _bpListeners.delete(id);
    }
}

// -- FlareResizable ----------------------------------------------------------
const _resizeHandles = new Map();

export function registerResizeHandle(container, handle, edge, minSize, maxSize, dotNetRef) {
    if (!handle || !container) return;
    const key = handle;
    let dragging = false;
    let startPos = 0;
    let startSize = 0;

    function onPointerDown(e) {
        dragging = true;
        startPos = edge === 'right' || edge === 'left' ? e.clientX : e.clientY;
        const rect = container.getBoundingClientRect();
        startSize = edge === 'right' || edge === 'left' ? rect.width : rect.height;
        handle.setPointerCapture(e.pointerId);
        e.preventDefault();
    }

    function onPointerMove(e) {
        if (!dragging) return;
        const current = edge === 'right' || edge === 'left' ? e.clientX : e.clientY;
        let delta = edge === 'right' || edge === 'bottom' ? current - startPos : startPos - current;
        let newSize = Math.max(0, startSize + delta);
        if (minSize) newSize = Math.max(parseFloat(minSize), newSize);
        if (maxSize) newSize = Math.min(parseFloat(maxSize), newSize);
        const prop = edge === 'right' || edge === 'left' ? 'width' : 'height';
        container.style[prop] = newSize + 'px';
    }

    function onPointerUp(e) {
        if (!dragging) return;
        dragging = false;
        const prop = edge === 'right' || edge === 'left' ? 'width' : 'height';
        const finalSize = container.style[prop];
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnResizedCallback', finalSize);
    }

    handle.addEventListener('pointerdown', onPointerDown);
    handle.addEventListener('pointermove', onPointerMove);
    handle.addEventListener('pointerup', onPointerUp);
    _resizeHandles.set(key, { onPointerDown, onPointerMove, onPointerUp });
}

export function removeResizeHandle(handle) {
    const handlers = _resizeHandles.get(handle);
    if (handlers) {
        handle.removeEventListener('pointerdown', handlers.onPointerDown);
        handle.removeEventListener('pointermove', handlers.onPointerMove);
        handle.removeEventListener('pointerup', handlers.onPointerUp);
        _resizeHandles.delete(handle);
    }
}

// -- FlareSplitter (standalone handle that resizes its two flex siblings) -----
const _splitters = new Map();

function _flareParsePx(v) {
    if (!v) return 0;
    const n = parseFloat(String(v));
    return isNaN(n) ? 0 : n;
}

// Resolve the resize axis: explicit 'horizontal'/'vertical', or 'auto' from the parent flex direction.
function _flareSplitterHoriz(gutter, orientation) {
    if (orientation === 'horizontal') return true;
    if (orientation === 'vertical') return false;
    const dir = gutter.parentElement ? getComputedStyle(gutter.parentElement).flexDirection : 'row';
    return dir !== 'column' && dir !== 'column-reverse';
}

// Reflect the resolved axis on the element so the CSS handle (size/cursor) and aria are correct.
function _flareSplitterReflect(gutter, horiz) {
    gutter.classList.toggle('flare-splitter--vertical', !horiz);
    gutter.classList.toggle('flare-splitter--horizontal', horiz);
    gutter.setAttribute('aria-orientation', horiz ? 'vertical' : 'horizontal');
}

// Apply new sizes to the previous/next siblings, keeping their combined size constant.
function _flareApplySiblingSize(prev, next, horiz, newPrev, total, minPx, maxPx, dotNetRef) {
    const min = minPx || 0;
    let p = Math.max(min, Math.min(newPrev, total - min));
    if (maxPx) p = Math.min(p, maxPx);
    const n = total - p;
    const dim = horiz ? 'width' : 'height';
    prev.style.flex = '0 0 ' + p + 'px';
    prev.style[dim] = p + 'px';
    next.style.flex = '0 0 ' + n + 'px';
    next.style[dim] = n + 'px';
    if (dotNetRef) dotNetRef.invokeMethodAsync('OnSplitChanged', Math.round(p) + 'px');
}

export function registerSiblingSplitter(gutter, orientation, minSize, maxSize, dotNetRef) {
    if (!gutter) return;
    // Reflect the (possibly auto-detected) axis on the element so the handle renders correctly.
    if (orientation === 'auto') _flareSplitterReflect(gutter, _flareSplitterHoriz(gutter, 'auto'));

    function onDown(e) {
        const prev = gutter.previousElementSibling;
        const next = gutter.nextElementSibling;
        if (!prev || !next) return;
        const horiz = _flareSplitterHoriz(gutter, orientation); // re-resolve in case the layout changed
        e.preventDefault();
        const startPos = horiz ? e.clientX : e.clientY;
        const prevSize = horiz ? prev.getBoundingClientRect().width : prev.getBoundingClientRect().height;
        const nextSize = horiz ? next.getBoundingClientRect().width : next.getBoundingClientRect().height;
        const total = prevSize + nextSize;
        const min = _flareParsePx(minSize);
        const max = _flareParsePx(maxSize);

        const onMove = (ev) => {
            const pos = horiz ? ev.clientX : ev.clientY;
            _flareApplySiblingSize(prev, next, horiz, prevSize + (pos - startPos), total, min, max, dotNetRef);
        };
        const onUp = () => {
            document.removeEventListener('mousemove', onMove);
            document.removeEventListener('mouseup', onUp);
            document.body.style.cursor = '';
            document.body.style.userSelect = '';
        };
        document.body.style.cursor = horiz ? 'col-resize' : 'row-resize';
        document.body.style.userSelect = 'none';
        document.addEventListener('mousemove', onMove);
        document.addEventListener('mouseup', onUp);
    }

    gutter.addEventListener('mousedown', onDown);
    _splitters.set(gutter, onDown);
}

export function nudgeSiblingSplitter(gutter, orientation, deltaPx, keyAxis, minSize, maxSize, dotNetRef) {
    if (!gutter) return;
    const horiz = _flareSplitterHoriz(gutter, orientation);
    // Ignore arrow keys perpendicular to the resize axis.
    if ((horiz && keyAxis !== 'x') || (!horiz && keyAxis !== 'y')) return;
    const prev = gutter.previousElementSibling;
    const next = gutter.nextElementSibling;
    if (!prev || !next) return;
    const prevSize = horiz ? prev.getBoundingClientRect().width : prev.getBoundingClientRect().height;
    const nextSize = horiz ? next.getBoundingClientRect().width : next.getBoundingClientRect().height;
    const total = prevSize + nextSize;
    _flareApplySiblingSize(prev, next, horiz, prevSize + deltaPx, total,
        _flareParsePx(minSize), _flareParsePx(maxSize), dotNetRef);
}

export function removeSiblingSplitter(gutter) {
    const onDown = _splitters.get(gutter);
    if (onDown) {
        gutter.removeEventListener('mousedown', onDown);
        _splitters.delete(gutter);
    }
}

// -- FlareTabs overflow scroller --------------------------------------------
const _tabScrollers = new Map();

export function registerTabScroller(bar, dotNetRef) {
    if (!bar) return;
    function update() {
        const overflowing = bar.scrollWidth > bar.clientWidth + 1;
        const atStart = bar.scrollLeft <= 1;
        const atEnd = bar.scrollLeft + bar.clientWidth >= bar.scrollWidth - 1;
        if (dotNetRef) dotNetRef.invokeMethodAsync('OnTabScrollState', overflowing, atStart, atEnd);
    }
    const onScroll = () => update();
    const ro = new ResizeObserver(() => update());
    bar.addEventListener('scroll', onScroll, { passive: true });
    ro.observe(bar);
    _tabScrollers.set(bar, { onScroll, ro });
    update();
}

export function scrollTabs(bar, dir) {
    if (!bar) return;
    bar.scrollBy({ left: dir * bar.clientWidth * 0.8, behavior: 'smooth' });
}

export function removeTabScroller(bar) {
    const handlers = _tabScrollers.get(bar);
    if (handlers) {
        bar.removeEventListener('scroll', handlers.onScroll);
        handlers.ro.disconnect();
        _tabScrollers.delete(bar);
    }
}

// -- FlareShortcuts ----------------------------------------------------------
let _shortcutDotNetRef = null;

export function registerShortcutListener(dotNetRef) {
    _shortcutDotNetRef = dotNetRef;
    document.addEventListener('keydown', _handleShortcutKeyDown);
}

export function removeShortcutListener() {
    document.removeEventListener('keydown', _handleShortcutKeyDown);
    _shortcutDotNetRef = null;
}

function _handleShortcutKeyDown(e) {
    if (!_shortcutDotNetRef) return;
    const parts = [];
    if (e.ctrlKey || e.metaKey) parts.push('ctrl');
    if (e.altKey) parts.push('alt');
    if (e.shiftKey) parts.push('shift');
    const key = e.key.toLowerCase();
    if (key !== 'control' && key !== 'alt' && key !== 'shift' && key !== 'meta') parts.push(key);
    const combo = parts.join('+');
    _shortcutDotNetRef.invokeMethodAsync('HandleKeyDown', combo).catch(() => {});
}

// -- FlareCodeBlock syntax highlighter ---------------------------------------
// A small, dependency-free tokenizer. It replaces the old window.hljs reliance
// so FlareCodeBlock highlights autonomously (no host <script> needed). Output is
// wrapped in <span class="flare-hl-*"> elements coloured by Flare theme tokens
// in codeblock.css, so the syntax palette follows the active design system and
// light/dark mode. Covers the languages the library documents (csharp, razor,
// html, json, css, javascript); anything else falls back to a generic ruleset.

const _hlAlias = {
    cs: 'csharp', 'c#': 'csharp', csharp: 'csharp',
    razor: 'razor', cshtml: 'razor',
    html: 'html', xml: 'html', svg: 'html',
    js: 'javascript', javascript: 'javascript', ts: 'javascript', typescript: 'javascript',
    json: 'json', css: 'css', scss: 'css',
};

const _csKw = new Set('abstract as async await base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in init int interface internal is lock long nameof namespace new null object operator out override params private protected public readonly record ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using value var virtual void volatile when while with yield'.split(' '));
const _jsKw = new Set('abstract arguments async await break case catch class const continue debugger default delete do else export extends false finally for from function if import in instanceof let new null of return static super switch this throw true try typeof undefined var void while with yield'.split(' '));

function _esc(s) { return s.replace(/[&<>]/g, c => (c === '&' ? '&amp;' : c === '<' ? '&lt;' : '&gt;')); }

// Each rule: { c: tokenClass | null, re: sticky RegExp, id?: Set }. When `id` is set
// the matched identifier becomes a keyword (in set), a type (Capitalized) or plain.
function _hlRules(lang) {
    const comBlock = { c: 'comment', re: /\/\*[\s\S]*?\*\//y };
    const comLine = { c: 'comment', re: /\/\/[^\n]*/y };
    const num = { c: 'number', re: /\b\d[\d_]*(?:\.\d+)?(?:[eE][+-]?\d+)?\b/y };
    switch (lang) {
        case 'csharp': return [
            comLine, comBlock,
            { c: 'meta', re: /#\s*\w+/y },
            { c: 'string', re: /[$@]*"(?:""|\\.|[^"\\])*"/y },
            { c: 'string', re: /'(?:\\.|[^'\\])'/y },
            num,
            { id: _csKw, re: /[A-Za-z_]\w*/y },
        ];
        case 'json': return [
            { c: 'property', re: /"(?:\\.|[^"\\])*"(?=\s*:)/y },
            { c: 'string', re: /"(?:\\.|[^"\\])*"/y },
            { c: 'literal', re: /\b(?:true|false|null)\b/y },
            num,
        ];
        case 'css': return [
            comBlock,
            { c: 'meta', re: /@[\w-]+/y },
            { c: 'string', re: /"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/y },
            { c: 'number', re: /#[0-9a-fA-F]{3,8}\b/y },
            { c: 'number', re: /\b\d+(?:\.\d+)?(?:px|rem|em|%|vh|vw|s|ms|deg|fr|pt)?\b/y },
            { c: 'keyword', re: /![a-z]+/y },
            { c: 'property', re: /[A-Za-z-]+(?=\s*:)/y },
        ];
        case 'javascript': return [
            comLine, comBlock,
            { c: 'string', re: /`(?:\\.|[^`\\])*`/y },
            { c: 'string', re: /"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/y },
            num,
            { id: _jsKw, re: /[A-Za-z_$][\w$]*/y },
        ];
        case 'html': return [
            { c: 'comment', re: /<!--[\s\S]*?-->/y },
            { c: 'string', re: /"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/y },
            { c: 'tag', re: /<\/?[A-Za-z][\w-]*|\/?>/y },
            { c: 'attr', re: /[A-Za-z_:][\w:.-]*(?=\s*=)/y },
        ];
        case 'razor': return [
            { c: 'comment', re: /@\*[\s\S]*?\*@/y },
            { c: 'comment', re: /<!--[\s\S]*?-->/y },
            comLine,
            { c: 'string', re: /"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/y },
            { c: 'meta', re: /@[A-Za-z_]\w*/y },
            { c: 'tag', re: /<\/?[A-Za-z][\w-]*|\/?>/y },
            { c: 'attr', re: /[A-Za-z_:@][\w:.-]*(?=\s*=)/y },
            num,
            { id: _csKw, re: /[A-Za-z_]\w*/y },
        ];
        default: return [
            comLine, comBlock,
            { c: 'string', re: /"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'/y },
            num,
        ];
    }
}

function _highlight(code, lang) {
    const rules = _hlRules(_hlAlias[(lang || '').toLowerCase()] || 'plain');
    let out = '';
    for (let i = 0, n = code.length; i < n;) {
        let hit = false;
        for (const r of rules) {
            r.re.lastIndex = i;
            const m = r.re.exec(code);
            if (m && m.index === i && m[0].length) {
                const t = m[0];
                const cls = r.id
                    ? (r.id.has(t) ? 'keyword' : /^[A-Z]/.test(t) ? 'type' : null)
                    : r.c;
                out += cls ? `<span class="flare-hl-${cls}">${_esc(t)}</span>` : _esc(t);
                i = r.re.lastIndex;
                hit = true;
                break;
            }
        }
        if (!hit) { out += _esc(code[i]); i++; }
    }
    return out;
}

export function getHighlightedHtml(code, lang) {
    try { return _highlight(code ?? '', lang); }
    catch { return null; } // null -> component keeps its own plain-text encoding
}

// -- FlareColorPicker --------------------------------------------------------

export const flareColorPicker = (() => {
    const _state = new Map(); // canvas el -> { dotNetRef, hue, isDragging }

    function _drawCanvas(canvas, hue) {
        const ctx = canvas.getContext('2d');
        const w = canvas.width, h = canvas.height;

        // Saturation-Lightness gradient for the given hue
        // Base: pure hue colour (sat=100, l=50 in HSL)
        const hueColor = `hsl(${hue},100%,50%)`;

        // White -> hue (left-to-right saturation)
        const gradH = ctx.createLinearGradient(0, 0, w, 0);
        gradH.addColorStop(0, '#fff');
        gradH.addColorStop(1, hueColor);
        ctx.fillStyle = gradH;
        ctx.fillRect(0, 0, w, h);

        // Transparent -> black (top-to-bottom lightness)
        const gradV = ctx.createLinearGradient(0, 0, 0, h);
        gradV.addColorStop(0, 'rgba(0,0,0,0)');
        gradV.addColorStop(1, '#000');
        ctx.fillStyle = gradV;
        ctx.fillRect(0, 0, w, h);
    }

    function _drawCrosshair(canvas, sat, l) {
        // sat 0-100 -> x; lightness 0-100 -> y (inverted: 100=top white, 0=bottom black)
        // In canvas space: x = sat%, y = (100-l)% mapped to [0, height]
        // but HSL lightness at sat=100 goes from white(top-left) to black(bottom)
        // so treat x as saturation and y as (100 - lightness) approximately
        const ctx = canvas.getContext('2d');
        const w = canvas.width, h = canvas.height;

        // Derived x/y from sat and lightness:
        // At sat=0: always white regardless of lightness -> x=0
        // At l=100: white (top of canvas) -> y=0
        // At l=0:  black (bottom) -> y=h
        // x = sat / 100 * w, y = (1 - l/100) * h  (approximate for the gradient used)
        const x = (sat / 100) * w;
        const y = (1 - l / 100) * h;

        ctx.save();
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.strokeStyle = '#fff';
        ctx.lineWidth = 2;
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(x, y, 6, 0, Math.PI * 2);
        ctx.strokeStyle = 'rgba(0,0,0,0.4)';
        ctx.lineWidth = 1;
        ctx.stroke();
        ctx.restore();
    }

    function _render(canvas) {
        const s = _state.get(canvas);
        if (!s) return;
        _drawCanvas(canvas, s.hue);
        _drawCrosshair(canvas, s.sat, s.l);
    }

    function _pickFromEvent(canvas, e) {
        const rect = canvas.getBoundingClientRect();
        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;
        const x = Math.max(0, Math.min(clientX - rect.left, rect.width));
        const y = Math.max(0, Math.min(clientY - rect.top, rect.height));
        const sat = (x / rect.width) * 100;
        const l   = (1 - y / rect.height) * 100;
        return { sat, l };
    }

    return {
        init(canvas, dotNetRef, hue, sat, l) {
            if (!canvas) return;
            const existing = _state.get(canvas);
            if (existing) {
                existing.dotNetRef = dotNetRef;
                existing.hue = hue;
                existing.sat = sat;
                existing.l = l;
                _render(canvas);
                return;
            }

            const state = { dotNetRef, hue, sat, l, isDragging: false };
            _state.set(canvas, state);

            function pick(e) {
                const { sat: ns, l: nl } = _pickFromEvent(canvas, e);
                state.sat = ns; state.l = nl;
                _render(canvas);
                dotNetRef.invokeMethodAsync('OnCanvasPick', ns, nl).catch(() => {});
            }

            function onDown(e) { state.isDragging = true; pick(e); e.preventDefault(); }
            function onMove(e) { if (state.isDragging) { pick(e); e.preventDefault(); } }
            function onUp()   { state.isDragging = false; }

            canvas.addEventListener('mousedown', onDown);
            canvas.addEventListener('mousemove', onMove);
            window.addEventListener('mouseup', onUp);
            canvas.addEventListener('touchstart', onDown, { passive: false });
            canvas.addEventListener('touchmove',  onMove, { passive: false });
            window.addEventListener('touchend',  onUp);

            state._cleanup = () => {
                canvas.removeEventListener('mousedown', onDown);
                canvas.removeEventListener('mousemove', onMove);
                window.removeEventListener('mouseup', onUp);
                canvas.removeEventListener('touchstart', onDown);
                canvas.removeEventListener('touchmove',  onMove);
                window.removeEventListener('touchend',  onUp);
            };

            _render(canvas);
        },

        setHue(canvas, hue) {
            const s = _state.get(canvas);
            if (!s) return;
            s.hue = hue;
            _render(canvas);
        },

        setSatL(canvas, sat, l) {
            const s = _state.get(canvas);
            if (!s) return;
            s.sat = sat; s.l = l;
            _render(canvas);
        },

        destroy(canvas) {
            const s = _state.get(canvas);
            if (s) { s._cleanup?.(); _state.delete(canvas); }
        }
    };
})();

// -- Kanban touch drag --------------------------------------------------------

export function getKanbanColumnAtPoint(x, y, attr) {
    const el = document.elementFromPoint(x, y);
    if (!el) return null;
    return el.closest(`[data-kanban-column]`)?.dataset?.kanbanColumn ?? null;
}

// -- FlareTreeItem drag-and-drop zone ----------------------------------------
// DragEventArgs in Blazor exposes ClientY but not the target element's bounds, so
// the drop indicator (before/inside/after) cannot be computed in C# alone. Given a
// row element and the cursor's clientY, return which third of the row the cursor is
// over: top ~25% -> 'before', bottom ~25% -> 'after', middle -> 'inside'.
export function getDropZone(rowEl, clientY) {
    if (!rowEl) return 'inside';
    const rect = rowEl.getBoundingClientRect();
    if (rect.height <= 0) return 'inside';
    const offset = clientY - rect.top;
    if (offset < rect.height * 0.25) return 'before';
    if (offset > rect.height * 0.75) return 'after';
    return 'inside';
}

// -- EyeDropper API ----------------------------------------------------------

export function supportsEyeDropper() {
    return 'EyeDropper' in window;
}

export async function openEyeDropper() {
    if (!('EyeDropper' in window)) return null;
    try {
        const result = await new window.EyeDropper().open();
        return result.sRGBHex || null;
    } catch {
        return null;
    }
}

// -- Code editor key handling (Tab indent + bracket auto-close) ---------------

const _editorKeyHandlers = new WeakMap();
const _editorPairs = { '(': ')', '[': ']', '{': '}', '"': '"', "'": "'", '`': '`' };

export function enableCodeEditorKeys(textareaEl, indentSize) {
    if (!textareaEl || _editorKeyHandlers.has(textareaEl)) return;
    const indent = ' '.repeat(indentSize > 0 ? indentSize : 2);

    function setValue(value, caret) {
        textareaEl.value = value;
        textareaEl.selectionStart = textareaEl.selectionEnd = caret;
        // Notify Blazor's @oninput so Value stays in sync.
        textareaEl.dispatchEvent(new Event('input', { bubbles: true }));
    }

    function onKeyDown(e) {
        const start = textareaEl.selectionStart;
        const end = textareaEl.selectionEnd;
        const val = textareaEl.value;

        if (e.key === 'Tab') {
            e.preventDefault();
            setValue(val.slice(0, start) + indent + val.slice(end), start + indent.length);
            return;
        }

        const close = _editorPairs[e.key];
        if (close && start === end) {
            e.preventDefault();
            setValue(val.slice(0, start) + e.key + close + val.slice(end), start + 1);
            return;
        }

        // Type-over a matching closing char instead of inserting a duplicate.
        if ((e.key === ')' || e.key === ']' || e.key === '}') && start === end && val[start] === e.key) {
            e.preventDefault();
            textareaEl.selectionStart = textareaEl.selectionEnd = start + 1;
        }
    }

    textareaEl.addEventListener('keydown', onKeyDown);
    _editorKeyHandlers.set(textareaEl, onKeyDown);
}

export function disableCodeEditorKeys(textareaEl) {
    if (!textareaEl) return;
    const handler = _editorKeyHandlers.get(textareaEl);
    if (handler) {
        textareaEl.removeEventListener('keydown', handler);
        _editorKeyHandlers.delete(textareaEl);
    }
}
