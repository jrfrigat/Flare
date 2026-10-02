using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-149: a day template fills the day cell while the picker keeps the cell's button, label, focus and
/// state; a custom parser reads free text and keeps the value when it cannot.
/// </summary>
public class PickerExtensibilityTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");

    private static readonly RenderFragment<DateOnly> Dot = d => b =>
    {
        b.OpenElement(0, "span");
        b.AddAttribute(1, "class", "event-dot");
        b.AddContent(2, d.Day);
        b.CloseElement();
    };

    [Fact]
    public void DayTemplate_FillsTheCellButKeepsItsButtonAndLabel()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)).Add(x => x.DayTemplate, Dot)
            .Add(x => x.IsDateDisabled, d => d.Day == 16));

        var cell = cut.Find($"[aria-label='{new DateOnly(2026, 10, 15).ToString("D", Ru)}']");
        Assert.Equal("BUTTON", cell.TagName);
        Assert.Equal("true", cell.GetAttribute("aria-selected"));
        Assert.NotNull(cell.QuerySelector(".event-dot"));
        Assert.True(cut.Find($"[aria-label='{new DateOnly(2026, 10, 16).ToString("D", Ru)}']").HasAttribute("disabled"));
    }

    [Fact]
    public void DayTemplate_ReachesTheRangeAndDateTimeCalendars()
    {
        var range = Render<FlareDateRangePicker>(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar).Add(x => x.DayTemplate, Dot));
        Assert.Equal(42 - range.FindAll($".{Css.Classes.Picker.Grid} span[aria-hidden]").Count, range.FindAll(".event-dot").Count);

        var dateTime = Render<FlareDateTimePicker>(p => p.Add(x => x.DayTemplate, Dot));
        dateTime.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.NotEmpty(dateTime.FindAll(".event-dot"));
    }

    [Fact]
    public void ParseInput_ReadsFreeTextOnChange()
    {
        DateOnly? result = null;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Ru)
            .Add(x => x.ParseInput, s => s == "tomorrow" ? new DateOnly(2026, 10, 16) : null)
            .Add(x => x.ValueChanged, (DateOnly? v) => result = v));

        cut.Find("input").Focus();
        cut.Find("input").Input("tomorrow");
        Assert.Equal("tomorrow", cut.Find("input").GetAttribute("value"));   // no digit mask
        Assert.Null(result);

        cut.Find("input").Change("tomorrow");
        Assert.Equal(new DateOnly(2026, 10, 16), result);
    }

    [Fact]
    public void ParseInput_RefusingTheText_KeepsTheValue()
    {
        var calls = 0;
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Culture, Ru).Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ParseInput, _ => null).Add(x => x.ValueChanged, (DateOnly? _) => calls++));

        cut.Find("input").Change("nonsense");
        cut.Find("input").Blur();

        Assert.Equal(0, calls);
        Assert.Equal("15.10.2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void DateTimeParseInput_StillObeysMinMax()
    {
        var calls = 0;
        var min = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Min, min)
            .Add(x => x.ParseInput, _ => new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero))
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => calls++));

        cut.Find("input").Change("last month");

        Assert.Equal(0, calls);
    }
}
