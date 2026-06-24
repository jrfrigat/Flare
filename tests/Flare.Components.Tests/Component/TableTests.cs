using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareTable  (8 tests from Wave8)
// ------------------------------------------------------------------------------

public class C_FlareTableTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareTableElement()
    {
        var cut = RenderComponent<FlareTable<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-table"));
    }

    [Fact]
    public void StickyHeader_True_AddsStickyClass()
    {
        var cut = RenderComponent<FlareTable<string>>(p => p
            .Add(x => x.StickyHeader, true));

        Assert.Contains("flare-table-container--sticky", cut.Find(".flare-table-container").ClassName ?? "");
    }

    [Fact]
    public void GroupBy_RendersGroupRow_WhenItemsAreGrouped()
    {
        var items = new[] { "Apple", "Avocado", "Banana" };
        var cut = RenderComponent<FlareTable<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.GroupBy, (Func<string, string>)(s => s.StartsWith("A") ? "A-Group" : "B-Group"))
            .Add(x => x.RowTemplate, (RenderFragment<string>)(item => builder =>
            {
                builder.OpenElement(0, "tr");
                builder.OpenElement(1, "td");
                builder.AddContent(2, item);
                builder.CloseElement();
                builder.CloseElement();
            })));

        Assert.NotEmpty(cut.FindAll(".flare-table__group-row"));
    }

    [Fact]
    public void ShowFooter_True_RendersTfoot()
    {
        var items = new[] { "One", "Two" };
        var cut = RenderComponent<FlareTable<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.ShowFooter, true)
            .Add(x => x.FooterContent, (RenderFragment<IEnumerable<string>>)(allItems => builder =>
            {
                builder.OpenElement(0, "tr");
                builder.OpenElement(1, "td");
                builder.AddContent(2, "footer");
                builder.CloseElement();
                builder.CloseElement();
            })));

        Assert.NotEmpty(cut.FindAll("tfoot"));
    }

    [Fact]
    public void Items_WithoutGroupBy_RendersItemsNormally()
    {
        var items = new[] { "Alpha", "Beta", "Gamma" };
        var cut = RenderComponent<FlareTable<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.RowTemplate, (RenderFragment<string>)(item => builder =>
            {
                builder.OpenElement(0, "tr");
                builder.AddAttribute(1, "data-item", item);
                builder.CloseElement();
            })));

        Assert.Equal(3, cut.FindAll("tr[data-item]").Count);
    }

    [Fact]
    public void ChildContent_BackwardCompat_RendersWithoutError()
    {
        var cut = RenderComponent<FlareTable<string>>(p => p
            .AddChildContent("<thead><tr><th>Name</th></tr></thead><tbody></tbody>"));

        Assert.NotEmpty(cut.FindAll("th"));
    }

    [Fact]
    public void GroupRow_IsCollapsible_ClickingTogglesChildren()
    {
        var items = new[] { "Apple", "Avocado" };
        var cut = RenderComponent<FlareTable<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.GroupBy, (Func<string, string>)(_ => "A-Group"))
            .Add(x => x.RowTemplate, (RenderFragment<string>)(item => builder =>
            {
                builder.OpenElement(0, "tr");
                builder.AddAttribute(1, "data-row", item);
                builder.CloseElement();
            })));

        var rowsBefore = cut.FindAll("tr[data-row]").Count;
        Assert.Equal(2, rowsBefore);

        cut.Find(".flare-table__group-row").Click();

        var rowsAfter = cut.FindAll("tr[data-row]").Count;
        Assert.Equal(0, rowsAfter);
    }

    [Fact]
    public void ColSpan_ParamExists_RendersWithoutError()
    {
        var items = new[] { "X" };
        var cut = RenderComponent<FlareTable<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.ColSpan, 5)
            .Add(x => x.GroupBy, (Func<string, string>)(_ => "G"))
            .Add(x => x.RowTemplate, (RenderFragment<string>)(item => builder =>
            {
                builder.OpenElement(0, "tr");
                builder.CloseElement();
            })));

        Assert.Equal("5", cut.Find(".flare-table__group-row td").GetAttribute("colspan"));
    }
}
