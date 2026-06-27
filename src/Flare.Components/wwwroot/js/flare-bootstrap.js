// Flare anti-FOUC bootstrap. Include synchronously in <head> with ONE line, before the
// Blazor script:
//   <script src="_content/Flare.Components/js/flare-bootstrap.js"></script>
// Optional data-* attributes override the defaults:
//   data-default-theme / data-default-palette / data-default-mode / data-splash-light / data-splash-dark
//
// It (1) sets the saved theme/palette/mode classes on <html> before first paint,
// (2) paints a theme-aware backdrop + a full-screen splash so there is no white flash,
// and exposes window.hideFlareSplash() for the app to call once it is styled and ready.
(function () {
    var cfg = (document.currentScript && document.currentScript.dataset) || {};
    var defTheme = cfg.defaultTheme || 'md3-expressive';
    var defPalette = cfg.defaultPalette || 'md3-violet';
    var defMode = cfg.defaultMode || 'auto';
    var splashLight = cfg.splashLight || '#FEF7FF';
    var splashDark = cfg.splashDark || '#141218';
    // Safety net: how long to keep the splash before revealing anyway, in case the app never reaches
    // its ready gate (e.g. a consumer that does not wrap its UI in FlareThemeProvider, or a boot
    // error). FlareThemeProvider normally reveals far sooner, as soon as theme CSS + fonts are ready.
    var splashTimeout = parseInt(cfg.splashTimeout, 10);
    if (!(splashTimeout > 0)) splashTimeout = 8000;

    var d = document.documentElement, s = localStorage;
    var t = s.getItem('flare-theme') || defTheme;
    var p = s.getItem('flare-palette') || defPalette;
    var m = s.getItem('flare-mode') || defMode;
    var dark = m === 'dark' || (m === 'auto' && matchMedia('(prefers-color-scheme: dark)').matches);

    d.classList.add('flare-theme-' + t, 'flare-palette-' + p);
    if (dark) d.classList.add('flare-mode-dark');

    var css =
        'html{background:' + splashLight + '}' +
        'html.flare-mode-dark{background:' + splashDark + ';color-scheme:dark}' +
        '#flare-splash{position:fixed;inset:0;z-index:99999;display:flex;align-items:center;' +
        'justify-content:center;background:' + splashLight + ';transition:opacity .25s ease}' +
        'html.flare-mode-dark #flare-splash{background:' + splashDark + '}' +
        '#flare-splash.flare-splash--hidden{opacity:0;pointer-events:none}' +
        '#flare-splash .flare-splash__spinner{width:44px;height:44px;border-radius:50%;' +
        'border:4px solid rgba(128,128,128,.25);border-top-color:#888;' +
        'animation:flare-splash-spin 1s linear infinite}' +
        '@keyframes flare-splash-spin{to{transform:rotate(360deg)}}' +
        '@media (prefers-reduced-motion:reduce){#flare-splash .flare-splash__spinner{animation-duration:2.4s}}';
    var st = document.createElement('style');
    st.textContent = css;
    (document.head || d).appendChild(st);

    function addSplash() {
        if (document.getElementById('flare-splash')) return;
        var el = document.createElement('div');
        el.id = 'flare-splash';
        el.innerHTML = '<div class="flare-splash__spinner"></div>';
        document.body.insertBefore(el, document.body.firstChild);
    }
    if (document.body) addSplash();
    else document.addEventListener('DOMContentLoaded', addSplash);

    window.hideFlareSplash = function () {
        var el = document.getElementById('flare-splash');
        if (!el) return;
        el.classList.add('flare-splash--hidden');
        setTimeout(function () { if (el.parentNode) el.parentNode.removeChild(el); }, 300);
    };

    // Safety net: reveal anyway if the app never reaches its ready gate, so the splash can never
    // strand the page. The normal path (FlareThemeProvider) hides it as soon as the UI is styled.
    setTimeout(window.hideFlareSplash, splashTimeout);
})();
