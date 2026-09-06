namespace Flare.Integration.Tests.Screens;

/// <summary>One row of the orders grid. A record so the grid's row identity is by value.</summary>
/// <param name="Reference">Human-readable order number shown in the first column.</param>
/// <param name="Customer">Who placed the order.</param>
/// <param name="Total">Order total, in the shop's currency.</param>
public sealed record Order(string Reference, string Customer, decimal Total)
{
    /// <summary>A deterministic set of rows: nine orders across three customers, totals ascending
    /// with the reference so a sort has something unambiguous to reorder.</summary>
    public static IReadOnlyList<Order> Sample { get; } =
    [
        new("ORD-001", "Aster", 10m),
        new("ORD-002", "Borage", 20m),
        new("ORD-003", "Clover", 30m),
        new("ORD-004", "Aster", 40m),
        new("ORD-005", "Borage", 50m),
        new("ORD-006", "Clover", 60m),
        new("ORD-007", "Aster", 70m),
        new("ORD-008", "Borage", 80m),
        new("ORD-009", "Clover", 90m),
    ];
}
