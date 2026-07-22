using Querio.Language;
using Querio.Sql;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers writing a query back out as text. The round trip here is deliberately weaker than the one
/// for the description or the query string: foreign-key sugar spends itself into joins, and nothing
/// in the query records that it was ever typed as a dot.
/// </summary>
public sealed class QueryLanguageWriterTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    /// <summary>
    /// Writing, reading and writing again lands on the same text. This is the property the editor
    /// depends on: a query built anywhere else opens in it and survives being read back.
    /// </summary>
    private static void RoundTrips(QuerySpec spec)
    {
        var written = QueryLanguage.Write(spec, Schema);
        var read = QueryLanguage.Parse(written, Schema);

        Assert.Equal(written, QueryLanguage.Write(read, Schema));
    }

    [Fact]
    public void WritesAQueryAPersonWouldRecognise()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .CountRows("total")
            .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day).Equal("r", "error", true))
            .GroupBy("r", "route")
            .OrderBySelectDescending("total")
            .Limit(20)
            .Build();

        Assert.Equal(
            "select [r].[route] as [route], count(*) as [total]" + Environment.NewLine +
            "from [requests] as [r]" + Environment.NewLine +
            "where [r].[timestamp] >= now - 30 day and [r].[error] = true" + Environment.NewLine +
            "group by [r].[route]" + Environment.NewLine +
            "order by [total] desc" + Environment.NewLine +
            "limit 20",
            QueryLanguage.Write(spec, Schema));
    }

    [Fact]
    public void ReadsBackWhatItWrote()
    {
        RoundTrips(QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .CountRows("total")
            .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day))
            .GroupBy("r", "route")
            .Having(f => f.SelectGreaterThan("total", 100))
            .OrderBySelectDescending("total")
            .Distinct()
            .Limit(20)
            .Offset(40)
            .Build());
    }

    [Fact]
    public void ReadsBackJoinsAndAggregates()
    {
        RoundTrips(QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k")
            .LeftJoin("users", "u", "apiKey_owner")
            .SelectAndGroupByDay("r", "timestamp", "day")
            .Count("r", "route", distinct: true, outputAlias: "routes")
            .Percentile("r", "durationMs", 0.95, "p95")
            .Build());
    }

    [Fact]
    public void ReadsBackEveryShapeOfCondition()
    {
        RoundTrips(QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Where(f => f
                .Between("r", "durationMs", 10, 500)
                .In("r", "status", new[] { 200, 404 })
                .IsNotNull("r", "apiKeyId")
                .Contains("r", "route", "/api")
                .AnyOf(g => g.Equal("r", "error", true).Equal("r", "cacheHit", false)))
            .Build());
    }

    [Fact]
    public void ReadsBackCallsAndTableFunctions()
    {
        RoundTrips(QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "name")
            .Build());

        RoundTrips(QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(7, QueryTimeUnit.Day)), "a")
            .Select("a", "name", "name")
            .Build());
    }

    [Fact]
    public void ReadsBackANameCarryingTheCharacterThatDelimitsIt()
    {
        // The schema is free to label a field with a closing bracket; the text has to survive it.
        var schema = new QuerySchema(
        [
            new QueryEntity("audit", "Audit",
            [
                new QueryField("value]with]brackets", "Payload", QueryFieldType.Text),
            ]),
        ]);

        var spec = QueryBuilder.From(schema, "audit", "a").Select("a", "value]with]brackets").Build();
        var written = QueryLanguage.Write(spec, schema);

        Assert.Contains("[value]]with]]brackets]", written, StringComparison.Ordinal);
        Assert.Equal(written, QueryLanguage.Write(QueryLanguage.Parse(written, schema), schema));
    }

    [Fact]
    public void SpendsTheForeignKeySugarIntoJoinsRatherThanKeepingIt()
    {
        // Worth stating plainly: this round trip is stable in meaning, not in characters. The dots
        // become joins, and writing back out shows the joins.
        const string typed = "select [r].[apiKeyId].[name] as [key] from [requests] as [r]";

        var written = QueryLanguage.Write(QueryLanguage.Parse(typed, Schema), Schema);

        Assert.DoesNotContain("[apiKeyId].[name]", written, StringComparison.Ordinal);
        Assert.Contains("left join [apiKeys]", written, StringComparison.Ordinal);
        // Reading the written form again changes nothing further, which is what has to hold.
        Assert.Equal(written, QueryLanguage.Write(QueryLanguage.Parse(written, Schema), Schema));
    }

    [Fact]
    public void MeansTheSameThingToADatabaseAfterARoundTrip()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k")
            .Select("k", "name", "key")
            .CountRows("total")
            .GroupBy("k", "name")
            .Build();

        var read = QueryLanguage.Parse(QueryLanguage.Write(spec, Schema), Schema);

        Assert.Equal(
            SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance).Sql,
            SqlRenderer.Render(read, Schema, SqlServerDialect.Instance).Sql);
    }
}
