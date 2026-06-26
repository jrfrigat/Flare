using Flare.Abstractions.Tokens;
using Flare.Extensions;
using Flare.Legacy;
using Flare.Legacy.Services;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.VisualStudio;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

// Single-language Russian app: pin the culture so Flare's component strings (FlareStrings.ru) and
// ru-RU number/date formatting are used regardless of the browser/OS language.
var ruRu = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = ruRu;
CultureInfo.DefaultThreadCurrentUICulture = ruRu;
CultureInfo.CurrentCulture = ruRu;
CultureInfo.CurrentUICulture = ruRu;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFlare(opts =>
{
    opts.DefaultTheme = LegacyTheme.Instance;
    opts.DefaultPalette = LegacyPalettes.Legacy;
    opts.DefaultMode = ThemeMode.Light; // the legacy look is always light
    // Assembly auto-discovery cannot see WASM theme assemblies that aren't reached by used code,
    // so register every theme explicitly below instead.
    opts.RegisterAllBuiltInThemes = false;
});

// Explicitly register every theme (instantiating forces its assembly into the WASM boot set) so the
// Settings page can switch between them - including the custom Legacy theme.
builder.Services.AddFlareTheme(LegacyTheme.Instance);
builder.Services.AddFlareTheme(new Md3Theme());
builder.Services.AddFlareTheme(new Fluent2Theme());
builder.Services.AddFlareTheme(new AeroTheme());
builder.Services.AddFlareTheme(new LiquidGlassTheme());
builder.Services.AddFlareTheme(new VisualStudioTheme());

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Version-check service: every 10s it reads the deployed app version via the service worker and
// raises NewVersionAvailable when a newer build is published; MainLayout shows an actionable toast.
// The service also registers and applies the service worker (ServiceWorkerPath defaults to
// service-worker.js); only the SW file itself (wwwroot/service-worker*.js) is the app's.
builder.Services.AddFlareVersionCheck(opts =>
{
    opts.Interval = TimeSpan.FromSeconds(10);
    opts.UseServiceWorker = true;
});

// Demo app services: settings + a fake latency-injecting backend. WorkspaceState is scoped (one per
// app session in WASM) so the open loan/client tabs survive navigating to other pages and back.
builder.Services.AddScoped<AppSettings>();
builder.Services.AddScoped<LoanBackend>();
builder.Services.AddScoped<WorkspaceState>();

await builder.Build().RunAsync();
