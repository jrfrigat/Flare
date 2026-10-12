using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartPointerPerformanceTests : FlareTestContext
{
    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Bar)]
    public void CrossingHitsDoesNotReadDrawingOrTableDataAgain(ChartType type)
    {
        var values = new CountedValues(1000);
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, new ChartData([new("Values", values)]))
            .Add(x => x.Type, type).Add(x => x.DataTable, true).Add(x => x.Zoomable, true));
        values.Reads = 0;
        for (int i = 400; i < 420; i++)
        {
            cut.Find($"[data-flare-chart-hit='category:{i}']").MouseEnter(new MouseEventArgs { ClientX = i, ClientY = 100 });
            Assert.Contains($"Values: {i}", cut.Find("[role=tooltip]").TextContent);
            cut.Find($"[data-flare-chart-hit='category:{i}']").MouseLeave();
        }
        Assert.Equal(0, values.Reads);
        cut.Render();
        Assert.True(values.Reads > 0);
    }

    private sealed class CountedValues(int count) : IReadOnlyList<double>
    {
        public int Reads { get; set; }
        public int Count => count;
        public double this[int index] { get { Reads++; return index; } }
        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < count; i++) yield return this[i];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

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
