import { afterEach, expect, it, vi } from 'vitest';
import { registerTabScroller, removeTabScroller, scrollTabs } from '../../src/Flare.Components/wwwroot/js/flare-ui.js';
let bar;
afterEach(() => { if (bar) removeTabScroller(bar); document.body.replaceChildren(); vi.unstubAllGlobals(); });

function setup(rtl, offset, scrollWidth = 400) {
    bar = document.createElement('div'); bar.style.direction = rtl ? 'rtl' : 'ltr';
    document.body.append(bar);
    Object.defineProperties(bar, { clientWidth: { value: 100 }, scrollWidth: { value: scrollWidth }, scrollLeft: { value: offset, writable: true } });
    vi.stubGlobal('ResizeObserver', class { observe() {} disconnect() {} });
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue() };
    registerTabScroller(bar, callback);
    return callback.invokeMethodAsync.mock.calls[0];
}

function wheel(options) {
    const event = new WheelEvent('wheel', { bubbles: true, cancelable: true, ...options });
    bar.dispatchEvent(event);
    return event.defaultPrevented;
}

it.each([[false, 0, 400, -20], [false, 300, 400, 20], [true, 0, 400, -20],
    [true, -300, 400, 20], [false, 0, 100, 20]])(
    'leaves the wheel native at an edge or without overflow (%s, %s, %s, %s)', (rtl, offset, width, deltaY) => {
        setup(rtl, offset, width);
        bar.scrollBy = vi.fn();
        expect(wheel({ deltaY })).toBe(false);
        expect(bar.scrollBy).not.toHaveBeenCalled();
    });

it.each([[false, 20], [true, -20]])('scrolls the overflowing strip in its reading direction (%s)', (rtl, left) => {
    setup(rtl, 0);
    bar.scrollBy = vi.fn();
    expect(wheel({ deltaY: 20 })).toBe(true);
    expect(bar.scrollBy).toHaveBeenCalledWith({ left, behavior: 'auto' });
});

it.each([{ deltaX: 20 }, { deltaY: 20, ctrlKey: true }, { deltaY: 20, shiftKey: true }])(
    'preserves native horizontal, zoom and modified wheel gestures (%j)', options => {
        setup(false, 100);
        bar.scrollBy = vi.fn();
        expect(wheel(options)).toBe(false);
        expect(bar.scrollBy).not.toHaveBeenCalled();
    });

it.each([[false, 0, true, false], [false, 150, false, false], [false, 300, false, true],
    [true, 0, true, false], [true, -150, false, false], [true, -300, false, true]])(
    'reports logical edges for direction %s offset %s', (rtl, offset, start, end) => {
        expect(setup(rtl, offset)).toEqual(['OnTabScrollState', true, start, end]);
    });

it('scrolls toward logical next in RTL and honors reduced motion', () => {
    setup(true, 0); bar.scrollBy = vi.fn();
    vi.stubGlobal('matchMedia', () => ({ matches: true }));
    scrollTabs(bar, 1);
    expect(bar.scrollBy).toHaveBeenCalledWith({ left: -80, behavior: 'auto' });
    vi.stubGlobal('matchMedia', () => ({ matches: false }));
    scrollTabs(bar, -1);
    expect(bar.scrollBy).toHaveBeenLastCalledWith({ left: 80, behavior: 'smooth' });
});
