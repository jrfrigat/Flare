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

    [Fact]
    public void DefaultMaxIsFive()
    {
        var cut = Render<FlareAvatarGroup>(p => p
            .Add(x => x.Max, 5));

        Assert.Empty(cut.FindAll($".{Css.Classes.Avatar.GroupOverflow}"));
    }
}

// ------------------------------------------------------------------------------
// FlareChip single  (5 tests from Wave3)
// ------------------------------------------------------------------------------
