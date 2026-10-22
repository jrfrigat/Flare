using System.Reflection;

namespace Flare.ApiDocGen;

/// <summary>
/// Reads the compiler's raw nullable annotations for a property. <see cref="NullabilityInfoContext"/>
/// cannot be used here: it reports an unconstrained generic parameter as nullable whether the source
/// says <c>T</c> or <c>T?</c>, because both are nullable in the CLR. The compiler's NullableAttribute
/// does record the difference (1 = not annotated, 2 = annotated), so the formatter decodes those bytes
/// in the same order the compiler emitted them.
/// </summary>
internal readonly struct NullabilityAnnotations
{
    private const string NullableAttribute = "System.Runtime.CompilerServices.NullableAttribute";
    private const string NullableContextAttribute = "System.Runtime.CompilerServices.NullableContextAttribute";

    /// <summary>No annotations: every type is treated as oblivious (no <c>?</c> is added).</summary>
    public static readonly NullabilityAnnotations Oblivious = new(null, 0);

    private readonly byte[]? _flags;

    /// <summary>Value applied to a type when the property carries no annotation of its own.</summary>
    public byte Context { get; }

    private NullabilityAnnotations(byte[]? flags, byte context)
    {
        _flags = flags;
        Context = context;
    }

    /// <summary>
    /// Takes the annotation for the next type node in pre-order. Value types that hold no annotations
    /// (an <c>int</c>, a <c>DateTime</c>) do not call this, mirroring the compiler's layout.
    /// </summary>
    public byte Next(ref int index)
    {
        if (_flags is not { Length: > 0 })
            return Context;
        // A single byte is the compiler's compact form for "this value applies to the whole type".
        if (_flags.Length == 1)
            return _flags[0];
        return index < _flags.Length ? _flags[index++] : Context;
    }

    /// <summary>Reads the annotations of a property, falling back to its getter's return parameter.</summary>
    public static NullabilityAnnotations Read(PropertyInfo property)
    {
        var flags = FindFlags(property.CustomAttributes);
        var getter = property.GetMethod;
        flags ??= getter is null ? null : FindFlags(getter.ReturnParameter.CustomAttributes);

        var context =
            (getter is null ? null : FindContext(getter.CustomAttributes))
            ?? FindContext(property.DeclaringType?.CustomAttributes)
            ?? FindContext(property.Module.CustomAttributes)
            ?? 0;

        return new NullabilityAnnotations(flags, context);
    }

    private static byte[]? FindFlags(IEnumerable<CustomAttributeData>? attributes)
    {
        foreach (var attribute in attributes ?? [])
        {
            if (attribute.AttributeType.FullName != NullableAttribute || attribute.ConstructorArguments.Count != 1)
                continue;

            return attribute.ConstructorArguments[0].Value switch
            {
                byte flag => [flag],
                IEnumerable<CustomAttributeTypedArgument> items =>
                    items.Select(item => item.Value is byte b ? b : (byte)0).ToArray(),
                _ => null,
            };
        }

        return null;
    }

    private static byte? FindContext(IEnumerable<CustomAttributeData>? attributes)
    {
        foreach (var attribute in attributes ?? [])
            if (attribute.AttributeType.FullName == NullableContextAttribute &&
                attribute.ConstructorArguments is [{ Value: byte context }])
                return context;

        return null;
    }
}
