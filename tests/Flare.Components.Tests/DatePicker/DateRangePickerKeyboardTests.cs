using System.Globalization;
using Bunit;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-128: the range calendar moves a keyboard cursor with the arrow keys (skipping unavailable days and
/// crossing months), previews the range under the cursor while the end is picked, and keeps the month the
/// user navigated to across a re-render.
/// </summary>
public class DateRangePickerKeyboardTests : FlareTestContext
{
    private static string Label(DateOnly d) => d.ToString("D", CultureInfo.CurrentCulture);

    private IRenderedComponent<FlareDateRangePicker> RenderCalendar(
        DateOnly? start = null, DateOnly? end = null, Func<DateOnly, bool>? isDisabled = null)
        => Render<FlareDateRangePicker>(p => p
            .Add(x => x.Mode, DateRangePickerMode.Calendar)
            .Add(x => x.StartDate, start)
            .Add(x => x.EndDate, end)
            .Add(x => x.IsDateDisabled, isDisabled));

    private static void Key(IRenderedComponent<FlareDateRangePicker> cut, string key)
        => cut.Find($".{Css.Classes.Picker.Grid}").KeyDown(key);

    private static string? TabTarget(IRenderedComponent<FlareDateRangePicker> cut)
        => cut.Find($".{Css.Classes.Picker.Day}[tabindex='0']").GetAttribute("aria-label");

    [Fact]
    public void ArrowKey_MovesTheCursorAndFocus()
    {
        var cut = RenderCalendar(new DateOnly(2026, 10, 15), new DateOnly(2026, 10, 20));

        Key(cut, "ArrowRight");

        Assert.Equal(Label(new DateOnly(2026, 10, 16)), TabTarget(cut));
        JSInterop.VerifyFocusAsyncInvoke();
    }

    [Fact]
    public void ArrowKey_SkipsUnavailableDays()
    {
        var cut = RenderCalendar(new DateOnly(2026, 10, 15), new DateOnly(2026, 10, 20), d => d.Day == 16);

        Key(cut, "ArrowRight");

        Assert.Equal(Label(new DateOnly(2026, 10, 17)), TabTarget(cut));
    }

    [Fact]
    public void ArrowKey_CrossesIntoTheNextMonth()
    {
        var cut = RenderCalendar(new DateOnly(2026, 10, 31), new DateOnly(2026, 10, 31));

        Key(cut, "ArrowRight");

        Assert.Equal(new DateTime(2026, 11, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture),
            cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
        Assert.Equal(Label(new DateOnly(2026, 11, 1)), TabTarget(cut));
    }

    [Fact]
    public void WhilePickingTheEnd_ThePreviewFollowsTheCursor()
    {
        var cut = RenderCalendar(new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 2));
        cut.Find($"[aria-label='{Label(new DateOnly(2026, 10, 10))}']").Click();   // start a new range

        Key(cut, "ArrowRight");
        Key(cut, "ArrowRight");

        var end = cut.Find($"[aria-label='{Label(new DateOnly(2026, 10, 12))}']");
        Assert.Contains(Css.Classes.Daterangepicker.DayEnd, end.ClassList);
    }

    [Fact]
    public void Rerender_KeepsTheNavigatedMonth()
    {
        var cut = RenderCalendar(new DateOnly(2026, 10, 15), new DateOnly(2026, 10, 20));
        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();   // next month

        cut.Render(p => p.Add(x => x.StartDate, new DateOnly(2026, 10, 15)));

        Assert.Equal(new DateTime(2026, 11, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture),
            cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent);
    }
}
