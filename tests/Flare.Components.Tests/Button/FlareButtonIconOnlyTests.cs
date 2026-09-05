using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareButtonIconOnlyTests : FlareTestContext
{
    private static RenderFragment Icon => b => b.AddMarkupContent(0, "<i class=\"icon\"></i>");

    [Fact]
    public void IconWithoutText_AddsIconOnlyClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "default"));

        Assert.Contains(Css.Classes.Button.IconOnly, cut.Find("button").ClassName);
    }

    [Fact]
    public void IconWithText_DoesNotAddIconOnlyClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.LeadingIcon, Icon)
            .AddChildContent("Label"));

        Assert.DoesNotContain(Css.Classes.Button.IconOnly, cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersWithFilledVariant()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Filled)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "filled"));

        Assert.Contains(Css.Classes.Button.Filled, cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersWithTonalVariant()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Tonal)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "tonal"));

        Assert.Contains(Css.Classes.Button.Tonal, cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersWithOutlinedVariant()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Outlined)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "outlined"));

        Assert.Contains(Css.Classes.Button.Outlined, cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersSmallSize()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Size, ButtonSize.Sm)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "small"));

        Assert.Contains(Css.Classes.Button.Sm, cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersLargeSize()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "large"));

        Assert.Contains(Css.Classes.Button.Lg, cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "disabled"));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersAriaLabel()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "close dialog"));

        Assert.Equal("close dialog", cut.Find("button").GetAttribute("aria-label"));
    }
}

// ------------------------------------------------------------------------------
// FlareButtonGroup  (6 tests)
// ------------------------------------------------------------------------------
