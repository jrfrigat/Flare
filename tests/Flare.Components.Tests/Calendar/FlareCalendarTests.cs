using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareCalendarTests : FlareTestContext
{
    [Fact]
    public void RendersSevenDayLabels()
    {
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1)));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Calendar.Root}"));
        Assert.Equal(7, cut.FindAll($".{Css.Classes.Calendar.DayLabel}").Count);
    }

    [Fact]
    public void ClickingDay_RaisesSelectedDateChanged()
    {
        DateOnly? picked = null;
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1))
            .Add(x => x.SelectedDateChanged, d => picked = d));

        cut.FindAll($".{Css.Classes.Calendar.Cell}")
            .First(c => !(c.ClassName ?? "").Contains("--other"))
            .Click();

        Assert.NotNull(picked);
    }
}
