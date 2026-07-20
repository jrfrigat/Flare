using System.Collections.Generic;

namespace Querio;

/// <summary>
/// Composes one node of a condition tree. Every method returns the same builder, so conditions chain;
/// <see cref="Group"/> and <see cref="AnyOf"/> nest a child node, which is how AND and OR mix.
/// <para>
/// Values are accepted as plain .NET objects and formatted through <see cref="QueryValue"/>, so a
/// caller writes <c>Equal("r", "error", true)</c> rather than hand-formatting the stored string.
/// </para>
/// </summary>
public sealed class QueryFilterBuilder
{
    private readonly List<QueryCondition> _conditions = [];
    private readonly List<QueryFilterGroup> _groups = [];
    private bool _or;

    /// <summary>Combines this node's children with OR. They are combined with AND by default.</summary>
    public QueryFilterBuilder Any()
    {
        _or = true;
        return this;
    }

    /// <summary>Combines this node's children with AND, undoing a previous <see cref="Any"/>.</summary>
    public QueryFilterBuilder All()
    {
        _or = false;
        return this;
    }

    /// <summary>Adds an already-built condition, for cases the typed helpers do not cover.</summary>
    /// <param name="condition">The condition to add.</param>
    public QueryFilterBuilder Add(QueryCondition condition)
    {
        _conditions.Add(condition);
        return this;
    }

    /// <summary>Adds a comparison with explicitly-built operands.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="op">The comparison to apply.</param>
    /// <param name="value">The operand compared against, if the operator takes one.</param>
    /// <param name="value2">The second operand, for the range operators.</param>
    public QueryFilterBuilder Compare(
        string alias, string field, QueryOperator op,
        QueryOperand? value = null, QueryOperand? value2 = null)
    {
        _conditions.Add(new QueryCondition(new QueryFieldRef(alias, field), op)
        {
            Value = value,
            Value2 = value2,
        });
        return this;
    }

    /// <summary>Adds a comparison against a fixed value.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder Equal(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.Equals, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder NotEqual(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.NotEquals, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder Contains(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.Contains, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder StartsWith(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.StartsWith, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder EndsWith(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.EndsWith, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder GreaterThan(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.GreaterThan, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder GreaterOrEqual(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.GreaterThanOrEqual, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder LessThan(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.LessThan, Literal(value));

    /// <inheritdoc cref="Equal"/>
    public QueryFilterBuilder LessOrEqual(string alias, string field, object? value)
        => Compare(alias, field, QueryOperator.LessThanOrEqual, Literal(value));

    /// <summary>Adds an inclusive range comparison.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="from">Lower bound, inclusive.</param>
    /// <param name="to">Upper bound, inclusive.</param>
    public QueryFilterBuilder Between(string alias, string field, object? from, object? to)
        => Compare(alias, field, QueryOperator.Between, Literal(from), Literal(to));

    /// <summary>Adds a set-membership test.</summary>
    /// <typeparam name="T">Element type of the value set.</typeparam>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="values">The values to test membership against.</param>
    public QueryFilterBuilder In<T>(string alias, string field, IEnumerable<T> values)
        => Compare(alias, field, QueryOperator.In, QueryOperand.List(QueryValue.ToInvariantList(values)));

    /// <inheritdoc cref="In{T}"/>
    public QueryFilterBuilder NotIn<T>(string alias, string field, IEnumerable<T> values)
        => Compare(alias, field, QueryOperator.NotIn, QueryOperand.List(QueryValue.ToInvariantList(values)));

    /// <summary>Requires the field to hold no value.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    public QueryFilterBuilder IsNull(string alias, string field)
        => Compare(alias, field, QueryOperator.IsNull);

    /// <summary>Requires the field to hold some value.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    public QueryFilterBuilder IsNotNull(string alias, string field)
        => Compare(alias, field, QueryOperator.IsNotNull);

    /// <summary>Compares two fields against each other, as in <c>a.x = b.y</c>.</summary>
    /// <param name="alias">Alias of the participant the left field belongs to.</param>
    /// <param name="field">Logical name of the left field.</param>
    /// <param name="op">The comparison to apply.</param>
    /// <param name="otherAlias">Alias of the participant the right field belongs to.</param>
    /// <param name="otherField">Logical name of the right field.</param>
    public QueryFilterBuilder CompareField(
        string alias, string field, QueryOperator op, string otherAlias, string otherField)
        => Compare(alias, field, op, QueryOperand.Of(new QueryFieldRef(otherAlias, otherField)));

    /// <summary>Requires two fields to be equal, as in <c>a.x = b.y</c>.</summary>
    /// <param name="alias">Alias of the participant the left field belongs to.</param>
    /// <param name="field">Logical name of the left field.</param>
    /// <param name="otherAlias">Alias of the participant the right field belongs to.</param>
    /// <param name="otherField">Logical name of the right field.</param>
    public QueryFilterBuilder EqualField(string alias, string field, string otherAlias, string otherField)
        => CompareField(alias, field, QueryOperator.Equals, otherAlias, otherField);

    /// <summary>
    /// Requires a timestamp to fall within the last so many units. The offset stays relative, so a
    /// saved query still means "the last 30 days" the next time it runs.
    /// </summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="amount">How many units back the window reaches.</param>
    /// <param name="unit">The unit the window is counted in.</param>
    public QueryFilterBuilder Since(string alias, string field, int amount, QueryTimeUnit unit)
        => Compare(alias, field, QueryOperator.GreaterThanOrEqual, QueryOperand.Ago(amount, unit));

    /// <summary>Requires a timestamp to be older than the given relative offset.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="amount">How many units back the cutoff sits.</param>
    /// <param name="unit">The unit the offset is counted in.</param>
    public QueryFilterBuilder Before(string alias, string field, int amount, QueryTimeUnit unit)
        => Compare(alias, field, QueryOperator.LessThan, QueryOperand.Ago(amount, unit));

    /// <summary>
    /// Compares a computed aggregate, named by its select output alias, against a value. Belongs in a
    /// HAVING clause: it filters groups after aggregation, which is where a count can be tested.
    /// </summary>
    /// <param name="outputAlias">Output name of the selected aggregate.</param>
    /// <param name="op">The comparison to apply.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder CompareSelect(string outputAlias, QueryOperator op, object? value)
    {
        _conditions.Add(new QueryCondition(null, op)
        {
            Select = outputAlias,
            Value = Literal(value),
        });
        return this;
    }

    /// <summary>Keeps groups whose computed aggregate is greater than the value.</summary>
    /// <param name="outputAlias">Output name of the selected aggregate.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder SelectGreaterThan(string outputAlias, object? value)
        => CompareSelect(outputAlias, QueryOperator.GreaterThan, value);

    /// <summary>Keeps groups whose computed aggregate is less than the value.</summary>
    /// <param name="outputAlias">Output name of the selected aggregate.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder SelectLessThan(string outputAlias, object? value)
        => CompareSelect(outputAlias, QueryOperator.LessThan, value);

    /// <summary>Keeps groups whose computed aggregate equals the value.</summary>
    /// <param name="outputAlias">Output name of the selected aggregate.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder SelectEqual(string outputAlias, object? value)
        => CompareSelect(outputAlias, QueryOperator.Equals, value);

    /// <summary>Compares the result of a value function against a fixed value.</summary>
    /// <param name="call">The value function call being tested.</param>
    /// <param name="op">The comparison to apply.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder CompareCall(QueryFunctionCall call, QueryOperator op, object? value)
    {
        _conditions.Add(new QueryCondition(null, op) { Call = call, Value = Literal(value) });
        return this;
    }

    /// <summary>Requires the result of a value function to equal a fixed value.</summary>
    /// <param name="call">The value function call being tested.</param>
    /// <param name="value">The value to compare against.</param>
    public QueryFilterBuilder EqualCall(QueryFunctionCall call, object? value)
        => CompareCall(call, QueryOperator.Equals, value);

    /// <summary>Compares a field against the result of a value function.</summary>
    /// <param name="alias">Alias of the participant the field belongs to.</param>
    /// <param name="field">Logical field name.</param>
    /// <param name="op">The comparison to apply.</param>
    /// <param name="call">The value function call supplying the other side.</param>
    public QueryFilterBuilder CompareToCall(
        string alias, string field, QueryOperator op, QueryFunctionCall call)
        => Compare(alias, field, op, QueryOperand.Function(call));

    /// <summary>Nests a child node whose own children are combined with AND.</summary>
    /// <param name="configure">Builds the nested node.</param>
    public QueryFilterBuilder Group(Action<QueryFilterBuilder> configure)
        => Nest(configure, or: false);

    /// <summary>Nests a child node whose own children are combined with OR.</summary>
    /// <param name="configure">Builds the nested node.</param>
    public QueryFilterBuilder AnyOf(Action<QueryFilterBuilder> configure)
        => Nest(configure, or: true);

    /// <summary>Materializes the node built so far.</summary>
    public QueryFilterGroup Build() => new()
    {
        Or = _or,
        Conditions = _conditions.ToArray(),
        Groups = _groups.ToArray(),
    };

    private QueryFilterBuilder Nest(Action<QueryFilterBuilder> configure, bool or)
    {
        if (configure is null) return this;
        var nested = new QueryFilterBuilder();
        if (or) nested.Any();
        configure(nested);
        var built = nested.Build();
        // An empty node would render as an always-true clause that only adds noise.
        if (!built.IsEmpty) _groups.Add(built);
        return this;
    }

    private static QueryOperand Literal(object? value) => QueryOperand.Literal(QueryValue.ToInvariant(value));
}
