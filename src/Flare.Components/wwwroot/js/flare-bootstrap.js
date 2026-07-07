// Flare boot script. Include synchronously in <head>, before the Blazor script:
//   <script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
// Optional data-* attributes override the defaults:
//   data-default-theme / data-default-palette / data-default-mode / data-ready-timeout
//
// It does two things, and NOTHING visual:
//   (1) applies the saved theme/palette/mode classes to <html> before the first paint, so there is
//       no theme flash;
//   (2) exposes window.hideFlareSplash() and dispatches a "flare:ready" event once the app is styled
//       and ready (FlareThemeProvider calls this after theme CSS + fonts have loaded).
//
// Flare does NOT draw a loading spinner or a background - each app owns its own splash (markup +
// background + animation) in its index.html. To have Flare fade your splash out on ready, give that
// element id="flare-splash" or a [data-flare-splash] attribute; otherwise just listen for the
// "flare:ready" event and hide it yourself.
(function () {
    var cfg = (document.currentScript && document.currentScript.dataset) || {};
    var defTheme = cfg.defaultTheme || 'md3-expressive';
    var defPalette = cfg.defaultPalette || 'md3-violet';
    var defMode = cfg.defaultMode || 'auto';
    // Safety net: fire readiness anyway after this long, in case the app never reaches its ready gate
    // (e.g. it does not wrap its UI in FlareThemeProvider, or boot fails). The legacy
    // data-splash-timeout name is still accepted.
    var readyTimeout = parseInt(cfg.readyTimeout || cfg.splashTimeout, 10);
    if (!(readyTimeout > 0)) readyTimeout = 8000;

    var d = document.documentElement, s = localStorage;
    var t = s.getItem('flare-theme') || defTheme;
    var p = s.getItem('flare-palette') || defPalette;
    var m = s.getItem('flare-mode') || defMode;
    var dark = m === 'dark' || (m === 'auto' && matchMedia('(prefers-color-scheme: dark)').matches);

    d.classList.add('flare-theme-' + t, 'flare-palette-' + p);
    if (dark) d.classList.add('flare-mode-dark');

    var fired = false;
    // Signals "the app is styled and ready". Kept under the historical name so existing callers
    // (FlareThemeProvider, revealApp) keep working. It draws nothing itself: it dispatches
    // "flare:ready" and, as a convenience, fades out the app's own splash element if one is tagged.
    window.hideFlareSplash = function () {
        if (fired) return;
        fired = true;
        try { window.dispatchEvent(new Event('flare:ready')); } catch (e) { }
        var el = document.getElementById('flare-splash') || document.querySelector('[data-flare-splash]');
        if (!el) return;
        el.style.transition = 'opacity .25s ease';
        el.style.opacity = '0';
        el.style.pointerEvents = 'none';
        setTimeout(function () { if (el.parentNode) el.parentNode.removeChild(el); }, 300);
    };

    // Safety net so the app is never stranded behind its own splash.
    setTimeout(window.hideFlareSplash, readyTimeout);
})();
