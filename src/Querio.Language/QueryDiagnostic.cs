using System.Collections.Generic;
using System.Linq;

namespace Querio.Language;

/// <summary>How badly wrong something is.</summary>
public enum QuerySeverity
{
    /// <summary>The query cannot be built until this is dealt with.</summary>
    Error,

    /// <summary>The query works, but something about it is probably not meant.</summary>
    Warning,
}

/// <summary>
/// One problem found in query text, and where it is. A span rather than a bare message, because the
/// thing reading this is usually an editor that has to underline it.
/// </summary>
/// <param name="Message">What is wrong, in words a person writing the query can act on.</param>
/// <param name="Start">Zero-based offset of the first character at fault.</param>
/// <param name="Length">How many characters are at fault. Zero marks a position rather than a range.</param>
/// <param name="Severity">How badly wrong it is.</param>
public sealed record QueryDiagnostic(string Message, int Start, int Length, QuerySeverity Severity)
{
    /// <summary>Builds an error at a span.</summary>
    /// <param name="message">What is wrong.</param>
    /// <param name="token">The token at fault.</param>
    public static QueryDiagnostic Error(string message, QueryToken token)
        => new(message, token.Start, token.Length, QuerySeverity.Error);

    /// <summary>One past the last character at fault.</summary>
    public int End => Start + Length;
}

/// <summary>
/// What reading query text produced: the query, everything wrong with it, or both.
/// <para>
/// Both, usually. An editor needs every problem at once rather than the first one, and it still
/// wants whatever query could be made out of the rest so it can keep offering sensible suggestions
/// while the text is broken.
/// </para>
/// </summary>
public sealed class QueryParseResult
{
    internal QueryParseResult(QuerySpec? spec, IReadOnlyList<QueryDiagnostic> diagnostics)
    {
        Spec = spec;
        Diagnostics = diagnostics;
    }

    /// <summary>The query that was read, or null when not even a partial one could be made.</summary>
    public QuerySpec? Spec { get; }

    /// <summary>Everything found wrong, in the order it was found.</summary>
    public IReadOnlyList<QueryDiagnostic> Diagnostics { get; }

    /// <summary>Whether a query was read with nothing wrong.</summary>
    public bool IsValid => Spec is not null && !Diagnostics.Any(d => d.Severity == QuerySeverity.Error);

    /// <summary>
    /// The query, or a failure carrying every problem at once. For a caller that wants a query or
    /// nothing; an editor should read <see cref="Spec"/> and <see cref="Diagnostics"/> instead.
    /// </summary>
    /// <exception cref="QueryParseException">Anything is wrong with the text.</exception>
    public QuerySpec Require()
    {
        if (IsValid) return Spec!;
        var problems = string.Join("; ", Diagnostics.Select(d => $"{d.Message} (at {d.Start})"));
        throw new QueryParseException(
            problems.Length == 0 ? "The text does not read as a query." : problems);
    }
}
