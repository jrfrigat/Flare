namespace Flare.Components.Tests;

/// <summary>
/// TASK-174: the column popup is a listbox per unit the keyboard fully drives (columns, steps over the offered cells,
/// Home/End, the seconds column, Enter on key-up), and the clock dial is a slider its keys move.
/// </summary>
public class TimePickerKeyboardTests : FlareTestContext
{
    private IRenderedComponent<FlareTimePicker> OpenColumns(TimeOnly value, Action<TimeOnly?> changed, bool seconds = false,
        int minuteStep = 1, TimeOnly? max = null)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.PopupVariant, TimePickerVariant.Dropdown).Add(x => x.Use24Hour, true)
            .Add(x => x.ShowSeconds, seconds).Add(x => x.MinuteStep, minuteStep).Add(x => x.Max, max)
            .Add(x => x.Value, value).Add(x => x.ValueChanged, (TimeOnly? v) => changed(v)));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        return cut;
    }

    private static void Keys(IRenderedComponent<FlareTimePicker> cut, params string[] keys)
    {
        foreach (var k in keys) cut.Find($".{Css.Classes.TimePicker.Columns}").KeyDown(k);
    }

    private static void Ok(IRenderedComponent<FlareTimePicker> cut) =>
        cut.FindAll($".{Css.Classes.TimePicker.Actions} button").Last().Click();

    [Fact]
    public void SecondsColumn_IsReachedAndTyped()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 0, 0), v => committed = v, seconds: true);
        Keys(cut, "ArrowRight", "ArrowRight", "4", "5");
        Ok(cut);
        Assert.Equal(new TimeOnly(10, 0, 45), committed);
    }

    [Fact]
    public void TypedMinute_MovesOnToTheSeconds()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 0, 0), v => committed = v, seconds: true);
        Keys(cut, "1", "2", "3", "4", "5", "6");
        Ok(cut);
        Assert.Equal(new TimeOnly(12, 34, 56), committed);
    }

    [Fact]
    public void ArrowsStepTheActiveColumn_ByItsStep()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 15), v => committed = v, minuteStep: 15);
        Keys(cut, "ArrowDown", "ArrowRight", "ArrowDown", "ArrowDown", "ArrowUp");
        Ok(cut);
        Assert.Equal(new TimeOnly(11, 30), committed);
    }

    [Fact]
    public void Arrows_StopAtTheEnds_AndSkipCellsPastMax()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(16, 0), v => committed = v, max: new TimeOnly(17, 0));
        Keys(cut, "ArrowDown", "ArrowDown", "ArrowDown");
        Ok(cut);
        Assert.Equal(new TimeOnly(17, 0), committed);
    }

    [Fact]
    public void HomeAndEnd_JumpToTheFirstAndLastCell()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 20), v => committed = v, minuteStep: 5);
        Keys(cut, "End", "ArrowRight", "Home");
        Ok(cut);
        Assert.Equal(new TimeOnly(23, 0), committed);
    }

    [Fact]
    public void ArrowSelection_IsAnnouncedAsTheActiveCell()
    {
        var cut = OpenColumns(new TimeOnly(10, 0), _ => { });
        Keys(cut, "ArrowDown");
        var columns = cut.Find($".{Css.Classes.TimePicker.Columns}");
        Assert.Equal("11", cut.Find($"#{columns.GetAttribute("aria-activedescendant")}").TextContent.Trim());
    }

    [Fact]
    public void EnterOnKeyUp_Confirms()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 0), v => committed = v);
        Keys(cut, "ArrowDown");
        Keys(cut, "Enter");
        Assert.Null(committed);
        Assert.Single(cut.FindAll("[role=dialog]"));
        cut.Find($".{Css.Classes.TimePicker.Columns}").KeyUp("Enter");
        Assert.Equal(new TimeOnly(11, 0), committed);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void OpeningEnterRelease_DoesNotConfirmColumns()
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 0), v => committed = v);
        cut.Find($".{Css.Classes.TimePicker.Columns}").KeyUp("Enter");
        Assert.Null(committed);
        Assert.Single(cut.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData("Escape")]
    [InlineData("Toggle")]
    [InlineData("Api")]
    public async Task CancelledEnter_DoesNotConfirmTheNextOpening(string close)
    {
        TimeOnly? committed = null;
        var cut = OpenColumns(new TimeOnly(10, 0), v => committed = v);
        Keys(cut, "Enter");
        if (close == "Api") await cut.InvokeAsync(() => cut.Instance.CloseAsync());
        else if (close == "Toggle") cut.Find($".{Css.Classes.Input.Toggle}").Click();
        else cut.Find("[role=dialog]").KeyDown("Escape");
        Assert.Empty(cut.FindAll("[role=dialog]"));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.Find($".{Css.Classes.TimePicker.Columns}").KeyUp("Enter");
        Assert.Null(committed);
        Assert.Single(cut.FindAll("[role=dialog]"));
        Keys(cut, "ArrowDown", "Enter");
        cut.Find($".{Css.Classes.TimePicker.Columns}").KeyUp("Enter");
        Assert.Equal(new TimeOnly(11, 0), committed);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Theory]
    [InlineData("ArrowUp", 11, 30)]
    [InlineData("ArrowDown", 9, 30)]
    [InlineData("PageUp", 11, 30)]
    [InlineData("Home", 0, 30)]
    [InlineData("End", 23, 30)]
    public void Dial_HourKeys(string key, int hour, int minute)
    {
        int h = 10, m = 30;
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Hour, 10).Add(x => x.Minute, 30).Add(x => x.Is24Hour, true)
            .Add(x => x.HourChanged, (int v) => h = v).Add(x => x.MinuteChanged, (int v) => m = v));
        cut.Find("[role=slider]").KeyDown(key);
        Assert.Equal((hour, minute), (h, m));
    }

    [Theory]
    [InlineData("ArrowUp", 31)]
    [InlineData("ArrowDown", 29)]
    [InlineData("PageUp", 35)]
    [InlineData("PageDown", 25)]
    [InlineData("End", 59)]
    public void Dial_MinuteKeys(string key, int minute)
    {
        int m = 30;
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Hour, 10).Add(x => x.Minute, 30).Add(x => x.Is24Hour, true)
            .Add(x => x.MinuteChanged, (int v) => m = v));
        cut.Find("[role=slider]").KeyDown("ArrowRight");
        cut.Find("[role=slider]").KeyDown(key);
        Assert.Equal(minute, m);
    }

    [Fact]
    public void ColumnCells_SayTrueOrFalseForSelected()
    {
        var cut = OpenColumns(new TimeOnly(10, 0), _ => { });
        var hours = cut.FindAll($".{Css.Classes.TimePicker.Col}")[0].QuerySelectorAll("[role=option]");
        Assert.Equal("true", hours.First(b => b.TextContent.Trim() == "10").GetAttribute("aria-selected"));
        Assert.Equal("false", hours.First(b => b.TextContent.Trim() == "11").GetAttribute("aria-selected"));
    }

    // The popup toggles of all three pickers state aria-expanded as true/false, not as an empty boolean attribute.
    [Fact]
    public void Toggles_SayTrueOrFalseForExpanded()
    {
        var toggles = new Func<AngleSharp.Dom.IElement>[]
        {
            Toggle(Render<FlareTimePicker>()), Toggle(Render<FlareDatePicker>()), Toggle(Render<FlareDateTimePicker>()),
        };
        Assert.All(toggles, t => Assert.Equal("false", t().GetAttribute("aria-expanded")));
        foreach (var t in toggles) t().Click();
        Assert.All(toggles, t => Assert.Equal("true", t().GetAttribute("aria-expanded")));
    }

    private static Func<AngleSharp.Dom.IElement> Toggle<T>(IRenderedComponent<T> cut) where T : Microsoft.AspNetCore.Components.IComponent =>
        () => cut.Find($".{Css.Classes.Input.Toggle}");

    [Fact]
    public void Dial_IsAFocusableSlider_WithAReadableValue()
    {
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Hour, 14).Add(x => x.Minute, 5).Add(x => x.Is24Hour, false)
            .Add(x => x.Culture, System.Globalization.CultureInfo.GetCultureInfo("en-US")));
        var slider = cut.Find("[role=slider]");
        Assert.Equal("0", slider.GetAttribute("tabindex"));
        Assert.Equal("2 PM", slider.GetAttribute("aria-valuetext"));
        Assert.Null(cut.Find($".{Css.Classes.ClockDial.Root}").GetAttribute("tabindex"));
        var period = cut.FindAll($".{Css.Classes.ClockDial.PeriodBtn}");
        Assert.Equal(new[] { "false", "true" }, period.Select(b => b.GetAttribute("aria-pressed")));
    }
}
