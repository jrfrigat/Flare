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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Autocomplete.Root}"));
        var input = cut.Find($"input.{Css.Classes.Input.Control}");
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("false", input.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Dropdown_closed_initially()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        Assert.Empty(cut.FindAll($".{Css.Classes.Listbox.Root}"));
    }

    [Fact]
    public void Focus_opens_dropdown_with_all_options()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        cut.Find("input").Focus();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Listbox.Root}"));
        Assert.Equal(4, cut.FindAll($".{Css.Classes.Listbox.Option}").Count);
        Assert.Equal("true", cut.Find("input").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Typing_filters_options()
    {
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));
        cut.Find("input").Focus();
        cut.Find("input").Input("lo");
        var options = cut.FindAll($".{Css.Classes.Listbox.Option}");
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
        cut.FindAll($".{Css.Classes.Listbox.Option}")[0].Click();

        Assert.Equal("Berlin", bound);
        Assert.Empty(cut.FindAll($".{Css.Classes.Listbox.Root}"));   // closed on select
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
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Listbox.GroupHeader}"));
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
        var selected = cut.FindAll($".{Css.Classes.Listbox.Option}").Single(o => o.GetAttribute("aria-selected") == "true");
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
        var berlin = cut.FindAll($".{Css.Classes.Listbox.Option}").First(o => o.TextContent.Contains("Berlin"));
        berlin.Click();
        Assert.Null(bound);   // disabled -> rejected
    }

    [Fact]
    public void Declarative_options_are_parsed()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .AddChildContent("<option value=\"s\">Small</option><option value=\"m\">Medium</option>"));
        cut.Find("input").Focus();
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Listbox.Option}").Count);
    }

    [Fact]
    public void Chevron_opens_and_closes_the_list()
    {
        // The chevron was a decorative <span> with no handler, so the affordance that looks like the
        // way to open the list was the one thing that could not. Focusing the input still worked, which
        // is why it went unnoticed. It is a real button now, and it has to toggle - opening only would
        // leave a control that visibly points UP and does nothing.
        var cut = Render<FlareCombobox<string>>(p => p.Add(x => x.Items, Cities));

        var chevron = cut.Find($".{Css.Classes.Autocomplete.Icon}");
        Assert.Equal("BUTTON", chevron.TagName);
        Assert.Equal("false", chevron.GetAttribute("aria-expanded"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Listbox.Option}"));

        chevron.Click();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Listbox.Option}"));
        Assert.Equal("true", cut.Find($".{Css.Classes.Autocomplete.Icon}").GetAttribute("aria-expanded"));

        cut.Find($".{Css.Classes.Autocomplete.Icon}").Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.Listbox.Option}"));
        Assert.Equal("false", cut.Find($".{Css.Classes.Autocomplete.Icon}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Chevron_does_nothing_when_the_field_is_not_interactive()
    {
        foreach (var disabled in new[] { true, false })
        {
            var cut = Render<FlareCombobox<string>>(p => p
                .Add(x => x.Items, Cities)
                .Add(x => x.Disabled, disabled)
                .Add(x => x.ReadOnly, !disabled));

            var chevron = cut.Find($".{Css.Classes.Autocomplete.Icon}");
            Assert.True(chevron.HasAttribute("disabled"),
                $"the chevron must be inert when the field is {(disabled ? "disabled" : "read-only")}");
        }
    }
}
