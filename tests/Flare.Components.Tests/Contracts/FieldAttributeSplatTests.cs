using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// Where an attribute written on a field actually lands. The rule is one rule for the whole family: an
/// unmatched attribute splats onto the field's ROOT, like every other Flare component, and
/// <c>InputAttributes</c> splats onto the control the user types into. Before this was settled, four
/// components put the unmatched splat on the control, seven on the root, and one on both - so a
/// <c>data-testid</c> landed somewhere different depending on which field it was written on.
/// </summary>
public class FieldAttributeSplatTests : FlareTestContext
{
    private static readonly Dictionary<string, object> Inner = new() { ["data-inner"] = "yes" };

    private static void AssertSplit(IRenderedComponent<IComponent> cut, string controlSelector)
    {
        var root = cut.Find($".{Css.Classes.Input.Root}");
        Assert.Equal("outer", root.GetAttribute("data-outer"));
        Assert.False(root.HasAttribute("data-inner"));

        var control = cut.Find(controlSelector);
        Assert.Equal("yes", control.GetAttribute("data-inner"));
        Assert.False(control.HasAttribute("data-outer"));
    }

    [Fact]
    public void FlareField_splits_root_and_input()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
    }

    [Fact]
    public void FlareTextArea_splits_root_and_textarea()
    {
        var cut = Render<FlareTextArea>(p => p
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "textarea");
    }

    [Fact]
    public void FlareNumericField_splits_root_and_input()
    {
        var cut = Render<FlareNumericField<int>>(p => p
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
    }

    [Fact]
    public void FlareMaskedField_puts_the_unmatched_splat_on_the_root_only()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Mask, "000-000")
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
        // It used to splat onto both, so one attribute appeared twice in the DOM.
        Assert.Single(cut.FindAll("[data-outer]"));
    }

    [Fact]
    public void FlareDatePicker_splits_root_and_input()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
    }

    [Fact]
    public void FlareTimePicker_splits_root_and_input()
    {
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
    }

    [Fact]
    public void FlareSelect_splits_root_and_combobox_control()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b" })
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, $".{Css.Classes.Select.Control}");
    }

    [Fact]
    public void FlareMultiSelect_splits_root_and_combobox_control()
    {
        var cut = Render<FlareMultiSelect<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b" })
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, $".{Css.Classes.Multiselect.Control}");
    }

    [Fact]
    public void FlareCombobox_splits_root_and_input()
    {
        var cut = Render<FlareCombobox<string>>(p => p
            .Add(x => x.Items, new[] { "a", "b" })
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
    }

    [Fact]
    public void FlareTagField_splits_root_and_input()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.InputAttributes, Inner)
            .AddUnmatched("data-outer", "outer"));

        AssertSplit(cut, "input");
    }

    [Fact]
    public void The_select_arrow_is_out_of_the_accessibility_tree()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.Items, new[] { "a", "b" }));

        var arrow = cut.Find($".{Css.Classes.Input.Arrow}");
        Assert.Equal("true", arrow.GetAttribute("aria-hidden"));
        Assert.Equal("-1", arrow.GetAttribute("tabindex"));
        // A named button inside role="combobox" is what the review flagged; the name is gone with it.
        Assert.False(arrow.HasAttribute("aria-label"));
    }
}
