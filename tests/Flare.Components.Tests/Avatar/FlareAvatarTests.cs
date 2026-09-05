namespace Flare.Components.Tests;

public class FlareAvatarTests : FlareTestContext
{

    [Fact]
    public void RendersInitialsWhenTextProvided()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Text, "John Doe"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Initials}"));
    }

    [Fact]
    public void RendersImgWhenSrcProvided()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Src, "https://example.com/avatar.png"));

        Assert.NotEmpty(cut.FindAll($"img.{Css.Classes.Avatar.Img}"));
    }

    [Fact]
    public void SizeSmall_HasSmallClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Size, AvatarSize.Sm));

        Assert.Contains(Css.Classes.Avatar.Sm, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }

    [Fact]
    public void SizeLarge_HasLargeClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Size, AvatarSize.Lg));

        Assert.Contains(Css.Classes.Avatar.Lg, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }

    [Fact]
    public void ShapeSquare_HasSquareClass()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.Shape, AvatarShape.Square));

        Assert.Contains(Css.Classes.Avatar.Square, cut.Find($".{Css.Classes.Avatar.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareAvatarGroup  (4 tests from Wave3)
// ------------------------------------------------------------------------------
