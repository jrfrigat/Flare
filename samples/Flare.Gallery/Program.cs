using Flare.Abstractions.Tokens;
using Flare.Components.IDE;
using Flare.Extensions;
using Flare.Gallery;
using Flare.Gallery.Services;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.VisualStudio;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = new MaterialDesign3ExpressiveTheme();
    // A theme is not bound to a palette. Until the user picks one, use each theme's OWN default
    // palette (ThemePaletteFollower keeps it in sync on theme switches, e.g. Fluent -> Fluent blue).
    // The Dynamic Color palette stays registered and selectable (it derives from the OS/browser
    // accent via the active theme's generator; Chrome/Edge hide the real accent to avoid
    // fingerprinting, so it degrades to the curated MD3 Violet fallback), just not forced as default.
    opts.UseDynamicPalette = true;
    opts.DynamicFallbackPalette = Md3Palettes.Violet;
    opts.DefaultPaletteId = Md3Palettes.Violet.Id; // default theme (MD3 Expressive) own palette
    // Every theme below is registered explicitly, so skip the reflection-based auto-discovery. That
    // avoids force-loading the whole assembly graph (Assembly.Load + GetTypes over every referenced
    // assembly) at startup and keeps the path trim/AOT-friendly.
    opts.RegisterAllBuiltInThemes = false;
});

// Themes are independent packages now -- the Gallery showcases all of them, so each is registered
// explicitly (AddFlareTheme also forces the theme assembly to load, which mere references don't in a
// trimmed/WASM app). Each theme brings its own palettes via ITheme.Palettes.
builder.Services.AddFlareTheme(new MaterialDesign3Theme());
builder.Services.AddFlareTheme(new MaterialDesign2Theme());
builder.Services.AddFlareTheme(new FluentUI2Theme());
builder.Services.AddFlareTheme(new AeroTheme());
builder.Services.AddFlareTheme(new LiquidGlassTheme());
builder.Services.AddFlareTheme(new VisualStudioTheme());

builder.Services.AddFlareIde();

// Version-check service: every 10s it reads the deployed app version via the service worker and
// raises NewVersionAvailable when a newer build is published; MainLayout shows an actionable toast.
// The service also registers and applies the service worker (ServiceWorkerPath defaults to
// service-worker.js); only the SW file itself (wwwroot/service-worker*.js) is the app's.
builder.Services.AddFlareVersionCheck(opts =>
{
    opts.Interval = TimeSpan.FromSeconds(10);
    opts.UseServiceWorker = true;
});

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddLocalization();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<RailLabelService>();
builder.Services.AddSingleton<GallerySearchService>();
builder.Services.AddSingleton<ChangelogService>();
// A theme is not bound to a palette; when the user switches theme without having pinned a palette,
// follow the new theme's own default palette (e.g. Fluent -> Fluent blue). Activated after build.
// Scoped: it depends on the scoped IThemeService (a singleton here would fail DI validation).
builder.Services.AddScoped<ThemePaletteFollower>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// КРИТИЧНО ДЛЯ PWA: Считываем сохраненный язык ИЗ LOCALSTORAGE перед стартом UI
var languageService = host.Services.GetRequiredService<LanguageService>();
await languageService.InitializeCultureAsync();

// Restore the saved rail-label preference before first paint so the rail renders in the user's
// preferred variant (no flash from the default).
var railLabelService = host.Services.GetRequiredService<RailLabelService>();
await railLabelService.InitializeAsync();

// Activate the palette follower so it subscribes to theme changes before the first user switch.
_ = host.Services.GetRequiredService<ThemePaletteFollower>();

await host.RunAsync();
