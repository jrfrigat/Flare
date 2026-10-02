namespace Flare.Components.Tests;

/// <summary>
/// TASK-101: the date-time calendar must honour Min/Max. Days fully outside the range are unselectable,
/// and confirming an out-of-range date+time does not commit (matching the text-input behaviour).
/// </summary>
public class DateTimePickerRangeTests : FlareTestContext
{
    [Fact]
    public void OutOfRangeDays_AreDisabledInTheCalendar()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Min, new DateTimeOffset(new DateTime(2026, 10, 10), TimeSpan.Zero))
            .Add(x => x.Max, new DateTimeOffset(new DateTime(2026, 10, 20), TimeSpan.Zero)));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.Grid} button[disabled]"));
    }

    // TASK-137: a day counts in the offset of the value being picked, not in the bound's own offset.
    private static readonly System.Globalization.CultureInfo Ru = new("ru-RU");

    private static bool IsDisabled(IRenderedComponent<FlareDateTimePicker> cut, DateOnly day) =>
        cut.Find($"[aria-label='{day.ToString("D", Ru)}']").HasAttribute("disabled");

    [Fact]
    public void MinInAnotherOffset_KeepsTheBoundaryDaySelectable()
    {
        var value = new DateTimeOffset(2026, 10, 14, 20, 0, 0, TimeSpan.Zero);
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru).Add(x => x.Value, value)
            .Add(x => x.Min, new DateTimeOffset(2026, 10, 15, 0, 0, 0, TimeSpan.FromHours(5))));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.False(IsDisabled(cut, new DateOnly(2026, 10, 14)));
        Assert.True(IsDisabled(cut, new DateOnly(2026, 10, 13)));
    }

    [Fact]
    public void MaxInAnotherOffset_KeepsTheBoundaryDaySelectable()
    {
        // Max is 15.10 23:00 at -05:00 = 16.10 04:00Z, so the morning of 16.10 at +00:00 is still allowed.
        var value = new DateTimeOffset(2026, 10, 10, 12, 0, 0, TimeSpan.Zero);
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru).Add(x => x.Value, value)
            .Add(x => x.Max, new DateTimeOffset(2026, 10, 15, 23, 0, 0, TimeSpan.FromHours(-5))));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.False(IsDisabled(cut, new DateOnly(2026, 10, 16)));
        Assert.True(IsDisabled(cut, new DateOnly(2026, 10, 17)));
    }

    [Fact]
    public void BoundsAtTheEndsOfTheRange_DoNotThrow()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Value, new DateTimeOffset(1, 1, 1, 12, 0, 0, TimeSpan.Zero))
            .Add(x => x.Min, DateTimeOffset.MinValue)
            .Add(x => x.Max, DateTimeOffset.MaxValue));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.Grid} button"));
    }
}
