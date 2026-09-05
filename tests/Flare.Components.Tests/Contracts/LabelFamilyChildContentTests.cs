using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The same slot on the label family, where a caller most naturally writes markup rather than a string:
/// a link in a consent line, a unit, an inline badge.
/// </summary>
public class LabelFamilyChildContentTests : FlareTestContext
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
