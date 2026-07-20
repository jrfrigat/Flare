using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Querio;

/// <summary>
/// Answers what a query can be given next: which fields it can reach, what can be joined onto it,
/// which operators and aggregates apply where, and what it can be ordered by.
/// <para>
/// A query is built one step at a time, and each step narrows the next: choosing a source decides
/// which columns exist, joining decides which more appear, selecting decides what can be ordered by.
/// Working that out is the same job whoever is doing the building - a visual designer, a command
/// line, a tool a model calls - so it belongs here rather than in any one of them.
/// </para>
/// <para>
/// Pass an <see cref="IQueryCapabilities"/> to narrow the answers to one target. The offered choices
/// then differ by where the query is going: a target with no set operators never offers "is one of",
/// and one that cannot keep unmatched rows never offers an outer join. Nothing is offered that would
/// only fail at render time.
/// </para>
/// <para>
/// Nothing here validates. A query being built is incomplete by definition, so an unknown entity or
/// a dangling alias is passed over rather than reported - use <see cref="QueryValidator"/> for that.
/// </para>
/// </summary>
public sealed class QueryChoices
{
    private readonly QuerySpec _spec;
    private readonly List<QueryParticipantChoice> _participants = [];
    private readonly List<QueryFieldChoice> _fields = [];
    private readonly Dictionary<string, QueryFieldChoice> _byReference =
        new(StringComparer.OrdinalIgnoreCase);

    private QueryChoices(QuerySpec spec, QuerySchema schema, IQueryCapabilities capabilities)
    {
        _spec = spec;
        Schema = schema;
        Capabilities = capabilities;

        Add(spec.From.Entity, spec.From.Call?.Function, spec.From.Alias);
        foreach (var join in spec.Joins) Add(join.Entity, join.Call?.Function, join.Alias);
    }

    /// <summary>The schema the query is built against.</summary>
    public QuerySchema Schema { get; }

    /// <summary>What the intended target can do, which every answer here is narrowed to.</summary>
    public IQueryCapabilities Capabilities { get; }

    /// <summary>
    /// What a query can start from: every entity, and every table function when the target supports
    /// them. Answered without a query, since this is the choice made before there is one.
    /// </summary>
    /// <param name="schema">The schema to draw from.</param>
    /// <param name="capabilities">What the intended target can do. Null offers everything.</param>
    public static IReadOnlyList<QueryRootChoice> Roots(
        QuerySchema schema, IQueryCapabilities? capabilities = null)
    {
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        var allowed = capabilities ?? QueryCapabilities.All;

        var roots = schema.Entities
            .Select(entity => new QueryRootChoice(entity.Key, entity.Label, QuerySourceKind.Entity)
            {
                SuggestedAlias = Initial(entity.Key),
            })
            .ToList();

        if (!allowed.Supports(QueryFeature.TableFunctions)) return roots;

        roots.AddRange(schema.Functions
            .Where(function => function.Kind == QueryFunctionKind.Table)
            .Select(function => new QueryRootChoice(function.Key, function.Label, QuerySourceKind.Function)
            {
                Parameters = function.Parameters,
                SuggestedAlias = Initial(function.Key),
            }));
        return roots;
    }

    /// <summary>Reads what a query as it currently stands can be given next.</summary>
    /// <param name="spec">The query so far. It need not be complete or coherent.</param>
    /// <param name="schema">The schema it is built against.</param>
    /// <param name="capabilities">What the intended target can do. Null offers everything.</param>
    public static QueryChoices For(
        QuerySpec spec, QuerySchema schema, IQueryCapabilities? capabilities = null)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        if (schema is null) throw new ArgumentNullException(nameof(schema));
        return new QueryChoices(spec, schema, capabilities ?? QueryCapabilities.All);
    }

    /// <summary>The sources already in the query, the root first.</summary>
    public IReadOnlyList<QueryParticipantChoice> Participants => _participants;

    /// <summary>
    /// Every field the query can reach right now. This is the answer to "what may I select", and
    /// the pool the other answers are drawn from.
    /// </summary>
    public IReadOnlyList<QueryFieldChoice> Fields => _fields;

    // The lambda parameters below are named "member" rather than "field": inside a property accessor
    // "field" is a contextual keyword in C# 14 and would bind to a backing field instead.

    /// <summary>The fields a condition may test.</summary>
    public IReadOnlyList<QueryFieldChoice> Filterable
        => _fields.Where(member => member.Filterable).ToList();

    /// <summary>The fields a grouping may use as a key.</summary>
    public IReadOnlyList<QueryFieldChoice> Groupable
        => _fields.Where(member => member.Groupable).ToList();

    /// <summary>
    /// What can be brought into the query next, one choice per declared relation reaching each
    /// source already present. An entity already joined is still offered, since a query may reach
    /// the same entity twice by different paths.
    /// </summary>
    public IReadOnlyList<QueryJoinChoice> Joins
    {
        get
        {
            var kinds = new[]
                {
                    QueryJoinKind.Inner, QueryJoinKind.Left, QueryJoinKind.Right,
                    QueryJoinKind.Full, QueryJoinKind.Cross,
                }
                .Where(Allows)
                .ToList();

            var choices = new List<QueryJoinChoice>();
            foreach (var participant in _participants)
            {
                if (participant.Entity is null) continue;
                foreach (var relation in Schema.RelationsOf(participant.Entity))
                {
                    var reached = string.Equals(relation.From, participant.Entity, StringComparison.OrdinalIgnoreCase)
                        ? relation.To
                        : relation.From;
                    var entity = Schema.FindEntity(reached);
                    if (entity is null) continue;

                    choices.Add(new QueryJoinChoice(
                        relation.Key, relation.Label ?? relation.Key, participant.Alias, entity.Key, entity.Label)
                    {
                        Cardinality = relation.Cardinality,
                        SuggestedAlias = SuggestAlias(entity.Key),
                        Kinds = kinds,
                    });
                }
            }
            return choices;
        }
    }

    /// <summary>The value functions a query may call, or none when the target cannot call any.</summary>
    public IReadOnlyList<QueryFunction> ValueFunctions
        => Capabilities.Supports(QueryFeature.ValueFunctions)
            ? Schema.Functions.Where(function => function.Kind == QueryFunctionKind.Value).ToList()
            : [];

    /// <summary>The table functions a query may draw rows from, or none when the target cannot.</summary>
    public IReadOnlyList<QueryFunction> TableFunctions
        => Capabilities.Supports(QueryFeature.TableFunctions)
            ? Schema.Functions.Where(function => function.Kind == QueryFunctionKind.Table).ToList()
            : [];

    /// <summary>The periods a moment may be collapsed to, or none when the target cannot do it.</summary>
    public IReadOnlyList<QueryDateTruncation> Periods
        => Capabilities.Supports(QueryFeature.DateTruncation)
            ? (IReadOnlyList<QueryDateTruncation>)
                [.. Enum.GetValues(typeof(QueryDateTruncation)).Cast<QueryDateTruncation>()]
            : [];

    /// <summary>Whether rows themselves may be counted, which needs no field to count.</summary>
    public bool CountsRows => Capabilities.Supports(QueryFeature.Aggregates);

    /// <summary>The output names the query has given the things it selects.</summary>
    public IReadOnlyList<string> Outputs
        => _spec.Select
            .Where(item => !string.IsNullOrEmpty(item.Alias))
            .Select(item => item.Alias!)
            .ToList();

    /// <summary>
    /// What the result can be ordered by: any field it reaches, and anything already selected under
    /// a name - which is how a result gets ordered by a computed aggregate.
    /// </summary>
    public IReadOnlyList<QuerySortChoice> SortTargets
    {
        get
        {
            var targets = _fields
                .Select(member => new QuerySortChoice($"{member.ParticipantLabel}: {member.Label}")
                {
                    Field = member.Reference,
                })
                .ToList();
            targets.AddRange(Outputs.Select(name => new QuerySortChoice(name) { Select = name }));
            return targets;
        }
    }

    /// <summary>
    /// What a grouping filter may test: the named outputs, and the fields the query groups by. A
    /// condition on anything else would be asking about a row the grouping has already collapsed.
    /// </summary>
    public IReadOnlyList<QuerySortChoice> GroupingFilterTargets
    {
        get
        {
            if (!Capabilities.Supports(QueryFeature.Having)) return [];

            var targets = Outputs.Select(name => new QuerySortChoice(name) { Select = name }).ToList();
            foreach (var group in _spec.GroupBy)
            {
                if (group.Field is null) continue;
                var found = Find(group.Field);
                if (found is not null)
                {
                    targets.Add(new QuerySortChoice($"{found.ParticipantLabel}: {found.Label}")
                    {
                        Field = found.Reference,
                    });
                }
            }
            return targets;
        }
    }

    /// <summary>Finds one reachable field. Null when the alias or the field is not in the query.</summary>
    /// <param name="field">The reference to resolve.</param>
    public QueryFieldChoice? Find(QueryFieldRef field)
        => field is not null && _byReference.TryGetValue($"{field.Alias}.{field.Field}", out var found)
            ? found
            : null;

    /// <summary>The operators offered for one field, or none when the query cannot reach it.</summary>
    /// <param name="field">The field a condition would test.</param>
    public IReadOnlyList<QueryOperatorChoice> OperatorsFor(QueryFieldRef field)
        => Find(field)?.Operators ?? [];

    /// <summary>The operators offered for a kind of value, without naming a particular field.</summary>
    /// <param name="type">The semantic kind being compared.</param>
    public IReadOnlyList<QueryOperatorChoice> OperatorsFor(QueryFieldType type)
        => Narrow(QueryDefaults.OperatorsFor(type));

    /// <summary>The aggregates offered for one field, or none when the query cannot reach it.</summary>
    /// <param name="field">The field an aggregate would be computed over.</param>
    public IReadOnlyList<QueryAggregate> AggregatesFor(QueryFieldRef field)
        => Find(field)?.Aggregates ?? [];

    /// <summary>
    /// What may stand on the right of a condition on this field: a fixed value always, and a set, a
    /// another field, a moment relative to now or a function call where the target allows it.
    /// </summary>
    /// <param name="field">The field being compared.</param>
    public IReadOnlyList<QueryOperandKind> ValueKindsFor(QueryFieldRef field)
    {
        var found = Find(field);
        if (found is null) return [];

        var kinds = new List<QueryOperandKind> { QueryOperandKind.Literal };
        if (Capabilities.Supports(QueryFeature.SetOperators)) kinds.Add(QueryOperandKind.List);
        if (Capabilities.Supports(QueryFeature.FieldComparison)) kinds.Add(QueryOperandKind.Field);
        // Only a moment can be offset from now; offering it elsewhere would mean nothing.
        if (found.Type == QueryFieldType.DateTime && Capabilities.Supports(QueryFeature.RelativeTime))
        {
            kinds.Add(QueryOperandKind.Relative);
        }
        if (Capabilities.Supports(QueryFeature.ValueFunctions)) kinds.Add(QueryOperandKind.Function);
        return kinds;
    }

    /// <summary>
    /// The other fields this one may be compared against. Only fields of the same kind are offered,
    /// since comparing a moment against a name is a mistake rather than a query.
    /// </summary>
    /// <param name="field">The field on the left of the comparison.</param>
    public IReadOnlyList<QueryFieldChoice> ComparableTo(QueryFieldRef field)
    {
        var found = Find(field);
        if (found is null || !Capabilities.Supports(QueryFeature.FieldComparison)) return [];

        return _fields
            .Where(candidate => candidate.Type == found.Type && !ReferenceEquals(candidate, found))
            .ToList();
    }

    /// <summary>
    /// An alias not yet used in the query, derived from a key. Suggests the first letter, then adds
    /// a number, which is what a person writing the query by hand would do.
    /// </summary>
    /// <param name="key">The entity or function key the alias stands for.</param>
    public string SuggestAlias(string key)
    {
        var seed = Initial(key);
        if (!Taken(seed)) return seed;

        for (var suffix = 2; ; suffix++)
        {
            var candidate = seed + suffix.ToString(CultureInfo.InvariantCulture);
            if (!Taken(candidate)) return candidate;
        }
    }

    private bool Taken(string alias)
        => _participants.Any(participant =>
            string.Equals(participant.Alias, alias, StringComparison.OrdinalIgnoreCase));

    private static string Initial(string key)
    {
        foreach (var character in key)
        {
            if (char.IsLetter(character)) return char.ToLowerInvariant(character).ToString();
        }
        return "t";
    }

    private void Add(string? entityKey, string? functionKey, string alias)
    {
        var entity = entityKey is null ? null : Schema.FindEntity(entityKey);
        var function = functionKey is null ? null : Schema.FindFunction(functionKey);
        var label = entity?.Label ?? function?.Label ?? entityKey ?? functionKey ?? alias;

        _participants.Add(new QueryParticipantChoice(alias, label, entity?.Key, function?.Key));

        var fields = entity?.Fields ?? function?.Columns ?? [];
        foreach (var field in fields)
        {
            var choice = new QueryFieldChoice(alias, field.Key, field.Label, field.Type)
            {
                ParticipantLabel = label,
                Nullable = field.Nullable,
                Filterable = field.Filterable,
                Groupable = field.Groupable,
                Operators = Narrow(field.AllowedOperators),
                Aggregates = Narrow(field.AllowedAggregates),
                EnumMembers = field.Type == QueryFieldType.Enum ? field.EnumMembers : null,
            };
            _fields.Add(choice);
            _byReference[$"{alias}.{field.Key}"] = choice;
        }
    }

    /// <summary>Drops the operators the target cannot express, and records what each one expects.</summary>
    private IReadOnlyList<QueryOperatorChoice> Narrow(IReadOnlyList<QueryOperator> operators)
    {
        var kept = new List<QueryOperatorChoice>(operators.Count);
        foreach (var op in operators)
        {
            var feature = op switch
            {
                QueryOperator.In or QueryOperator.NotIn => QueryFeature.SetOperators,
                QueryOperator.Between or QueryOperator.NotBetween => QueryFeature.RangeOperators,
                QueryOperator.Contains or QueryOperator.StartsWith or QueryOperator.EndsWith
                    => QueryFeature.TextSearch,
                _ => (QueryFeature?)null,
            };
            if (feature is not null && !Capabilities.Supports(feature.Value)) continue;

            kept.Add(new QueryOperatorChoice(op, Arity(op)));
        }
        return kept;
    }

    private IReadOnlyList<QueryAggregate> Narrow(IReadOnlyList<QueryAggregate> aggregates)
    {
        if (!Capabilities.Supports(QueryFeature.Aggregates)) return [];
        if (Capabilities.Supports(QueryFeature.Percentile)) return aggregates;
        return aggregates.Where(aggregate => aggregate != QueryAggregate.Percentile).ToList();
    }

    private bool Allows(QueryJoinKind kind) => kind switch
    {
        QueryJoinKind.Left => Capabilities.Supports(QueryFeature.LeftJoin),
        QueryJoinKind.Right => Capabilities.Supports(QueryFeature.RightJoin),
        QueryJoinKind.Full => Capabilities.Supports(QueryFeature.FullJoin),
        QueryJoinKind.Cross => Capabilities.Supports(QueryFeature.CrossJoin),
        _ => true,
    };

    private static QueryValueArity Arity(QueryOperator op)
    {
        if (QueryDefaults.TakesNoValue(op)) return QueryValueArity.None;
        if (QueryDefaults.TakesTwoValues(op)) return QueryValueArity.Two;
        if (QueryDefaults.TakesValueList(op)) return QueryValueArity.List;
        return QueryValueArity.One;
    }
}
