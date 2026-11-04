using System.Globalization;
using Flare.Abstractions;
using Flare.Components.Resources;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public sealed class LocalizedDataTests : FlareTestContext
{
    private sealed record Row(string Name);

    [Fact]
    public async Task LastSortAnnouncementIsResolvedAgainWithoutLosingSortOrRows()
    {
        var culture = new FlareLocalization();
        culture.SetCulture(CultureInfo.GetCultureInfo("en"));
        Services.AddSingleton<IFlareLocalization>(culture);
        RenderFragment columns = b =>
        {
            b.OpenComponent<FlareColumn<Row>>(0);
            b.AddAttribute(1, "Title", "Name");
            b.AddAttribute(2, "Field", (Func<Row, object?>)(x => x.Name));
            b.AddAttribute(3, "Sortable", true);
            b.CloseComponent();
        };
        var cut = Render<FlareLocalizationProvider>(p => p.AddChildContent<FlareDataGrid<Row>>(g => g
            .Add(x => x.Items, new[] { new Row("B"), new Row("A") }).Add(x => x.Columns, columns)));
        var grid = cut.FindComponent<FlareDataGrid<Row>>().Instance;
        cut.Find("th.flare-datagrid__th--sortable").Click();
        Assert.Contains("Sorted by Name", cut.Find("[role=status]").TextContent);
        await cut.InvokeAsync(() => culture.SetCulture(CultureInfo.GetCultureInfo("ru")));
        var expected = culture.GetLocalizer<FlareStrings>()["DataGrid_AnnounceSortAsc", "Name"].Value;
        cut.WaitForAssertion(() => Assert.Equal(expected, cut.Find("[role=status]").TextContent));
        Assert.Same(grid, cut.FindComponent<FlareDataGrid<Row>>().Instance);
        Assert.Equal("ascending", cut.Find("th[aria-sort]").GetAttribute("aria-sort"));
        Assert.Contains("A", cut.Find("tbody").TextContent);
        Assert.Contains("B", cut.Find("tbody").TextContent);
    }
}
