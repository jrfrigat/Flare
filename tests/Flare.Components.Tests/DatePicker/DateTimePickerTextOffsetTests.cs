using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-135: typing into the date-time field edits the wall date and time only. The value keeps its offset
/// and the seconds the format does not show, so a one-minute edit moves the instant by one minute.
/// </summary>
public class DateTimePickerTextOffsetTests : FlareTestContext
{
    private static readonly CultureInfo Ru = new("ru-RU");

    private IRenderedComponent<FlareDateTimePicker> RenderPicker(
        DateTimeOffset? value, Action<DateTimeOffset?> onChange, string? format = null) =>
        Render<FlareDateTimePicker>(p =>
        {
            p.Add(x => x.Culture, Ru).Add(x => x.Value, value).Add(x => x.ValueChanged, onChange);
            if (format is not null) p.Add(x => x.DateTimeFormat, format);
        });

    [Fact]
    public void MinuteEdit_KeepsOffsetAndHiddenSeconds()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(5)).AddTicks(1234567);
        DateTimeOffset? result = null;
        var cut = RenderPicker(original, v => result = v);

        cut.Find("input").Focus();
        cut.Find("input").Input("15.10.2026 14:31");

        Assert.Equal(original.AddMinutes(1), result);
        Assert.Equal(TimeSpan.FromHours(5), result!.Value.Offset);
    }

    [Fact]
    public void TextChange_KeepsOffset()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(-7));
        DateTimeOffset? result = null;
        var cut = RenderPicker(original, v => result = v);

        cut.Find("input").Change("16.10.2026 09:05");

        Assert.Equal(new DateTimeOffset(2026, 10, 16, 9, 5, 0, TimeSpan.FromHours(-7)), result);
    }

    [Fact]
    public void NewValue_TakesTheLocalOffsetAtThatWallTime()
    {
        DateTimeOffset? result = null;
        var cut = RenderPicker(null, v => result = v);

        cut.Find("input").Focus();
        cut.Find("input").Input("15.07.2026 12:00");

        var wall = new DateTime(2026, 7, 15, 12, 0, 0);
        Assert.Equal(new DateTimeOffset(wall, TimeZoneInfo.Local.GetUtcOffset(wall)), result);
    }

    [Fact]
    public void FormatWithOffset_AppliesTheTypedOffset()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(5));
        DateTimeOffset? result = null;
        var cut = RenderPicker(original, v => result = v, "dd.MM.yyyy HH:mm zzz");

        cut.Find("input").Change("15.10.2026 14:30 +02:00");

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(2)), result);
    }

    [Fact]
    public void FormatWithSeconds_KeepsOnlyTheFraction()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 45, TimeSpan.FromHours(5)).AddTicks(1234567);
        DateTimeOffset? result = null;
        var cut = RenderPicker(original, v => result = v, "dd.MM.yyyy HH:mm:ss");

        cut.Find("input").Change("15.10.2026 14:30:10");

        Assert.Equal(new DateTimeOffset(2026, 10, 15, 14, 30, 10, TimeSpan.FromHours(5)).AddTicks(1234567), result);
    }

    [Fact]
    public void WallTimeOutsideTheOffsetRange_IsNotCommitted()
    {
        var original = new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.FromHours(5));
        var calls = 0;
        var cut = RenderPicker(original, _ => calls++);

        cut.Find("input").Change("01.01.0001 00:00");

        Assert.Equal(0, calls);
    }
}
