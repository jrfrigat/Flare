using System.Reflection;
using Flare.Components.Combobox;

namespace Flare.Components.Tests.Combobox;

// Guard: the pure combobox engine must never reference the Blazor/JS surface. This is the single invariant
// that keeps the engine unit-testable and keeps behaviour out of the render layer (the exact failure mode
// that grew the old 611-line FlareSelectBase). It is a required gate, not a nicety.
public class EnginePurityGuardTests
{
    // Signature types anywhere in these namespaces are forbidden in the engine core.
    private static readonly string[] ForbiddenNamespaces =
    {
        "Microsoft.AspNetCore.Components",
        "Microsoft.JSInterop",
    };

    // Deliberately-exempt shell helpers that legitimately consume a RenderFragment (declarative options).
    private static readonly HashSet<string> Exempt = new(StringComparer.Ordinal)
    {
        "DeclaredOptions",
        "DeclaredOptionSet`1",
    };

    private static IEnumerable<Type> EngineTypes() =>
        typeof(ComboboxState<>).Assembly.GetTypes()
            .Where(t => t.Namespace == "Flare.Components.Combobox")
            .Where(t => !Exempt.Contains(t.Name));

    [Fact]
    public void Engine_types_exist_and_are_discovered()
    {
        var names = EngineTypes().Select(t => t.Name).ToList();
        Assert.Contains("ComboboxState`1", names);
        Assert.Contains("ListCollection`1", names);
        Assert.Contains("SelectionManager`1", names);
        Assert.Contains("ComboboxPolicy`1", names);
    }

    [Fact]
    public void Engine_core_references_no_blazor_or_js_types()
    {
        var violations = new List<string>();

        foreach (var type in EngineTypes())
        {
            const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic |
                                     BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

            foreach (var f in type.GetFields(all))
                Check(type, $"field {f.Name}", f.FieldType, violations);

            foreach (var p in type.GetProperties(all))
                Check(type, $"property {p.Name}", p.PropertyType, violations);

            foreach (var c in type.GetConstructors(all))
                foreach (var par in c.GetParameters())
                    Check(type, $"ctor param {par.Name}", par.ParameterType, violations);

            foreach (var m in type.GetMethods(all))
            {
                Check(type, $"method {m.Name} return", m.ReturnType, violations);
                foreach (var par in m.GetParameters())
                    Check(type, $"method {m.Name} param {par.Name}", par.ParameterType, violations);
            }
        }

        Assert.True(violations.Count == 0,
            "Engine purity violated:\n" + string.Join("\n", violations));
    }

    private static void Check(Type owner, string where, Type used, List<string> violations)
    {
        foreach (var t in Flatten(used))
        {
            var ns = t.Namespace ?? string.Empty;
            if (ForbiddenNamespaces.Any(f => ns == f || ns.StartsWith(f + ".", StringComparison.Ordinal)))
                violations.Add($"{owner.Name}.{where} references {t.FullName}");
        }
    }

    // Yields a type plus any generic-argument / element types it composes.
    private static IEnumerable<Type> Flatten(Type t)
    {
        yield return t;
        if (t.HasElementType && t.GetElementType() is { } el)
            foreach (var e in Flatten(el)) yield return e;
        if (t.IsGenericType)
            foreach (var g in t.GetGenericArguments())
                foreach (var e in Flatten(g)) yield return e;
    }
}
