using Flare.Components.Combobox;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareOptionList - the dumb dropdown surface used by the select family (was FlareListbox).
// Renders a prebuilt ComboboxRow projection; highlight/selection come from parameters.
// ------------------------------------------------------------------------------

public class C_FlareOptionListTests : FlareTestContext
{
    private static readonly string[] _items = ["Alpha", "Beta", "Gamma"];

    private static IReadOnlyList<ComboboxRow<string>> Rows(params string[] items) =>
        items.Select((s, i) => new ComboboxRow<string>(false, null, s, i)).ToList();

    private static IReadOnlyList<ComboboxRow<string>> GroupedRows(Func<string, string> group, params string[] items)
    {
        var rows = new List<ComboboxRow<string>>();
        string? last = null;
        var optIdx = 0;
        foreach (var s in items)
        {
            var g = group(s);
            if (!string.Equals(g, last, StringComparison.Ordinal)) { last = g; rows.Add(new ComboboxRow<string>(true, g, default!, -1)); }
            rows.Add(new ComboboxRow<string>(false, null, s, optIdx++));
        }
        return rows;
    }

    [Fact]
    public void RendersRootListboxWithRole()
    {
        var cut = Render<FlareOptionList<string>>(p => p.Add(x => x.Rows, Rows(_items)));
        Assert.Equal("listbox", cut.Find(".flare-listbox").GetAttribute("role"));
    }

    [Fact]
    public void RendersOneOptionPerItem()
    {
        var cut = Render<FlareOptionList<string>>(p => p.Add(x => x.Rows, Rows(_items)));
        Assert.Equal(3, cut.FindAll(".flare-listbox__option").Count);
    }

    [Fact]
    public void GroupBy_RendersGroupHeaders()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, GroupedRows(s => s.StartsWith("A") ? "A" : "B", "Apple", "Avocado", "Banana")));

        var headers = cut.FindAll(".flare-listbox__group-header");
        Assert.Equal(2, headers.Count);
        Assert.Equal("A", headers[0].TextContent);
        Assert.Equal("B", headers[1].TextContent);
    }

    [Fact]
    public void Selected_AppliesSelectedClassAndCheck()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.IsSelected, (Func<string, bool>)(s => s == "Beta"))
            .Add(x => x.ShowCheck, true));

        var opts = cut.FindAll(".flare-listbox__option");
        Assert.Contains("flare-listbox__option--selected", opts[1].ClassName);
        Assert.DoesNotContain("flare-listbox__option--selected", opts[0].ClassName);
        Assert.Equal("true", opts[1].GetAttribute("aria-selected"));
        Assert.Single(cut.FindAll(".flare-listbox__check"));
    }

    [Fact]
    public void HighlightedIndex_AppliesActiveClass()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.HighlightedIndex, 2));

        var opts = cut.FindAll(".flare-listbox__option");
        Assert.Contains("flare-listbox__option--active", opts[2].ClassName);
        Assert.DoesNotContain("flare-listbox__option--active", opts[0].ClassName);
    }

    [Fact]
    public void ShowCheckbox_RendersLeadingVisualCheckboxes_NotNestedControls()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.ShowCheckbox, true));

        // R8: a visual aria-hidden checkbox span per option, not a nested interactive FlareCheckbox.
        Assert.Equal(3, cut.FindAll(".flare-listbox__option .flare-listbox__checkbox").Count);
        Assert.Empty(cut.FindAll(".flare-listbox__option [role='checkbox']"));
    }

    [Fact]
    public void OnSelect_FiresWithClickedItem()
    {
        string? clicked = null;
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.OnSelect, (string s) => clicked = s));

        cut.FindAll(".flare-listbox__option")[1].Click();
        Assert.Equal("Beta", clicked);
    }

    [Fact]
    public void Empty_ShownWhenNoRows()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows())
            .Add(x => x.Empty, (RenderFragment)(b => b.AddMarkupContent(0, "<div class=\"empty-slot\">none</div>"))));

        Assert.NotEmpty(cut.FindAll(".empty-slot"));
        Assert.Empty(cut.FindAll(".flare-listbox__option"));
    }

    [Fact]
    public void ItemTemplate_RendersCustomMarkup()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(v => b => b.AddMarkupContent(0, $"<em class=\"tpl\">{v}</em>"))));

        Assert.Equal(3, cut.FindAll(".flare-listbox__option .tpl").Count);
    }

    [Fact]
    public void OptionClass_AppliedToEveryRow()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.OptionClass, "host-opt"));

        Assert.Equal(3, cut.FindAll(".flare-listbox__option.host-opt").Count);
    }

    [Fact]
    public void Class_AppliedToRoot()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.Class, "host-dropdown"));

        Assert.Contains("host-dropdown", cut.Find(".flare-listbox").ClassName);
    }

    [Fact]
    public void Multiselectable_SetsAriaAttribute()
    {
        var cut = Render<FlareOptionList<string>>(p => p
            .Add(x => x.Rows, Rows(_items))
            .Add(x => x.Multiselectable, true));

        Assert.True(cut.Find(".flare-listbox").HasAttribute("aria-multiselectable"));
    }
}
