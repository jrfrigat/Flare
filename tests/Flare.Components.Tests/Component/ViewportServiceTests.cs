using Flare.Components;
using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// IBrowserViewportService (BrowserViewportService) + the Xxl breakpoint rollout on
// FlareHidden / FlareCol. Under bUnit's loose JS interop the JS module returns no
// measurable size, so the service falls back to Md and exercises the C#-side fan-out,
// immediate-fire, and disposable-token contract.
// ------------------------------------------------------------------------------

public class C_BrowserViewportServiceTests : FlareTestContext
{
    private IBrowserViewportService Viewport => Services.GetRequiredService<IBrowserViewportService>();

    [Fact]
    public async Task GetBreakpoint_FallsBackToMd_WhenNoMeasurableWidth()
    {
        Assert.Equal(Breakpoint.Md, await Viewport.GetBreakpointAsync());
    }

    [Fact]
    public async Task GetViewportSize_ReturnsDefault_WhenNoBrowser()
    {
        var size = await Viewport.GetViewportSizeAsync();
        Assert.Equal(0, size.Width);
        Assert.Equal(0, size.Height);
    }

    [Fact]
    public async Task SubscribeBreakpoint_FiresImmediately_AndReturnsDisposableToken()
    {
        Breakpoint? seen = null;
        var token = await Viewport.SubscribeBreakpointAsync(bp => seen = bp);

        Assert.NotNull(token);
        Assert.Equal(Breakpoint.Md, seen); // immediate fire with the current (fallback) tier

        await token.DisposeAsync();        // must not throw
    }

    [Fact]
    public async Task SubscribeBreakpoint_SuppressesImmediate_WhenNotRequested()
    {
        var fired = false;
        var token = await Viewport.SubscribeBreakpointAsync(_ => fired = true, fireImmediately: false);

        Assert.False(fired); // no synthetic first notification
        await token.DisposeAsync();
    }

    [Fact]
    public async Task Subscribe_FullChange_ReportsImmediateFlag()
    {
        ViewportChange? change = null;
        var token = await Viewport.SubscribeAsync(c => change = c);

        Assert.NotNull(change);
        Assert.True(change!.Value.IsImmediate);
        Assert.True(change.Value.BreakpointChanged);
        await token.DisposeAsync();
    }

    [Fact]
    public async Task MatchesAsync_ReturnsFalse_WhenNoBrowser()
    {
        Assert.False(await Viewport.MatchesAsync("(min-width: 600px)"));
    }
}

public class C_HiddenXxlTests : FlareTestContext
{
    [Theory]
    [InlineData("flare-hidden--only-xxl")]
    public void Only_Xxl_EmitsClass(string expected)
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Only, Breakpoint.Xxl).AddChildContent("<i>x</i>"));
        Assert.Contains(expected, cut.Markup);
    }

    [Fact]
    public void Below_Xxl_EmitsClass()
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Below, Breakpoint.Xxl).AddChildContent("<i>x</i>"));
        Assert.Contains("flare-hidden--below-xxl", cut.Markup);
    }

    [Fact]
    public void Above_Xl_EmitsClass()
    {
        var cut = Render<FlareHidden>(p => p.Add(x => x.Above, Breakpoint.Xl).AddChildContent("<i>x</i>"));
        Assert.Contains("flare-hidden--above-xl", cut.Markup);
    }

    [Fact]
    public void Invert_Only_Xxl_EmitsClass()
    {
        var cut = Render<FlareHidden>(p => p
            .Add(x => x.Only, Breakpoint.Xxl).Add(x => x.Invert, true).AddChildContent("<i>x</i>"));
        Assert.Contains("flare-hidden--invert-only-xxl", cut.Markup);
    }
}

public class C_ColXxlTests : FlareTestContext
{
    [Fact]
    public void Xxl_Span_EmitsCssVariable()
    {
        var cut = Render<FlareCol>(p => p.Add(x => x.Xxl, 4).AddChildContent("<i>x</i>"));
        Assert.Contains("--flare-col-span-xxl:4", cut.Find(".flare-col").GetAttribute("style"));
    }
}
