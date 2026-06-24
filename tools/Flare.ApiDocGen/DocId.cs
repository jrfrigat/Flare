using System.Reflection;
using System.Text;

namespace Flare.ApiDocGen;

/// <summary>
/// Computes ECMA-335 documentation comment member IDs (the values used in the
/// <c>name</c> attribute of <c>&lt;member&gt;</c> elements in XML doc files).
/// </summary>
internal static class DocId
{
    /// <summary>The type portion used after "T:" and as the declaring-type prefix of members.</summary>
    public static string ForType(Type type) => DeclaringName(type);

    /// <summary>Declaring-type name with namespace, nested types joined by '.', generic arity kept as `n.</summary>
    public static string DeclaringName(Type type)
    {
        if (type.IsNested)
            return DeclaringName(type.DeclaringType!) + "." + type.Name;

        var ns = type.Namespace;
        return string.IsNullOrEmpty(ns) ? type.Name : ns + "." + type.Name;
    }

    /// <summary>Full "M:" id for a method, including generic arity and encoded parameter list.</summary>
    public static string ForMethod(MethodInfo method)
    {
        var sb = new StringBuilder("M:");
        sb.Append(DeclaringName(method.DeclaringType!));
        sb.Append('.');
        sb.Append(method.Name);

        if (method.IsGenericMethodDefinition || method.IsGenericMethod)
        {
            var arity = method.GetGenericArguments().Length;
            sb.Append("``").Append(arity);
        }

        var parameters = method.GetParameters();
        if (parameters.Length > 0)
        {
            sb.Append('(');
            for (var i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(TypeReference(parameters[i].ParameterType));
            }
            sb.Append(')');
        }

        return sb.ToString();
    }

    /// <summary>Encodes a type as it appears inside a method parameter list of a member id.</summary>
    private static string TypeReference(Type type)
    {
        if (type.IsByRef)
            return TypeReference(type.GetElementType()!) + "@";

        if (type.IsPointer)
            return TypeReference(type.GetElementType()!) + "*";

        if (type.IsArray)
        {
            var element = TypeReference(type.GetElementType()!);
            var rank = type.GetArrayRank();
            return rank == 1
                ? element + "[]"
                : element + "[" + string.Join(",", Enumerable.Repeat("0:", rank)) + "]";
        }

        if (type.IsGenericParameter)
            return (type.DeclaringMethod is not null ? "``" : "`") + type.GenericParameterPosition;

        if (type.IsGenericType)
        {
            var def = type.GetGenericTypeDefinition();
            var name = StripArity(FullDottedName(def));
            var args = type.GetGenericArguments();
            return name + "{" + string.Join(",", args.Select(TypeReference)) + "}";
        }

        return FullDottedName(type);
    }

    private static string FullDottedName(Type type)
    {
        if (type.IsNested)
            return FullDottedName(type.DeclaringType!) + "." + type.Name;
        var ns = type.Namespace;
        return string.IsNullOrEmpty(ns) ? type.Name : ns + "." + type.Name;
    }

    private static string StripArity(string name)
    {
        var idx = name.IndexOf('`');
        return idx >= 0 ? name[..idx] : name;
    }
}
