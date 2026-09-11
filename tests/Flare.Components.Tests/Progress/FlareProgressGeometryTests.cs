namespace Flare.Components.Tests;

public sealed class FlareProgressGeometryTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-progress-geometry.js";

    /// <summary>
    /// The ring is the only shape that needs measuring, and now the only component that asks for it.
    /// A linear bar resolving no geometry is the point of the split: it neither injects the service
    /// nor calls it, so a page full of bars pays nothing for the ring's observer.
    /// </summary>
    [Fact]
    public void OnlyTheCircularIndicatorObservesGeometry()
    {
        var module = JSInterop.SetupModule(Module);

        Render<FlareProgressLinear>(p => p.Add(x => x.Value, 25d));
        Assert.Empty(module.Invocations["observe"]);

        var ring = Render<FlareProgressCircular>(p => p.Add(x => x.Value, 25d));
        Assert.Single(module.Invocations["observe"]);

        // Re-rendering keeps the one observer rather than stacking another on every update.
        ring.Render(p => p.Add(x => x.Value, 75d).Add(x => x.Size, TrackSize.Xl));
        Assert.Single(module.Invocations["observe"]);
        Assert.Equal("75", ring.Find("svg").GetAttribute("data-value"));
    }

    [Fact]
    public void ALinearBarDoesNotRequireTheGeometryService()
    {
        // No SetupModule here: an unconfigured module call throws in bUnit, so this passing is the
        // assertion - the linear component never reaches for the ring's script.
        var cut = Render<FlareProgressLinear>(p => p.Add(x => x.Value, 40d));
        Assert.Empty(cut.FindAll("svg"));
    }

    [Fact]
    public async Task DisposalDisconnectsTheGeometryObserver()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = Render<FlareProgressCircular>();
        await cut.Instance.DisposeAsync();
        Assert.Single(module.Invocations["unobserve"]);
    }

    [Fact]
    public void IndeterminateModeRemovesTheBrowserValueAndKeepsPercentPaths()
    {
        var cut = Render<FlareProgressCircular>(p => p.Add(x => x.Value, 37.25d));
        Assert.Equal("37.25", cut.Find("svg").GetAttribute("data-value"));
        cut.Render(p => p.Add(x => x.Value, (double?)null));
        Assert.Null(cut.Find("svg").GetAttribute("data-value"));
        foreach (var circle in cut.FindAll("circle"))
        {
            Assert.Equal("100", circle.GetAttribute("pathLength"));
            Assert.Null(circle.GetAttribute("stroke-dasharray"));
        }
    }
}
