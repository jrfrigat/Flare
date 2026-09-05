using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareLink Typo
// ------------------------------------------------------------------------------
public class FlareLinkTypoTests : FlareTestContext
{
    [Fact]
    public void Typo_AddsTypeScaleClass()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "#")
            .Add(x => x.Typo, TypographyScale.TitleMedium)
            .AddChildContent("Link"));

        Assert.Contains(Css.Classes.Text.TitleMedium, cut.Find("a").ClassName);
    }

    [Fact]
    public void NoTypo_NoTypeScaleClass()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "#")
            .AddChildContent("Link"));

        Assert.DoesNotContain("flare-text--", cut.Find("a").ClassName);
    }
}
