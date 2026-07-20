namespace Querio.Tests;

/// <summary>
/// Covers what a query can be given next. These answers are what any builder needs - a visual
/// designer, a command line, a tool a model calls - which is why they live in the core rather than
/// in whichever one happens to exist.
/// </summary>
public sealed class QueryChoicesTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    [Fact]
    public void OffersEveryEntityAndTableFunctionToStartFrom()
    {
        var roots = QueryChoices.Roots(Schema);

        Assert.Contains(roots, root => root.Key == "requests" && root.Kind == QuerySourceKind.Entity);
        var table = Assert.Single(roots, root => root.Kind == QuerySourceKind.Function);
        Assert.Equal("activeUsers", table.Key);
        // A table function has to be given its arguments, so a caller is told what they are.
        Assert.Equal("since", Assert.Single(table.Parameters).Key);
    }

    [Fact]
    public void OffersNoTableFunctionToATargetThatCannotDrawRowsFromOne()
    {
        var roots = QueryChoices.Roots(Schema, QueryCapabilities.All.Without(QueryFeature.TableFunctions));

        Assert.All(roots, root => Assert.Equal(QuerySourceKind.Entity, root.Kind));
    }

    [Fact]
    public void ReachesTheFieldsOfEverythingTheQueryHasJoined()
    {
        var choices = QueryChoices.For(
            QueryBuilder.From(Schema, "requests", "r").Join("apiKeys", "k").Build(), Schema);

        Assert.Equal(["r", "k"], choices.Participants.Select(participant => participant.Alias));
        Assert.Contains(choices.Fields, member => member.ToString() == "r.route");
        Assert.Contains(choices.Fields, member => member.ToString() == "k.name");
        // The caption comes from the schema, so a picker can qualify a field by where it came from.
        Assert.Equal("API keys", choices.Find(new QueryFieldRef("k", "name"))!.ParticipantLabel);
    }

    [Fact]
    public void LeavesOutFieldsTheSchemaClosedOff()
    {
        var choices = QueryChoices.For(QueryBuilder.From(Schema, "users", "u").Build(), Schema);

        // "secret" is selectable but the schema forbids filtering and grouping on it.
        Assert.Contains(choices.Fields, member => member.Field == "secret");
        Assert.DoesNotContain(choices.Filterable, member => member.Field == "secret");
        Assert.DoesNotContain(choices.Groupable, member => member.Field == "secret");
    }

    [Fact]
    public void OffersEveryRelationReachingWhatIsAlreadyInTheQuery()
    {
        var choices = QueryChoices.For(QueryBuilder.From(Schema, "requests", "r").Build(), Schema);

        var join = Assert.Single(choices.Joins);
        Assert.Equal("request_apiKey", join.Relation);
        Assert.Equal("r", join.FromAlias);
        Assert.Equal("apiKeys", join.Entity);
        Assert.Equal("a", join.SuggestedAlias);
    }

    [Fact]
    public void OffersAnEntityAgainWhenARelationPointsBackAtIt()
    {
        // A user's manager is another user, so joining users onto users is a real choice, not a bug.
        var choices = QueryChoices.For(QueryBuilder.From(Schema, "users", "u").Build(), Schema);

        var self = Assert.Single(choices.Joins, join => join.Relation == "user_manager");
        Assert.Equal("users", self.Entity);
        // The obvious alias is taken, so the next one is offered instead.
        Assert.Equal("u2", self.SuggestedAlias);
    }

    [Fact]
    public void OffersOnlyTheJoinKindsTheTargetCanShape()
    {
        var choices = QueryChoices.For(
            QueryBuilder.From(Schema, "requests", "r").Build(),
            Schema,
            QueryCapabilities.All.Without(QueryFeature.CrossJoin, QueryFeature.FullJoin));

        var kinds = Assert.Single(choices.Joins).Kinds;
        Assert.Contains(QueryJoinKind.Inner, kinds);
        Assert.Contains(QueryJoinKind.Left, kinds);
        Assert.DoesNotContain(QueryJoinKind.Cross, kinds);
        Assert.DoesNotContain(QueryJoinKind.Full, kinds);
    }

    [Fact]
    public void WithholdsOperatorsATargetCannotExpress()
    {
        var route = new QueryFieldRef("r", "route");
        var spec = QueryBuilder.From(Schema, "requests", "r").Build();

        var everything = QueryChoices.For(spec, Schema).OperatorsFor(route);
        var narrowed = QueryChoices
            .For(spec, Schema, QueryCapabilities.All.Without(QueryFeature.TextSearch, QueryFeature.SetOperators))
            .OperatorsFor(route);

        Assert.Contains(everything, choice => choice.Operator == QueryOperator.Contains);
        Assert.DoesNotContain(narrowed, choice => choice.Operator == QueryOperator.Contains);
        Assert.DoesNotContain(narrowed, choice => choice.Operator == QueryOperator.In);
        Assert.Contains(narrowed, choice => choice.Operator == QueryOperator.Equals);
    }

    [Fact]
    public void SaysHowManyValuesEachOperatorNeeds()
    {
        // This is what decides the shape of the editor: no box, one box, a pair, or a set.
        var choices = QueryChoices.For(QueryBuilder.From(Schema, "requests", "r").Build(), Schema);
        var moment = choices.OperatorsFor(new QueryFieldRef("r", "timestamp"))
            .ToDictionary(choice => choice.Operator, choice => choice.Arity);
        var text = choices.OperatorsFor(new QueryFieldRef("r", "route"))
            .ToDictionary(choice => choice.Operator, choice => choice.Arity);

        Assert.Equal(QueryValueArity.None, moment[QueryOperator.IsNull]);
        Assert.Equal(QueryValueArity.One, moment[QueryOperator.GreaterThan]);
        Assert.Equal(QueryValueArity.Two, moment[QueryOperator.Between]);
        Assert.Equal(QueryValueArity.List, text[QueryOperator.In]);

        // A range over a moment is meaningful; a range over a name is not, and is not offered.
        Assert.False(text.ContainsKey(QueryOperator.Between));
    }

    [Fact]
    public void WithholdsAnAggregateATargetCannotCompute()
    {
        var duration = new QueryFieldRef("r", "durationMs");
        var spec = QueryBuilder.From(Schema, "requests", "r").Build();

        Assert.Contains(QueryAggregate.Percentile, QueryChoices.For(spec, Schema).AggregatesFor(duration));

        var narrowed = QueryChoices.For(spec, Schema, QueryCapabilities.All.Without(QueryFeature.Percentile));
        Assert.DoesNotContain(QueryAggregate.Percentile, narrowed.AggregatesFor(duration));
        Assert.Contains(QueryAggregate.Sum, narrowed.AggregatesFor(duration));

        var none = QueryChoices.For(spec, Schema, QueryCapabilities.All.Without(QueryFeature.Aggregates));
        Assert.Empty(none.AggregatesFor(duration));
        Assert.False(none.CountsRows);
    }

    [Fact]
    public void OffersARelativeMomentOnlyWhereAMomentIsBeingCompared()
    {
        var choices = QueryChoices.For(QueryBuilder.From(Schema, "requests", "r").Build(), Schema);

        Assert.Contains(QueryOperandKind.Relative, choices.ValueKindsFor(new QueryFieldRef("r", "timestamp")));
        Assert.DoesNotContain(QueryOperandKind.Relative, choices.ValueKindsFor(new QueryFieldRef("r", "route")));
        Assert.Contains(QueryOperandKind.Literal, choices.ValueKindsFor(new QueryFieldRef("r", "route")));
    }

    [Fact]
    public void OffersOnlyFieldsOfTheSameKindToCompareAgainst()
    {
        var choices = QueryChoices.For(
            QueryBuilder.From(Schema, "transfers", "t").Join("users", "u", "transfer_sender").Build(), Schema);

        var comparable = choices.ComparableTo(new QueryFieldRef("t", "fromUserId"));

        Assert.All(comparable, member => Assert.Equal(QueryFieldType.Guid, member.Type));
        Assert.Contains(comparable, member => member.ToString() == "u.id");
        Assert.DoesNotContain(comparable, member => member.ToString() == "t.fromUserId");
    }

    [Fact]
    public void LetsTheResultBeOrderedByAnythingItSelected()
    {
        var choices = QueryChoices.For(
            QueryBuilder.From(Schema, "requests", "r")
                .Select("r", "route", "route")
                .CountRows("total")
                .GroupBy("r", "route")
                .Build(),
            Schema);

        Assert.Equal(["route", "total"], choices.Outputs);
        Assert.Contains(choices.SortTargets, target => target.Select == "total");
        Assert.Contains(choices.SortTargets, target => target.Field?.Field == "durationMs");
    }

    [Fact]
    public void LetsAGroupingFilterTestOnlyWhatSurvivesTheGrouping()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .CountRows("total")
            .GroupBy("r", "route")
            .Build();

        var targets = QueryChoices.For(spec, Schema).GroupingFilterTargets;

        Assert.Contains(targets, target => target.Select == "total");
        Assert.Contains(targets, target => target.Field?.Field == "route");
        // A field the grouping collapsed is not something a group can be asked about.
        Assert.DoesNotContain(targets, target => target.Field?.Field == "durationMs");

        var without = QueryChoices.For(spec, Schema, QueryCapabilities.All.Without(QueryFeature.Having));
        Assert.Empty(without.GroupingFilterTargets);
    }

    [Fact]
    public void AnswersForAQueryThatIsStillHalfBuilt()
    {
        // A query being built is incomplete by definition, so nothing here may refuse to answer.
        var spec = new QuerySpec(new QuerySource("nothingLikeThis", "x"))
        {
            Joins = [new QueryJoin("alsoMissing", "y")],
        };

        var choices = QueryChoices.For(spec, Schema);

        Assert.Equal(2, choices.Participants.Count);
        Assert.Empty(choices.Fields);
        Assert.Empty(choices.Joins);
        Assert.Null(choices.Find(new QueryFieldRef("x", "route")));
        Assert.Empty(choices.OperatorsFor(new QueryFieldRef("x", "route")));
    }

    [Fact]
    public void SuggestsAnAliasNobodyHasTakenYet()
    {
        var choices = QueryChoices.For(
            QueryBuilder.From(Schema, "users", "u").Join("users", "u2", "user_manager").Build(), Schema);

        Assert.Equal("u3", choices.SuggestAlias("users"));
    }
}
