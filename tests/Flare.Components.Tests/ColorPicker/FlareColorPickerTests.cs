using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareColorPickerTests : FlareTestContext
{
    [Fact]
    public void RendersRootAndTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p.Add(x => x.Value, "#ff0000"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Colorpicker.Root}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Colorpicker.Trigger}"));
    }

    [Fact]
    public void Inline_RendersInlinePanel()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "#00ff00")
            .Add(x => x.Inline, true));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Colorpicker.Root}"));
    }

    [Fact]
    public void Label_RendersAboveTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "#ff0000")
            .Add(x => x.Label, "Brand color"));
        var label = cut.Find($"label.{Css.Classes.Colorpicker.Label}");
        Assert.Equal("Brand color", label.TextContent);
        // for/id wiring points the label at the trigger button
        var trigger = cut.Find($".{Css.Classes.Colorpicker.Trigger}");
        Assert.Equal(trigger.Id, label.GetAttribute("for"));
    }

    [Fact]
    public void HexValue_DefaultFormat_ShowsHexOnTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p.Add(x => x.Value, "#1188ee"));
        Assert.Equal("#1188EE", cut.Find($".{Css.Classes.Colorpicker.TriggerLabel}").TextContent);
    }

    [Fact]
    public void RgbaValue_WithRgbFormat_RoundTripsOnTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "rgba(255, 0, 0, 0.5)")
            .Add(x => x.Format, ColorFormat.Rgb));
        Assert.Equal("rgba(255, 0, 0, 0.5)", cut.Find($".{Css.Classes.Colorpicker.TriggerLabel}").TextContent);
    }

    [Fact]
    public void RgbValue_ParsesAndShowsRgb()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "rgb(0, 128, 255)")
            .Add(x => x.Format, ColorFormat.Rgb));
        Assert.Equal("rgb(0, 128, 255)", cut.Find($".{Css.Classes.Colorpicker.TriggerLabel}").TextContent);
    }

    [Fact]
    public void RgbaValue_DefaultFormat_ShowsHex8OnTrigger()
    {
        // Parsed as a color, emitted as hex (the default format) -- alpha 0.5 -> 80.
        var cut = Render<FlareColorPicker>(p => p.Add(x => x.Value, "rgba(255, 0, 0, 0.5)"));
        Assert.Equal("#FF000080", cut.Find($".{Css.Classes.Colorpicker.TriggerLabel}").TextContent);
    }

    [Fact]
    public void HslValue_WithHslFormat_RoundTripsOnTrigger()
    {
        var cut = Render<FlareColorPicker>(p => p
            .Add(x => x.Value, "hsl(210, 100%, 50%)")
            .Add(x => x.Format, ColorFormat.Hsl));
        Assert.Equal("hsl(210, 100%, 50%)", cut.Find($".{Css.Classes.Colorpicker.TriggerLabel}").TextContent);
    }
}
