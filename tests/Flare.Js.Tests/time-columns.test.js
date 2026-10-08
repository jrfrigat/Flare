import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flareTimeColumns } from '../../src/Flare.Components/wwwroot/js/flare-time-columns.js';

// Unit tests of input normalization and lifecycle. jsdom geometry is supplied below;
// these do not claim to test physical wheel input, native snapping or touch inertia.
let root, col, receiver, observer;
function setup(values = [0, 15, 30, 45], current = 30, disabled = []) {
    root = document.createElement('div');
    col = document.createElement('div');
    col.style.overflowY = 'auto';
    col.dataset.timeColumn = '1'; col.dataset.timeCurrent = String(current);
    Object.defineProperty(col, 'clientHeight', { value: 192 });
    col.getBoundingClientRect = () => ({ top: 0, height: 192 });
    values.forEach((value, i) => {
        const cell = document.createElement('button');
        cell.dataset.timeValue = String(value); cell.disabled = disabled.includes(value);
        cell.setAttribute('aria-selected', String(value === current));
        cell.getBoundingClientRect = () => ({ top: parseFloat(col.style.paddingTop || 0) + i * 38 - col.scrollTop, height: 36 });
        col.append(cell);
    });
    root.append(col); document.body.append(root);
    receiver = { invokeMethodAsync: vi.fn((_method, _column, value) => {
        col.dataset.timeCurrent = String(value);
        for (const c of col.children) c.setAttribute('aria-selected', String(Number(c.dataset.timeValue) === value));
        return Promise.resolve();
    }) };
    flareTimeColumns.sync(root, receiver, 7);
}
const wheel = (deltaY, extra = {}) => {
    const event = new WheelEvent('wheel', { deltaY, cancelable: true, ...extra });
    col.dispatchEvent(event); return event;
};
const flush = async () => { for (let i = 0; i < 12; i++) await Promise.resolve(); };
beforeEach(() => {
    vi.useFakeTimers();
    vi.stubGlobal('ResizeObserver', class {
        constructor(callback) { observer = { callback, disconnect: vi.fn() }; }
        observe() {} disconnect() { observer.disconnect(); }
    });
});
afterEach(() => { flareTimeColumns.release(root); document.body.replaceChildren(); vi.useRealTimers(); vi.unstubAllGlobals(); });

describe('time column selection', () => {
    it('selects both directions in four visible options and stops at the ends', async () => {
        setup();
        expect(wheel(100).defaultPrevented).toBe(true);
        wheel(100); wheel(-100); await flush();
        expect(receiver.invokeMethodAsync.mock.calls.map(c => c[2])).toEqual([45, 30]);
        expect(col.scrollTop).toBe(76);
    });
    it('accumulates trackpad pixels and resets when direction changes', async () => {
        setup(); wheel(10); wheel(10); await flush(); expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
        wheel(16); await flush(); expect(receiver.invokeMethodAsync).toHaveBeenLastCalledWith('SelectColumn', 1, 45, 7);
        wheel(-10); wheel(-26); await flush(); expect(receiver.invokeMethodAsync).toHaveBeenLastCalledWith('SelectColumn', 1, 30, 7);
    });
    it.each([1, 2])('normalizes deltaMode %i to one step and skips disabled cells', async deltaMode => {
        setup([0, 5, 10, 15, 20], 5, [10]); wheel(1, { deltaMode }); await flush();
        expect(receiver.invokeMethodAsync).toHaveBeenCalledWith('SelectColumn', 1, 15, 7);
    });
    it('keeps off-grid values at the boundary instead of wrapping', async () => {
        setup([0, 15, 30, 45], 59); wheel(100); await flush();
        expect(receiver.invokeMethodAsync).toHaveBeenCalledWith('SelectColumn', 1, 45, 7);
    });
    it('does not consume zoom, horizontal gestures or locked input', async () => {
        setup(); expect(wheel(100, { ctrlKey: true }).defaultPrevented).toBe(false);
        expect(wheel(10, { deltaX: 100 }).defaultPrevented).toBe(false);
        root.dataset.timeLocked = 'true'; expect(wheel(100).defaultPrevented).toBe(false);
        await flush(); expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
    });
    it('settles scrolling on the nearest enabled option only after touch ends', async () => {
        setup([0, 15, 30, 45], 15, [30]);
        col.dispatchEvent(new Event('touchstart'));
        col.scrollTop = 110; col.dispatchEvent(new Event('scroll'));
        await vi.advanceTimersByTimeAsync(200); expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
        col.dispatchEvent(new Event('touchend')); await vi.advanceTimersByTimeAsync(100);
        expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
        col.scrollTop = 114; col.dispatchEvent(new Event('scroll')); await vi.advanceTimersByTimeAsync(160); await flush();
        expect(receiver.invokeMethodAsync).toHaveBeenCalledWith('SelectColumn', 1, 45, 7);
        expect(col.scrollTop).toBe(114);
    });
    it('synchronizes programmatic selection without sending a scroll selection', async () => {
        setup(); col.querySelector('[aria-selected=true]').setAttribute('aria-selected', 'false');
        col.lastElementChild.setAttribute('aria-selected', 'true'); col.dataset.timeCurrent = '45';
        flareTimeColumns.sync(root, receiver, 7); col.dispatchEvent(new Event('scroll'));
        await vi.advanceTimersByTimeAsync(200); expect(col.scrollTop).toBe(114);
        expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
    });
    it('ignores layout scrolling from changes to a neighbouring column', async () => {
        setup(); col.scrollTop = 0; col.dispatchEvent(new Event('scroll'));
        await vi.advanceTimersByTimeAsync(200); expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
    });
    it('keeps unstyled columns clickable without adding padding or listeners', async () => {
        setup(); flareTimeColumns.release(root); col.style.overflowY = '';
        flareTimeColumns.sync(root, receiver, 8); wheel(100); await flush();
        expect(col.style.paddingTop).toBe(''); expect(receiver.invokeMethodAsync).not.toHaveBeenCalled();
    });
    it('releases observers, listeners and pending callbacks', async () => {
        setup(); wheel(100); col.scrollTop = 0; col.dispatchEvent(new Event('scroll'));
        flareTimeColumns.release(root); wheel(-100); await vi.advanceTimersByTimeAsync(200); await flush();
        expect(receiver.invokeMethodAsync).not.toHaveBeenCalled(); expect(observer.disconnect).toHaveBeenCalledOnce();
        expect(col.style.paddingTop).toBe('');
    });
});
