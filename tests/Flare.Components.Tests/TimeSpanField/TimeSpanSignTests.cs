using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// The sign is a property OF the value, not state beside it: a field handed a negative duration rendered
// a "+" until this was fixed, because only the commit path ever wrote the flag.
public class TimeSpanSignTests : FlareTestContext
{
    [Fact]
    public void ANegativeValueRendersANegativeSign()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(-75)));

        Assert.Equal("-", cut.Find($".{Css.Classes.TimeSpanField.Sign}").TextContent.Trim());
        Assert.Equal("true", cut.Find($".{Css.Classes.TimeSpanField.Sign}").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void SegmentsShowTheMagnitudeOfANegativeDuration()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(-75)));

        var inputs = cut.FindAll($".{Css.Classes.TimeSpanField.Input}");
        Assert.Equal("1", inputs[0].GetAttribute("value"));
        Assert.Equal("15", inputs[1].GetAttribute("value"));
    }

    [Fact]
    public void APositiveValueRendersAPositiveSign()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Value, TimeSpan.FromHours(3)));

        Assert.Equal("+", cut.Find($".{Css.Classes.TimeSpanField.Sign}").TextContent.Trim());
    }

    // Editing a segment of a negative duration must keep it negative - the sign is not something the
    // user has to re-apply after every keystroke.
    [Fact]
    public void EditingKeepsTheSign()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(-75))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.FindAll($".{Css.Classes.TimeSpanField.Input}")[1].Change("30");

        Assert.Equal(TimeSpan.FromMinutes(-90), captured);
    }
}
