using Querio.Sql;
using Querio.Text;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers reading a description back into the query it describes. A description that could not be
/// read back would be a summary rather than a translation, so these tests are about recovery rather
/// than about how the sentence reads.
/// </summary>
public sealed class QueryTextRoundTripTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    /// <summary>
    /// Describing, reading and describing again lands on the same sentence. Tree equality would be
    /// the wrong assertion - reading folds away nesting the words did not carry - but the meaning
    /// has to survive, and identical text is the honest measure of that.
    /// </summary>
    private static void RoundTrips(QuerySpec spec)
    {
        var described = QueryDescriber.Describe(spec, Schema);
        var read = QueryDescriber.Parse(described, Schema);

        Assert.Equal(described, QueryDescriber.Describe(read, Schema));
    }

    [Fact]
    public void ReadsBackAnAggregateQuery()
    {
        RoundTrips(Aggregate());
    }

    [Fact]
    public void RecoversAQueryADatabaseStillAgreesWith()
    {
        // The real proof that the words carried the query rather than a description of it: hand both
        // to a target that has never seen a sentence.
        var spec = Aggregate();
        var read = QueryDescriber.Parse(QueryDescriber.Describe(spec, Schema), Schema);

        Assert.Equal(
            SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql,
            SqlRenderer.Render(read, Schema, PostgreSqlDialect.Instance).Sql);
    }

    [Fact]
    public void ReadsBackTheJoinsAndWhichSourceEachFieldCameFrom()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k")
            .LeftJoin("users", "u", "apiKey_owner")
            .Select("r", "route", "route")
            .Select("u", "name", "owner")
            .Where(f => f.Equal("k", "name", "alpha"))
            .Build();

        var described = QueryDescriber.Describe(spec, Schema);

        Assert.Contains("joined with API keys (k) through request_apiKey", described, StringComparison.Ordinal);
        Assert.Contains("keeping unmatched rows on the left", described, StringComparison.Ordinal);
        // With more than one source in play, every field says which one it came from.
        Assert.Contains("Route (r)", described, StringComparison.Ordinal);
        Assert.Contains("Name (u)", described, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackTwoPathsToTheSameEntity()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .Join("users", "sender", "transfer_sender")
            .Join("users", "recipient", "transfer_recipient", from: "t")
            .Select("sender", "name", "from")
            .Select("recipient", "name", "to")
            .Build();

        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackAJoinWrittenOutAsFieldMatches()
    {
        var spec = QueryBuilder.From(Schema, "orders", "o")
            .JoinOn("orderLines", "l",
            [
                new QueryJoinCondition(new QueryFieldRef("o", "tenantId"), new QueryFieldRef("l", "tenantId")),
                new QueryJoinCondition(new QueryFieldRef("o", "number"), new QueryFieldRef("l", "orderNumber")),
            ])
            .Select("l", "sku", "sku")
            .Build();

        Assert.Contains("matching Tenant (o) is Tenant (l)", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackEveryShapeOfCondition()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Where(f => f
                .Between("r", "durationMs", 10, 500)
                .In("r", "status", new[] { 200, 404 })
                .IsNotNull("r", "apiKeyId")
                .Contains("r", "route", "api")
                .Since("r", "timestamp", 2, QueryTimeUnit.Week)
                .AnyOf(g => g.Equal("r", "error", true).Equal("r", "cacheHit", false)))
            .Build();

        var described = QueryDescriber.Describe(spec, Schema);

        Assert.Contains("Duration, ms is between \"10\" and \"500\"", described, StringComparison.Ordinal);
        Assert.Contains("Status is one of \"200\" or \"404\"", described, StringComparison.Ordinal);
        Assert.Contains("API key is not empty", described, StringComparison.Ordinal);
        Assert.Contains("Timestamp is at least the last 2 weeks", described, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackALabelThatContainsAComma()
    {
        // "Duration, ms" is a single label, not two words separated by a list comma. Reading matches
        // the longest thing the schema knows, which is what keeps that from going wrong.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "durationMs", "duration")
            .Select("r", "route", "route")
            .Build();

        var read = QueryDescriber.Parse(QueryDescriber.Describe(spec, Schema), Schema);

        Assert.Equal(2, read.Select.Count);
        Assert.Equal("durationMs", read.Select[0].Field!.Field);
        Assert.Equal("route", read.Select[1].Field!.Field);
    }

    [Fact]
    public void ReadsBackAggregatesAndTheirDetail()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .SelectAndGroupByDay("r", "timestamp", "day")
            .Count("r", "route", distinct: true, outputAlias: "routes")
            .Percentile("r", "durationMs", 0.95, "p95")
            .Having(f => f.SelectGreaterThan("routes", 3))
            .OrderBySelectDescending("p95")
            .Build();

        var described = QueryDescriber.Describe(spec, Schema);

        Assert.Contains("the number of distinct Route called routes", described, StringComparison.Ordinal);
        Assert.Contains("the 95 percentile of Duration, ms called p95", described, StringComparison.Ordinal);
        Assert.Contains("Timestamp by day called day", described, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackCallsToDeclaredFunctions()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "name")
            .Where(f => f.EqualCall(QueryFunctionCall.OfFields("upper", "u", "name"), "ANN"))
            .Build();

        Assert.Contains("Upper case of Name", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackATableFunctionStandingWhereAnEntityWould()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(7, QueryTimeUnit.Day)), "a")
            .Select("a", "name", "name")
            .Build();

        Assert.StartsWith("From Active users of the last 7 days (a)", QueryDescriber.Describe(spec, Schema), StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackPagingAndDistinct()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Distinct()
            .Offset(40)
            .Limit(10)
            .Build();

        var described = QueryDescriber.Describe(spec, Schema);

        Assert.Contains("without duplicates", described, StringComparison.Ordinal);
        Assert.Contains("skipping 40", described, StringComparison.Ordinal);
        Assert.Contains("first 10", described, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void ReadsBackAValueThatContainsTheCharactersTheSentenceUses()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Where(f => f.Equal("r", "route", "a \"quoted\", bracketed (thing) and more"))
            .Build();

        var read = QueryDescriber.Parse(QueryDescriber.Describe(spec, Schema), Schema);

        Assert.Equal("a \"quoted\", bracketed (thing) and more", read.Where!.Conditions[0].Value!.Value);
    }

    [Fact]
    public void ReadsASentenceWrittenInAnotherLanguage()
    {
        // The words are the caller's, so a query written in them reads back in them. Nothing about
        // the walk knows which language it is in.
        var labels = QueryDescriptionLabels.Default with
        {
            From = "из",
            Showing = "показываем",
            Where = "где",
            Called = "как",
            And = "и",
        };
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .CountRows("total")
            .Where(f => f.Equal("r", "error", true))
            .Build();

        var described = QueryDescriber.Describe(spec, Schema, labels);
        var read = QueryDescriber.Parse(described, Schema, labels);

        Assert.StartsWith("Из Requests (r), показываем Route как route и", described, StringComparison.Ordinal);
        Assert.Equal(described, QueryDescriber.Describe(read, Schema, labels));
    }

    [Fact]
    public void RefusesWordsItDoesNotKnow()
    {
        Assert.Throws<QueryParseException>(() => QueryDescriber.Parse("From Nothing At All (n)", Schema));
        Assert.Throws<QueryParseException>(() => QueryDescriber.Parse("Requests (r), showing Route", Schema));
        Assert.Throws<QueryParseException>(
            () => QueryDescriber.Parse("From Requests (r), showing Route, where Route wobbles \"a\"", Schema));
    }

    private static QuerySpec Aggregate() => QueryBuilder.From(Schema, "requests", "r")
        .Select("r", "route")
        .CountRows("total")
        .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day).Equal("r", "error", true))
        .GroupBy("r", "route")
        .OrderBySelectDescending("total")
        .Limit(20)
        .Build();
}
