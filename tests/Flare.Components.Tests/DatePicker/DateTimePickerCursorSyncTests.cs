using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-618, the date-time sibling of the date picker's cursor sync: a value committed from the field
/// becomes the calendar cursor, so the roving tabindex, the initial focus of the reopened popup and the
/// arrow keys follow the accepted value instead of a day left over from an earlier pick.
/// </summary>
public class DateTimePickerCursorSyncTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");

    private static readonly DateTimeOffset Initial =
        new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(5));

    private static string DayLabel(DateOnly d) => d.ToString("D", Ru);
    private static string ToggleSelector => $".{Css.Classes.Input.Toggle}";
    private static string InitialFocusSelector => "[data-flare-initial-focus='true']";

    [Fact]
    public void TypedCommit_MovesTheReopenedCalendarToTheAcceptedDate()
    {
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.Value, Initial)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        // Leave the cursor on another day, then close without committing it.
        cut.Find(ToggleSelector).Click();
        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("ArrowRight");
        cut.Find($".{Css.Classes.Picker.Scrim}").Click();

        cut.Find("input").Focus();
        cut.Find("input").Change("20.10.2026 14:30");
        Assert.Equal(new DateOnly(2026, 10, 20), DateOnly.FromDateTime(committed!.Value.DateTime));

        cut.Render(p => p
            .Add(x => x.Culture, Ru)
            .Add(x => x.Mode, DateTimeVariant.Panels)
            .Add(x => x.Value, committed)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));
        cut.Find(ToggleSelector).Click();

        var cursor = Assert.Single(cut.FindAll(InitialFocusSelector));
        Assert.Equal(DayLabel(new DateOnly(2026, 10, 20)), cursor.GetAttribute("aria-label"));
        Assert.Equal("0", cursor.GetAttribute("tabindex"));
        Assert.Single(cut.FindAll($".{Css.Classes.Picker.Day}[tabindex='0']"));
    }
}
