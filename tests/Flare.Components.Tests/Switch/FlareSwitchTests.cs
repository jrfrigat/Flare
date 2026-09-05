using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests;

public class FlareSwitchTests : FlareTestContext
{
    [Fact]
    public void RendersRootLabel()
    {
        var cut = Render<FlareSwitch>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Switch.Root}"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Label, "Enable feature"));

        Assert.Contains("Enable feature", cut.Find($".{Css.Classes.Switch.Label}").TextContent);
    }

    [Fact]
    public void HasCorrectInputType()
    {
        var cut = Render<FlareSwitch>();

        var input = cut.Find("input");
        Assert.Equal("checkbox", input.GetAttribute("type"));
    }

    [Fact]
    public void RendersCheckedWhenValueTrue()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Value, true));

        Assert.True(cut.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void RendersUncheckedWhenValueFalse()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Value, false));

        Assert.False(cut.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersErrorText()
    {
        var cut = Render<FlareSwitch>(p => p
            .Add(x => x.ErrorText, "Must be enabled"));

        Assert.Contains("Must be enabled", cut.Find($".{Css.Classes.Switch.Error}").TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareRadio  (6 tests from Wave4)
// ------------------------------------------------------------------------------
