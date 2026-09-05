using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// Content written between a chip's tags has to reach the label slot.
/// </summary>
/// <remarks>
/// It did not before, and nothing said so: every Flare component inherits an
/// <c>AdditionalAttributes</c> catch-all, so Razor emits the child fragment as an untyped attribute
/// instead of failing to compile, it lands in the unmatched dictionary, and splatting drops it. The
/// chip rendered empty with a clean build. These pin the slot that closes it for this component; the
/// other twenty are tracked in docs/issues/implicit-child-content.md.
/// </remarks>
public class C_ChipChildContentTests : FlareTestContext
{
    [Fact]
    public void Content_between_the_tags_reaches_the_label()
    {
        var cut = Render<FlareChip>(p => p
            .AddChildContent("<b>42</b>"));

        var label = cut.Find($".{Css.Classes.Chip.Label}");
        Assert.Equal("42", label.TextContent);
        Assert.NotEmpty(label.QuerySelectorAll("b"));
    }

    [Fact]
    public void Child_content_wins_over_the_string_shorthand()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "shorthand")
            .AddChildContent("markup"));

        var label = cut.Find($".{Css.Classes.Chip.Label}").TextContent;
        Assert.Equal("markup", label);
        Assert.DoesNotContain("shorthand", label);
    }

    [Fact]
    public void The_string_shorthand_still_renders_on_its_own()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Science"));

        Assert.Equal("Science", cut.Find($".{Css.Classes.Chip.Label}").TextContent);
    }

    [Fact]
    public void An_empty_chip_renders_an_empty_label_rather_than_throwing()
    {
        var cut = Render<FlareChip>();

        Assert.Equal(string.Empty, cut.Find($".{Css.Classes.Chip.Label}").TextContent);
    }

    [Fact]
    public void A_component_child_renders_too()
    {
        // The reported shape: a component, not markup, written directly between the tags.
        var cut = Render<FlareChip>(p => p
            .AddChildContent<FlareBadge>(b => b.Add(x => x.Text, "9")));

        Assert.NotEmpty(cut.Find($".{Css.Classes.Chip.Label}").QuerySelectorAll($".{Css.Classes.Badge.Root}"));
    }
}

/// <summary>
/// The same slot on the label family, where a caller most naturally writes markup rather than a string:
/// a link in a consent line, a unit, an inline badge.
/// </summary>
public class C_LabelFamilyChildContentTests : FlareTestContext
{
    [Fact]
    public void Checkbox_renders_content_between_the_tags()
    {
        var cut = Render<FlareCheckbox>(p => p.AddChildContent("<a href=\"/terms\">terms</a>"));

        var label = cut.Find($".{Css.Classes.Checkbox.Label}");
        Assert.Equal("terms", label.TextContent);
        Assert.NotEmpty(label.QuerySelectorAll("a"));
    }

    [Fact]
    public void Switch_content_wins_over_the_string()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "shorthand")
            .AddChildContent("markup"));

        Assert.Equal("markup", cut.Find($".{Css.Classes.Switch.Label}").TextContent);
    }

    [Fact]
    public void Radio_still_takes_the_string_shorthand()
    {
        var cut = Render<FlareRadio<string>>(p => p.Add(x => x.Label, "Second"));

        Assert.Equal("Second", cut.Find($".{Css.Classes.Radio.Label}").TextContent);
    }

    [Fact]
    public void No_label_and_no_content_renders_no_label_element()
    {
        var cut = Render<FlareCheckbox>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Checkbox.Label}"));
    }
}

/// <summary>
/// The field family carries its label slot on <c>FlareFieldBase</c> and forwards it through the shared
/// chrome, so one parameter covers every field rather than thirteen copies of the same decision.
/// </summary>
public class C_FieldLabelContentTests : FlareTestContext
{
    [Fact]
    public void A_field_renders_markup_in_its_label()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.LabelContent, (RenderFragment)(b =>
            {
                b.AddMarkupContent(0, "Price, <abbr title=\"euro\">EUR</abbr>");
            })));

        var label = cut.Find($".{Css.Classes.Input.Label}");
        Assert.Contains("Price", label.TextContent);
        Assert.NotEmpty(label.QuerySelectorAll("abbr"));
    }

    [Fact]
    public void Label_content_wins_over_the_string()
    {
        var cut = Render<FlareField<string>>(p => p
            .Add(x => x.Label, "shorthand")
            .Add(x => x.LabelContent, (RenderFragment)(b => b.AddContent(0, "markup"))));

        Assert.Equal("markup", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void The_string_label_still_renders_alone()
    {
        var cut = Render<FlareField<string>>(p => p.Add(x => x.Label, "Email"));

        Assert.Equal("Email", cut.Find($".{Css.Classes.Input.Label}").TextContent);
    }

    [Fact]
    public void Neither_renders_no_label_element()
    {
        var cut = Render<FlareField<string>>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Input.Label}"));
    }
}
