using System.Globalization;
using Flare.Browser.Tests.Host;
using Flare.Extensions;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.VisualStudio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Flare.Browser.Tests;

/// <summary>A real loopback Blazor Server host and one shared Chromium, with an isolated context per test.</summary>
public sealed class BrowserFixture : IAsyncLifetime
{
    private WebApplication? _app;
    private IPlaywright? _playwright;
    public IBrowser Browser { get; private set; } = null!;
    public string BaseUrl { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        var root = FindRepository();
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(TestApp).Assembly.GetName().Name,
            ContentRootPath = root,
            EnvironmentName = "Development"
        });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.WebHost.UseStaticWebAssets();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddFlare(o => o.DefaultTheme = new MaterialDesign3ExpressiveTheme());
        builder.Services.AddFlareTheme(new MaterialDesign3Theme());
        builder.Services.AddFlareTheme(new MaterialDesign2Theme());
        builder.Services.AddFlareTheme(new FluentUI2Theme());
        builder.Services.AddFlareTheme(new AeroTheme());
        builder.Services.AddFlareTheme(new LiquidGlassTheme());
        builder.Services.AddFlareTheme(new VisualStudioTheme());
        _app = builder.Build();
        // Use the same build manifest and RCL assets as a consuming Blazor application.
        _app.MapStaticAssets();
        _app.UseAntiforgery();
        _app.MapRazorComponents<TestApp>().AddInteractiveServerRenderMode();
        try
        {
            await _app.StartAsync();
            BaseUrl = _app.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()!.Addresses.Single();
            _playwright = await Playwright.CreateAsync();
            // Missing browsers are a failure, never a silent skip or an automatic network download.
            Browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null) await Browser.DisposeAsync();
        _playwright?.Dispose();
        if (_app is not null) await _app.DisposeAsync();
    }

    private static string FindRepository()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "Flare.slnx"))) return dir.FullName;
        throw new DirectoryNotFoundException("Run browser tests from a Flare repository checkout.");
    }
}
