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
});

it('starts a drag only from the declared handle', () => {
    const root = document.createElement('div');
    root.className = 'flare-drag-context';
    root.innerHTML = '<div data-flare-drag="item"><span data-flare-drag-handle></span><button>Tab</button></div><div data-flare-drop="zone"></div>';
    document.body.append(root);
    registerDragContext(root, { invokeMethodAsync: vi.fn().mockResolvedValue(null) });

    const item = root.querySelector('[data-flare-drag]');
    const handle = root.querySelector('[data-flare-drag-handle]');
    const button = root.querySelector('button');

    expect(drag.hooks.filter({ target: button })).toBe(false);
    expect(drag.hooks.filter({ target: item })).toBe(false);
    expect(drag.hooks.filter({ target: handle })).toBe(true);
});
