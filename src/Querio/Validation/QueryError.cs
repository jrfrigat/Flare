using System.Collections.Generic;

namespace Querio;

/// <summary>
/// What is wrong with a query. Codes are stable so a caller can react to a specific problem - and a
/// visual designer can point at the offending row - without matching on message text.
/// </summary>
public enum QueryErrorCode
{
    /// <summary>The schema declares no entity by that key.</summary>
    UnknownEntity,

    /// <summary>The entity declares no field by that key.</summary>
    UnknownField,

    /// <summary>The alias belongs to no participant in this query.</summary>
    UnknownAlias,

    /// <summary>Two participants claim the same alias.</summary>
    DuplicateAlias,

    /// <summary>A participant was given no alias, so its fields cannot be referenced.</summary>
    MissingAlias,

    /// <summary>The schema declares no relation by that key.</summary>
    UnknownRelation,

    /// <summary>The relation does not connect the joined entity to anything already in the query.</summary>
    RelationNotConnected,

    /// <summary>The join names neither a relation nor explicit match conditions.</summary>
    MissingJoinCondition,

    /// <summary>The schema does not permit that operator on that field.</summary>
    OperatorNotAllowed,

    /// <summary>The schema does not permit that aggregate on that field.</summary>
    AggregateNotAllowed,

    /// <summary>The schema marks the field as not filterable.</summary>
    FieldNotFilterable,

    /// <summary>The schema marks the field as not groupable.</summary>
    FieldNotGroupable,

    /// <summary>The operator needs an operand that the condition does not supply.</summary>
    MissingOperand,

    /// <summary>The operator takes no operand, yet one was supplied.</summary>
    UnexpectedOperand,

    /// <summary>The query groups rows, so every plain selected field must also be grouped by.</summary>
    MissingGroupBy,

    /// <summary>The aggregate needs a field to work on; only a row count may omit one.</summary>
    AggregateWithoutField,

    /// <summary>The selected item names neither a field nor an aggregate, so it returns nothing.</summary>
    EmptySelectItem,

    /// <summary>A percentile was requested without saying which one.</summary>
    MissingPercentileRank,

    /// <summary>The percentile rank falls outside the range 0 to 1.</summary>
    InvalidPercentileRank,

    /// <summary>The condition or ordering names an output alias that nothing selects.</summary>
    UnknownSelectAlias,

    /// <summary>A condition on a computed aggregate appears outside HAVING, where no aggregate exists yet.</summary>
    SelectConditionOutsideHaving,

    /// <summary>The condition names neither a field nor a select output alias.</summary>
    MissingConditionTarget,

    /// <summary>The condition names both a field and a select output alias, so its target is ambiguous.</summary>
    AmbiguousConditionTarget,

    /// <summary>Date truncation was applied to a field that does not hold a timestamp.</summary>
    TruncationNotApplicable,

    /// <summary>A relative time offset was compared against a field that does not hold a timestamp.</summary>
    RelativeValueNotApplicable,

    /// <summary>The row limit is negative.</summary>
    InvalidLimit,

    /// <summary>The row offset is negative.</summary>
    InvalidOffset,
}

/// <summary>One problem found in a query, with a stable code and the location it was found at.</summary>
/// <param name="Code">What kind of problem this is.</param>
/// <param name="Message">A readable description, meant for a developer or a designer's error list.</param>
/// <param name="Path">Where in the query it sits, such as <c>Select[2]</c> or <c>Where.Conditions[0]</c>.</param>
public sealed record QueryError(QueryErrorCode Code, string Message, string Path);

/// <summary>The outcome of validating a query: every problem found, or none at all.</summary>
public sealed class QueryValidationResult
{
    /// <summary>Builds a result over the problems found.</summary>
    /// <param name="errors">The problems, in the order they were found.</param>
    public QueryValidationResult(IReadOnlyList<QueryError> errors) => Errors = errors ?? [];

    /// <summary>Every problem found, in the order they were found.</summary>
    public IReadOnlyList<QueryError> Errors { get; }

    /// <summary>True when the query has no problems and is safe to render.</summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>Throws <see cref="QueryValidationException"/> when the query has any problem.</summary>
    public void ThrowIfInvalid()
    {
        if (!IsValid) throw new QueryValidationException(Errors);
    }
}

/// <summary>Thrown when a query that must be valid turns out not to be.</summary>
public sealed class QueryValidationException : Exception
{
    /// <summary>Builds the exception over the problems that caused it.</summary>
    /// <param name="errors">The problems found while validating.</param>
    public QueryValidationException(IReadOnlyList<QueryError> errors)
        : base(Describe(errors)) => Errors = errors ?? [];

    /// <summary>Every problem that caused this exception.</summary>
    public IReadOnlyList<QueryError> Errors { get; }

    private static string Describe(IReadOnlyList<QueryError> errors)
    {
        if (errors is null || errors.Count == 0) return "The query is not valid.";
        var parts = new string[errors.Count];
        for (var i = 0; i < errors.Count; i++) parts[i] = $"{errors[i].Path}: {errors[i].Message}";
        return "The query is not valid. " + string.Join(" ", parts);
    }
}
