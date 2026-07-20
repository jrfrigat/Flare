using System.Globalization;
using Querio;

namespace Flare.Components;

/// <summary>
/// The editable shape of a query, as the designer manipulates it while a person is still making up
/// their mind. It is deliberately mutable and forgiving - half-filled rows are normal mid-edit -
/// whereas <see cref="QuerySpec"/> is immutable and complete. <see cref="ToSpec"/> converts one to
/// the other, dropping whatever is not finished yet.
/// </summary>
public sealed class QueryDraft
{
    /// <summary>Key of the entity the query draws from, when its source is an entity.</summary>
    public string RootEntity { get; set; } = string.Empty;

    /// <summary>Key of the table function the query draws from, when its source is a function.</summary>
    public string? RootFunction { get; set; }

    /// <summary>Arguments for <see cref="RootFunction"/>.</summary>
    public List<QueryArgumentDraft> RootArguments { get; } = [];

    /// <summary>Alias the source is referred to by.</summary>
    public string RootAlias { get; set; } = string.Empty;

    /// <summary>Entities brought into the query alongside the root.</summary>
    public List<QueryJoinDraft> Joins { get; } = [];

    /// <summary>The rows of the column grid: what is returned, grouped and ordered.</summary>
    public List<QueryColumnDraft> Columns { get; } = [];

    /// <summary>Conditions applied to rows.</summary>
    public QueryConditionGroupDraft Where { get; set; } = new();

    /// <summary>Conditions applied to groups, which may test computed aggregates.</summary>
    public QueryConditionGroupDraft Having { get; set; } = new();

    /// <summary>Whether duplicate rows are dropped.</summary>
    public bool Distinct { get; set; }

    /// <summary>Maximum rows to return, or null for all of them.</summary>
    public int? Limit { get; set; }

    /// <summary>Rows to skip before returning any, or null to start at the first.</summary>
    public int? Offset { get; set; }

    /// <summary>Every participant currently in the query, root first, as alias and entity key.</summary>
    public IReadOnlyList<QueryParticipant> Participants
    {
        get
        {
            var participants = new List<QueryParticipant> { new(RootAlias, RootEntity, RootFunction) };
            foreach (var join in Joins) participants.Add(new QueryParticipant(join.Alias, join.Entity, null));
            return participants;
        }
    }

    /// <summary>Output names a HAVING condition or an ordering can refer to.</summary>
    public IReadOnlyList<string> OutputAliases
        => Columns.Where(column => !string.IsNullOrWhiteSpace(column.OutputAlias))
            .Select(column => column.OutputAlias!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>Converts the draft into a query, leaving out rows that are not filled in yet.</summary>
    public QuerySpec ToSpec()
    {
        var select = new List<QuerySelect>();
        var groupBy = new List<QueryGroupBy>();
        var orderBy = new List<QuerySort>();

        foreach (var column in Columns)
        {
            var call = column.ToCall();
            var reference = call is null ? column.ToFieldRef() : null;

            // A row count is the one thing that needs no value, so any other row without one is
            // still being filled in.
            var counting = column.Aggregate == QueryAggregate.Count && call is null && reference is null;
            if (call is null && reference is null && !counting) continue;

            select.Add(new QuerySelect
            {
                Field = reference,
                Call = call,
                Aggregate = column.Aggregate,
                Distinct = column.Distinct,
                Percentile = column.Aggregate == QueryAggregate.Percentile ? column.Percentile : null,
                Truncate = column.Aggregate is null ? column.Truncate : null,
                Alias = string.IsNullOrWhiteSpace(column.OutputAlias) ? null : column.OutputAlias,
            });

            if (column.Group && (reference is not null || call is not null))
            {
                groupBy.Add(new QueryGroupBy(reference) { Call = call, Truncate = column.Truncate });
            }

            if (column.Sort is not null)
            {
                orderBy.Add(!string.IsNullOrWhiteSpace(column.OutputAlias)
                    ? new QuerySort { Select = column.OutputAlias, Direction = column.Sort.Value }
                    : new QuerySort { Field = reference, Call = call, Direction = column.Sort.Value });
            }
        }

        var where = Where.ToGroup();
        var having = Having.ToGroup();

        return new QuerySpec(BuildSource())
        {
            Joins = Joins.Where(join => !string.IsNullOrWhiteSpace(join.Entity) && !string.IsNullOrWhiteSpace(join.Alias))
                .Select(join => new QueryJoin(join.Entity, join.Alias)
                {
                    Kind = join.Kind,
                    Relation = string.IsNullOrWhiteSpace(join.Relation) ? null : join.Relation,
                    From = string.IsNullOrWhiteSpace(join.From) ? null : join.From,
                })
                .ToList(),
            Select = select,
            Where = where.IsEmpty ? null : where,
            GroupBy = groupBy,
            Having = having.IsEmpty ? null : having,
            OrderBy = orderBy,
            Distinct = Distinct,
            Limit = Limit,
            Offset = Offset,
        };
    }

    private QuerySource BuildSource()
    {
        if (string.IsNullOrWhiteSpace(RootFunction)) return new QuerySource(RootEntity, RootAlias);

        var call = new QueryFunctionCall(RootFunction!)
        {
            Arguments = RootArguments.Select(argument => argument.ToOperand()).ToList(),
        };
        return QuerySource.FromFunction(call, RootAlias);
    }

    /// <summary>
    /// Rebuilds a draft from a query, so a spec built in code - or restored from storage - can be
    /// opened in the designer and carried on with.
    /// </summary>
    /// <param name="spec">The query to open. Null starts an empty draft.</param>
    /// <param name="schema">The schema the query is built against.</param>
    public static QueryDraft FromSpec(QuerySpec? spec, QuerySchema schema)
    {
        var draft = new QueryDraft();
        if (spec is null)
        {
            var first = schema.Entities.Count > 0 ? schema.Entities[0] : null;
            draft.RootEntity = first?.Key ?? string.Empty;
            draft.RootAlias = Initial(draft.RootEntity);
            return draft;
        }

        draft.RootEntity = spec.From.Entity ?? string.Empty;
        draft.RootAlias = spec.From.Alias;
        draft.Distinct = spec.Distinct;
        draft.Limit = spec.Limit;
        draft.Offset = spec.Offset;

        if (spec.From.Call is not null)
        {
            draft.RootFunction = spec.From.Call.Function;
            foreach (var argument in spec.From.Call.Arguments)
            {
                draft.RootArguments.Add(QueryArgumentDraft.FromOperand(argument));
            }
        }

        foreach (var join in spec.Joins)
        {
            draft.Joins.Add(new QueryJoinDraft
            {
                Entity = join.Entity ?? string.Empty,
                Alias = join.Alias,
                Kind = join.Kind,
                Relation = join.Relation,
                From = join.From,
            });
        }

        foreach (var item in spec.Select) draft.Columns.Add(QueryColumnDraft.FromSelect(item, spec));

        draft.Where = QueryConditionGroupDraft.FromGroup(spec.Where);
        draft.Having = QueryConditionGroupDraft.FromGroup(spec.Having);
        return draft;
    }

    /// <summary>Suggests an unused alias for an entity, the way the fluent builder would.</summary>
    /// <param name="entityKey">The entity being added.</param>
    public string SuggestAlias(string entityKey)
    {
        var seed = Initial(entityKey);
        var taken = Participants.Select(participant => participant.Alias).ToList();
        if (!taken.Contains(seed, StringComparer.OrdinalIgnoreCase)) return seed;

        for (var suffix = 2; ; suffix++)
        {
            var candidate = seed + suffix.ToString(CultureInfo.InvariantCulture);
            if (!taken.Contains(candidate, StringComparer.OrdinalIgnoreCase)) return candidate;
        }
    }

    private static string Initial(string entityKey)
    {
        foreach (var character in entityKey)
        {
            if (char.IsLetter(character)) return char.ToLowerInvariant(character).ToString();
        }
        return "t";
    }
}

/// <summary>One participant in the query: the alias it is referred to by and what stands behind it.</summary>
/// <param name="Alias">The alias field references use.</param>
/// <param name="Entity">Key of the entity, when the participant is one.</param>
/// <param name="Function">Key of the table function, when the participant is one.</param>
public sealed record QueryParticipant(string Alias, string Entity, string? Function = null);

/// <summary>
/// One argument of a function call, as the designer edits it: either a field of some participant or
/// a value typed in. The designer keeps arguments flat; nesting one call inside another stays
/// available in code, where it reads better than it would in a grid.
/// </summary>
public sealed class QueryArgumentDraft
{
    /// <summary>Whether the argument is a field rather than a typed value.</summary>
    public bool IsField { get; set; } = true;

    /// <summary>Alias of the participant the field belongs to.</summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>Logical field name.</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>The typed value, when the argument is not a field.</summary>
    public string? Value { get; set; }

    /// <summary>Converts the argument into an operand.</summary>
    public QueryOperand ToOperand()
        => IsField ? QueryOperand.Of(new QueryFieldRef(Alias, Field)) : QueryOperand.Literal(Value);

    /// <summary>Rebuilds an editable argument from an operand.</summary>
    /// <param name="operand">The operand to open.</param>
    public static QueryArgumentDraft FromOperand(QueryOperand operand) => operand.Kind switch
    {
        QueryOperandKind.Field => new QueryArgumentDraft
        {
            IsField = true,
            Alias = operand.Field?.Alias ?? string.Empty,
            Field = operand.Field?.Field ?? string.Empty,
        },
        _ => new QueryArgumentDraft { IsField = false, Value = operand.Value },
    };
}

/// <summary>An editable join row.</summary>
public sealed class QueryJoinDraft
{
    /// <summary>Key of the entity being joined in.</summary>
    public string Entity { get; set; } = string.Empty;

    /// <summary>Alias the joined entity is referred to by.</summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>Key of the schema relation traversed, when one is chosen.</summary>
    public string? Relation { get; set; }

    /// <summary>Alias of the participant this join attaches to, when more than one could be meant.</summary>
    public string? From { get; set; }

    /// <summary>How unmatched rows are treated.</summary>
    public QueryJoinKind Kind { get; set; } = QueryJoinKind.Inner;
}

/// <summary>
/// One row of the column grid. A single row carries what is returned, whether it takes part in the
/// grouping and how it is ordered, which is how a designer keeps those three decisions in one place
/// rather than spread over three lists.
/// </summary>
public sealed class QueryColumnDraft
{
    /// <summary>Alias of the participant the field belongs to.</summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>Logical field name.</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>Key of a value function to call instead of returning a field.</summary>
    public string? Function { get; set; }

    /// <summary>Arguments for <see cref="Function"/>.</summary>
    public List<QueryArgumentDraft> Arguments { get; } = [];

    /// <summary>The aggregate to compute, or null to return the value itself.</summary>
    public QueryAggregate? Aggregate { get; set; }

    /// <summary>The rank for a percentile aggregate, as a fraction.</summary>
    public double? Percentile { get; set; } = 0.95;

    /// <summary>The period a timestamp is collapsed into, for a time series.</summary>
    public QueryDateTruncation? Truncate { get; set; }

    /// <summary>Whether the aggregate considers each repeated value once.</summary>
    public bool Distinct { get; set; }

    /// <summary>Output name for the column.</summary>
    public string? OutputAlias { get; set; }

    /// <summary>Whether this column takes part in the grouping.</summary>
    public bool Group { get; set; }

    /// <summary>Which way the result is ordered by this column, or null not to order by it.</summary>
    public QuerySortDirection? Sort { get; set; }

    /// <summary>The function call this row stands for, or null when it returns a field.</summary>
    public QueryFunctionCall? ToCall()
        => string.IsNullOrWhiteSpace(Function)
            ? null
            : new QueryFunctionCall(Function!) { Arguments = Arguments.Select(a => a.ToOperand()).ToList() };

    /// <summary>The field this row returns, or null when it is a call or still incomplete.</summary>
    public QueryFieldRef? ToFieldRef()
        => string.IsNullOrWhiteSpace(Alias) || string.IsNullOrWhiteSpace(Field)
            ? null
            : new QueryFieldRef(Alias, Field);

    /// <summary>Rebuilds an editable row from a selected item, recovering its grouping and ordering.</summary>
    /// <param name="item">The selected item to open.</param>
    /// <param name="spec">The query it belongs to, which carries the grouping and ordering.</param>
    public static QueryColumnDraft FromSelect(QuerySelect item, QuerySpec spec)
    {
        var column = new QueryColumnDraft
        {
            Alias = item.Field?.Alias ?? string.Empty,
            Field = item.Field?.Field ?? string.Empty,
            Function = item.Call?.Function,
            Aggregate = item.Aggregate,
            Distinct = item.Distinct,
            Percentile = item.Percentile,
            Truncate = item.Truncate,
            OutputAlias = item.Alias,
        };

        if (item.Call is not null)
        {
            foreach (var argument in item.Call.Arguments)
            {
                column.Arguments.Add(QueryArgumentDraft.FromOperand(argument));
            }
        }

        foreach (var group in spec.GroupBy)
        {
            var matchesField = item.Field is not null && group.Field is not null
                && string.Equals(group.Field.Alias, item.Field.Alias, StringComparison.OrdinalIgnoreCase)
                && string.Equals(group.Field.Field, item.Field.Field, StringComparison.OrdinalIgnoreCase);
            var matchesCall = item.Call is not null && group.Call is not null
                && string.Equals(group.Call.Function, item.Call.Function, StringComparison.OrdinalIgnoreCase);
            if (!matchesField && !matchesCall) continue;
            column.Group = true;
            column.Truncate = group.Truncate;
            break;
        }

        foreach (var sort in spec.OrderBy)
        {
            var matchesAlias = sort.Select is not null
                && string.Equals(sort.Select, item.Alias, StringComparison.OrdinalIgnoreCase);
            var matchesField = sort.Field is not null && item.Field is not null
                && string.Equals(sort.Field.Alias, item.Field.Alias, StringComparison.OrdinalIgnoreCase)
                && string.Equals(sort.Field.Field, item.Field.Field, StringComparison.OrdinalIgnoreCase);
            if (!matchesAlias && !matchesField) continue;
            column.Sort = sort.Direction;
            break;
        }

        return column;
    }
}
