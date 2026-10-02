using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-148: the range length (MinDays/MaxDays) and the blackout policy (AllowDisabledDatesInRange) hold for
/// the calendar, the fields and the presets alike, and leave a value bound from outside as it is.
/// </summary>
public class DateRangeRulesTests : FlareTestContext
{
    private static readonly DateOnly Oct10 = new(2026, 10, 10);

    private static string Label(DateOnly d) => d.ToString("D", CultureInfo.CurrentUICulture);

    private static void ClickDay(IRenderedComponent<FlareDateRangePicker> cut, DateOnly d) =>
        cut.Find($"[aria-label='{Label(d)}']").Click();

    private static bool IsOff(IRenderedComponent<FlareDateRangePicker> cut, DateOnly d) =>
        cut.Find($"[aria-label='{Label(d)}']").HasAttribute("disabled");

    private IRenderedComponent<FlareDateRangePicker> Calendar(Action<ComponentParameterCollectionBuilder<FlareDateRangePicker>> extra,
        Action<DateOnly?>? end = null) =>
        Render<FlareDateRangePicker>(p =>
        {
            p.Add(x => x.Mode, DateRangePickerMode.Calendar);
            p.Add(x => x.EndDateChanged, (DateOnly? v) => end?.Invoke(v));
            extra(p);
        });

    [Fact]
    public void Rules_MeasureTheRangeWithBothEnds()
    {
        var rules = new DateRangeRules(3, 5, true, null);
        Assert.False(rules.Allows(Oct10, Oct10.AddDays(1)));   // 2 days
        Assert.True(rules.Allows(Oct10.AddDays(2), Oct10));    // 3 days, either order
        Assert.True(rules.Allows(Oct10, Oct10.AddDays(4)));    // 5 days
        Assert.False(rules.Allows(Oct10, Oct10.AddDays(5)));   // 6 days
    }

    [Fact]
    public void Rules_RejectADisabledDayInsideOnlyWhenAsked()
    {
        Func<DateOnly, bool> blackout = d => d == Oct10.AddDays(2);
        Assert.True(new DateRangeRules(null, null, true, blackout).Allows(Oct10, Oct10.AddDays(4)));
        Assert.False(new DateRangeRules(null, null, false, blackout).Allows(Oct10, Oct10.AddDays(4)));
        Assert.Equal((DateOnly?)Oct10.AddDays(2), new DateRangeRules(null, null, false, blackout).Reach(Oct10).Hi);
    }

    [Fact]
    public void Calendar_DisablesEndsOutsideTheLengthWhilePickingTheEnd()
    {
        DateOnly? end = null;
        var cut = Calendar(p => p.Add(x => x.MinDays, 3).Add(x => x.MaxDays, 5).Add(x => x.StartDate, Oct10), v => end = v);

        ClickDay(cut, Oct10);
        Assert.True(IsOff(cut, Oct10.AddDays(1)));
        Assert.False(IsOff(cut, Oct10.AddDays(2)));
        Assert.False(IsOff(cut, Oct10.AddDays(4)));
        Assert.True(IsOff(cut, Oct10.AddDays(5)));

        ClickDay(cut, Oct10.AddDays(4));
        Assert.Equal(Oct10.AddDays(4), end);
    }

    [Fact]
    public void Calendar_StopsAtADisabledDayWhenDisabledDaysMayNotBeInside()
    {
        var cut = Calendar(p => p.Add(x => x.StartDate, Oct10)
            .Add(x => x.IsDateDisabled, d => d == Oct10.AddDays(3))
            .Add(x => x.AllowDisabledDatesInRange, false));

        ClickDay(cut, Oct10);

        Assert.False(IsOff(cut, Oct10.AddDays(2)));
        Assert.True(IsOff(cut, Oct10.AddDays(5)));
    }

    [Fact]
    public void Presets_OutsideTheRulesAreNotApplied()
    {
        var calls = 0;
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.ShowPresets, true).Add(x => x.MaxDays, 7)
            .Add(x => x.StartDateChanged, (DateOnly? _) => calls++));

        var last30 = cut.FindAll($".{Css.Classes.Daterangepicker.Preset}").First(b => b.TextContent.Contains("30"));
        last30.Click();

        Assert.Equal(0, calls);
    }

    [Fact]
    public void Fields_NarrowTheirBoundsFromTheOtherEnd()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.MinDays, 3).Add(x => x.MaxDays, 5)
            .Add(x => x.StartDate, Oct10).Add(x => x.EndDate, Oct10.AddDays(3)));

        var pickers = cut.FindComponents<FlareDatePicker>();
        Assert.Equal(Oct10.AddDays(2), pickers[1].Instance.Min);
        Assert.Equal(Oct10.AddDays(4), pickers[1].Instance.Max);
        Assert.Equal(Oct10.AddDays(3 - 4), pickers[0].Instance.Min);
        Assert.Equal(Oct10.AddDays(1), pickers[0].Instance.Max);
    }

    [Fact]
    public void ABoundValueOutsideTheRules_IsShownAsItIs()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.MaxDays, 3)
            .Add(x => x.StartDate, Oct10).Add(x => x.EndDate, Oct10.AddDays(20)));

        Assert.Equal(Oct10.AddDays(20), cut.Instance.EndDate);
        Assert.NotEmpty(cut.FindAll("input")[1].GetAttribute("value") ?? "");
    }
}
