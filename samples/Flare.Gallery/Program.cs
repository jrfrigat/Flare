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
    opts.DefaultTheme = new Md3Theme();
    // Open with Material Design 3 Expressive + the Dynamic Color palette by default. The dynamic
    // palette derives from the OS/browser accent color (Windows/macOS accent, Android Material You)
    // via the active theme's generator -- this works in browsers that expose the real accent (e.g.
    // Firefox). Chrome/Edge do NOT expose the genuine OS accent on the open web (they return a fixed
    // placeholder to mitigate fingerprinting), so there the Dynamic palette uses the fallback below:
    // the curated MD3 Violet palette, rather than an arbitrary blue.
    opts.UseDynamicPalette = true;
    opts.DynamicFallbackPalette = Md3Palettes.Violet;
    opts.DefaultPaletteId = Palette.DynamicId;
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
builder.Services.AddFlareTheme(new Fluent2Theme());
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
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// КРИТИЧНО ДЛЯ PWA: Считываем сохраненный язык ИЗ LOCALSTORAGE перед стартом UI
var languageService = host.Services.GetRequiredService<LanguageService>();
await languageService.InitializeCultureAsync();

// Restore the saved rail-label preference before first paint so the rail renders in the user's
// preferred variant (no flash from the default).
var railLabelService = host.Services.GetRequiredService<RailLabelService>();
await railLabelService.InitializeAsync();

await host.RunAsync();
