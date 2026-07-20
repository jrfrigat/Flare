using System.Collections.Generic;

namespace Querio;

/// <summary>
/// Builds a <see cref="QuerySpec"/> in code. It produces exactly the same object a visual designer
/// emits, so a query can be started in code and finished in a designer, or the other way round.
/// <para>
/// Passing a <see cref="QuerySchema"/> is optional but worthwhile: with one, a join can find its
/// relation on its own when only one path connects the two entities.
/// </para>
/// </summary>
/// <example>
/// <code>
/// var spec = QueryBuilder.From(schema, "requests", "r")
///     .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day).Equal("r", "error", true))
///     .GroupBy("r", "route")
///     .CountRows("total")
///     .Percentile("r", "durationMs", 0.95, "p95")
///     .OrderBySelectDescending("total")
///     .Limit(100)
///     .Build();
/// </code>
/// </example>
public sealed class QueryBuilder
{
    private readonly QuerySchema? _schema;
    private readonly QuerySource _from;
    private readonly List<QueryJoin> _joins = [];
    private readonly List<QuerySelect> _select = [];
    private readonly List<QueryGroupBy> _groupBy = [];
    private readonly List<QuerySort> _orderBy = [];
    private readonly List<KeyValuePair<string, string>> _participants = [];
    private readonly HashSet<string> _aliases = new(StringComparer.OrdinalIgnoreCase);
    private QueryFilterBuilder? _where;
    private QueryFilterBuilder? _having;
    private bool _distinct;
    private int? _limit;
    private int? _offset;

    private QueryBuilder(QuerySchema? schema, string entity, string? alias)
    {
        _schema = schema;
        var rootAlias = TakeAlias(entity, alias);
        _from = new QuerySource(entity, rootAlias);
        _participants.Add(new KeyValuePair<string, string>(entity, rootAlias));
    }

    private QueryBuilder(QuerySchema? schema, QueryFunctionCall call, string? alias)
    {
        _schema = schema;
        var rootAlias = TakeAlias(call.Function, alias);
        _from = QuerySource.FromFunction(call, rootAlias);
        // A function participant belongs to no entity, so nothing can be inferred as reaching it.
        _participants.Add(new KeyValuePair<string, string>(string.Empty, rootAlias));
    }

    /// <summary>Alias assigned to the root participant, useful when it was generated rather than given.</summary>
    public string RootAlias => _from.Alias;

    /// <summary>Starts a query without a schema. Joins must then name their relation explicitly.</summary>
    /// <param name="entity">Key of the entity to draw from.</param>
    /// <param name="alias">Alias for it. Generated from the entity key when omitted.</param>
    public static QueryBuilder From(string entity, string? alias = null) => new(null, entity, alias);

    /// <summary>Starts a query against a schema, which lets joins resolve their own relation.</summary>
    /// <param name="schema">The schema the query is built against.</param>
    /// <param name="entity">Key of the entity to draw from.</param>
    /// <param name="alias">Alias for it. Generated from the entity key when omitted.</param>
    public static QueryBuilder From(QuerySchema schema, string entity, string? alias = null)
        => new(schema, entity, alias);

    /// <summary>Starts a query that draws its rows from a table function.</summary>
    /// <param name="schema">The schema the query is built against.</param>
    /// <param name="call">The table function call supplying the rows.</param>
    /// <param name="alias">Alias for it. Generated from the function name when omitted.</param>
    public static QueryBuilder FromFunction(QuerySchema schema, QueryFunctionCall call, string? alias = null)
        => new(schema, call, alias);

    /// <summary>
    /// Brings a table function into the query. A function has no declared relations, so the match
    /// conditions are always given explicitly.
    /// </summary>
    /// <param name="call">The table function call supplying the rows.</param>
    /// <param name="alias">Alias for it.</param>
    /// <param name="on">The field pairs that must match.</param>
    /// <param name="kind">How unmatched rows are treated. Defaults to an inner join.</param>
    public QueryBuilder JoinFunction(
        QueryFunctionCall call, string alias, IReadOnlyList<QueryJoinCondition> on,
        QueryJoinKind kind = QueryJoinKind.Inner)
    {
        var joinAlias = TakeAlias(call.Function, alias);
        _joins.Add(new QueryJoin(null, joinAlias) { Kind = kind, Call = call, On = on });
        _participants.Add(new KeyValuePair<string, string>(string.Empty, joinAlias));
        return this;
    }

    /// <summary>Returns the result of a value function.</summary>
    /// <param name="call">The value function call.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder SelectCall(QueryFunctionCall call, string? outputAlias = null)
    {
        _select.Add(new QuerySelect { Call = call, Alias = outputAlias });
        return this;
    }

    /// <summary>Applies an aggregate to the result of a value function.</summary>
    /// <param name="aggregate">The aggregate to compute.</param>
    /// <param name="call">The value function call.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder AggregateCall(
        QueryAggregate aggregate, QueryFunctionCall call, string? outputAlias = null)
    {
        _select.Add(new QuerySelect { Call = call, Aggregate = aggregate, Alias = outputAlias });
        return this;
    }

    /// <summary>Groups by the result of a value function.</summary>
    /// <param name="call">The value function call.</param>
    /// <param name="outputAlias">Name for the grouping column.</param>
    public QueryBuilder GroupByCall(QueryFunctionCall call, string? outputAlias = null)
    {
        _groupBy.Add(new QueryGroupBy(null) { Call = call, Alias = outputAlias });
        return this;
    }

    /// <summary>Orders by the result of a value function.</summary>
    /// <param name="call">The value function call.</param>
    /// <param name="direction">Which way the ordering runs.</param>
    public QueryBuilder OrderByCall(
        QueryFunctionCall call, QuerySortDirection direction = QuerySortDirection.Ascending)
    {
        _orderBy.Add(new QuerySort { Call = call, Direction = direction });
        return this;
    }

    /// <summary>
    /// Brings another entity into the query. When <paramref name="relation"/> is omitted and a schema
    /// was supplied, the relation is inferred - but only when exactly one path connects the new
    /// entity to one already present, so an ambiguous join is never silently guessed.
    /// </summary>
    /// <param name="entity">Key of the entity to join in.</param>
    /// <param name="alias">Alias for it. Generated from the entity key when omitted.</param>
    /// <param name="relation">Key of the schema relation to traverse.</param>
    /// <param name="kind">How unmatched rows are treated. Defaults to an inner join.</param>
    /// <param name="from">Alias of the participant to attach to, when more than one could be meant.</param>
    public QueryBuilder Join(
        string entity, string? alias = null, string? relation = null,
        QueryJoinKind kind = QueryJoinKind.Inner, string? from = null)
    {
        var joinAlias = TakeAlias(entity, alias);
        _joins.Add(new QueryJoin(entity, joinAlias)
        {
            Kind = kind,
            Relation = relation ?? InferRelation(entity),
            From = from,
        });
        _participants.Add(new KeyValuePair<string, string>(entity, joinAlias));
        return this;
    }

    /// <summary>Brings another entity in, keeping rows that find no match on the other side.</summary>
    /// <param name="entity">Key of the entity to join in.</param>
    /// <param name="alias">Alias for it. Generated from the entity key when omitted.</param>
    /// <param name="relation">Key of the schema relation to traverse.</param>
    public QueryBuilder LeftJoin(string entity, string? alias = null, string? relation = null)
        => Join(entity, alias, relation, QueryJoinKind.Left);

    /// <summary>Brings another entity in on explicit field matches, for a join the schema has not declared.</summary>
    /// <param name="entity">Key of the entity to join in.</param>
    /// <param name="alias">Alias for it.</param>
    /// <param name="on">The field pairs that must match.</param>
    /// <param name="kind">How unmatched rows are treated. Defaults to an inner join.</param>
    public QueryBuilder JoinOn(
        string entity, string alias, IReadOnlyList<QueryJoinCondition> on,
        QueryJoinKind kind = QueryJoinKind.Inner)
    {
        var joinAlias = TakeAlias(entity, alias);
        _joins.Add(new QueryJoin(entity, joinAlias) { Kind = kind, On = on });
        _participants.Add(new KeyValuePair<string, string>(entity, joinAlias));
        return this;
    }

    /// <summary>Returns a field as it stands.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder Select(string alias, string field, string? outputAlias = null)
    {
        _select.Add(new QuerySelect { Field = new QueryFieldRef(alias, field), Alias = outputAlias });
        return this;
    }

    /// <summary>
    /// Returns a timestamp collapsed to the start of its period. Pair it with the matching
    /// <see cref="GroupByPeriod"/> to build a time series.
    /// </summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="truncate">The period to collapse each timestamp into.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder SelectPeriod(
        string alias, string field, QueryDateTruncation truncate, string? outputAlias = null)
    {
        _select.Add(new QuerySelect
        {
            Field = new QueryFieldRef(alias, field),
            Truncate = truncate,
            Alias = outputAlias,
        });
        return this;
    }

    /// <summary>
    /// Returns a timestamp collapsed to its day and groups by the same, the usual shape of a daily
    /// trend.
    /// </summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder SelectAndGroupByDay(string alias, string field, string? outputAlias = null)
        => SelectPeriod(alias, field, QueryDateTruncation.Day, outputAlias)
            .GroupByPeriod(alias, field, QueryDateTruncation.Day);

    /// <summary>Counts rows in each group, rather than values of a field.</summary>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder CountRows(string? outputAlias = null)
    {
        _select.Add(new QuerySelect { Aggregate = QueryAggregate.Count, Alias = outputAlias });
        return this;
    }

    /// <summary>Counts values of a field, optionally only the distinct ones.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="distinct">Counts each repeated value once.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder Count(string alias, string field, bool distinct = false, string? outputAlias = null)
    {
        _select.Add(new QuerySelect
        {
            Field = new QueryFieldRef(alias, field),
            Aggregate = QueryAggregate.Count,
            Distinct = distinct,
            Alias = outputAlias,
        });
        return this;
    }

    /// <summary>Totals a numeric field across each group.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder Sum(string alias, string field, string? outputAlias = null)
        => Aggregate(QueryAggregate.Sum, alias, field, outputAlias);

    /// <inheritdoc cref="Sum"/>
    public QueryBuilder Avg(string alias, string field, string? outputAlias = null)
        => Aggregate(QueryAggregate.Avg, alias, field, outputAlias);

    /// <inheritdoc cref="Sum"/>
    public QueryBuilder Min(string alias, string field, string? outputAlias = null)
        => Aggregate(QueryAggregate.Min, alias, field, outputAlias);

    /// <inheritdoc cref="Sum"/>
    public QueryBuilder Max(string alias, string field, string? outputAlias = null)
        => Aggregate(QueryAggregate.Max, alias, field, outputAlias);

    /// <summary>Computes a percentile of a numeric field across each group.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="rank">The rank as a fraction: 0.95 means the 95th percentile.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder Percentile(string alias, string field, double rank, string? outputAlias = null)
    {
        _select.Add(new QuerySelect
        {
            Field = new QueryFieldRef(alias, field),
            Aggregate = QueryAggregate.Percentile,
            Percentile = rank,
            Alias = outputAlias,
        });
        return this;
    }

    /// <summary>Applies any aggregate to a field.</summary>
    /// <param name="aggregate">The aggregate to compute.</param>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the returned column.</param>
    public QueryBuilder Aggregate(
        QueryAggregate aggregate, string alias, string field, string? outputAlias = null)
    {
        _select.Add(new QuerySelect
        {
            Field = new QueryFieldRef(alias, field),
            Aggregate = aggregate,
            Alias = outputAlias,
        });
        return this;
    }

    /// <summary>
    /// Adds conditions on rows. Calling this more than once keeps adding to the same node, so the
    /// conditions accumulate with AND rather than the later call replacing the earlier one.
    /// </summary>
    /// <param name="configure">Builds the conditions.</param>
    public QueryBuilder Where(Action<QueryFilterBuilder> configure)
    {
        configure?.Invoke(_where ??= new QueryFilterBuilder());
        return this;
    }

    /// <summary>Adds conditions on groups, applied after aggregation. Accumulates like <see cref="Where"/>.</summary>
    /// <param name="configure">Builds the conditions.</param>
    public QueryBuilder Having(Action<QueryFilterBuilder> configure)
    {
        configure?.Invoke(_having ??= new QueryFilterBuilder());
        return this;
    }

    /// <summary>Groups by a field's exact value.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the grouping column.</param>
    public QueryBuilder GroupBy(string alias, string field, string? outputAlias = null)
    {
        _groupBy.Add(new QueryGroupBy(new QueryFieldRef(alias, field)) { Alias = outputAlias });
        return this;
    }

    /// <summary>Groups a timestamp into periods, which is what turns rows into a time series.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="truncate">The period to collapse each timestamp into.</param>
    /// <param name="outputAlias">Name for the grouping column.</param>
    public QueryBuilder GroupByPeriod(
        string alias, string field, QueryDateTruncation truncate, string? outputAlias = null)
    {
        _groupBy.Add(new QueryGroupBy(new QueryFieldRef(alias, field))
        {
            Truncate = truncate,
            Alias = outputAlias,
        });
        return this;
    }

    /// <summary>Groups a timestamp by day.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the grouping column.</param>
    public QueryBuilder GroupByDay(string alias, string field, string? outputAlias = null)
        => GroupByPeriod(alias, field, QueryDateTruncation.Day, outputAlias);

    /// <summary>Groups a timestamp by hour.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="outputAlias">Name for the grouping column.</param>
    public QueryBuilder GroupByHour(string alias, string field, string? outputAlias = null)
        => GroupByPeriod(alias, field, QueryDateTruncation.Hour, outputAlias);

    /// <summary>Orders by a field.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="direction">Which way the ordering runs.</param>
    public QueryBuilder OrderBy(
        string alias, string field, QuerySortDirection direction = QuerySortDirection.Ascending)
    {
        _orderBy.Add(new QuerySort { Field = new QueryFieldRef(alias, field), Direction = direction });
        return this;
    }

    /// <summary>Orders by a field, largest first.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    public QueryBuilder OrderByDescending(string alias, string field)
        => OrderBy(alias, field, QuerySortDirection.Descending);

    /// <summary>Orders by the output name of a selected item, such as a computed aggregate.</summary>
    /// <param name="outputAlias">Output name of the selected item.</param>
    /// <param name="direction">Which way the ordering runs.</param>
    public QueryBuilder OrderBySelect(
        string outputAlias, QuerySortDirection direction = QuerySortDirection.Ascending)
    {
        _orderBy.Add(new QuerySort { Select = outputAlias, Direction = direction });
        return this;
    }

    /// <summary>Orders by the output name of a selected item, largest first.</summary>
    /// <param name="outputAlias">Output name of the selected item.</param>
    public QueryBuilder OrderBySelectDescending(string outputAlias)
        => OrderBySelect(outputAlias, QuerySortDirection.Descending);

    /// <summary>Drops duplicate rows from the result.</summary>
    /// <param name="value">Whether duplicates are dropped.</param>
    public QueryBuilder Distinct(bool value = true)
    {
        _distinct = value;
        return this;
    }

    /// <summary>Caps how many rows come back.</summary>
    /// <param name="count">Maximum number of rows.</param>
    public QueryBuilder Limit(int count)
    {
        _limit = count;
        return this;
    }

    /// <summary>Skips this many rows before returning any.</summary>
    /// <param name="count">Number of rows to skip.</param>
    public QueryBuilder Offset(int count)
    {
        _offset = count;
        return this;
    }

    /// <summary>Materializes the query built so far.</summary>
    public QuerySpec Build()
    {
        var where = _where?.Build();
        var having = _having?.Build();
        return new QuerySpec(_from)
        {
            Joins = _joins.ToArray(),
            Select = _select.ToArray(),
            Where = where is null || where.IsEmpty ? null : where,
            GroupBy = _groupBy.ToArray(),
            Having = having is null || having.IsEmpty ? null : having,
            OrderBy = _orderBy.ToArray(),
            Distinct = _distinct,
            Limit = _limit,
            Offset = _offset,
        };
    }

    // Finds the one relation connecting the new entity to something already in the query. Returns
    // null when there is no candidate or more than one, leaving the join for the validator to report
    // rather than picking a path the caller did not ask for.
    private string? InferRelation(string entity)
    {
        if (_schema is null) return null;
        string? found = null;
        foreach (var relation in _schema.Relations)
        {
            if (!Connects(relation, entity)) continue;
            if (found is not null) return null;
            found = relation.Key;
        }
        return found;
    }

    private bool Connects(QueryRelation relation, string entity)
    {
        var joinsNew =
            string.Equals(relation.From, entity, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(relation.To, entity, StringComparison.OrdinalIgnoreCase);
        if (!joinsNew) return false;

        // The other end has to be an entity the query already contains, otherwise the relation
        // describes a path that does not reach this query at all.
        var other = string.Equals(relation.From, entity, StringComparison.OrdinalIgnoreCase)
            ? relation.To
            : relation.From;
        foreach (var participant in _participants)
        {
            if (string.Equals(participant.Key, other, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    // Registers an alias, generating one from the entity key when the caller did not supply it and
    // suffixing digits until it is unique - so a self-join gets distinct aliases without ceremony.
    private string TakeAlias(string entity, string? requested)
    {
        if (!string.IsNullOrEmpty(requested))
        {
            _aliases.Add(requested!);
            return requested!;
        }

        var seed = string.Empty;
        for (var i = 0; i < entity.Length; i++)
        {
            if (char.IsLetter(entity[i]))
            {
                seed = char.ToLowerInvariant(entity[i]).ToString();
                break;
            }
        }
        if (seed.Length == 0) seed = "t";

        var candidate = seed;
        var suffix = 1;
        while (_aliases.Contains(candidate))
        {
            suffix++;
            candidate = seed + suffix.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        _aliases.Add(candidate);
        return candidate;
    }
}
