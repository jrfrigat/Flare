// Flare boot script. Include synchronously in <head>, before the Blazor script:
//   <script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
// Configured through data-* attributes:
//   data-default-theme / data-default-palette - the theme to paint on a first visit, when nothing is
//     saved yet. Set them to the theme you register in Program.cs; without them the first frame is
//     unthemed until .NET boots.
//   data-default-mode / data-ready-timeout - optional.
//
// It does three things, and NOTHING visual:
//   (1) applies the saved theme/palette/mode classes to <html> before the first paint, so there is
//       no theme flash;
//   (2) starts fetching the theme engine's interop module, which every Flare app needs and which
//       the app can only ask for once .NET has finished booting - a round trip that would otherwise
//       be spent with the interface on screen but unthemed;
//   (3) exposes window.hideFlareSplash() and dispatches a "flare:ready" event once the app is styled
//       and ready (FlareThemeProvider calls this after theme CSS + fonts have loaded).
//
// Flare does NOT draw a loading spinner or a background - each app owns its own splash (markup +
// background + animation) in its index.html. To have Flare fade your splash out on ready, give that
// element id="flare-splash" or a [data-flare-splash] attribute; otherwise just listen for the
// "flare:ready" event and hide it yourself.
(function () {
    var cfg = (document.currentScript && document.currentScript.dataset) || {};
    // No theme or palette is assumed. Flare ships no theme of its own and must not name one here:
    // guessing is worse than not guessing, because the app then paints a full frame in the WRONG
    // theme before .NET boots and corrects it - the very flash this script exists to prevent. An app
    // that wants a themed first paint states its own theme in data-default-theme.
    var defTheme = cfg.defaultTheme || '';
    var defPalette = cfg.defaultPalette || '';
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

    if (t) d.classList.add('flare-theme-' + t);
    if (p) d.classList.add('flare-palette-' + p);
    if (dark) d.classList.add('flare-mode-dark');

    // The theme engine imports this module by a fixed, unfingerprinted path (see CssVariableInjector),
    // so the URL is known here and warming it costs one request that would otherwise happen only
    // after the whole .NET runtime has loaded.
    var preload = document.createElement('link');
    preload.rel = 'modulepreload';
    preload.href = new URL('_content/Flare.Components/js/flare-theme.js', document.baseURI).href;
    document.head.appendChild(preload);

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
