using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartTooltipTests : FlareTestContext
{
    [Theory]
    [InlineData(ChartType.Line, "rect[fill='transparent']")]
    [InlineData(ChartType.Donut, "path[fill='transparent']")]
    public async Task TooltipFollowsViewportPointerAndReleasesOnLeave(ChartType type, string hitSelector)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, type)
            .Add(x => x.Data, new ChartData([new ChartSeries("Revenue", [10, 20])], ["Jan", "Feb"])));

        cut.Find(hitSelector).MouseEnter(new MouseEventArgs { ClientX = 321.5, ClientY = 456.25 });
        AssertAnchor(321.5, 456.25);
        int renders = cut.RenderCount;
        await cut.Instance.ChartPointerMoved(331, 479);
        Assert.Equal(renders, cut.RenderCount);
        AssertAnchor(331, 479);
        Assert.Equal(2, module.Invocations["positionAnchoredPanel"].Count);

        await cut.Instance.ChartPointerMoved(331, 479);
        Assert.Equal(2, module.Invocations["positionAnchoredPanel"].Count);
        cut.Find(".flare-chart__plot").PointerLeave();
        Assert.Empty(cut.FindAll(".flare-chart__tooltip"));
        Assert.Single(module.Invocations["removeAnchoredPanel"]);

        void AssertAnchor(double x, double y)
        {
            var json = (string)module.Invocations["positionAnchoredPanel"][^1].Arguments[3]!;
            using var options = JsonDocument.Parse(json);
            var rect = options.RootElement.GetProperty("anchorRect");
            Assert.Equal(x, rect.GetProperty("x").GetDouble());
            Assert.Equal(y, rect.GetProperty("y").GetDouble());
            Assert.Equal(8, options.RootElement.GetProperty("gap").GetDouble());
            Assert.False(options.RootElement.TryGetProperty("anchorPoint", out _));
        }
    }
}
