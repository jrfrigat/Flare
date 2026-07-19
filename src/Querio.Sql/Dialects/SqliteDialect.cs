using System.Globalization;

namespace Querio.Sql;

/// <summary>
/// SQLite. The narrowest of the bundled dialects, and the reason the capability model exists: it has
/// no percentile function at all, so a query asking for one is refused here rather than quietly
/// answered with something else.
/// <para>
/// Right and full outer joins need SQLite 3.39 or newer. They are declared as supported because
/// that is the common case now; pair an older engine with a capability set that removes them.
/// </para>
/// </summary>
public sealed class SqliteDialect : SqlDialect
{
    /// <summary>A ready-to-use instance; the dialect holds no state.</summary>
    public static SqliteDialect Instance { get; } = new();

    /// <inheritdoc/>
    public override string Name => "SQLite";

    /// <inheritdoc/>
    protected override IQueryCapabilities Capabilities { get; }
        = QueryCapabilities.All.Without(QueryFeature.Percentile);

    /// <inheritdoc/>
    public override string Quote(string identifier) => "\"" + Escape(identifier, '"') + "\"";

    /// <inheritdoc/>
    public override string TruncateDate(string expression, QueryDateTruncation truncation) => truncation switch
    {
        QueryDateTruncation.Minute => $"strftime('%Y-%m-%d %H:%M:00', {expression})",
        QueryDateTruncation.Hour => $"strftime('%Y-%m-%d %H:00:00', {expression})",
        QueryDateTruncation.Day => $"date({expression})",
        // 'weekday 0' moves forward to the next Sunday, so stepping back a week lands on the Sunday
        // on or before the value - the usual SQLite idiom for the start of the week.
        QueryDateTruncation.Week => $"date({expression}, 'weekday 0', '-7 days')",
        QueryDateTruncation.Month => $"strftime('%Y-%m-01', {expression})",
        QueryDateTruncation.Year => $"strftime('%Y-01-01', {expression})",
        QueryDateTruncation.Quarter => throw new QueryRenderException(
            "SQLite has no quarter function, and expressing one inline would produce an expression too " +
            "fragile to trust. Group by month and combine the quarters in the consumer.",
            QueryFeature.DateTruncation),
        _ => $"date({expression})",
    };

    /// <inheritdoc/>
    public override string RelativeMoment(int amount, QueryTimeUnit unit)
    {
        // SQLite takes a modifier string; weeks and quarters have no modifier of their own, so they
        // are converted into the units it does understand.
        var (count, modifier) = unit switch
        {
            QueryTimeUnit.Minute => (amount, "minutes"),
            QueryTimeUnit.Hour => (amount, "hours"),
            QueryTimeUnit.Day => (amount, "days"),
            QueryTimeUnit.Week => (amount * 7, "days"),
            QueryTimeUnit.Month => (amount, "months"),
            QueryTimeUnit.Quarter => (amount * 3, "months"),
            QueryTimeUnit.Year => (amount, "years"),
            _ => (amount, "days"),
        };
        var signed = count >= 0
            ? "+" + count.ToString(CultureInfo.InvariantCulture)
            : count.ToString(CultureInfo.InvariantCulture);
        return $"datetime('now', '{signed} {modifier}')";
    }
}
