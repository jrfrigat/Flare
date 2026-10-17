import { afterEach, expect, it } from 'vitest';
import { setThemeLineageClasses } from '../../src/Flare.Components/wwwroot/js/flare-theme.js';

// The document element has to carry the same mode classes as the theme root (Css.Classes.Theme.ModeClasses):
// overlays portaled to <body> are styled by <html>, the app content by the root.
afterEach(() => { document.documentElement.className = ''; });

const modeClasses = () => [...document.documentElement.classList].filter(c => c.startsWith('flare-mode-')).sort();

it.each([
    [false, false, ['flare-mode-light']],
    [true, false, ['flare-mode-dark']],
    [true, true, ['flare-mode-dark', 'flare-mode-high-contrast']],
])('dark=%s highContrast=%s sets the root map', (dark, highContrast, expected) => {
    setThemeLineageClasses(['md3'], 'violet', dark, highContrast);
    expect(modeClasses()).toEqual(expected);
});

it('replaces every earlier mode class, including the light one the bootstrap painted', () => {
    document.documentElement.className = 'flare-mode-light flare-mode-high-contrast keep-me';
    setThemeLineageClasses(['md3'], 'violet', true, false);
    expect(modeClasses()).toEqual(['flare-mode-dark']);
    expect(document.documentElement.classList.contains('keep-me')).toBe(true);
});
