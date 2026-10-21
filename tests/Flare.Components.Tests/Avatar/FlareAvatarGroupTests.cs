namespace Flare.Components.Tests;

public class FlareAvatarGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareAvatarGroup>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Group}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .AddChildContent("<div class=\"child-avatar\">A</div>"));

        Assert.NotEmpty(cut.FindAll(".child-avatar"));
    }

    [Fact]
    public void SpacingAppliedAsStyle()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .Add(x => x.Spacing, "-1rem"));

        var style = cut.Find($".{Css.Classes.Avatar.Group}").GetAttribute("style") ?? string.Empty;
        Assert.Contains("-1rem", style);
    }

    // The theme's GroupSpacing token is what the stylesheet reads; writing the old default inline kept
    // every theme from changing the overlap.
    [Fact]
    public void WithoutSpacing_TheThemeValueIsNotOverridden()
    {
        var cut = Render<FlareAvatarGroup>();

        var style = cut.Find($".{Css.Classes.Avatar.Group}").GetAttribute("style") ?? string.Empty;
        Assert.DoesNotContain(Css.Tokens.AvatarField.GroupSpacing, style);
    }

    [Fact]
    public void DefaultMaxIsFive()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .Add(x => x.Max, 5));

        Assert.Empty(cut.FindAll($".{Css.Classes.Avatar.GroupOverflow}"));
    }
}
