using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class MultiSelectSelectAllKeyboardTests : FlareTestContext
{
    private static readonly string[] Cities = ["Berlin", "Boston", "Paris"];
    private static string Control => $".{Css.Classes.Multiselect.Control}";
    private static string SearchInput => $"input.{Css.Classes.Select.Search}";
    private static string SelectAllRow => $".{Css.Classes.Multiselect.OptionSelectAll}";

    private static KeyboardEventArgs CtrlA => new() { Key = "a", CtrlKey = true };

    private static void Open(IRenderedComponent<FlareMultiSelect<string>> cut)
        => cut.Find(Control).Click();

    private IRenderedComponent<FlareMultiSelect<string>> RenderCities(
        Action<ComponentParameterCollectionBuilder<FlareMultiSelect<string>>>? configure = null,
        List<IReadOnlyList<string>>? captured = null)
        => Render<FlareMultiSelect<string>>(p =>
        {
            p.Add(x => x.Items, Cities).Add(x => x.ShowSelectAll, true);
            if (captured is not null)
                p.Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(
                    captured, v => captured.Add(v)));
            configure?.Invoke(p);
        });

    [Fact]
    public void CtrlA_TogglesEveryVisibleItem()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderCities(captured: captured);
        Open(cut);

        cut.Find(Control).KeyDown(CtrlA);
        Assert.Equal(Cities, captured[^1]);

        cut.Find(Control).KeyDown(CtrlA);
        Assert.Empty(captured[^1]);
    }

    [Fact]
    public void CtrlA_SkipsDisabledItems()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderCities(p => p.Add(x => x.ItemDisabled, s => s == "Boston"), captured);
        Open(cut);

        cut.Find(Control).KeyDown(CtrlA);

        Assert.Equal(["Berlin", "Paris"], captured[^1]);
    }

    [Fact]
    public void CtrlA_StopsAtMaxSelections()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderCities(p => p.Add(x => x.MaxSelections, 2), captured);
        Open(cut);

        cut.Find(Control).KeyDown(CtrlA);

        Assert.Equal(2, captured[^1].Count);
    }

    [Fact]
    public void CtrlA_AppliesToTheFilteredRows()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = RenderCities(p => p.Add(x => x.Searchable, true), captured);
        cut.Find(SearchInput).Input("ber");

        cut.Find(SearchInput).KeyDown(CtrlA);

        Assert.Equal(["Berlin"], captured[^1]);
    }

    [Fact]
    public void CtrlA_DoesNothingWithoutSelectAll()
    {
        var captured = new List<IReadOnlyList<string>>();
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(
                captured, v => captured.Add(v))));
        Open(cut);

        cut.Find(Control).KeyDown(CtrlA);

        Assert.Empty(captured);
    }

    [Fact]
    public void SelectAllRow_AnnouncesTheMixedState()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.ShowSelectAll, true)
            .Add(x => x.Values, new[] { "Berlin" }));
        Open(cut);

        Assert.Equal("mixed", cut.Find(SelectAllRow).GetAttribute("aria-checked"));
        Assert.Equal("Control+A Meta+A", cut.Find(Control).GetAttribute("aria-keyshortcuts"));

        cut.Find(Control).KeyDown(CtrlA);

        Assert.Equal("true", cut.Find(SelectAllRow).GetAttribute("aria-checked"));
    }

    [Fact]
    public void Keyshortcut_IsAbsentWithoutSelectAll()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Items, Cities));

        Assert.Null(cut.Find(Control).GetAttribute("aria-keyshortcuts"));
    }
}
