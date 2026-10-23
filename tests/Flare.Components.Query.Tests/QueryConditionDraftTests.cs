using Querio;

namespace Flare.Components.Tests;

/// <summary>
/// The designer's editable condition rows and nodes: what a half-filled row yields, and that a
/// condition opened in the designer comes back out unchanged.
/// </summary>
public class QueryConditionDraftTests
{
    private static QueryCondition RoundTrip(QueryCondition condition)
        => QueryConditionDraft.FromCondition(condition).ToCondition()!;

    [Fact]
    public void FieldCondition_RoundTrips()
    {
        var condition = new QueryCondition(new QueryFieldRef("r", "route"), QueryOperator.StartsWith)
        {
            Value = QueryOperand.Literal("/api"),
        };

        var back = RoundTrip(condition);

        Assert.Equal(condition.Field, back.Field);
        Assert.Equal(QueryOperator.StartsWith, back.Operator);
        Assert.Equal("/api", back.Value!.Value);
        Assert.Null(back.Select);
        Assert.Null(back.Call);
    }

    [Fact]
    public void AggregateCondition_TestsTheOutputNameAndNoField()
    {
        var draft = new QueryConditionDraft
        {
            Alias = "r", Field = "route", SelectAlias = "total",
            Operator = QueryOperator.GreaterThan, Value = "10",
        };

        var condition = draft.ToCondition()!;

        Assert.Null(condition.Field);
        Assert.Equal("total", condition.Select);
        Assert.Equal("total", RoundTrip(condition).Select);
    }

    [Fact]
    public void FunctionCondition_KeepsItsFieldAndValueArguments()
    {
        var condition = new QueryCondition(null, QueryOperator.Equals)
        {
            Call = new QueryFunctionCall("left")
            {
                Arguments = [QueryOperand.Of(new QueryFieldRef("r", "route")), QueryOperand.Literal("4")],
            },
            Value = QueryOperand.Literal("/api"),
        };

        var draft = QueryConditionDraft.FromCondition(condition);
        Assert.Equal("left", draft.Function);
        Assert.True(draft.Arguments[0].IsField);
        Assert.False(draft.Arguments[1].IsField);

        var back = draft.ToCondition()!;
        Assert.Null(back.Field);
        Assert.Equal("left", back.Call!.Function);
        Assert.Equal(new QueryFieldRef("r", "route"), back.Call.Arguments[0].Field);
        Assert.Equal("4", back.Call.Arguments[1].Value);
    }

    [Fact]
    public void NoValueOperator_IsCompleteWithoutAValue()
    {
        var draft = new QueryConditionDraft { Alias = "r", Field = "route", Operator = QueryOperator.IsNull };

        var condition = draft.ToCondition();

        Assert.NotNull(condition);
        Assert.Null(condition!.Value);
        Assert.Equal(QueryOperator.IsNull, RoundTrip(condition).Operator);
    }

    [Fact]
    public void ListOperator_TrimsAndDropsEmptyItems_AndAnEmptyListIsIncomplete()
    {
        var draft = new QueryConditionDraft
        {
            Alias = "r", Field = "route", Operator = QueryOperator.In, Values = " a, b ,, c ,",
        };

        var condition = draft.ToCondition()!;
        Assert.Equal(["a", "b", "c"], condition.Value!.Values!);
        Assert.Equal("a, b, c", QueryConditionDraft.FromCondition(condition).Values);

        draft.Values = " , ";
        Assert.Null(draft.ToCondition());
    }

    [Fact]
    public void RangeOperator_NeedsBothBounds()
    {
        var draft = new QueryConditionDraft
        {
            Alias = "r", Field = "durationMs", Operator = QueryOperator.Between, Value = "10",
        };
        Assert.Null(draft.ToCondition());

        draft.Value2 = "20";
        var condition = draft.ToCondition()!;
        Assert.Equal("10", condition.Value!.Value);
        Assert.Equal("20", condition.Value2!.Value);

        var back = QueryConditionDraft.FromCondition(condition);
        Assert.Equal(("10", "20"), (back.Value, back.Value2));
    }

    [Theory]
    [InlineData(true, 30, QueryTimeUnit.Day)]
    [InlineData(false, 2, QueryTimeUnit.Week)]
    public void RelativeOperand_RoundTripsItsDirectionAmountAndUnit(bool past, int amount, QueryTimeUnit unit)
    {
        var draft = new QueryConditionDraft
        {
            Alias = "r", Field = "at", Operator = QueryOperator.GreaterThan,
            Relative = true, RelativePast = past, RelativeAmount = amount, RelativeUnit = unit,
        };

        var condition = draft.ToCondition()!;
        Assert.Equal(QueryOperandKind.Relative, condition.Value!.Kind);
        Assert.Equal(past ? -amount : amount, condition.Value.Relative!.Amount);

        var back = QueryConditionDraft.FromCondition(condition);
        Assert.True(back.Relative);
        Assert.Equal((past, amount, unit), (back.RelativePast, back.RelativeAmount, back.RelativeUnit));
    }

    [Fact]
    public void RowWithoutATarget_OrWithoutAValue_IsIncomplete()
    {
        Assert.Null(new QueryConditionDraft { Value = "x" }.ToCondition());
        Assert.Null(new QueryConditionDraft { Alias = "r", Value = "x" }.ToCondition());
        Assert.Null(new QueryConditionDraft { Alias = "r", Field = "route", Value = " " }.ToCondition());
    }

    [Fact]
    public void Group_LeavesOutIncompleteRowsAndEmptyNodes_AndKeepsItsConnectors()
    {
        var root = new QueryConditionGroupDraft { Or = true };
        root.Conditions.Add(new QueryConditionDraft { Alias = "r", Field = "route", Value = "/a" });
        root.Conditions.Add(new QueryConditionDraft { Alias = "r", Field = "route" });
        var nested = new QueryConditionGroupDraft();
        nested.Conditions.Add(new QueryConditionDraft { Alias = "r", Field = "durationMs", Value = "5" });
        root.Groups.Add(nested);
        var empty = new QueryConditionGroupDraft();
        empty.Conditions.Add(new QueryConditionDraft());
        root.Groups.Add(empty);

        var group = root.ToGroup();

        Assert.True(group.Or);
        Assert.Single(group.Conditions);
        Assert.Single(group.Groups);
        Assert.False(group.Groups[0].Or);

        var reopened = QueryConditionGroupDraft.FromGroup(group);
        Assert.True(reopened.Or);
        Assert.Single(reopened.Conditions);
        Assert.Equal("5", reopened.Groups[0].Conditions[0].Value);
    }

    [Fact]
    public void FromGroup_Null_IsAnEmptyNode()
    {
        var draft = QueryConditionGroupDraft.FromGroup(null);

        Assert.False(draft.Or);
        Assert.Empty(draft.Conditions);
        Assert.Empty(draft.Groups);
        Assert.True(draft.ToGroup().IsEmpty);
    }
}
