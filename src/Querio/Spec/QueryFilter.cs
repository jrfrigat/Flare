namespace Querio;

/// <summary>
/// One comparison: something to test, an operator, and up to two operands. <see cref="Value"/> is
/// null for the null checks, and <see cref="Value2"/> carries the upper bound of a range comparison.
/// <para>
/// The left side is either a <see cref="Field"/> or, in a HAVING clause, the <see cref="Select"/>
/// output name of a computed aggregate - which is how "groups with more than ten rows" is expressed.
/// Exactly one of the two applies; a condition on a select name belongs only in
/// <see cref="QuerySpec.Having"/>, since aggregates do not exist yet where
/// <see cref="QuerySpec.Where"/> is applied.
/// </para>
/// </summary>
/// <param name="Field">The field being tested. Null when <see cref="Select"/> names the left side.</param>
/// <param name="Operator">The comparison to apply.</param>
public sealed record QueryCondition(QueryFieldRef? Field, QueryOperator Operator)
{
    /// <summary>Output name of a selected aggregate to test instead of a field. Used only in HAVING.</summary>
    public string? Select { get; init; }

    /// <summary>The operand compared against. Null for <see cref="QueryOperator.IsNull"/> and its opposite.</summary>
    public QueryOperand? Value { get; init; }

    /// <summary>The second operand, used only by the range operators.</summary>
    public QueryOperand? Value2 { get; init; }
}

/// <summary>
/// A node of the condition tree: conditions and nested groups combined with AND, or with OR when
/// <see cref="Or"/> is set. Nesting is what makes a real filter expressible rather than a flat list
/// of ANDed comparisons.
/// <para>
/// Note that the collection members compare by reference under the synthesized record equality, so
/// two structurally identical trees built separately are not <c>==</c> to each other. Compare
/// serialized form when structural comparison is what is meant.
/// </para>
/// </summary>
public sealed record QueryFilterGroup
{
    /// <summary>Combines the children with OR when true, AND when false.</summary>
    public bool Or { get; init; }

    /// <summary>The comparisons held directly by this group.</summary>
    public IReadOnlyList<QueryCondition> Conditions { get; init; } = [];

    /// <summary>Nested groups, letting AND and OR mix to any depth.</summary>
    public IReadOnlyList<QueryFilterGroup> Groups { get; init; } = [];

    /// <summary>True when the group constrains nothing, so a renderer can omit the clause entirely.</summary>
    public bool IsEmpty => Conditions.Count == 0 && Groups.Count == 0;
}
