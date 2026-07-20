using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Querio.Http;

/// <summary>
/// Writes a query as a query string, and reads one back.
/// <para>
/// The only target that goes both ways, which is what it is for: a query written here survives a
/// link, a bookmark, a saved report or a request from a client, and comes back meaning exactly what
/// it meant. Every other target is a one-way translation into somebody else's language.
/// </para>
/// <para>
/// Keys are written logically - the entity and field names the schema declares, never the physical
/// names a store happens to use - so the same text still means the same query when it is later
/// pointed at a different store.
/// </para>
/// </summary>
public static class QueryHttp
{
    /// <summary>Anything a query can express can be written down, so nothing is refused.</summary>
    public static IQueryCapabilities Capabilities => QueryHttpWriter.Capabilities;

    /// <summary>
    /// Writes a query as a readable query string. Values are left as they are rather than
    /// percent-encoded, since this is the form to read, log and store; use
    /// <see cref="ToUri(QuerySpec, QuerySchema, string)"/> to put it in a URL.
    /// </summary>
    /// <param name="spec">The query to write.</param>
    /// <param name="schema">The schema it was built against.</param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    public static string Render(QuerySpec spec, QuerySchema schema)
        => new QueryHttpWriter(spec, schema).Run();

    /// <summary>
    /// Writes a query as a URL, percent-encoding each value so it survives being sent.
    /// </summary>
    /// <param name="spec">The query to write.</param>
    /// <param name="schema">The schema it was built against.</param>
    /// <param name="path">What the query string is appended to. Defaults to nothing.</param>
    public static string ToUri(QuerySpec spec, QuerySchema schema, string path = "")
    {
        var encoded = string.Join("&", Render(spec, schema).Split('&').Select(pair =>
        {
            var split = pair.IndexOf('=');
            return split < 0
                ? pair
                : pair.Substring(0, split + 1) + Uri.EscapeDataString(pair.Substring(split + 1));
        }));
        return path.Length == 0 ? encoded : path + (path.IndexOf('?') >= 0 ? "&" : "?") + encoded;
    }

    /// <summary>
    /// Reads a query back out of the readable form.
    /// <para>
    /// Reading is partial where writing is total: text can say things this model has no room for,
    /// and anything it cannot hold is refused rather than quietly dropped.
    /// </para>
    /// </summary>
    /// <param name="query">The query string to read.</param>
    /// <param name="schema">
    /// The schema to check the result against. Null skips the check, which is what a caller wants
    /// when it does not yet know which schema the text belongs to.
    /// </param>
    /// <exception cref="QueryParseException">The text does not read as a query.</exception>
    /// <exception cref="QueryValidationException">It reads, but says nothing coherent about the schema.</exception>
    public static QuerySpec Parse(string query, QuerySchema? schema = null)
    {
        var spec = QueryHttpReader.Read(query);
        if (schema is not null) spec.Validate(schema).ThrowIfInvalid();
        return spec;
    }

    /// <summary>
    /// Reads a query back out of a URL or an encoded query string, undoing the percent-encoding
    /// first. Use this on what arrived over the wire, and <see cref="Parse"/> on what was stored.
    /// </summary>
    /// <param name="uri">The URL, or just its query string.</param>
    /// <param name="schema">The schema to check the result against. Null skips the check.</param>
    /// <exception cref="QueryParseException">The text does not read as a query.</exception>
    public static QuerySpec ParseUri(string uri, QuerySchema? schema = null)
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        var start = uri.IndexOf('?');
        var query = start < 0 ? uri : uri.Substring(start + 1);

        var decoded = new StringBuilder();
        foreach (var pair in query.Split('&'))
        {
            if (decoded.Length > 0) decoded.Append('&');
            var split = pair.IndexOf('=');
            if (split < 0) { decoded.Append(pair); continue; }
            decoded.Append(pair, 0, split + 1).Append(Uri.UnescapeDataString(pair.Substring(split + 1)));
        }
        return Parse(decoded.ToString(), schema);
    }
}
