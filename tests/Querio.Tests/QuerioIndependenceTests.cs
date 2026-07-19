using System.Reflection;

namespace Querio.Tests;

/// <summary>
/// Architecture-boundary guard. Querio is a standalone product that happens to live in this
/// repository while its contract settles; a backend service must be able to consume it without
/// dragging in a Blazor component library. The moment a Flare type looks convenient inside the core,
/// extraction to its own repository stops being a directory move - so the boundary is enforced by a
/// failing build rather than by good intentions.
/// </summary>
public sealed class QuerioIndependenceTests
{
    private static readonly string[] ForbiddenPrefixes =
    [
        // The whole point: the query model knows nothing about the UI library.
        "Flare",
        // Nor about the web stack, so it stays usable from a console app, a worker or a backend service.
        "Microsoft.AspNetCore",
        "Microsoft.JSInterop",
        "Microsoft.Extensions",
    ];

    [Fact]
    public void Querio_DependsOnNothingButTheBaseClassLibrary()
    {
        var assembly = typeof(QuerySpec).Assembly;
        var offenders = assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .Where(IsForbidden)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.True(offenders.Length == 0,
            $"{assembly.GetName().Name} references {string.Join(", ", offenders)}. " +
            "Querio must stay independent of Flare and of the web stack so it can be consumed on its " +
            "own and later extracted without untangling anything.");
    }

    private static bool IsForbidden(string assemblyName)
        => ForbiddenPrefixes.Any(prefix =>
            assemblyName.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || assemblyName.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void Querio_ShipsNoPublicTypeOutsideItsOwnNamespace()
    {
        var strays = typeof(QuerySpec).Assembly.GetExportedTypes()
            .Where(type => type.Namespace is null
                || !type.Namespace.Equals("Querio", StringComparison.Ordinal))
            .Select(type => type.FullName ?? type.Name)
            .ToArray();

        Assert.True(strays.Length == 0,
            "Every public Querio type belongs to the Querio namespace, so the package claims no name " +
            $"it does not own. Found: {string.Join(", ", strays)}");
    }

    [Fact]
    public void Querio_ExposesEveryPublicTypeWithDocumentation()
    {
        // The XML doc file is what drives generated API docs; CS1591 is an error in the csproj, so a
        // missing file here means the packaging changed rather than a single comment going astray.
        var assembly = typeof(QuerySpec).Assembly;
        var documentation = Path.ChangeExtension(assembly.Location, ".xml");

        Assert.True(File.Exists(documentation),
            $"Expected generated XML documentation next to {assembly.Location}.");
    }
}
