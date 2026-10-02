using Bunit;
using Flare.Components.Resources;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-130: the dial popup switches to keyboard entry - an hour and a minute text field, AM/PM on a 12-hour
/// clock - as in the MD2 and MD3 specifications. The typed time follows Min/Max like the dial's.
/// </summary>
public class TimePickerKeyboardEntryTests : FlareTestContext
{
    private IRenderedComponent<FlareTimePicker> Open(Action<ComponentParameterCollectionBuilder<FlareTimePicker>>? configure = null,
        Action<TimeOnly?>? changed = null, bool use24Hour = true, TimeOnly? value = null)
    {
        var cut = Render<FlareTimePicker>(p =>
        {
            p.Add(x => x.Use24Hour, use24Hour).Add(x => x.Value, value ?? new TimeOnly(10, 0));
            if (changed is not null) p.Add(x => x.ValueChanged, (TimeOnly? v) => changed(v));
            configure?.Invoke(p);
        });
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut;
    }

    private static void SwitchToKeyboard(IRenderedComponent<FlareTimePicker> cut) =>
        cut.Find($".{Css.Classes.TimePicker.ActionsLead} button").Click();

    private static AngleSharp.Dom.IElement HourInput(IRenderedComponent<FlareTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[0];

    private static AngleSharp.Dom.IElement MinuteInput(IRenderedComponent<FlareTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[1];

    private static AngleSharp.Dom.IElement OkButton(IRenderedComponent<FlareTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.TimePicker.Actions} button").Last();

    [Fact]
    public void Toggle_SwitchesBetweenDialAndEntry()
    {
        var cut = Open();
        Assert.Single(cut.FindAll($".{Css.Classes.ClockDial.Root}"));

        SwitchToKeyboard(cut);
        Assert.Empty(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
        Assert.Equal("10", HourInput(cut).GetAttribute("value"));
        Assert.Equal("00", MinuteInput(cut).GetAttribute("value"));
        Assert.Equal(FlareStrings.TimePicker_EnterTime, cut.Find($".{Css.Classes.TimePicker.PanelHeadline}").TextContent);
        Assert.Equal(FlareStrings.TimePicker_SwitchToDial, cut.Find($".{Css.Classes.TimePicker.ActionsLead} button").GetAttribute("aria-label"));

        SwitchToKeyboard(cut);
        Assert.Single(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
        Assert.Equal(FlareStrings.TimePicker_Headline, cut.Find($".{Css.Classes.TimePicker.PanelHeadline}").TextContent);
    }

    [Fact]
    public void TypedTime_IsConfirmedByOk()
    {
        TimeOnly? committed = null;
        var cut = Open(changed: v => committed = v);
        SwitchToKeyboard(cut);

        HourInput(cut).Input("14");
        MinuteInput(cut).Input("35");
        OkButton(cut).Click();

        Assert.Equal(new TimeOnly(14, 35), committed);
        Assert.Empty(cut.FindAll($".{Css.Classes.TimeEntry.Root}"));
    }

    [Fact]
    public void Enter_ConfirmsTheTypedTime()
    {
        TimeOnly? committed = null;
        var cut = Open(changed: v => committed = v);
        SwitchToKeyboard(cut);

        MinuteInput(cut).Input("7");
        MinuteInput(cut).KeyUp("Enter");

        Assert.Equal(new TimeOnly(10, 7), committed);
    }

    [Fact]
    public void OutOfRangeHour_IsMarkedAndBlocksOk()
    {
        TimeOnly? committed = null;
        var cut = Open(changed: v => committed = v);
        SwitchToKeyboard(cut);

        HourInput(cut).Input("25");

        Assert.Contains(Css.Classes.TimeEntry.InputInvalid, HourInput(cut).ClassList);
        Assert.Equal("true", HourInput(cut).GetAttribute("aria-invalid"));
        Assert.True(OkButton(cut).HasAttribute("disabled"));
        MinuteInput(cut).KeyUp("Enter");
        Assert.Null(committed);

        // Leaving the field shows the time the popup holds again and lets OK through.
        HourInput(cut).Blur();
        Assert.Equal("10", HourInput(cut).GetAttribute("value"));
        Assert.DoesNotContain(Css.Classes.TimeEntry.InputInvalid, HourInput(cut).ClassList);
        Assert.False(OkButton(cut).HasAttribute("disabled"));
    }

    [Fact]
    public void NonDigits_AreDropped()
    {
        var cut = Open();
        SwitchToKeyboard(cut);

        HourInput(cut).Input("1a");

        Assert.Equal("1", HourInput(cut).GetAttribute("value"));
    }

    [Fact]
    public void TwelveHourEntry_UsesThePeriod()
    {
        TimeOnly? committed = null;
        var cut = Open(changed: v => committed = v, use24Hour: false, value: new TimeOnly(9, 0));
        SwitchToKeyboard(cut);
        Assert.Equal("09", HourInput(cut).GetAttribute("value"));

        cut.FindAll($".{Css.Classes.TimeEntry.Root} .{Css.Classes.ClockDial.PeriodBtn}")[1].Click();   // PM
        HourInput(cut).Input("12");
        OkButton(cut).Click();
        Assert.Equal(new TimeOnly(12, 0), committed);

        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        SwitchToKeyboard(cut);
        HourInput(cut).Input("13");
        Assert.Contains(Css.Classes.TimeEntry.InputInvalid, HourInput(cut).ClassList);
        HourInput(cut).Input("0");
        Assert.Contains(Css.Classes.TimeEntry.InputInvalid, HourInput(cut).ClassList);
    }

    [Fact]
    public void TimeOutsideMinMax_BlocksOk_AsOnTheDial()
    {
        var cut = Open(p => p.Add(x => x.Min, new TimeOnly(9, 0)));
        SwitchToKeyboard(cut);

        HourInput(cut).Input("08");

        Assert.True(OkButton(cut).HasAttribute("disabled"));
    }

    [Fact]
    public void Toggle_IsHiddenWhenTurnedOff_AndInTheDropdown()
    {
        var off = Open(p => p.Add(x => x.ShowKeyboardToggle, false));
        Assert.Empty(off.FindAll($".{Css.Classes.TimePicker.ActionsLead}"));

        var dropdown = Open(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown));
        Assert.Empty(dropdown.FindAll($".{Css.Classes.TimePicker.ActionsLead}"));
    }

    [Fact]
    public void EveryOpening_StartsOnTheDial()
    {
        var cut = Open();
        SwitchToKeyboard(cut);
        cut.FindAll($".{Css.Classes.TimePicker.Actions} button")[1].Click();   // Cancel

        cut.Find($".{Css.Classes.Input.Toggle}").Click();

        Assert.Single(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
    }
}
