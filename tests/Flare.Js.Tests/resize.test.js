import { afterEach, expect, it, vi } from 'vitest';
import { registerResizeHandle, removeResizeHandle, registerSiblingSplitter, removeSiblingSplitter } from '../../src/Flare.Components/wwwroot/js/flare-drag.js';

const handles = [];
const splitters = [];
function resizable(edge = 'right', min = '120px', max = '480px') {
    const parent = document.createElement('div');
    const container = document.createElement('div');
    const handle = document.createElement('div');
    handle.tabIndex = 0;
    const controls = document.createElement('div');
    controls.dataset.flareResizeControls = '';
    controls.getBoundingClientRect = () => ({ width: 80, height: 40 });
    for (const step of [-1, 1]) {
        const button = document.createElement('button');
        button.dataset.flareResizeStep = String(step);
        controls.append(button);
    }
    container.append(handle, controls); parent.append(container); document.body.append(parent);
    parent.getBoundingClientRect = () => ({ width: 600, height: 400 });
    container.getBoundingClientRect = () => ({ width: parseFloat(container.style.width) || 240, height: parseFloat(container.style.height) || 240 });
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };
    registerResizeHandle(container, handle, edge, min, max, callback);
    handles.push(handle);
    return { container, handle, controls, callback };
}
function key(handle, name) {
    const event = new KeyboardEvent('keydown', { key: name, bubbles: true, cancelable: true });
    handle.dispatchEvent(event);
    return event;
}
afterEach(() => {
    handles.splice(0).forEach(removeResizeHandle);
    splitters.splice(0).forEach(removeSiblingSplitter);
    document.body.replaceChildren();
});

function splitter(axis = 'horizontal') {
    const parent = document.createElement('div');
    parent.style.display = 'flex';
    const prev = document.createElement('div');
    const gutter = document.createElement('div');
    const next = document.createElement('div');
    gutter.tabIndex = 0;
    gutter.dataset.flareSplitterKeyboardStep = '12';
    for (const step of [-1, 1]) {
        const span = document.createElement('span');
        span.dataset.flareSplitterStep = String(step); gutter.append(span);
    }
    for (const pane of [prev, next]) pane.getBoundingClientRect = () => ({ width: parseFloat(pane.style.width) || 240, height: parseFloat(pane.style.height) || 240 });
    parent.append(prev, gutter, next); document.body.append(parent);
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };
    registerSiblingSplitter(gutter, axis, '120px', '400px', callback);
    splitters.push(gutter);
    return { prev, gutter, next, callback };
}

it.each([['horizontal', 'ArrowRight', 'width'], ['vertical', 'ArrowDown', 'height']])(
    'exposes and updates the %s splitter range without scrolling the page', (axis, arrow, dimension) => {
        const s = splitter(axis);
        expect(s.gutter.getAttribute('aria-valuenow')).toBe('240');
        expect(s.gutter.getAttribute('aria-valuemin')).toBe('120');
        expect(s.gutter.getAttribute('aria-valuemax')).toBe('360');
        expect(key(s.gutter, arrow).defaultPrevented).toBe(true);
        expect(s.prev.style[dimension]).toBe('252px');
        expect(s.next.style[dimension]).toBe('228px');
        key(s.gutter, 'End');
        expect(s.prev.style[dimension]).toBe('360px');
        key(s.gutter, 'Home');
        expect(s.prev.style[dimension]).toBe('120px');
        expect(s.gutter.getAttribute('aria-valuenow')).toBe('120');
    });

it('offers single-click splitter steps and releases them on disposal', () => {
    const s = splitter();
    s.gutter.children[1].click();
    expect(s.prev.style.width).toBe('252px');
    expect(s.callback.invokeMethodAsync).toHaveBeenCalledExactlyOnceWith('OnSplitChanged', '252px');
    removeSiblingSplitter(s.gutter);
    s.gutter.children[0].click(); key(s.gutter, 'ArrowRight');
    expect(s.prev.style.width).toBe('252px');
    expect(s.callback.invokeMethodAsync).toHaveBeenCalledTimes(1);
});

it.each([
    ['right', 'ArrowRight', 'width', 256, 'vertical'],
    ['left', 'ArrowRight', 'width', 224, 'vertical'],
    ['bottom', 'ArrowDown', 'height', 256, 'horizontal'],
    ['top', 'ArrowDown', 'height', 224, 'horizontal'],
])('resizes the %s edge from the keyboard and exposes its measured range', (edge, arrow, dimension, value, orientation) => {
    const r = resizable(edge);
    expect(r.handle.getAttribute('aria-valuenow')).toBe('240');
    expect(r.handle.getAttribute('aria-orientation')).toBe(orientation);
    expect(key(r.handle, arrow).defaultPrevented).toBe(true);
    expect(r.container.style[dimension]).toBe(value + 'px');
    expect(r.handle.getAttribute('aria-valuenow')).toBe(String(value));
    expect(r.callback.invokeMethodAsync).toHaveBeenCalledExactlyOnceWith('OnResizedCallback', value + 'px');
});

it('supports non-drag buttons and Home/End without escaping the range', () => {
    const r = resizable();
    r.controls.children[1].click();
    expect(r.container.style.width).toBe('256px');
    key(r.handle, 'Home');
    expect(r.container.style.width).toBe('120px');
    expect(r.controls.children[0].disabled).toBe(true);
    key(r.handle, 'End');
    expect(r.container.style.width).toBe('480px');
    expect(r.controls.children[1].disabled).toBe(true);
    expect(key(r.handle, 'ArrowDown').defaultPrevented).toBe(false);
});

it('detaches keyboard and button handlers with the drag registration', () => {
    const r = resizable();
    removeResizeHandle(r.handle);
    key(r.handle, 'ArrowRight');
    r.controls.children[1].click();
    expect(r.container.style.width).toBe('');
    expect(r.callback.invokeMethodAsync).not.toHaveBeenCalled();
});

it('keeps the resize controls reachable at Home without an explicit minimum', () => {
    const r = resizable('right', null, null);
    key(r.handle, 'Home');
    expect(r.container.style.width).toBe('80px');
    expect(r.handle.getAttribute('aria-valuemin')).toBe('80');
    expect(r.controls.children[1].disabled).toBe(false);
    r.controls.children[1].click();
    expect(r.container.style.width).toBe('96px');
});
