import { afterEach, expect, it } from 'vitest';
import { positionAnchoredPanel, removeAnchoredPanel } from '../../src/Flare.Components/wwwroot/js/flare-overlay.js';

const ids = [];
afterEach(() => {
    ids.splice(0).forEach(id => removeAnchoredPanel(id));
    document.body.replaceChildren();
});

function pair() {
    const anchor = document.createElement('button');
    const panel = document.createElement('div');
    document.body.append(anchor, panel);
    anchor.getBoundingClientRect = () => ({ left: 100, top: 100, right: 300, bottom: 140, width: 200, height: 40 });
    panel.getBoundingClientRect = () => {
        const left = parseFloat(panel.style.left) || 0;
        const top = parseFloat(panel.style.top) || 0;
        return { left, top, right: left + 100, bottom: top + 50, width: 100, height: 50 };
    };
    return { anchor, panel };
}

function place(placement, direction) {
    const { anchor, panel } = pair();
    panel.style.direction = direction;
    const id = `panel-${ids.length}`;
    ids.push(id);
    positionAnchoredPanel(id, anchor, panel, { placement, gap: 4 });
    return panel;
}

it('mirrors Start and End against the inline direction under RTL', () => {
    // bottom-start aligns the panel's start edge with the anchor's start edge.
    expect(place('bottom-start', 'ltr').style.left).toBe('100px');
    // Under RTL the start edge is the right one: the panel's right edge meets the anchor's right edge.
    expect(place('bottom-start', 'rtl').style.left).toBe('200px');

    expect(place('bottom-end', 'ltr').style.left).toBe('200px');
    expect(place('bottom-end', 'rtl').style.left).toBe('100px');
});

it('keeps the physical left and right SIDES physical in RTL', () => {
    const panel = place('right-start', 'rtl');
    // Still to the right of the anchor; only the Start alignment follows the direction.
    expect(parseFloat(panel.style.left)).toBe(304);
    expect(parseFloat(panel.style.top)).toBe(90);
});

it('publishes where the anchor centre lands inside the panel for the arrow', () => {
    const panel = place('bottom-start', 'ltr');
    // Anchor centre 200 - panel left 100 = 100, clamped to 88 so a 12px arrow stays inside the 100px panel.
    expect(panel.style.getPropertyValue('--_flare-anchor-offset')).toBe('88px');
});
