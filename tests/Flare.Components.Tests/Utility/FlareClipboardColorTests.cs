using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareClipboard forwards Color to the inner button (emphasized copy control)
// ------------------------------------------------------------------------------
public class FlareClipboardColorTests : FlareTestContext
{
    [Fact]
    public void Color_IsForwardedToInnerButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "secret")
            .Add(x => x.Color, FlareColor.Primary));

        Assert.Contains(Css.Classes.Color.Primary, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void DefaultColor_AddsNoColorClass()
    {
        var cut = Render<FlareClipboard>(p => p.Add(x => x.Text, "secret"));

        Assert.DoesNotContain("flare-color-", cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }
}
