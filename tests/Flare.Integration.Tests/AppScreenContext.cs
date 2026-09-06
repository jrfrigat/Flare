using System.Globalization;
using System.Runtime.CompilerServices;
using Flare.Extensions;
using Flare.Theme.MaterialDesign3;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Integration.Tests;

/// <summary>
/// Pins the run to the invariant culture so resource-backed strings (FlareStrings) resolve to their
/// neutral English values whatever the machine locale is. A [ModuleInitializer] runs once per assembly,
/// so every test project needs its own.
/// </summary>
internal static class TestCultureInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
    }
}

/// <summary>
/// The render context these tests use: services come from <c>AddFlare()</c> - the same call a consuming
/// application makes - rather than from a hand-written list of registrations.
/// <para>
/// That is the difference from the component suite's context, and it is deliberate. A suite that
/// registers the services it needs itself can never notice one missing from the composition root; the
/// screen renders because the test supplied the service, and the application that only called
/// <c>AddFlare</c> is the one that finds out. Here a screen that injects something <c>AddFlare</c> does
/// not register fails to render, which is the contract stated in that method: "AddFlare() must be
/// sufficient on its own".
/// </para>
/// </summary>
public class AppScreenContext : BunitContext
{
    protected AppScreenContext()
    {
        // A theme is not optional - Flare ships none and every component is unstyled without one, so a
        // screen is rendered under a real theme rather than a stub. Auto-discovery is off because the
        // reference is explicit here, and scanning the whole assembly graph would only be slower.
        Services.AddFlare(opts =>
        {
            opts.DefaultTheme = new MaterialDesign3Theme();
            opts.RegisterAllBuiltInThemes = false;
        });

        // Screens reach for JS the way any browser-hosted component does; nothing here asserts on the
        // calls, so loose mode answers them with defaults instead of failing the render.
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
