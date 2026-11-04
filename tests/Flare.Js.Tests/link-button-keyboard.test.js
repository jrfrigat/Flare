import { afterEach, expect, it, vi } from 'vitest';
import '../../src/Flare.Components/wwwroot/js/flare-link-button.js';

document.dispatchEvent(new Event('DOMContentLoaded'));
afterEach(() => document.body.replaceChildren());

function link(disabled = false) {
    const el = document.createElement('a');
    el.setAttribute('role', 'button');
    if (disabled) el.setAttribute('aria-disabled', 'true');
    const click = vi.fn();
    el.addEventListener('click', click);
    document.body.append(el);
    return { el, click };
}

function space(el, options = {}) {
    const event = new KeyboardEvent('keydown', { key: ' ', bubbles: true, cancelable: true, ...options });
    el.dispatchEvent(event);
    return event;
}

it('blocks native Space scrolling on a disabled link without activating it', () => {
    const x = link(true);
    expect(space(x.el).defaultPrevented).toBe(true);
    expect(x.click).not.toHaveBeenCalled();
});

it('blocks repeated Space scrolling without repeating activation', () => {
    const x = link();
    expect(space(x.el).defaultPrevented).toBe(true);
    expect(space(x.el, { repeat: true }).defaultPrevented).toBe(true);
    expect(x.click).toHaveBeenCalledTimes(1);
});

it('leaves ordinary links, fields and modified keys to the browser', () => {
    const x = link();
    expect(space(x.el, { ctrlKey: true }).defaultPrevented).toBe(false);
    const field = document.createElement('input'); x.el.append(field);
    expect(space(field).defaultPrevented).toBe(false);
    x.el.removeAttribute('role');
    expect(space(x.el).defaultPrevented).toBe(false);
    expect(x.click).not.toHaveBeenCalled();
});
