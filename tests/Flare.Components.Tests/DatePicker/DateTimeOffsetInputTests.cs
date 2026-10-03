using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-165: a format that shows the offset (zzz) lets the keyboard change it - the focused field edits the
/// offset after the time, and the mask keeps its sign and digits instead of dropping them.
/// </summary>
public class DateTimeOffsetInputTests : FlareTestContext
{
    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");
    private static readonly DateTimeOffset Start = new(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(5));

    private IRenderedComponent<FlareDateTimePicker> RenderOffset(DateTimeOffset? value, Action<DateTimeOffset?> changed,
        CultureInfo? culture = null) =>
        Render<FlareDateTimePicker>(p => p.Add(x => x.Culture, culture ?? Ru).Add(x => x.Value, value)
            .Add(x => x.DateTimeFormat, "dd.MM.yyyy HH:mm zzz").Add(x => x.ValueChanged, (DateTimeOffset? v) => changed(v)));

    [Fact]
    public void FocusedField_EditsTheOffset()
    {
        var cut = RenderOffset(Start, _ => { });
        cut.Find("input").Focus();
        Assert.Equal("15.10.2026 14:30 +05:00", cut.Find("input").GetAttribute("value"));
    }

    [Theory]
    [InlineData("15.10.2026 14:30 +02:00", 2)]
    [InlineData("15.10.2026 14:30 -03:30", -3.5)]
    [InlineData("151020261430-0330", -3.5)]
    public void TypedOffset_IsCommitted(string typed, double hours)
    {
        DateTimeOffset? committed = null;
        var cut = RenderOffset(Start, v => committed = v);
        cut.Find("input").Focus();
        cut.Find("input").Input(typed);
        Assert.Equal(new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(hours)), committed);
    }

    [Fact]
    public void HalfTypedOffset_IsNotCommitted_AndKeepsItsSign()
    {
        DateTimeOffset? committed = null;
        var cut = RenderOffset(Start, v => committed = v);
        cut.Find("input").Focus();
        cut.Find("input").Input("15.10.2026 14:30 -0");
        Assert.Null(committed);
        Assert.Equal("15.10.2026 14:30 -0", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void DashDateSeparator_IsNotTakenForTheSign()
    {
        DateTimeOffset? committed = null;
        var cut = RenderOffset(Start, v => committed = v, CultureInfo.GetCultureInfo("sv-SE"));
        cut.Find("input").Focus();
        Assert.Equal("2026-10-15 14:30 +05:00", cut.Find("input").GetAttribute("value"));
        cut.Find("input").Input("2026-10-16 14:30 +05:00");
        Assert.Equal(Start.AddDays(1), committed);
    }

    [Fact]
    public void HiddenSeconds_SurviveATypedOffset()
    {
        DateTimeOffset? committed = null;
        var cut = RenderOffset(Start.AddSeconds(45).AddTicks(123), v => committed = v);
        cut.Find("input").Focus();
        cut.Find("input").Input("15.10.2026 14:30 +02:00");
        Assert.Equal(new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(2)).AddTicks(123), committed);
    }

    [Fact]
    public void FormatWithoutOffset_StillEditsWithoutIt()
    {
        var cut = Render<FlareDateTimePicker>(p => p.Add(x => x.Culture, Ru).Add(x => x.Value, Start));
        cut.Find("input").Focus();
        Assert.Equal("15.10.2026 14:30", cut.Find("input").GetAttribute("value"));
    }
}
