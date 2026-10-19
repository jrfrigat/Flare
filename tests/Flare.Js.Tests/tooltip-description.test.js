import { afterEach, expect, it } from 'vitest';
import { syncTooltipDescription } from '../../src/Flare.Components/wwwroot/js/flare-overlay.js';

afterEach(() => document.body.replaceChildren());

function tooltip(child = '<button id="trigger">go</button>') {
    const root = document.createElement('div');
    root.className = 'flare-tooltip';
    root.innerHTML = child + '<div class="flare-tooltip__content" id="tip-1">help</div>';
    document.body.append(root);
    return root;
}

it('links the description to the focusable trigger and keeps existing ids', () => {
    const root = tooltip();
    const trigger = root.querySelector('#trigger');
    trigger.setAttribute('aria-describedby', 'existing');

    syncTooltipDescription(root, 'tip-1', true);

    expect(trigger.getAttribute('aria-describedby')).toBe('existing tip-1');
});

it('links a link, an input and a custom tabindex trigger', () => {
    const linkRoot = tooltip('<a id="trigger" href="#">go</a>');
    const inputRoot = tooltip('<input id="trigger" />');
    const customRoot = tooltip('<div id="trigger" tabindex="0">go</div>');

    syncTooltipDescription(linkRoot, 'tip-1', true);
    syncTooltipDescription(inputRoot, 'tip-1', true);
    syncTooltipDescription(customRoot, 'tip-1', true);

    expect(linkRoot.querySelector('#trigger').getAttribute('aria-describedby')).toBe('tip-1');
    expect(inputRoot.querySelector('#trigger').getAttribute('aria-describedby')).toBe('tip-1');
    expect(customRoot.querySelector('#trigger').getAttribute('aria-describedby')).toBe('tip-1');
});

it('moves the id to a new trigger and leaves the old one clean', () => {
    const root = tooltip();
    const first = root.querySelector('#trigger');
    syncTooltipDescription(root, 'tip-1', true);

    first.remove();
    const second = document.createElement('button');
    second.id = 'trigger2';
    root.prepend(second);
    syncTooltipDescription(root, 'tip-1', true);

    expect(first.hasAttribute('aria-describedby')).toBe(false);
    expect(second.getAttribute('aria-describedby')).toBe('tip-1');
});

// A re-render can swap the trigger element without the component calling sync again: the DOM watcher
// installed by the first call must keep the relation on whichever element is the trigger now.
it('relinks the description when the trigger element is replaced', async () => {
    const root = tooltip();
    syncTooltipDescription(root, 'tip-1', true);
    const first = root.querySelector('#trigger');
    expect(first.getAttribute('aria-describedby')).toBe('tip-1');

    first.remove();
    const second = document.createElement('button');
    second.id = 'trigger2';
    root.prepend(second);
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(first.hasAttribute('aria-describedby')).toBe(false);
    expect(second.getAttribute('aria-describedby')).toBe('tip-1');
});

it('links a trigger that appears after the first sync', async () => {
    const root = tooltip('<span id="plain">plain trigger</span>');
    syncTooltipDescription(root, 'tip-1', true);
    expect(root.querySelector('#plain').hasAttribute('aria-describedby')).toBe(false);

    const late = document.createElement('button');
    late.id = 'late';
    root.prepend(late);
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(late.getAttribute('aria-describedby')).toBe('tip-1');
});

it('stops watching once the tooltip is disabled', async () => {
    const root = tooltip();
    syncTooltipDescription(root, 'tip-1', true);
    syncTooltipDescription(root, 'tip-1', false);

    const first = root.querySelector('#trigger');
    first.remove();
    const second = document.createElement('button');
    second.id = 'trigger2';
    root.prepend(second);
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(second.hasAttribute('aria-describedby')).toBe(false);
});

it('removes only its own id when the tooltip is disabled or empty', () => {
    const root = tooltip();
    const trigger = root.querySelector('#trigger');
    trigger.setAttribute('aria-describedby', 'other tip-1');

    syncTooltipDescription(root, 'tip-1', true);
    syncTooltipDescription(root, 'tip-1', false);

    expect(trigger.getAttribute('aria-describedby')).toBe('other');
});

it('never describes a focusable action inside the bubble', () => {
    const root = tooltip('<div id="trigger" tabindex="0">custom</div>');
    const bubble = root.querySelector('.flare-tooltip__content');
    bubble.innerHTML = '<button id="action">action</button>';

    syncTooltipDescription(root, 'tip-1', true);

    expect(root.querySelector('#action').hasAttribute('aria-describedby')).toBe(false);
    expect(root.querySelector('#trigger').getAttribute('aria-describedby')).toBe('tip-1');
});

it('does nothing when the root has no focusable element', () => {
    const root = tooltip('<span id="plain">plain trigger</span>');

    syncTooltipDescription(root, 'tip-1', true);

    expect(root.querySelector('#plain').hasAttribute('aria-describedby')).toBe(false);
});
