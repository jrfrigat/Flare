// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareDivider text/TextAlign tests  (5 tests)
// -----------------------------------------------------------------------------

public class FlareDividerTextAlignTests : FlareTestContext
{
    [Fact]
    public void Default_RendersFlareDividerElement()
    {
        var cut = Render<FlareDivider>();

        Assert.Contains(Css.Classes.Divider.Root, cut.Find("hr").ClassName ?? "");
    }

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

    [Fact]
    public void Vertical_True_RendersVerticalVariant()
    {
        var cut = Render<FlareDivider>(p => p
            .Add(x => x.Vertical, true));

        Assert.Contains(Css.Classes.Divider.Vertical, cut.Find("div").ClassName ?? "");
    }
}
