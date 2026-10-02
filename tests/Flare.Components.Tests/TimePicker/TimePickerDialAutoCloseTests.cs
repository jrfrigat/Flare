using Bunit;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-112: AutoClose also finishes the dial popup, once the minute is settled (released or typed), and
/// never in the middle of a drag.
/// </summary>
public class TimePickerDialAutoCloseTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-components.js";

    private IRenderedComponent<FlareTimePicker> Open(bool autoClose, Action<TimeOnly?> changed)
    {
        // A 200px dial: the centre is (100, 100).
        JSInterop.SetupModule(Module)
            .Setup<ElementBounds>("flareGetBounds", _ => true)
            .SetResult(new ElementBounds(0, 200, 0, 200, 800, 800));

        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Use24Hour, true)
            .Add(x => x.Value, new TimeOnly(10, 0))
            .Add(x => x.AutoClose, autoClose)
            .Add(x => x.ValueChanged, (TimeOnly? v) => changed(v)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut;
    }

    private static bool IsOpen(IRenderedComponent<FlareTimePicker> cut)
        => cut.FindAll($".{Css.Classes.ClockDial.Root}").Count > 0;

    [Fact]
    public void TypedMinute_ConfirmsAndCloses()
    {
        TimeOnly? committed = null;
        var cut = Open(autoClose: true, v => committed = v);

        foreach (var k in new[] { "1", "4", "3", "0" })
            cut.Find($".{Css.Classes.ClockDial.Root}").KeyDown(k);

        Assert.Equal(new TimeOnly(14, 30), committed);
        Assert.False(IsOpen(cut));
    }

    [Fact]
    public void ReleasedMinute_ConfirmsAndCloses_ButADragDoesNot()
    {
        TimeOnly? committed = null;
        var cut = Open(autoClose: true, v => committed = v);
        cut.Find($".{Css.Classes.ClockDial.Root}").KeyDown("ArrowRight");   // minute dial

        // Press at 3 o'clock (15 min) and drag: nothing is confirmed while the pointer is down.
        cut.Find($".{Css.Classes.ClockDial.Dial}").PointerDown(new PointerEventArgs { OffsetX = 200, OffsetY = 100 });
        cut.Find($".{Css.Classes.ClockDial.Dial}").PointerMove(new PointerEventArgs { OffsetX = 100, OffsetY = 200 });
        Assert.Null(committed);
        Assert.True(IsOpen(cut));

        cut.Find($".{Css.Classes.ClockDial.Dial}").PointerUp(new PointerEventArgs { OffsetX = 100, OffsetY = 200 });

        Assert.Equal(new TimeOnly(10, 30), committed);
        Assert.False(IsOpen(cut));
    }

    [Fact]
    public void WithoutAutoClose_TheDialWaitsForOk()
    {
        TimeOnly? committed = null;
        var cut = Open(autoClose: false, v => committed = v);

        foreach (var k in new[] { "1", "4", "3", "0" })
            cut.Find($".{Css.Classes.ClockDial.Root}").KeyDown(k);

        Assert.Null(committed);
        Assert.True(IsOpen(cut));
    }
}
