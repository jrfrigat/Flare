using System.Globalization;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-185: the week picker commits the first day of a week - picked from any of its days, by the keyboard, typed
/// as an ISO week or a date - numbers it by the culture's rule or ISO 8601, paints it whole and honours Min/Max and
/// the locked states.
/// </summary>
public class WeekPickerTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    private IRenderedComponent<FlareWeekPicker> RenderPicker(Action<DateOnly?> changed, DateOnly? value = null,
        bool iso = false, CultureInfo? culture = null, DateOnly? min = null, bool inline = true) =>
        Render<FlareWeekPicker>(p => p.Add(x => x.Inline, inline).Add(x => x.Culture, culture ?? Ru).Add(x => x.IsoWeeks, iso)
            .Add(x => x.Value, value).Add(x => x.Min, min).Add(x => x.ValueChanged, (DateOnly? v) => changed(v)));

    private static AngleSharp.Dom.IElement Day(IRenderedComponent<FlareWeekPicker> cut, int day) =>
        cut.FindAll($"button[role=gridcell]:not(.{Css.Classes.Picker.DayOutside})").First(b => b.TextContent.Trim() == day.ToString());

    [Fact]
    public void ClickingADay_CommitsTheFirstDayOfItsWeek()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 10, 1));
        Day(cut, 15).Click(); // Thursday
        Assert.Equal(new DateOnly(2026, 10, 12), committed); // ru-RU weeks start on Monday
    }

    // A week that starts in the month before must not move the calendar there.
    [Fact]
    public void PickingAWeekThatStartsLastMonth_KeepsTheMonth()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 10, 1));
        var october = cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim();
        Day(cut, 2).Click();
        cut.Render(p => p.Add(x => x.Value, committed));
        Assert.Equal(new DateOnly(2026, 9, 28), committed);
        Assert.Equal(october, cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
    }

    [Fact]
    public void TheWeek_IsPaintedAndSelectedWhole()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 10, 14));
        Assert.Equal(7, cut.FindAll("button[role=gridcell][aria-selected=true]").Count);
        Assert.Contains(Css.Classes.Daterangepicker.DayStart, Day(cut, 12).ClassName);
        Assert.Contains(Css.Classes.Daterangepicker.DayInRange, Day(cut, 15).ClassName);
        Assert.Contains(Css.Classes.Daterangepicker.DayEnd, Day(cut, 18).ClassName);
    }

    [Fact]
    public void FirstDayOfWeek_DecidesWhereTheWeekStarts()
    {
        DateOnly? committed = null;
        var cut = Render<FlareWeekPicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru).Add(x => x.FirstDayOfWeek, DayOfWeek.Sunday)
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v).Add(x => x.Value, new DateOnly(2026, 10, 1)));
        Day(cut, 15).Click();
        Assert.Equal(new DateOnly(2026, 10, 11), committed);
    }

    [Fact]
    public void IsoWeeks_UseTheIsoYear_AndWriteYyyyWww()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2025, 12, 31), iso: true, inline: false);
        Assert.Equal("2026-W01", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void CultureRule_WritesWeekNumberAndYear()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 10, 14), inline: false);
        var week = Ru.Calendar.GetWeekOfYear(new DateTime(2026, 10, 12), Ru.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);
        Assert.Equal(string.Format(Ru, FlareStrings.WeekPicker_Value, week, 2026), cut.Find("input").GetAttribute("value"));
    }

    [Theory]
    [InlineData("2026-W41", 2026, 10, 5)]
    [InlineData("2026w01", 2025, 12, 29)]
    [InlineData("14.10.2026", 2026, 10, 12)]
    public void TypedWeekOrDate_IsReadIntoItsWeek(string typed, int year, int month, int day)
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, inline: false);
        cut.Find("input").Change(typed);
        Assert.Equal(new DateOnly(year, month, day), committed);
    }

    [Fact]
    public void BadText_IsNotCommitted()
    {
        var published = false;
        var cut = RenderPicker(_ => published = true, inline: false);
        cut.Find("input").Change("2026-W60");
        cut.Find("input").Change("soon");
        Assert.False(published);
    }

    [Fact]
    public void ArrowDownAndEnter_PickTheNextWeek()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 10, 14));
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("button[role=gridcell][tabindex='0']").Click(); // Enter/Space activate the focused day natively
        Assert.Equal(new DateOnly(2026, 10, 19), committed);
    }

    [Fact]
    public void Min_DisablesEarlierDays_AndStopsAWeekWithNoneLeft()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 10, 20), min: new DateOnly(2026, 10, 15));
        Assert.True(Day(cut, 13).HasAttribute("disabled"));
        Day(cut, 16).Click();
        Assert.Equal(new DateOnly(2026, 10, 12), committed);
        committed = null;
        cut.Find("input").Change("01.10.2026");
        Assert.Null(committed);
    }

    [Fact]
    public void ReadOnlyAndDisabled_ChangeNothing()
    {
        var published = false;
        var ro = Render<FlareWeekPicker>(p => p.Add(x => x.Inline, true).Add(x => x.ReadOnly, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 14)).Add(x => x.ValueChanged, (DateOnly? _) => published = true));
        ro.FindAll($".{Css.Classes.DatePicker.Footer} button").Last().Click();
        Assert.False(published);
        var off = Render<FlareWeekPicker>(p => p.Add(x => x.Inline, true).Add(x => x.Disabled, true).Add(x => x.Culture, Ru));
        Assert.All(off.FindAll("button[role=gridcell]"), b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void IsoWeekColumn_ShowsIsoNumbers()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 1, 1), iso: true);
        Assert.Equal("1", cut.FindAll("[role=rowheader]")[0].TextContent.Trim()); // 29.12.2025 starts 2026-W01
    }

    [Fact]
    public void OutsideValue_ShowsItsMonth()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 10, 14));
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2027, 2, 10)));
        Assert.Equal(new DateOnly(2027, 2, 1).ToString("MMMM yyyy", Ru), cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
    }

    [Fact]
    public void Persian_WeekYearIsPersian()
    {
        var fa = CultureInfo.GetCultureInfo("fa-IR");
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 10, 3), culture: fa, inline: false);
        Assert.Contains("1405", cut.Find("input").GetAttribute("value"));
    }

    [Theory]
    [InlineData(DayOfWeek.Sunday)]
    [InlineData(DayOfWeek.Monday)]
    [InlineData(DayOfWeek.Tuesday)]
    [InlineData(DayOfWeek.Wednesday)]
    [InlineData(DayOfWeek.Thursday)]
    [InlineData(DayOfWeek.Friday)]
    [InlineData(DayOfWeek.Saturday)]
    public void BoundaryWeeks_SelectOnlyRepresentableDays(DayOfWeek first)
    {
        foreach (var value in new[] { DateOnly.MinValue, DateOnly.MaxValue })
        {
            DateOnly? committed = null;
            var cut = Render<FlareWeekPicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
                .Add(x => x.FirstDayOfWeek, first).Add(x => x.Value, value)
                .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));
            var offset = ((int)value.DayOfWeek - (int)first + 7) % 7;
            var start = Math.Max(0, value.DayNumber - offset);
            var end = Math.Min(DateOnly.MaxValue.DayNumber, value.DayNumber + 6 - offset);
            Assert.Equal(end - start + 1, cut.FindAll("button[role=gridcell][aria-selected=true]").Count);
            Day(cut, value.Day).Click();
            Assert.Equal(DateOnly.FromDayNumber(start), committed);
        }
    }

    [Fact]
    public void TruncatedFirstWeek_EndsBeforeTheNextSunday()
    {
        var cut = Render<FlareWeekPicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.FirstDayOfWeek, DayOfWeek.Sunday).Add(x => x.Value, DateOnly.MinValue));
        Assert.Contains(Css.Classes.Daterangepicker.DayStart, Day(cut, 1).ClassName);
        Assert.Contains(Css.Classes.Daterangepicker.DayEnd, Day(cut, 6).ClassName);
        Assert.Equal("false", Day(cut, 7).GetAttribute("aria-selected"));
    }

    [Theory]
    [InlineData("0000-W01")]
    [InlineData("2026-W00")]
    [InlineData("2026-W54")]
    [InlineData("2025-W53")]
    [InlineData("9999-W53")]
    public void InvalidIsoWeek_PreservesValueWithoutThrowing(string text)
    {
        var published = false;
        var cut = RenderPicker(_ => published = true, new DateOnly(2026, 10, 14), iso: true, inline: false);
        var before = cut.Find("input").GetAttribute("value");
        cut.Find("input").Change(text);
        Assert.False(published);
        Assert.Equal(before, cut.Find("input").GetAttribute("value"));
    }

    [Theory]
    [InlineData("0001-W01", 1, 1, 1)]
    [InlineData("2020-W53", 2020, 12, 28)]
    [InlineData("2026-W01", 2025, 12, 29)]
    [InlineData("9999-W52", 9999, 12, 27)]
    public void ValidIsoWeek_IncludingYearRollover_IsCommitted(string text, int year, int month, int day)
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, iso: true, inline: false);
        cut.Find("input").Change(text);
        Assert.Equal(new DateOnly(year, month, day), committed);
    }

    [Fact]
    public void LastWeek_AvailabilityHonorsTheOnlyAllowedDay()
    {
        DateOnly? committed = null;
        var cut = Render<FlareWeekPicker>(p => p.Add(x => x.Inline, true).Add(x => x.IsoWeeks, true)
            .Add(x => x.Value, DateOnly.MaxValue).Add(x => x.Min, DateOnly.MaxValue).Add(x => x.Max, DateOnly.MaxValue)
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));
        cut.Find("input").Change("9999-W52");
        Assert.Equal(new DateOnly(9999, 12, 27), committed);
        committed = null;
        cut.Render(p => p.Add(x => x.IsDateDisabled, (DateOnly d) => d == DateOnly.MaxValue));
        cut.Find("input").Change("9999-W52");
        Assert.Null(committed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UnsupportedCultureCalendarDate_StillHasAWeekLabel(bool last)
    {
        var cut = RenderPicker(_ => { }, last ? DateOnly.MaxValue : DateOnly.MinValue,
            culture: CultureInfo.GetCultureInfo("ar-SA"), inline: false);
        Assert.False(string.IsNullOrEmpty(cut.Find("input").GetAttribute("value")));
    }
}
