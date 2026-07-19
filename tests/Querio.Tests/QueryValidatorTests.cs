namespace Querio.Tests;

/// <summary>
/// Covers the dialect-free checks. Each test builds a query that is wrong in exactly one way and
/// asserts the validator names that specific problem, so a designer can point at the offending row.
/// </summary>
public sealed class QueryValidatorTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    private static void AssertReports(QuerySpec spec, QueryErrorCode expected)
    {
        var errors = spec.Validate(Schema).Errors;
        Assert.True(
            errors.Any(error => error.Code == expected),
            $"Expected {expected}. Got: " +
            (errors.Count == 0
                ? "no errors."
                : string.Join("; ", errors.Select(error => $"{error.Code} at {error.Path}"))));
    }

    private static QueryFilterGroup Where(params QueryCondition[] conditions)
        => new() { Conditions = conditions };

    [Fact]
    public void AcceptsAWellFormedQuery()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .CountRows("total")
            .GroupBy("r", "route")
            .Build();

        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void RejectsAnEntityTheSchemaDoesNotDeclare()
        => AssertReports(new QuerySpec(new QuerySource("nope", "n")), QueryErrorCode.UnknownEntity);

    [Fact]
    public void RejectsAFieldTheEntityDoesNotDeclare()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select = [new QuerySelect { Field = new QueryFieldRef("r", "nope") }],
        };
        AssertReports(spec, QueryErrorCode.UnknownField);
    }

    [Fact]
    public void RejectsAnAliasNoParticipantOwns()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("zz", "route"), QueryOperator.Equals)
            {
                Value = QueryOperand.Literal("x"),
            }),
        };
        AssertReports(spec, QueryErrorCode.UnknownAlias);
    }

    [Fact]
    public void RejectsTwoParticipantsClaimingOneAlias()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Joins = [new QueryJoin("apiKeys", "r") { Relation = "request_apiKey" }],
        };
        AssertReports(spec, QueryErrorCode.DuplicateAlias);
    }

    [Fact]
    public void RejectsAJoinThatSaysNothingAboutHowToMatch()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Joins = [new QueryJoin("apiKeys", "a")],
        };
        AssertReports(spec, QueryErrorCode.MissingJoinCondition);
    }

    [Fact]
    public void RejectsARelationTheSchemaDoesNotDeclare()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Joins = [new QueryJoin("apiKeys", "a") { Relation = "nope" }],
        };
        AssertReports(spec, QueryErrorCode.UnknownRelation);
    }

    [Fact]
    public void RejectsARelationThatReachesNothingTheQueryAlreadyHas()
    {
        // user_manager connects users to users, but this query has no users yet - so the join would
        // attach the new participant to itself and quietly return nothing useful.
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Joins = [new QueryJoin("users", "u") { Relation = "user_manager" }],
        };
        AssertReports(spec, QueryErrorCode.RelationNotConnected);
    }

    [Fact]
    public void AcceptsASelfJoinWhenTheEntityIsAlreadyPresent()
    {
        var spec = new QuerySpec(new QuerySource("users", "u"))
        {
            Joins = [new QueryJoin("users", "mgr") { Relation = "user_manager", Kind = QueryJoinKind.Left }],
        };
        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void AcceptsAJoinOverACompositeKey()
    {
        var spec = new QuerySpec(new QuerySource("orders", "o"))
        {
            Joins = [new QueryJoin("orderLines", "l") { Relation = "order_lines" }],
        };
        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void RejectsAnAggregateTheFieldDoesNotSupport()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select = [new QuerySelect { Field = new QueryFieldRef("r", "route"), Aggregate = QueryAggregate.Sum }],
        };
        AssertReports(spec, QueryErrorCode.AggregateNotAllowed);
    }

    [Fact]
    public void RejectsAPlainFieldThatIsNotOneOfTheGroupingKeys()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .CountRows("total")
            .GroupBy("r", "status")
            .Build();

        AssertReports(spec, QueryErrorCode.MissingGroupBy);
    }

    [Fact]
    public void RejectsAPercentileWithoutSayingWhichOne()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select =
            [
                new QuerySelect
                {
                    Field = new QueryFieldRef("r", "durationMs"),
                    Aggregate = QueryAggregate.Percentile,
                },
            ],
        };
        AssertReports(spec, QueryErrorCode.MissingPercentileRank);
    }

    [Fact]
    public void RejectsAPercentileRankOutsideZeroToOne()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Percentile("r", "durationMs", 95, "p95")
            .Build();

        AssertReports(spec, QueryErrorCode.InvalidPercentileRank);
    }

    [Fact]
    public void RejectsAnAggregateWithNoFieldToWorkOn()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select = [new QuerySelect { Aggregate = QueryAggregate.Sum }],
        };
        AssertReports(spec, QueryErrorCode.AggregateWithoutField);
    }

    [Fact]
    public void RejectsASelectedItemThatReturnsNothing()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r")) { Select = [new QuerySelect()] };
        AssertReports(spec, QueryErrorCode.EmptySelectItem);
    }

    [Fact]
    public void RejectsAnOperatorTheFieldTypeDoesNotSupport()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("r", "durationMs"), QueryOperator.Contains)
            {
                Value = QueryOperand.Literal("5"),
            }),
        };
        AssertReports(spec, QueryErrorCode.OperatorNotAllowed);
    }

    [Fact]
    public void RejectsAComparisonWithNothingToCompareAgainst()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("r", "route"), QueryOperator.Equals)),
        };
        AssertReports(spec, QueryErrorCode.MissingOperand);
    }

    [Fact]
    public void RejectsANullCheckThatWasGivenAValue()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("r", "route"), QueryOperator.IsNull)
            {
                Value = QueryOperand.Literal("x"),
            }),
        };
        AssertReports(spec, QueryErrorCode.UnexpectedOperand);
    }

    [Fact]
    public void RejectsARangeMissingItsUpperBound()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("r", "durationMs"), QueryOperator.Between)
            {
                Value = QueryOperand.Literal("1"),
            }),
        };
        AssertReports(spec, QueryErrorCode.MissingOperand);
    }

    [Fact]
    public void RejectsASetOperatorGivenASingleValue()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("r", "route"), QueryOperator.In)
            {
                Value = QueryOperand.Literal("x"),
            }),
        };
        AssertReports(spec, QueryErrorCode.MissingOperand);
    }

    [Fact]
    public void RejectsAnAggregateConditionOutsideHaving()
    {
        // Aggregates do not exist yet where Where is applied; this belongs in Having.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .CountRows("total")
            .Where(f => f.SelectGreaterThan("total", 10))
            .Build();

        AssertReports(spec, QueryErrorCode.SelectConditionOutsideHaving);
    }

    [Fact]
    public void RejectsAHavingConditionOnSomethingNothingSelects()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .CountRows("total")
            .Having(h => h.SelectGreaterThan("nope", 10))
            .Build();

        AssertReports(spec, QueryErrorCode.UnknownSelectAlias);
    }

    [Fact]
    public void RejectsAnOrderingOnSomethingNothingSelects()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .CountRows("total")
            .OrderBySelectDescending("nope")
            .Build();

        AssertReports(spec, QueryErrorCode.UnknownSelectAlias);
    }

    [Fact]
    public void RejectsDateTruncationOnAFieldThatHoldsNoTimestamp()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            GroupBy =
            [
                new QueryGroupBy(new QueryFieldRef("r", "route")) { Truncate = QueryDateTruncation.Day },
            ],
        };
        AssertReports(spec, QueryErrorCode.TruncationNotApplicable);
    }

    [Fact]
    public void RejectsARelativeOffsetComparedAgainstANonTimestamp()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("r", "durationMs"), QueryOperator.GreaterThanOrEqual)
            {
                Value = QueryOperand.Ago(7, QueryTimeUnit.Day),
            }),
        };
        AssertReports(spec, QueryErrorCode.RelativeValueNotApplicable);
    }

    [Fact]
    public void RespectsAFieldTheSchemaMarksAsNotFilterable()
    {
        var spec = new QuerySpec(new QuerySource("users", "u"))
        {
            Where = Where(new QueryCondition(new QueryFieldRef("u", "secret"), QueryOperator.Equals)
            {
                Value = QueryOperand.Literal("x"),
            }),
        };
        AssertReports(spec, QueryErrorCode.FieldNotFilterable);
    }

    [Fact]
    public void RespectsAFieldTheSchemaMarksAsNotGroupable()
    {
        var spec = new QuerySpec(new QuerySource("users", "u"))
        {
            GroupBy = [new QueryGroupBy(new QueryFieldRef("u", "secret"))],
        };
        AssertReports(spec, QueryErrorCode.FieldNotGroupable);
    }

    [Fact]
    public void RejectsANegativeRowLimit()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r")) { Limit = -1 };
        AssertReports(spec, QueryErrorCode.InvalidLimit);
    }

    [Fact]
    public void RejectsAConditionWithNoTargetAtAll()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Where = Where(new QueryCondition(null, QueryOperator.Equals) { Value = QueryOperand.Literal("x") }),
        };
        AssertReports(spec, QueryErrorCode.MissingConditionTarget);
    }

    [Fact]
    public void RejectsAConditionNamingBothAFieldAndAnAggregate()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Having = Where(new QueryCondition(new QueryFieldRef("r", "route"), QueryOperator.Equals)
            {
                Select = "total",
                Value = QueryOperand.Literal("x"),
            }),
        };
        AssertReports(spec, QueryErrorCode.AmbiguousConditionTarget);
    }

    [Fact]
    public void ReportsEveryProblemRatherThanStoppingAtTheFirst()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select = [new QuerySelect { Field = new QueryFieldRef("r", "nope") }],
            Limit = -5,
        };

        var result = spec.Validate(Schema);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == QueryErrorCode.UnknownField);
        Assert.Contains(result.Errors, e => e.Code == QueryErrorCode.InvalidLimit);
    }

    [Fact]
    public void ThrowsOnDemandCarryingEveryProblem()
    {
        var spec = new QuerySpec(new QuerySource("nope", "n"));
        var exception = Assert.Throws<QueryValidationException>(() => spec.Validate(Schema).ThrowIfInvalid());
        Assert.Contains(exception.Errors, e => e.Code == QueryErrorCode.UnknownEntity);
    }

    [Fact]
    public void PointsAtWhereTheProblemIs()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select =
            [
                new QuerySelect { Field = new QueryFieldRef("r", "route") },
                new QuerySelect { Field = new QueryFieldRef("r", "nope") },
            ],
        };

        var error = Assert.Single(spec.Validate(Schema).Errors);
        Assert.Equal("Select[1]", error.Path);
    }
}
