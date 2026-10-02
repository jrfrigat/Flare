using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-150: the calendar grid is Gregorian. A culture whose calendar has other months (Persian, Um al-Qura)
/// is shown on its Gregorian calendar so the header, the grid and the field name the same date; Thai Buddhist
/// keeps its own years.
/// </summary>
public class NonGregorianCultureTests : FlareTestContext
{
    private static readonly DateOnly Oct15 = new(2026, 10, 15);

    [Theory]
    [InlineData("fa-IR")]
    [InlineData("ar-SA")]
    public void MonthsOutsideTheGregorianGrid_UseTheGregorianCalendar(string name)
    {
        var culture = new CultureInfo(name);
        var gregorian = CalendarMath.PickerCulture(culture);
        Assert.IsType<GregorianCalendar>(gregorian.DateTimeFormat.Calendar);
        Assert.Equal(name, gregorian.Name);

        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, culture).Add(x => x.Value, Oct15));
        var header = cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent;
        Assert.Equal(new DateTime(2026, 10, 1).ToString("MMMM yyyy", gregorian), header.Trim());
        Assert.Equal(Oct15.ToString(gregorian.DateTimeFormat.ShortDatePattern, gregorian), cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void TypedTextOnTheGregorianCalendar_IsTheSameDate()
    {
        var fa = new CultureInfo("fa-IR");
        DateOnly? result = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, fa).Add(x => x.ValueChanged, (DateOnly? v) => result = v));

        cut.Find("input").Change(Oct15.ToString("d", CalendarMath.PickerCulture(fa)));

        Assert.Equal(Oct15, result);
    }

    [Fact]
    public void ThaiBuddhist_KeepsItsYears()
    {
        var th = new CultureInfo("th-TH");
        Assert.Same(th, CalendarMath.PickerCulture(th));

        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, th).Add(x => x.Value, Oct15)
            .Add(x => x.OpenTo, PickerOpenTo.Year));

        Assert.Contains(cut.FindAll($".{Css.Classes.DatePicker.YearBtn}"), b => b.TextContent.Trim() == "2569");
    }

    [Fact]
    public void YearLabel_FallsBackOutsideTheCalendarRange()
    {
        var ja = (CultureInfo)new CultureInfo("ja-JP").Clone();
        ja.DateTimeFormat.Calendar = new JapaneseCalendar();
        Assert.Equal("1700", CalendarMath.YearLabel(1700, ja));
        Assert.Equal("2026", CalendarMath.YearLabel(2026, CultureInfo.InvariantCulture));
    }
}
