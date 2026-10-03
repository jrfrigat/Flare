using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-161: a typed end or start passes the same length and blackout rules as a calendar pick, also past the
/// span the fields' Min/Max are worked out for.
/// </summary>
public class DateRangeFieldRulesTests : FlareTestContext
{
    private static readonly DateOnly Blackout = new(2020, 1, 1);

    private IRenderedComponent<FlareDateRangePicker> RenderFields(DateOnly? start, DateOnly? end,
        Action<DateOnly?> startChanged, Action<DateOnly?> endChanged, int? maxDays = null) =>
        Render<FlareDateRangePicker>(p => p.Add(x => x.Mode, DateRangePickerMode.Fields)
            .Add(x => x.Culture, CultureInfo.GetCultureInfo("en-GB"))
            .Add(x => x.StartDate, start).Add(x => x.EndDate, end).Add(x => x.MaxDays, maxDays)
            .Add(x => x.AllowDisabledDatesInRange, false)
            .Add(x => x.IsDateDisabled, d => d == Blackout)
            .Add(x => x.StartDateChanged, (DateOnly? v) => startChanged(v))
            .Add(x => x.EndDateChanged, (DateOnly? v) => endChanged(v)));

    [Fact]
    public void FarEnd_PastABlackoutDay_IsRejected_AndTheFieldShowsTheOldEnd()
    {
        var published = false;
        var cut = RenderFields(new DateOnly(2000, 1, 1), null, _ => { }, _ => published = true);

        cut.FindAll("input")[1].Change("01/01/2040");

        Assert.False(published);
        Assert.Equal(string.Empty, cut.FindAll("input")[1].GetAttribute("value") ?? string.Empty);
    }

    [Fact]
    public void FarStart_BeforeABlackoutDay_IsRejected()
    {
        var published = false;
        var cut = RenderFields(null, new DateOnly(2040, 1, 1), _ => published = true, _ => { });

        cut.FindAll("input")[0].Change("01/01/2000");

        Assert.False(published);
    }

    [Fact]
    public void FarEnd_WithoutABlackoutDayInside_IsAccepted()
    {
        DateOnly? end = null;
        var cut = RenderFields(new DateOnly(2021, 1, 1), null, _ => { }, v => end = v);

        cut.FindAll("input")[1].Change("01/01/2040");

        Assert.Equal(new DateOnly(2040, 1, 1), end);
    }

    [Fact]
    public void MaxDays_StillLimitsATypedEnd()
    {
        var published = false;
        var cut = RenderFields(new DateOnly(2021, 1, 1), null, _ => { }, _ => published = true, maxDays: 7);

        cut.FindAll("input")[1].Change("09/01/2021");

        Assert.False(published);
    }
}
