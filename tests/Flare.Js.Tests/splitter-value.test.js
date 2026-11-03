import { afterEach, expect, it, vi } from 'vitest';
import { registerSiblingSplitter, removeSiblingSplitter } from '../../src/Flare.Components/wwwroot/js/flare-drag.js';

const handles = [];
afterEach(() => { handles.splice(0).forEach(removeSiblingSplitter); document.body.replaceChildren(); });
function control(reverse = true, rtl = false) {
    const root = document.createElement('div');
    root.id = 'scope'; root.style.direction = rtl ? 'rtl' : 'ltr';
    root.innerHTML = '<div id="panel"></div><div tabindex="0"></div>';
    const gutter = root.lastElementChild;
    Object.assign(gutter.dataset, { flareSplitterValue: '', flareSplitterTarget: '#panel', flareSplitterScope: '#scope',
        flareSplitterVariable: '--size', flareSplitterReverse: String(reverse), flareSplitterKeyboardStep: '8',
        flareSplitterLargeStep: '24', flareSplitterMin: '120px', flareSplitterMax: '85%' });
    root.getBoundingClientRect = () => ({ width: 1000, height: 500 });
    root.firstElementChild.getBoundingClientRect = () => ({ width: parseFloat(root.style.getPropertyValue('--size')) || 288 });
    document.body.append(root);
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue() };
    registerSiblingSplitter(gutter, 'horizontal', '120px', '85%', callback);
    handles.push(gutter);
    return { root, gutter, callback };
}
it('measures a CSS-sized target and reverses a trailing edge with a configurable Shift step', () => {
    const { root, gutter, callback } = control();
    expect(gutter.getAttribute('aria-valuenow')).toBe('288');
    gutter.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowLeft', shiftKey: true }));
    expect(root.style.getPropertyValue('--size')).toBe('312px');
    expect(callback.invokeMethodAsync).toHaveBeenCalledExactlyOnceWith('OnSplitChanged', '312px');
    gutter.dispatchEvent(new KeyboardEvent('keydown', { key: 'End' }));
    expect(root.style.getPropertyValue('--size')).toBe('850px');
    gutter.dispatchEvent(new KeyboardEvent('keydown', { key: 'Home' }));
    expect(root.style.getPropertyValue('--size')).toBe('120px');
});
it('combines Reverse with RTL rather than assuming a physical edge', () => {
    const { root, gutter } = control(true, true);
    gutter.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowRight' }));
    expect(root.style.getPropertyValue('--size')).toBe('296px');
});
