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

    /// <summary>Returns a friendly C# representation of <paramref name="type"/> without nullability.</summary>
    public static string Format(Type type) => Format(type, NullabilityAnnotations.Oblivious);

    /// <summary>Returns a friendly C# representation of <paramref name="type"/>, honoring <paramref name="annotations"/>.</summary>
    public static string Format(Type type, NullabilityAnnotations annotations)
    {
        var index = 0;
        return FormatCore(type, annotations, ref index);
    }

    private static string FormatCore(Type type, NullabilityAnnotations annotations, ref int index)
    {
        if (type.IsByRef)
            return FormatCore(type.GetElementType()!, annotations, ref index);

        // Nullable<T> is encoded in the type itself and takes no annotation slot.
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
            return FormatCore(underlying, NullabilityAnnotations.Oblivious, ref index) + "?";

        // The compiler writes one annotation byte for every reference type, generic parameter and generic
        // struct, and none for a plain value type. Generic structs consume a slot (usually 0) and still
        // descend into their arguments, which is why EventCallback<T> starts with 0, not 1.
        var isValueType = type.IsValueType;
        var consumesSlot = !isValueType || type.IsGenericType;
        var flag = consumesSlot ? annotations.Next(ref index) : (byte)0;

        string text;
        if (type.IsArray)
        {
            text = FormatCore(type.GetElementType()!, annotations, ref index) + "[]";
        }
        else if (type.IsGenericType)
        {
            var name = TrimGenericArity(type.GetGenericTypeDefinition().Name);
            var args = type.GetGenericArguments();
            var formattedArgs = new string[args.Length];
            for (var i = 0; i < args.Length; i++)
                formattedArgs[i] = FormatCore(args[i], annotations, ref index);

            text = $"{name}<{string.Join(", ", formattedArgs)}>";
        }
        else if (type.FullName is { } full && Aliases.TryGetValue(full, out var alias))
        {
            text = alias;
        }
        else
        {
            text = type.Name;
        }

        // Annotated (2) means nullable; 1 is explicit non-nullable and 0 is oblivious, as before.
        if (!isValueType && flag == 2)
            text += "?";

        return text;
    }

    private static string TrimGenericArity(string name)
    {
        var idx = name.IndexOf('`');
        return idx >= 0 ? name[..idx] : name;
    }
}
