using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-172: on a 12-hour clock the field shows and takes "hh:mm AM" with the culture's designators - a typed a/p picks
/// the period, a hour past 12 is refused - and the Culture parameter sets the default clock and the designators.
/// </summary>
public class TimePickerTwelveHourTests : FlareTestContext
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

    private IRenderedComponent<FlareTimePicker> RenderTwelve(TimeOnly? value, Action<TimeOnly?> changed,
        CultureInfo? culture = null, bool seconds = false) =>
        Render<FlareTimePicker>(p => p.Add(x => x.Use24Hour, false).Add(x => x.Culture, culture ?? En)
            .Add(x => x.Value, value).Add(x => x.ShowSeconds, seconds).Add(x => x.ValueChanged, (TimeOnly? v) => changed(v)));

    private static string Text(IRenderedComponent<FlareTimePicker> cut) => cut.Find("input").GetAttribute("value") ?? "";

    [Fact]
    public void Field_ShowsTheTimeOnATwelveHourClock()
    {
        var cut = RenderTwelve(new TimeOnly(14, 30), _ => { });
        Assert.Equal("02:30 PM", Text(cut));
    }

    [Theory]
    [InlineData("0230p", 14, 30)]
    [InlineData("0230P", 14, 30)]
    [InlineData("1230a", 0, 30)]
    [InlineData("1230p", 12, 30)]
    [InlineData("0230", 2, 30)]
    [InlineData("02:30p AM", 14, 30)]
    [InlineData("02:30 PMa", 2, 30)]
    [InlineData("02:30 P", 14, 30)]
    public void TypedTime_TakesThePeriodFromTheTypedLetter(string typed, int hour, int minute)
    {
        TimeOnly? committed = null;
        var cut = RenderTwelve(null, v => committed = v);
        cut.Find("input").Input(typed);
        Assert.Equal(new TimeOnly(hour, minute), committed);
    }

    [Fact]
    public void EditingTheDigits_KeepsTheValuesPeriod()
    {
        TimeOnly? committed = null;
        var cut = RenderTwelve(new TimeOnly(14, 30), v => committed = v);
        cut.Find("input").Input("0345");
        Assert.Equal(new TimeOnly(15, 45), committed);
        Assert.Equal("03:45 PM", Text(cut));
    }

    [Fact]
    public void ReplacingThePeriod_SwitchesIt()
    {
        TimeOnly? committed = null;
        var cut = RenderTwelve(new TimeOnly(14, 30), v => committed = v);
        cut.Find("input").Input("02:30 a");
        Assert.Equal(new TimeOnly(2, 30), committed);
    }

    [Fact]
    public void HourPastTwelve_IsNotCommitted()
    {
        var published = false;
        var cut = RenderTwelve(null, _ => published = true);
        cut.Find("input").Input("1330p");
        Assert.False(published);
    }

    [Fact]
    public void Seconds_OnATwelveHourClock()
    {
        TimeOnly? committed = null;
        var cut = RenderTwelve(new TimeOnly(14, 30, 45), v => committed = v, seconds: true);
        Assert.Equal("02:30:45 PM", Text(cut));
        cut.Find("input").Input("023050p");
        Assert.Equal(new TimeOnly(14, 30, 50), committed);
    }

    [Fact]
    public void CultureDesignators_AreUsed_AndASharedFirstLetterKeepsTheCurrentPeriod()
    {
        var ko = CultureInfo.GetCultureInfo("ko-KR");
        TimeOnly? committed = null;
        var cut = RenderTwelve(new TimeOnly(14, 30), v => committed = v, ko);
        Assert.Equal($"02:30 {ko.DateTimeFormat.PMDesignator}", Text(cut));
        cut.Find("input").Input("0345");
        Assert.Equal(new TimeOnly(15, 45), committed);
    }

    [Theory]
    [InlineData("en-US", false)]
    [InlineData("ru-RU", true)]
    public void Culture_SetsTheDefaultClock(string name, bool twentyFour)
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Culture, CultureInfo.GetCultureInfo(name)).Add(x => x.Value, new TimeOnly(14, 30)));
        Assert.Equal(twentyFour ? "14:30" : "02:30 PM", Text(cut));
        Assert.Equal(twentyFour ? "numeric" : null, cut.Find("input").GetAttribute("inputmode"));
    }

    [Fact]
    public void Dial_UsesTheCulturesDesignators()
    {
        var ko = CultureInfo.GetCultureInfo("ko-KR");
        var cut = Render<FlareClockDial>(p => p.Add(x => x.Hour, 14).Add(x => x.Is24Hour, false).Add(x => x.Culture, ko));
        Assert.Contains(cut.FindAll($".{Css.Classes.ClockDial.PeriodBtn}"), b => b.TextContent.Trim() == ko.DateTimeFormat.PMDesignator);
    }
}
