using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartPointerPerformanceTests : FlareTestContext
{
    private static readonly ChartData Data = new([new("Values", Enumerable.Range(0, 1000).Select(i => (double)i).ToArray())]);

    [Fact]
    public async Task IdleMovementDoesNotRenderOrPlaceDetails()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        int renders = cut.RenderCount;
        for (int i = 0; i < 120; i++) await cut.Instance.ChartPointerMoved(300, i);
        Assert.Equal(renders, cut.RenderCount);
        Assert.Empty(module.Invocations["positionAnchoredPanel"]);
    }

    [Fact]
    public async Task VisibleMovementPlacesLatestAnchorWithoutRenderingGeometry()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        cut.Find("[data-flare-chart-hit='category:500']").MouseEnter(new MouseEventArgs { ClientX = 300, ClientY = 50 });
        int renders = cut.RenderCount;
        for (int i = 0; i < 120; i++) await cut.Instance.ChartPointerMoved(300, 100 + i);
        Assert.Equal(renders, cut.RenderCount);
        Assert.Equal(121, module.Invocations["positionAnchoredPanel"].Count);
        using var json = System.Text.Json.JsonDocument.Parse((string)module.Invocations["positionAnchoredPanel"][^1].Arguments[3]!);
        Assert.Equal(219, json.RootElement.GetProperty("anchorRect").GetProperty("y").GetDouble());
        await cut.Instance.ChartPointerMoved(double.NaN, 100);
        Assert.Equal(121, module.Invocations["positionAnchoredPanel"].Count);
    }
}
