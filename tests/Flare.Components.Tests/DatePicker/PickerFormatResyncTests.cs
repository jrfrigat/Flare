using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-113: a change to the display parameters (format, culture, seconds) rewrites the field text even
/// when the value itself stays the same.
/// </summary>
public class PickerFormatResyncTests : FlareTestContext
{
    [Fact]
    public void DatePicker_NewDateFormat_RewritesTheText()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, CultureInfo.InvariantCulture)
            .Add(x => x.DateFormat, "yyyy-MM-dd")
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));
        Assert.Equal("2026-10-15", cut.Find("input").GetAttribute("value"));

        cut.Render(p => p.Add(x => x.DateFormat, "dd/MM/yyyy"));

        Assert.Equal("15/10/2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void DatePicker_NewCulture_RewritesTheText()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Culture, new CultureInfo("en-US"))
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        cut.Render(p => p.Add(x => x.Culture, new CultureInfo("ru-RU")));

        Assert.Equal("15.10.2026", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void TimePicker_ShowSeconds_RewritesTheText()
    {
        var cut = Render<FlareTimePicker>(p => p.Add(x => x.Value, new TimeOnly(14, 30, 45)));
        Assert.Equal("14:30", cut.Find("input").GetAttribute("value"));

        cut.Render(p => p.Add(x => x.ShowSeconds, true));

        Assert.Equal("14:30:45", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void DateTimePicker_NewFormat_RewritesTheText()
    {
        var cut = Render<FlareDateTimePicker>(p => p
            .Add(x => x.Culture, CultureInfo.InvariantCulture)
            .Add(x => x.DateTimeFormat, "yyyy-MM-dd HH:mm")
            .Add(x => x.Value, new DateTimeOffset(2026, 10, 15, 14, 30, 0, TimeSpan.Zero)));

        cut.Render(p => p.Add(x => x.DateTimeFormat, "dd.MM.yyyy HH:mm"));

        Assert.Equal("15.10.2026 14:30", cut.Find("input").GetAttribute("value"));
    }
}
