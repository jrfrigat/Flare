using System.Collections.Generic;
using System.Linq;

namespace Querio;

/// <summary>
/// One thing a query draws fields from. An entity and a table function are interchangeable here:
/// both contribute an alias and a set of fields.
/// </summary>
/// <param name="Alias">The alias field references use.</param>
/// <param name="Label">Name for diagnostics.</param>
/// <param name="Fields">The fields this participant contributes.</param>
/// <param name="EntityKey">Key of the entity, or null when a table function stands here.</param>
public sealed record QueryParticipantShape(
    string Alias,
    string Label,
    IReadOnlyList<QueryField> Fields,
    string? EntityKey);

/// <summary>One resolved field pair of a join's match condition.</summary>
/// <param name="LeftAlias">Alias on the left of the match.</param>
/// <param name="LeftField">Field key on the left of the match.</param>
/// <param name="RightAlias">Alias on the right of the match.</param>
/// <param name="RightField">Field key on the right of the match.</param>
public readonly record struct QueryJoinMatch(
    string LeftAlias,
    string LeftField,
    string RightAlias,
    string RightField);

/// <summary>
/// The shared walk over a query, with the meaning of each node left to the target.
/// <para>
/// Everything that is the same for every target lives here: resolving participants and field types,
/// working out which side of a join a relation attaches to, checking what the target can do, and
/// composing the condition tree. A target supplies only what a field, a literal, a call and a
/// comparison actually mean for it.
/// </para>
/// <para>
/// <typeparamref name="TExpression"/> is deliberately open. A text target uses <c>string</c>; a LINQ
/// target uses <c>System.Linq.Expressions.Expression</c>; a document target uses its own node type.
/// That is what keeps the model free of any assumption that rendering produces text at all.
/// </para>
/// </summary>
/// <typeparam name="TExpression">Whatever the target builds a value out of.</typeparam>
public abstract class QueryRenderer<TExpression>
{
    private readonly Dictionary<string, QueryParticipantShape> _byAlias = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<QueryParticipantShape> _participants = [];

    /// <summary>Builds a renderer over a query and the schema it was built against.</summary>
    /// <param name="spec">The query being rendered.</param>
    /// <param name="schema">The schema it was built against.</param>
    protected QueryRenderer(QuerySpec spec, QuerySchema schema)
    {
        Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        Schema = schema ?? throw new ArgumentNullException(nameof(schema));
    }

    /// <summary>The query being rendered.</summary>
    protected QuerySpec Spec { get; }

    /// <summary>The schema the query was built against.</summary>
    protected QuerySchema Schema { get; }

    /// <summary>Participants in declaration order, the root first.</summary>
    protected IReadOnlyList<QueryParticipantShape> Participants => _participants;

    /// <summary>
    /// Expressions behind each output name, filled in by the target as it renders the selected items.
    /// A condition on a computed aggregate reads them, since not every target can refer to an output
    /// name from inside its own grouping filter.
    /// </summary>
    protected Dictionary<string, TExpression> OutputExpressions { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>What each output name yields, so a value compared against it is read correctly.</summary>
    protected Dictionary<string, QueryFieldType> OutputTypes { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Runs the checks every target needs before writing anything: the query must be coherent, and
    /// the target must be able to express it. Rendering an invalid query would only produce something
    /// wrong in a new way.
    /// </summary>
    /// <param name="capabilities">What the target can do, or null to skip the capability check.</param>
    /// <exception cref="QueryValidationException">The query is not coherent.</exception>
    /// <exception cref="QueryRenderException">The target cannot express the query.</exception>
    protected void Prepare(IQueryCapabilities? capabilities = null)
    {
        Spec.Validate(Schema).ThrowIfInvalid();
        CollectParticipants();
        if (capabilities is not null) RequireCapabilities(capabilities);
    }

    private void CollectParticipants()
    {
        Register(Spec.From.Entity, Spec.From.Call, Spec.From.Alias);
        foreach (var join in Spec.Joins) Register(join.Entity, join.Call, join.Alias);
    }

    // Validation already proved these resolve, so a miss here is impossible by construction.
    private void Register(string? entityKey, QueryFunctionCall? call, string alias)
    {
        var shape = call is not null
            ? new QueryParticipantShape(alias, call.Function, Schema.FindFunction(call.Function)!.Columns, null)
            : new QueryParticipantShape(alias, entityKey!, Schema.FindEntity(entityKey!)!.Fields, entityKey);
        _participants.Add(shape);
        _byAlias[alias] = shape;
    }

    /// <summary>Finds a field on a participant. Null when the alias or the field is unknown.</summary>
    /// <param name="alias">Alias of the participant.</param>
    /// <param name="fieldKey">Logical field name.</param>
    protected QueryField? FindField(string alias, string fieldKey)
    {
        if (!_byAlias.TryGetValue(alias, out var participant)) return null;
        for (var i = 0; i < participant.Fields.Count; i++)
        {
            if (string.Equals(participant.Fields[i].Key, fieldKey, StringComparison.OrdinalIgnoreCase))
            {
                return participant.Fields[i];
            }
        }
        return null;
    }

    // ---- What a target must supply ---------------------------------------------------------------

    /// <summary>What a field of a participant means to the target.</summary>
    /// <param name="alias">Alias of the participant.</param>
    /// <param name="field">The resolved field.</param>
    protected abstract TExpression Field(string alias, QueryField field);

    /// <summary>What a fixed value means to the target, already read into a .NET value.</summary>
    /// <param name="value">The value, parsed from its stored form.</param>
    /// <param name="type">The declared type it was read as.</param>
    protected abstract TExpression Literal(object? value, QueryFieldType type);

    /// <summary>What a moment offset from now means to the target.</summary>
    /// <param name="offset">The signed offset.</param>
    protected abstract TExpression Relative(QueryRelativeValue offset);

    /// <summary>What a call to a declared function means to the target.</summary>
    /// <param name="function">The declared function.</param>
    /// <param name="arguments">The already-rendered arguments.</param>
    protected abstract TExpression Call(QueryFunction function, IReadOnlyList<TExpression> arguments);

    /// <summary>What a comparison means to the target.</summary>
    /// <param name="left">The already-rendered left side.</param>
    /// <param name="op">The comparison applied.</param>
    /// <param name="type">What the left side yields, so the target can read the operands.</param>
    /// <param name="right">The already-rendered operand, absent for the null checks.</param>
    /// <param name="upper">The already-rendered upper bound, present only for a range.</param>
    protected abstract TExpression Comparison(
        TExpression left, QueryOperator op, QueryFieldType type, TExpression? right, TExpression? upper);

    /// <summary>What a set-membership test means to the target.</summary>
    /// <param name="left">The already-rendered left side.</param>
    /// <param name="op">Either <see cref="QueryOperator.In"/> or its negation.</param>
    /// <param name="values">The already-rendered values.</param>
    protected abstract TExpression Membership(
        TExpression left, QueryOperator op, IReadOnlyList<TExpression> values);

    /// <summary>How the target joins several conditions together.</summary>
    /// <param name="or">Whether the parts are combined with OR rather than AND.</param>
    /// <param name="parts">The already-rendered parts. Never empty.</param>
    protected abstract TExpression Combine(bool or, IReadOnlyList<TExpression> parts);

    // ---- Shared walks ----------------------------------------------------------------------------

    /// <summary>Renders a value that is either a field or a call, whichever the caller supplied.</summary>
    /// <param name="field">The field, when the value is one.</param>
    /// <param name="call">The call, when the value is one.</param>
    protected TExpression Value(QueryFieldRef? field, QueryFunctionCall? call)
    {
        if (field is not null) return Field(field.Alias, FindField(field.Alias, field.Field)!);
        return RenderCall(call!);
    }

    /// <summary>What a value expression yields.</summary>
    /// <param name="field">The field, when the value is one.</param>
    /// <param name="call">The call, when the value is one.</param>
    protected QueryFieldType ValueType(QueryFieldRef? field, QueryFunctionCall? call)
        => field is not null
            ? FindField(field.Alias, field.Field)?.Type ?? QueryFieldType.Text
            : Schema.FindFunction(call!.Function)?.ReturnType ?? QueryFieldType.Text;

    private TExpression RenderCall(QueryFunctionCall call)
    {
        var function = Schema.FindFunction(call.Function)!;
        var arguments = new List<TExpression>(call.Arguments.Count);
        for (var i = 0; i < call.Arguments.Count; i++)
        {
            // An argument is read as the parameter it fills, which is what lets a date argument
            // reach the target typed rather than as text.
            var type = i < function.Parameters.Count ? function.Parameters[i].Type : QueryFieldType.Text;
            arguments.Add(Operand(call.Arguments[i], type));
        }
        return Call(function, arguments);
    }

    /// <summary>Renders one operand, reading a fixed value as the given type.</summary>
    /// <param name="operand">The operand to render.</param>
    /// <param name="type">The type a fixed value is read as.</param>
    protected TExpression Operand(QueryOperand operand, QueryFieldType type) => operand.Kind switch
    {
        QueryOperandKind.Field => Field(operand.Field!.Alias, FindField(operand.Field.Alias, operand.Field.Field)!),
        QueryOperandKind.Relative => Relative(operand.Relative!),
        QueryOperandKind.Function => RenderCall(operand.Call!),
        _ => Literal(QueryValue.Parse(operand.Value, type), type),
    };

    /// <summary>Renders a condition tree, or returns default when it constrains nothing.</summary>
    /// <param name="group">The tree to render.</param>
    protected TExpression? Filter(QueryFilterGroup? group)
    {
        if (group is null || group.IsEmpty) return default;

        var parts = new List<TExpression>(group.Conditions.Count + group.Groups.Count);
        foreach (var condition in group.Conditions) parts.Add(Condition(condition));
        foreach (var nested in group.Groups)
        {
            var rendered = Filter(nested);
            if (rendered is not null) parts.Add(rendered);
        }
        return parts.Count == 1 ? parts[0] : Combine(group.Or, parts);
    }

    /// <summary>Renders one condition, resolving its left side and reading its operands.</summary>
    /// <param name="condition">The condition to render.</param>
    protected TExpression Condition(QueryCondition condition)
    {
        TExpression left;
        QueryFieldType type;
        if (!string.IsNullOrEmpty(condition.Select))
        {
            // A grouping filter names a computed aggregate. Some targets cannot refer to an output
            // name there, so the expression behind it is used instead.
            left = OutputExpressions[condition.Select!];
            type = OutputTypes.TryGetValue(condition.Select!, out var known) ? known : QueryFieldType.Number;
        }
        else
        {
            left = Value(condition.Field, condition.Call);
            type = ValueType(condition.Field, condition.Call);
        }

        if (QueryDefaults.TakesNoValue(condition.Operator))
        {
            return Comparison(left, condition.Operator, type, default, default);
        }
        if (QueryDefaults.TakesValueList(condition.Operator))
        {
            var values = (condition.Value!.Values ?? [])
                .Select(value => Literal(QueryValue.Parse(value, type), type))
                .ToList();
            return Membership(left, condition.Operator, values);
        }
        if (QueryDefaults.TakesTwoValues(condition.Operator))
        {
            return Comparison(
                left, condition.Operator, type,
                Operand(condition.Value!, type),
                Operand(condition.Value2!, type));
        }
        return Comparison(left, condition.Operator, type, Operand(condition.Value!, type), default);
    }

    /// <summary>
    /// Resolves what a join actually matches on, whether it names a relation or explicit conditions.
    /// Never guesses which occurrence of an entity was meant.
    /// </summary>
    /// <param name="join">The join to resolve.</param>
    /// <param name="index">Its position, since a join may only attach to a participant before it.</param>
    /// <exception cref="QueryRenderException">The join is ambiguous or reaches nothing.</exception>
    protected IReadOnlyList<QueryJoinMatch> JoinMatches(QueryJoin join, int index)
    {
        if (join.On is { Count: > 0 })
        {
            return join.On
                .Select(pair => new QueryJoinMatch(
                    pair.Left.Alias, pair.Left.Field, pair.Right.Alias, pair.Right.Field))
                .ToList();
        }

        var relation = Schema.FindRelation(join.Relation!)!;

        // A self-relation matches on both ends. Reading the new participant as the target - "the
        // manager of the user we already have" - is the direction a person means when adding it.
        var newIsTarget = string.Equals(relation.To, join.Entity, StringComparison.OrdinalIgnoreCase);
        var otherEntity = newIsTarget ? relation.From : relation.To;
        var otherAlias = join.From ?? ResolveAttachment(otherEntity, index, join.Alias);

        return relation.On
            .Select(pair => newIsTarget
                ? new QueryJoinMatch(otherAlias, pair.FromField, join.Alias, pair.ToField)
                : new QueryJoinMatch(join.Alias, pair.FromField, otherAlias, pair.ToField))
            .ToList();
    }

    private string ResolveAttachment(string entityKey, int joinIndex, string joinAlias)
    {
        var candidates = _participants
            .Take(joinIndex + 1) // the root plus every join declared before this one
            .Where(participant => !string.Equals(participant.Alias, joinAlias, StringComparison.OrdinalIgnoreCase)
                && string.Equals(participant.EntityKey, entityKey, StringComparison.OrdinalIgnoreCase))
            .Select(participant => participant.Alias)
            .ToList();

        if (candidates.Count == 1) return candidates[0];
        if (candidates.Count == 0)
        {
            throw new QueryRenderException(
                $"The join onto '{joinAlias}' traverses a relation reaching '{entityKey}', which no earlier participant uses.");
        }
        throw new QueryRenderException(
            $"'{entityKey}' appears more than once ({string.Join(", ", candidates)}), so the join onto " +
            $"'{joinAlias}' is ambiguous. Set QueryJoin.From to the alias it attaches to.");
    }

    /// <summary>
    /// Refuses anything the target cannot express. Being strict here is the point: producing
    /// something close would answer a different question, which is worse than failing.
    /// </summary>
    /// <param name="capabilities">What the target can do.</param>
    protected void RequireCapabilities(IQueryCapabilities capabilities)
    {
        void Require(QueryFeature feature)
        {
            if (capabilities.Supports(feature)) return;
            throw new QueryRenderException($"{TargetName} does not support {feature}.", feature);
        }

        if (Spec.From.Call is not null) Require(QueryFeature.TableFunctions);
        foreach (var join in Spec.Joins)
        {
            switch (join.Kind)
            {
                case QueryJoinKind.Left: Require(QueryFeature.LeftJoin); break;
                case QueryJoinKind.Right: Require(QueryFeature.RightJoin); break;
                case QueryJoinKind.Full: Require(QueryFeature.FullJoin); break;
                case QueryJoinKind.Cross: Require(QueryFeature.CrossJoin); break;
            }
            if (join.Call is not null) Require(QueryFeature.TableFunctions);
        }

        foreach (var item in Spec.Select)
        {
            if (item.Aggregate is not null) Require(QueryFeature.Aggregates);
            if (item.Distinct) Require(QueryFeature.DistinctAggregate);
            if (item.Aggregate == QueryAggregate.Percentile) Require(QueryFeature.Percentile);
            if (item.Truncate is not null) Require(QueryFeature.DateTruncation);
            if (item.Call is not null) Require(QueryFeature.ValueFunctions);
        }

        if (Spec.GroupBy.Count > 0) Require(QueryFeature.Grouping);
        if (Spec.GroupBy.Any(group => group.Truncate is not null)) Require(QueryFeature.DateTruncation);
        if (Spec.GroupBy.Any(group => group.Call is not null)) Require(QueryFeature.ValueFunctions);
        if (Spec.OrderBy.Any(sort => sort.Call is not null)) Require(QueryFeature.ValueFunctions);
        if (Spec.Having is not null) Require(QueryFeature.Having);
        if (Spec.Distinct) Require(QueryFeature.Distinct);
        if (Spec.Limit is not null) Require(QueryFeature.Limit);
        if (Spec.Offset is not null) Require(QueryFeature.Offset);

        foreach (var condition in AllConditions(Spec.Where).Concat(AllConditions(Spec.Having)))
        {
            if (condition.Operator is QueryOperator.In or QueryOperator.NotIn) Require(QueryFeature.SetOperators);
            if (condition.Operator is QueryOperator.Between or QueryOperator.NotBetween) Require(QueryFeature.RangeOperators);
            if (condition.Operator is QueryOperator.Contains or QueryOperator.StartsWith or QueryOperator.EndsWith)
            {
                Require(QueryFeature.TextSearch);
            }
            if (condition.Call is not null) Require(QueryFeature.ValueFunctions);
            foreach (var operand in new[] { condition.Value, condition.Value2 })
            {
                if (operand is null) continue;
                if (operand.Kind == QueryOperandKind.Field) Require(QueryFeature.FieldComparison);
                if (operand.Kind == QueryOperandKind.Relative) Require(QueryFeature.RelativeTime);
                if (operand.Kind == QueryOperandKind.Function) Require(QueryFeature.ValueFunctions);
            }
        }
    }

    /// <summary>The target's name, used when reporting something it cannot express.</summary>
    protected abstract string TargetName { get; }

    /// <summary>Every condition in a tree, nested ones included.</summary>
    /// <param name="group">The tree to walk.</param>
    protected static IEnumerable<QueryCondition> AllConditions(QueryFilterGroup? group)
    {
        if (group is null) yield break;
        foreach (var condition in group.Conditions) yield return condition;
        foreach (var nested in group.Groups)
        {
            foreach (var condition in AllConditions(nested)) yield return condition;
        }
    }
}
