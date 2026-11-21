import { afterEach, expect, it, vi } from 'vitest';
import { register, unregister } from '../../src/Flare.Components.IDE/wwwroot/js/flare-dock.js';

class ResizeObserverStub {
    observe() { }
    disconnect() { }
}

afterEach(() => {
    for (const canvas of document.querySelectorAll('.dock-actions-test')) unregister(canvas);
    document.body.replaceChildren();
    vi.unstubAllGlobals();
});

it('closes an open actions menu only for a pointer outside it', () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const canvas = document.createElement('div');
    canvas.className = 'dock-actions-test';
    canvas.innerHTML = '<button data-flare-dock-actions-toggle></button><div data-flare-dock-actions></div>';
    document.body.append(canvas);
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) };
    register(canvas, callback);

    canvas.querySelector('[data-flare-dock-actions]').dispatchEvent(new Event('pointerdown', { bubbles: true }));
    expect(callback.invokeMethodAsync).not.toHaveBeenCalled();

    document.body.dispatchEvent(new Event('pointerdown', { bubbles: true }));
    expect(callback.invokeMethodAsync).toHaveBeenCalledWith('CloseDockActions');
});
