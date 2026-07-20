using System.Linq.Expressions;

namespace Querio.Linq;

/// <summary>
/// Turns a query's conditions into a predicate over one type.
/// <para>
/// This is the piece that reaches a real database without this package knowing about one. Entity
/// Framework, and every other LINQ provider, translates expression trees itself - so a saved query
/// can be handed straight to <c>Where</c> and become SQL the provider wrote, against the provider's
/// own mapping, with its own parameterisation.
/// </para>
/// </summary>
public static class QueryPredicate
{
    /// <summary>
    /// Builds a predicate from the query's conditions, over the type standing for one participant.
    /// <para>
    /// Only the conditions are used: joining, grouping and ordering are things a caller expresses in
    /// LINQ itself, and pretending to own them here would take away the query the caller is building.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type the predicate tests.</typeparam>
    /// <param name="spec">The query whose conditions to take.</param>
    /// <param name="schema">The schema it was built against.</param>
    /// <param name="alias">
    /// Which participant <typeparamref name="T"/> stands for. Null uses the query's root.
    /// </param>
    /// <param name="functions">The .NET behind any functions the conditions call.</param>
    /// <param name="now">
    /// The moment relative windows are measured from. Null uses the current UTC time, pinned into
    /// the predicate so it keeps meaning the same window wherever it is later evaluated.
    /// </param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    /// <exception cref="QueryRenderException">
    /// A condition reads a different participant, which one type cannot answer for, or the type has
    /// no member for a field the query names.
    /// </exception>
    public static Expression<Func<T, bool>> For<T>(
        QuerySpec spec,
        QuerySchema schema,
        string? alias = null,
        QueryFunctionLibrary? functions = null,
        DateTime? now = null)
        => new SingleTypePlan<T>(
            spec, schema, functions ?? QueryFunctionLibrary.Empty, now ?? DateTime.UtcNow, alias).Build();

    private sealed class SingleTypePlan<T> : QueryLinqRenderer
    {
        private readonly ParameterExpression _parameter = Expression.Parameter(typeof(T), "item");
        private readonly string? _requested;
        private string _alias = string.Empty;

        internal SingleTypePlan(
            QuerySpec spec, QuerySchema schema, QueryFunctionLibrary library, DateTime now, string? alias)
            : base(spec, schema, library, now) => _requested = alias;

        internal Expression<Func<T, bool>> Build()
        {
            Prepare(Capabilities);
            _alias = _requested ?? Spec.From.Alias;

            var body = Filter(Spec.Where);
            return Expression.Lambda<Func<T, bool>>(body ?? Expression.Constant(true), _parameter);
        }

        protected override Expression Participant(string alias)
        {
            if (string.Equals(alias, _alias, StringComparison.OrdinalIgnoreCase)) return _parameter;

            throw new QueryRenderException(
                $"A condition reads '{alias}', but a predicate over a single type can only answer for " +
                $"'{_alias}'. Run the query with {nameof(QueryExecutor)} instead, or express the join in LINQ.");
        }
    }
}
