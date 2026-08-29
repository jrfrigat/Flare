using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// A column header can carry markup - a unit, an icon, a help affordance - while the column keeps a
/// plain-text name.
/// </summary>
/// <remarks>
/// The two are deliberately separate. <c>Title</c> is the column's identity and the name it is known by
/// in the export, the filter menu, the column picker and the aggregate rows, all of which need text;
/// <c>TitleContent</c> only changes what the header cell paints. Making the fragment replace the string
/// would have silently emptied every one of those.
/// </remarks>
public class C_DataGridHeaderContentTests : FlareTestContext
{
    private record Row(string Name, int Qty);

    private static readonly Row[] _rows = [new("Beta", 2), new("Alpha", 1)];

    private static RenderFragment Grid(IEnumerable<DataGridColumn<Row>> definitions) => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddComponentParameter(1, nameof(FlareDataGrid<Row>.Items), _rows);
        b.AddComponentParameter(2, nameof(FlareDataGrid<Row>.ColumnDefinitions), definitions);
        b.CloseComponent();
    };

    [Fact]
    public void A_header_renders_markup_when_the_column_supplies_it()
    {
        var cut = Render(Grid([
            new()
            {
                Title = "Price",
                Value = r => r.Qty,
                TitleContent = b => b.AddMarkupContent(0, "Price, <abbr title=\"euro\">EUR</abbr>"),
            },
        ]));

        var th = cut.Find("thead th");
        Assert.Contains("Price", th.TextContent);
        Assert.NotEmpty(th.QuerySelectorAll("abbr"));
    }

    [Fact]
    public void The_plain_title_still_renders_when_no_markup_is_given()
    {
        var cut = Render(Grid([new() { Title = "Qty", Value = r => r.Qty }]));

        Assert.Equal("Qty", cut.Find("thead th").TextContent.Trim());
    }

    [Fact]
    public void Markup_in_the_header_does_not_take_over_the_columns_name()
    {
        // The filter menu names columns in text. If TitleContent had replaced Title rather than only the
        // painted cell, this list would have gone empty for the column carrying markup.
        var cut = Render(Grid([
            new()
            {
                Title = "Price",
                Value = r => r.Qty,
                Filterable = true,
                TitleContent = b => b.AddContent(0, "P"),
            },
            new() { Title = "Name", Value = r => r.Name, Filterable = true },
        ]));

        var grid = cut.FindComponent<FlareDataGrid<Row>>().Instance;
        Assert.Equal(["Price", "Name"], grid.FilterableColumns.Select(c => c.Title));
    }

    [Fact]
    public void A_sortable_header_keeps_its_sort_affordance_beside_the_markup()
    {
        var cut = Render(Grid([
            new()
            {
                Title = "Qty",
                Value = r => r.Qty,
                Sortable = true,
                TitleContent = b => b.AddMarkupContent(0, "<em>Qty</em>"),
            },
        ]));

        var th = cut.Find("thead th");
        Assert.NotEmpty(th.QuerySelectorAll("em"));

        // The grid has this one column, so the cells are the Qty values: 2, 1 unsorted -> 1, 2 ascending.
        th.Click();
        Assert.Equal(["1", "2"], cut.FindAll("tbody tr td").Select(td => td.TextContent.Trim()));
    }
}
