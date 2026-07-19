using System.Collections.Generic;

namespace Querio.OneC;

/// <summary>
/// One value the rendered query refers to by name. 1C takes parameters as a name-to-value map, and
/// values are never written into the query text, so injection has nowhere to happen.
/// </summary>
/// <param name="Name">The parameter name, without the leading ampersand.</param>
/// <param name="Value">The value, already converted from its stored form to a .NET type.</param>
public sealed record OneCQueryParameter(string Name, object? Value);

/// <summary>Rendered 1C query text together with the parameters it refers to.</summary>
public sealed class OneCRenderResult
{
    /// <summary>Builds a result over rendered text and its parameters.</summary>
    /// <param name="query">The rendered query text.</param>
    /// <param name="parameters">The parameters the text refers to, in the order they were created.</param>
    public OneCRenderResult(string query, IReadOnlyList<OneCQueryParameter> parameters)
    {
        Query = query;
        Parameters = parameters ?? [];
    }

    /// <summary>The rendered query text. Contains parameter names, never literal values.</summary>
    public string Query { get; }

    /// <summary>The parameters the query refers to, in the order they were created.</summary>
    public IReadOnlyList<OneCQueryParameter> Parameters { get; }

    /// <inheritdoc/>
    public override string ToString() => Query;
}
