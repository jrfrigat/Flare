namespace Flare.Components;

/// <summary>One editable condition row in the advanced filter builder.</summary>
public sealed class FilterCondition
{
    /// <summary>Column key this condition filters on.</summary>
    public string Key { get; set; } = "";
    /// <summary>Comparison operator.</summary>
    public FilterOperator Operator { get; set; } = FilterOperator.Equals;
    /// <summary>Primary comparison value.</summary>
    public string? Value { get; set; }
    /// <summary>Second value for range operators (e.g. Between).</summary>
    public string? Value2 { get; set; }
    /// <summary>Values for set operators (e.g. In).</summary>
    public List<string> Values { get; } = [];
}

/// <summary>A logical group in the advanced builder: child conditions and sub-groups combined with AND/OR.</summary>
public sealed class FilterGroupNode
{
    /// <summary>Or.</summary>
    public bool Or { get; set; }                       // false = And, true = Or
    /// <summary>Conditions.</summary>
    public List<FilterCondition> Conditions { get; } = [];
    /// <summary>Groups.</summary>
    public List<FilterGroupNode> Groups { get; } = [];
}
