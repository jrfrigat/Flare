using Querio;

namespace Flare.Components.Tests;

/// <summary>
/// The condition editor's target dropdown: switching between a field, a value function and a computed
/// aggregate resets what no longer applies, seeds the function's arguments, keeps the operator one the
/// new target allows and reports the change.
/// </summary>
public class QueryConditionEditorTests : FlareTestContext
{
    private static readonly QuerySchema Schema = new(
    [
        new QueryEntity("requests", "Requests",
        [
            new QueryField("route", "Route", QueryFieldType.Text),
            new QueryField("durationMs", "Duration, ms", QueryFieldType.Number),
            new QueryField("at", "At", QueryFieldType.DateTime),
        ]),
    ],
    functions:
    [
        new QueryFunction("len", "Length", QueryFunctionKind.Value)
        {
            Parameters = [new QueryFunctionParameter("text", "Text", QueryFieldType.Text)],
            ReturnType = QueryFieldType.Number,
        },
        new QueryFunction("concat", "Concat", QueryFunctionKind.Value)
        {
            Parameters =
            [
                new QueryFunctionParameter("left", "Left", QueryFieldType.Text),
                new QueryFunctionParameter("right", "Right", QueryFieldType.Text),
            ],
        },
    ]);

    private static readonly QueryChoices Choices = QueryChoices.For(
        QueryBuilder.From(Schema, "requests", "r").Select("r", "route").CountRows("total").Build(), Schema);

    private int _changes;

    private (IRenderedComponent<QueryConditionEditor> Cut, QueryConditionDraft Row) Render(
        bool aggregates = true, QueryOperator op = QueryOperator.Contains)
    {
        var group = new QueryConditionGroupDraft();
        var row = new QueryConditionDraft { Alias = "r", Field = "route", Operator = op, Value = "x" };
        group.Conditions.Add(row);
        var cut = Render<QueryConditionEditor>(p => p
            .Add(x => x.Group, group)
            .Add(x => x.Schema, Schema)
            .Add(x => x.Choices, Choices)
            .Add(x => x.AllowAggregates, aggregates)
            .Add(x => x.OnChanged, () => _changes++));
        return (cut, row);
    }

    private static FlareSelect<string> Target(IRenderedComponent<QueryConditionEditor> cut)
        => cut.FindComponents<FlareSelect<string>>()[0].Instance;

    private static void Choose(IRenderedComponent<QueryConditionEditor> cut, string? token)
        => cut.InvokeAsync(() => Target(cut).ValueChanged.InvokeAsync(token)).GetAwaiter().GetResult();

    private static IReadOnlyList<QueryOperator> Allowed(QueryFieldType type)
        => Choices.OperatorsFor(type).Select(choice => choice.Operator).ToList();

    [Fact]
    public void Targets_OfferFilterableFieldsAndFunctions_AndAggregatesOnlyWhenAllowed()
    {
        var withAggregates = Target(Render().Cut).Items!.ToList();
        Assert.Contains("f:r.route", withAggregates);
        Assert.Contains("fn:len", withAggregates);
        Assert.Contains("s:total", withAggregates);

        var rowsOnly = Target(Render(aggregates: false).Cut).Items!.ToList();
        Assert.DoesNotContain(rowsOnly, token => token.StartsWith("s:", StringComparison.Ordinal));
    }

    [Fact]
    public void SwitchingToAFunction_ClearsTheField_SeedsItsArguments_AndKeepsAnAllowedOperator()
    {
        var (cut, row) = Render();
        Assert.DoesNotContain(QueryOperator.Contains, Allowed(QueryFieldType.Number));

        Choose(cut, "fn:len");

        Assert.Equal((string.Empty, string.Empty), (row.Alias, row.Field));
        Assert.Equal("len", row.Function);
        Assert.Single(row.Arguments);
        Assert.Equal(Allowed(QueryFieldType.Number)[0], row.Operator);
        Assert.True(_changes > 0);

        Choose(cut, "fn:concat");
        Assert.Equal(2, row.Arguments.Count);
    }

    [Fact]
    public void SwitchingToAnAggregate_ClearsTheFunctionAndItsArguments()
    {
        var (cut, row) = Render();
        Choose(cut, "fn:concat");

        Choose(cut, "s:total");

        Assert.Equal("total", row.SelectAlias);
        Assert.Null(row.Function);
        Assert.Empty(row.Arguments);
        Assert.Contains(row.Operator, Allowed(QueryFieldType.Number));
    }

    [Fact]
    public void SwitchingToAField_ReadsItsAliasAndName_AndKeepsAStillAllowedOperator()
    {
        var (cut, row) = Render(op: QueryOperator.Equals);
        Choose(cut, "s:total");

        Choose(cut, "f:r.durationMs");

        Assert.Equal(("r", "durationMs"), (row.Alias, row.Field));
        Assert.Null(row.SelectAlias);
        Assert.Equal(QueryOperator.Equals, row.Operator);
    }

    [Fact]
    public void ClearingTheTarget_LeavesAnIncompleteRow()
    {
        var (cut, row) = Render();

        Choose(cut, null);

        Assert.Equal((string.Empty, string.Empty), (row.Alias, row.Field));
        Assert.Null(row.ToCondition());
    }

    [Fact]
    public void Buttons_ToggleTheConnector_AddAndRemoveRowsAndNestedNodes()
    {
        var group = new QueryConditionGroupDraft();
        var cut = Render<QueryConditionEditor>(p => p
            .Add(x => x.Group, group)
            .Add(x => x.Schema, Schema)
            .Add(x => x.Choices, Choices)
            .Add(x => x.OnChanged, () => _changes++));

        cut.Find($".{Flare.Css.Classes.QueryBuilder.Connector}").Click();
        Assert.True(group.Or);

        cut.FindAll("button").First(b => b.TextContent.Trim() == "Condition").Click();
        var added = Assert.Single(group.Conditions);
        Assert.Equal("f:r.route", $"f:{added.Alias}.{added.Field}");

        cut.FindAll("button").First(b => b.TextContent.Trim() == "Group").Click();
        Assert.Single(group.Groups);

        // The nested node's own head carries a remove button; the root's does not.
        var removes = cut.FindAll("button[aria-label='Remove']");
        removes[^1].Click();
        Assert.Empty(group.Groups);

        cut.FindAll("button[aria-label='Remove']")[0].Click();
        Assert.Empty(group.Conditions);
        Assert.True(_changes >= 5);
    }

    [Fact]
    public void ATimestampTarget_OffersTheRelativeToggle()
    {
        var (cut, _) = Render(op: QueryOperator.GreaterThan);
        Assert.DoesNotContain("Relative", cut.Markup);

        Choose(cut, "f:r.at");

        Assert.Contains("Relative", cut.Markup);
    }
}
