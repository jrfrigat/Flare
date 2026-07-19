using System.Collections.Generic;

namespace Querio.Sql;

/// <summary>
/// One value the rendered SQL refers to by placeholder. Values are never spliced into the text, so
/// a caller hands these straight to its driver and injection has nowhere to happen.
/// </summary>
/// <param name="Name">The parameter name the driver expects, without any dialect prefix.</param>
/// <param name="Value">The value, already converted from its stored form to a .NET type.</param>
public sealed record SqlQueryParameter(string Name, object? Value);

/// <summary>Rendered SQL together with the parameters it refers to.</summary>
public sealed class SqlRenderResult
{
    /// <summary>Builds a result over rendered text and its parameters.</summary>
    /// <param name="sql">The rendered SQL.</param>
    /// <param name="parameters">The parameters the text refers to, in the order they were created.</param>
    public SqlRenderResult(string sql, IReadOnlyList<SqlQueryParameter> parameters)
    {
        Sql = sql;
        Parameters = parameters ?? [];
    }

    /// <summary>The rendered SQL. Contains placeholders, never literal values.</summary>
    public string Sql { get; }

    /// <summary>The parameters the SQL refers to, in the order they were created.</summary>
    public IReadOnlyList<SqlQueryParameter> Parameters { get; }

    /// <inheritdoc/>
    public override string ToString() => Sql;
}
