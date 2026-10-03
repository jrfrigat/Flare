namespace Flare.Components.Tests;

/// <summary>
/// TASK-159: the time picker keeps what it does not show - the fraction of a second, and the seconds without
/// ShowSeconds - when the popup is confirmed or the text is edited, as the date-time picker does (TASK-136).
/// </summary>
public class TimePickerPrecisionTests : FlareTestContext
{
    private static readonly TimeOnly Precise = new TimeOnly(14, 30, 45).Add(TimeSpan.FromTicks(1234567));

    private IRenderedComponent<FlareTimePicker> RenderTime(TimeOnly? value, bool seconds, Action<TimeOnly?> changed,
        TimePickerVariant variant = TimePickerVariant.Dropdown, TimeOnly? max = null) =>
        Render<FlareTimePicker>(p => p.Add(x => x.Value, value).Add(x => x.ShowSeconds, seconds)
            .Add(x => x.PopupVariant, variant).Add(x => x.Use24Hour, true).Add(x => x.Max, max)
            .Add(x => x.ValueChanged, (TimeOnly? v) => changed(v)));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConfirmWithoutAChange_KeepsTheValue(bool seconds)
    {
        TimeOnly? committed = null;
        var cut = RenderTime(Precise, seconds, v => committed = v);
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.TimePicker.Actions} button").Last().Click();
        Assert.Equal(Precise, committed);
    }

    [Fact]
    public void MinuteChange_KeepsTheHiddenSecondsAndFraction()
    {
        TimeOnly? committed = null;
        var cut = RenderTime(Precise, false, v => committed = v);
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.TimePicker.Col}")[1].QuerySelectorAll("button").First(b => b.TextContent.Trim() == "31").Click();
        cut.FindAll($".{Css.Classes.TimePicker.Actions} button").Last().Click();
        Assert.Equal(Precise.AddMinutes(1), committed);
    }

    [Fact]
    public void TypedTime_KeepsTheHiddenTicks()
    {
        TimeOnly? committed = null;
        var cut = RenderTime(Precise, false, v => committed = v);
        cut.Find("input").Change("15:05");
        Assert.Equal(new TimeOnly(15, 5).Add(Precise.ToTimeSpan() - new TimeOnly(14, 30).ToTimeSpan()), committed);
    }

    [Fact]
    public void TypingExactlyMax_IsAllowed_WhenHiddenSecondsWouldPassIt()
    {
        TimeOnly? committed = null;
        var cut = RenderTime(new TimeOnly(9, 0, 30), false, v => committed = v, max: new TimeOnly(17, 0));
        cut.Find("input").Change("17:00");
        Assert.Equal(new TimeOnly(17, 0), committed);
    }

    [Fact]
    public void PickingTheSelectedListOption_KeepsTheValue()
    {
        TimeOnly? committed = null;
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, Precise).Add(x => x.PopupVariant, TimePickerVariant.List)
            .Add(x => x.MinuteStep, 30).Add(x => x.Use24Hour, true).Add(x => x.ValueChanged, (TimeOnly? v) => committed = v));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll("[role=option]").First(o => o.TextContent.Trim() == "14:30").Click();
        Assert.Null(committed);

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll("[role=option]").First(o => o.TextContent.Trim() == "15:00").Click();
        Assert.Equal(new TimeOnly(15, 0), committed);
    }
}
