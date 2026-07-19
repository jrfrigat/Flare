using System.Globalization;
using System.Text;

namespace Querio.Sql;

/// <summary>
/// Everything that differs between one SQL engine and the next: how names are quoted, how parameters
/// are referenced, how a timestamp is truncated, how rows are limited, and what the engine simply
/// cannot do. <see cref="SqlRenderer"/> holds the shape of a query; a dialect supplies the spelling.
/// <para>
/// A dialect declares its capabilities, and the renderer refuses to render a query that asks for
/// something missing. Producing "close enough" SQL would hand back a result that looks right and
/// answers a different question, which is the one failure mode worth being strict about.
/// </para>
/// </summary>
public abstract class SqlDialect : IQueryCapabilities
{
    /// <summary>The engine this dialect targets, used in diagnostics.</summary>
    public abstract string Name { get; }

    /// <summary>What this engine can do. Anything absent makes the renderer fail loudly.</summary>
    protected abstract IQueryCapabilities Capabilities { get; }

    /// <inheritdoc/>
    public bool Supports(QueryFeature feature) => Capabilities.Supports(feature);

    /// <summary>Quotes one identifier, escaping any quote character it contains.</summary>
    /// <param name="identifier">The bare identifier.</param>
    public abstract string Quote(string identifier);

    /// <summary>
    /// Quotes a possibly qualified physical name such as <c>dbo.RequestLog</c>, quoting each part
    /// separately so the qualifier survives.
    /// </summary>
    /// <param name="name">The physical name, optionally dotted.</param>
    public virtual string QuoteQualified(string name)
    {
        if (name.IndexOf('.') < 0) return Quote(name);

        var parts = name.Split('.');
        for (var i = 0; i < parts.Length; i++) parts[i] = Quote(parts[i]);
        return string.Join(".", parts);
    }

    /// <summary>
    /// How a parameter is referred to inside the SQL text. The <c>@name</c> form is understood by the
    /// mainstream drivers for all three engines here, so it is the default.
    /// </summary>
    /// <param name="ordinal">Zero-based position in the parameter list.</param>
    public virtual string ParameterPlaceholder(int ordinal) => "@" + ParameterName(ordinal);

    /// <summary>The name a driver expects for the parameter, without any prefix.</summary>
    /// <param name="ordinal">Zero-based position in the parameter list.</param>
    public virtual string ParameterName(int ordinal)
        => "p" + ordinal.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// The operator used for the text-matching comparisons. Querio defines those as case-insensitive,
    /// so an engine whose <c>LIKE</c> respects case says so here.
    /// </summary>
    public virtual string LikeOperator => "LIKE";

    /// <summary>The character that escapes a wildcard inside a pattern.</summary>
    public virtual char LikeEscape => '\\';

    /// <summary>
    /// Escapes the wildcards in a value so a search for a literal percent sign finds a percent sign
    /// rather than matching everything.
    /// </summary>
    /// <param name="value">The raw value a user typed.</param>
    public virtual string EscapeLikePattern(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character == LikeEscape || character == '%' || character == '_') builder.Append(LikeEscape);
            builder.Append(character);
        }
        return builder.ToString();
    }

    /// <summary>Collapses a timestamp expression to the start of the given period.</summary>
    /// <param name="expression">The already-rendered timestamp expression.</param>
    /// <param name="truncation">The period to collapse into.</param>
    public abstract string TruncateDate(string expression, QueryDateTruncation truncation);

    /// <summary>
    /// An expression for a moment offset from now, evaluated by the engine so a saved query keeps
    /// meaning "the last 30 days" rather than freezing the day it was built.
    /// </summary>
    /// <param name="amount">Signed offset; negative reaches into the past.</param>
    /// <param name="unit">The unit the offset is counted in.</param>
    public abstract string RelativeMoment(int amount, QueryTimeUnit unit);

    /// <summary>Computes a percentile. Engines that cannot do this leave it throwing.</summary>
    /// <param name="expression">The already-rendered value expression.</param>
    /// <param name="rank">The rank as a fraction, where 0.95 is the 95th percentile.</param>
    /// <param name="grouped">Whether the query groups rows, which some engines cannot combine with this.</param>
    public virtual string Percentile(string expression, double rank, bool grouped)
        => throw new QueryRenderException(
            $"{Name} cannot compute percentiles.", QueryFeature.Percentile);

    /// <summary>
    /// Text inserted immediately after <c>SELECT</c>, for engines that cap rows there rather than at
    /// the end. Returns an empty string when the engine pages at the end instead.
    /// </summary>
    /// <param name="limit">Maximum rows, or null for all of them.</param>
    /// <param name="offset">Rows to skip, or null to start at the first.</param>
    public virtual string RenderTop(int? limit, int? offset) => string.Empty;

    /// <summary>Appends the trailing paging clause. The default is the common LIMIT/OFFSET form.</summary>
    /// <param name="sql">The statement being built.</param>
    /// <param name="limit">Maximum rows, or null for all of them.</param>
    /// <param name="offset">Rows to skip, or null to start at the first.</param>
    /// <param name="hasOrderBy">Whether the statement already carries an ORDER BY.</param>
    public virtual void AppendPaging(StringBuilder sql, int? limit, int? offset, bool hasOrderBy)
    {
        if (limit is not null)
        {
            sql.Append(" LIMIT ").Append(limit.Value.ToString(CultureInfo.InvariantCulture));
        }
        if (offset is not null)
        {
            sql.Append(" OFFSET ").Append(offset.Value.ToString(CultureInfo.InvariantCulture));
        }
    }

    /// <summary>Doubles every occurrence of a closing quote so an identifier cannot break out of it.</summary>
    /// <param name="identifier">The bare identifier.</param>
    /// <param name="quote">The closing quote character to double.</param>
    protected static string Escape(string identifier, char quote)
        => identifier.Replace(quote.ToString(), new string(quote, 2));
}
