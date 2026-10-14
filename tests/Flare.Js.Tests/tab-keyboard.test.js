import { afterEach, expect, it, vi } from 'vitest';
import { registerTabKeyboard, removeTabKeyboard, revealFocusedTab } from '../../src/Flare.Components/wwwroot/js/flare-ui.js';

let bar;
afterEach(() => { if (bar) removeTabKeyboard(bar); document.body.replaceChildren(); });

function setup(vertical = false, rtl = false) {
    bar = document.createElement('div');
    bar.setAttribute('role', 'tablist');
    bar.setAttribute('aria-orientation', vertical ? 'vertical' : 'horizontal');
    bar.style.direction = rtl ? 'rtl' : 'ltr';
    bar.innerHTML = '<button role="tab"><span>One</span></button>';
    document.body.append(bar);
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue() };
    registerTabKeyboard(bar, callback);
    return callback;
}

function press(key, target = bar.querySelector('span'), options = {}) {
    const event = new KeyboardEvent('keydown', { key, bubbles: true, cancelable: true, ...options });
    target.dispatchEvent(event);
    return event.defaultPrevented;
}

it('prevents only the navigation axis and forwards computed RTL', () => {
    const callback = setup(false, true);
    expect(press('ArrowLeft')).toBe(true);
    expect(callback.invokeMethodAsync).toHaveBeenCalledWith('OnTabNavigation', 'ArrowLeft', true);
    for (const key of ['Tab', 'Enter', ' ', 'ArrowUp', 'ArrowDown']) expect(press(key)).toBe(false);
    expect(press('ArrowRight', undefined, { ctrlKey: true })).toBe(false);
});

it('vertical lists handle up down home end but leave horizontal arrows alone', () => {
    setup(true);
    for (const key of ['ArrowUp', 'ArrowDown', 'Home', 'End']) expect(press(key)).toBe(true);
    expect(press('ArrowRight')).toBe(false);
});

it('Delete is reserved only for a closeable tab', () => {
    setup();
    expect(press('Delete')).toBe(false);
    bar.querySelector('button').dataset.closeable = 'true';
    expect(press('Delete')).toBe(true);
});

it('ignores nested tablists and removes the listener', () => {
    const callback = setup();
    const nested = document.createElement('div');
    nested.setAttribute('role', 'tablist');
    nested.innerHTML = '<button role="tab">Nested</button>';
    bar.append(nested);
    expect(press('ArrowRight', nested.firstChild)).toBe(false);
    expect(callback.invokeMethodAsync).not.toHaveBeenCalled();
    removeTabKeyboard(bar);
    expect(press('ArrowRight')).toBe(false);
});

it('reveals a focused tab by scrolling only its own bar', () => {
    setup();
    const tab = bar.querySelector('button'); tab.focus();
    bar.getBoundingClientRect = () => ({ left: 10, right: 110, top: 20, bottom: 60 });
    tab.getBoundingClientRect = () => ({ left: 90, right: 150, top: 20, bottom: 60 });
    bar.scrollBy = vi.fn();
    revealFocusedTab(bar);
    expect(bar.scrollBy).toHaveBeenCalledWith({ left: 40, top: 0, behavior: 'auto' });
});
