using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class MultiSelectChipRemovalTests : FlareTestContext
{
    private static string ChipRemove => $".{Css.Classes.Multiselect.ChipRemove}";
    private static string Chip => $".{Css.Classes.Multiselect.Chip}";
    private static string SearchInput => $"input.{Css.Classes.Select.Search}";

    private IRenderedComponent<FlareMultiSelect<string>> RenderWithSelectedDisabled(
        List<IReadOnlyList<string>> captured, bool searchable = false)
        => Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, new[] { "Berlin", "Boston" })
            .Add(x => x.Values, new[] { "Berlin" })
            .Add(x => x.Chips, true)
            .Add(x => x.Searchable, searchable)
            .Add(x => x.ItemDisabled, s => s == "Berlin")
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(
                captured, v => captured.Add(v))));

    [Fact]
    public void RemovingADisabledSelectedChip_DropsItAndPushesTheValues()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderWithSelectedDisabled(captured);

        cut.Find(ChipRemove).Click();

        Assert.Single(captured);
        Assert.Empty(captured[0]);
        Assert.Empty(cut.FindAll(Chip));
    }

    [Fact]
    public void Backspace_RemovesADisabledSelectedChip()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderWithSelectedDisabled(captured, searchable: true);

        cut.Find(SearchInput).KeyDown(new KeyboardEventArgs { Key = "Backspace" });

        Assert.Single(captured);
        Assert.Empty(captured[0]);
    }

    [Fact]
    public void RemovingAnEnabledSelectedChip_StillPushesTheValues()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, new[] { "Berlin", "Boston" })
            .Add(x => x.Values, new[] { "Berlin" })
            .Add(x => x.Chips, true)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(
                captured, v => captured.Add(v))));

        cut.Find(ChipRemove).Click();

        Assert.Single(captured);
        Assert.Empty(captured[0]);
        Assert.Empty(cut.FindAll(Chip));
    }
}
