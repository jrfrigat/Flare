namespace Querio.Language;

/// <summary>
/// The SQL-shaped language a person writes a Querio query in.
/// <para>
/// It looks like SQL because that is what people already know, but it is not SQL and does not
/// pretend to be. It names entities and fields the way the schema declares them, so the same text
/// still means the same query against a differently-named store; it accepts the physical names too,
/// because somebody reading a database writes what they can see. And it can travel a foreign key,
/// any number of hops, which no SQL can:
/// </para>
/// <code>
/// select [r].[route], [r].[apiKeyId].[ownerId].[name] as [owner]
/// from [dbo].[RequestLog] as [r]
/// where [r].[timestamp] >= now - 30 day and [r].[error] = true
/// order by [r].[timestamp] desc
/// limit 20
/// </code>
/// <para>
/// Each hop becomes a join, which the target then renders however suits it. That is why writing a
/// query back out shows the joins rather than the dots: the sugar has been spent.
/// </para>
/// </summary>
public static class QueryLanguage
{
    /// <summary>
    /// Reads query text, reporting everything wrong with it rather than stopping at the first
    /// fault, and returning whatever query could be made from the rest.
    /// </summary>
    /// <param name="text">The query text.</param>
    /// <param name="schema">The schema it is written against.</param>
    public static QueryParseResult Read(string text, QuerySchema schema)
    {
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        return QueryLanguageReader.Read(text ?? string.Empty, schema);
    }

    /// <summary>
    /// Reads query text, refusing anything that is not a whole coherent query. For a caller that
    /// wants a query or nothing; an editor should use <see cref="Read"/>.
    /// </summary>
    /// <param name="text">The query text.</param>
    /// <param name="schema">The schema it is written against.</param>
    /// <exception cref="QueryParseException">The text does not read as a query.</exception>
    /// <exception cref="QueryValidationException">It reads, but says nothing coherent about the schema.</exception>
    public static QuerySpec Parse(string text, QuerySchema schema)
    {
        var spec = Read(text, schema).Require();
        spec.Validate(schema).ThrowIfInvalid();
        return spec;
    }

    /// <summary>
    /// Writes a query as text, so a query built in the designer or in code opens in the editor.
    /// <para>
    /// Joins are written out, the ones a foreign-key path produced included: once a hop has become a
    /// join, nothing records that it was typed as a dot. Reading this text back yields the same
    /// query, but not the same characters somebody originally typed.
    /// </para>
    /// </summary>
    /// <param name="spec">The query to write.</param>
    /// <param name="schema">The schema it was built against.</param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    public static string Write(QuerySpec spec, QuerySchema schema)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        return new QueryLanguageWriter(spec, schema).Run();
    }
}
