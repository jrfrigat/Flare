using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-110: the masked date fields follow the culture's own segment order, so year-first cultures
/// (ja-JP, sv-SE) can type a date as well as day-first and month-first ones.
/// </summary>
public class CultureDateOrderTests : FlareTestContext
{
    [Theory]
    [InlineData("ru-RU", "dd.MM.yyyy")]
    [InlineData("en-US", "MM/dd/yyyy")]
    [InlineData("ja-JP", "yyyy/MM/dd")]
    [InlineData("sv-SE", "yyyy-MM-dd")]
    public void NumericDatePattern_FollowsTheCultureOrder(string culture, string expected)
    {
        var c = new CultureInfo(culture);
        Assert.Equal(expected, MaskedInput.NumericDatePattern(c, c.DateTimeFormat.DateSeparator));
    }

    [Theory]
    [InlineData("1102026", "dd.MM.yyyy", "11.02.026")]
    [InlineData("11", "dd.MM.yyyy", "11")]
    [InlineData("110", "dd.MM.yyyy", "11.0")]
    [InlineData("20261015", "yyyy/MM/dd", "2026/10/15")]
    [InlineData("20261", "yyyy/MM/dd", "2026/1")]
    [InlineData("2026101512309", "dd.MM.yyyy HH:mm", "20.26.1015 12:30")]
    [InlineData("", "dd.MM.yyyy", "")]
    public void MaskByPattern_LaysDigitsOnThePattern(string raw, string pattern, string expected)
        => Assert.Equal(expected, MaskedInput.MaskByPattern(raw, pattern));

    [Theory]
    [InlineData("ja-JP", "20261015", "2026/10/15")]
    [InlineData("sv-SE", "20261015", "2026-10-15")]
    [InlineData("ru-RU", "15102026", "15.10.2026")]
    [InlineData("en-US", "10152026", "10/15/2026")]
    public void DatePicker_TypedDigits_CommitInEveryCultureOrder(string culture, string typed, string masked)
    {
        DateOnly? committed = null;
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, new CultureInfo(culture))
            .Add(x => x.ValueChanged, (DateOnly? v) => committed = v));

        cut.Find("input").Focus();
        cut.Find("input").Input(typed);

        Assert.Equal(new DateOnly(2026, 10, 15), committed);
        Assert.Equal(masked, cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void DateTimePicker_YearFirstCulture_Commits()
    {
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ja-JP"))
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        cut.Find("input").Focus();
        cut.Find("input").Input("202610151230");

        Assert.Equal(new DateTime(2026, 10, 15, 12, 30, 0), committed?.DateTime);
    }
}
