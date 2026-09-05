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
        Assert.NotEmpty(cut.FindAll($"hr.{Css.Classes.Divider.Root}"));
    }

    [Fact]
    public void VerticalRendersDivNotHr()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Vertical, true));
        Assert.Empty(cut.FindAll("hr"));
        Assert.NotEmpty(cut.FindAll($"div.{Css.Classes.Divider.Vertical}"));
    }

    [Fact]
    public void TextRendersTextVariant()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "OR"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Divider.Text}"));
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
        Assert.Contains(Css.Classes.Divider.TextLeft, cut.Find($".{Css.Classes.Divider.Text}").ClassName);
    }

    [Fact]
    public void TextAlignRightAppliesClass()
    {
        var cut = Render<FlareDivider>(p =>
        {
            p.Add(x => x.Text, "End");
            p.Add(x => x.TextAlign, DividerTextAlign.Right);
        });
        Assert.Contains(Css.Classes.Divider.TextRight, cut.Find($".{Css.Classes.Divider.Text}").ClassName);
    }

    [Fact]
    public void TextAlignCenterIsDefault()
    {
        var cut = Render<FlareDivider>(p => p.Add(x => x.Text, "Center"));
        Assert.Contains(Css.Classes.Divider.TextCenter, cut.Find($".{Css.Classes.Divider.Text}").ClassName);
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
