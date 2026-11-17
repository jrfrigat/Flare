import { afterEach, expect, it, vi } from 'vitest';
import { registerTabFitting } from '../../src/Flare.Components.IDE/wwwroot/js/flare-ide.js';

// jsdom supplies the DOM but no geometry and no ResizeObserver: the strip width is defined on the
// element and the captured callback stands in for a delivered resize batch.
let fitting;
afterEach(() => {
    fitting?.dispose();
    fitting = null;
    document.body.replaceChildren();
    vi.unstubAllGlobals();
});

function strip(width, count = 3) {
    const bar = document.createElement('div');
    let currentWidth = width;
    Object.defineProperty(bar, 'clientWidth', { get: () => currentWidth });
    for (let i = 0; i < count; i++) {
        const tab = document.createElement('button');
        tab.setAttribute('role', 'tab');
        tab.setAttribute('data-flare-tab-icon', 'true');
        tab.style.fontSize = '12px';
        bar.append(tab);
    }
    document.body.append(bar);

    let observer;
    vi.stubGlobal('ResizeObserver', class {
        constructor(callback) { observer = () => callback([]); }
        observe() { }
        disconnect() { }
    });

    fitting = registerTabFitting(bar);
    return {
        compact: () => [...bar.children].map(tab => tab.hasAttribute('data-flare-tab-compact')),
        resize: () => observer(),
        width: value => currentWidth = value,
    };
}

// 12px tabs: entering is below 6em (72px per tab), leaving is above 8em (96px per tab).
it('keeps a compact strip icon-only through consecutive callbacks at an unchanged narrow width', () => {
    const s = strip(180); // 60px per tab, below the 6em compact threshold
    expect(s.compact()).toEqual([true, true, true]);

    s.resize();
    expect(s.compact()).toEqual([true, true, true]);
    s.resize();
    expect(s.compact()).toEqual([true, true, true]);
});

it('keeps a compact strip through the hysteresis band and clears it only above 8em', () => {
    const s = strip(180);
    expect(s.compact()).toEqual([true, true, true]);

    s.width(252); // 84px per tab: wider than 6em, narrower than 8em
    s.resize();
    expect(s.compact()).toEqual([true, true, true]);

    s.width(288); // exactly 8em per tab: still compact
    s.resize();
    expect(s.compact()).toEqual([true, true, true]);

    s.width(300); // 100px per tab: above 8em, the labels return
    s.resize();
    expect(s.compact()).toEqual([false, false, false]);
});

it('enters the icon-only state only below the 6em threshold', () => {
    const s = strip(252); // 84px per tab on a fresh strip
    expect(s.compact()).toEqual([false, false, false]);
});
