using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSelectSearchTests : FlareTestContext
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
