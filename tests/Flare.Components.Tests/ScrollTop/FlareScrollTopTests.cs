using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareScrollTopTests : FlareTestContext
{
    [Fact]
    public void RendersButtonElement()
    {
        var cut = Render<FlareScrollTop>();

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Scroll.Top}"));
    }

    [Fact]
    public void ButtonHasAriaLabel()
    {
        var cut = Render<FlareScrollTop>();

        Assert.Equal("Back to top", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public void ButtonHasTypeButton()
    {
        var cut = Render<FlareScrollTop>();

        Assert.Equal("button", cut.Find("button").GetAttribute("type"));
    }

    [Fact]
    public void NotVisibleByDefault_NoVisibleClass()
    {
        var cut = Render<FlareScrollTop>();

        Assert.DoesNotContain(Css.Classes.Scroll.TopVisible, cut.Find("button").ClassName ?? "");
    }

    // Hidden is opacity and pointer-events only, so without inert the button is still a Tab stop and
    // still announced while nobody can see it.
    [Fact]
    public void HiddenButton_IsInertAndOutOfTheAccessibilityTree()
    {
        var cut = Render<FlareScrollTop>();

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("inert"));
        Assert.Equal("true", button.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void RendersDefaultArrowIcon()
    {
        var cut = Render<FlareScrollTop>();

        // The default arrow is now the built-in SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll("svg path"));
    }

    [Fact]
    public void ThresholdParam_AcceptsCustomValue()
    {
        var cut = Render<FlareScrollTop>(p => p
            .Add(x => x.Threshold, 500));

        Assert.Equal(500, cut.Instance.Threshold);
    }
}
