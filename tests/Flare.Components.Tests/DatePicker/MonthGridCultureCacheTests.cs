using System.Globalization;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-164: the month grid rebuilds its day labels for a new culture object even when it keeps the name, and
/// keeps them for the same object.
/// </summary>
public class MonthGridCultureCacheTests : FlareTestContext
{
    private static string Label(IRenderedComponent<FlareMonthGrid> cut, string day) =>
        cut.FindAll("button[role=gridcell]").First(b => b.TextContent.Trim() == day).GetAttribute("aria-label") ?? "";

    [Fact]
    public void SameNamedClone_WithOtherDayNames_RelabelsTheDays()
    {
        var first = new CultureInfo("en-US");
        var second = (CultureInfo)first.Clone();
        var names = second.DateTimeFormat.DayNames;
        names[(int)DayOfWeek.Thursday] = "CustomThursday";
        second.DateTimeFormat.DayNames = names;

        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10).Add(x => x.Culture, first));
        Assert.Contains("Thursday", Label(cut, "15"));

        cut.Render(p => p.Add(x => x.Culture, second));
        Assert.Contains("CustomThursday", Label(cut, "15"));
    }

    [Fact]
    public void AnotherCulture_RelabelsTheDays()
    {
        var cut = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.Culture, CultureInfo.GetCultureInfo("en-US")));
        cut.Render(p => p.Add(x => x.Culture, CultureInfo.GetCultureInfo("de-DE")));
        Assert.Contains("Donnerstag", Label(cut, "15"));
    }
}
