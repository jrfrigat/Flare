using Querio.Tests;
using Querio.Text;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers the description renderer. It exists partly to be useful and partly as proof: it walks the
/// same query through the same shared renderer base, yet produces no query at all - which is the
/// clearest evidence the model is not tied to generating text for a database.
/// </summary>
public sealed class QueryDescriberTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    [Fact]
    public void DescribesAnAggregateQueryAsASentence()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .CountRows("total")
            .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day).Equal("r", "error", true))
            .GroupBy("r", "route")
            .OrderBySelectDescending("total")
            .Limit(20)
            .Build();

        var description = QueryDescriber.Describe(spec, Schema);

        Assert.Equal(
            "From Requests, showing Route and the number of rows, " +
            "where (Timestamp is at least the last 30 days and Error is true), " +
            "grouped by Route, ordered by the number of rows descending, first 20",
            description);
    }

    [Fact]
    public void UsesTheLabelsAPersonPickedRatherThanStorageNames()
    {
        // The schema calls it "durationMs" but labels it "Duration, ms"; a description is for people.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "durationMs")
            .Build();

        var description = QueryDescriber.Describe(spec, Schema);

        Assert.Contains("Duration, ms", description, StringComparison.Ordinal);
        Assert.DoesNotContain("durationMs", description, StringComparison.Ordinal);
    }

    [Fact]
    public void ReadsARelativeWindowAsAWindowRatherThanADate()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .Where(f => f.Since("r", "timestamp", 1, QueryTimeUnit.Week))
            .Build();

        Assert.Contains("the last 1 week", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
    }

    [Fact]
    public void SpellsOutAPercentileWithItsRank()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Percentile("r", "durationMs", 0.95, "p95")
            .Build();

        Assert.Contains("the 95 percentile of Duration, ms", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
    }

    [Fact]
    public void DescribesAFunctionCallByItsLabel()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "name")
            .Build();

        Assert.Contains("Upper case of Name", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
    }

    [Fact]
    public void DescribesATimeSeriesByItsPeriod()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .SelectAndGroupByDay("r", "timestamp", "day")
            .CountRows("total")
            .Build();

        Assert.Contains("Timestamp by day", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
    }

    [Fact]
    public void TakesItsWordingFromTheLabelsItIsGiven()
    {
        // Swapping the wording is all a translation needs; the walk stays the same.
        var labels = QueryDescriptionLabels.Default with { From = "из", Showing = "показываем", Where = "где" };
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .Where(f => f.Equal("r", "error", true))
            .Build();

        var description = QueryDescriber.Describe(spec, Schema, labels);

        Assert.StartsWith("Из Requests, показываем Route, где", description, StringComparison.Ordinal);
    }
}
