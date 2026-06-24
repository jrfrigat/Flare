using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareListbox - the shared dropdown surface used by Select / MultiSelect / Autocomplete
// ------------------------------------------------------------------------------

public class C_FlareListboxTests : FlareTestContext
{
    private static readonly string[] _items = ["Alpha", "Beta", "Gamma"];

    [Fact]
    public void RendersRootListboxWithRole()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items));

        var root = cut.Find(".flare-listbox");
        Assert.Equal("listbox", root.GetAttribute("role"));
    }

    [Fact]
    public void RendersOneOptionPerItem()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items));

        Assert.Equal(3, cut.FindAll(".flare-listbox__option").Count);
    }

    [Fact]
    public void GroupBy_RendersGroupHeaders()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, new[] { "Apple", "Avocado", "Banana" })
            .Add(x => x.GroupBy, (Func<string, string>)(s => s.StartsWith("A") ? "A" : "B")));

        var headers = cut.FindAll(".flare-listbox__group-header");
        Assert.Equal(2, headers.Count);
        Assert.Equal("A", headers[0].TextContent);
        Assert.Equal("B", headers[1].TextContent);
    }

    [Fact]
    public void Selected_AppliesSelectedClassAndCheck()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Selected, (Func<string, bool>)(s => s == "Beta"))
            .Add(x => x.ShowCheck, true));

        var opts = cut.FindAll(".flare-listbox__option");
        Assert.Contains("flare-listbox__option--selected", opts[1].ClassName);
        Assert.DoesNotContain("flare-listbox__option--selected", opts[0].ClassName);
        Assert.True(opts[1].HasAttribute("aria-selected"));
        Assert.Single(cut.FindAll(".flare-listbox__check"));
    }

    [Fact]
    public void ActiveIndex_AppliesActiveClass()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.ActiveIndex, 2));

        var opts = cut.FindAll(".flare-listbox__option");
        Assert.Contains("flare-listbox__option--active", opts[2].ClassName);
        Assert.DoesNotContain("flare-listbox__option--active", opts[0].ClassName);
    }

    [Fact]
    public void Multiple_RendersLeadingCheckboxRows()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Multiple, true));

        Assert.Equal(3, cut.FindAll(".flare-listbox__option .flare-checkbox").Count);
    }

    [Fact]
    public void OnSelect_FiresWithClickedItem()
    {
        string? clicked = null;
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.OnSelect, (string s) => clicked = s));

        cut.FindAll(".flare-listbox__option")[1].Click();

        Assert.Equal("Beta", clicked);
    }

    [Fact]
    public void EmptyContent_ShownWhenNoItems()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, Array.Empty<string>())
            .Add(x => x.EmptyContent, (RenderFragment)(b => b.AddMarkupContent(0, "<div class=\"empty-slot\">none</div>"))));

        Assert.NotEmpty(cut.FindAll(".empty-slot"));
        Assert.Empty(cut.FindAll(".flare-listbox__option"));
    }

    [Fact]
    public void ItemTemplate_RendersCustomMarkup()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(v => b => b.AddMarkupContent(0, $"<em class=\"tpl\">{v}</em>"))));

        Assert.Equal(3, cut.FindAll(".flare-listbox__option .tpl").Count);
    }

    [Fact]
    public void OptionClass_AppliedToEveryRow()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.OptionClass, "host-opt"));

        Assert.Equal(3, cut.FindAll(".flare-listbox__option.host-opt").Count);
    }

    [Fact]
    public void DropdownClass_AppliedToRoot()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.DropdownClass, "host-dropdown"));

        Assert.Contains("host-dropdown", cut.Find(".flare-listbox").ClassName);
    }

    [Fact]
    public void Multiselectable_SetsAriaAttribute()
    {
        var cut = RenderComponent<FlareListbox<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Multiselectable, true));

        Assert.True(cut.Find(".flare-listbox").HasAttribute("aria-multiselectable"));
    }
}
