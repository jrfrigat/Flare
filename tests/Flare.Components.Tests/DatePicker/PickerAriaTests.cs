namespace Flare.Components.Tests;

/// <summary>
/// TASK-106: the picker popup is a dialog, the day grid follows the ARIA grid pattern (rows, full-date
/// names, one tabbable cell) and the clock dial announces its range.
/// </summary>
public class PickerAriaTests : FlareTestContext
{
    [Fact]
    public void MonthGrid_HasARowPerWeek_FullDateName_AndOneTabbableCell()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        Assert.Equal(6, cut.FindAll($".{Css.Classes.Picker.Row}[role='row']").Count);
        Assert.Single(cut.FindAll($".{Css.Classes.Picker.Day}[tabindex='0']"));

        var cell = cut.Find($".{Css.Classes.Picker.Day}[role='gridcell']");
        Assert.Equal("row", cell.ParentElement?.GetAttribute("role"));

        var selected = cut.Find($".{Css.Classes.Picker.Day}[aria-selected='true']");
        Assert.Contains("2026", selected.GetAttribute("aria-label") ?? string.Empty);
    }

    [Fact]
    public void Popup_HasDialogRole_AndEscapeClosesIt()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal("dialog", cut.Find($".{Css.Classes.Picker.Panel}").GetAttribute("role"));

        cut.Find($".{Css.Classes.Picker.Grid}").KeyDown("Escape");

        Assert.Empty(cut.FindAll($".{Css.Classes.Picker.Panel}"));
    }

    // TASK-127: a 12-hour dial shows 1-12, so it announces that range and the displayed hour.
    [Fact]
    public void ClockDial_TwelveHour_AnnouncesTheDisplayedHourInOneToTwelve()
    {
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Hour, 15));

        var dial = cut.Find($".{Css.Classes.ClockDial.Dial}");
        Assert.Equal("1", dial.GetAttribute("aria-valuemin"));
        Assert.Equal("12", dial.GetAttribute("aria-valuemax"));
        Assert.Equal("3", dial.GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void ClockDial_TwentyFourHour_AnnouncesZeroToTwentyThree()
    {
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Is24Hour, true).Add(x => x.Hour, 15));

        var dial = cut.Find($".{Css.Classes.ClockDial.Dial}");
        Assert.Equal("0", dial.GetAttribute("aria-valuemin"));
        Assert.Equal("23", dial.GetAttribute("aria-valuemax"));
        Assert.Equal("15", dial.GetAttribute("aria-valuenow"));
    }
}
