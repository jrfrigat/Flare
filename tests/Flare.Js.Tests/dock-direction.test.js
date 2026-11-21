import { afterEach, expect, it, vi } from 'vitest';
import { register, unregister } from '../../src/Flare.Components.IDE/wwwroot/js/flare-dock.js';

class ResizeObserverStub {
    static callbacks = [];
    constructor(callback) { ResizeObserverStub.callbacks.push(callback); }
    observe() { }
    disconnect() { }
}

const tick = () => new Promise(resolve => setTimeout(resolve, 0));

function mount() {
    const wrapper = document.createElement('div');
    wrapper.className = 'dock-direction-test';
    const canvas = document.createElement('div');
    canvas.className = 'dock-direction-test';
    wrapper.append(canvas);
    document.body.append(wrapper);
    return { wrapper, canvas };
}

afterEach(() => {
    for (const canvas of document.querySelectorAll('.dock-direction-test')) unregister(canvas);
    document.body.replaceChildren();
    ResizeObserverStub.callbacks = [];
    vi.unstubAllGlobals();
});

it('reports a direction change made on an ancestor without a resize', async () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const { wrapper, canvas } = mount();
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };

    register(canvas, callback);
    wrapper.dir = 'rtl';
    await tick();

    expect(callback.invokeMethodAsync).toHaveBeenCalledWith('UpdateDockViewport', 0, 0, true, 12, 96);
});

it('does not report the same viewport twice', async () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const { wrapper, canvas } = mount();
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };

    register(canvas, callback);
    wrapper.dir = 'rtl';
    await tick();
    callback.invokeMethodAsync.mockClear();
    wrapper.className = 'dock-direction-test changed';
    await tick();

    expect(callback.invokeMethodAsync).not.toHaveBeenCalled();
});

it('ignores the splitter preview written to the canvas style', async () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const { canvas } = mount();
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };

    register(canvas, callback);
    canvas.style.setProperty('--flare-dock-preview-split', '40px');
    await tick();

    expect(callback.invokeMethodAsync).not.toHaveBeenCalled();
});

it('reports a real size change once per new size', async () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const { canvas } = mount();
    let width = 100;
    let height = 50;
    Object.defineProperty(canvas, 'clientWidth', { get: () => width });
    Object.defineProperty(canvas, 'clientHeight', { get: () => height });
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };

    register(canvas, callback);
    const resize = ResizeObserverStub.callbacks.at(-1);
    resize();
    await tick();
    width = 200;
    resize();
    await tick();

    expect(callback.invokeMethodAsync.mock.calls.map(call => call.slice(1, 4))).toEqual([
        [100, 50, false],
        [200, 50, false],
    ]);
});
