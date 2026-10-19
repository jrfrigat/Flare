using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class SelectClearLayoutTests : FlareTestContext
{
    private static readonly string[] Items = ["a", "b", "c"];
    private static string ClearButton => $".{Css.Classes.Autocomplete.Clear}";

    private static IElement ArrowAfter(IElement clear) => clear.NextElementSibling!;

    [Fact]
    public void ClearingAValue_KeepsTheListClosedAndRaisesNull()
    {
        var values = new List<string?>();
        var opens = new List<bool>();
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Value, "a")
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, values.Add))
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, opens.Add)));

        cut.Find(ClearButton).Click(new MouseEventArgs());

        Assert.Null(values[^1]);
        Assert.Empty(opens);
        Assert.Empty(cut.FindAll("[role=listbox]"));
    }

    [Fact]
    public void ClearingASearchableValue_KeepsTheListClosed()
    {
        var values = new List<string?>();
        var opens = new List<bool>();
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Value, "a")
            .Add(x => x.Searchable, true)
            .Add(x => x.Clearable, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, values.Add))
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, opens.Add)));

        cut.Find(ClearButton).Click(new MouseEventArgs());

        Assert.Null(values[^1]);
        Assert.Empty(opens);
    }

    [Fact]
    public void TheClearButton_ComesRightBeforeTheArrow()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Value, "a")
            .Add(x => x.Clearable, true));

        var clear = cut.Find(ClearButton);
        Assert.Contains(Css.Classes.Input.Arrow, ArrowAfter(clear).ClassName ?? string.Empty);
    }

    [Fact]
    public void Multiselect_PutsTheClearButtonRightBeforeTheArrow()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Values, new[] { "a" })
            .Add(x => x.Clearable, true));

        var clear = cut.Find(ClearButton);
        Assert.Contains(Css.Classes.Input.Arrow, ArrowAfter(clear).ClassName ?? string.Empty);
    }

    [Fact]
    public void MultiselectClear_DropsEveryValueWithoutOpeningTheList()
    {
        var values = new List<IReadOnlyList<string>>();
        var opens = new List<bool>();
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Values, new[] { "a", "b" })
            .Add(x => x.Clearable, true)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, values.Add))
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, opens.Add)));

        cut.Find(ClearButton).Click(new MouseEventArgs());

        Assert.Empty(values[^1]);
        Assert.Empty(opens);
    }

    [Fact]
    public void AnEmptyValue_HasNoClearButton()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Items)
            .Add(x => x.Clearable, true));

        Assert.Empty(cut.FindAll(ClearButton));
    }
}
