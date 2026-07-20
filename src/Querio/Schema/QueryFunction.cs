namespace Querio;

/// <summary>What a declared function yields, which decides where a query may use it.</summary>
public enum QueryFunctionKind
{
    /// <summary>Yields one value, so it stands anywhere a field does.</summary>
    Value,

    /// <summary>Yields rows, so it stands where an entity does: as the source, or as a join target.</summary>
    Table,
}

/// <summary>One parameter of a declared function.</summary>
/// <param name="Key">Parameter name, used for diagnostics and for labelling the argument in a designer.</param>
/// <param name="Label">Human-readable caption.</param>
/// <param name="Type">The semantic kind of value the parameter takes.</param>
public sealed record QueryFunctionParameter(string Key, string Label, QueryFieldType Type)
{
    /// <summary>Whether the argument may be omitted. Optional parameters have to come last.</summary>
    public bool Optional { get; init; }
}

/// <summary>
/// A function the consumer allows queries to call. Like entities and fields, a function is declared
/// rather than assumed: Querio knows nothing about what any store provides, so the schema is the only
/// place a query learns that <c>CalcTax</c> exists, what it takes and what it gives back.
/// <para>
/// <see cref="Source"/> carries the physical name, which is what lets one logical function render
/// against a different name per backend - and lets a value function map onto a built-in of whatever
/// query language is being targeted.
/// </para>
/// </summary>
/// <param name="Key">Logical function name, unique within the schema. Matched case-insensitively.</param>
/// <param name="Label">Human-readable caption shown in a function picker.</param>
/// <param name="Kind">Whether the function yields a value or rows.</param>
public sealed record QueryFunction(string Key, string Label, QueryFunctionKind Kind)
{
    /// <summary>The parameters the function takes, in order.</summary>
    public IReadOnlyList<QueryFunctionParameter> Parameters { get; init; } = [];

    /// <summary>
    /// What a <see cref="QueryFunctionKind.Value"/> function yields. Drives which operators and
    /// aggregates apply to a call, exactly as a field's type does. Ignored for a table function.
    /// </summary>
    public QueryFieldType ReturnType { get; init; } = QueryFieldType.Text;

    /// <summary>
    /// The columns a <see cref="QueryFunctionKind.Table"/> function yields. Field references against
    /// the call resolve here, so a table function participates like any entity. Ignored for a value
    /// function.
    /// </summary>
    public IReadOnlyList<QueryField> Columns { get; init; } = [];

    /// <summary>Physical name when it differs from <see cref="Key"/>. Null means they match.</summary>
    public string? Source { get; init; }

    /// <summary>The physical name to render: <see cref="Source"/> when set, otherwise <see cref="Key"/>.</summary>
    public string PhysicalName => string.IsNullOrEmpty(Source) ? Key : Source!;

    /// <summary>How many arguments must be supplied before the optional tail begins.</summary>
    public int RequiredParameterCount
    {
        get
        {
            var required = 0;
            for (var i = 0; i < Parameters.Count; i++)
            {
                if (!Parameters[i].Optional) required = i + 1;
            }
            return required;
        }
    }

    /// <summary>Finds a column of a table function by key, case-insensitively.</summary>
    /// <param name="key">The logical column name.</param>
    public QueryField? FindColumn(string key)
    {
        for (var i = 0; i < Columns.Count; i++)
        {
            if (string.Equals(Columns[i].Key, key, StringComparison.OrdinalIgnoreCase)) return Columns[i];
        }
        return null;
    }
}
