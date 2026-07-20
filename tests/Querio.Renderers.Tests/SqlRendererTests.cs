using Querio.Sql;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers the SQL renderer and the three bundled dialects. The point of most of these is that the
/// same query renders differently per engine while meaning the same thing - and that an engine which
/// cannot mean the same thing says so instead of guessing.
/// </summary>
public sealed class SqlRendererTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    private static QuerySpec Aggregate() => QueryBuilder.From(Schema, "requests", "r")
        .Select("r", "route")
        .CountRows("total")
        .Where(f => f.Equal("r", "error", true))
        .GroupBy("r", "route")
        .OrderBySelectDescending("total")
        .Limit(10)
        .Build();

    [Fact]
    public void RendersAnAggregateQueryForPostgreSql()
    {
        var result = SqlRenderer.Render(Aggregate(), Schema, PostgreSqlDialect.Instance);

        Assert.Equal(
            "SELECT \"r\".\"route\", COUNT(*) AS \"total\" " +
            "FROM \"dbo\".\"RequestLog\" AS \"r\" " +
            "WHERE \"r\".\"error\" = @p0 " +
            "GROUP BY \"r\".\"route\" " +
            "ORDER BY \"total\" DESC " +
            "LIMIT 10",
            result.Sql);
    }

    [Fact]
    public void CapsRowsWithTopOnSqlServer()
    {
        var result = SqlRenderer.Render(Aggregate(), Schema, SqlServerDialect.Instance);

        Assert.Equal(
            "SELECT TOP (10) [r].[route], COUNT(*) AS [total] " +
            "FROM [dbo].[RequestLog] AS [r] " +
            "WHERE [r].[error] = @p0 " +
            "GROUP BY [r].[route] " +
            "ORDER BY [total] DESC",
            result.Sql);
    }

    [Fact]
    public void CarriesValuesAsTypedParametersRatherThanText()
    {
        var result = SqlRenderer.Render(Aggregate(), Schema, PostgreSqlDialect.Instance);

        var parameter = Assert.Single(result.Parameters);
        Assert.Equal("p0", parameter.Name);
        Assert.Equal(true, parameter.Value);
        Assert.DoesNotContain("true", result.Sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReadsEachValueAccordingToItsDeclaredType()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f
                .GreaterThan("r", "durationMs", 250)
                .Equal("r", "id", Guid.Empty)
                .Between("r", "timestamp", new DateTime(2026, 1, 1), new DateTime(2026, 2, 1)))
            .Build();

        var values = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance)
            .Parameters.Select(parameter => parameter.Value).ToArray();

        Assert.Equal(250L, values[0]);
        Assert.Equal(Guid.Empty, values[1]);
        Assert.Equal(new DateTime(2026, 1, 1), values[2]);
        Assert.Equal(new DateTime(2026, 2, 1), values[3]);
    }

    [Fact]
    public void UsesTheCaseInsensitiveMatchOnPostgreSql()
    {
        // Querio defines text matching as case-insensitive, and PostgreSQL's LIKE is not.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f.Contains("r", "route", "/api"))
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        Assert.Contains("ILIKE @p0 ESCAPE '\\'", result.Sql, StringComparison.Ordinal);
        Assert.Equal("%/api%", result.Parameters[0].Value);
    }

    [Fact]
    public void EscapesWildcardsSoASearchForAPercentFindsAPercent()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f.Contains("r", "route", "100%_done"))
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        Assert.Equal("%100\\%\\_done%", result.Parameters[0].Value);
    }

    [Fact]
    public void EscapesTheBracketAsWellOnSqlServer()
    {
        // T-SQL reads a bracket as a character class even with an ESCAPE clause in play.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f.Contains("r", "route", "a[b"))
            .Build();

        var result = SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance);

        Assert.Equal("%a\\[b%", result.Parameters[0].Value);
    }

    [Theory]
    [InlineData("PostgreSQL", "(now() + INTERVAL '1 day' * (-30))")]
    [InlineData("SQL Server", "DATEADD(day, -30, SYSUTCDATETIME())")]
    [InlineData("SQLite", "datetime('now', '-30 days')")]
    public void LetsTheEngineResolveARelativeWindow(string engine, string expected)
    {
        // Resolving the window in SQL rather than at render time is what keeps a saved query meaning
        // "the last 30 days" every time it runs.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day))
            .Build();

        var result = SqlRenderer.Render(spec, Schema, Dialect(engine));

        Assert.Contains(expected, result.Sql, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("PostgreSQL", "date_trunc('day', \"r\".\"timestamp\")")]
    [InlineData("SQL Server", "DATEADD(day, DATEDIFF(day, 0, [r].[timestamp]), 0)")]
    [InlineData("SQLite", "date(\"r\".\"timestamp\")")]
    public void TruncatesTimestampsTheWayEachEngineSpellsIt(string engine, string expected)
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .SelectAndGroupByDay("r", "timestamp", "day")
            .CountRows("total")
            .Build();

        var result = SqlRenderer.Render(spec, Schema, Dialect(engine));

        Assert.Contains(expected, result.Sql, StringComparison.Ordinal);
    }

    [Fact]
    public void ComputesAPercentilePerGroupOnPostgreSql()
    {
        var result = SqlRenderer.Render(Percentile(), Schema, PostgreSqlDialect.Instance);

        Assert.Contains(
            "percentile_cont(0.95) WITHIN GROUP (ORDER BY \"r\".\"durationMs\")",
            result.Sql,
            StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesAGroupedPercentileOnSqlServerRatherThanApproximatingIt()
    {
        // SQL Server's PERCENTILE_CONT is a window function, so it cannot combine with GROUP BY.
        var error = Assert.Throws<QueryRenderException>(
            () => SqlRenderer.Render(Percentile(), Schema, SqlServerDialect.Instance));

        Assert.Equal(QueryFeature.Percentile, error.Feature);
    }

    [Fact]
    public void RefusesAPercentileOnSqliteWhichHasNone()
    {
        var error = Assert.Throws<QueryRenderException>(
            () => SqlRenderer.Render(Percentile(), Schema, SqliteDialect.Instance));

        Assert.Equal(QueryFeature.Percentile, error.Feature);
        Assert.False(SqliteDialect.Instance.Supports(QueryFeature.Percentile));
    }

    [Fact]
    public void RefusesAQuarterOnSqliteRatherThanEmittingAFragileExpression()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .SelectPeriod("r", "timestamp", QueryDateTruncation.Quarter, "q")
            .GroupByPeriod("r", "timestamp", QueryDateTruncation.Quarter)
            .Build();

        Assert.Throws<QueryRenderException>(
            () => SqlRenderer.Render(spec, Schema, SqliteDialect.Instance));
    }

    [Fact]
    public void PagesWithOffsetAndFetchOnSqlServer()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .OrderBy("r", "route")
            .Limit(20)
            .Offset(40)
            .Build();

        var result = SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance);

        Assert.EndsWith("ORDER BY [r].[route] ASC OFFSET 40 ROWS FETCH NEXT 20 ROWS ONLY", result.Sql, StringComparison.Ordinal);
        Assert.DoesNotContain("TOP", result.Sql, StringComparison.Ordinal);
    }

    [Fact]
    public void SuppliesAnOrderingWhenSqlServerNeedsOneToPage()
    {
        // OFFSET is only legal after an ORDER BY, so an unordered paged query needs a placeholder.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .Offset(40)
            .Build();

        var result = SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance);

        Assert.Contains("ORDER BY (SELECT NULL) OFFSET 40 ROWS", result.Sql, StringComparison.Ordinal);
    }

    [Fact]
    public void JoinsOnACompositeKey()
    {
        var spec = QueryBuilder.From(Schema, "orders", "o")
            .Join("orderLines", "l", relation: "order_lines")
            .Select("o", "number")
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        Assert.Contains(
            "INNER JOIN \"orderLines\" AS \"l\" ON \"o\".\"tenantId\" = \"l\".\"tenantId\" AND \"o\".\"number\" = \"l\".\"orderNumber\"",
            result.Sql,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ReadsASelfJoinAsReachingFromTheExistingRowToTheNewOne()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .LeftJoin("users", "mgr", relation: "user_manager")
            .Select("u", "name")
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        Assert.Contains(
            "LEFT JOIN \"users\" AS \"mgr\" ON \"u\".\"managerId\" = \"mgr\".\"id\"",
            result.Sql,
            StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesToPickASideWhenTheSameEntityAppearsTwice()
    {
        // transfers reaches users twice over, and a chain of managers is genuinely ambiguous.
        var spec = QueryBuilder.From(Schema, "users", "u")
            .Join("users", "mgr", relation: "user_manager")
            .Join("users", "boss", relation: "user_manager")
            .Select("u", "name")
            .Build();

        var error = Assert.Throws<QueryRenderException>(
            () => SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance));

        Assert.Contains("ambiguous", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FollowsAnExplicitlyChosenSideWithoutComplaint()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .Join("users", "mgr", relation: "user_manager")
            .Join("users", "boss", relation: "user_manager", from: "mgr")
            .Select("u", "name")
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        Assert.Contains("\"mgr\".\"managerId\" = \"boss\".\"id\"", result.Sql, StringComparison.Ordinal);
    }

    [Fact]
    public void SpellsOutTheAggregateInHavingRatherThanNamingIt()
    {
        // SQL Server refuses an output alias in HAVING, so the expression behind it is emitted.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .CountRows("total")
            .GroupBy("r", "route")
            .Having(h => h.SelectGreaterThan("total", 10))
            .Build();

        var result = SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance);

        Assert.Contains("HAVING COUNT(*) > @p0", result.Sql, StringComparison.Ordinal);
        Assert.Equal(10L, result.Parameters[0].Value);
    }

    [Fact]
    public void ComparesTwoColumnsWithoutInventingAParameter()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .Where(f => f.CompareField("t", "fromUserId", QueryOperator.NotEquals, "t", "toUserId"))
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        Assert.Contains("\"t\".\"fromUserId\" <> \"t\".\"toUserId\"", result.Sql, StringComparison.Ordinal);
        Assert.Empty(result.Parameters);
    }

    [Fact]
    public void ClosesTheIdentifierQuotingHole()
    {
        // Aliases can come from a designer, so a user could type one. Escaping keeps it inert.
        var spec = new QuerySpec(new QuerySource("requests", "a]b"))
        {
            Select = [new QuerySelect { Field = new QueryFieldRef("a]b", "route") }],
        };

        var result = SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance);

        Assert.Contains("[a]]b]", result.Sql, StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesAnIncoherentQueryBeforeWritingAnySql()
    {
        var spec = new QuerySpec(new QuerySource("nope", "n"));

        Assert.Throws<QueryValidationException>(
            () => SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance));
    }

    private static QuerySpec Percentile() => QueryBuilder.From(Schema, "requests", "r")
        .Select("r", "route")
        .Percentile("r", "durationMs", 0.95, "p95")
        .GroupBy("r", "route")
        .Build();

    private static SqlDialect Dialect(string engine) => engine switch
    {
        "PostgreSQL" => PostgreSqlDialect.Instance,
        "SQL Server" => SqlServerDialect.Instance,
        _ => SqliteDialect.Instance,
    };
}
