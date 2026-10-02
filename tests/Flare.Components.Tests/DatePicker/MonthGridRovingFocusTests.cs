using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-124: the calendar grid keeps DOM focus on the keyboard cursor, so the native click of Enter/Space
/// selects the day the cursor shows, and exactly one available day of the displayed month is tabbable.
/// </summary>
public class MonthGridRovingFocusTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");

    private static string Label(DateOnly d) => d.ToString("D", Ru);

    [Fact]
    public void ArrowKey_MovesDomFocusOntoTheNewCursor()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        // Blazor stamps the reference id only when the element is created, so read it before the key press.
        var nextLabel = Label(new DateOnly(2026, 10, 16));
        var expectedId = cut.Find($"[aria-label='{nextLabel}']").GetAttribute("blazor:elementreference");

        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");

        var target = cut.Find($".{Css.Classes.Picker.Day}[tabindex='0']");
        Assert.Equal(nextLabel, target.GetAttribute("aria-label"));
        var focused = Assert.IsType<ElementReference>(JSInterop.VerifyFocusAsyncInvoke().Arguments[0]);
        Assert.Equal(expectedId, focused.Id);
    }

    [Fact]
    public void ArrowsMoveOnlyTheCursor_AndEnterIsLeftToTheNativeClick()
    {
        var changes = 0;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? _) => changes++));

        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("Enter");

        Assert.Equal(0, changes);
    }

    [Fact]
    public void CursorOutsideTheDisplayedMonth_FallsBackToAnAvailableDay()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2000, 1, 15)));

        // Next month: neither the value nor today is on screen.
        cut.FindAll($".{Css.Classes.DatePicker.Header} button").Last().Click();

        var target = Assert.Single(cut.FindAll($".{Css.Classes.Picker.Day}[tabindex='0']"));
        Assert.Equal(Label(new DateOnly(2000, 2, 1)), target.GetAttribute("aria-label"));
    }

    [Fact]
    public void DisabledCursor_FallsBackToTheFirstAvailableDay()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Culture, Ru)
            .Add(x => x.Min, new DateOnly(2000, 1, 20))
            .Add(x => x.Value, new DateOnly(2000, 1, 15)));

        var target = Assert.Single(cut.FindAll($".{Css.Classes.Picker.Day}[tabindex='0']"));
        Assert.Equal(Label(new DateOnly(2000, 1, 20)), target.GetAttribute("aria-label"));
        Assert.False(target.HasAttribute("disabled"));
    }

    [Fact]
    public void DateTimePicker_ArrowKeys_SkipUnavailableDays()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 0, 0, TimeSpan.FromHours(3)))
            .Add(x => x.Max, new DateTimeOffset(2026, 10, 15, 23, 0, 0, TimeSpan.FromHours(3))));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");

        var selected = cut.Find($".{Css.Classes.Picker.Day}[aria-selected='true']");
        Assert.Equal(Label(new DateOnly(2026, 10, 15)), selected.GetAttribute("aria-label"));
    }
}
