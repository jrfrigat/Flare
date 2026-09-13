using Flare.Components.Services;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public sealed class FlareClockDialTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-components.js";

    /// <summary>
    /// The dial diameter is a theme token, so a press is resolved against the rendered width. On a
    /// 400px dial the bottom edge is six o'clock; measured from a fixed 128px centre it reads as five.
    /// </summary>
    [Fact]
    public void APressIsResolvedAgainstTheRenderedDialSize()
    {
        MeasureDialAs(400);
        var hour = -1;
        var cut = Render<FlareClockDial>(p => p
            .Add(x => x.Hour, 9)
            .Add(x => x.HourChanged, h => hour = h));

        cut.Find($".{Css.Classes.ClockDial.Dial}").PointerDown(new PointerEventArgs { OffsetX = 200, OffsetY = 380 });

        Assert.Equal(6, hour);
    }

    /// <summary>The 24-hour inner ring is a fraction of the rendered radius, not of a fixed one.</summary>
    [Fact]
    public void TheInnerRingScalesWithTheRenderedDialSize()
    {
        MeasureDialAs(400);
        var hour = -1;
        var cut = Render<FlareClockDial>(p => p
            .Add(x => x.Hour, 9)
            .Add(x => x.Is24Hour, true)
            .Add(x => x.HourChanged, h => hour = h));

        cut.Find($".{Css.Classes.ClockDial.Dial}").PointerDown(new PointerEventArgs { OffsetX = 200, OffsetY = 100 });

        Assert.Equal(0, hour);
    }

    /// <summary>
    /// A tap's pointerup arrives while the press is still waiting for the dial to be measured, and
    /// pointerup moves the dial on to minutes. The tapped hour must still land on the hour.
    /// </summary>
    [Fact]
    public async Task ATapThatEndsBeforeTheMeasurementReturnsStillSetsTheHour()
    {
        var bounds = JSInterop.SetupModule(Module).Setup<ElementBounds>("flareGetBounds", _ => true);
        int hour = -1, minute = -1;
        var cut = Render<FlareClockDial>(p => p
            .Add(x => x.Hour, 9)
            .Add(x => x.HourChanged, h => hour = h)
            .Add(x => x.MinuteChanged, m => minute = m));
        string Dial() => $".{Css.Classes.ClockDial.Dial}";

        var press = cut.Find(Dial()).PointerDownAsync(new PointerEventArgs { OffsetX = 200, OffsetY = 380 });
        cut.Find(Dial()).PointerUp();
        bounds.SetResult(new ElementBounds(0, 400, 0, 400, 1000, 1000));
        await press;

        Assert.Equal(6, hour);
        Assert.Equal(-1, minute);
    }

    private void MeasureDialAs(double width) =>
        JSInterop.SetupModule(Module)
            .Setup<ElementBounds>("flareGetBounds", _ => true)
            .SetResult(new ElementBounds(0, width, 0, width, 1000, 1000));
}
