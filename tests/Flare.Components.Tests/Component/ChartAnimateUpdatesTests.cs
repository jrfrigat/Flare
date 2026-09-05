namespace Flare.Components.Tests.Component;

/// <summary>
/// A chart replaces its whole drawing when Data changes, so every point is somewhere else the instant
/// the new dataset arrives - twenty points teleport, and the one that actually changed is invisible
/// among them. AnimateUpdates walks the geometry to its new place instead.
///
/// The walk itself belongs to the browser (flare-chart-motion.js watches the plot's attributes and
/// interpolates them), so what a rendering test can hold is the CONTRACT with it: that a chart asks to
/// be watched exactly when it was told to animate, asks once, and stops when it is turned off or goes
/// away. That contract is the part that can silently break - a component that never places the call
/// looks identical in the markup.
/// </summary>
public sealed class ChartAnimateUpdatesTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-chart-motion.js";

    private static ChartData Series(params double[] values) =>
        new([new ChartSeries("live", values)], values.Select((_, i) => i.ToString()).ToList());

    private IRenderedComponent<FlareChart> RenderChart(bool animateUpdates) =>
        Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.AnimateUpdates, animateUpdates)
            .Add(x => x.Data, Series(10, 20, 30)));

    // The chart draws itself without the browser, and that stays true for every chart that did not ask
    // for this: a page of forty charts loads no module and makes no call.
    [Fact]
    public void AChartThatDoesNotAnimateUpdatesAsksTheBrowserForNothing()
    {
        var module = JSInterop.SetupModule(Module);

        RenderChart(animateUpdates: false);

        Assert.Empty(module.Invocations["observePlot"]);
    }

    [Fact]
    public void AnimatingUpdatesWatchesThePlot()
    {
        var module = JSInterop.SetupModule(Module);

        RenderChart(animateUpdates: true);

        Assert.Single(module.Invocations["observePlot"]);
    }

    // Once for the life of the chart, not once per render: the browser watches the plot, so a repaint
    // has nothing to re-register. A call per update would be the cost this design exists to avoid.
    [Fact]
    public void ItIsAskedOnceHoweverOftenTheDataChanges()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderChart(animateUpdates: true);

        cut.Render(p => p.Add(x => x.Data, Series(40, 50, 60)));
        cut.Render(p => p.Add(x => x.Data, Series(70, 80, 90)));

        Assert.Single(module.Invocations["observePlot"]);
    }

    [Fact]
    public void TurningItOffStopsTheWatch()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderChart(animateUpdates: true);

        cut.Render(p => p.Add(x => x.AnimateUpdates, false));

        Assert.Single(module.Invocations["unobservePlot"]);
    }

    [Fact]
    public async Task AChartThatGoesAwayStopsTheWatch()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = RenderChart(animateUpdates: true);

        await cut.Instance.DisposeAsync();

        Assert.Single(module.Invocations["unobservePlot"]);
    }
}
