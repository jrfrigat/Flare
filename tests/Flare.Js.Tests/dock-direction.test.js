import { afterEach, expect, it, vi } from 'vitest';
import { register, unregister } from '../../src/Flare.Components.IDE/wwwroot/js/flare-dock.js';

class ResizeObserverStub {
    observe() { }
    disconnect() { }
}

afterEach(() => {
    for (const canvas of document.querySelectorAll('.dock-direction-test')) unregister(canvas);
    document.body.replaceChildren();
    vi.unstubAllGlobals();
});

it('reports a direction change without a resize', async () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const canvas = document.createElement('div');
    canvas.className = 'dock-direction-test';
    document.body.append(canvas);
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };

    register(canvas, callback);
    canvas.style.direction = 'rtl';
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(callback.invokeMethodAsync).toHaveBeenCalledWith('UpdateDockViewport', 0, 0, true);
});
