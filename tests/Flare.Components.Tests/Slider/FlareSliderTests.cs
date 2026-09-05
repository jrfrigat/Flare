using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSliderTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareSlider>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.Root}"));
    }

    [Fact]
    public void RendersRangeInput()
    {
        var cut = Render<FlareSlider>();

        var input = cut.Find("input[type='range']");
        Assert.NotNull(input);
    }

    [Fact]
    public void DisabledState_InputHasDisabledAttribute()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input[type='range']").HasAttribute("disabled"));
    }

    [Fact]
    public void MinMaxAttributes_AppliedToInput()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 10.0)
            .Add(x => x.Max, 200.0));

        var input = cut.Find("input[type='range']");
        Assert.Equal("10", input.GetAttribute("min"));
        Assert.Equal("200", input.GetAttribute("max"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Label, "Volume"));

        Assert.Contains("Volume", cut.Find($".{Css.Classes.Slider.Label}").TextContent);
    }

    [Theory]
    [InlineData(TrackSize.Xs, Css.Classes.Slider.Xs)]
    [InlineData(TrackSize.Sm, Css.Classes.Slider.Sm)]
    [InlineData(TrackSize.Md, Css.Classes.Slider.Md)]
    [InlineData(TrackSize.Lg, Css.Classes.Slider.Lg)]
    [InlineData(TrackSize.Xl, Css.Classes.Slider.Xl)]
    public void Size_AppliesSizeClass(TrackSize size, string expected)
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Size, size));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Slider.Root}").ClassName);
    }

    [Fact]
    public void StartAndEndIcons_Rendered()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.StartIcon, FlareIcons.VolumeOff)
            .Add(x => x.EndIcon, FlareIcons.VolumeUp));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.IconStart}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.IconEnd}"));
    }

    [Fact]
    public void Stepper_RendersStopIndicators()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 4.0).Add(x => x.Step, 1.0)
            .Add(x => x.Stepper, true));

        // 0,1,2,3,4 -> 5 stops
        Assert.Equal(5, cut.FindAll($".{Css.Classes.Slider.Tick}").Count);
    }

    [Fact]
    public void Stepper_ActiveStops_MarkedBelowValue()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 4.0).Add(x => x.Step, 1.0)
            .Add(x => x.Value, 2.0).Add(x => x.Stepper, true));

        // values 0,1,2 are <= 2 -> 3 active ticks
        Assert.Equal(3, cut.FindAll($".{Css.Classes.Slider.TickActive}").Count);
    }

    [Fact]
    public void Indicator_RendersValueBubble()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Indicator, true).Add(x => x.Value, 42.0));

        Assert.Contains("42", cut.Find($".{Css.Classes.Slider.Bubble}").TextContent);
    }

    [Fact]
    public void Stepper_HugeStepCount_SkipsRendering()
    {
        // range/step = 100000 > MaxStops cap -> no ticks rendered
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100000.0).Add(x => x.Step, 1.0)
            .Add(x => x.Stepper, true));

        Assert.Empty(cut.FindAll($".{Css.Classes.Slider.Tick}"));
    }

    [Fact]
    public void RendersRailSegments_WithActiveFill()
    {
        // Init=Min (default), Value mid -> inactive|active|inactive split, >=1 active
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Value, 50.0));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.Seg}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.SegActive}"));
    }

    [Fact]
    public void Vertical_AppliesClassAndAriaOrientation()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Vertical, true));

        Assert.Contains(Css.Classes.Slider.Vertical, cut.Find($".{Css.Classes.Slider.Root}").ClassName);
        Assert.Equal("vertical", cut.Find("input[type='range']").GetAttribute("aria-orientation"));
    }

    [Fact]
    public void Horizontal_NoAriaOrientation()
    {
        var cut = Render<FlareSlider>();

        Assert.False(cut.Find("input[type='range']").HasAttribute("aria-orientation"));
    }

    [Fact]
    public void Color_Role_AppliesColorClass()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Color, FlareColor.Secondary));

        Assert.Contains(Css.Classes.Color.Secondary, cut.Find($".{Css.Classes.Slider.Root}").ClassName);
    }

    [Fact]
    public void Color_Custom_InlinesAccentToken()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Color, FlareColor.Custom("#E91E63")));

        Assert.Contains($"{Css.Tokens.LocalColor.Main}:#E91E63", cut.Find($".{Css.Classes.Slider.Root}").GetAttribute("style"));
    }

    [Fact]
    public void Range_RendersTwoInputsAndRangeClass()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 80.0));

        Assert.Contains(Css.Classes.Slider.Range, cut.Find($".{Css.Classes.Slider.Root}").ClassName);
        Assert.Equal(2, cut.FindAll("input[type='range']").Count);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.InputLow}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Slider.InputHigh}"));
    }

    [Fact]
    public void HandleOnHover_MarksTheRootAndKeepsTheControlReachable()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.HandleOnHover, true)
            .Add(x => x.Value, 40.0));

        Assert.Contains(Css.Classes.Slider.HandleOnHover, cut.Find($".{Css.Classes.Slider.Root}").ClassName);

        // Only the handle's paint is hidden. The control must keep everything that makes it
        // operable, or a seek bar becomes unreachable by keyboard and silent to a screen reader.
        var input = cut.Find("input[type='range']");
        Assert.Null(input.GetAttribute("disabled"));
        Assert.Null(input.GetAttribute("tabindex"));   // still in the natural tab order
        // A native range reports its value from the attribute itself, not from aria-valuenow.
        Assert.Equal("40", input.GetAttribute("value"));
    }

    [Fact]
    public void NoHandleOnHover_ByDefault()
    {
        var cut = Render<FlareSlider>(p => p.Add(x => x.Value, 40.0));

        Assert.DoesNotContain("handle-on-hover", cut.Find($".{Css.Classes.Slider.Root}").ClassName);
    }

    [Fact]
    public void Range_SetsBothPctVars()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 80.0));

        var style = cut.Find($".{Css.Classes.Slider.TrackWrap}").GetAttribute("style")!;
        Assert.Contains("--_pct:20.00%", style);
        Assert.Contains("--_pct-end:80.00%", style);
    }

    [Fact]
    public void Range_InvertedValues_OrderedForDisplay()
    {
        // Value > ValueEnd -> the rendered low/high pct never invert
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0)
            .Add(x => x.Value, 80.0).Add(x => x.ValueEnd, 20.0));

        var style = cut.Find($".{Css.Classes.Slider.TrackWrap}").GetAttribute("style")!;
        Assert.Contains("--_pct:20.00%", style);
        Assert.Contains("--_pct-end:80.00%", style);
    }

    [Fact]
    public void Range_ShowValue_RendersLowHigh()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Range, true).Add(x => x.Label, "Band")
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 80.0));

        Assert.Contains("20", cut.Find($".{Css.Classes.Slider.Value}").TextContent);
        Assert.Contains("80", cut.Find($".{Css.Classes.Slider.Value}").TextContent);
    }

    [Fact]
    public void InteriorInit_AddsAnchorGap_ThreeSegments()
    {
        // Init interior (0) between Min=-100/Max=100 with Value=-50 -> notch at handle AND
        // at the anchor -> three rail segments.
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, -100.0).Add(x => x.Max, 100.0)
            .Add(x => x.Init, 0.0).Add(x => x.Value, -50.0));

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Slider.Seg}").Count);
    }

    [Fact]
    public void Init_DefaultsToMin_FillStartsAtZero()
    {
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Value, 50.0));

        var wrap = cut.Find($".{Css.Classes.Slider.TrackWrap}");
        Assert.Contains("--_init:0.00%", wrap.GetAttribute("style"));
    }

    [Fact]
    public void Init_SetsAnchorPercent()
    {
        // Min=-100, Max=100, Init=0 -> init anchor at 50%
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, -100.0).Add(x => x.Max, 100.0)
            .Add(x => x.Init, 0.0).Add(x => x.Value, -50.0));

        var wrap = cut.Find($".{Css.Classes.Slider.TrackWrap}");
        var style = wrap.GetAttribute("style")!;
        Assert.Contains("--_init:50.00%", style);
        Assert.Contains("--_pct:25.00%", style);   // value -50 -> 25%
    }

    [Fact]
    public void Init_ActiveStops_SpanInitToValue()
    {
        // Min=-100,Max=100,Init=0,Value=-50,Step=50 -> stops at -100,-50,0,50,100
        // active span [-50, 0] -> stops -50 and 0 active = 2
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Min, -100.0).Add(x => x.Max, 100.0)
            .Add(x => x.Init, 0.0).Add(x => x.Value, -50.0)
            .Add(x => x.Step, 50.0).Add(x => x.Stepper, true));

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Slider.TickActive}").Count);
    }

    [Fact]
    public void MouseWheel_ScrollUp_IncrementsByStep()
    {
        double? changed = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double v) => changed = v));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.Equal(55.0, changed);
    }

    [Fact]
    public void MouseWheel_ScrollDown_DecrementsByStep()
    {
        double? changed = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double v) => changed = v));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = 1 });

        Assert.Equal(45.0, changed);
    }

    [Fact]
    public void MouseWheel_ClampsAtMax_NoCallbackWhenAlreadyAtBound()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 100.0)
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Disabled_DoesNothing()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Disabled, true)
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Off_DoesNothing()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.Value, 50.0)
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Range_PlainWheel_MovesLowHandleOnly()
    {
        double? low = null, high = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 60.0)
            .Add(x => x.ValueChanged, (double v) => low = v)
            .Add(x => x.ValueEndChanged, (double v) => high = v));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.Equal(25.0, low);   // low handle moved
        Assert.Null(high);         // high handle untouched
    }

    [Fact]
    public void MouseWheel_Range_CtrlWheel_MovesHighHandleOnly()
    {
        double? low = null, high = null;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 20.0).Add(x => x.ValueEnd, 60.0)
            .Add(x => x.ValueChanged, (double v) => low = v)
            .Add(x => x.ValueEndChanged, (double v) => high = v));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1, CtrlKey = true });

        Assert.Equal(65.0, high);  // high handle moved
        Assert.Null(low);          // low handle untouched
    }

    [Fact]
    public void MouseWheel_Range_PlainWheel_LowCannotPassHigh()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 60.0).Add(x => x.ValueEnd, 60.0)   // low already at high
            .Add(x => x.ValueChanged, (double _) => invoked = true));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = -1 });

        Assert.False(invoked);
    }

    [Fact]
    public void MouseWheel_Range_CtrlWheel_HighCannotDropBelowLow()
    {
        var invoked = false;
        var cut = Render<FlareSlider>(p => p
            .Add(x => x.MouseWheel, true).Add(x => x.Range, true)
            .Add(x => x.Min, 0.0).Add(x => x.Max, 100.0).Add(x => x.Step, 5.0)
            .Add(x => x.Value, 60.0).Add(x => x.ValueEnd, 60.0)   // high already at low
            .Add(x => x.ValueEndChanged, (double _) => invoked = true));

        cut.Find($".{Css.Classes.Slider.TrackArea}").TriggerEvent("onwheel", new WheelEventArgs { DeltaY = 1, CtrlKey = true });

        Assert.False(invoked);
    }
}

// ------------------------------------------------------------------------------
// FlareList  (6 tests from Wave5)
// ------------------------------------------------------------------------------
