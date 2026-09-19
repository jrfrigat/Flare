/* HTML sanitizing for the rich text editor.

   Sanitizing happens HERE, in the browser, on the parsed tree - not in C# on a string. A string-level
   sanitizer is guessing at what a browser will do with the markup, and browsers repair malformed HTML
   in ways no pattern predicts: an unclosed attribute, a stray `<`, a tag the pattern did not expect to
   nest. The parser is the only thing that knows the tree that will actually exist, so the allowlist is
   applied to that tree.

   Two implementations, in order of preference:

   1. `Element.setHTML()` - the platform's own sanitizer. It parses and filters in one step inside the
      engine, so there is no window in which a dangerous node exists, and nothing about string shape can
      fool it. Used whenever the engine has it.
   2. A DOMParser walk. `DOMParser.parseFromString(html, 'text/html')` produces an INERT document:
      scripts do not run, `<img onerror>` does not fire, resources are not fetched. That inertness is
      what makes walking it safe - the nodes are examined before they ever belong to a live document.

   What survives is what a rich text editor legitimately produces: formatting, lists, headings, quotes,
   links and images. Everything else, and every attribute not named below, is dropped. */

const ALLOWED_TAGS = new Set([
    'p', 'div', 'br', 'span',
    'b', 'strong', 'i', 'em', 'u', 's', 'strike', 'del', 'ins', 'sub', 'sup', 'mark', 'small',
    'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
    'ul', 'ol', 'li',
    'blockquote', 'pre', 'code',
    'a', 'img',
    'table', 'thead', 'tbody', 'tfoot', 'tr', 'th', 'td', 'caption', 'colgroup', 'col',
    'hr',
]);

// Per tag, on top of the global ones. An attribute not listed anywhere is removed.
const GLOBAL_ATTRS = new Set(['style', 'title', 'dir', 'lang']);
const TAG_ATTRS = {
    a: new Set(['href', 'target', 'rel']),
    img: new Set(['src', 'alt', 'width', 'height']),
    td: new Set(['colspan', 'rowspan']),
    th: new Set(['colspan', 'rowspan', 'scope']),
    col: new Set(['span']),
    colgroup: new Set(['span']),
    ol: new Set(['start', 'type']),
};

// Schemes a link or an image may point at. Everything else - javascript:, vbscript:, data: on an
// anchor, and any scheme an engine happens to support - becomes nothing at all rather than
// about:blank, so a stripped link cannot be mistaken for a working one.
const SAFE_LINK_SCHEMES = new Set(['http:', 'https:', 'mailto:', 'tel:', 'ftp:']);
const SAFE_IMAGE_SCHEMES = new Set(['http:', 'https:']);

// CSS that reaches outside the declaration it sits in: a fetch, a legacy script hook, a binding.
const DANGEROUS_CSS = /(expression\s*\(|url\s*\(|-moz-binding|behavior\s*:|@import)/i;

// `setHTML` takes the same allowlist, so both paths agree on what survives.
const SANITIZER_CONFIG = {
    elements: [...ALLOWED_TAGS],
    attributes: [
        ...[...GLOBAL_ATTRS].map(name => ({ name })),
        ...Object.entries(TAG_ATTRS).flatMap(([tag, attrs]) =>
            [...attrs].map(name => ({ name, elements: [tag] }))),
    ],
};

function schemeOf(value, base) {
    try { return new URL(value, base ?? document.baseURI).protocol.toLowerCase(); }
    catch { return null; }
}

function scrubAttributes(el) {
    const tag = el.tagName.toLowerCase();
    const allowed = TAG_ATTRS[tag];

    for (const attr of [...el.attributes]) {
        const name = attr.name.toLowerCase();

        // Every on* handler, and anything not on a list. Checking the list rather than hunting for
        // known-bad names is what makes a handler spelled in an unexpected way still fail to survive.
        if (!GLOBAL_ATTRS.has(name) && !(allowed && allowed.has(name))) {
            el.removeAttribute(attr.name);
            continue;
        }

        if (name === 'style' && DANGEROUS_CSS.test(attr.value)) {
            el.removeAttribute(attr.name);
            continue;
        }

        if (name === 'href' || name === 'src') {
            const schemes = tag === 'img' ? SAFE_IMAGE_SCHEMES : SAFE_LINK_SCHEMES;
            if (!schemes.has(schemeOf(attr.value))) el.removeAttribute(attr.name);
        }
    }

    // A link that opens a new context must not hand it a live opener reference.
    if (tag === 'a' && el.getAttribute('target') === '_blank') {
        el.setAttribute('rel', 'noopener noreferrer');
    }
}

/* Walks an inert tree and returns a DocumentFragment of what survived.

   A disallowed ELEMENT is unwrapped rather than deleted - its text and its allowed descendants stay -
   because a caller pasting a paragraph inside an unknown wrapper means the paragraph, not nothing.
   A disallowed NODE TYPE (comment, processing instruction) is dropped: it carries no reading. */
function keepAllowed(node, out) {
    for (const child of [...node.childNodes]) {
        if (child.nodeType === Node.TEXT_NODE) {
            out.appendChild(child.cloneNode(false));
            continue;
        }
        if (child.nodeType !== Node.ELEMENT_NODE) continue;   // comments and the rest

        const tag = child.tagName.toLowerCase();
        // script/style carry no reading at all, so unwrapping them would paste their source as text.
        if (tag === 'script' || tag === 'style' || tag === 'template' || tag === 'noscript') continue;

        if (!ALLOWED_TAGS.has(tag)) {
            keepAllowed(child, out);          // unwrap: keep what is inside
            continue;
        }

        const clone = child.cloneNode(false);
        scrubAttributes(clone);
        keepAllowed(child, clone);
        out.appendChild(clone);
    }
    return out;
}

/** Replaces the contents of `el` with a sanitized rendering of `html`. */
export function setSanitizedHtml(el, html) {
    const markup = html ?? '';

    // The platform's own sanitizer: it never materialises a dangerous node at all.
    if (typeof el.setHTML === 'function') {
        try { el.setHTML(markup, { sanitizer: SANITIZER_CONFIG }); return; }
        catch { /* older signature or a rejected config - fall through to the walk */ }
    }

    // Inert: parsing here runs no script and fires no error handler.
    const doc = new DOMParser().parseFromString(markup, 'text/html');
    const fragment = keepAllowed(doc.body, doc.createDocumentFragment());

    el.replaceChildren(...document.importNode(fragment, true).childNodes);
}

/** Sanitizes `html` and returns it as a string, for a paste that is inserted at the caret. */
export function sanitizeToString(html) {
    const holder = document.createElement('div');
    setSanitizedHtml(holder, html);
    return holder.innerHTML;
}
