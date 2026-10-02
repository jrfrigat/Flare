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
}
