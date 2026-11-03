import { afterEach, expect, it, vi } from 'vitest';
import { bindKeyboardDrag, showKeyboardHint, endKeyboardHint } from '../../src/Flare.Components/wwwroot/js/flare-drag-keyboard.js';

const disposers = [];
afterEach(() => { disposers.splice(0).forEach(dispose => dispose()); document.body.replaceChildren(); });

function board() {
    const root = document.createElement('div');
    root.innerHTML = '<span tabindex="-1" data-flare-keyboard-focus></span><div data-flare-drop="todo"><div data-flare-virtual-count="500"><button data-flare-drag="source">Source</button></div></div>';
    document.body.append(root);
    const list = root.querySelector('[data-flare-virtual-count]');
    Object.defineProperty(list, 'scrollHeight', { value: 35000 });
    root.querySelector('button').scrollIntoView = vi.fn();
    const calls = { invokeMethodAsync: vi.fn(() => Promise.resolve()) };
    disposers.push(bindKeyboardDrag(root, calls));
    return { root, list, calls };
}

it('reveals a model slot and draws it when the virtualizer supplies its item', async () => {
    const { root, list } = board();
    const draw = vi.fn();
    showKeyboardHint(root, 'todo', 299, 'source', 'remote', draw, vi.fn());
    expect(list.scrollTop).toBe(299 / 500 * 35000);
    expect(document.activeElement).toBe(root.querySelector('[data-flare-keyboard-focus]'));
    const remote = document.createElement('button');
    remote.dataset.flareDrag = 'remote';
    remote.scrollIntoView = vi.fn();
    list.replaceChildren(remote);
    await Promise.resolve();
    expect(draw).toHaveBeenCalledWith(root.querySelector('[data-flare-drop]'), remote, false);
});

it('keeps reorder keys alive without the source node and releases them on disposal', () => {
    const { root, list, calls } = board();
    showKeyboardHint(root, 'todo', 299, 'source', 'remote', vi.fn(), vi.fn());
    list.replaceChildren();
    const event = new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true, cancelable: true });
    document.activeElement.dispatchEvent(event);
    expect(event.defaultPrevented).toBe(true);
    expect(calls.invokeMethodAsync).toHaveBeenCalledWith('OnKeyboardReorderAsync', 'ArrowDown');
    disposers.splice(0).forEach(dispose => dispose());
    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
    expect(calls.invokeMethodAsync).toHaveBeenCalledTimes(1);
});

it('restores the original window and focus after cancellation', async () => {
    const { root, list } = board();
    showKeyboardHint(root, 'todo', 299, 'source', 'remote', vi.fn(), vi.fn());
    list.replaceChildren();
    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    endKeyboardHint(root);
    expect(list.scrollTop).toBe(0);
    const source = document.createElement('button');
    source.dataset.flareDrag = 'source';
    list.append(source);
    await Promise.resolve();
    expect(document.activeElement).toBe(source);
});
