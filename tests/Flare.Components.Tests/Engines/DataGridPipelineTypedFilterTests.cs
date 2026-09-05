using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class DataGridPipelineTypedFilterTests
{
    private record Row(int Score, DateTime Date);

    private static readonly Row[] _rows =
    [
        new(9,   new DateTime(2026, 1, 1)),
        new(85,  new DateTime(2026, 6, 1)),
        new(100, new DateTime(2026, 12, 1)),
    ];

    private static DataGridColumnStrategies<Row> Strategies() => new()
    {
        FilterSelectors = new Dictionary<string, Func<Row, object?>>
        {
            ["Score"] = r => r.Score,
            ["Date"] = r => r.Date,
        },
        ColumnTypes = new Dictionary<string, ColumnDataType>
        {
            ["Score"] = ColumnDataType.Number,
            ["Date"] = ColumnDataType.DateTime,
        },
    };

    private static List<int> RunScores(DataGridFilter f) =>
        DataGridPipeline<Row>.Execute(_rows, [], [f], null, null, null, null, 0, 100, Strategies())
            .Items.Select(r => r.Score).ToList();

    [Fact]
    public void Number_GreaterThan_ComparesNumerically_NotLexically()
    {
        // Lexically "9" > "85" and "100" < "85"; numerically only 100 > 85.
        Assert.Equal([100], RunScores(new DataGridFilter("Score", FilterOperator.GreaterThan, "85")));
    }

    [Fact]
    public void Number_Equals_IgnoresNumericFormatting()
    {
        // "85" != "85.0" as strings; equal as numbers.
        Assert.Equal([85], RunScores(new DataGridFilter("Score", FilterOperator.Equals, "85.0")));
    }

    [Fact]
    public void Number_Between_IsInclusive()
    {
        Assert.Equal([85], RunScores(new DataGridFilter("Score", FilterOperator.Between, "10", "90")));
    }

    [Fact]
    public void Date_GreaterThan_ComparesChronologically_FromIsoInput()
    {
        // ISO date input vs DateTime cells; June and December are after 2026-05-01.
        var n = DataGridPipeline<Row>.Execute(_rows, [], [new DataGridFilter("Date", FilterOperator.GreaterThan, "2026-05-01")],
            null, null, null, null, 0, 100, Strategies()).Items.Count();
        Assert.Equal(2, n);
    }

    [Fact]
    public void Date_Between_SelectsRange()
    {
        var n = DataGridPipeline<Row>.Execute(_rows, [], [new DataGridFilter("Date", FilterOperator.Between, "2026-03-01", "2026-09-01")],
            null, null, null, null, 0, 100, Strategies()).Items.Count();
        Assert.Equal(1, n); // only June
    }
}
