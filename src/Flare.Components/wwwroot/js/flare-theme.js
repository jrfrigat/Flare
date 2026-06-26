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
