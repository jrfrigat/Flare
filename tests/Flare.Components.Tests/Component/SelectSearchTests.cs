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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Select.Control} .{Css.Classes.Select.Search}"));
    }

    [Fact]
    public void Select_NotSearchable_HasNoSearchInput()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits));

        Assert.Empty(cut.FindAll($".{Css.Classes.Select.Search}"));
    }

    [Fact]
    public void Select_Searchable_TypingFiltersOptions()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        cut.Find($".{Css.Classes.Select.Search}").Input("Ban");

        var options = cut.FindAll($".{Css.Classes.Select.Option}");
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Multiselect.Control} .{Css.Classes.Select.Search}"));
        // The old in-dropdown search box no longer exists.
        Assert.Empty(cut.FindAll($".{Css.Classes.Multiselect.Root}__search"));
    }

    [Fact]
    public void MultiSelect_Searchable_TypingFiltersOptions()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        cut.Find($".{Css.Classes.Select.Search}").Input("Ch");

        var options = cut.FindAll($".{Css.Classes.Multiselect.Option}");
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

        cut.Find($".{Css.Classes.Select.Search}").KeyDown(new KeyboardEventArgs { Key = "Backspace" });

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

        var combo = cut.Find($".{Css.Classes.Multiselect.Control}");
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
        Assert.Null(cut.Find($".{Css.Classes.Select.Control}").GetAttribute("role"));
        Assert.Equal("combobox", cut.Find($".{Css.Classes.Select.Search}").GetAttribute("role"));
    }

    [Fact]
    public void Searchable_ArrowDown_SetsActiveDescendantToARenderedOption()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _fruits)
            .Add(x => x.Searchable, true));

        cut.Find($".{Css.Classes.Select.Control}").Click();   // opens + enters edit mode
        cut.Find($".{Css.Classes.Select.Search}").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        var active = cut.Find($".{Css.Classes.Select.Search}").GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(active));
        // The referenced option id actually exists in the rendered listbox.
        Assert.NotNull(cut.Find($"#{active}"));
    }
}

// ------------------------------------------------------------------------------
// Uncontrolled selection (no @bind-Value / @bind-Values)
// ------------------------------------------------------------------------------

public class C_FlareSelectUncontrolledTests : FlareTestContext
{
    private static readonly string[] _items = ["Alpha", "Beta", "Gamma"];

    [Fact]
    public void Select_Uncontrolled_ShowsSelectionWithoutBinding()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Items, _items));

        cut.Find($".{Css.Classes.Select.Control}").Click();
        cut.FindAll($".{Css.Classes.Select.Option}")[1].Click();   // pick "Beta"

        Assert.Contains("Beta", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    [Fact]
    public void Select_Uncontrolled_SeedsFromOneWayValue()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Value, "Gamma"));

        Assert.Contains("Gamma", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    [Fact]
    public void MultiSelect_Uncontrolled_AccumulatesSelectionWithoutBinding()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Items, _items));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll($".{Css.Classes.Multiselect.Option}")[0].Click();   // Alpha
        cut.FindAll($".{Css.Classes.Multiselect.Option}")[2].Click();   // Gamma

        var shown = cut.Find($".{Css.Classes.Multiselect.Value}").TextContent;
        Assert.Contains("Alpha", shown);
        Assert.Contains("Gamma", shown);
    }
}
