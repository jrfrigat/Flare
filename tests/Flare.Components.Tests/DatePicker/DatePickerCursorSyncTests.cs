using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-602: a date committed from the field becomes the calendar's keyboard cursor. The roving tabindex,
/// the initial focus of an opened popup and the day the arrow keys start from follow the accepted value,
/// not the cursor left over from an earlier pick; a value set from outside anchors the cursor the same way.
/// </summary>
public class DatePickerCursorSyncTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    private static string DayLabel(DateOnly d) => d.ToString("D", Ru);
    private static string ToggleSelector => $".{Css.Classes.Input.Toggle}";
    private static string InitialFocusSelector => "[data-flare-initial-focus='true']";

    [Fact]
    public void TypedCommit_MovesTheInitialCalendarFocusToTheAcceptedDate()
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        PickDay(cut, new DateOnly(2026, 10, 16));   // the cursor is left on the picked day
        cut.Find("input").Focus();
        cut.Find("input").Input("20102026");        // a full mask commits at once
        Assert.Equal(new DateOnly(2026, 10, 20), committed);
        cut.Render(p => p.Add(x => x.Value, committed));   // the parent binds the value back

        cut.Find(ToggleSelector).Click();

        var cursor = Assert.Single(cut.FindAll(InitialFocusSelector));
        Assert.Equal(DayLabel(new DateOnly(2026, 10, 20)), cursor.GetAttribute("aria-label"));
        Assert.Equal("0", cursor.GetAttribute("tabindex"));
        Assert.Equal("true", cursor.GetAttribute("aria-selected"));
        Assert.Single(cut.FindAll($".{Css.Classes.Picker.Day}[tabindex='0']"));
    }

    [Fact]
    public void AfterTypedCommit_ArrowKeysStartFromTheAcceptedDate()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        PickDay(cut, new DateOnly(2026, 10, 16));
        cut.Find("input").Focus();
        cut.Find("input").Input("20102026");
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2026, 10, 20)));
        cut.Find(ToggleSelector).Click();

        var next = new DateOnly(2026, 10, 21);
        var expectedId = cut.Find($"[aria-label='{DayLabel(next)}']").GetAttribute("blazor:elementreference");
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");

        var target = cut.Find($".{Css.Classes.Picker.Day}[tabindex='0']");
        Assert.Equal(DayLabel(next), target.GetAttribute("aria-label"));
        var focus = JSInterop.Invocations.Last(i => i.Identifier == "Blazor._internal.domWrapper.focus");
        var focused = Assert.IsType<ElementReference>(focus.Arguments[0]);
        Assert.Equal(expectedId, focused.Id);
    }

    [Fact]
    public void ExternalValue_ReanchorsTheCursorWhenThePopupReopens()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find(ToggleSelector).Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");   // the cursor moves to the 16th
        cut.Find($".{Css.Classes.Picker.Scrim}").Click();
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2026, 10, 20)));
        cut.Find(ToggleSelector).Click();

        var cursor = Assert.Single(cut.FindAll(InitialFocusSelector));
        Assert.Equal(DayLabel(new DateOnly(2026, 10, 20)), cursor.GetAttribute("aria-label"));
    }

    // A pick sets the host cursor through FocusedDateChanged, which is the state the text commit used to leave behind.
    private static void PickDay(IRenderedComponent<FlareDatePicker> cut, DateOnly day)
    {
        cut.Find(ToggleSelector).Click();
        cut.Find($"[aria-label='{DayLabel(day)}']").Click();
    }
}
