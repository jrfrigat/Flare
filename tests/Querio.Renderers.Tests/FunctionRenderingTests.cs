using Querio.OneC;
using Querio.Sql;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers how a declared function reaches each target. The physical name is the whole trick: one
/// logical function renders as a user routine on one engine and as a built-in on another, and a
/// target that cannot host table functions at all refuses rather than improvising.
/// </summary>
public sealed class FunctionRenderingTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    private static QuerySpec Tax() => QueryBuilder.From(Schema, "transfers", "t")
        .SelectCall(QueryFunctionCall.OfFields("calcTax", "t", "amount"), "tax")
        .Build();

    [Fact]
    public void RendersAValueCallWithTheEngineQuoting()
    {
        Assert.Contains(
            "\"dbo\".\"CalcTax\"(\"t\".\"amount\") AS \"tax\"",
            SqlRenderer.Render(Tax(), Schema, PostgreSqlDialect.Instance).Sql,
            StringComparison.Ordinal);

        Assert.Contains(
            "[dbo].[CalcTax]([t].[amount]) AS [tax]",
            SqlRenderer.Render(Tax(), Schema, SqlServerDialect.Instance).Sql,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ReadsALiteralArgumentAsTheParameterItFills()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(
                QueryFunctionCall.Of(
                    "calcTax",
                    QueryOperand.Of(new QueryFieldRef("t", "amount")),
                    QueryOperand.Literal("0.2")),
                "tax")
            .Build();

        var result = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);

        // Declared as a number, so it reaches the driver as one rather than as text.
        Assert.Equal(0.2m, Assert.Single(result.Parameters).Value);
    }

    [Fact]
    public void NestsCallsWithoutLosingParameterisation()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(
                QueryFunctionCall.Of(
                    "calcTax",
                    QueryOperand.Function(QueryFunctionCall.OfFields("calcTax", "t", "amount"))),
                "twice")
            .Build();

        Assert.Contains(
            "\"dbo\".\"CalcTax\"(\"dbo\".\"CalcTax\"(\"t\".\"amount\"))",
            SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql,
            StringComparison.Ordinal);
    }

    [Fact]
    public void GroupsByACallOnBothSidesSoTheyAgree()
    {
        var upper = QueryFunctionCall.OfFields("upper", "u", "name");
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(upper, "name")
            .CountRows("total")
            .GroupByCall(upper)
            .Build();

        var sql = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql;

        Assert.Contains("SELECT UPPER(\"u\".\"name\") AS \"name\"", sql, StringComparison.Ordinal);
        Assert.Contains("GROUP BY UPPER(\"u\".\"name\")", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void CallsABuiltInByItsBareNameAndARoutineByItsQualifiedOne()
    {
        // Quoting a built-in sends the engine looking for a routine of that name instead: neither
        // [UPPER](x) nor "UPPER"(x) reaches the function actually meant. A qualified name is a
        // routine, and does have to be quoted so a reserved word in it survives.
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(QueryFunctionCall.OfFields("upper", "t", "id"), "id")
            .SelectCall(
                QueryFunctionCall.Of("calcTax", QueryOperand.Of(new QueryFieldRef("t", "amount"))),
                "tax")
            .Build();

        var server = SqlRenderer.Render(spec, Schema, SqlServerDialect.Instance).Sql;
        var postgres = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql;

        Assert.Contains("UPPER([t].[id])", server, StringComparison.Ordinal);
        Assert.DoesNotContain("[UPPER]", server, StringComparison.Ordinal);
        Assert.Contains("[dbo].[CalcTax](", server, StringComparison.Ordinal);

        Assert.Contains("UPPER(\"t\".\"id\")", postgres, StringComparison.Ordinal);
        Assert.DoesNotContain("\"UPPER\"", postgres, StringComparison.Ordinal);
        Assert.Contains("\"dbo\".\"CalcTax\"(", postgres, StringComparison.Ordinal);
    }

    [Fact]
    public void DrawsRowsFromATableFunction()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(30, QueryTimeUnit.Day)), "a")
            .Select("a", "name")
            .Build();

        var sql = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql;

        Assert.Contains("FROM \"dbo\".\"GetActiveUsers\"(", sql, StringComparison.Ordinal);
        Assert.Contains(") AS \"a\"", sql, StringComparison.Ordinal);
        // The window is still resolved by the engine, even inside an argument.
        Assert.Contains("now()", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void JoinsATableFunctionOnExplicitConditions()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .JoinFunction(
                QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(7, QueryTimeUnit.Day)),
                "a",
                [new QueryJoinCondition(new QueryFieldRef("t", "fromUserId"), new QueryFieldRef("a", "id"))])
            .Select("a", "name")
            .Build();

        Assert.Contains(
            "INNER JOIN \"dbo\".\"GetActiveUsers\"",
            SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance).Sql,
            StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesATableFunctionOnSqliteWhichHasNone()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(1, QueryTimeUnit.Day)), "a")
            .Select("a", "name")
            .Build();

        var error = Assert.Throws<QueryRenderException>(
            () => SqlRenderer.Render(spec, Schema, SqliteDialect.Instance));

        Assert.Equal(QueryFeature.TableFunctions, error.Feature);
    }

    [Fact]
    public void RendersAValueCallUnquotedForOneC()
    {
        // 1C cannot quote a name, so the call comes out bare - and the identifier is validated instead.
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "Имя")
            .Build();

        Assert.Contains(
            "UPPER(u.name) КАК Имя",
            OneCRenderer.Render(spec, Schema).Query,
            StringComparison.Ordinal);
    }

    [Fact]
    public void RefusesATableFunctionForOneC()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(1, QueryTimeUnit.Day)), "a")
            .Select("a", "name")
            .Build();

        var error = Assert.Throws<QueryRenderException>(() => OneCRenderer.Render(spec, Schema));

        Assert.Equal(QueryFeature.TableFunctions, error.Feature);
    }

    [Fact]
    public void DeclaresFunctionSupportPerTarget()
    {
        Assert.True(PostgreSqlDialect.Instance.Supports(QueryFeature.TableFunctions));
        Assert.True(SqlServerDialect.Instance.Supports(QueryFeature.TableFunctions));
        Assert.False(SqliteDialect.Instance.Supports(QueryFeature.TableFunctions));

        Assert.True(OneCRenderer.Capabilities.Supports(QueryFeature.ValueFunctions));
        Assert.False(OneCRenderer.Capabilities.Supports(QueryFeature.TableFunctions));
    }
}
