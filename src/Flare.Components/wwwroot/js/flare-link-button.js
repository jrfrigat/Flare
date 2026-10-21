// A FlareButton rendered as a link carries role="button", so it must answer Space the way a native
// button does. Enter is the anchor's own activation; only Space needs help. The listener is delegated
// once per page and ignores fields, disabled links and modifier combinations.
const install = () => {
    if (globalThis.__flareLinkButtonKeyboard) return;
    globalThis.__flareLinkButtonKeyboard = true;
    document.addEventListener('keydown', e => {
        if (e.key !== ' ' && e.key !== 'Spacebar') return;
        if (e.altKey || e.ctrlKey || e.metaKey || e.repeat || e.isComposing) return;
        const target = e.target;
        if (!(target instanceof Element)) return;
        if (target.closest('input, textarea, select, [contenteditable]')) return;
        const link = target.closest('a[role="button"]');
        if (!link || link.getAttribute('aria-disabled') === 'true') return;
        e.preventDefault();
        link.click();
    });
};

// The theme module can load before or after the document is parsed.
if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', install, { once: true });
else install();
