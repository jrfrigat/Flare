using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// Column flags are independent switches and have to survive being combined. They were only ever
/// demonstrated and tested one at a time - the Gallery has a frozen demo and a sortable demo and, until
/// this batch, none with both - which is how "Sortable + Frozen throws at runtime" could be reported
/// against a shipped release with a green suite.
///
/// Every combination is rendered AND sorted (the header click runs the full sort path, re-projection and
/// re-render included), because the reported failure was on interaction, not on first paint.
/// </summary>
public class DataGridColumnFlagCombinationTests : FlareTestContext
{
    private record Row(string Name, int Qty, bool Active);

    private static readonly Row[] _rows =
    [
        new("Beta", 2, true),
        new("Alpha", 1, false),
        new("Gamma", 3, true),
    ];

    private static RenderFragment Grid(
        bool sortable, bool frozen, bool frozenRight, bool resizable, bool filterable) => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Row>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name));
            if (sortable) inner.AddAttribute(13, "Sortable", true);
            if (frozen) inner.AddAttribute(14, "Frozen", true);
            if (frozenRight) inner.AddAttribute(15, "FrozenRight", true);
            if (resizable) inner.AddAttribute(16, "Resizable", true);
            if (filterable)
            {
                inner.AddAttribute(17, "Filterable", true);
                inner.AddAttribute(18, "FilterDebounceMs", 0);
            }
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Row>>(20);
            inner.AddAttribute(21, "Title", "Qty");
            inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Qty));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    public static TheoryData<bool, bool, bool, bool, bool> Combinations()
    {
        var data = new TheoryData<bool, bool, bool, bool, bool>();
        foreach (var sortable in new[] { false, true })
        foreach (var frozen in new[] { false, true })
        foreach (var frozenRight in new[] { false, true })
        foreach (var resizable in new[] { false, true })
        foreach (var filterable in new[] { false, true })
        {
            // Frozen and FrozenRight are mutually exclusive by design (a column pins to one edge).
            if (frozen && frozenRight) continue;
            data.Add(sortable, frozen, frozenRight, resizable, filterable);
        }
        return data;
    }

    [Theory]
    [MemberData(nameof(Combinations))]
    public void EveryColumnFlagCombination_RendersAndSorts(
        bool sortable, bool frozen, bool frozenRight, bool resizable, bool filterable)
    {
        var cut = Render(Grid(sortable, frozen, frozenRight, resizable, filterable));
        Assert.NotEmpty(cut.FindAll("th"));

        // Clicking the header is a no-op on a non-sortable column and the whole sort path on a sortable
        // one; either way it must not throw, and twice covers the ascending -> descending toggle.
        cut.FindAll("th")[0].Click();
        cut.FindAll("th")[0].Click();

        if (sortable)
        {
            var first = cut.FindAll("tbody tr td")[0].TextContent.Trim();
            Assert.Equal("Gamma", first); // descending after two clicks
        }
    }

    [Fact]
    public void FrozenAndSortableColumn_CarriesBothHeaderModifiers()
    {
        var cut = Render(Grid(sortable: true, frozen: true, frozenRight: false, resizable: false, filterable: false));
        var th = cut.FindAll("th")[0];
        Assert.Contains("flare-datagrid__th--sortable", th.ClassName);
        Assert.Contains("flare-datagrid__th--frozen", th.ClassName);
    }
}
