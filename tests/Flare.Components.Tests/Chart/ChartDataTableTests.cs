namespace Flare.Components.Tests;

public class ChartDataTableTests : FlareTestContext
{
    public static IEnumerable<object[]> Types => Enum.GetValues<ChartType>().Select(type => new object[] { type });

    [Theory]
    [MemberData(nameof(Types))]
    public void EveryChartTypeExposesItsCompleteUnderlyingData(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.DataTable, true)
            .Add(x => x.Title, "Full data")
            .Add(x => x.Data, new ChartData([
                new ChartSeries("First", [1, 2, 3], Points: [new(1, 2, 3), new(4, 5, 6)])], ["A"])));

        Assert.Equal(type is ChartType.Scatter or ChartType.Bubble ? 2 : 3, cut.FindAll("table tbody tr").Count);
        Assert.Equal("Full data", cut.Find("table caption").TextContent);
        Assert.All(cut.FindAll("thead th"), x => Assert.Equal("col", x.GetAttribute("scope")));
        Assert.All(cut.FindAll("tbody th"), x => Assert.Equal("row", x.GetAttribute("scope")));
    }

    [Theory]
    [InlineData("en", "Category", "No value", "No data")]
    [InlineData("ru", "\u041a\u0430\u0442\u0435\u0433\u043e\u0440\u0438\u044f", "\u041d\u0435\u0442 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f", "\u041d\u0435\u0442 \u0434\u0430\u043d\u043d\u044b\u0445")]
    public void TableFeedbackFollowsTheCurrentUiCulture(string culture, string category, string missing, string empty)
    {
        var previous = System.Globalization.CultureInfo.CurrentUICulture;
        try
        {
            System.Globalization.CultureInfo.CurrentUICulture = new(culture);
            var cut = Render<FlareChart>(p => p.Add(x => x.DataTable, true)
                .Add(x => x.Data, new ChartData([new ChartSeries("First", [double.NaN])])));
            Assert.Equal(category, cut.Find("thead th").TextContent);
            Assert.Equal(missing, cut.Find("tbody td").TextContent);
            cut.Render(p => p.Add(x => x.Data, (ChartData?)null));
            Assert.Equal(empty, cut.Find("tbody td").TextContent);
        }
        finally { System.Globalization.CultureInfo.CurrentUICulture = previous; }
    }

    [Fact]
    public void HidingSeriesDoesNotRemoveTheirValuesFromTheTable()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.DataTable, true)
            .Add(x => x.Data, new ChartData([new ChartSeries("First", [10, 20])])));
        var before = cut.Find("table").OuterHtml;
        cut.Find($".{Css.Classes.Chart.LegendItem}").Click();
        Assert.Equal(before, cut.Find("table").OuterHtml);
    }

    [Fact]
    public void ShortLabelsDoNotTruncateValuesAndMissingValuesAreReadable()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.DataTable, true)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("First", [0, double.NaN, double.PositiveInfinity]),
                new ChartSeries("Second", [42])], ["A <tspan> & Q"])));

        Assert.Equal(3, cut.FindAll("table tbody tr").Count);
        Assert.Equal("A <tspan> & Q", cut.Find("tbody th").TextContent);
        Assert.Empty(cut.FindAll("table tspan"));
        Assert.Equal("0", cut.Find("tbody td").TextContent);
        Assert.DoesNotContain("NaN", cut.Find("table").TextContent);
        Assert.DoesNotContain("Infinity", cut.Find("table").TextContent);
        Assert.All(cut.FindAll("tbody td").Skip(2), x => Assert.False(string.IsNullOrWhiteSpace(x.TextContent)));
    }

    [Theory]
    [InlineData(ChartType.Scatter, 4)]
    [InlineData(ChartType.Bubble, 5)]
    public void PointCoordinatesAndBubbleWeightsAreAvailable(ChartType type, int columns)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.DataTable, true)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("Points", [], Points: [new(1.25, 2.5, 9), new(3, 4, 16)])])));

        Assert.Equal(2, cut.FindAll("table tbody tr").Count);
        Assert.Equal(columns, cut.FindAll("table thead th").Count);
        Assert.Contains("1.25", cut.Find("tbody tr").TextContent);
        Assert.Contains("2.5", cut.Find("tbody tr").TextContent);
        if (type == ChartType.Bubble) Assert.Equal("9", cut.FindAll("tbody tr")[0].LastElementChild!.TextContent);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void TableHasCaptionAndScopedHeadersWithoutTitle(string? title)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.DataTable, true).Add(x => x.Title, title)
            .Add(x => x.Data, new ChartData([new ChartSeries("First", [1, 2])])));

        Assert.False(string.IsNullOrWhiteSpace(cut.Find("table caption").TextContent));
        Assert.All(cut.FindAll("thead th"), x => Assert.Equal("col", x.GetAttribute("scope")));
        Assert.All(cut.FindAll("tbody th"), x => Assert.Equal("row", x.GetAttribute("scope")));
    }

    [Fact]
    public void EmptyDataStillHasAccessibleTableFeedback()
    {
        foreach (var data in new ChartData?[] { null, new([]), new([new ChartSeries("Empty", [])]) })
        {
            var cut = Render<FlareChart>(p => p.Add(x => x.DataTable, true).Add(x => x.Data, data));
            Assert.False(string.IsNullOrWhiteSpace(cut.Find("table tbody").TextContent));
        }
    }
}
