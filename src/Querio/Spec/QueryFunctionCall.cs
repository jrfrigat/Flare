using System.Collections.Generic;
using System.Linq;

namespace Querio;

/// <summary>
/// A call to a function the schema declares. Arguments are operands, so an argument may be a fixed
/// value, a field of any participant, a moment relative to now, or another call - which is what lets
/// calls nest without the model needing an expression grammar of its own.
/// <para>
/// A call to a <see cref="QueryFunctionKind.Value"/> function stands anywhere a field does. A call to
/// a <see cref="QueryFunctionKind.Table"/> function stands where an entity does, on
/// <see cref="QuerySource"/> or <see cref="QueryJoin"/>.
/// </para>
/// </summary>
/// <param name="Function">Key of the declared function being called.</param>
public sealed record QueryFunctionCall(string Function)
{
    /// <summary>The arguments, in the order the function declares its parameters.</summary>
    public IReadOnlyList<QueryOperand> Arguments { get; init; } = [];

    /// <summary>Builds a call to a function by key with the given arguments.</summary>
    /// <param name="function">Key of the declared function.</param>
    /// <param name="arguments">The arguments, in parameter order.</param>
    public static QueryFunctionCall Of(string function, params QueryOperand[] arguments)
        => new(function) { Arguments = arguments };

    /// <summary>Builds a call whose arguments are all fields of one participant.</summary>
    /// <param name="function">Key of the declared function.</param>
    /// <param name="alias">Alias of the participant the fields belong to.</param>
    /// <param name="fields">Logical field names, in parameter order.</param>
    public static QueryFunctionCall OfFields(string function, string alias, params string[] fields)
        => new(function)
        {
            Arguments = fields.Select(field => QueryOperand.Of(new QueryFieldRef(alias, field))).ToArray(),
        };
}
