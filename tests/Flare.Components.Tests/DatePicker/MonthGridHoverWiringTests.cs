namespace Flare.Components.Tests;

/// <summary>
/// TASK-115: the day cells carry a hover handler only for a host that listens (the range picker); the
/// date and date-time pickers render none.
/// </summary>
public class MonthGridHoverWiringTests : FlareTestContext
{
    private static int HoverHandlers(string markup) => markup.Split("blazor:onmouseenter").Length - 1;

    [Fact]
    public void DatePicker_RendersNoHoverHandlers()
    {
        var cut = Render<FlareDatePicker>(p => p
            .Add(x => x.Inline, true)
            .Add(x => x.Value, new DateOnly(2026, 10, 15)));

        Assert.Equal(0, HoverHandlers(cut.Markup));
    }

    [Fact]
    public void RangeCalendar_KeepsItsHoverPreview()
    {
        var cut = Render<FlareDateRangePicker>(p => p.Add(x => x.Mode, DateRangePickerMode.Calendar));

        Assert.True(HoverHandlers(cut.Markup) >= 28);
    }
}
