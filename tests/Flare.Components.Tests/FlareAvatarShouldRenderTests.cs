// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// FlareAvatar ShouldRender (6 tests)
public class FlareAvatarShouldRenderTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = RenderComponent<FlareAvatar>();
        Assert.NotEmpty(cut.FindAll(".flare-avatar"));
    }

    [Fact]
    public void DefaultShowsPersonIcon()
    {
        var cut = RenderComponent<FlareAvatar>();
        Assert.Contains("person", cut.Markup);
    }

    [Fact]
    public void TextShowsInitials()
    {
        var cut = RenderComponent<FlareAvatar>(p => p.Add(x => x.Text, "John Doe"));
        Assert.Contains("JD", cut.Markup);
    }

    [Fact]
    public void SrcRendersImg()
    {
        var cut = RenderComponent<FlareAvatar>(p => p.Add(x => x.Src, "/img/avatar.png"));
        Assert.NotEmpty(cut.FindAll("img.flare-avatar__img"));
    }

    [Fact]
    public void SmallSizeAppliesClass()
    {
        var cut = RenderComponent<FlareAvatar>(p => p.Add(x => x.Size, AvatarSize.Sm));
        Assert.Contains("flare-avatar--sm", cut.Find(".flare-avatar").ClassName);
    }

    [Fact]
    public void SquareShapeAppliesClass()
    {
        var cut = RenderComponent<FlareAvatar>(p => p.Add(x => x.Shape, AvatarShape.Square));
        Assert.Contains("flare-avatar--square", cut.Find(".flare-avatar").ClassName);
    }
}
