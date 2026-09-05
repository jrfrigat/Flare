using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSelectAriaTests : FlareTestContext
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
