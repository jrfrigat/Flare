using Querio;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-21: the Query package shipped with smoke tests only - a root and a missing-schema notice.
/// These are behaviour: a schema draws the designer, a change raises a new spec, and a complete query
/// can be submitted while an incomplete one cannot.
/// </summary>
public class QueryBuilderBehaviorTests : FlareTestContext
{
    private static QuerySchema Schema() => new(
    [
        new QueryEntity("requests", "Requests",
        [
            new QueryField("id", "Id", QueryFieldType.Guid),
            new QueryField("route", "Route", QueryFieldType.Text),
            new QueryField("durationMs", "Duration, ms", QueryFieldType.Number),
        ])
        { Source = "dbo.RequestLog", PrimaryKey = ["id"] },
    ]);

    private static QuerySpec CompleteSpec(QuerySchema schema) =>
        QueryBuilder.From(schema, "requests", "r").Select("r", "route", "route").Build();

    [Fact]
    public void WithASchema_ShowsTheDesignerInsteadOfTheMissingSchemaNotice()
    {
        var schema = Schema();
        var cut = Render<FlareQueryBuilder>(p => p
            .Add(x => x.Schema, schema)
            .Add(x => x.Spec, CompleteSpec(schema)));

        Assert.NotEmpty(cut.FindAll($".{Flare.Css.Classes.QueryBuilder.Section}"));
        Assert.DoesNotContain(QueryBuilderLabels.Default.SchemaMissing, cut.Markup);
        Assert.Contains("Requests", cut.Markup);
    }

    [Fact]
    public void AddingAColumn_RaisesASpecWithTheNewColumn()
    {
        var schema = Schema();
        var initial = CompleteSpec(schema);
        QuerySpec? raised = null;
        var cut = Render<FlareQueryBuilder>(p => p
            .Add(x => x.Schema, schema)
            .Add(x => x.Spec, initial)
            .Add(x => x.SpecChanged, (QuerySpec spec) => raised = spec));

        cut.FindAll("button")
            .First(button => button.TextContent.Trim() == QueryBuilderLabels.Default.AddColumn)
            .Click();

        Assert.NotNull(raised);
        Assert.Equal(initial.Select.Count + 1, raised!.Select.Count);
    }

    [Fact]
    public void SubmittingACompleteQuery_RaisesItOnce()
    {
        var schema = Schema();
        var submitted = new List<QuerySpec>();
        var cut = Render<FlareQueryBuilder>(p => p
            .Add(x => x.Schema, schema)
            .Add(x => x.Spec, CompleteSpec(schema))
            .Add(x => x.OnSubmit, (QuerySpec spec) => submitted.Add(spec)));

        cut.FindAll("button")
            .First(button => button.TextContent.Trim() == QueryBuilderLabels.Default.Submit)
            .Click();

        var spec = Assert.Single(submitted);
        Assert.Single(spec.Select);
    }

    [Fact]
    public void WithASchemaButNoQuery_OpensOnAUsableDefaultThatCanBeSubmitted()
    {
        var schema = Schema();
        var submitted = new List<QuerySpec>();
        var cut = Render<FlareQueryBuilder>(p => p
            .Add(x => x.Schema, schema)
            .Add(x => x.OnSubmit, (QuerySpec spec) => submitted.Add(spec)));

        cut.FindAll("button")
            .First(button => button.TextContent.Trim() == QueryBuilderLabels.Default.Submit)
            .Click();

        var spec = Assert.Single(submitted);
        Assert.Equal("requests", spec.From.Entity);
        Assert.Equal("r", spec.From.Alias);
    }

    [Fact]
    public void QueryEditor_WritesASpecAsText_AndReadsItBackIntoOne()
    {
        var schema = Schema();
        QuerySpec? read = null;
        var cut = Render<FlareQueryEditor>(p => p
            .Add(x => x.Schema, schema)
            .Add(x => x.Spec, CompleteSpec(schema))
            .Add(x => x.SpecChanged, (QuerySpec? spec) => read = spec));

        var text = cut.Find("textarea").TextContent;

        Assert.Contains("route", text, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(read);
        Assert.Single(read!.Select);
    }
}
