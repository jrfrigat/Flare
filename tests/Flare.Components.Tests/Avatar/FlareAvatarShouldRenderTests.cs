namespace Flare.Components.Tests;

// FlareAvatar ShouldRender (3 tests)
public class FlareAvatarShouldRenderTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareAvatar>();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Root}"));
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
}
