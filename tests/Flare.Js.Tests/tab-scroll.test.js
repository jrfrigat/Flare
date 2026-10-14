import { afterEach, expect, it, vi } from 'vitest';
import { registerTabScroller, removeTabScroller, scrollTabs } from '../../src/Flare.Components/wwwroot/js/flare-ui.js';
let bar;
afterEach(() => { if (bar) removeTabScroller(bar); document.body.replaceChildren(); vi.unstubAllGlobals(); });

function setup(rtl, offset) {
    bar = document.createElement('div'); bar.style.direction = rtl ? 'rtl' : 'ltr';
    document.body.append(bar);
    Object.defineProperties(bar, { clientWidth: { value: 100 }, scrollWidth: { value: 400 }, scrollLeft: { value: offset, writable: true } });
    vi.stubGlobal('ResizeObserver', class { observe() {} disconnect() {} });
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue() };
    registerTabScroller(bar, callback);
    return callback.invokeMethodAsync.mock.calls[0];
}

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
