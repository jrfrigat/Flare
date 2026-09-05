using Flare.Components;
using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class BrowserViewportServiceTests : FlareTestContext
{
    private IBrowserViewportService Viewport => Services.GetRequiredService<IBrowserViewportService>();

    [Fact]
    public async Task GetBreakpoint_FallsBackToMd_WhenNoMeasurableWidth()
    {
        Assert.Equal(Breakpoint.Md, await Viewport.GetBreakpointAsync(Xunit.TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetViewportSize_ReturnsDefault_WhenNoBrowser()
    {
        var size = await Viewport.GetViewportSizeAsync(Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(0, size.Width);
        Assert.Equal(0, size.Height);
    }

    [Fact]
    public async Task SubscribeBreakpoint_FiresImmediately_AndReturnsDisposableToken()
    {
        Breakpoint? seen = null;
        var token = await Viewport.SubscribeBreakpointAsync(
            bp => seen = bp, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.NotNull(token);
        Assert.Equal(Breakpoint.Md, seen); // immediate fire with the current (fallback) tier

        await token.DisposeAsync();        // must not throw
    }

    [Fact]
    public async Task SubscribeBreakpoint_SuppressesImmediate_WhenNotRequested()
    {
        var fired = false;
        var token = await Viewport.SubscribeBreakpointAsync(
            _ => fired = true, fireImmediately: false,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.False(fired); // no synthetic first notification
        await token.DisposeAsync();
    }

    [Fact]
    public async Task Subscribe_FullChange_ReportsImmediateFlag()
    {
        ViewportChange? change = null;
        var token = await Viewport.SubscribeAsync(
            c => change = c, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.NotNull(change);
        Assert.True(change!.Value.IsImmediate);
        Assert.True(change.Value.BreakpointChanged);
        await token.DisposeAsync();
    }

    [Fact]
    public async Task MatchesAsync_ReturnsFalse_WhenNoBrowser()
    {
        Assert.False(await Viewport.MatchesAsync(
            "(min-width: 600px)", Xunit.TestContext.Current.CancellationToken));
    }
}
