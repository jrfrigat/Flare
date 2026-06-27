// Flare theming: CSS variables, theme/palette/mode classes, the static theme stylesheet, custom
// token overrides and the live system color-scheme subscription.

export function setCssVariables(vars) {
    const root = document.documentElement;
    for (const [name, value] of Object.entries(vars))
        root.style.setProperty(name, value);
}

// ClassToggle strategy: put the active theme/palette/mode classes on <html> so the
// generated CSS applies to everything, including overlays portaled to <body>.
export function setThemeClasses(themeId, paletteId, dark) {
    const r = document.documentElement;
    r.className = r.className
        .replace(/\bflare-theme-\S+/g, '')
        .replace(/\bflare-palette-\S+/g, '')
        .replace(/\bflare-mode-dark\b/g, '')
        .replace(/\s+/g, ' ')
        .trim();
    r.classList.add('flare-theme-' + themeId, 'flare-palette-' + paletteId);
    if (dark) r.classList.add('flare-mode-dark');
}

// Runtime safety net: make sure a stylesheet <link> is present (for themes/palettes
// registered after the initial <head> render). No-op if already loaded.
export function ensureStylesheet(href) {
    if (!href) return;
    if (!document.querySelector(`link[rel="stylesheet"][href="${CSS.escape(href)}"]`)) {
        const l = document.createElement('link');
        l.rel = 'stylesheet';
        l.href = href;
        document.head.appendChild(l);
    }
}

// ClassToggle strategy: keep the generated class-scoped theme CSS in a single <style>.
// Switching theme/palette/mode is then just a class change on the root element.
export function setStaticThemeCss(css) {
    let el = document.getElementById('flare-theme-static');
    if (!el) {
        el = document.createElement('style');
        el.id = 'flare-theme-static';
        document.head.appendChild(el);
    }
    el.textContent = css;
}

// Custom tokens must override the ClassToggle theme. The theme/palette/mode classes sit on BOTH
// <html> (for body-portaled overlays) and the FlareThemeProvider's .flare-root element, and that
// element re-declares the color/design tokens for its subtree -- so an inline custom value on <html>
// alone is shadowed inside the app. Write to both: <html> (covers overlays) and the outermost
// .flare-root (covers the in-app subtree); inline wins over the element's own class rules.
function flareTokenTargets() {
    const targets = [document.documentElement];
    const root = document.querySelector('.flare-root');
    if (root) targets.push(root);
    return targets;
}

export function setCustomTokens(tokens) {
    for (const el of flareTokenTargets())
        for (const [key, value] of Object.entries(tokens))
            el.style.setProperty(key, value);
}

export function clearCustomTokens(tokenNames) {
    for (const el of flareTokenTargets())
        for (const name of tokenNames)
            el.style.removeProperty(name);
}

export function getCssVariable(name) {
    return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
}

export function prefersColorSchemeDark() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
}

// --- OS accent color (Dynamic Color) ---
// Reads the OS/browser accent color exposed via the CSS `AccentColor` system color (Windows accent,
// macOS accent, Android Material You). Returns a #RRGGBB hex, or null when the real OS accent is not
// available -- the caller then falls back to the configured seed.
//
// IMPORTANT: Chromium (Chrome/Edge) deliberately does NOT expose the real OS accent on the open web
// to mitigate fingerprinting -- it returns a fixed default (#0075FF, rgb(0,117,255), identical in
// light and dark) for the `AccentColor` keyword regardless of the user's Windows/macOS accent, even
// in installed PWAs on current versions. Only Firefox returns the true accent broadly. So we treat
// that Chromium sentinel as "no real accent" and return null, so the Dynamic palette uses the app's
// configured seed (a deliberate brand color) instead of an arbitrary blue that is the same for every
// user. Where a browser DOES expose the genuine accent (e.g. Firefox), it flows through unchanged.
// Gated on CSS.supports so we never mistake an inherited text color for the accent.
const CHROMIUM_DEFAULT_ACCENT = '#0075FF'; // rgb(0,117,255): Chromium's placeholder, not a real OS accent

export function getAccentColor() {
    try {
        if (typeof CSS === 'undefined' || !CSS.supports || !CSS.supports('color', 'AccentColor')) return null;
        const el = document.createElement('span');
        el.style.cssText = 'color:AccentColor;position:absolute;opacity:0;pointer-events:none';
        document.body.appendChild(el);
        const c = getComputedStyle(el).color;
        el.remove();
        const m = c && c.match(/[\d.]+/g);
        if (!m || m.length < 3) return null;
        const hex = '#' + m.slice(0, 3).map(n => Math.round(parseFloat(n)).toString(16).padStart(2, '0')).join('').toUpperCase();
        return hex === CHROMIUM_DEFAULT_ACCENT ? null : hex;
    } catch {
        return null;
    }
}

// The OS accent has no dedicated change event; re-read on window focus (cheap) so a mid-session
// accent change is picked up the next time the app regains focus.
const _accentListeners = new Map();

export function subscribeAccent(id, dotNetRef) {
    unsubscribeAccent(id);
    const handler = () => dotNetRef.invokeMethodAsync('OnAccentColorChanged');
    window.addEventListener('focus', handler);
    _accentListeners.set(id, handler);
}

export function unsubscribeAccent(id) {
    const handler = _accentListeners.get(id);
    if (handler) {
        window.removeEventListener('focus', handler);
        _accentListeners.delete(id);
    }
}

// --- System color-scheme live subscription ---
const _schemeListeners = new Map();

export function subscribeColorScheme(id, dotNetRef) {
    unsubscribeColorScheme(id);
    const mq = window.matchMedia('(prefers-color-scheme: dark)');
    const handler = (e) => dotNetRef.invokeMethodAsync('OnSystemColorSchemeChanged', e.matches);
    _schemeListeners.set(id, { mq, handler });
    mq.addEventListener('change', handler);
}

export function unsubscribeColorScheme(id) {
    const entry = _schemeListeners.get(id);
    if (entry) {
        entry.mq.removeEventListener('change', entry.handler);
        _schemeListeners.delete(id);
    }
}
