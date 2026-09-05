// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// FlareAvatar ShouldRender (6 tests)
public class FlareAvatarShouldRenderTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareAvatar>();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Root}"));
    }

    [Fact]
    public void DefaultShowsPersonIcon()
    {
        var cut = Render<FlareAvatar>();
        // Default fallback is the built-in person SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Icon} path"));
    }

    [Fact]
    public void TextShowsInitials()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Text, "John Doe"));
        Assert.Contains("JD", cut.Markup);
    }

    [Fact]
    public void SrcRendersImg()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Src, "/img/avatar.png"));
        Assert.NotEmpty(cut.FindAll($"img.{Css.Classes.Avatar.Img}"));
    }

    [Fact]
    public void SmallSizeAppliesClass()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Size, AvatarSize.Sm));
        Assert.Contains(Css.Classes.Avatar.Sm, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }

    [Fact]
    public void SquareShapeAppliesClass()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Shape, AvatarShape.Square));
        Assert.Contains(Css.Classes.Avatar.Square, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }
}
