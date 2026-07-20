using System.Globalization;

namespace Querio.Sql;

/// <summary>
/// PostgreSQL. The most complete of the bundled dialects: it has a real percentile aggregate that
/// combines with grouping, and <c>date_trunc</c> covers every period Querio can ask for.
/// </summary>
public sealed class PostgreSqlDialect : SqlDialect
{
    /// <summary>A ready-to-use instance; the dialect holds no state.</summary>
    public static PostgreSqlDialect Instance { get; } = new();

    /// <inheritdoc/>
    public override string Name => "PostgreSQL";

    /// <inheritdoc/>
    protected override IQueryCapabilities Capabilities { get; } = QueryCapabilities.All;

    /// <inheritdoc/>
    public override string Quote(string identifier) => "\"" + Escape(identifier, '"') + "\"";

    /// <summary>
    /// PostgreSQL's <c>LIKE</c> respects case, and Querio defines text matching as case-insensitive,
    /// so the case-insensitive variant is the correct translation.
    /// </summary>
    public override string LikeOperator => "ILIKE";

    /// <inheritdoc/>
    public override string TruncateDate(string expression, QueryDateTruncation truncation)
        => $"date_trunc('{Period(truncation)}', {expression})";

    /// <inheritdoc/>
    public override string RelativeMoment(int amount, QueryTimeUnit unit)
    {
        // Multiplying a unit interval keeps the amount out of the SQL text as arithmetic rather than
        // string-building, and covers quarters, which PostgreSQL has no interval unit for.
        var (step, count) = unit switch
        {
            QueryTimeUnit.Minute => ("1 minute", amount),
            QueryTimeUnit.Hour => ("1 hour", amount),
            QueryTimeUnit.Day => ("1 day", amount),
            QueryTimeUnit.Week => ("1 week", amount),
            QueryTimeUnit.Month => ("1 month", amount),
            QueryTimeUnit.Quarter => ("3 months", amount),
            QueryTimeUnit.Year => ("1 year", amount),
            _ => ("1 day", amount),
        };
        return $"(now() + INTERVAL '{step}' * ({count.ToString(CultureInfo.InvariantCulture)}))";
    }

    /// <inheritdoc/>
    public override string Percentile(string expression, double rank, bool grouped)
        => $"percentile_cont({rank.ToString("0.####", CultureInfo.InvariantCulture)}) WITHIN GROUP (ORDER BY {expression})";

    private static string Period(QueryDateTruncation truncation) => truncation switch
    {
        QueryDateTruncation.Minute => "minute",
        QueryDateTruncation.Hour => "hour",
        QueryDateTruncation.Day => "day",
        QueryDateTruncation.Week => "week",
        QueryDateTruncation.Month => "month",
        QueryDateTruncation.Quarter => "quarter",
        QueryDateTruncation.Year => "year",
        _ => "day",
    };
}
