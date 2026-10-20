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

it('resolves percentage, rem and unsupported limits instead of reading them as pixels', () => {
    // 20% and 50% of the 600px parent; the control footprint (80px) stays the floor.
    const percent = resizable('right', '20%', '50%');
    key(percent.handle, 'Home');
    expect(percent.container.style.width).toBe('120px');
    key(percent.handle, 'End');
    expect(percent.container.style.width).toBe('300px');

    // rem is against the root font (16px in jsdom), em against the container's.
    const rem = resizable('right', '10rem', '20rem');
    key(rem.handle, 'Home');
    expect(rem.container.style.width).toBe('160px');
    key(rem.handle, 'End');
    expect(rem.container.style.width).toBe('320px');

    // An inverted range cannot produce min > max or a NaN width.
    const inverted = resizable('right', '80%', '10%');
    key(inverted.handle, 'Home');
    expect(inverted.container.style.width).toBe('480px');
    key(inverted.handle, 'End');
    expect(inverted.container.style.width).toBe('480px');

    // "banana" is not a length and must not turn into NaNpx by way of parseFloat.
    const invalid = resizable('right', 'banana', 'nonsense');
    key(invalid.handle, 'Home');
    expect(invalid.container.style.width).toBe('80px');
    expect(invalid.handle.getAttribute('aria-valuenow')).toBe('80');
});

it('keeps the splitter following the pointer under RTL and reversed flex directions', () => {
    const rtl = splitter('horizontal');
    rtl.gutter.parentElement.style.direction = 'rtl';
    key(rtl.gutter, 'ArrowRight');          // the previous sibling is on the right: the key shrinks it
    expect(rtl.prev.style.width).toBe('228px');
    key(rtl.gutter, 'ArrowLeft');
    expect(rtl.prev.style.width).toBe('240px');

    const rowReverse = splitter('horizontal');
    rowReverse.gutter.parentElement.style.flexDirection = 'row-reverse';
    key(rowReverse.gutter, 'ArrowRight');
    expect(rowReverse.prev.style.width).toBe('228px');

    const columnReverse = splitter('vertical');
    columnReverse.gutter.parentElement.style.flexDirection = 'column-reverse';
    key(columnReverse.gutter, 'ArrowDown');
    expect(columnReverse.prev.style.height).toBe('228px');
});

it('follows an orientation change and a parent flex change without re-registering', async () => {
    const s = splitter('auto');
    expect(s.gutter.getAttribute('aria-orientation')).toBe('vertical');

    // A parent that turns into a column before any interaction: classes and aria follow.
    s.gutter.parentElement.style.flexDirection = 'column';
    await new Promise(resolve => setTimeout(resolve, 0));
    expect(s.gutter.getAttribute('aria-orientation')).toBe('horizontal');
    expect(s.gutter.classList.contains('flare-splitter--vertical')).toBe(true);
    key(s.gutter, 'ArrowDown');
    expect(s.prev.style.height).toBe('252px');

    // An explicit Orientation parameter set after mount wins over the layout.
    s.gutter.dataset.flareSplitterOrientation = 'horizontal';
    await new Promise(resolve => setTimeout(resolve, 0));
    expect(s.gutter.getAttribute('aria-orientation')).toBe('vertical');
    expect(s.gutter.classList.contains('flare-splitter--horizontal')).toBe(true);
});
