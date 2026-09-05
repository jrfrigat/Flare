using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTextAnchorTests : FlareTestContext
{
    [Fact]
    public void AnchorId_SetsIdAndAnchorLink()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.HeadlineSmall)
            .Add(x => x.AnchorId, "getting-started")
            .AddChildContent("Getting started"));

        var heading = cut.Find("h4");
        Assert.Equal("getting-started", heading.GetAttribute("id"));

        var anchor = cut.Find($"a.{Css.Classes.Text.Anchor}");
        Assert.EndsWith("#getting-started", anchor.GetAttribute("href"));
        Assert.Equal("true", anchor.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void NoAnchorId_RendersNoIdOrAnchor()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.HeadlineSmall)
            .AddChildContent("Plain"));

        Assert.False(cut.Find("h4").HasAttribute("id"));
        Assert.Empty(cut.FindAll($"a.{Css.Classes.Text.Anchor}"));
    }
}
