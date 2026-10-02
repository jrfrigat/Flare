using System.Globalization;
using Bunit;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-107: the date-time popup keeps its own draft. Every opening starts from the current value, a
/// dismissed pick is dropped, and a parent re-render does not wipe a pick in progress.
/// </summary>
public class DateTimePickerDraftTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");
    private static readonly DateTimeOffset Initial = new(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(3));

    private IRenderedComponent<FlareDateTimePicker> RenderPicker(Action<DateTimeOffset?>? changed = null)
        => Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, Initial)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => changed?.Invoke(v)));

    private static void Open(IRenderedComponent<FlareDateTimePicker> cut)
        => cut.Find($".{Css.Classes.Input.Toggle}").Click();

    private static void PickDay(IRenderedComponent<FlareDateTimePicker> cut, int day)
        => cut.Find($"[aria-label='{new DateOnly(2026, 10, day).ToString("D", Ru)}']").Click();

    private static void Ok(IRenderedComponent<FlareDateTimePicker> cut)
        => cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last().Click();

    private static string? SelectedLabel(IRenderedComponent<FlareDateTimePicker> cut)
        => cut.FindAll($".{Css.Classes.Picker.Day}[aria-selected='true']").FirstOrDefault()?.GetAttribute("aria-label");

    [Fact]
    public void DismissedPick_IsDroppedOnTheNextOpening()
    {
        var cut = RenderPicker();
        Open(cut);
        PickDay(cut, 20);
        cut.Find($".{Css.Classes.Picker.Scrim}").Click();

        Open(cut);

        Assert.Equal(new DateOnly(2026, 10, 15).ToString("D", Ru), SelectedLabel(cut));
    }

    [Fact]
    public void DismissedPick_IsNotConfirmedLater()
    {
        DateTimeOffset? committed = null;
        var cut = RenderPicker(v => committed = v);
        Open(cut);
        PickDay(cut, 20);
        cut.Find($".{Css.Classes.Picker.Scrim}").Click();

        Open(cut);
        Ok(cut);

        Assert.Equal(15, committed?.Day);
    }

    [Fact]
    public void ValueClearedFromOutside_OpensWithNothingSelected()
    {
        var cut = RenderPicker();
        cut.Render(p => p.Add(x => x.Value, (DateTimeOffset?)null));

        Open(cut);

        Assert.Null(SelectedLabel(cut));
    }

    [Fact]
    public void ParentRerender_KeepsThePickInProgress()
    {
        DateTimeOffset? committed = null;
        var cut = RenderPicker(v => committed = v);
        Open(cut);
        PickDay(cut, 20);

        cut.Render(p => p.Add(x => x.Value, Initial));   // the parent re-renders with the same value
        Ok(cut);

        Assert.Equal(20, committed?.Day);
        Assert.Equal(12, committed?.Hour);
    }
}
