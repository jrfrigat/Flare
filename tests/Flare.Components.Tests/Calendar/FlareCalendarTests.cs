using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareCalendarTests : FlareTestContext
{
    [Fact]
    public void RendersSevenDayLabels()
    {
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1)));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Calendar.Root}"));
        Assert.Equal(7, cut.FindAll($".{Css.Classes.Calendar.DayLabel}").Count);
    }

    // A date grid has to be a real grid: rows of cells under column headers, with the selected state
    // expressed as aria-selected rather than a button's pressed state.
    [Fact]
    public void RendersARowPerWeekAndGridCellsUnderColumnHeaders()
    {
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1))
            .Add(x => x.Culture, new CultureInfo("en-US")));

        var grid = cut.Find("[role='grid']");
        var rows = grid.QuerySelectorAll("[role='row']");
        Assert.Equal(7, rows.Length);

        var headers = rows[0].QuerySelectorAll("[role='columnheader']");
        Assert.Equal(7, headers.Length);
        Assert.Contains(headers, h => h.GetAttribute("aria-label") == "Sunday");
        Assert.All(rows.Skip(1), row => Assert.Equal(7, row.QuerySelectorAll("[role='gridcell']").Length));
        Assert.Empty(grid.QuerySelectorAll("[role='button']"));
    }

    [Fact]
    public void SelectedCell_UsesStringAriaSelected_AndTheMonthAnnouncesChanges()
    {
        var selected = new DateOnly(2026, 6, 10);
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1))
            .Add(x => x.SelectedDate, selected)
            .Add(x => x.Culture, new CultureInfo("en-US")));

        var cell = cut.FindAll("[role='gridcell']")
            .Single(c => c.GetAttribute("aria-label") == selected.ToString("D", new CultureInfo("en-US")));
        Assert.Equal("true", cell.GetAttribute("aria-selected"));
        Assert.NotNull(cut.Find($".{Css.Classes.Calendar.Header} [aria-live='polite']"));
    }

    [Fact]
    public void ClickingDay_RaisesSelectedDateChanged()
    {
        DateOnly? picked = null;
        var cut = Render<FlareCalendar>(p => p
            .Add(x => x.InitialDate, new DateOnly(2026, 6, 1))
            .Add(x => x.SelectedDateChanged, d => picked = d));

        cut.FindAll($".{Css.Classes.Calendar.Cell}")
            .First(c => !(c.ClassName ?? "").Contains("--other"))
            .Click();

        Assert.NotNull(picked);
    }
}
