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

    // TASK-136: the fraction of a second is part of the instant too.
    [Fact]
    public void Confirm_KeepsTheFractionOfASecond()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(5)).AddTicks(1234567);
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Value, original)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.Equal(original.UtcTicks, committed!.Value.UtcTicks);
        Assert.Equal(original.Offset, committed.Value.Offset);
    }

    [Fact]
    public void MinuteChange_KeepsSecondsAndFraction()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(5)).AddTicks(1234567);
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Value, original)
            .Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[1].Change("31");
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.Equal(original.AddMinutes(1), committed);
    }
}
