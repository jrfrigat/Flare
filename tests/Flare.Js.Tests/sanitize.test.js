import { describe, it, expect, beforeEach } from 'vitest';
import { setSanitizedHtml, sanitizeToString } from
  '../../src/Flare.Components.RichTextEditor/wwwroot/js/richtext-sanitize.js';

/* The rich text editor's security boundary.
 *
 * `Value` is public API, so a stored draft or a server response arrives untrusted; it used to be
 * assigned straight to innerHTML, which made an editor whose content survives a round trip through
 * storage re-run whatever was smuggled into it on every later view.
 *
 * WHICH PATH THIS COVERS. `setSanitizedHtml` prefers `Element.setHTML()` - the platform's own
 * sanitizer - and falls back to walking an inert DOMParser tree. jsdom implements no `setHTML`, so
 * everything below exercises the FALLBACK. That is the path worth covering here anyway: the platform
 * one is the engine's own code and is not ours to regress, while the walk is. The last block asserts
 * the preference itself, so a change that stopped reaching for the platform sanitizer still fails.
 *
 * A case that "passes" because the markup was mangled into nothing is not proof, so the attacks assert
 * on what is absent (the handler, the scheme, the tag) and the legitimate markup asserts on what
 * survives, byte for byte.
 */

const clean = html => {
  const el = document.createElement('div');
  setSanitizedHtml(el, html);
  return el.innerHTML;
};

/* Every event-handler attribute left anywhere in the result.
 *
 * Structural rather than a regex over the string, and the difference is not pedantry: `on\w+=` also
 * matches inside an ATTRIBUTE VALUE, where it is text and not a handler. The mXSS case below ends as
 * `<p title="...onerror=alert(1)...">` - a tooltip string, nothing executable - and a string match
 * calls that a failure. What matters is whether any ELEMENT carries such an attribute, so that is
 * what this asks. */
const handlersIn = html => {
  const holder = document.createElement('div');
  setSanitizedHtml(holder, html);
  return [...holder.querySelectorAll('*')]
    .flatMap(el => [...el.attributes].map(a => a.name))
    .filter(name => /^on/i.test(name));
};

beforeEach(() => { document.body.innerHTML = ''; });

describe('attacks are neutralised', () => {
  it.each([
    ['script tag',            '<p>a</p><script>alert(1)<\/script>'],
    ['img onerror',           '<img src=x onerror="alert(1)">'],
    ['img ONERROR upper',     '<img src=x ONERROR="alert(1)">'],
    ['img onerror newline',   '<img src=x\nonerror="alert(1)">'],
    ['svg onload',            '<svg onload="alert(1)"><circle r="1"/></svg>'],
    ['body onload',           '<body onload=alert(1)>hi</body>'],
    ['iframe srcdoc',         '<iframe srcdoc="<script>alert(1)<\/script>"></iframe>'],
    ['details ontoggle',      '<details ontoggle="alert(1)" open>x</details>'],
    ['unclosed attribute',    '<img src="x" onerror="alert(1)>'],
    ['malformed nesting',     '<div><p>text<img src=x onerror=alert(1)></div>'],
    ['math mtext',            '<math><mtext><img src=x onerror=alert(1)></mtext></math>'],
    ['noscript mXSS',         '<noscript><p title="</noscript><img src=x onerror=alert(1)>">'],
    ['style onload',          '<style onload="alert(1)">p{}</style>'],

    /* Handlers on tags that SURVIVE. These are the cases that actually exercise the attribute filter:
     * everything above either uses onerror or sits on a tag the walk unwraps anyway, so a filter that
     * only knew the name "onerror" passed all of them. It did - measured - until these were added. */
    ['onclick on a p',        '<p onclick="alert(1)">x</p>'],
    ['onmouseover on a link', '<a href="https://example.com" onmouseover="alert(1)">x</a>'],
    ['onload on an image',    '<img src="https://example.com/a.png" onload="alert(1)">'],
    ['onfocus on a span',     '<span onfocus="alert(1)" tabindex="0">x</span>'],
    ['onanimationend on li',  '<ul><li onanimationend="alert(1)">x</li></ul>'],
    ['ondblclick on td',      '<table><tr><td ondblclick="alert(1)">x</td></tr></table>'],
    ['a handler nobody lists','<p oninvented="alert(1)">x</p>'],
  ])('drops every handler: %s', (_name, html) => {
    expect(handlersIn(html)).toEqual([]);
  });

  /* The filter works from an allowlist, not a list of known-bad names. An attribute nobody named is
   * dropped whether or not it looks dangerous - which is what makes a handler spelled in a way this
   * suite never thought of still fail to survive. */
  it.each([
    ['formaction', '<p formaction="https://evil.example">x</p>'],
    ['srcset',     '<img src="https://example.com/a.png" srcset="https://evil.example 2x">'],
    ['ping',       '<a href="https://example.com" ping="https://evil.example">x</a>'],
    ['is',         '<p is="evil-element">x</p>'],
    ['data-*',     '<p data-anything="x">x</p>'],
  ])('drops an attribute no list names: %s', (name, html) => {
    const out = clean(html);
    expect(out.toLowerCase()).not.toContain(name.replace('-*', ''));
  });

  it.each([
    ['javascript:',       '<a href="javascript:alert(1)">x</a>'],
    ['JaVaScRiPt: mixed', '<a href="JaVaScRiPt:alert(1)">x</a>'],
    ['tab inside scheme', '<a href="java\tscript:alert(1)">x</a>'],
    ['newline in scheme', '<a href="java\nscript:alert(1)">x</a>'],
    ['leading spaces',    '<a href="   javascript:alert(1)">x</a>'],
    ['vbscript:',         '<a href="vbscript:msgbox(1)">x</a>'],
    ['data: on an anchor','<a href="data:text/html,<script>alert(1)<\/script>">x</a>'],
  ])('strips the href but keeps the text: %s', (_name, html) => {
    const out = clean(html);
    expect(out).not.toMatch(/href=/i);
    expect(out).toContain('x');          // the link text is reading, and reading is kept
  });

  it('drops a data: image source, which is not a fetchable scheme for us', () => {
    expect(clean('<img src="data:image/svg+xml,<svg onload=alert(1)>">')).not.toMatch(/src=/i);
  });

  it.each([
    ['iframe', '<iframe src="https://example.com"></iframe>'],
    ['object', '<object data="x"></object>'],
    ['embed',  '<embed src="y">'],
    ['form',   '<form action="/post"><input name="a"></form>'],
    ['meta',   '<meta http-equiv="refresh" content="0;url=https://example.com">'],
    ['base',   '<base href="https://evil.example">'],
    ['link',   '<link rel="stylesheet" href="https://evil.example/x.css">'],
  ])('removes the tag entirely: %s', (name, html) => {
    expect(clean(html).toLowerCase()).not.toContain('<' + name);
  });

  it.each([
    ['url()',        '<p style="background:url(javascript:alert(1))">x</p>'],
    ['expression()', '<p style="width:expression(alert(1))">x</p>'],
    ['-moz-binding', '<p style="-moz-binding:url(http://evil.example/x.xml)">x</p>'],
    ['@import',      '<p style="@import url(http://evil.example)">x</p>'],
  ])('drops the whole style when it reaches outside itself: %s', (_name, html) => {
    const out = clean(html);
    expect(out).not.toMatch(/style=/i);
    expect(out).toContain('x');
  });

  it('keeps no script text as visible content when unwrapping', () => {
    // script/style are removed rather than unwrapped: unwrapping would paste their source as prose.
    expect(clean('<div><script>alert(1)<\/script></div>')).not.toContain('alert');
  });
});

describe('legitimate markup survives', () => {
  it.each([
    ['inline formatting', '<p><strong>bold</strong> and <em>it</em> and <u>u</u></p>'],
    ['unordered list',    '<ul><li>one</li><li>two</li></ul>'],
    ['ordered list',      '<ol><li>one</li></ol>'],
    ['heading',           '<h2>Title</h2>'],
    ['blockquote',        '<blockquote>quoted</blockquote>'],
    ['the editor style',  '<span style="font-weight:bold">b</span>'],
    ['https image',       '<img src="https://example.com/a.png" alt="a">'],
  ])('unchanged: %s', (_name, html) => {
    expect(clean(html)).toBe(html);
  });

  it('keeps an https link and its text', () => {
    expect(clean('<a href="https://example.com">go</a>')).toBe('<a href="https://example.com">go</a>');
  });

  it.each(['mailto:someone@example.com', 'tel:+123', 'ftp://example.com/f'])(
    'keeps the %s scheme', href => {
      expect(clean(`<a href="${href}">x</a>`)).toContain(`href="${href}"`);
    });

  it('gives a new-context link a rel that severs the opener', () => {
    const out = clean('<a href="https://example.com" target="_blank">go</a>');
    expect(out).toContain('rel="noopener noreferrer"');
  });

  it('unwraps an unknown element rather than deleting what it holds', () => {
    // A caller pasting a paragraph inside an unknown wrapper means the paragraph, not nothing.
    expect(clean('<article><p>kept</p></article>')).toBe('<p>kept</p>');
  });

  it('handles empty and nullish input', () => {
    expect(clean('')).toBe('');
    expect(clean(null)).toBe('');
    expect(clean(undefined)).toBe('');
  });
});

describe('sanitizeToString, the paste path', () => {
  it('filters the same way as the element path', () => {
    expect(sanitizeToString('<img src=x onerror="alert(1)">')).not.toMatch(/on\w+=/i);
  });

  it('keeps pasted formatting', () => {
    expect(sanitizeToString('<p><strong>b</strong></p>')).toBe('<p><strong>b</strong></p>');
  });
});

describe('the platform sanitizer is preferred when the engine has one', () => {
  it('calls setHTML and does not fall through to the walk', () => {
    const el = document.createElement('div');
    let calledWith = null;
    el.setHTML = (html, options) => { calledWith = { html, options }; el.textContent = 'from-setHTML'; };

    setSanitizedHtml(el, '<p>x</p>');

    expect(calledWith).not.toBeNull();
    expect(calledWith.html).toBe('<p>x</p>');
    expect(calledWith.options?.sanitizer?.elements).toContain('p');
    expect(el.textContent).toBe('from-setHTML');
  });

  it('falls back to the walk when setHTML rejects the config', () => {
    const el = document.createElement('div');
    el.setHTML = () => { throw new TypeError('unsupported options'); };

    setSanitizedHtml(el, '<p>kept</p><script>alert(1)<\/script>');

    expect(el.innerHTML).toBe('<p>kept</p>');
  });
});
