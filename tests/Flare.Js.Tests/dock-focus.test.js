import { afterEach, expect, it, vi } from 'vitest';
import { captureFocus, register, restoreFocus, unregister } from '../../src/Flare.Components.IDE/wwwroot/js/flare-dock.js';

class ResizeObserverStub {
    observe() { }
    disconnect() { }
}

afterEach(() => {
    for (const canvas of document.querySelectorAll('.dock-focus-test')) unregister(canvas);
    document.body.replaceChildren();
    vi.unstubAllGlobals();
});

it('keeps the caret and selection when a pane moves inside the canvas', () => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    const canvas = document.createElement('div');
    canvas.className = 'dock-focus-test';
    canvas.innerHTML = '<div data-flare-dock-pane><input value="unfinished"></div>';
    document.body.append(canvas);
    register(canvas, { invokeMethodAsync: vi.fn().mockResolvedValue(undefined) });

    const input = canvas.querySelector('input');
    input.focus();
    input.setSelectionRange(2, 5);
    captureFocus(canvas);

    const pane = canvas.querySelector('[data-flare-dock-pane]');
    canvas.append(pane);
    restoreFocus(canvas, null);

    expect(document.activeElement).toBe(input);
    expect([input.selectionStart, input.selectionEnd]).toEqual([2, 5]);
});
