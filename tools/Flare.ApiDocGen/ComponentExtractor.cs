using System.Globalization;
using System.Reflection;

namespace Flare.ApiDocGen;

/// <summary>Walks component types via reflection and builds their <see cref="ComponentDoc"/> models.</summary>
internal sealed class ComponentExtractor
{
    private const string ParameterAttr = "Microsoft.AspNetCore.Components.ParameterAttribute";
    private const string CascadingParameterAttr = "Microsoft.AspNetCore.Components.CascadingParameterAttribute";
    private const string EditorRequiredAttr = "Microsoft.AspNetCore.Components.EditorRequiredAttribute";
    internal const string BaseTypeName = "Flare.Components.FlareComponentBase";

    private readonly XmlDocReader _docs;
    private readonly NullabilityInfoContext _nullability = new();
    private readonly Dictionary<Type, object?> _instanceCache = new();

    public ComponentExtractor(XmlDocReader docs) => _docs = docs;

    /// <summary>Returns true if <paramref name="type"/> is a documentable Flare component.</summary>
    public static bool IsComponent(Type type)
    {
        if (!type.IsClass || type.IsAbstract || !type.IsPublic)
            return false;

        for (var b = type.BaseType; b is not null; b = b.BaseType)
            if (b.FullName == BaseTypeName)
                return true;

        return false;
    }

    /// <summary>Returns true if <paramref name="type"/> is a documentable public enum.</summary>
    public static bool IsDocumentableEnum(Type type) => type.IsEnum && type.IsPublic;

    public EnumDoc ExtractEnum(Type type)
    {
        var typeDoc = _docs.ForType(type);
        var members = new List<EnumMemberDoc>();

        foreach (var name in Enum.GetNames(type))
        {
            string value;
            try { value = Convert.ToInt64(Enum.Parse(type, name)).ToString(CultureInfo.InvariantCulture); }
            catch { value = string.Empty; }

            var memberDoc = _docs.ForField(type, name);
            members.Add(new EnumMemberDoc(name, value, memberDoc?.Summary));
        }

        return new EnumDoc(
            Name: CleanName(type.Name),
            FullName: CleanName(type.FullName ?? type.Name),
            Namespace: type.Namespace ?? string.Empty,
            Summary: typeDoc?.Summary,
            Remarks: typeDoc?.Remarks,
            Members: members,
            UsedBy: System.Array.Empty<string>());
    }

    public ComponentDoc Extract(Type type)
    {
        var typeDoc = _docs.ForType(type);

        return new ComponentDoc(
            Name: CleanName(type.Name),
            FullName: CleanName(type.FullName ?? type.Name),
            Namespace: type.Namespace ?? string.Empty,
            Summary: typeDoc?.Summary,
            Remarks: typeDoc?.Remarks,
            Parameters: ExtractParameters(type),
            Methods: ExtractMethods(type),
            Inherits: BaseChain(type),
            DerivedBy: System.Array.Empty<string>());
    }

    private List<ParameterDoc> ExtractParameters(Type type)
    {
        var result = new List<ParameterDoc>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var isParameter = HasAttribute(prop, ParameterAttr);
            var isCascading = HasAttribute(prop, CascadingParameterAttr);
            if (!isParameter && !isCascading)
                continue;

            var nullability = SafeNullability(prop);
            var doc = _docs.ForProperty(prop);
            var propType = prop.PropertyType;
            var isEventCallback = IsEventCallback(propType);

            result.Add(new ParameterDoc(
                Name: prop.Name,
                Type: TypeNameFormatter.Format(propType, nullability),
                Default: ReadDefault(type, prop, isEventCallback),
                Summary: doc?.Summary,
                Remarks: doc?.Remarks,
                IsCascading: isCascading,
                IsEventCallback: isEventCallback,
                IsRequired: HasAttribute(prop, EditorRequiredAttr),
                DeclaringType: CleanName(prop.DeclaringType?.Name ?? type.Name)));
        }

        // Stable, readable ordering: own parameters first (alphabetical), inherited last.
        return result
            .OrderBy(p => p.DeclaringType == type.Name ? 0 : 1)
            .ThenBy(p => p.Name, StringComparer.Ordinal)
            .ToList();
    }

    private List<MethodDoc> ExtractMethods(Type type)
    {
        var result = new List<MethodDoc>();

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (method.IsSpecialName || method.Name == "BuildRenderTree")
                continue;
            if (method.DeclaringType?.FullName == BaseTypeName)
                continue;

            var doc = _docs.ForMethod(method);
            var parameters = new List<MethodParamDoc>();
            foreach (var p in method.GetParameters())
            {
                var paramName = p.Name ?? string.Empty;
                string? pSummary = null;
                doc?.Parameters.TryGetValue(paramName, out pSummary);
                parameters.Add(new MethodParamDoc(
                    Name: paramName,
                    Type: TypeNameFormatter.Format(p.ParameterType),
                    Summary: pSummary));
            }

            result.Add(new MethodDoc(
                Name: method.Name,
                Signature: BuildSignature(method),
                ReturnType: TypeNameFormatter.Format(method.ReturnType),
                ReturnSummary: doc?.Returns,
                Summary: doc?.Summary,
                Parameters: parameters));
        }

        return result.OrderBy(m => m.Name, StringComparer.Ordinal).ToList();
    }

    private static string BuildSignature(MethodInfo method)
    {
        var paramText = method.GetParameters()
            .Select(p => $"{TypeNameFormatter.Format(p.ParameterType)} {p.Name}");
        return $"{method.Name}({string.Join(", ", paramText)})";
    }

    private string? ReadDefault(Type type, PropertyInfo prop, bool isEventCallback)
    {
        if (isEventCallback || !prop.CanRead)
            return null;

        var instance = GetInstance(type);
        if (instance is null)
            return null;

        object? value;
        try { value = prop.GetValue(instance); }
        catch { return null; }

        return FormatValue(value, prop.PropertyType);
    }

    private object? GetInstance(Type type)
    {
        if (_instanceCache.TryGetValue(type, out var cached))
            return cached;

        object? instance = null;
        if (!type.ContainsGenericParameters)
        {
            try { instance = Activator.CreateInstance(type); }
            catch { instance = null; }
        }

        _instanceCache[type] = instance;
        return instance;
    }

    private static string? FormatValue(object? value, Type type)
    {
        if (value is null)
            return null;

        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        switch (value)
        {
            case bool b:
                return b ? "true" : "false";
            case string s:
                return s.Length == 0 ? "\"\"" : "\"" + s + "\"";
            case char c:
                return "'" + c + "'";
        }

        if (underlying.IsEnum)
            return underlying.Name + "." + value;

        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture);

        // Collections, structs (EventCallback) and other complex objects have no meaningful
        // textual default; show nothing rather than an internal runtime type name.
        return null;
    }

    /// <summary>Strips generic arity markers (e.g. "FlareInput`1" -> "FlareInput") for route-safe names.</summary>
    private static string CleanName(string name) =>
        System.Text.RegularExpressions.Regex.Replace(name, "`\\d+", string.Empty);

    /// <summary>Base-type chain from the immediate base up to (and including) System.Object.</summary>
    private static List<string> BaseChain(Type type)
    {
        var chain = new List<string>();
        for (var b = type.BaseType; b is not null; b = b.BaseType)
            chain.Add(b == typeof(object) ? "object" : CleanName(b.Name));
        return chain;
    }

    private static bool IsEventCallback(Type type) =>
        type.Namespace == "Microsoft.AspNetCore.Components" &&
        (type.Name == "EventCallback" || type.Name == "EventCallback`1");

    private static bool HasAttribute(MemberInfo member, string attributeFullName)
    {
        foreach (var attr in member.GetCustomAttributes(inherit: true))
            if (attr.GetType().FullName == attributeFullName)
                return true;
        return false;
    }

    private NullabilityInfo? SafeNullability(PropertyInfo prop)
    {
        try { return _nullability.Create(prop); }
        catch { return null; }
    }
}
