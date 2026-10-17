import { afterEach, expect, it } from 'vitest';
import { registerTreeKeyboard, removeTreeKeyboard, focusTreeItem } from '../../src/Flare.Components/wwwroot/js/flare-ui.js';

let root;
afterEach(() => { if (root) removeTreeKeyboard(root); document.body.replaceChildren(); root = undefined; });

function tree(count) {
    root = document.createElement('div');
    root.setAttribute('role', 'tree');
    for (let i = 0; i < count; i++) {
        const row = document.createElement('div');
        row.setAttribute('role', 'treeitem');
        row.dataset.flareTreeIndex = String(i);
        row.tabIndex = -1;
        row.innerHTML = '<input>';
        root.append(row);
    }
    document.body.append(root);
    return root;
}

function press(key, target) {
    const e = new KeyboardEvent('keydown', { key, bubbles: true, cancelable: true });
    target.dispatchEvent(e);
    return e.defaultPrevented;
}

it('keeps the tree keys from the browser only on a row', () => {
    tree(2);
    registerTreeKeyboard(root);
    const row = root.querySelector('[role="treeitem"]');
    for (const key of ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Home', 'End', ' ']) expect(press(key, row)).toBe(true);
    for (const key of ['Tab', 'Enter', 'a']) expect(press(key, row)).toBe(false);
    expect(press('ArrowDown', row.querySelector('input'))).toBe(false);
});

it('focuses a rendered row at once', async () => {
    tree(3);
    expect(await focusTreeItem(root, 2)).toBe(true);
    expect(document.activeElement).toBe(root.querySelector('[data-flare-tree-index="2"]'));
});

it('waits for a virtualized row to be rendered, then focuses it', async () => {
    tree(1);
    const pending = focusTreeItem(root, 40);
    const late = document.createElement('div');
    late.setAttribute('role', 'treeitem');
    late.dataset.flareTreeIndex = '40';
    late.tabIndex = 0;
    root.append(late);
    expect(await pending).toBe(true);
    expect(document.activeElement).toBe(late);
});

it('gives up quietly when the row never appears', async () => {
    tree(1);
    expect(await focusTreeItem(root, 99, 20)).toBe(false);
});
