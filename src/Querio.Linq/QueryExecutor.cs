namespace Querio.Linq;

/// <summary>
/// Runs a query against objects.
/// <para>
/// This is the target that keeps the model honest. A SQL renderer could quietly assume the model is
/// SQL-shaped and nobody would notice; running the same query as .NET code cannot, because there is
/// no query language to hide behind. Anything the model can express has to mean something here too.
/// </para>
/// </summary>
public static class QueryExecutor
{
    /// <summary>
    /// What running a query as .NET code can express. Only the outer joins that keep unmatched rows
    /// from the side being added are missing: a sequence of objects has no natural shape for them.
    /// </summary>
    public static IQueryCapabilities Capabilities => QueryLinqRenderer.Capabilities;

    /// <summary>Runs a query and returns its rows.</summary>
    /// <param name="spec">The query to run.</param>
    /// <param name="schema">The schema it was built against.</param>
    /// <param name="sources">The objects standing for each entity the query names.</param>
    /// <param name="functions">The .NET behind any functions the query calls. Null when it calls none.</param>
    /// <param name="now">
    /// The moment relative windows are measured from. Null uses the current UTC time, pinned once so
    /// every condition in the query agrees on when "now" was.
    /// </param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    /// <exception cref="QueryRenderException">
    /// The query needs something this target cannot do, an entity has no objects bound to it, or a
    /// function it calls has no registered implementation.
    /// </exception>
    public static QueryResult Execute(
        QuerySpec spec,
        QuerySchema schema,
        QuerySources sources,
        QueryFunctionLibrary? functions = null,
        DateTime? now = null)
    {
        if (sources is null) throw new ArgumentNullException(nameof(sources));
        return new QueryLinqPlan(
            spec, schema, sources, functions ?? QueryFunctionLibrary.Empty, now ?? DateTime.UtcNow).Execute();
    }
}
