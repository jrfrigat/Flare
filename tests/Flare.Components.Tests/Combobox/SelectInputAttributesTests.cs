using AngleSharp.Dom;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-504: InputAttributes belong to the control the user actually operates. A searchable select forwards
/// them to its text input, a select-only one keeps them on the combobox trigger, the wrapper stays clean,
/// and the component's own combobox role and listbox wiring are not overwritten.
/// </summary>
public class SelectInputAttributesTests : FlareTestContext
{
    private static Dictionary<string, object> Attributes()
    {
        return new()
        {
            ["aria-label"] = "Custom label",
            ["name"] = "plan",
            ["data-testid"] = "plan-picker",
            ["tabindex"] = "3",
        };
    }

    private static void AssertForwarded(IElement element)
    {
        Assert.Equal("Custom label", element.GetAttribute("aria-label"));
        Assert.Equal("plan", element.GetAttribute("name"));
        Assert.Equal("plan-picker", element.GetAttribute("data-testid"));
        Assert.Equal("3", element.GetAttribute("tabindex"));
    }

    [Fact]
    public void SearchableSelect_ForwardsInputAttributesToTheTextInput()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.InputAttributes, Attributes()));

        AssertForwarded(cut.Find($".{Css.Classes.Select.Search}"));
        var control = cut.Find($".{Css.Classes.Select.Control}");
        Assert.False(control.HasAttribute("name"));
        Assert.False(control.HasAttribute("data-testid"));
        Assert.False(control.HasAttribute("aria-label"));
        Assert.NotEqual("3", control.GetAttribute("tabindex"));   // the trigger keeps its own -1
    }

    [Fact]
    public void SelectOnly_KeepsInputAttributesOnTheComboboxTrigger()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.InputAttributes, Attributes()));

        AssertForwarded(cut.Find($".{Css.Classes.Select.Control}"));
        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void SearchableMultiSelect_ForwardsInputAttributesToTheTextInput()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.InputAttributes, Attributes()));

        AssertForwarded(cut.Find($".{Css.Classes.Multiselect.Control} input"));
        var control = cut.Find($".{Css.Classes.Multiselect.Control}");
        Assert.False(control.HasAttribute("name"));
        Assert.False(control.HasAttribute("data-testid"));
        Assert.False(control.HasAttribute("aria-label"));
        Assert.NotEqual("3", control.GetAttribute("tabindex"));   // the trigger keeps its own -1
    }

    [Fact]
    public void SelectOnlyMultiSelect_KeepsInputAttributesOnTheComboboxTrigger()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.InputAttributes, Attributes()));

        AssertForwarded(cut.Find($".{Css.Classes.Multiselect.Control}"));
        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void SelectOnly_KeepsItsComboboxRoleAndListboxWiring()
    {
        var attributes = Attributes();
        attributes["role"] = "button";
        attributes["aria-haspopup"] = "dialog";
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.InputAttributes, attributes));

        var control = cut.Find($".{Css.Classes.Select.Control}");
        Assert.Equal("combobox", control.GetAttribute("role"));
        Assert.Equal("listbox", control.GetAttribute("aria-haspopup"));
    }

    [Fact]
    public void SelectOnlyMultiSelect_KeepsItsComboboxRoleAndListboxWiring()
    {
        var attributes = Attributes();
        attributes["role"] = "button";
        attributes["aria-haspopup"] = "dialog";
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.InputAttributes, attributes));

        var control = cut.Find($".{Css.Classes.Multiselect.Control}");
        Assert.Equal("combobox", control.GetAttribute("role"));
        Assert.Equal("listbox", control.GetAttribute("aria-haspopup"));
    }

    [Fact]
    public void SearchableSelect_KeepsItsComboboxRoleAndListboxWiring()
    {
        var attributes = Attributes();
        attributes["role"] = "button";
        attributes["aria-haspopup"] = "dialog";
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.InputAttributes, attributes));

        var input = cut.Find($".{Css.Classes.Select.Search}");
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("listbox", input.GetAttribute("aria-haspopup"));
    }

    [Fact]
    public void SearchableMultiSelect_KeepsItsComboboxRoleAndListboxWiring()
    {
        var attributes = Attributes();
        attributes["role"] = "button";
        attributes["aria-haspopup"] = "dialog";
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Searchable, true)
            .Add(x => x.InputAttributes, attributes));

        var input = cut.Find($".{Css.Classes.Multiselect.Control} input");
        Assert.Equal("combobox", input.GetAttribute("role"));
        Assert.Equal("listbox", input.GetAttribute("aria-haspopup"));
    }
}
