namespace Flare.Components.Tests;

/// <summary>
/// TASK-103: confirming the date-time without editing must not move the instant. The DateTimeOffset's
/// offset and seconds belong to the value, not to the machine's local zone.
/// </summary>
public class DateTimePickerOffsetTests : FlareTestContext
{
    [Fact]
    public void Confirm_KeepsTheValuesOffsetAndSeconds()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(5));
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Value, original)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.NotNull(committed);
        Assert.Equal(TimeSpan.FromHours(5), committed.Value.Offset);
        Assert.Equal(original, committed.Value);
    }
}
