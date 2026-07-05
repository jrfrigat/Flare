using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Combobox;

// bUnit tests for the FlareCombobox shell (the editable combobox over the headless engine).
public class FlareComboboxTests : FlareTestContext
{
    private static readonly string[] Cities = ["Berlin", "London", "Paris", "Tokyo"];

    [Fact]
    public void Renders_root_input_with_combobox_role()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        Assert.NotEmpty(cut.FindAll(".flare-autocomplete"));
        var input = cut.Find("input.flare-input__control");
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("false", input.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Dropdown_closed_initially()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        Assert.Empty(cut.FindAll(".flare-listbox"));
    }

    [Fact]
    public void Focus_opens_dropdown_with_all_options()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        cut.Find("input").Focus();
        Assert.NotEmpty(cut.FindAll(".flare-listbox"));
        Assert.Equal(4, cut.FindAll(".flare-listbox__option").Count);
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Typing_filters_options()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        cut.Find("input").Focus();
        cut.Find("input").Input("lo");
        var options = cut.FindAll(".flare-listbox__option");
        Assert.Single(options);
        Assert.Contains("London", options[0].TextContent);
    }

    [Fact]
    public void Clicking_option_commits_value_and_closes()
    {
        string? bound = null;
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find("input").Focus();
        cut.FindAll(".flare-listbox__option")[0].Click();

        Assert.Equal("Berlin", bound);
        Assert.Empty(cut.FindAll(".flare-listbox"));   // closed on select
    }

    [Fact]
    public void ArrowDown_sets_active_descendant()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        cut.Find("input").Focus();
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        var active = cut.Find("input").GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(active));
        Assert.Contains("-opt-", active);
    }

    [Fact]
    public void GroupBy_renders_group_headers()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "Apple", "Avocado", "Banana" })
            .Add(x => x.GroupBy, (Func<string, string>)(s => s.StartsWith("A") ? "A" : "B")));
        cut.Find("input").Focus();
        Assert.NotEmpty(cut.FindAll(".flare-listbox__group-header"));
    }

    [Fact]
    public void Selected_option_has_aria_selected_not_nested_checkbox()
    {
        string? bound = "London";
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));
        cut.Find("input").Focus();
        var selected = cut.FindAll(".flare-listbox__option").Single(o => o.GetAttribute("aria-selected") == "true");
        Assert.Contains("London", selected.TextContent);
        // R8: selection is on the option, never a nested checkbox control inside it.
        Assert.Empty(selected.QuerySelectorAll("[role='checkbox']"));
    }

    [Fact]
    public void Custom_value_commits_on_enter_when_allowed()
    {
        string? bound = null;
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.AllowsCustomValue, true)
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find("input").Focus();
        cut.Find("input").Input("Reykjavik");     // not in the list
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal("Reykjavik", bound);
    }

    [Fact]
    public void No_custom_value_on_enter_when_disallowed()
    {
        string? bound = null;
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.AllowsCustomValue, false)
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find("input").Focus();
        cut.Find("input").Input("Reykjavik");
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Null(bound);
    }

    [Fact]
    public void Clear_button_resets_the_value()
    {
        string? bound = "Paris";
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find($".{Css.Classes.Autocomplete.Clear}").Click();
        Assert.Null(bound);
    }

    [Fact]
    public void Disabled_option_does_not_commit()
    {
        string? bound = null;
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, Cities)
            .Add(x => x.ItemDisabled, (Func<string, bool>)(s => s == "Berlin"))
            .Add(x => x.Value, bound)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => bound = v)));

        cut.Find("input").Focus();
        var berlin = cut.FindAll(".flare-listbox__option").First(o => o.TextContent.Contains("Berlin"));
        berlin.Click();
        Assert.Null(bound);   // disabled -> rejected
    }

    [Fact]
    public void Declarative_options_are_parsed()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .AddChildContent("<option value=\"s\">Small</option><option value=\"m\">Medium</option>"));
        cut.Find("input").Focus();
        Assert.Equal(2, cut.FindAll(".flare-listbox__option").Count);
    }
}
