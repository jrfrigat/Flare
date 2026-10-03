using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-184: the month picker commits the first day of a month - picked from the shared month and year views,
/// typed on a month mask in the culture's order, or the current month - on the culture's calendar, honouring
/// Min/Max and the locked states; the shared grid is driven by the keyboard.
/// </summary>
public class MonthPickerTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

    private IRenderedComponent<FlareMonthPicker> RenderPicker(Action<DateOnly?> changed, DateOnly? value = null,
        CultureInfo? culture = null, DateOnly? min = null, DateOnly? max = null, bool inline = false) =>
        Render<FlareMonthPicker>(p => p.Add(x => x.Culture, culture ?? Ru).Add(x => x.Value, value).Add(x => x.Min, min)
            .Add(x => x.Max, max).Add(x => x.Inline, inline).Add(x => x.ValueChanged, (DateOnly? v) => changed(v)));

    private static AngleSharp.Dom.IElement[] Months(IRenderedComponent<FlareMonthPicker> cut) =>
        cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}").ToArray();

    [Fact]
    public void PickingAMonth_CommitsItsFirstDay_AndCloses()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 3, 17));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        Assert.Equal("true", Months(cut)[2].GetAttribute("aria-pressed"));
        Months(cut)[9].Click();
        Assert.Equal(new DateOnly(2026, 10, 1), committed);
        Assert.Empty(cut.FindAll("[role=dialog]"));
    }

    [Fact]
    public void Field_ShowsTheMonth_AndTakesTypedDigits()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 3, 1));
        Assert.Equal(new DateOnly(2026, 3, 1).ToString(Ru.DateTimeFormat.YearMonthPattern, Ru), cut.Find("input").GetAttribute("value"));
        cut.Find("input").Input("102026");
        Assert.Equal(new DateOnly(2026, 10, 1), committed);
    }

    [Theory]
    [InlineData("ja-JP", "yyyy/MM", "202610")]
    [InlineData("en-US", "MM/yyyy", "102026")]
    public void MonthMask_FollowsTheCultureOrder(string name, string pattern, string typed)
    {
        var culture = CultureInfo.GetCultureInfo(name);
        Assert.Equal(pattern, MaskedInput.NumericMonthPattern(culture, culture.DateTimeFormat.DateSeparator));
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, culture: culture);
        cut.Find("input").Input(typed);
        Assert.Equal(new DateOnly(2026, 10, 1), committed);
    }

    [Fact]
    public void MinAndMax_DisableMonths_AndStopATypedOne()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 6, 1), min: new DateOnly(2026, 4, 20), max: new DateOnly(2026, 8, 1));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        var enabled = Months(cut).Select((b, i) => (b, m: i + 1)).Where(t => !t.b.HasAttribute("disabled")).Select(t => t.m);
        Assert.Equal(new[] { 4, 5, 6, 7, 8 }, enabled);
        cut.Find("input").Input("012026");
        Assert.Null(committed);
    }

    [Fact]
    public void YearView_PicksAYear_ThenItsMonths()
    {
        DateOnly? committed = null;
        var cut = Render<FlareMonthPicker>(p => p.Add(x => x.Culture, Ru).Add(x => x.OpenTo, PickerOpenTo.Year)
            .Add(x => x.Value, new DateOnly(2026, 1, 1)).Add(x => x.ValueChanged, (DateOnly? v) => committed = v));
        cut.Find($".{Css.Classes.Input.Toggle}").Click();
        cut.FindAll($".{Css.Classes.DatePicker.YearBtn}").First(b => b.TextContent.Trim() == "2020").Click();
        Months(cut)[4].Click();
        Assert.Equal(new DateOnly(2020, 5, 1), committed);
    }

    [Fact]
    public void Keyboard_MovesOverTheGrid_AndSkipsDisabledMonths()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 5, 1), max: new DateOnly(2026, 9, 30), inline: true);
        string Tabbable() => cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}[tabindex='0']").Single().TextContent.Trim();
        var grid = () => cut.Find($".{Css.Classes.DatePicker.MonthGrid}");
        var may = Tabbable();
        grid().KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        grid().KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal(Months(cut)[8].TextContent.Trim(), Tabbable()); // June + 3 = September
        grid().KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal(Months(cut)[8].TextContent.Trim(), Tabbable()); // December is past Max
        grid().KeyDown(new KeyboardEventArgs { Key = "End" });
        Assert.Equal(Months(cut)[8].TextContent.Trim(), Tabbable());
        grid().KeyDown(new KeyboardEventArgs { Key = "Home" });
        Assert.Equal(Months(cut)[0].TextContent.Trim(), Tabbable());
        Assert.NotEqual(may, Tabbable());
    }

    [Fact]
    public void PageKeys_PageTheYear()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 5, 1), inline: true);
        cut.Find($".{Css.Classes.DatePicker.MonthGrid}").KeyDown(new KeyboardEventArgs { Key = "PageDown" });
        Assert.Equal("2027", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
    }

    [Fact]
    public void PagingTheYear_KeepsTheKeyboardMonth()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 5, 1), inline: true);
        var grid = () => cut.Find($".{Css.Classes.DatePicker.MonthGrid}");
        grid().KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        grid().KeyDown(new KeyboardEventArgs { Key = "PageDown" });
        Assert.Equal(Months(cut)[5].TextContent.Trim(),
            cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}[tabindex='0']").Single().TextContent.Trim());
    }

    [Fact]
    public void WithNoValue_TheCurrentMonthIsMarked_AndTabbable()
    {
        var cut = RenderPicker(_ => { }, inline: true);
        var current = cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}[aria-current=date]").Single();
        Assert.Equal("0", current.GetAttribute("tabindex"));
        Assert.Equal(Months(cut)[DateTime.Now.Month - 1].TextContent, current.TextContent);
    }

    [Fact]
    public void ReadOnly_BrowsesButCommitsNothing_DisabledTurnsAllOff()
    {
        var published = false;
        var ro = Render<FlareMonthPicker>(p => p.Add(x => x.Inline, true).Add(x => x.ReadOnly, true).Add(x => x.Culture, Ru)
            .Add(x => x.ValueChanged, (DateOnly? _) => published = true));
        Months(ro)[3].Click();
        ro.FindAll($".{Css.Classes.DatePicker.Footer} button").Last().Click();
        Assert.False(published);
        Assert.False(ro.FindAll($".{Css.Classes.DatePicker.Header} button[aria-label]").Last().HasAttribute("disabled"));

        var off = Render<FlareMonthPicker>(p => p.Add(x => x.Inline, true).Add(x => x.Disabled, true).Add(x => x.Culture, Ru));
        Assert.All(Months(off), b => Assert.True(b.HasAttribute("disabled")));
        Assert.All(off.FindAll($".{Css.Classes.DatePicker.Footer} button"), b => Assert.True(b.HasAttribute("disabled")));
    }

    [Fact]
    public void ThisMonth_PicksTheCurrentMonth()
    {
        DateOnly? committed = null;
        var cut = RenderPicker(v => committed = v, inline: true);
        cut.FindAll($".{Css.Classes.DatePicker.Footer} button").Last().Click();
        var now = DateTime.Now;
        Assert.Equal(new DateOnly(now.Year, now.Month, 1), committed);
    }

    [Fact]
    public void OutsideValue_ShowsItsYear()
    {
        var cut = RenderPicker(_ => { }, new DateOnly(2026, 5, 1), inline: true);
        cut.Render(p => p.Add(x => x.Value, new DateOnly(2031, 2, 1)));
        Assert.Equal("2031", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
    }

    [Fact]
    public void Persian_PicksAPersianMonth()
    {
        DateOnly? committed = null;
        var fa = CultureInfo.GetCultureInfo("fa-IR");
        var cut = RenderPicker(v => committed = v, new DateOnly(2026, 10, 3), fa, inline: true);
        Assert.Equal("1405", cut.Find($".{Css.Classes.DatePicker.MonthLabel}").TextContent.Trim());
        Months(cut)[7].Click(); // Aban
        Assert.Equal(new DateOnly(2026, 10, 23), committed);
    }

    [Fact]
    public void Hebrew_LeapYear_HasThirteenMonths()
    {
        var cut = Render<FlareMonthPicker>(p => p.Add(x => x.Inline, true).Add(x => x.Culture, CultureInfo.GetCultureInfo("he-IL"))
            .Add(x => x.Calendar, new HebrewCalendar()).Add(x => x.Value, new DateOnly(2026, 10, 3)));
        Assert.Equal(13, Months(cut).Length);
    }

    [Fact]
    public void DatePickerMonthView_HasTheKeyboardToo()
    {
        var cut = Render<FlareDatePicker>(p => p.Add(x => x.Inline, true).Add(x => x.Value, new DateOnly(2026, 5, 1))
            .Add(x => x.OpenTo, PickerOpenTo.Month).Add(x => x.Culture, Ru));
        cut.Find($".{Css.Classes.DatePicker.MonthGrid}").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Assert.Equal(cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}")[5].TextContent.Trim(),
            cut.FindAll($".{Css.Classes.DatePicker.MonthBtn}[tabindex='0']").Single().TextContent.Trim());
    }
}
