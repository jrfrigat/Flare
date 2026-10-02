using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-147: the date-time picker takes the time options of FlareTimePicker (ShowSeconds, HourStep,
/// MinuteStep) and the day filter of FlareDatePicker (IsDateDisabled), with the same meaning.
/// </summary>
public class DateTimePickerTimeOptionsTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");
    private static readonly TimeSpan Plus5 = TimeSpan.FromHours(5);

    private IRenderedComponent<FlareDateTimePicker> Picker(Action<ComponentParameterCollectionBuilder<FlareDateTimePicker>> extra,
        DateTimeOffset? value, Action<DateTimeOffset?> changed) =>
        Render<FlareDateTimePicker>(p =>
        {
            p.Add(x => x.Culture, Ru).Add(x => x.Value, value).Add(x => x.ValueChanged, changed)
             .Add(x => x.Mode, DateTimeVariant.Panels);
            extra(p);
        });

    [Fact]
    public void ShowSeconds_TypesSecondsAndKeepsTheFraction()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, Plus5).AddTicks(1234567);
        DateTimeOffset? result = null;
        var cut = Picker(p => p.Add(x => x.ShowSeconds, true), original, v => result = v);

        Assert.Equal("15.10.2026 14:30:45", cut.Find("input").GetAttribute("value"));
        cut.Find("input").Focus();
        cut.Find("input").Input("15102026143010");

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 14, 30, 10, Plus5).AddTicks(1234567), result);
    }

    [Fact]
    public void ShowSeconds_AddsASecondsBoxToThePopup()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, Plus5).AddTicks(1234567);
        DateTimeOffset? result = null;
        var cut = Picker(p => p.Add(x => x.ShowSeconds, true), original, v => result = v);

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var boxes = cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}");
        Assert.Equal(3, boxes.Count);
        boxes[2].Change("5");
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 14, 30, 5, Plus5).AddTicks(1234567), result);
    }

    [Fact]
    public void Steps_SetTheBoxStepAndDropATypedValueToIt()
    {
        DateTimeOffset? result = null;
        var cut = Picker(p => p.Add(x => x.MinuteStep, 15).Add(x => x.HourStep, 2),
            new DateTimeOffset(2026, 10, 15, 10, 0, 0, Plus5), v => result = v);

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var boxes = cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}");
        Assert.Equal("2", boxes[0].GetAttribute("step"));
        Assert.Equal("15", boxes[1].GetAttribute("step"));
        boxes[0].Change("13");
        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[1].Change("37");
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 12, 30, 0, Plus5), result);
    }

    [Fact]
    public void IsDateDisabled_DisablesTheDayAndRefusesTypedText()
    {
        var calls = 0;
        var cut = Picker(p => p.Add(x => x.IsDateDisabled, d => d.Day == 16),
            new DateTimeOffset(2026, 10, 15, 10, 0, 0, Plus5), _ => calls++);

        cut.Find("input").Change("16.10.2026 10:00");
        Assert.Equal(0, calls);

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.True(cut.Find($"[aria-label='{new DateOnly(2026, 10, 16).ToString("D", Ru)}']").HasAttribute("disabled"));
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal(new DateOnly(2026, 10, 17).ToString("D", Ru),
            cut.Find($".{Css.Classes.Picker.Day}[tabindex='0']").GetAttribute("aria-label"));
    }
}
