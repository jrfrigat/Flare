using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

// New-feature tests for the engine-backed FlareSelect / FlareMultiSelect (Clearable, ItemDisabled,
// type-ahead, select-all, MaxSelections). The pre-existing Select suites cover the retained behaviour.
public class FlareSelectRebuildTests : FlareTestContext
{
    private static readonly string[] Fruits = ["Apple", "Apricot", "Banana", "Cherry"];

    [Fact]
    public void Select_clear_button_resets_value()
    {
        string? bound = "Banana";
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find($".{Css.Classes.Autocomplete.Clear}").Click();
        Assert.Null(bound);
    }

    [Fact]
    public void Select_typeahead_highlights_matching_option()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Items, Fruits));
        var control = cut.Find($".{Css.Classes.Select.Control}");
        control.KeyDown(new KeyboardEventArgs { Key = "b" });   // type-ahead -> Banana

        var active = control.GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(active));
        var highlighted = cut.Find($".{Css.Classes.Listbox.OptionActive}");
        Assert.Contains("Banana", highlighted.TextContent);
    }

    [Fact]
    public void Select_disabled_option_is_not_committable()
    {
        string? bound = null;
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.ItemDisabled, (Func<string, bool>)(s => s == "Banana"))
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find($".{Css.Classes.Select.Control}").Click();   // open
        var banana = cut.FindAll($".{Css.Classes.Listbox.Option}").First(o => o.TextContent.Contains("Banana"));
        Assert.Equal("true", banana.GetAttribute("aria-disabled"));
        banana.Click();
        Assert.Null(bound);
    }

    [Fact]
    public void MultiSelect_chips_render_for_selected_values()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.Chips, true)
            .Add(x => x.Values, new[] { "Apple", "Cherry" }));

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Multiselect.Chip}").Count);
    }

    [Fact]
    public void MultiSelect_comma_list_when_chips_off()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.Chips, false)
            .Add(x => x.Values, new[] { "Apple", "Cherry" }));

        Assert.Contains("Apple, Cherry", cut.Find($".{Css.Classes.Multiselect.Value}").TextContent);
    }

    [Fact]
    public void MultiSelect_select_all_selects_every_option()
    {
        IReadOnlyList<string> bound = [];
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.ShowSelectAll, true)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => bound = v)));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();   // open
        cut.Find($".{Css.Classes.Multiselect.OptionSelectAll}").Click();

        Assert.Equal(4, bound.Count);
    }

    [Fact]
    public void MultiSelect_stays_open_after_selecting_an_option()
    {
        // Regression: an unbound Open=false must not force-close the dropdown when the parent re-renders
        // after ValuesChanged. Multi-select keeps the dropdown open for the next pick.
        IReadOnlyList<string> bound = [];
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => bound = v)));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();      // open
        cut.FindAll($".{Css.Classes.Listbox.Option}")[0].Click();     // pick Apple

        Assert.Single(bound);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Listbox.Root}"));        // still open
    }

    [Fact]
    public void MultiSelect_uncontrolled_keeps_selection_across_parent_rerender()
    {
        // Regression: uncontrolled (no @bind-Values) selection must survive a parent re-render. Overwriting
        // the incoming-Values tracker on our own push made OnParametersSet wipe the engine selection.
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Items, Fruits));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll($".{Css.Classes.Listbox.Option}")[0].Click();   // pick Apple
        Assert.Contains("Apple", cut.Find($".{Css.Classes.Multiselect.Value}").TextContent);

        cut.Render(p => p.Add(x => x.Items, Fruits));   // simulate a parent re-render
        Assert.Contains("Apple", cut.Find($".{Css.Classes.Multiselect.Value}").TextContent);
    }

    [Fact]
    public void MultiSelect_MaxSelections_blocks_and_fires_callback()
    {
        var reached = 0;
        IReadOnlyList<string> bound = ["Apple", "Apricot"];
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, Fruits)
            .Add(x => x.MaxSelections, 2)
            .Add(x => x.Values, bound)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => bound = v))
            .Add(x => x.OnMaxSelectionsReached, EventCallback.Factory.Create(this, () => reached++)));

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        var cherry = cut.FindAll($".{Css.Classes.Listbox.Option}").First(o => o.TextContent.Contains("Cherry"));
        cherry.Click();

        Assert.Equal(2, bound.Count);      // unchanged
        Assert.Equal(1, reached);
    }

    [Fact]
    public void MultiSelect_options_have_aria_multiselectable_and_no_nested_checkbox()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p.Add(x => x.Items, Fruits));
        cut.Find($".{Css.Classes.Multiselect.Control}").Click();

        Assert.True(cut.Find($".{Css.Classes.Listbox.Root}").HasAttribute("aria-multiselectable"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Listbox.Option} [role='checkbox']"));   // R8
        Assert.Equal(4, cut.FindAll($".{Css.Classes.Listbox.Option} .{Css.Classes.Listbox.Checkbox}").Count);
    }
}
