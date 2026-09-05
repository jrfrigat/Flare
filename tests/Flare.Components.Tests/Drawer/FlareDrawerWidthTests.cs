using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// The drawer's resting width belongs to the theme. It did not: the component wrote width:280px inline on
// every render, and an inline style beats the stylesheet - so --flare-drawer-width (360px, and spec-exact)
// could never render, and a theme setting its own width was ignored. Only a caller-supplied Width may go
// inline now.
public class FlareDrawerWidthTests : FlareTestContext
{
    [Fact]
    public void NoWidth_WritesNoInlineWidth_SoTheThemeTokenDecides()
    {
        var cut = Render<FlareDrawer>(p => p.Add(x => x.Open, true).Add(x => x.ChildContent, "<p>x</p>"));

        var style = cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("style") ?? "";
        Assert.DoesNotContain("width:", style);
    }

    [Fact]
    public void CallerWidth_StillGoesInline()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Width, "22rem")
            .Add(x => x.ChildContent, "<p>x</p>"));

        Assert.Contains("width:22rem", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("style"));
    }

    // Top/bottom drawers size on the other axis; that path is unchanged.
    [Fact]
    public void TopAnchor_StillWritesHeight()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Anchor, DrawerAnchor.Top)
            .Add(x => x.ChildContent, "<p>x</p>"));

        Assert.Contains("height:", cut.Find($".{Css.Classes.Drawer.Root}").GetAttribute("style"));
    }
}
