using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareHiddenTests : FlareTestContext
{
    [Fact]
    public void NoBreakpoint_RendersChildContent()
    {
        var cut = Render<FlareHidden>(p => p
            .AddChildContent("<span class=\"kid\">visible</span>"));
        Assert.NotEmpty(cut.FindAll(".kid"));
    }

    [Fact]
    public void NoBreakpoint_DoesNotCarryUnconditionalBaseClass()
    {
        // With no breakpoint set the component is a plain pass-through wrapper:
        // it must never emit the bare `flare-hidden` utility (which would hide
        // its content in every viewport).
        var cut = Render<FlareHidden>(p => p
            .AddChildContent("<span class=\"kid\">visible</span>"));
        var div = cut.Find("div");
        Assert.DoesNotContain(Css.Classes.Hidden.Root, (div.GetAttribute("class") ?? string.Empty).Split(' '));
    }

    [Fact]
    public void Below_EmitsModifierClassWithoutUnconditionalBaseClass()
    {
        var cut = Render<FlareHidden>(p => p
            .Add(x => x.Below, Breakpoint.Sm)
            .AddChildContent("<span class=\"kid\">conditional</span>"));

        // Child content is always rendered; the breakpoint decides visibility via CSS.
        Assert.NotEmpty(cut.FindAll(".kid"));

        var classes = (cut.Find("div").GetAttribute("class") ?? string.Empty).Split(' ');
        Assert.Contains(Css.Classes.Hidden.BelowSm, classes);
        Assert.DoesNotContain(Css.Classes.Hidden.Root, classes);
    }
}
