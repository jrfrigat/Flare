using System.Reflection;

namespace Flare.ApiDocGen;

/// <summary>Formats <see cref="Type"/> instances into friendly C# type names for documentation.</summary>
internal static class TypeNameFormatter
{
    private static readonly Dictionary<string, string> Aliases = new()
    {
        ["System.Boolean"] = "bool",
        ["System.Byte"] = "byte",
        ["System.SByte"] = "sbyte",
        ["System.Char"] = "char",
        ["System.Decimal"] = "decimal",
        ["System.Double"] = "double",
        ["System.Single"] = "float",
        ["System.Int32"] = "int",
        ["System.UInt32"] = "uint",
        ["System.Int64"] = "long",
        ["System.UInt64"] = "ulong",
        ["System.Int16"] = "short",
        ["System.UInt16"] = "ushort",
        ["System.Object"] = "object",
        ["System.String"] = "string",
        ["System.Void"] = "void",
    };

    /// <summary>Returns a friendly C# representation of <paramref name="type"/>, honoring the property's nullability.</summary>
    public static string Format(Type type, NullabilityInfo? nullability = null)
    {
        var text = FormatCore(type, nullability);
        // Annotate nullable reference types (value-type nullables are handled via Nullable<T>).
        if (nullability is { ReadState: NullabilityState.Nullable } && !type.IsValueType)
            text += "?";
        return text;
    }

    private static string FormatCore(Type type, NullabilityInfo? nullability)
    {
        if (type.IsByRef)
            return FormatCore(type.GetElementType()!, nullability);

        if (type.IsArray)
        {
            var elementNullability = nullability?.ElementType;
            return Format(type.GetElementType()!, elementNullability) + "[]";
        }

        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
            return FormatCore(underlying, null) + "?";

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            var name = TrimGenericArity(def.Name);
            var args = type.GetGenericArguments();
            var formattedArgs = new string[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var argNullability = nullability is not null && i < nullability.GenericTypeArguments.Length
                    ? nullability.GenericTypeArguments[i]
                    : null;
                formattedArgs[i] = Format(args[i], argNullability);
            }

            return $"{name}<{string.Join(", ", formattedArgs)}>";
        }

        if (type.FullName is { } full && Aliases.TryGetValue(full, out var alias))
            return alias;

        return type.Name;
    }

    private static string TrimGenericArity(string name)
    {
        var idx = name.IndexOf('`');
        return idx >= 0 ? name[..idx] : name;
    }
}
