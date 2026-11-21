import { afterEach, expect, it, vi } from 'vitest';
import { registerDragContext, removeDragContext } from '../../src/Flare.Components/wwwroot/js/flare-dragdrop.js';

const drag = vi.hoisted(() => ({ hooks: null }));
vi.mock('../../src/Flare.Components/wwwroot/js/flare-drag.js', () => ({
    startDrag: (_root, hooks) => {
        drag.hooks = hooks;
        return () => { };
    }
}));

afterEach(() => {
    for (const root of document.querySelectorAll('.flare-drag-context')) removeDragContext(root);
    document.body.replaceChildren();
    vi.unstubAllGlobals();
    delete document.elementFromPoint;
});

it('releases the drag without sending a drop when a pointer is canceled on a valid target', () => {
    vi.stubGlobal('requestAnimationFrame', vi.fn(() => 1));
    vi.stubGlobal('cancelAnimationFrame', vi.fn());
    const root = document.createElement('div');
    root.className = 'flare-drag-context';
    root.innerHTML = '<button data-flare-drag="source">Source</button><div data-flare-drop="target"></div>';
    document.body.append(root);
    const source = root.querySelector('button');
    const target = root.querySelector('[data-flare-drop]');
    document.elementFromPoint = () => target;
    const callback = { invokeMethodAsync: vi.fn().mockResolvedValue(null) };

    registerDragContext(root, callback);
    drag.hooks.onStart({ target: source, clientX: 2, clientY: 4 });
    drag.hooks.onMove(10, 20, { clientX: 12, clientY: 24 });
    expect(target.classList.contains('flare-drop-zone--over')).toBe(true);

    drag.hooks.onCancel();
    drag.hooks.onEnd();

    expect(target.classList.contains('flare-drop-zone--over')).toBe(false);
    expect(document.querySelector('.flare-drag-preview')).toBeNull();
    expect(callback.invokeMethodAsync.mock.calls.map(([name]) => name))
        .toEqual(['OnDragStartAsync', 'OnDragEndAsync']);
});

it('does not cancel when a touch transfers capture from a child to the drag handle', async () => {
    const { startDrag } = await vi.importActual('../../src/Flare.Components/wwwroot/js/flare-drag.js');
    const handle = document.createElement('div');
    const child = document.createElement('button');
    handle.append(child);
    document.body.append(handle);
    const cancel = vi.fn();
    const dispose = startDrag(handle, { onCancel: cancel });
    const pointer = type => new PointerEvent(type, { bubbles: true, pointerType: 'touch', pointerId: 5 });
    try {
        child.dispatchEvent(pointer('pointerdown'));
        child.dispatchEvent(pointer('lostpointercapture'));
        expect(cancel).not.toHaveBeenCalled();

        handle.dispatchEvent(pointer('lostpointercapture'));
        expect(cancel).toHaveBeenCalledTimes(1);
    } finally {
        dispose();
    }
});
