using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Flare.Components.Tests;

public class FlareRadioTests : FlareTestContext
{
    [Fact]
    public void RendersRootLabel()
    {
        var cut = Render<FlareRadio<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Radio.Root}"));
    }

    [Fact]
    public void RendersInputTypeRadio()
    {
        var cut = Render<FlareRadio<string>>();

        var input = cut.Find("input");
        Assert.Equal("radio", input.GetAttribute("type"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Label, "Option A"));

        Assert.Contains("Option A", cut.Find($".{Css.Classes.Radio.Label}").TextContent);
    }

    [Fact]
    public void RendersDisabledWhenParameterSet()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
    }

    [Fact]
    public void NotCheckedByDefault()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Value, "a"));

        Assert.False(cut.Find("input").HasAttribute("checked"));
    }

    [Fact]
    public void DisabledClassAppliedWhenDisabled()
    {
        var cut = Render<FlareRadio<string>>(p => p
            .Add(x => x.Disabled, true));

        Assert.Contains(Css.Classes.Radio.Disabled, cut.Find("label").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareFormField  (9 tests from Wave10)
// ------------------------------------------------------------------------------
