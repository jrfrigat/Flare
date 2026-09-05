using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareDrawer.ContentPadding: the caller decides the content's horizontal inset, because the drawer cannot.
// A nav menu inside it carries its own item insets and wants its state layer to reach the panel edge; a form
// wants to sit off the edge and align with the header. Full-bleed stays the default so neither breaks.
public class FlareDrawerContentPaddingTests : FlareTestContext
{
    [Fact]
    public void DefaultsToFullBleed_NoClassNoInlinePadding()
    {
        var cut = Render<FlareDrawer>(p => p.Add(x => x.Open, true).Add(x => x.ChildContent, "<p>body</p>"));

        var root = cut.Find($".{Css.Classes.Drawer.Root}");
        Assert.DoesNotContain("content-pad", root.ClassName);
        Assert.DoesNotContain("--_content-pad-x", root.GetAttribute("style") ?? "");
    }

    [Theory]
    [InlineData(FlareSpacing.XXSmall, Css.Classes.Drawer.ContentPadXxSmall)]
    [InlineData(FlareSpacing.XSmall, Css.Classes.Drawer.ContentPadXSmall)]
    [InlineData(FlareSpacing.Small, Css.Classes.Drawer.ContentPadSmall)]
    [InlineData(FlareSpacing.Medium, Css.Classes.Drawer.ContentPadMedium)]
    [InlineData(FlareSpacing.Large, Css.Classes.Drawer.ContentPadLarge)]
    [InlineData(FlareSpacing.XLarge, Css.Classes.Drawer.ContentPadXLarge)]
    public void Step_AppliesTheMatchingClass(FlareSpacing step, string expected)
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ContentPadding, step)
            .Add(x => x.ChildContent, "<p>body</p>"));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    // The step must only SELECT a class - the value itself belongs to the theme's spacing scale, so no
    // geometry may travel through C#.
    [Fact]
    public void Step_CarriesNoGeometryInline()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ContentPadding, FlareSpacing.Large)
            .Add(x => x.ChildContent, "<p>body</p>"));

        Assert.DoesNotContain("--_content-pad-x", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void Custom_EmitsTheRawValue()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ContentPadding, FlareSpacing.Custom)
            .Add(x => x.ContentPaddingValue, "3.5rem")
            .Add(x => x.ChildContent, "<p>body</p>"));

        var root = cut.Find($".{Css.Classes.Drawer.Root}");
        Assert.Contains("--_content-pad-x:3.5rem", root.GetAttribute("style"));
        Assert.DoesNotContain("content-pad-", root.ClassName);   // Custom carries no step class
    }

    // Caller-supplied CSS goes through the same sanitiser as every other raw value.
    [Fact]
    public void Custom_SanitisesTheValue()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ContentPadding, FlareSpacing.Custom)
            .Add(x => x.ContentPaddingValue, "1rem;background:url(javascript:alert(1))")
            .Add(x => x.ChildContent, "<p>body</p>"));

        Assert.DoesNotContain("javascript:", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("style") ?? "");
    }
}
