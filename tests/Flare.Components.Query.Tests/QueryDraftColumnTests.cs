using Querio;

namespace Flare.Components.Tests;

/// <summary>
/// A query opened in the designer and written back must mean the same query: each column keeps its
/// own grouping and ordering, also when the columns are function calls.
/// </summary>
public class QueryDraftColumnTests
{
    private static readonly QuerySchema Schema = new(
    [
        new QueryEntity("requests", "Requests",
        [
            new QueryField("route", "Route", QueryFieldType.Text),
            new QueryField("method", "Method", QueryFieldType.Text),
        ]),
    ]);

    private static QueryFunctionCall Lower(string field)
        => new("lower") { Arguments = [QueryOperand.Of(new QueryFieldRef("r", field))] };

    [Fact]
    public void FieldColumns_KeepTheirGroupingAndOrdering()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route")
            .Select("r", "method")
            .Build() with
        {
            GroupBy = [new QueryGroupBy(new QueryFieldRef("r", "route"))],
            OrderBy = [new QuerySort { Field = new QueryFieldRef("r", "method"), Direction = QuerySortDirection.Descending }],
        };

        var draft = QueryDraft.FromSpec(spec, Schema);

        Assert.Equal((true, (QuerySortDirection?)null), (draft.Columns[0].Group, draft.Columns[0].Sort));
        Assert.Equal((false, (QuerySortDirection?)QuerySortDirection.Descending), (draft.Columns[1].Group, draft.Columns[1].Sort));
        var back = draft.ToSpec();
        Assert.Single(back.GroupBy);
        Assert.Single(back.OrderBy);
    }

    [Fact]
    public void CallColumns_OfTheSameFunction_KeepTheirOwnGroupingAndOrdering()
    {
        var spec = new QuerySpec(new QuerySource("requests", "r"))
        {
            Select = [new QuerySelect { Call = Lower("route") }, new QuerySelect { Call = Lower("method") }],
            GroupBy = [new QueryGroupBy(null) { Call = Lower("route") }],
            OrderBy = [new QuerySort { Call = Lower("method"), Direction = QuerySortDirection.Descending }],
        };

        var draft = QueryDraft.FromSpec(spec, Schema);

        Assert.True(draft.Columns[0].Group);
        Assert.False(draft.Columns[1].Group);
        Assert.Null(draft.Columns[0].Sort);
        Assert.Equal(QuerySortDirection.Descending, draft.Columns[1].Sort);

        var back = draft.ToSpec();
        var group = Assert.Single(back.GroupBy);
        Assert.Equal("route", group.Call!.Arguments[0].Field!.Field);
        var sort = Assert.Single(back.OrderBy);
        Assert.Equal("method", sort.Call!.Arguments[0].Field!.Field);
    }

    [Fact]
    public void IncompleteColumns_AreLeftOut_ButARowCountIsKept()
    {
        var draft = QueryDraft.FromSpec(null, Schema);
        draft.Columns.Add(new QueryColumnDraft { Alias = "r" });
        draft.Columns.Add(new QueryColumnDraft { Aggregate = QueryAggregate.Count, OutputAlias = "total" });

        var spec = draft.ToSpec();

        var select = Assert.Single(spec.Select);
        Assert.Equal(QueryAggregate.Count, select.Aggregate);
        Assert.Equal("requests", spec.From.Entity);
        Assert.Equal("r", spec.From.Alias);
    }
}
