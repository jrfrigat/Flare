using System.Globalization;

namespace Flare.Components.Tests;

public class MonthGridSharedDataTests : FlareTestContext
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

    [Fact]
    public void TwoGrids_KeepSelectionAndDisabledDaysIndependent()
    {
        var selected = new DateOnly(2026, 10, 15);
        var first = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.Culture, En).Add(x => x.Selected, d => d == selected).Add(x => x.Disabled, d => d == selected));
        var second = Render<FlareMonthGrid>(p => p.Add(x => x.ViewYear, 2026).Add(x => x.ViewMonth, 10)
            .Add(x => x.Culture, En));
        var a = first.FindAll("button[role=gridcell]").Single(x => x.TextContent.Trim() == "15");
        var b = second.FindAll("button[role=gridcell]").Single(x => x.TextContent.Trim() == "15");
        Assert.True(a.HasAttribute("disabled"));
        Assert.Equal("true", a.GetAttribute("aria-selected"));
        Assert.False(b.HasAttribute("disabled"));
        Assert.Equal("false", b.GetAttribute("aria-selected"));
    }

    [Fact]
    public void MutableCulture_NewMountReadsChangedLabels()
    {
        var culture = new CultureInfo("en-US");
        var first = MonthGridData.Get(culture, 2026, 10, DayOfWeek.Sunday);
        var names = culture.DateTimeFormat.DayNames;
        names[(int)DayOfWeek.Thursday] = "ChangedThursday";
        culture.DateTimeFormat.DayNames = names;
        var second = MonthGridData.Get(culture, 2026, 10, DayOfWeek.Sunday);
        Assert.Contains(first.Labels, x => x is not null && x.Contains("Thursday"));
        Assert.DoesNotContain(first.Labels, x => x is not null && x.Contains("ChangedThursday"));
        Assert.Contains(second.Labels, x => x is not null && x.Contains("ChangedThursday"));
    }

    [Fact]
    public void ReadOnlyCultures_WithSameNameKeepTheirOwnLabels()
    {
        var custom = new CultureInfo("en-US");
        var names = custom.DateTimeFormat.DayNames;
        names[(int)DayOfWeek.Thursday] = "CustomThursday";
        custom.DateTimeFormat.DayNames = names;
        var a = MonthGridData.Get(En, 2026, 10, DayOfWeek.Sunday);
        var b = MonthGridData.Get(CultureInfo.ReadOnly(custom), 2026, 10, DayOfWeek.Sunday);
        Assert.DoesNotContain(a.Labels, x => x is not null && x.Contains("CustomThursday"));
        Assert.Contains(b.Labels, x => x is not null && x.Contains("CustomThursday"));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(9999, 12)]
    public void ExtremeMonths_KeepEmptyCellsAndLabelsAligned(int year, int month)
    {
        var data = MonthGridData.Get(En, year, month, DayOfWeek.Sunday);
        var days = data.Weeks.SelectMany(x => x).ToArray();
        Assert.Equal(42, days.Length);
        Assert.Contains(days, x => x is null);
        for (var i = 0; i < days.Length; i++)
            Assert.Equal(days[i] is { } day ? day.ToString("D", En) : null, data.Labels[i]);
    }

    [Fact]
    public void FirstWeekdayChangesTheGridAndItsLabels()
    {
        var sunday = MonthGridData.Get(En, 2026, 10, DayOfWeek.Sunday);
        var monday = MonthGridData.Get(En, 2026, 10, DayOfWeek.Monday);
        Assert.Equal(DayOfWeek.Sunday, sunday.Weeks[0][0]!.Value.DayOfWeek);
        Assert.Equal(DayOfWeek.Monday, monday.Weeks[0][0]!.Value.DayOfWeek);
        Assert.NotEqual(sunday.Labels[0], monday.Labels[0]);
    }

    [Fact]
    public async Task ConcurrentGrids_ShareReadOnlyDataWithinTheBoundedCache()
    {
        var culture = CultureInfo.ReadOnly(new CultureInfo("en-GB"));
        var results = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => Task.Run(() =>
            MonthGridData.Get(culture, 2026, 10, DayOfWeek.Monday))));
        Assert.All(results, x => Assert.Same(results[0], x));
        for (var month = 1; month <= 9; month++) MonthGridData.Get(culture, 2027, month, DayOfWeek.Monday);
        Assert.NotSame(results[0], MonthGridData.Get(culture, 2026, 10, DayOfWeek.Monday));
        Assert.Equal(results[0].Labels, MonthGridData.Get(culture, 2026, 10, DayOfWeek.Monday).Labels);
    }
}
