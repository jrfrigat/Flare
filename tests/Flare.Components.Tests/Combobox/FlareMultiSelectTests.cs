using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareMultiSelectTests : FlareTestContext
{
    private static readonly string[] _fruits = ["Apple", "Banana", "Cherry"];

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Label, "Fruits")
            .Add(x => x.Items, _fruits));

        var label = cut.Find($"label.{Css.Classes.Input.Label}");
        Assert.Equal("Fruits", label.TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Placeholder, "Pick fruit"));

        Assert.Contains("Pick fruit", cut.Find($".{Css.Classes.Input.Placeholder}").TextContent);
    }

    [Fact]
    public void DropdownOpensOnClick()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Multiselect.Dropdown}"));
    }

    [Fact]
    public void DropdownClosesOnEscape()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Multiselect.Dropdown}"));

        cut.Find($".{Css.Classes.Multiselect.Control}").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.Empty(cut.FindAll($".{Css.Classes.Multiselect.Dropdown}"));
    }

    [Fact]
    public void SelectItem_AddsToSelectedValues()
    {
        IReadOnlyList<string>? captured = null;
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.ValuesChanged, v => captured = v));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll($".{Css.Classes.Multiselect.Option}")[0].Click();

        Assert.NotNull(captured);
        Assert.Contains("Apple", captured!);
    }

    [Fact]
    public void DeselectItem_RemovesFromSelectedValues()
    {
        IReadOnlyList<string>? captured = null;
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Values, new[] { "Apple" })
            .Add(x => x.ValuesChanged, v => captured = v));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll($".{Css.Classes.Multiselect.Option}")[0].Click();

        Assert.NotNull(captured);
        Assert.DoesNotContain("Apple", captured!);
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.HelperText, "Choose one or more"));

        Assert.Contains("Choose one or more", cut.Find($".{Css.Classes.Input.Helper}").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.ErrorText, "Selection required"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Input.HelperError}"));
    }

    [Fact]
    public void RendersDisabled_DropdownDoesNotOpen()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Disabled, true));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();

        Assert.Empty(cut.FindAll($".{Css.Classes.Multiselect.Dropdown}"));
    }

    [Fact]
    public void RendersAllItemsInDropdown()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();

        var options = cut.FindAll($".{Css.Classes.Multiselect.Option}");
        Assert.Equal(3, options.Count);
        Assert.Contains("Apple", options[0].TextContent);
        Assert.Contains("Banana", options[1].TextContent);
        Assert.Contains("Cherry", options[2].TextContent);
    }

    [Fact]
    public void ItemTemplate_RendersCustomMarkup()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(v => b => b.AddMarkupContent(0, $"<em class=\"tpl\">{v}</em>"))));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Multiselect.Option} .tpl").Count);
    }
}

// ------------------------------------------------------------------------------
// FlareChipGroup  (8 tests from Wave1)
// ------------------------------------------------------------------------------
