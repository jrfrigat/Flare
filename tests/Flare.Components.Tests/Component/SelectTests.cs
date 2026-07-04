using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareSelect  (8 tests from Wave4)
// ------------------------------------------------------------------------------

public class C_FlareSelectTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareSelect<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-select"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Label, "My Label"));

        Assert.Contains("My Label", cut.Find(".flare-input__label").TextContent);
    }

    [Fact]
    public void RendersControl()
    {
        var cut = Render<FlareSelect<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-select__control"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains("flare-select--disabled", cut.Find(".flare-select").ClassName);
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.HelperText, "Hint text"));

        Assert.Contains("Hint text", cut.Find(".flare-input__helper").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.ErrorText, "Required"));

        Assert.Contains("Required", cut.Find(".flare-input__helper--error").TextContent);
    }

    [Fact]
    public void RendersOptionItems()
    {
        var items = new[] { "Alpha", "Beta", "Gamma" };
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, items));

        cut.Find(".flare-select__control").Click();

        var options = cut.FindAll(".flare-select__option");
        Assert.Equal(3, options.Count);
    }

    [Fact]
    public void AcceptsAdditionalAttributes()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .AddUnmatched("data-testid", "my-select"));

        Assert.Equal("my-select", cut.Find(".flare-select").GetAttribute("data-testid"));
    }

    [Fact]
    public void ItemTemplate_RendersCustomMarkup()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b" })
            .Add(x => x.ItemTemplate, (RenderFragment<string>)(v => b => b.AddMarkupContent(0, $"<em class=\"tpl\">{v}</em>"))));

        cut.Find(".flare-select__control").Click();

        Assert.Equal(2, cut.FindAll(".flare-select__option .tpl").Count);
    }
}

// ------------------------------------------------------------------------------
// FlareAutocomplete  (8 tests from Wave2)
// ------------------------------------------------------------------------------

public class C_FlareAutocompleteTests : FlareTestContext
{
    private static readonly string[] _cities = ["Berlin", "London", "Paris", "Tokyo"];

    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, _cities));

        Assert.NotEmpty(cut.FindAll(".flare-autocomplete"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Label, "City")
            .Add(x => x.Items, _cities));

        Assert.Equal("City", cut.Find("label.flare-autocomplete__label").TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Placeholder, "Search city...")
            .Add(x => x.Items, _cities));

        Assert.Equal("Search city...", cut.Find("input").GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, _cities)
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, _cities)
            .Add(x => x.HelperText, "Start typing to search"));

        Assert.Contains("Start typing to search", cut.Find(".flare-autocomplete__helper").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, _cities)
            .Add(x => x.ErrorText, "City required"));

        Assert.NotEmpty(cut.FindAll(".flare-autocomplete__helper--error"));
    }

    [Fact]
    public void DropdownNotShownInitially()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, _cities));

        Assert.Empty(cut.FindAll(".flare-autocomplete__dropdown"));
    }

    [Fact]
    public void RendersInputField()
    {
        var cut = Render<FlareAutocomplete<string>>(p => p
            .Add(x => x.Items, _cities));

        Assert.NotNull(cut.Find("input.flare-autocomplete__control"));
    }
}

// ------------------------------------------------------------------------------
// FlareMultiSelect  (10 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlareMultiSelectTests : FlareTestContext
{
    private static readonly string[] _fruits = ["Apple", "Banana", "Cherry"];

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Label, "Fruits")
            .Add(x => x.Items, _fruits));

        var label = cut.Find("label.flare-multiselect__label");
        Assert.Equal("Fruits", label.TextContent);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Placeholder, "Pick fruit"));

        Assert.Contains("Pick fruit", cut.Find(".flare-multiselect__placeholder").TextContent);
    }

    [Fact]
    public void DropdownOpensOnClick()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        cut.Find(".flare-multiselect__control").Click();

        Assert.NotEmpty(cut.FindAll(".flare-multiselect__dropdown"));
    }

    [Fact]
    public void DropdownClosesOnEscape()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        cut.Find(".flare-multiselect__control").Click();
        Assert.NotEmpty(cut.FindAll(".flare-multiselect__dropdown"));

        cut.Find(".flare-multiselect__control").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.Empty(cut.FindAll(".flare-multiselect__dropdown"));
    }

    [Fact]
    public void SelectItem_AddsToSelectedValues()
    {
        IReadOnlyCollection<string>? captured = null;
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.SelectedValuesChanged, v => captured = v));

        cut.Find(".flare-multiselect__control").Click();
        cut.FindAll(".flare-multiselect__option")[0].Click();

        Assert.NotNull(captured);
        Assert.Contains("Apple", captured!);
    }

    [Fact]
    public void DeselectItem_RemovesFromSelectedValues()
    {
        IReadOnlyCollection<string>? captured = null;
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.SelectedValues, new[] { "Apple" })
            .Add(x => x.SelectedValuesChanged, v => captured = v));

        cut.Find(".flare-multiselect__control").Click();
        cut.FindAll(".flare-multiselect__option")[0].Click();

        Assert.NotNull(captured);
        Assert.DoesNotContain("Apple", captured!);
    }

    [Fact]
    public void RendersHelperText()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.HelperText, "Choose one or more"));

        Assert.Contains("Choose one or more", cut.Find(".flare-multiselect__helper").TextContent);
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.ErrorText, "Selection required"));

        Assert.NotEmpty(cut.FindAll(".flare-multiselect__helper--error"));
    }

    [Fact]
    public void RendersDisabled_DropdownDoesNotOpen()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Disabled, true));

        cut.Find(".flare-multiselect__control").Click();

        Assert.Empty(cut.FindAll(".flare-multiselect__dropdown"));
    }

    [Fact]
    public void RendersAllItemsInDropdown()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        cut.Find(".flare-multiselect__control").Click();

        var options = cut.FindAll(".flare-multiselect__option");
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

        cut.Find(".flare-multiselect__control").Click();

        Assert.Equal(3, cut.FindAll(".flare-multiselect__option .tpl").Count);
    }
}

// ------------------------------------------------------------------------------
// FlareChipGroup  (8 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlareChipGroupTests : FlareTestContext
{
    private static RenderFragment ChipGroupWith(string[] chipValues, bool multiSelect = false,
        IReadOnlyCollection<string>? selectedValues = null,
        EventCallback<IReadOnlyCollection<string>> selectedValuesChanged = default) =>
        b =>
        {
            b.OpenComponent<FlareChipGroup>(0);
            b.AddAttribute(1, "MultiSelect", multiSelect);
            if (selectedValues is not null)
                b.AddAttribute(2, "SelectedValues", selectedValues);
            if (selectedValuesChanged.HasDelegate)
                b.AddAttribute(3, "SelectedValuesChanged", selectedValuesChanged);
            b.AddAttribute(4, "ChildContent", (RenderFragment)(inner =>
            {
                var seq = 10;
                foreach (var v in chipValues)
                {
                    inner.OpenComponent<FlareChip>(seq++);
                    inner.AddAttribute(seq++, "Value", v);
                    inner.AddAttribute(seq++, "Label", v);
                    inner.CloseComponent();
                }
            }));
            b.CloseComponent();
        };

    [Fact]
    public void RendersChildren()
    {
        var cut = Render(ChipGroupWith(["Red", "Green", "Blue"]));

        Assert.Equal(3, cut.FindAll(".flare-chip").Count);
    }

    [Fact]
    public void SingleSelectMode_OnlyOneChipSelected()
    {
        var cut = Render(ChipGroupWith(["A", "B"], multiSelect: false,
            selectedValues: ["A"]));

        var chips = cut.FindAll(".flare-chip");
        Assert.Contains("flare-chip--selected", chips[0].ClassName);
        Assert.DoesNotContain("flare-chip--selected", chips[1].ClassName);
    }

    [Fact]
    public void MultiSelectMode_AllowsMultipleSelected()
    {
        var cut = Render(ChipGroupWith(["X", "Y", "Z"], multiSelect: true,
            selectedValues: ["X", "Z"]));

        var chips = cut.FindAll(".flare-chip");
        Assert.Contains("flare-chip--selected", chips[0].ClassName);
        Assert.DoesNotContain("flare-chip--selected", chips[1].ClassName);
        Assert.Contains("flare-chip--selected", chips[2].ClassName);
    }

    [Fact]
    public void RendersWithSelectedValues()
    {
        var cut = Render(ChipGroupWith(["One", "Two"], selectedValues: ["Two"]));

        var chips = cut.FindAll(".flare-chip");
        Assert.DoesNotContain("flare-chip--selected", chips[0].ClassName);
        Assert.Contains("flare-chip--selected", chips[1].ClassName);
    }

    [Fact]
    public void SelectedValuesChanged_FiresOnChipToggle()
    {
        IReadOnlyCollection<string>? captured = null;
        var callback = EventCallback.Factory.Create<IReadOnlyCollection<string>>(
            this, v => captured = v);

        var cut = Render(ChipGroupWith(["Alpha", "Beta"],
            selectedValuesChanged: callback));

        cut.FindAll(".flare-chip")[0].Click();

        Assert.NotNull(captured);
        Assert.Contains("Alpha", captured!);
    }

    [Fact]
    public void ChipWithValue_RendersCorrectLabel()
    {
        var cut = Render(ChipGroupWith(["MyValue"]));

        Assert.Contains("MyValue", cut.Find(".flare-chip__label").TextContent);
    }

    [Fact]
    public void RendersSelectedChipWithClass()
    {
        var cut = Render(ChipGroupWith(["P", "Q"], selectedValues: ["Q"]));

        var chips = cut.FindAll(".flare-chip");
        Assert.Contains("flare-chip--selected", chips[1].ClassName);
    }

    [Fact]
    public void ClearSelection_TogglingSelectedChipInSingleMode()
    {
        IReadOnlyCollection<string>? captured = null;
        var callback = EventCallback.Factory.Create<IReadOnlyCollection<string>>(
            this, v => captured = v);

        var cut = Render(ChipGroupWith(["Cat", "Dog"],
            multiSelect: false,
            selectedValues: ["Cat"],
            selectedValuesChanged: callback));

        cut.FindAll(".flare-chip")[0].Click();

        Assert.NotNull(captured);
        Assert.Empty(captured!);
    }
}
