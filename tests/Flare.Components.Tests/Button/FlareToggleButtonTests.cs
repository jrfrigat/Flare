using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareToggleButtonTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareToggleButton>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Button.Root}"));
    }

    [Fact]
    public void UnpressedState_AriaPressedFalse()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Toggled, false));

        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void PressedState_AriaPressedTrue()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Toggled, true));

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void DisabledState_ButtonHasDisabledAttribute()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareToggleButton>(p => p
            .AddChildContent("Bookmark"));

        Assert.Contains("Bookmark", cut.Find($".{Css.Classes.Button.Label}").TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareButton Loading  (8 tests from Wave7)
// ------------------------------------------------------------------------------
