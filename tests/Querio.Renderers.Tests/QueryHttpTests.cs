using Querio.Http;
using Querio.Sql;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers the one target that goes both ways. Every other target translates a query into somebody
/// else's language and stops there; this one has to be able to read back exactly what it wrote, so
/// the tests are mostly about that rather than about how the text looks.
/// </summary>
public sealed class QueryHttpTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    /// <summary>
    /// The property that matters: writing, reading and writing again lands on the same text. Tree
    /// equality would be the wrong assertion - reading folds away nesting the text did not carry -
    /// but meaning has to survive, and identical text is the honest measure of that.
    /// </summary>
    private static void RoundTrips(QuerySpec spec)
    {
        var written = QueryHttp.Render(spec, Schema);
        var read = QueryHttp.Parse(written, Schema);

        Assert.Equal(written, QueryHttp.Render(read, Schema));
    }

    [Fact]
    public void WritesAQueryAsSomethingAPersonCanRead()
    {
        Assert.Equal(
            "from=requests:r" +
            "&select=r.route as route,count() as total" +
            "&where=r.timestamp ge -30d and r.error eq 'true'" +
            "&groupby=r.route" +
            "&orderby=total desc" +
            "&top=20",
            QueryHttp.Render(Aggregate(), Schema));
    }

    [Fact]
    public void ReadsBackWhatItWrote()
    {
        RoundTrips(Aggregate());
    }

    [Fact]
    public void ReadsBackAQueryThatSaysSomethingToADatabaseToo()
    {
        // The real proof that reading recovered the query rather than something that merely writes
        // the same: hand both to a target that knows nothing about this format.
        var spec = Aggregate();
        var read = QueryHttp.Parse(QueryHttp.Render(spec, Schema), Schema);

        Assert.Equal(
            SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql,
            SqlRenderer.Render(read, Schema, PostgreSqlDialect.Instance).Sql);
    }

    [Fact]
    public void CarriesJoinsWithTheirRelationAndKind()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k")
            .LeftJoin("users", "u", "apiKey_owner")
            .Select("u", "name", "owner")
            .Build();

        Assert.Contains("join=apiKeys:k:request_apiKey", QueryHttp.Render(spec, Schema), StringComparison.Ordinal);
        Assert.Contains("join=users:u:apiKey_owner:left", QueryHttp.Render(spec, Schema), StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesAJoinThatNamesTheSideItAttachesTo()
    {
        // Two paths reach the same entity, so which one this join hangs off has to survive.
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .Join("users", "sender", "transfer_sender")
            .Join("users", "recipient", "transfer_recipient", from: "t")
            .Select("sender", "name", "from")
            .Select("recipient", "name", "to")
            .Build();

        Assert.Contains(":inner:t", QueryHttp.Render(spec, Schema), StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesAJoinWrittenOutAsFieldMatches()
    {
        var spec = QueryBuilder.From(Schema, "orders", "o")
            .JoinOn("orderLines", "l",
            [
                new QueryJoinCondition(new QueryFieldRef("o", "tenantId"), new QueryFieldRef("l", "tenantId")),
                new QueryJoinCondition(new QueryFieldRef("o", "number"), new QueryFieldRef("l", "orderNumber")),
            ])
            .Select("l", "sku", "sku")
            .Build();

        Assert.Contains(
            "join=orderLines:l:on(o.tenantId=l.tenantId,o.number=l.orderNumber)",
            QueryHttp.Render(spec, Schema),
            StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesEveryShapeOfCondition()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Where(f => f
                .Between("r", "durationMs", 10, 500)
                .In("r", "status", new[] { 200, 404 })
                .NotIn("r", "route", new[] { "/health" })
                .IsNotNull("r", "apiKeyId")
                .Contains("r", "route", "api")
                .Since("r", "timestamp", 2, QueryTimeUnit.Week)
                .AnyOf(g => g.Equal("r", "error", true).Equal("r", "cacheHit", false)))
            .Build();

        var written = QueryHttp.Render(spec, Schema);

        Assert.Contains("r.durationMs between '10' and '500'", written, StringComparison.Ordinal);
        Assert.Contains("r.status in ('200','404')", written, StringComparison.Ordinal);
        Assert.Contains("r.route not in ('/health')", written, StringComparison.Ordinal);
        Assert.Contains("r.apiKeyId is not null", written, StringComparison.Ordinal);
        Assert.Contains("r.timestamp ge -2w", written, StringComparison.Ordinal);
        Assert.Contains("(r.error eq 'true' or r.cacheHit eq 'false')", written, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesAggregatesAndTheirDetail()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .SelectAndGroupByDay("r", "timestamp", "day")
            .Count("r", "route", distinct: true, outputAlias: "routes")
            .Percentile("r", "durationMs", 0.95, "p95")
            .Having(f => f.SelectGreaterThan("routes", 3))
            .OrderBySelectDescending("p95")
            .Build();

        var written = QueryHttp.Render(spec, Schema);

        Assert.Contains("r.timestamp:day as day", written, StringComparison.Ordinal);
        Assert.Contains("count(distinct r.route) as routes", written, StringComparison.Ordinal);
        Assert.Contains("percentile(r.durationMs, 0.95) as p95", written, StringComparison.Ordinal);
        Assert.Contains("having=routes gt '3'", written, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesCallsToDeclaredFunctions()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "name")
            .Where(f => f.EqualCall(QueryFunctionCall.OfFields("upper", "u", "name"), "ANN"))
            .Build();

        var written = QueryHttp.Render(spec, Schema);

        Assert.Contains("select=upper(u.name) as name", written, StringComparison.Ordinal);
        Assert.Contains("where=upper(u.name) eq 'ANN'", written, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesATableFunctionStandingWhereAnEntityWould()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(7, QueryTimeUnit.Day)), "a")
            .Select("a", "name", "name")
            .Build();

        Assert.StartsWith("from=activeUsers(-7d):a", QueryHttp.Render(spec, Schema), StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesAValueThatContainsTheCharactersTheFormatUses()
    {
        // A value is data, not syntax; quotes, commas and brackets inside one have to come back.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Where(f => f.Equal("r", "route", "it's /a,b (c) : d"))
            .Build();

        var read = QueryHttp.Parse(QueryHttp.Render(spec, Schema), Schema);

        Assert.Equal("it's /a,b (c) : d", read.Where!.Conditions[0].Value!.Value);
        RoundTrips(spec);
    }

    [Fact]
    public void CarriesPagingAndDistinct()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Distinct()
            .Offset(40)
            .Limit(10)
            .Build();

        var written = QueryHttp.Render(spec, Schema);

        Assert.Contains("distinct=true", written, StringComparison.Ordinal);
        Assert.Contains("top=10", written, StringComparison.Ordinal);
        Assert.Contains("skip=40", written, StringComparison.Ordinal);
        RoundTrips(spec);
    }

    [Fact]
    public void SurvivesBeingPutInAUrlAndTakenOutAgain()
    {
        var spec = Aggregate();

        var url = QueryHttp.ToUri(spec, Schema, "/reports");
        var read = QueryHttp.ParseUri(url, Schema);

        Assert.StartsWith("/reports?from=requests%3Ar", url, StringComparison.Ordinal);
        Assert.Equal(QueryHttp.Render(spec, Schema), QueryHttp.Render(read, Schema));
    }

    [Fact]
    public void RefusesTextThatSaysNothingItCanHold()
    {
        Assert.Throws<QueryParseException>(() => QueryHttp.Parse("select=r.route"));
        Assert.Throws<QueryParseException>(() => QueryHttp.Parse("from=requests:r&where=r.route like 'a'"));
        Assert.Throws<QueryParseException>(() => QueryHttp.Parse("from=requests:r&where=(r.error eq 'true'"));
        Assert.Throws<QueryParseException>(() => QueryHttp.Parse("from=requests:r&top=lots"));
        Assert.Throws<QueryParseException>(() => QueryHttp.Parse("from=requests:r&where=r.timestamp ge -30x"));
    }

    [Fact]
    public void SaysWhereItStoppedReading()
    {
        var error = Assert.Throws<QueryParseException>(
            () => QueryHttp.Parse("from=requests:r&where=r.route zz 'a'"));

        Assert.NotNull(error.Position);
        Assert.Contains("'zz' is not an operator", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ChecksWhatItReadAgainstTheSchemaWhenGivenOne()
    {
        const string text = "from=requests:r&select=r.nothingLikeThis";

        // Without a schema it reads fine - the text is well formed, it just means nothing here.
        Assert.NotNull(QueryHttp.Parse(text));
        Assert.Throws<QueryValidationException>(() => QueryHttp.Parse(text, Schema));
    }

    private static QuerySpec Aggregate() => QueryBuilder.From(Schema, "requests", "r")
        .Select("r", "route", "route")
        .CountRows("total")
        .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day).Equal("r", "error", true))
        .GroupBy("r", "route")
        .OrderBySelectDescending("total")
        .Limit(20)
        .Build();
}
