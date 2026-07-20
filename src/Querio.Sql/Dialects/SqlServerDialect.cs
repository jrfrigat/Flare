using System.Globalization;
using System.Text;

namespace Querio.Sql;

/// <summary>
/// Microsoft SQL Server. Two things set it apart: rows are capped with <c>TOP</c> unless an offset is
/// involved, and its percentile is a window function rather than an aggregate, so it cannot be
/// combined with grouping.
/// </summary>
public sealed class SqlServerDialect : SqlDialect
{
    /// <summary>A ready-to-use instance; the dialect holds no state.</summary>
    public static SqlServerDialect Instance { get; } = new();

    /// <inheritdoc/>
    public override string Name => "SQL Server";

    /// <inheritdoc/>
    protected override IQueryCapabilities Capabilities { get; } = QueryCapabilities.All;

    /// <inheritdoc/>
    public override string Quote(string identifier) => "[" + Escape(identifier, ']') + "]";

    /// <summary>
    /// Adds the square bracket to the escaped set: T-SQL treats it as a pattern character, so a value
    /// containing one would otherwise be read as a character class rather than as text.
    /// </summary>
    public override string EscapeLikePattern(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character == LikeEscape || character == '%' || character == '_' || character == '[')
            {
                builder.Append(LikeEscape);
            }
            builder.Append(character);
        }
        return builder.ToString();
    }

    /// <inheritdoc/>
    public override string TruncateDate(string expression, QueryDateTruncation truncation)
    {
        // The DATEADD/DATEDIFF pair rounds down to the start of the period against the zero date,
        // which is the portable T-SQL idiom for truncation.
        var part = Part(truncation);
        return $"DATEADD({part}, DATEDIFF({part}, 0, {expression}), 0)";
    }

    /// <inheritdoc/>
    public override string RelativeMoment(int amount, QueryTimeUnit unit)
        => $"DATEADD({Part(unit)}, {amount.ToString(CultureInfo.InvariantCulture)}, SYSUTCDATETIME())";

    /// <inheritdoc/>
    public override string Percentile(string expression, double rank, bool grouped)
    {
        if (grouped)
        {
            throw new QueryRenderException(
                "SQL Server's PERCENTILE_CONT is a window function, not an aggregate, so it cannot be " +
                "combined with GROUP BY in a single statement. Compute the percentile without grouping, " +
                "or run this query against PostgreSQL.",
                QueryFeature.Percentile);
        }
        var fraction = rank.ToString("0.####", CultureInfo.InvariantCulture);
        return $"PERCENTILE_CONT({fraction}) WITHIN GROUP (ORDER BY {expression}) OVER ()";
    }

    /// <inheritdoc/>
    public override string RenderTop(int? limit, int? offset)
        // With an offset in play the OFFSET/FETCH form handles both, so TOP would only conflict.
        => limit is not null && offset is null
            ? $"TOP ({limit.Value.ToString(CultureInfo.InvariantCulture)}) "
            : string.Empty;

    /// <inheritdoc/>
    public override void AppendPaging(StringBuilder sql, int? limit, int? offset, bool hasOrderBy)
    {
        if (offset is null) return; // A plain limit was already emitted as TOP.

        // OFFSET is only legal after an ORDER BY, so an unordered paged query needs a placeholder one.
        if (!hasOrderBy) sql.Append(" ORDER BY (SELECT NULL)");

        sql.Append(" OFFSET ").Append(offset.Value.ToString(CultureInfo.InvariantCulture)).Append(" ROWS");
        if (limit is not null)
        {
            sql.Append(" FETCH NEXT ")
               .Append(limit.Value.ToString(CultureInfo.InvariantCulture))
               .Append(" ROWS ONLY");
        }
    }

    private static string Part(QueryDateTruncation truncation) => truncation switch
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

    private static string Part(QueryTimeUnit unit) => unit switch
    {
        QueryTimeUnit.Minute => "minute",
        QueryTimeUnit.Hour => "hour",
        QueryTimeUnit.Day => "day",
        QueryTimeUnit.Week => "week",
        QueryTimeUnit.Month => "month",
        QueryTimeUnit.Quarter => "quarter",
        QueryTimeUnit.Year => "year",
        _ => "day",
    };
}
