using System.Globalization;

namespace Querio.Tests;

/// <summary>
/// Covers the fluent builder: that it produces the spec a visual designer would, and that the
/// conveniences (alias generation, relation inference, accumulating conditions) behave predictably.
/// </summary>
public sealed class QueryBuilderTests
{
    [Fact]
    public void BuildsAnAggregateQueryOverOneEntity()
    {
        var schema = TestSchema.Build();

        var spec = QueryBuilder.From(schema, "requests", "r")
            .Select("r", "route")
            .CountRows("total")
            .Percentile("r", "durationMs", 0.95, "p95")
            .Where(f => f
                .Since("r", "timestamp", 30, QueryTimeUnit.Day)
                .Equal("r", "error", true))
            .GroupBy("r", "route")
            .OrderBySelectDescending("total")
            .Limit(100)
            .Build();

        Assert.Equal("requests", spec.From.Entity);
        Assert.Equal("r", spec.From.Alias);

        Assert.Equal(3, spec.Select.Count);
        Assert.Null(spec.Select[0].Aggregate);
        Assert.Equal(QueryAggregate.Count, spec.Select[1].Aggregate);
        Assert.Null(spec.Select[1].Field);
        Assert.Equal(QueryAggregate.Percentile, spec.Select[2].Aggregate);
        Assert.Equal(0.95, spec.Select[2].Percentile);

        Assert.Equal(2, spec.Where!.Conditions.Count);
        Assert.Equal(QueryOperator.GreaterThanOrEqual, spec.Where.Conditions[0].Operator);
        Assert.Equal("true", spec.Where.Conditions[1].Value!.Value);

        Assert.Single(spec.GroupBy);
        Assert.Equal("total", spec.OrderBy[0].Select);
        Assert.Equal(QuerySortDirection.Descending, spec.OrderBy[0].Direction);
        Assert.Equal(100, spec.Limit);

        Assert.True(spec.Validate(schema).IsValid);
    }

    [Fact]
    public void KeepsATimeWindowRelativeRatherThanResolvingIt()
    {
        // A saved query has to still mean "the last 30 days" the next time someone opens it.
        var spec = QueryBuilder.From("requests", "r")
            .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day))
            .Build();

        var operand = spec.Where!.Conditions[0].Value!;
        Assert.Equal(QueryOperandKind.Relative, operand.Kind);
        Assert.Equal(-30, operand.Relative!.Amount);
        Assert.Equal(QueryTimeUnit.Day, operand.Relative.Unit);
    }

    [Fact]
    public void GeneratesDistinctAliasesForRepeatedEntities()
    {
        var builder = QueryBuilder.From(TestSchema.Build(), "requests");
        Assert.Equal("r", builder.RootAlias);

        var spec = builder.Join("requests").Build();
        Assert.Equal("r2", spec.Joins[0].Alias);
    }

    [Fact]
    public void InfersTheRelationWhenExactlyOnePathConnects()
    {
        var spec = QueryBuilder.From(TestSchema.Build(), "requests", "r")
            .Join("apiKeys")
            .Build();

        Assert.Equal("request_apiKey", spec.Joins[0].Relation);
        Assert.Equal("a", spec.Joins[0].Alias);
    }

    [Fact]
    public void RefusesToGuessWhenTwoPathsConnectTheSameEntities()
    {
        // A transfer reaches users as both sender and recipient. Picking one silently would build a
        // query that runs and quietly answers a different question.
        var schema = TestSchema.Build();
        var spec = QueryBuilder.From(schema, "transfers", "t")
            .Join("users")
            .Build();

        Assert.Null(spec.Joins[0].Relation);
        Assert.Contains(spec.Validate(schema).Errors, e => e.Code == QueryErrorCode.MissingJoinCondition);
    }

    [Fact]
    public void JoinsTheSameEntityTwiceThroughDifferentRelations()
    {
        var schema = TestSchema.Build();

        var spec = QueryBuilder.From(schema, "transfers", "t")
            .Join("users", "sender", relation: "transfer_sender")
            .Join("users", "recipient", relation: "transfer_recipient")
            .Select("sender", "name", "fromUser")
            .Select("recipient", "name", "toUser")
            .Sum("t", "amount", "total")
            .GroupBy("sender", "name")
            .GroupBy("recipient", "name")
            .Build();

        Assert.True(spec.Validate(schema).IsValid);
    }

    [Fact]
    public void JoinsAnEntityToItselfThroughASelfRelation()
    {
        var schema = TestSchema.Build();

        var spec = QueryBuilder.From(schema, "users", "u")
            .LeftJoin("users", "mgr", relation: "user_manager")
            .Select("u", "name", "user")
            .Select("mgr", "name", "manager")
            .Build();

        Assert.Equal(QueryJoinKind.Left, spec.Joins[0].Kind);
        Assert.True(spec.Validate(schema).IsValid);
    }

    [Fact]
    public void AccumulatesConditionsAcrossSeveralWhereCalls()
    {
        var spec = QueryBuilder.From("requests", "r")
            .Where(f => f.Equal("r", "error", true))
            .Where(f => f.Equal("r", "cacheHit", false))
            .Build();

        Assert.Equal(2, spec.Where!.Conditions.Count);
        Assert.False(spec.Where.Or);
    }

    [Fact]
    public void OmitsAnEmptyFilterEntirely()
    {
        var spec = QueryBuilder.From("requests", "r").Where(_ => { }).Build();
        Assert.Null(spec.Where);
    }

    [Fact]
    public void NestsAnOrGroupInsideAnAndGroup()
    {
        var spec = QueryBuilder.From("requests", "r")
            .Where(f => f
                .Equal("r", "error", true)
                .AnyOf(any => any.Equal("r", "status", 500).Equal("r", "status", 503)))
            .Build();

        Assert.Single(spec.Where!.Conditions);
        var nested = Assert.Single(spec.Where.Groups);
        Assert.True(nested.Or);
        Assert.Equal(2, nested.Conditions.Count);
    }

    [Fact]
    public void ComparesTwoFieldsAgainstEachOther()
    {
        var spec = QueryBuilder.From("transfers", "t")
            .Where(f => f.CompareField("t", "fromUserId", QueryOperator.NotEquals, "t", "toUserId"))
            .Build();

        var operand = spec.Where!.Conditions[0].Value!;
        Assert.Equal(QueryOperandKind.Field, operand.Kind);
        Assert.Equal("t", operand.Field!.Alias);
        Assert.Equal("toUserId", operand.Field.Field);
    }

    [Fact]
    public void FiltersGroupsOnAComputedAggregate()
    {
        var schema = TestSchema.Build();

        var spec = QueryBuilder.From(schema, "requests", "r")
            .Select("r", "route")
            .CountRows("total")
            .GroupBy("r", "route")
            .Having(h => h.SelectGreaterThan("total", 10))
            .Build();

        Assert.Equal("total", spec.Having!.Conditions[0].Select);
        Assert.True(spec.Validate(schema).IsValid);
    }

    [Theory]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public void FormatsValuesTheSameWhateverTheCurrentCulture(string culture)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Assert.Equal("1.5", QueryValue.ToInvariant(1.5));
            Assert.Equal("true", QueryValue.ToInvariant(true));
            Assert.Equal("Day", QueryValue.ToInvariant(QueryTimeUnit.Day));
            Assert.Equal(
                "2026-07-19T10:20:30.0000000Z",
                QueryValue.ToInvariant(new DateTime(2026, 7, 19, 10, 20, 30, DateTimeKind.Utc)));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
