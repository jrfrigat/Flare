using Querio.Language;
using Querio.Sql;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers reading the SQL-shaped language. Most of it is about the one thing that is not SQL:
/// reaching a field through a foreign key, which is what makes the text worth writing by hand.
/// </summary>
public sealed class QueryLanguageTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    private static QuerySpec Parse(string text) => QueryLanguage.Parse(text, Schema);

    private static string ToSqlServer(string text)
        => SqlRenderer.Render(Parse(text), Schema, SqlServerDialect.Instance).Sql;

    [Fact]
    public void ReachesAFieldThroughAForeignKey()
    {
        // The shape the whole language exists for: no SQL can write this.
        var spec = Parse("select [r].[apiKeyId].[name] from [dbo].[RequestLog] as [r]");

        var join = Assert.Single(spec.Joins);
        Assert.Equal("apiKeys", join.Entity);
        Assert.Equal("request_apiKey", join.Relation);
        Assert.Equal("r", join.From);
        // Travelling a key must not drop rows whose key is empty, so the join is outer.
        Assert.Equal(QueryJoinKind.Left, join.Kind);

        var selected = Assert.Single(spec.Select);
        Assert.Equal(join.Alias, selected.Field!.Alias);
        Assert.Equal("name", selected.Field.Field);
    }

    [Fact]
    public void TravelsAsManyKeysAsAreWritten()
    {
        var spec = Parse("select [r].[apiKeyId].[ownerId].[name] from [requests] as [r]");

        Assert.Equal(2, spec.Joins.Count);
        Assert.Equal("apiKeys", spec.Joins[0].Entity);
        Assert.Equal("users", spec.Joins[1].Entity);
        // The second hop hangs off the first, not off the root.
        Assert.Equal(spec.Joins[0].Alias, spec.Joins[1].From);
        Assert.Equal(spec.Joins[1].Alias, spec.Select[0].Field!.Alias);
    }

    [Fact]
    public void TravelsTheSameKeyOnceHoweverOftenItIsWritten()
    {
        var spec = Parse(
            """
            select [r].[apiKeyId].[name], [r].[apiKeyId].[ownerId].[name] as [owner]
            from [requests] as [r]
            where [r].[apiKeyId].[name] <> 'test'
            """);

        // Three mentions of the same key, one join for it, plus the one the second hop needs.
        Assert.Equal(2, spec.Joins.Count);
        Assert.Equal(spec.Joins[0].Alias, spec.Select[0].Field!.Alias);
        Assert.Equal(spec.Joins[0].Alias, spec.Where!.Conditions[0].Field!.Alias);
    }

    [Fact]
    public void TravelsAKeyNamedByItsRelationToo()
    {
        // A composite key has no single field to name, so the relation itself is the way through.
        var spec = Parse("select [o].[order_lines].[sku] from [orders] as [o]");

        var join = Assert.Single(spec.Joins);
        Assert.Equal("orderLines", join.Entity);
        Assert.Equal("order_lines", join.Relation);
    }

    [Fact]
    public void TurnsATravelledKeyIntoAJoinTheDatabaseUnderstands()
    {
        var sql = ToSqlServer("select [r].[route], [r].[apiKeyId].[name] as [key] from [requests] as [r]");

        Assert.Contains("LEFT JOIN [apiKeys] AS [a1] ON [r].[apiKeyId] = [a1].[id]", sql, StringComparison.Ordinal);
        Assert.Contains("[a1].[name] AS [key]", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void TakesTheNameFromTheDatabaseOrTheNameFromTheSchema()
    {
        // Somebody reading a database writes what they can see; the query stores the logical name
        // either way, so the same text still means the same query somewhere else.
        var physical = Parse("select [r].[route] from [dbo].[RequestLog] as [r]");
        var logical = Parse("select [r].[route] from [requests] as [r]");

        Assert.Equal("requests", physical.From.Entity);
        Assert.Equal("requests", logical.From.Entity);
    }

    [Fact]
    public void ReadsAWholeQuery()
    {
        var spec = Parse(
            """
            select distinct [r].[route], count(*) as [total], avg([r].[durationMs]) as [mean]
            from [requests] as [r]
            where [r].[timestamp] >= now - 30 day and ([r].[error] = true or [r].[status] >= 500)
            group by [r].[route]
            having [total] > 100
            order by [total] desc
            limit 20 offset 40
            """);

        Assert.True(spec.Distinct);
        Assert.Equal(3, spec.Select.Count);
        Assert.Equal(QueryAggregate.Count, spec.Select[1].Aggregate);
        Assert.Equal(QueryAggregate.Avg, spec.Select[2].Aggregate);
        Assert.Single(spec.GroupBy);
        Assert.Equal("total", spec.Having!.Conditions[0].Select);
        Assert.Equal("total", spec.OrderBy[0].Select);
        Assert.Equal(QuerySortDirection.Descending, spec.OrderBy[0].Direction);
        Assert.Equal(20, spec.Limit);
        Assert.Equal(40, spec.Offset);

        // The window stays relative, so a saved query still means the last 30 days.
        Assert.Equal(QueryOperandKind.Relative, spec.Where!.Conditions[0].Value!.Kind);
        Assert.Equal(-30, spec.Where.Conditions[0].Value!.Relative!.Amount);
    }

    [Fact]
    public void ReadsAggregatesAndPeriods()
    {
        var spec = Parse(
            """
            select trunc([r].[timestamp], day) as [day],
                   count(distinct [r].[route]) as [routes],
                   percentile([r].[durationMs], 0.95) as [p95]
            from [requests] as [r]
            group by trunc([r].[timestamp], day)
            """);

        Assert.Equal(QueryDateTruncation.Day, spec.Select[0].Truncate);
        Assert.True(spec.Select[1].Distinct);
        Assert.Equal(0.95, spec.Select[2].Percentile);
        Assert.Equal(QueryDateTruncation.Day, spec.GroupBy[0].Truncate);
    }

    [Fact]
    public void ReadsJoinsWrittenOutInFull()
    {
        var byRelation = Parse(
            "select [k].[name] from [requests] as [r] left join [apiKeys] as [k] through [request_apiKey]");
        Assert.Equal(QueryJoinKind.Left, byRelation.Joins[0].Kind);
        Assert.Equal("request_apiKey", byRelation.Joins[0].Relation);

        var byMatch = Parse(
            "select [k].[name] from [requests] as [r] join [apiKeys] as [k] on [r].[apiKeyId] = [k].[id]");
        Assert.Single(byMatch.Joins[0].On!);
        Assert.Equal("apiKeyId", byMatch.Joins[0].On![0].Left.Field);
    }

    [Fact]
    public void ReadsEveryShapeOfCondition()
    {
        var spec = Parse(
            """
            select [r].[route] from [requests] as [r]
            where [r].[status] in (200, 404)
              and [r].[durationMs] between 10 and 500
              and [r].[apiKeyId] is not null
              and [r].[route] contains '/api'
            """);

        var conditions = spec.Where!.Conditions;
        Assert.Equal(QueryOperator.In, conditions[0].Operator);
        Assert.Equal(["200", "404"], conditions[0].Value!.Values!);
        Assert.Equal(QueryOperator.Between, conditions[1].Operator);
        Assert.Equal(QueryOperator.IsNotNull, conditions[2].Operator);
        Assert.Equal(QueryOperator.Contains, conditions[3].Operator);
    }

    [Fact]
    public void ReadsACallToADeclaredFunction()
    {
        var spec = Parse("select upper([u].[name]) as [name] from [users] as [u]");

        Assert.Equal("upper", spec.Select[0].Call!.Function);
        Assert.Contains("UPPER([u].[name])", ToSqlServer("select upper([u].[name]) as [name] from [users] as [u]"),
            StringComparison.Ordinal);
    }

    [Fact]
    public void SaysEverythingThatIsWrongRatherThanOnlyTheFirstThing()
    {
        // An editor underlines all of them at once; stopping at the first would make it useless
        // while a query is half typed.
        var result = QueryLanguage.Read(
            "select [r].[nope], [r].[alsoNope] from [requests] as [r] where [r].[missing] = 1", Schema);

        Assert.Equal(3, result.Diagnostics.Count);
        Assert.All(result.Diagnostics, problem => Assert.True(problem.Length > 0));
        Assert.All(result.Diagnostics, problem => Assert.Contains("no field", problem.Message, StringComparison.Ordinal));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void PointsAtTheExactWordsItCouldNotUnderstand()
    {
        const string text = "select [r].[route] from [nothingLikeThis] as [r]";

        var problem = Assert.Single(QueryLanguage.Read(text, Schema).Diagnostics);

        Assert.Contains("nothingLikeThis", problem.Message, StringComparison.Ordinal);
        Assert.Equal("[nothingLikeThis]", text.Substring(problem.Start, problem.Length));
    }

    [Fact]
    public void RefusesAKeyThatIsNotThere()
    {
        var problem = Assert.Single(
            QueryLanguage.Read("select [r].[route].[name] from [requests] as [r]", Schema).Diagnostics);

        Assert.Contains("not a foreign key", problem.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void KeepsWhatItCouldReadWhenSomethingElseIsBroken()
    {
        // Half a query is still worth having: it is what completion reads to keep making sense.
        var result = QueryLanguage.Read("select [r].[route], [r].[nope] from [requests] as [r]", Schema);

        Assert.NotNull(result.Spec);
        Assert.Equal("requests", result.Spec!.From.Entity);
        Assert.Equal("route", Assert.Single(result.Spec.Select).Field!.Field);
    }
}
