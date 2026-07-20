namespace Querio;

/// <summary>What the right-hand side of a condition actually holds.</summary>
public enum QueryOperandKind
{
    /// <summary>A fixed value, carried in <see cref="QueryOperand.Value"/>.</summary>
    Literal,

    /// <summary>Another field, carried in <see cref="QueryOperand.Field"/> - as in <c>a.x = b.y</c>.</summary>
    Field,

    /// <summary>A set of values, carried in <see cref="QueryOperand.Values"/>, for the set operators.</summary>
    List,

    /// <summary>An offset from the current moment, carried in <see cref="QueryOperand.Relative"/>.</summary>
    Relative,

    /// <summary>A call to a declared function, carried in <see cref="QueryOperand.Call"/>.</summary>
    Function,
}

/// <summary>The unit of a relative time offset.</summary>
public enum QueryTimeUnit
{
    /// <summary>Minutes.</summary>
    Minute,

    /// <summary>Hours.</summary>
    Hour,

    /// <summary>Days.</summary>
    Day,

    /// <summary>Weeks.</summary>
    Week,

    /// <summary>Months.</summary>
    Month,

    /// <summary>Quarters.</summary>
    Quarter,

    /// <summary>Years.</summary>
    Year,
}

/// <summary>
/// An offset from "now", signed: negative reaches into the past, positive into the future. Storing
/// the offset rather than a resolved timestamp is what lets a saved query mean "the last 30 days"
/// every time it runs, instead of freezing the 30 days that had just passed when it was built.
/// </summary>
/// <param name="Amount">Signed offset; -30 with <see cref="QueryTimeUnit.Day"/> means 30 days ago.</param>
/// <param name="Unit">The unit the offset is counted in.</param>
public sealed record QueryRelativeValue(int Amount, QueryTimeUnit Unit);

/// <summary>
/// The right-hand side of a condition. It is tagged with a <see cref="Kind"/> rather than modelled
/// as a type hierarchy so that it round-trips through any serializer without polymorphism settings -
/// the wire format has to stay boring, because saved queries outlive the code that wrote them.
/// <para>
/// Values travel as invariant-culture strings, never as rendered SQL. The renderer parses them using
/// the field's declared <see cref="QueryFieldType"/> and passes them as parameters, which is what
/// keeps injection impossible by construction.
/// </para>
/// </summary>
public sealed record QueryOperand
{
    /// <summary>Which of the value members below carries this operand's payload.</summary>
    public QueryOperandKind Kind { get; init; }

    /// <summary>The fixed value, when <see cref="Kind"/> is <see cref="QueryOperandKind.Literal"/>.</summary>
    public string? Value { get; init; }

    /// <summary>The value set, when <see cref="Kind"/> is <see cref="QueryOperandKind.List"/>.</summary>
    public IReadOnlyList<string>? Values { get; init; }

    /// <summary>The referenced field, when <see cref="Kind"/> is <see cref="QueryOperandKind.Field"/>.</summary>
    public QueryFieldRef? Field { get; init; }

    /// <summary>The time offset, when <see cref="Kind"/> is <see cref="QueryOperandKind.Relative"/>.</summary>
    public QueryRelativeValue? Relative { get; init; }

    /// <summary>The function call, when <see cref="Kind"/> is <see cref="QueryOperandKind.Function"/>.</summary>
    public QueryFunctionCall? Call { get; init; }

    /// <summary>An operand holding a fixed value, in invariant-culture string form.</summary>
    /// <param name="value">The value to compare against.</param>
    public static QueryOperand Literal(string? value)
        => new() { Kind = QueryOperandKind.Literal, Value = value };

    /// <summary>An operand holding another field, for field-to-field comparisons.</summary>
    /// <param name="field">The field to compare against.</param>
    public static QueryOperand Of(QueryFieldRef field)
        => new() { Kind = QueryOperandKind.Field, Field = field };

    /// <summary>An operand holding a set of values, for the set operators.</summary>
    /// <param name="values">The values to test membership against.</param>
    public static QueryOperand List(IReadOnlyList<string> values)
        => new() { Kind = QueryOperandKind.List, Values = values };

    /// <summary>An operand that calls a declared function.</summary>
    /// <param name="call">The call to evaluate.</param>
    public static QueryOperand Function(QueryFunctionCall call)
        => new() { Kind = QueryOperandKind.Function, Call = call };

    /// <summary>An operand that calls a declared function by key with the given arguments.</summary>
    /// <param name="function">Key of the declared function.</param>
    /// <param name="arguments">The arguments, in parameter order.</param>
    public static QueryOperand Function(string function, params QueryOperand[] arguments)
        => Function(new QueryFunctionCall(function) { Arguments = arguments });

    /// <summary>An operand meaning "this long before now", re-evaluated every time the query runs.</summary>
    /// <param name="amount">How many units into the past; pass a positive number.</param>
    /// <param name="unit">The unit the offset is counted in.</param>
    public static QueryOperand Ago(int amount, QueryTimeUnit unit)
        => new() { Kind = QueryOperandKind.Relative, Relative = new QueryRelativeValue(-Math.Abs(amount), unit) };

    /// <summary>An operand meaning "this long after now", re-evaluated every time the query runs.</summary>
    /// <param name="amount">How many units into the future; pass a positive number.</param>
    /// <param name="unit">The unit the offset is counted in.</param>
    public static QueryOperand FromNow(int amount, QueryTimeUnit unit)
        => new() { Kind = QueryOperandKind.Relative, Relative = new QueryRelativeValue(Math.Abs(amount), unit) };
}
