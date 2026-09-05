using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class TimeSpanPickerTests : FlareTestContext
{
    [Fact]
    public void ShowsOneSegmentPerRequestedUnit()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Units, TimeSpanUnits.All));

        Assert.Equal(4, cut.FindAll($".{Css.Classes.TimeSpanField.Input}").Count);
    }

    [Fact]
    public void HoursMinutesShowsTwo()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Units, TimeSpanUnits.HoursMinutes));

        Assert.Equal(2, cut.FindAll($".{Css.Classes.TimeSpanField.Input}").Count);
    }

    // The largest shown segment absorbs everything above it: a field showing only hours on a two-day
    // duration must read 48, not 0.
    [Fact]
    public void TheLargestSegmentCarriesTheOverflow()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromHours(50)));

        var hours = cut.FindAll($".{Css.Classes.TimeSpanField.Input}")[0];
        Assert.Equal("50", hours.GetAttribute("value"));
    }

    [Fact]
    public void SegmentsBelowTheLargestAreBoundedToTheirPlace()
    {
        var cut = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Units, TimeSpanUnits.All));
        var inputs = cut.FindAll($".{Css.Classes.TimeSpanField.Input}");

        Assert.Null(inputs[0].GetAttribute("max"));      // days: no ceiling
        Assert.Equal("23", inputs[1].GetAttribute("max"));
        Assert.Equal("59", inputs[2].GetAttribute("max"));
        Assert.Equal("59", inputs[3].GetAttribute("max"));
    }

    [Fact]
    public void EditingASegmentRecomposesTheWholeDuration()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Value, TimeSpan.FromMinutes(90))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.FindAll($".{Css.Classes.TimeSpanField.Input}")[1].Change("45");

        Assert.Equal(TimeSpan.FromMinutes(105), captured);
    }

    [Fact]
    public void ValuesClampIntoTheRange()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Units, TimeSpanUnits.HoursMinutes)
            .Add(x => x.Max, TimeSpan.FromHours(8))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.FindAll($".{Css.Classes.TimeSpanField.Input}")[0].Change("40");

        Assert.Equal(TimeSpan.FromHours(8), captured);
    }

    [Fact]
    public void TheSignToggleAppearsOnlyWhenNegativesAreAllowed()
    {
        var plain = Render<FlareTimeSpanPicker>();
        var signed = Render<FlareTimeSpanPicker>(p => p.Add(x => x.Negatable, true));

        Assert.Empty(plain.FindAll($".{Css.Classes.TimeSpanField.Sign}"));
        Assert.Single(signed.FindAll($".{Css.Classes.TimeSpanField.Sign}"));
    }

    [Fact]
    public void TogglingTheSignFlipsTheDuration()
    {
        TimeSpan? captured = null;
        var cut = Render<FlareTimeSpanPicker>(p => p
            .Add(x => x.Negatable, true)
            .Add(x => x.Value, TimeSpan.FromHours(2))
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<TimeSpan?>(this, v => captured = v)));

        cut.Find($".{Css.Classes.TimeSpanField.Sign}").Click();

        Assert.Equal(TimeSpan.FromHours(-2), captured);
    }
}

// ------------------------------------------------------------------------------
// FlarePullToRefresh - the gesture
// ------------------------------------------------------------------------------
