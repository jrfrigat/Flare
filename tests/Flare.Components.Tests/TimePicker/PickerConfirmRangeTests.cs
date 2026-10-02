using Bunit;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-125: the popup OK is offered only for a value inside Min/Max, and a refused confirm keeps the
/// popup open instead of closing it without a value.
/// </summary>
public class PickerConfirmRangeTests : FlareTestContext
{
    private static readonly TimeSpan Msk = TimeSpan.FromHours(3);

    private IRenderedComponent<FlareTimePicker> OpenTimePicker(bool autoClose = false, Action<TimeOnly?>? changed = null)
    {
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Use24Hour, true)
            .Add(x => x.Min, new TimeOnly(9, 0))
            .Add(x => x.Max, new TimeOnly(17, 0))
            .Add(x => x.Value, new TimeOnly(10, 0))
            .Add(x => x.AutoClose, autoClose)
            .Add(x => x.ValueChanged, (TimeOnly? v) => changed?.Invoke(v)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut;
    }

    private static void DialType(IRenderedComponent<FlareTimePicker> cut, params string[] keys)
    {
        foreach (var k in keys) cut.Find($".{Css.Classes.ClockDial.Root}").KeyDown(k);
    }

    private static AngleSharp.Dom.IElement TimeOk(IRenderedComponent<FlareTimePicker> cut)
        => cut.FindAll($".{Css.Classes.TimePicker.Actions} button").Last();

    [Fact]
    public void TimePicker_OutOfRangeDialTime_DisablesOk_AndKeepsThePopupOpen()
    {
        var changes = 0;
        var cut = OpenTimePicker(changed: _ => changes++);

        DialType(cut, "2", "0");   // 20:00, past Max

        Assert.True(TimeOk(cut).HasAttribute("disabled"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
        Assert.Equal(0, changes);
    }

    [Fact]
    public void TimePicker_InRangeDialTime_Confirms()
    {
        TimeOnly? committed = null;
        var cut = OpenTimePicker(changed: v => committed = v);

        DialType(cut, "1", "4");   // 14:00
        TimeOk(cut).Click();

        Assert.Equal(new TimeOnly(14, 0), committed);
        Assert.Empty(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
    }

    [Fact]
    public void TimePicker_AutoClose_OutOfRange_StaysOpen()
    {
        var changes = 0;
        var cut = OpenTimePicker(autoClose: true, changed: _ => changes++);

        DialType(cut, "2", "0", "3", "0");   // 20:30, minute completes the entry

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
        Assert.Equal(0, changes);
    }

    [Fact]
    public void DateTimePicker_OutOfRangeValue_DisablesOk_AndKeepsThePopupOpen()
    {
        var changes = 0;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Max, new DateTimeOffset(2026, 10, 15, 12, 0, 0, Msk))
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 13, 0, 0, Msk))
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => changes++));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var ok = cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last();
        Assert.True(ok.HasAttribute("disabled"));

        ok.Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DateTimePicker.Panel}"));
        Assert.Equal(0, changes);
    }

    [Fact]
    public void DateTimePicker_InRangeValue_Confirms()
    {
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Max, new DateTimeOffset(2026, 10, 15, 12, 0, 0, Msk))
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 11, 0, 0, Msk))
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 11, 0, 0, Msk), committed);
        Assert.Empty(cut.FindAll($".{Css.Classes.DateTimePicker.Panel}"));
    }
}
