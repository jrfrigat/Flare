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
