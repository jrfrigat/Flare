// Flare code support for FlareCodeBlock: a dependency-free syntax highlighter plus code-editor key
// handling (Tab indent + bracket auto-close). Extracted from the former flare-theme.js god-module.

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

// -- Caret reporting, for a suggestion list anchored to the caret -------------

/** Where the caret is, and where on screen it sits relative to the editor's own box. */
export function getCaretInfo(textareaEl) {
    if (!textareaEl) return null;
    const start = textareaEl.selectionStart;

    // A mirror of the textarea, laid out identically, is the only reliable way to find where a
    // character sits: a textarea exposes an offset but never a position.
    const style = window.getComputedStyle(textareaEl);
    const mirror = document.createElement('div');
    for (const name of [
        'fontFamily', 'fontSize', 'fontWeight', 'fontStyle', 'letterSpacing', 'lineHeight',
        'textTransform', 'wordSpacing', 'textIndent', 'whiteSpace', 'tabSize',
        'paddingTop', 'paddingRight', 'paddingBottom', 'paddingLeft',
        'borderTopWidth', 'borderRightWidth', 'borderBottomWidth', 'borderLeftWidth', 'boxSizing']) {
        mirror.style[name] = style[name];
    }
    mirror.style.position = 'absolute';
    mirror.style.visibility = 'hidden';
    mirror.style.width = style.width;
    // Wrapping is copied from the textarea rather than assumed. Measuring a wrapped mirror against
    // an unwrapped textarea reports the caret a line or more away from where it actually blinks.
    mirror.style.whiteSpace = style.whiteSpace;
    mirror.style.overflowWrap = style.overflowWrap;
    mirror.style.wordBreak = style.wordBreak;

    mirror.textContent = textareaEl.value.slice(0, start);
    const anchor = document.createElement('span');
    // Holds whatever follows the caret, so the span begins exactly at it. A full stop stands in when
    // nothing follows, because an empty span has no position to read.
    anchor.textContent = textareaEl.value.slice(start) || '.';
    mirror.appendChild(anchor);
    document.body.appendChild(mirror);

    const left = anchor.offsetLeft - textareaEl.scrollLeft;
    const top = anchor.offsetTop - textareaEl.scrollTop;
    const lineHeight = parseFloat(style.lineHeight) || parseFloat(style.fontSize) * 1.4;
    document.body.removeChild(mirror);

    return { start, end: textareaEl.selectionEnd, left, top, lineHeight };
}

/** Puts the caret at an offset and gives the textarea focus, after text was replaced. */
export function setCaret(textareaEl, offset) {
    if (!textareaEl) return;
    textareaEl.focus();
    textareaEl.selectionStart = textareaEl.selectionEnd = offset;
}

/**
 * Replaces the text, keeping the caret where it was.
 *
 * The editor never renders its own text back into the DOM, because assigning a textarea's value
 * sends the caret to the end - which is why typing in the middle used to jump. Text coming from
 * anywhere other than the person typing arrives through here instead.
 */
export function setText(textareaEl, text) {
    if (!textareaEl || textareaEl.value === text) return;
    const caret = Math.min(textareaEl.selectionStart, text.length);
    textareaEl.value = text;
    textareaEl.selectionStart = textareaEl.selectionEnd = caret;
}

// -- Keeping the layers on top of one another --------------------------------

const _editorSyncHandlers = new WeakMap();

/**
 * Makes the coloured copy, the underline layer and the line numbers follow the textarea's scroll.
 *
 * Only the textarea scrolls; the rest are moved to match. Left to themselves they would each scroll
 * separately, and the colours would drift away from the characters they belong to as soon as a line
 * ran past the right edge.
 *
 * Done here rather than through an event handler in C# because this runs on every scrolled frame,
 * and a round trip per frame would be felt.
 */
export function attachEditorSync(textareaEl) {
    if (!textareaEl || _editorSyncHandlers.has(textareaEl)) return;
    const root = textareaEl.parentElement;
    if (!root) return;

    function sync() {
        const top = textareaEl.scrollTop;
        const left = textareaEl.scrollLeft;
        for (const selector of ['.flare-codeblock__highlight', '.flare-codeblock__markers']) {
            const layer = root.querySelector(selector);
            if (layer) {
                layer.scrollTop = top;
                layer.scrollLeft = left;
            }
        }
        // The numbers travel down with their lines but never sideways: the gutter is pinned to the
        // left edge and the text slides underneath it.
        const gutter = root.querySelector('.flare-codeblock__linenumbers');
        if (gutter) gutter.scrollTop = top;
    }

    textareaEl.addEventListener('scroll', sync);
    // Typing can move the text under a fixed scroll offset, so the layers are squared up then too.
    textareaEl.addEventListener('input', sync);
    _editorSyncHandlers.set(textareaEl, sync);
    sync();
}

/** Squares the layers up now, after something other than typing replaced the text. */
export function syncEditorScroll(textareaEl) {
    const sync = textareaEl ? _editorSyncHandlers.get(textareaEl) : null;
    if (sync) sync();
}

export function detachEditorSync(textareaEl) {
    if (!textareaEl) return;
    const sync = _editorSyncHandlers.get(textareaEl);
    if (sync) {
        textareaEl.removeEventListener('scroll', sync);
        textareaEl.removeEventListener('input', sync);
        _editorSyncHandlers.delete(textareaEl);
    }
}
