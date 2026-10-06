using System.Globalization;
using Flare.Components.Resources;

namespace Flare.Components.Tests;

public class DateTimePickerKeyboardEntryTests : FlareTestContext
{
    private static readonly DateTimeOffset Initial =
        new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(5)).AddTicks(1234567);

    private IRenderedComponent<FlareDateTimePicker> Open(DateTimeVariant mode,
        Action<DateTimeOffset?>? changed = null, bool use24Hour = true)
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("en-US"))
            .Add(x => x.Mode, mode).Add(x => x.TimeVariant, TimePickerVariant.Dial)
            .Add(x => x.Use24Hour, use24Hour).Add(x => x.Value, Initial)
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => changed?.Invoke(v)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        if (mode == DateTimeVariant.Tabs) cut.FindAll("[role=tab]")[1].Click();
        return cut;
    }

    private static void Switch(IRenderedComponent<FlareDateTimePicker> cut) =>
        cut.Find($".{Css.Classes.DateTimePicker.Footer} button[aria-label]").Click();

    private static AngleSharp.Dom.IElement Ok(IRenderedComponent<FlareDateTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button").Last();

    [Theory]
    [InlineData(DateTimeVariant.Tabs)]
    [InlineData(DateTimeVariant.Panels)]
    public void Entry_PreservesDraftOffsetAndHiddenPrecision(DateTimeVariant mode)
    {
        DateTimeOffset? committed = null;
        var cut = Open(mode, v => committed = v);
        Switch(cut);
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[0].Input("16");
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[1].Input("37");
        Switch(cut);
        Assert.Single(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
        Switch(cut);
        Assert.Equal("16", cut.FindAll($".{Css.Classes.TimeEntry.Input}")[0].GetAttribute("value"));
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[1].KeyUp("Enter");
        Assert.Equal(Initial.AddHours(2).AddMinutes(7), committed);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void InvalidEntry_BlocksConfirmAndSwitchingBackClearsIt()
    {
        var calls = 0;
        var cut = Open(DateTimeVariant.Panels, _ => calls++);
        Switch(cut);
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[0].Input("25");
        Assert.True(Ok(cut).HasAttribute("disabled"));
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[1].KeyUp("Enter");
        Assert.Equal(0, calls);
        Switch(cut);
        Assert.False(Ok(cut).HasAttribute("disabled"));
        Ok(cut).Click();
        Assert.Equal(1, calls);
    }

    [Fact]
    public void TwelveHourEntry_ChangesPeriod()
    {
        DateTimeOffset? committed = null;
        var cut = Open(DateTimeVariant.Panels, v => committed = v, false);
        Switch(cut);
        cut.FindAll($".{Css.Classes.TimeEntry.Root} .{Css.Classes.ClockDial.PeriodBtn}")[0].Click();
        cut.FindAll($".{Css.Classes.TimeEntry.Input}")[0].Input("12");
        Ok(cut).Click();
        Assert.Equal(Initial.AddHours(-14), committed);
    }

    [Fact]
    public void Reopening_StartsOnDialAndLockedFieldCannotSwitch()
    {
        var cut = Open(DateTimeVariant.Panels);
        Switch(cut);
        cut.Find($".{Css.Classes.Picker.Scrim}").Click();
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Single(cut.FindAll($".{Css.Classes.ClockDial.Root}"));
        cut.Render(p => p.Add(x => x.ReadOnly, true));
        Assert.True(cut.Find($".{Css.Classes.DateTimePicker.Footer} button[aria-label]").HasAttribute("disabled"));
        Assert.Empty(cut.FindAll($".{Css.Classes.TimeEntry.Root}"));
    }

    [Fact]
    public void CalendarTabAndNumberPane_DoNotShowSwitch()
    {
        var cut = Open(DateTimeVariant.Tabs);
        cut.FindAll("[role=tab]")[0].Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button[aria-label]"));
        cut.Render(p => p.Add(x => x.TimeVariant, TimePickerVariant.Dropdown));
        cut.FindAll("[role=tab]")[1].Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.DateTimePicker.Footer} button[aria-label]"));
    }
}
