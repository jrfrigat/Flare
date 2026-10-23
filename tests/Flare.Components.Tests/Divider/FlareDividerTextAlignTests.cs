namespace Flare.Components.Tests;

public class FlareDividerTextAlignTests : FlareTestContext
{
    [Fact]
    public void Text_OR_RendersTextSpanWithContent()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Text, "OR"));

        Assert.Contains("OR", cut.Find($"span.{Css.Classes.Divider.TextContent}").TextContent);
    }

    [Fact]
    public void Text_OR_AddsDividerTextClass()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Text, "OR"));

        Assert.Contains(Css.Classes.Divider.Text, cut.Find($"div.{Css.Classes.Divider.Text}").ClassName ?? "");
    }

    [Fact]
    public void TextAlign_Left_AddsTextLeftClass()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Text, "OR")
            .Add(x => x.TextAlign, DividerTextAlign.Left));

        Assert.Contains(Css.Classes.Divider.TextLeft, cut.Find("div").ClassName ?? "");
    }
}
