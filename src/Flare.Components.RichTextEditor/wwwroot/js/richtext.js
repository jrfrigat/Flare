const editors = new Map();

export function init(editorId, dotNetRef) {
    const el = document.getElementById(editorId);
    if (!el) return;
    el.contentEditable = 'true';
    editors.set(editorId, { el, dotNetRef });
    el.addEventListener('input', () => {
        dotNetRef.invokeMethodAsync('OnContentChanged', el.innerHTML);
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
    dotNetRef.invokeMethodAsync('OnContentChanged', entry.el.innerHTML);
}

export function getContent(editorId) {
    return editors.get(editorId)?.el?.innerHTML ?? '';
}

export function setContent(editorId, html) {
    const entry = editors.get(editorId);
    if (entry) entry.el.innerHTML = html;
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
