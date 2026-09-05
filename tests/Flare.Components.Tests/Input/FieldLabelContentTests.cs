using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The field family carries its label slot on <c>FlareFieldBase</c> and forwards it through the shared
/// chrome, so one parameter covers every field rather than thirteen copies of the same decision.
/// </summary>
public class FieldLabelContentTests : FlareTestContext
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
