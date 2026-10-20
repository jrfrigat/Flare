using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// The layout follows its Responsive parameter after mount, and a drawer whose width, rail width,
// variant or anchor changes tells the shell to recompute the grid tracks it reserves.
public sealed class FlareLayoutResponsiveTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;
    private readonly BrowserViewportService _viewport;

    public FlareLayoutResponsiveTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        _viewport = new BrowserViewportService(_runtime);
        Services.AddScoped<IBrowserViewportService>(_ => _viewport);
    }

    private static string RootStyle(IRenderedComponent<FlareLayout> cut) =>
        cut.Find($".{Css.Classes.Layout.Root}").GetAttribute("style") ?? string.Empty;

    private static string RootClasses(IRenderedComponent<FlareLayout> cut) =>
        cut.Find($".{Css.Classes.Layout.Root}").ClassName ?? string.Empty;

    [Fact]
    public async Task TurningResponsiveOnAfterMount_SubscribesAndTurnsOffAgain()
    {
        var cut = Render<FlareLayout>(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent("<div>content</div>"));
        Assert.DoesNotContain("ensureViewportListener", _runtime.Calls);

        // The bug: the watcher was created only when firstRender ALSO saw Responsive. Turning it on
        // later left the layout deaf to the viewport.
        cut.Render(p => p
            .Add(x => x.Responsive, true)
            .AddChildContent("<div>content</div>"));
        cut.WaitForAssertion(() => Assert.Contains("ensureViewportListener", _runtime.Calls));

        await _viewport.OnViewportResized(320, 800);
        cut.WaitForAssertion(() => Assert.Contains(Css.Classes.Layout.Mobile, RootClasses(cut)));

        // Turning it off must drop the watcher and go back to the desktop state, not keep the last
        // mobile tier alive.
        cut.Render(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent("<div>content</div>"));
        cut.WaitForAssertion(() => Assert.DoesNotContain(Css.Classes.Layout.Mobile, RootClasses(cut)));
    }

    [Fact]
    public void ChangingDrawerWidthOrVariantAfterMount_UpdatesTheLayoutTracks()
    {
        var cut = Render<FlareLayout>(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent<FlareLayoutDrawer>(d => d
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .Add(x => x.Width, "16rem")
                .Add(x => x.Open, true)));
        Assert.Contains("16rem", RootStyle(cut));

        cut.Render(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent<FlareLayoutDrawer>(d => d
                .Add(x => x.Variant, DrawerVariant.Persistent)
                .Add(x => x.Width, "20rem")
                .Add(x => x.Open, true)));
        cut.WaitForAssertion(() => Assert.Contains("20rem", RootStyle(cut)));

        // Temporary floats over the content: it contributes no grid track at all.
        cut.Render(p => p
            .Add(x => x.Responsive, false)
            .AddChildContent<FlareLayoutDrawer>(d => d
                .Add(x => x.Variant, DrawerVariant.Temporary)
                .Add(x => x.Width, "20rem")
                .Add(x => x.Open, true)));
        cut.WaitForAssertion(() => Assert.DoesNotContain("20rem", RootStyle(cut)));
    }
}
