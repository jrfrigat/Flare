using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// In-field search (Searchable) for the unified Select / MultiSelect
// ------------------------------------------------------------------------------

public class C_FlareSelectSearchTests : FlareTestContext
{
    private static readonly string[] _fruits = ["Apple", "Banana", "Cherry", "Date"];

    [Fact]
    public void Select_Searchable_RendersSearchInputInsideTrigger()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        Assert.NotEmpty(cut.FindAll(".flare-select__control .flare-select__search"));
    }

    [Fact]
    public void Select_NotSearchable_HasNoSearchInput()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        Assert.Empty(cut.FindAll(".flare-select__search"));
    }

    [Fact]
    public void Select_Searchable_TypingFiltersOptions()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        cut.Find(".flare-select__search").Input("Ban");

        var options = cut.FindAll(".flare-select__option");
        Assert.Single(options);
        Assert.Contains("Banana", options[0].TextContent);
    }

    [Fact]
    public void MultiSelect_Searchable_RendersSearchInsideTrigger_NotInDropdown()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        // Search input lives in the trigger, sharing the flare-select__search class.
        Assert.NotEmpty(cut.FindAll(".flare-multiselect__control .flare-select__search"));
        // The old in-dropdown search box no longer exists.
        Assert.Empty(cut.FindAll(".flare-multiselect__search"));
    }

    [Fact]
    public void MultiSelect_Searchable_TypingFiltersOptions()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        cut.Find(".flare-select__search").Input("Ch");

        var options = cut.FindAll(".flare-multiselect__option");
        Assert.Single(options);
        Assert.Contains("Cherry", options[0].TextContent);
    }

    [Fact]
    public void MultiSelect_Searchable_BackspaceOnEmptyQueryRemovesLastChip()
    {
        IReadOnlyList<string>? captured = null;
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Values, new[] { "Apple", "Banana" })
            .Add(x => x.Searchable, true)
            .Add(x => x.ValuesChanged, v => captured = v));

        cut.Find(".flare-select__search").KeyDown(new KeyboardEventArgs { Key = "Backspace" });

        Assert.NotNull(captured);
        Assert.DoesNotContain("Banana", captured!);
        Assert.Contains("Apple", captured!);
    }
}

// ------------------------------------------------------------------------------
// Combobox ARIA on the unified Select / MultiSelect
// ------------------------------------------------------------------------------

public class C_FlareSelectAriaTests : FlareTestContext
{
    private static readonly string[] _fruits = ["Apple", "Banana", "Cherry", "Date"];

    [Fact]
    public void MultiSelect_NonSearchable_NamesComboboxFromPlaceholder_WhenNoLabel()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Placeholder, "Pick fruit"));

        var combo = cut.Find(".flare-multiselect__control");
        Assert.Equal("combobox", combo.GetAttribute("role"));
        Assert.Equal("Pick fruit", combo.GetAttribute("aria-label"));
    }

    [Fact]
    public void Searchable_MovesComboboxRoleFromDivToInput()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        // The div is presentational when searchable; the focused input carries the combobox role.
        Assert.Null(cut.Find(".flare-select__control").GetAttribute("role"));
        Assert.Equal("combobox", cut.Find(".flare-select__search").GetAttribute("role"));
    }

    [Fact]
    public void Searchable_ArrowDown_SetsActiveDescendantToARenderedOption()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        cut.Find(".flare-select__control").Click();   // opens + enters edit mode
        cut.Find(".flare-select__search").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        var active = cut.Find(".flare-select__search").GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(active));
        // The referenced option id actually exists in the rendered listbox.
        Assert.NotNull(cut.Find($"#{active}"));
    }
}
