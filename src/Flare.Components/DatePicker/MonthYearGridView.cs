namespace Flare.Components;

/// <summary>What a <see cref="FlareMonthYearGrid"/> lays out: the months of one year or the years of one decade.</summary>
public enum MonthYearGridView
{
    /// <summary>The months of <see cref="FlareMonthYearGrid.Year"/> (12, or 13 in a Hebrew leap year).</summary>
    Months,
    /// <summary>The twelve years of the decade that holds <see cref="FlareMonthYearGrid.Year"/>.</summary>
    Years,
}
