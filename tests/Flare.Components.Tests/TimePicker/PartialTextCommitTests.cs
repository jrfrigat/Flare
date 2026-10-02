using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-126: a half-edited date-time or time must not reach the lenient parser on blur and be saved as a
/// different value; the time field also goes back to the current value instead of keeping the half-edit.
/// </summary>
public class PartialTextCommitTests : FlareTestContext
{
    [Fact]
    public void DateTimePicker_IncompleteMinutes_AreNotCommitted()
    {
        var changes = 0;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ru-RU"))
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 12, 30, 0, TimeSpan.FromHours(3)))
            .Add(x => x.ValueChanged, (DateTimeOffset? _) => changes++));

        cut.Find("input").Focus();
        cut.Find("input").Input("15.10.2026 12:3");
        cut.Find("input").Change("15.10.2026 12:3");

        Assert.Equal(0, changes);
    }

    [Fact]
    public void DateTimePicker_CompleteText_StillCommits()
    {
        DateTimeOffset? committed = null;
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("ru-RU"))
            .Add(x => x.ValueChanged, (DateTimeOffset? v) => committed = v));

        cut.Find("input").Focus();
        cut.Find("input").Change("15.10.2026 12:45");

        Assert.Equal(new DateTime(2026, 10, 15, 12, 45, 0), committed?.DateTime);
    }

    [Fact]
    public void TimePicker_IncompleteMinutes_AreNotCommitted_AndTheFieldSnapsBack()
    {
        var changes = 0;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(12, 33))
            .Add(x => x.ValueChanged, (TimeOnly? _) => changes++));

        cut.Find("input").Input("12:3");
        cut.Find("input").Change("12:3");

        Assert.Equal(0, changes);
        Assert.Equal("12:33", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void TimePicker_CompleteText_StillCommits()
    {
        TimeOnly? committed = null;
        var cut = Render<FlareTimePicker>(p => p
            .Add(x => x.Value, new TimeOnly(12, 33))
            .Add(x => x.ValueChanged, (TimeOnly? v) => committed = v));

        cut.Find("input").Change("14:05");

        Assert.Equal(new TimeOnly(14, 5), committed);
    }
}
