// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// FlareDivider ShouldRender (7 tests)
public class FlareDividerShouldRenderTests : FlareTestContext
{
    [Fact]
    public void RendersHrByDefault()
    {
        var cut = Render<FlareDivider>();
        Assert.NotEmpty(cut.FindAll("hr.flare-divider"));
    }

    [Fact]
    public void VerticalRendersDivNotHr()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Vertical, true));
        Assert.Empty(cut.FindAll("hr"));
        Assert.NotEmpty(cut.FindAll("div.flare-divider--vertical"));
    }

    [Fact]
    public void TextRendersTextVariant()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "OR"));
        Assert.NotEmpty(cut.FindAll(".flare-divider--text"));
        Assert.Contains("OR", cut.Markup);
    }

    [Fact]
    public void TextAlignLeftAppliesClass()
    {
        var cut = Render<FlareDivider>(p =>
        {
            p.Add(x => x.Text, "Start");
            p.Add(x => x.TextAlign, DividerTextAlign.Left);
        });
        Assert.Contains("flare-divider--text-left", cut.Find(".flare-divider--text").ClassName);
    }

    [Fact]
    public void TextAlignRightAppliesClass()
    {
        var cut = Render<FlareDivider>(p =>
        {
            p.Add(x => x.Text, "End");
            p.Add(x => x.TextAlign, DividerTextAlign.Right);
        });
        Assert.Contains("flare-divider--text-right", cut.Find(".flare-divider--text").ClassName);
    }

    [Fact]
    public void TextAlignCenterIsDefault()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "Center"));
        Assert.Contains("flare-divider--text-center", cut.Find(".flare-divider--text").ClassName);
    }

    [Fact]
    public void ShouldRenderSkipsRedrawWhenUnchanged()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "OR"));
        var html1 = cut.Markup;
        cut.Render(p => p.Add(x => x.Text, "OR"));
        Assert.Equal(html1, cut.Markup);
    }
}
