namespace Querio;

/// <summary>
/// The comparison a condition applies. These are semantic, not syntactic: a renderer maps each one
/// onto whatever its dialect spells it as, and supplies the value as a parameter rather than as
/// inlined text.
/// </summary>
public enum QueryOperator
{
    /// <summary>The value appears somewhere inside the field's text.</summary>
    Contains,

    /// <summary>The field equals the value.</summary>
    Equals,

    /// <summary>The field differs from the value.</summary>
    NotEquals,

    /// <summary>The field's text begins with the value.</summary>
    StartsWith,

    /// <summary>The field's text ends with the value.</summary>
    EndsWith,

    /// <summary>The field is greater than the value.</summary>
    GreaterThan,

    /// <summary>The field is greater than or equal to the value.</summary>
    GreaterThanOrEqual,

    /// <summary>The field is less than the value.</summary>
    LessThan,

    /// <summary>The field is less than the value.</summary>
    LessThanOrEqual,

    /// <summary>The field falls within an inclusive range spanning both operands.</summary>
    Between,

    /// <summary>The field falls outside an inclusive range spanning both operands.</summary>
    NotBetween,

    /// <summary>The field matches one of a set of values.</summary>
    In,

    /// <summary>The field matches none of a set of values.</summary>
    NotIn,

    /// <summary>The field holds no value.</summary>
    IsNull,

    /// <summary>The field holds some value.</summary>
    IsNotNull,
}

/// <summary>
/// A value computed over a group of rows. <see cref="Percentile"/> carries its rank separately on
/// <see cref="QuerySelect.Percentile"/>, since which percentile is wanted is data, not a distinct
/// operation. Not every store can compute every aggregate, so a renderer declares what it supports
/// and fails loudly rather than quietly substituting something close.
/// </summary>
public enum QueryAggregate
{
    /// <summary>Number of rows in the group.</summary>
    Count,

    /// <summary>Arithmetic total of the field across the group.</summary>
    Sum,

    /// <summary>Arithmetic mean of the field across the group.</summary>
    Avg,

    /// <summary>Smallest value of the field in the group.</summary>
    Min,

    /// <summary>Largest value of the field in the group.</summary>
    Max,

    /// <summary>Value below which the given fraction of the group falls (see <see cref="QuerySelect.Percentile"/>).</summary>
    Percentile,
}
