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
        var cut = RenderComponent<FlareDivider>();

        Assert.Contains("flare-divider", cut.Find("hr").ClassName ?? "");
    }

    [Fact]
    public void Text_OR_RendersTextSpanWithContent()
    {
        var cut = RenderComponent<FlareDivider>(p => p
            .Add(x => x.Text, "OR"));

        Assert.Contains("OR", cut.Find("span.flare-divider__text").TextContent);
    }

    [Fact]
    public void Text_OR_AddsDividerTextClass()
    {
        var cut = RenderComponent<FlareDivider>(p => p
            .Add(x => x.Text, "OR"));

        Assert.Contains("flare-divider--text", cut.Find("div.flare-divider--text").ClassName ?? "");
    }

    [Fact]
    public void TextAlign_Left_AddsTextLeftClass()
    {
        var cut = RenderComponent<FlareDivider>(p => p
            .Add(x => x.Text, "OR")
            .Add(x => x.TextAlign, DividerTextAlign.Left));

        Assert.Contains("flare-divider--text-left", cut.Find("div").ClassName ?? "");
    }

    [Fact]
    public void Vertical_True_RendersVerticalVariant()
    {
        var cut = RenderComponent<FlareDivider>(p => p
            .Add(x => x.Vertical, true));

        Assert.Contains("flare-divider--vertical", cut.Find("div").ClassName ?? "");
    }
}
