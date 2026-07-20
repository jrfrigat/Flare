using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Querio.Linq;

/// <summary>
/// Runs a query over objects. Conditions, keys and values are compiled expression trees; joining,
/// grouping and paging are done here, because those are decisions about the shape of a result rather
/// than about the meaning of a value.
/// </summary>
internal sealed class QueryLinqPlan : QueryLinqRenderer
{
    private static readonly IComparer<object?> Ordering = Comparer<object?>.Create(QueryClrValue.Compare);

    private readonly QuerySources _sources;
    private readonly ParameterExpression _row = Expression.Parameter(typeof(object[]), "row");
    private readonly ParameterExpression _outputs = Expression.Parameter(typeof(object[]), "outputs");
    private readonly Dictionary<string, int> _slots = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _optional = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Type> _types = [];
    private readonly List<IEnumerable> _sequences = [];

    internal QueryLinqPlan(
        QuerySpec spec, QuerySchema schema, QuerySources sources, QueryFunctionLibrary library, DateTime now)
        : base(spec, schema, library, now) => _sources = sources;

    internal QueryResult Execute()
    {
        Prepare(Capabilities);
        Bind();

        var rows = Join();
        var where = Filter(Spec.Where);
        if (where is not null) rows = rows.Where(Predicate(where)).ToList();

        var plans = Outputs();
        var entries = Fold(rows, plans);
        entries = Restrict(entries, plans);
        entries = Sort(entries, plans);

        var results = entries.Select(entry => entry.Values).ToList();
        if (Spec.Distinct) results = Distinct(results);
        if (Spec.Offset is > 0) results = results.Skip(Spec.Offset.Value).ToList();
        if (Spec.Limit is not null) results = results.Take(Spec.Limit.Value).ToList();

        var columns = plans.Select(plan => new QueryResultColumn(plan.Name, plan.ClrType)).ToList();
        return new QueryResult(columns, results);
    }

    // ---- Binding ---------------------------------------------------------------------------------

    private void Bind()
    {
        for (var i = 0; i < Participants.Count; i++)
        {
            var participant = Participants[i];
            _slots[participant.Alias] = i;

            if (participant.EntityKey is not null)
            {
                if (!_sources.TryGet(participant.EntityKey, out var type, out var rows))
                {
                    throw new QueryRenderException(
                        $"No objects are bound to the entity '{participant.EntityKey}'. " +
                        $"Add them to {nameof(QuerySources)} before running the query.");
                }
                _types.Add(type);
                _sequences.Add(rows);
            }
            else
            {
                var call = i == 0 ? Spec.From.Call! : Spec.Joins[i - 1].Call!;
                var produced = Library.Table(call.Function, Arguments(call));
                _types.Add(produced.ElementType);
                _sequences.Add(produced.Rows);
            }

            if (i > 0 && Spec.Joins[i - 1].Kind == QueryJoinKind.Left) _optional.Add(participant.Alias);
        }
    }

    /// <summary>
    /// Reads a table function's arguments. They are settled before any row exists, so an argument
    /// that reads a field would have nothing to read from and is refused rather than guessed at.
    /// </summary>
    private IReadOnlyList<object?> Arguments(QueryFunctionCall call)
    {
        var function = Schema.FindFunction(call.Function)!;
        var values = new List<object?>(call.Arguments.Count);
        for (var i = 0; i < call.Arguments.Count; i++)
        {
            var argument = call.Arguments[i];
            if (argument.Kind == QueryOperandKind.Field)
            {
                throw new QueryRenderException(
                    $"The table function '{call.Function}' is given a field as an argument, which cannot be " +
                    "read before the query has rows.",
                    QueryFeature.TableFunctions);
            }
            var type = i < function.Parameters.Count ? function.Parameters[i].Type : QueryFieldType.Text;
            var built = Operand(argument, type);
            values.Add(Expression.Lambda<Func<object?>>(Expression.Convert(built, typeof(object))).Compile()());
        }
        return values;
    }

    /// <inheritdoc/>
    protected override Expression Participant(string alias)
        => Expression.Convert(Slot(alias), _types[_slots[alias]]);

    /// <inheritdoc/>
    protected override Expression MemberAccess(string alias, Expression instance, MemberInfo member)
    {
        var access = Expression.MakeMemberAccess(instance, member);
        if (!_optional.Contains(alias)) return access;

        // An outer join can leave the participant absent, and reading a field of something that is
        // not there yields nothing - which is why the result type has to admit nothing.
        var type = QueryClrValue.AcceptsNull(access.Type)
            ? access.Type
            : typeof(Nullable<>).MakeGenericType(access.Type);
        return Expression.Condition(
            Expression.Equal(Slot(alias), Expression.Constant(null, typeof(object))),
            Expression.Constant(null, type),
            QueryClrValue.Coerce(access, type));
    }

    private Expression Slot(string alias)
        => Expression.ArrayIndex(_row, Expression.Constant(_slots[alias]));

    // ---- Shaping the rows ------------------------------------------------------------------------

    private List<object?[]> Join()
    {
        var width = Participants.Count;
        var rows = new List<object?[]>();
        foreach (var item in _sequences[0])
        {
            var row = new object?[width];
            row[0] = item;
            rows.Add(row);
        }

        for (var i = 0; i < Spec.Joins.Count; i++)
        {
            var join = Spec.Joins[i];
            var slot = i + 1;
            var candidates = _sequences[slot].Cast<object?>().ToList();
            var matches = join.Kind == QueryJoinKind.Cross ? null : Predicate(Match(join, i));

            var next = new List<object?[]>(rows.Count);
            foreach (var row in rows)
            {
                var paired = false;
                foreach (var candidate in candidates)
                {
                    var probe = (object?[])row.Clone();
                    probe[slot] = candidate;
                    if (matches is not null && !matches(probe)) continue;
                    next.Add(probe);
                    paired = true;
                }
                // Left keeps the row it already had, with nothing standing in the new slot.
                if (!paired && join.Kind == QueryJoinKind.Left) next.Add((object?[])row.Clone());
            }
            rows = next;
        }
        return rows;
    }

    private Expression Match(QueryJoin join, int index)
    {
        Expression? test = null;
        foreach (var pair in JoinMatches(join, index))
        {
            var left = Field(pair.LeftAlias, FindField(pair.LeftAlias, pair.LeftField)!);
            var right = Field(pair.RightAlias, FindField(pair.RightAlias, pair.RightField)!);
            var (a, b) = QueryClrValue.Align(left, right);
            var equal = Expression.Equal(a, b);
            test = test is null ? equal : Expression.AndAlso(test, equal);
        }
        return test ?? Expression.Constant(true);
    }

    private Func<object?[], bool> Predicate(Expression body)
        => Expression.Lambda<Func<object?[], bool>>(body, _row).Compile();

    private Func<object?[], object?> Selector(Expression body)
        => Expression.Lambda<Func<object?[], object?>>(Expression.Convert(body, typeof(object)), _row).Compile();

    // ---- Producing the output --------------------------------------------------------------------

    private sealed class OutputPlan
    {
        internal string Name = string.Empty;
        internal Type ClrType = typeof(object);
        internal QueryFieldType SemanticType = QueryFieldType.Text;
        internal QuerySelect? Item;
        internal Func<object?[], object?>? Read;
    }

    private sealed class Entry
    {
        internal object?[] Values = [];
        internal object?[]? Representative;
    }

    private List<OutputPlan> Outputs()
    {
        var items = Spec.Select.Count > 0 ? Spec.Select : Everything();
        var plans = new List<OutputPlan>(items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var plan = new OutputPlan { Item = item, Name = item.Alias ?? $"column{i + 1}" };

            if (item.Aggregate is QueryAggregate.Count)
            {
                plan.ClrType = typeof(long);
                plan.SemanticType = QueryFieldType.Number;
            }
            else if (item.Aggregate is QueryAggregate.Sum or QueryAggregate.Avg or QueryAggregate.Percentile)
            {
                // An aggregate over a store's whole numbers is not itself a whole number, and an
                // aggregate over no rows is nothing at all rather than zero.
                plan.ClrType = typeof(double?);
                plan.SemanticType = QueryFieldType.Number;
            }
            else
            {
                var value = ValueOf(item.Field, item.Call, item.Truncate);
                plan.ClrType = item.Aggregate is null ? value.Type : Optional(value.Type);
                plan.SemanticType = item.Truncate is not null
                    ? QueryFieldType.DateTime
                    : ValueType(item.Field, item.Call);
            }

            if (item.Field is not null || item.Call is not null)
            {
                plan.Read = Selector(ValueOf(item.Field, item.Call, item.Truncate));
            }
            plans.Add(plan);
        }
        return plans;
    }

    /// <summary>The type widened so it can also hold nothing, since a group may have no rows.</summary>
    private static Type Optional(Type type)
        => type.IsValueType && Nullable.GetUnderlyingType(type) is null
            ? typeof(Nullable<>).MakeGenericType(type)
            : type;

    /// <summary>Every field of every participant, which is what a query selecting nothing means.</summary>
    private List<QuerySelect> Everything()
    {
        var items = new List<QuerySelect>();
        foreach (var participant in Participants)
        {
            foreach (var field in participant.Fields)
            {
                items.Add(new QuerySelect
                {
                    Field = new QueryFieldRef(participant.Alias, field.Key),
                    Alias = $"{participant.Alias}.{field.Key}",
                });
            }
        }
        return items;
    }

    private List<Entry> Fold(List<object?[]> rows, List<OutputPlan> plans)
    {
        var aggregating = plans.Any(plan => plan.Item?.Aggregate is not null);
        if (!aggregating && Spec.GroupBy.Count == 0)
        {
            return rows.Select(row => Compute(row, [row], plans)).ToList();
        }

        if (Spec.GroupBy.Count == 0)
        {
            // No keys at all still yields one row, the way a bare total does.
            return [Compute(rows.Count > 0 ? rows[0] : null, rows, plans)];
        }

        var keys = Spec.GroupBy.Select(group => Selector(ValueOf(group.Field, group.Call, group.Truncate))).ToList();
        var buckets = new List<(object?[] Key, List<object?[]> Rows)>();
        var lookup = new Dictionary<GroupKey, int>();
        foreach (var row in rows)
        {
            var key = keys.Select(read => read(row)).ToArray();
            var identity = new GroupKey(key);
            if (!lookup.TryGetValue(identity, out var at))
            {
                at = buckets.Count;
                lookup[identity] = at;
                buckets.Add((key, []));
            }
            buckets[at].Rows.Add(row);
        }
        return buckets.Select(bucket => Compute(bucket.Rows[0], bucket.Rows, plans)).ToList();
    }

    private static Entry Compute(object?[]? representative, List<object?[]> members, List<OutputPlan> plans)
    {
        var values = new object?[plans.Count];
        for (var i = 0; i < plans.Count; i++)
        {
            var plan = plans[i];
            var item = plan.Item!;
            if (item.Aggregate is null)
            {
                values[i] = representative is null ? null : plan.Read!(representative);
                continue;
            }
            values[i] = QueryAggregates.Compute(item, plan.Read, members);
        }
        return new Entry { Values = values, Representative = representative };
    }

    private List<Entry> Restrict(List<Entry> entries, List<OutputPlan> plans)
    {
        if (Spec.Having is null) return entries;

        for (var i = 0; i < plans.Count; i++)
        {
            OutputExpressions[plans[i].Name] =
                Expression.Convert(Expression.ArrayIndex(_outputs, Expression.Constant(i)), plans[i].ClrType);
            OutputTypes[plans[i].Name] = plans[i].SemanticType;
        }

        var body = Filter(Spec.Having);
        if (body is null) return entries;

        var test = Expression.Lambda<Func<object?[], object?[], bool>>(body, _row, _outputs).Compile();
        return entries.Where(entry => test(entry.Representative ?? new object?[Participants.Count], entry.Values)).ToList();
    }

    private List<Entry> Sort(List<Entry> entries, List<OutputPlan> plans)
    {
        if (Spec.OrderBy.Count == 0) return entries;

        IOrderedEnumerable<Entry>? ordered = null;
        foreach (var sort in Spec.OrderBy)
        {
            Func<Entry, object?> key;
            if (!string.IsNullOrEmpty(sort.Select))
            {
                var at = plans.FindIndex(plan =>
                    string.Equals(plan.Name, sort.Select, StringComparison.OrdinalIgnoreCase));
                if (at < 0) throw new QueryRenderException($"Nothing selected is named '{sort.Select}' to order by.");
                key = entry => entry.Values[at];
            }
            else
            {
                var read = Selector(Value(sort.Field, sort.Call));
                key = entry => entry.Representative is null ? null : read(entry.Representative);
            }

            var down = sort.Direction == QuerySortDirection.Descending;
            ordered = ordered is null
                ? down ? entries.OrderByDescending(key, Ordering) : entries.OrderBy(key, Ordering)
                : down ? ordered.ThenByDescending(key, Ordering) : ordered.ThenBy(key, Ordering);
        }
        return ordered!.ToList();
    }

    private static List<object?[]> Distinct(List<object?[]> rows)
    {
        var seen = new HashSet<GroupKey>();
        var kept = new List<object?[]>(rows.Count);
        foreach (var row in rows)
        {
            if (seen.Add(new GroupKey(row))) kept.Add(row);
        }
        return kept;
    }

    /// <summary>A row of values compared by what it holds, so equal keys land in the same group.</summary>
    private readonly struct GroupKey : IEquatable<GroupKey>
    {
        private readonly object?[] _values;

        internal GroupKey(object?[] values) => _values = values;

        public bool Equals(GroupKey other)
        {
            if (_values.Length != other._values.Length) return false;
            for (var i = 0; i < _values.Length; i++)
            {
                if (!Equals(_values[i], other._values[i])) return false;
            }
            return true;
        }

        public override bool Equals(object? obj) => obj is GroupKey other && Equals(other);

        public override int GetHashCode()
        {
            var hash = 17;
            foreach (var value in _values) hash = (hash * 31) + (value?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
