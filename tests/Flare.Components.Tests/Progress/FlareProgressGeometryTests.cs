namespace Flare.Components.Tests;

public sealed class FlareProgressGeometryTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-progress-geometry.js";

    [Fact]
    public void GeometryIsObservedOnlyWhileCircular()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = Render<FlareProgress>();
        Assert.Empty(module.Invocations["observe"]);

        cut.Render(p => p.Add(x => x.Variant, ProgressVariant.Circular).Add(x => x.Value, 25d));
        cut.Render(p => p.Add(x => x.Value, 75d).Add(x => x.Size, TrackSize.Xl));
        Assert.Single(module.Invocations["observe"]);
        Assert.Equal("75", cut.Find("svg").GetAttribute("data-value"));

        cut.Render(p => p.Add(x => x.Variant, ProgressVariant.Linear));
        Assert.Single(module.Invocations["unobserve"]);
        cut.Render(p => p.Add(x => x.Variant, ProgressVariant.Circular));
        Assert.Equal(2, module.Invocations["observe"].Count);
    }

    [Fact]
    public async Task DisposalDisconnectsTheGeometryObserver()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = Render<FlareProgress>(p => p.Add(x => x.Variant, ProgressVariant.Circular));
        await cut.Instance.DisposeAsync();
        Assert.Single(module.Invocations["unobserve"]);
    }

    [Fact]
    public void IndeterminateModeRemovesTheBrowserValueAndKeepsPercentPaths()
    {
        var cut = Render<FlareProgress>(p => p.Add(x => x.Variant, ProgressVariant.Circular).Add(x => x.Value, 37.25d));
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
