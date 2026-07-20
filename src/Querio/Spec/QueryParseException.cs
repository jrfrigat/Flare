namespace Querio;

/// <summary>
/// Thrown when text cannot be read back as a query.
/// <para>
/// Reading is not the mirror image of writing. Writing is total - every query can be written down -
/// but reading is partial, because most of what could be written in a target's language says
/// something this model cannot hold. Refusing loudly is the point: a parser that quietly dropped
/// what it did not understand would hand back a query that means something else.
/// </para>
/// </summary>
public sealed class QueryParseException : Exception
{
    /// <summary>Builds the failure with a message describing what could not be read.</summary>
    /// <param name="message">What went wrong, and where.</param>
    public QueryParseException(string message) : base(message)
    {
    }

    /// <summary>Builds the failure with a message and the position it was noticed at.</summary>
    /// <param name="message">What went wrong.</param>
    /// <param name="position">Zero-based offset into the text being read.</param>
    public QueryParseException(string message, int position) : base($"{message} (at position {position})")
        => Position = position;

    /// <summary>Builds the failure from an underlying one.</summary>
    /// <param name="message">What went wrong.</param>
    /// <param name="inner">The failure that caused it.</param>
    public QueryParseException(string message, Exception inner) : base(message, inner)
    {
    }

    /// <summary>Zero-based offset the failure was noticed at, or null when it has no single place.</summary>
    public int? Position { get; }
}
