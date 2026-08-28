using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// Columns as data. A grid must be buildable from a list of DataGridColumn objects with no markup at
/// all - the shape needed to generate columns from metadata - and the list has to behave like markup
/// columns do once it is in: sortable, filterable, hideable, reorderable.
/// </summary>
public class DataGridColumnDefinitionTests : FlareTestContext
{
    private record Row(string Name, int Qty);

    private static readonly Row[] _rows =
    [
        new("Beta", 2),
        new("Alpha", 1),
        new("Gamma", 3),
    ];

    private static List<DataGridColumn<Row>> Definitions() =>
    [
        new() { Title = "Name", Value = r => r.Name, Sortable = true },
        new() { Title = "Qty", Value = r => r.Qty, Sortable = true },
    ];

    private static RenderFragment Grid(
        IEnumerable<DataGridColumn<Row>>? definitions,
        DataGridContext<Row>? ctx = null,
        bool withMarkupColumn = false) => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        if (definitions is not null) b.AddAttribute(2, "ColumnDefinitions", definitions);
        if (ctx is not null) b.AddAttribute(3, "Context", ctx);
        if (withMarkupColumn)
        {
            b.AddAttribute(4, "Columns", (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10);
                inner.AddAttribute(11, "Title", "Name");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => "from-markup"));
                inner.CloseComponent();
            }));
        }
        b.CloseComponent();
    };

    [Fact]
    public void A_grid_can_be_built_from_definitions_alone()
    {
        var cut = Render(Grid(Definitions()));

        Assert.Equal(["Name", "Qty"], cut.FindAll("thead th").Select(th => th.TextContent.Trim()));
        Assert.Equal(3, cut.FindAll("tbody tr").Count);
        Assert.Equal("Beta", cut.FindAll("tbody td")[0].TextContent.Trim());
    }

    [Fact]
    public async Task Defined_columns_sort_like_declared_ones()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render(Grid(Definitions(), ctx));

        await cut.InvokeAsync(() => ctx.SortByAsync("Name"));

        Assert.Equal("Alpha", cut.FindAll("tbody td")[0].TextContent.Trim());
    }

    [Fact]
    public async Task Defined_columns_can_be_hidden_and_reordered_through_the_context()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render(Grid(Definitions(), ctx));

        await cut.InvokeAsync(() => ctx.MoveColumnAsync("Qty", "Name"));
        Assert.Equal(["Qty", "Name"], cut.FindAll("thead th").Select(th => th.TextContent.Trim()));

        await cut.InvokeAsync(() => ctx.SetColumnVisibleAsync("Qty", false));
        Assert.Equal(["Name"], cut.FindAll("thead th").Select(th => th.TextContent.Trim()));
    }

    [Fact]
    public void Markup_columns_win_a_key_collision_and_come_first()
    {
        var cut = Render(Grid(Definitions(), withMarkupColumn: true));

        Assert.Equal(["Name", "Qty"], cut.FindAll("thead th").Select(th => th.TextContent.Trim()));
        // The Name column that survived is the markup one, not the definition with the same key.
        Assert.Equal("from-markup", cut.FindAll("tbody td")[0].TextContent.Trim());
    }

    [Fact]
    public void Replacing_the_definition_list_rebuilds_the_columns()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows)
            .Add(x => x.ColumnDefinitions, Definitions()));

        Assert.Equal(2, cut.FindAll("thead th").Count);

        cut.Render(p => p.Add(x => x.ColumnDefinitions,
            new List<DataGridColumn<Row>> { new() { Title = "Qty", Value = r => r.Qty } }));

        Assert.Equal(["Qty"], cut.FindAll("thead th").Select(th => th.TextContent.Trim()));
    }
}
