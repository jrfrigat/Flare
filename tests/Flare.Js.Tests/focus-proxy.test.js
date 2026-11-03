import { afterEach, expect, it } from 'vitest';
import '../../src/Flare.Components/wwwroot/js/flare-focus-proxy.js';

afterEach(() => document.body.replaceChildren());

it('routes restored wrapper focus to the current child and leaves other child focus alone', () => {
    const wrapper = document.createElement('div');
    wrapper.tabIndex = -1;
    wrapper.dataset.flareFocusTarget = '.entry';
    wrapper.innerHTML = '<button class="entry">Item</button><button>Action</button>';
    document.body.append(wrapper);
    wrapper.focus();
    expect(document.activeElement).toBe(wrapper.firstElementChild);
    wrapper.lastElementChild.focus();
    expect(document.activeElement).toBe(wrapper.lastElementChild);
    wrapper.firstElementChild.replaceWith(Object.assign(document.createElement('button'), { className: 'entry' }));
    wrapper.focus();
    expect(document.activeElement).toBe(wrapper.firstElementChild);
});

it('does not activate disabled or hidden children and tolerates an invalid selector', () => {
    const wrapper = document.createElement('div');
    wrapper.tabIndex = -1;
    wrapper.dataset.flareFocusTarget = 'button';
    wrapper.innerHTML = '<button disabled>Disabled</button>';
    document.body.append(wrapper);
    wrapper.focus();
    expect(document.activeElement).toBe(wrapper);
    wrapper.dataset.flareFocusTarget = '[';
    wrapper.blur(); wrapper.focus();
    expect(document.activeElement).toBe(wrapper);
});
