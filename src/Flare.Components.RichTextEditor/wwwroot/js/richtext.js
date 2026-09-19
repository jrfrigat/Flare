import { setSanitizedHtml, sanitizeToString } from "./richtext-sanitize.js";

const editors = new Map();

// contentEditable is NOT set here. The component renders it as an attribute, so it is correct on the
// first paint and follows its ReadOnly parameter without this module being told twice. Setting it here
// as well would overrule that and make a read-only editor editable again.
export function init(editorId, dotNetRef) {
    const el = document.getElementById(editorId);
    if (!el) return;
    editors.set(editorId, { el, dotNetRef });
    el.addEventListener('input', () => {
        dotNetRef.invokeMethodAsync('OnContentChanged', el.innerHTML);
    });

    // Paste is the third way markup enters, beside the initial value and an external update, and the
    // only one the component never sees before the browser has it. Taking it over is what keeps the
    // contract whole: the default paste drops the clipboard HTML straight into the live tree.
    el.addEventListener("paste", e => {
        const data = e.clipboardData;
        if (!data) return;
        const html = data.getData("text/html");
        if (!html) return;                     // plain text needs no filtering

        e.preventDefault();
        const clean = sanitizeToString(html);
        const selection = window.getSelection();
        if (!selection || selection.rangeCount === 0) return;

        const range = selection.getRangeAt(0);
        range.deleteContents();
        const holder = document.createElement("div");
        holder.innerHTML = clean;              // sanitized on the line above
        const fragment = document.createDocumentFragment();
        fragment.append(...holder.childNodes);
        const last = fragment.lastChild;
        range.insertNode(fragment);
        if (last) {
            range.setStartAfter(last);
            range.collapse(true);
            selection.removeAllRanges();
            selection.addRange(range);
        }
        dotNetRef.invokeMethodAsync("OnContentChanged", el.innerHTML);
    });
}

export function execCommand(editorId, command, value = null) {
    const entry = editors.get(editorId);
    if (!entry) return;
    entry.el.focus();
    const sel = window.getSelection();
    const range = sel?.rangeCount ? sel.getRangeAt(0) : null;
    switch (command) {
        case 'bold': toggleInlineStyle(range, 'font-weight', 'bold', ''); break;
        case 'italic': toggleInlineStyle(range, 'font-style', 'italic', ''); break;
        case 'underline': toggleInlineStyle(range, 'text-decoration', 'underline', ''); break;
        case 'insertLink': wrapWithLink(range, value); break;
        case 'insertUnorderedList': wrapBlockElement(entry.el, range, 'ul'); break;
        case 'insertOrderedList': wrapBlockElement(entry.el, range, 'ol'); break;
        case 'formatBlock': if (value) wrapAsBlock(range, value); break;
        default: break;
    }
    entry.dotNetRef.invokeMethodAsync('OnContentChanged', entry.el.innerHTML);
}

export function getContent(editorId) {
    return editors.get(editorId)?.el?.innerHTML ?? '';
}

// The one door for markup arriving from C#. It never assigns innerHTML: Value is public API, so what a
// caller binds to it - a stored draft, a server response - is untrusted by construction.
export function setContent(editorId, html) {
    const entry = editors.get(editorId);
    if (entry) setSanitizedHtml(entry.el, html);
}

// The opt-out, for a caller that has stated its markup is trusted and wants it through untouched.
export function setContentUnsafe(editorId, html) {
    const entry = editors.get(editorId);
    if (entry) entry.el.innerHTML = html ?? "";
}

export function destroy(editorId) {
    editors.delete(editorId);
}

function toggleInlineStyle(range, prop, onValue, offValue) {
    if (!range || range.collapsed) return;
    const span = document.createElement('span');
    span.style[prop] = onValue;
    try { range.surroundContents(span); } catch {}
}

function wrapWithLink(range, url) {
    if (!range || range.collapsed || !url) return;
    const a = document.createElement('a');
    a.href = url;
    a.target = '_blank';
    try { range.surroundContents(a); } catch {}
}

function wrapAsBlock(range, tag) {
    if (!range) return;
    const block = document.createElement(tag);
    block.appendChild(range.collapsed ? document.createTextNode('​') : range.extractContents());
    range.insertNode(block);
}

function wrapBlockElement(editorEl, range, tag) {
    if (!range) return;
    const list = document.createElement(tag);
    const li = document.createElement('li');
    if (!range.collapsed) {
        li.appendChild(range.extractContents());
    }
    list.appendChild(li);
    range.insertNode(list);
}
