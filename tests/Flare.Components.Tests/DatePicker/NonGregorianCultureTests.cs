using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-182 (replaces the Gregorian swap of TASK-150): the pickers draw, name and write dates on the culture's own
/// calendar - Persian for fa-IR, Um al-Qura for ar-SA - or on the <c>Calendar</c> they are given among the
/// culture's optional calendars; Thai Buddhist and Japanese cultures keep the Gregorian months.
/// </summary>
public class NonGregorianCultureTests : FlareTestContext
{
    private static readonly DateOnly Oct3 = new(2026, 10, 3);
    private static readonly CultureInfo Fa = CultureInfo.GetCultureInfo("fa-IR");
    private static readonly CultureInfo Ar = CultureInfo.GetCultureInfo("ar-SA");
    private static readonly CultureInfo He = CultureInfo.GetCultureInfo("he-IL");

    private IRenderedComponent<FlareDatePicker> Inline(CultureInfo culture, DateOnly value, Calendar? calendar = null,
        PickerOpenTo openTo = PickerOpenTo.Day) =>
        Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, culture).Add(x => x.Calendar, calendar)
            .Add(x => x.Value, value).Add(x => x.OpenTo, openTo));

    private static string Header(IRenderedComponent<FlareDatePicker> cut) =>
        cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim();

    [Fact]
    public void Persian_DrawsThePersianMonth()
    {
        var cut = Inline(Fa, Oct3);
        var persian = new PersianCalendar();
        Assert.Equal(Oct3.ToString("MMMM yyyy", Fa), Header(cut));
        var days = cut.FindAll($"button[role=gridcell]:not(.{Css.Classes.Picker.DayOutside})");
        Assert.Equal(persian.GetDaysInMonth(1405, 7), days.Count);
        Assert.Equal("1", days[0].TextContent.Trim());
        Assert.Equal("1405/7/11", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Persian_TypedDate_IsReadOnThePersianCalendar()
    {
        DateOnly? result = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Fa).Add(x => x.ValueChanged, (DateOnly? v) => result = v));
        cut.Find("input").Change("1405/07/11");
        Assert.Equal(Oct3, result);
    }

    [Fact]
    public void Persian_ArrowsCrossIntoTheNextPersianMonth()
    {
        var cut = Inline(Fa, new DateOnly(2026, 10, 22)); // 30 Mehr 1405, the month's last day
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal(new DateOnly(2026, 10, 23).ToString("MMMM yyyy", Fa), Header(cut));
    }

    [Fact]
    public void UmAlQura_StopsAtTheEndOfItsRange()
    {
        var cut = Inline(Ar, new DateOnly(2077, 11, 10));
        var next = cut.FindAll($".{Css.Classes.DatePicker.Header} button[aria-label]").Last();
        Assert.True(next.HasAttribute("disabled"));
    }

    [Fact]
    public void UmAlQura_ValueOutsideItsRange_StillRenders()
    {
        var cut = Inline(Ar, new DateOnly(1800, 1, 1));
        Assert.False(string.IsNullOrEmpty(cut.Find("input").GetAttribute("value")));
        Assert.NotEmpty(cut.FindAll("button[role=gridcell]"));
    }

    [Fact]
    public void GregorianCalendarParameter_KeepsGregorianMonths()
    {
        var cut = Inline(Fa, Oct3, new GregorianCalendar());
        var gregorian = CalendarMath.PickerCulture(Fa, new GregorianCalendar());
        Assert.IsType<GregorianCalendar>(gregorian.DateTimeFormat.Calendar);
        Assert.Equal(new DateTime(2026, 10, 1).ToString("MMMM yyyy", gregorian), Header(cut));
        Assert.Equal(31, cut.FindAll($"button[role=gridcell]:not(.{Css.Classes.Picker.DayOutside})").Count);
    }

    [Fact]
    public void CalendarTheCultureDoesNotOffer_IsIgnored()
    {
        var ru = CultureInfo.GetCultureInfo("ru-RU");
        Assert.Same(ru, CalendarMath.PickerCulture(ru, new PersianCalendar()));
        var cut = Inline(ru, Oct3, new PersianCalendar());
        Assert.Equal("03.10.2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void PickerCulture_IsCachedPerCalendarType()
    {
        Assert.Same(CalendarMath.PickerCulture(He, new HebrewCalendar()), CalendarMath.PickerCulture(He, new HebrewCalendar()));
    }

    [Fact]
    public void Hebrew_LeapYearShowsThirteenMonths_AndTheFieldTakesItsLetters()
    {
        var hebrew = new HebrewCalendar();
        var cut = Inline(He, Oct3, hebrew, PickerOpenTo.Month); // 5787 is a leap year
        Assert.Equal(13, cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}").Count);

        var heb = CalendarMath.PickerCulture(He, hebrew);
        var text = Oct3.ToString("d", heb);
        Assert.Equal(text, cut.Find("input").GetAttribute("value"));

        DateOnly? result = null;
        var field = Render<FlareDatePicker>(p => p.Add(x => x.Culture, He).Add(x => x.Calendar, hebrew)
            .Add(x => x.ValueChanged, (DateOnly? v) => result = v));
        field.Find("input").Input(text);
        Assert.Null(result);
        field.Find("input").Change(text);
        Assert.Equal(Oct3, result);
    }

    [Fact]
    public void DateTimeAndRange_UseThePersianMonthToo()
    {
        var dt = Render<FlareDateTimePicker>(p => p.Add(x => x.Culture, Fa).Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 3, 12, 0, 0, TimeSpan.Zero)));
        dt.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal(Oct3.ToString("MMMM yyyy", Fa), dt.Find($".{Css.Classes.DateTimePicker.NavLabel}").TextContent.Trim());

        var range = Render<FlareDateRangePicker>(p => p.Add(x => x.Culture, Fa).Add(x => x.StartDate, Oct3));
        range.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal(Oct3.ToString("MMMM yyyy", Fa), range.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
    }

    [Fact]
    public void RangePresetThisMonth_IsThePersianMonth()
    {
        (DateOnly? Start, DateOnly? End) range = default;
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.Culture, Fa).Add(x => x.ShowPresets, true)
            .Add(x => x.StartDateChanged, (DateOnly? v) => range.Start = v).Add(x => x.EndDateChanged, (DateOnly? v) => range.End = v));
        cut.FindAll($".{Css.Classes.Daterangepicker.Presets} button")[4].Click();
        var p = new PersianCalendar();
        var now = TimeProvider.System.GetLocalNow().DateTime;
        var first = p.ToDateTime(p.GetYear(now), p.GetMonth(now), 1, 0, 0, 0, 0);
        var last = first.AddDays(p.GetDaysInMonth(p.GetYear(now), p.GetMonth(now)) - 1);
        Assert.Equal((DateOnly.FromDateTime(first), DateOnly.FromDateTime(last)), (range.Start, range.End));
    }

    [Fact]
    public void ThaiBuddhist_KeepsGregorianMonthsAndItsYears()
    {
        var th = new CultureInfo("th-TH");
        Assert.Same(th, CalendarMath.PickerCulture(th));
        var cut = Inline(th, Oct3, openTo: PickerOpenTo.Year);
        Assert.Contains(cut.FindAll($".{Css.Classes.DatePicker.YearBtn}"), b => b.TextContent.Trim() == "2569");
    }

    [Fact]
    public void YearLabel_FallsBackOutsideTheCalendarRange()
    {
        var ja = (CultureInfo)new CultureInfo("ja-JP").Clone();
        ja.DateTimeFormat.Calendar = new JapaneseCalendar();
        Assert.Equal("1700", CalendarMath.YearLabel(1700, ja));
        Assert.Equal("2026", CalendarMath.YearLabel(2026, CultureInfo.InvariantCulture));
        Assert.Equal("1405", CalendarMath.YearLabel(1405, Fa));
    }
}
