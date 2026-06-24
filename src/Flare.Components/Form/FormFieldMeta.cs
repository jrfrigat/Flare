using System.Collections.Concurrent;
using System.Reflection;

namespace Flare.Components;

internal sealed class FormFieldMeta
{
    public PropertyInfo Property { get; init; } = default!;
    public string Label { get; init; } = "";
    public string Placeholder { get; init; } = "";
    public int Order { get; init; }
    public FormFieldType FieldType { get; init; }
}

internal enum FormFieldType { Text, Number, Bool, Date, Enum }

/// <summary>Per-type reflection cache so FlareFormBuilder does not repeat reflection per render.</summary>
internal static class FormFieldMetaCache
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<FormFieldMeta>> _cache = new();

    internal static IReadOnlyList<FormFieldMeta> GetOrBuild(Type modelType, Func<Type, IReadOnlyList<FormFieldMeta>> factory)
        => _cache.GetOrAdd(modelType, factory);
}
