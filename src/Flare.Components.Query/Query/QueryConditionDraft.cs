using Querio;

namespace Flare.Components;

/// <summary>
/// One editable condition row. It holds what a person has typed so far, which is often not yet a
/// valid comparison - <see cref="ToCondition"/> returns null until it is, so a half-filled row
/// simply does not reach the query rather than making it invalid.
/// </summary>
public sealed class QueryConditionDraft
{
    /// <summary>Alias of the participant the field belongs to.</summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>Logical field name.</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>Output name of a computed aggregate to test instead of a field, in a HAVING clause.</summary>
    public string? SelectAlias { get; set; }

    /// <summary>Key of a value function to test instead of a field.</summary>
    public string? Function { get; set; }

    /// <summary>Arguments for <see cref="Function"/>.</summary>
    public List<QueryArgumentDraft> Arguments { get; } = [];

    /// <summary>The comparison applied.</summary>
    public QueryOperator Operator { get; set; } = QueryOperator.Equals;

    /// <summary>The value compared against, as typed.</summary>
    public string? Value { get; set; }

    /// <summary>The upper bound of a range comparison, as typed.</summary>
    public string? Value2 { get; set; }

    /// <summary>Values for a set comparison, separated by commas.</summary>
    public string? Values { get; set; }

    /// <summary>Whether the comparison is against a moment relative to now rather than a fixed value.</summary>
    public bool Relative { get; set; }

    /// <summary>How many units the relative window reaches.</summary>
    public int RelativeAmount { get; set; } = 30;

    /// <summary>The unit the relative window is counted in.</summary>
    public QueryTimeUnit RelativeUnit { get; set; } = QueryTimeUnit.Day;

    /// <summary>Whether the relative window reaches into the past rather than the future.</summary>
    public bool RelativePast { get; set; } = true;

    /// <summary>Converts the row into a condition, or null while it is still incomplete.</summary>
    public QueryCondition? ToCondition()
    {
        var testsAggregate = !string.IsNullOrWhiteSpace(SelectAlias);
        var call = string.IsNullOrWhiteSpace(Function)
            ? null
            : new QueryFunctionCall(Function!) { Arguments = Arguments.Select(a => a.ToOperand()).ToList() };

        if (!testsAggregate && call is null
            && (string.IsNullOrWhiteSpace(Alias) || string.IsNullOrWhiteSpace(Field)))
        {
            return null;
        }

        var condition = new QueryCondition(
            testsAggregate || call is not null ? null : new QueryFieldRef(Alias, Field),
            Operator)
        {
            Select = testsAggregate ? SelectAlias : null,
            Call = call,
        };

        if (QueryDefaults.TakesNoValue(Operator)) return condition;

        if (QueryDefaults.TakesValueList(Operator))
        {
            var items = SplitValues();
            return items.Count == 0 ? null : condition with { Value = QueryOperand.List(items) };
        }

        if (QueryDefaults.TakesTwoValues(Operator))
        {
            if (string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Value2)) return null;
            return condition with { Value = Operand()!, Value2 = QueryOperand.Literal(Value2) };
        }

        var operand = Operand();
        return operand is null ? null : condition with { Value = operand };
    }

    /// <summary>Rebuilds a row from a condition, so an existing query can be edited.</summary>
    /// <param name="condition">The condition to open.</param>
    public static QueryConditionDraft FromCondition(QueryCondition condition)
    {
        var draft = new QueryConditionDraft
        {
            Alias = condition.Field?.Alias ?? string.Empty,
            Field = condition.Field?.Field ?? string.Empty,
            SelectAlias = condition.Select,
            Function = condition.Call?.Function,
            Operator = condition.Operator,
        };

        if (condition.Call is not null)
        {
            foreach (var argument in condition.Call.Arguments)
            {
                draft.Arguments.Add(QueryArgumentDraft.FromOperand(argument));
            }
        }

        switch (condition.Value?.Kind)
        {
            case QueryOperandKind.Literal:
                draft.Value = condition.Value.Value;
                break;
            case QueryOperandKind.List:
                draft.Values = string.Join(", ", condition.Value.Values ?? []);
                break;
            case QueryOperandKind.Relative:
                draft.Relative = true;
                draft.RelativeAmount = Math.Abs(condition.Value.Relative!.Amount);
                draft.RelativePast = condition.Value.Relative.Amount <= 0;
                draft.RelativeUnit = condition.Value.Relative.Unit;
                break;
        }

        if (condition.Value2?.Kind == QueryOperandKind.Literal) draft.Value2 = condition.Value2.Value;
        return draft;
    }

    private QueryOperand? Operand()
    {
        if (Relative)
        {
            return RelativePast
                ? QueryOperand.Ago(RelativeAmount, RelativeUnit)
                : QueryOperand.FromNow(RelativeAmount, RelativeUnit);
        }
        return string.IsNullOrWhiteSpace(Value) ? null : QueryOperand.Literal(Value);
    }

    private List<string> SplitValues()
        => (Values ?? string.Empty)
            .Split(',')
            .Select(part => part.Trim())
            .Where(part => part.Length > 0)
            .ToList();
}

/// <summary>
/// An editable node of the condition tree: rows and nested nodes combined with AND, or with OR when
/// <see cref="Or"/> is set.
/// </summary>
public sealed class QueryConditionGroupDraft
{
    /// <summary>Combines this node's children with OR rather than AND.</summary>
    public bool Or { get; set; }

    /// <summary>The condition rows held directly by this node.</summary>
    public List<QueryConditionDraft> Conditions { get; } = [];

    /// <summary>Nested nodes, letting AND and OR mix.</summary>
    public List<QueryConditionGroupDraft> Groups { get; } = [];

    /// <summary>Converts the node into a condition tree, leaving out rows that are incomplete.</summary>
    public QueryFilterGroup ToGroup()
    {
        var conditions = Conditions
            .Select(condition => condition.ToCondition())
            .Where(condition => condition is not null)
            .Select(condition => condition!)
            .ToList();

        var groups = Groups
            .Select(group => group.ToGroup())
            .Where(group => !group.IsEmpty)
            .ToList();

        return new QueryFilterGroup { Or = Or, Conditions = conditions, Groups = groups };
    }

    /// <summary>Rebuilds an editable node from a condition tree.</summary>
    /// <param name="group">The tree to open. Null yields an empty node.</param>
    public static QueryConditionGroupDraft FromGroup(QueryFilterGroup? group)
    {
        var draft = new QueryConditionGroupDraft();
        if (group is null) return draft;

        draft.Or = group.Or;
        foreach (var condition in group.Conditions) draft.Conditions.Add(QueryConditionDraft.FromCondition(condition));
        foreach (var nested in group.Groups) draft.Groups.Add(FromGroup(nested));
        return draft;
    }
}
