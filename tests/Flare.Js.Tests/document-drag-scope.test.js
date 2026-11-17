import { beforeEach, expect, it } from 'vitest';
import {
    registerDocumentDragSource,
    takeDroppedDocument,
} from '../../src/Flare.Components.IDE/wwwroot/js/flare-ide.js';

const layout = id => {
    const element = document.createElement('div');
    element.dataset.flareIdeDockScope = id;
    document.body.append(element);
    return element;
};

const tab = (parent, key) => {
    const element = document.createElement('button');
    element.dataset.flareDocKey = key;
    element.setAttribute('draggable', 'true');
    parent.append(element);
    return element;
};

const dragstart = element => {
    const event = new Event('dragstart', { bubbles: true });
    Object.defineProperty(event, 'dataTransfer', {
        value: { effectAllowed: '', setData() { } },
    });
    element.dispatchEvent(event);
};

const drop = element => element.dispatchEvent(new Event('drop', { bubbles: true }));

beforeEach(() => {
    document.body.replaceChildren();
    takeDroppedDocument('cleanup');
    registerDocumentDragSource();
});

it('carries a document key through a drag inside one layout', () => {
    const first = layout('a');
    const source = tab(first, 'settings');

    dragstart(source);
    drop(first);

    expect(takeDroppedDocument('a')).toBe('settings');
    expect(takeDroppedDocument('a')).toBeNull();
});

it('does not hand a key to another layout on the same page', () => {
    const first = layout('a');
    const second = layout('b');
    const source = tab(first, 'settings');

    dragstart(source);
    drop(second);

    expect(takeDroppedDocument('b')).toBeNull();
    expect(takeDroppedDocument('a')).toBeNull();
});

it('does not hand an external drag to a layout', () => {
    const outside = document.createElement('div');
    const second = layout('b');
    const source = tab(outside, 'settings');

    dragstart(source);
    drop(second);

    expect(takeDroppedDocument('b')).toBeNull();
});
