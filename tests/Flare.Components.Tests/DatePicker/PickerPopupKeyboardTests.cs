using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-134: Escape anywhere in a date, time or date-time popup closes it without committing, and the
/// popup holds Tab inside through the shared focus trap while it is open.
/// </summary>
public class PickerPopupKeyboardTests : FlareTestContext
{
    private static readonly KeyboardEventArgs Escape = new() { Key = "Escape" };

    private int Calls(string identifier) => JSInterop.Invocations.Count(i => i.Identifier == identifier);

    [Theory]
    [InlineData(TimePickerVariant.Dial)]
    [InlineData(TimePickerVariant.Dropdown)]
    public void TimePicker_EscapeFromThePopup_ClosesWithoutCommitting(TimePickerVariant variant)
    {
        var calls = 0;
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, variant)
            .Add(x => x.Value, new TimeOnly(9, 30)).Add(x => x.ValueChanged, (TimeOnly? _) => calls++));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var area = variant == TimePickerVariant.Dial ? $".{Css.Classes.ClockDial.Root}" : $".{Css.Classes.TimePicker.Columns}";
        cut.Find(area).KeyDown("5");
        cut.Find(area).KeyDown(Escape);

        Assert.Empty(cut.FindAll("[role='dialog']"));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void DateTimePicker_EscapeFromTheNavigationAndTimePane_Closes()
    {
        var calls = 0;
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Mode, DateTimeVariant.Tabs)
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 9, 30, 0, TimeSpan.Zero))
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => calls++));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Nav} button").Last().KeyDown(Escape);
        Assert.Empty(cut.FindAll("[role='dialog']"));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.Tab}")[1].Click();
        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[0].Change("11");
        cut.FindAll($".{Css.Classes.DateTimePicker.TimeInput}")[0].KeyDown(Escape);

        Assert.Empty(cut.FindAll("[role='dialog']"));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void DatePicker_EscapeFromTheHeader_Closes()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DatePicker.Header} button").First().KeyDown(Escape);

        Assert.Empty(cut.FindAll("[role='dialog']"));
    }

    [Fact]
    public void InlineDatePicker_IgnoresEscapeAndSetsNoTrap()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find($".{Css.Classes.DatePicker.Header} button").KeyDown(Escape);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Picker.Grid}"));
        Assert.Equal(0, Calls("trapFocus"));
    }

    [Fact]
    public void Popups_TrapFocusWhileOpenAndReleaseItOnClose()
    {
        var date = Render<FlareDatePicker>();
        var time = Render<FlareTimePicker>();
        var dateTime = Render<FlareDateTimePicker>();

        date.Find($".{Css.Classes.Input.Toggle}").Click();
        time.Find($".{Css.Classes.Input.Toggle}").Click();
        dateTime.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal(3, Calls("trapFocus"));

        date.Find($".{Css.Classes.Picker.Scrim}").Click();
        time.Find($".{Css.Classes.Picker.Scrim}").Click();
        dateTime.Find($".{Css.Classes.Picker.Scrim}").Click();
        Assert.Equal(3, Calls("releaseFocusTrap"));
    }
}
