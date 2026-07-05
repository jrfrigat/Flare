using System.Reflection;
using System.Runtime.CompilerServices;
using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Components;

namespace Flare.Core.Tests;

/// <summary>
/// Architecture-boundary guard: Flare.Abstractions, Flare.Theming and Flare.Components must never
/// reference a theme package (Flare.Theme.*). Themes are external, optional add-ons discovered at
/// runtime via <c>ITheme</c>/<c>IPaletteProvider</c> reflection (see
/// <c>ServiceCollectionExtensions.DiscoverThemes</c>) - any developer can ship their own, so the core
/// must build and run with zero theme packages installed. A direct reference here would silently
/// couple the core to one theme's assembly, defeating that plugin model.
/// </summary>
public sealed class ThemeIndependenceTests
{
    // One marker type per ring that must stay theme-agnostic; each pulls in its assembly.
    private static readonly (string Ring, Assembly Assembly)[] CoreRings =
    [
        ("Flare.Abstractions", typeof(ITheme).Assembly),
        ("Flare.Theming", typeof(Flare.Theming.CssVarMap).Assembly),
        ("Flare.Components", typeof(FlareComponentBase).Assembly),
    ];

    [Theory]
    [MemberData(nameof(Rings))]
    public void CoreRing_DoesNotReferenceAnyThemePackage(string ringName, Assembly assembly)
    {
        var themeReferences = assembly.GetReferencedAssemblies()
            .Where(IsThemePackage)
            .Select(a => a.Name)
            .ToArray();

        Assert.True(themeReferences.Length == 0,
            $"{ringName} ({assembly.GetName().Name}) references theme package(s): " +
            string.Join(", ", themeReferences) +
            ". Themes are external add-ons - the core must not depend on any of them.");
    }

    public static IEnumerable<object[]> Rings() =>
        CoreRings.Select(r => new object[] { r.Ring, r.Assembly });

    // "Flare.Theme." (with the trailing dot) so this doesn't false-match "Flare.Theming" itself.
    private static bool IsThemePackage(AssemblyName name) =>
        name.Name is not null && name.Name.StartsWith("Flare.Theme.", StringComparison.Ordinal);

    /// <summary>
    /// Token-default guard: no token record in Flare.Abstractions may ship a literal default value.
    /// Flare is theme-agnostic - it works ONLY with a loaded theme and must not bake any theme's
    /// concrete CSS literal into the core. Every <c>string</c>-typed token member (the leaf that would
    /// otherwise carry a "16px"/"var(--...)"/"#RRGGBB" literal) must therefore be <c>required</c>, so
    /// a theme (or reference package) is forced to supply every value. A newly-added token that forgets
    /// <c>required</c> and ships a default fails here, keeping the "no core defaults" mandate enforced.
    /// </summary>
    [Fact]
    public void AbstractionsTokenRecords_ShipNoLiteralDefaults()
    {
        var nullability = new NullabilityInfoContext();
        var offenders = typeof(DesignTokens).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsPublic: true, Namespace: not null }
                        && t.Namespace.StartsWith("Flare.Abstractions.Tokens", StringComparison.Ordinal))
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                // Only the leaf, NON-NULLABLE string members can carry a baked CSS literal: a
                // non-required, non-nullable string must have a non-null initializer to compile clean,
                // which is exactly the theme literal the mandate forbids in the core. Nullable metadata
                // (string? Source/StyleAsset) defaults to null (absent, not a literal) and is allowed;
                // nested records are covered by their own members; non-string members (the Extended
                // extras map) are not theme literals.
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite)
                .Where(p => nullability.Create(p).WriteState != NullabilityState.Nullable)
                .Where(p => p.GetCustomAttribute<RequiredMemberAttribute>() is null)
                .Select(p => $"{t.Name}.{p.Name}"))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.True(offenders.Length == 0,
            "Flare.Abstractions token member(s) ship a default instead of being 'required' - the core " +
            "must not bake theme literals; make them 'required' and move the value to the theme/reference " +
            "package: " + string.Join(", ", offenders));
    }

    // Concrete theme identifiers (design-system product + font names) that must never appear in the
    // theme-agnostic core source. The core describes a token's ROLE, not a specific theme's value -
    // theme names and their concrete values (e.g. "MD3 = 80dp") belong in the theme packages. These
    // are product-name tokens, not English words, so they never false-match: "Material Design" and
    // "Material You" (NOT bare "material", which is the allowed "material-symbols" icon font or
    // "materialize"), "Fluent UI" / "FluentUI" / "Fluent2" (NOT the "fluent API" idiom), and the
    // theme-specific fonts "Roboto"/"Segoe". Uppercase "MD3"/"MD2"/"FUI2" cannot occur in lowercase-hex
    // GUIDs or SVG "M3"/"M2" moveto path data (which is why bare "M3"/"M2" is intentionally NOT listed).
    private static readonly string[] ForbiddenThemeNames =
    [
        "MD3", "MD2", "FUI2", "Material Design", "Material You", "Fluent UI", "FluentUI", "Fluent2",
        "Cupertino", "Roboto", "Segoe",
    ];

    // The three core rings whose SOURCE must stay free of concrete theme names.
    private static readonly string[] CoreRingDirs =
        ["Flare.Abstractions", "Flare.Theming", "Flare.Components"];

    /// <summary>
    /// Source-level guard for the "Flare knows no concrete theme" mandate: scans every source file in
    /// the three core rings and fails if any names a concrete theme. Comments/XML-docs count - a token
    /// documented as "MD3 = surface-container" silently teaches the core one theme's opinion, which is
    /// exactly what the theme-agnostic, self-sufficient token model forbids. Themes (and their concrete
    /// values) live in the external theme packages, discovered at runtime.
    /// </summary>
    [Fact]
    public void CoreSource_NamesNoConcreteTheme()
    {
        var root = FindRepoRoot();
        string[] sourceExtensions = [".cs", ".razor", ".css", ".js"];
        var offenders = new List<string>();

        foreach (var ring in CoreRingDirs)
        {
            var ringDir = Path.Combine(root, "src", ring);
            foreach (var file in Directory.EnumerateFiles(ringDir, "*", SearchOption.AllDirectories))
            {
                if (!sourceExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                    continue;
                // Skip build output (bin/obj carry stale generated XML docs and compiled assets).
                if (file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal) ||
                    file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                    continue;

                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var hit = ForbiddenThemeNames.FirstOrDefault(name => lines[i].Contains(name, StringComparison.Ordinal));
                    if (hit is not null)
                        offenders.Add($"{Path.GetRelativePath(root, file)}:{i + 1} ({hit})");
                }
            }
        }

        Assert.True(offenders.Count == 0,
            "The theme-agnostic core must not name a concrete theme - describe the token's role, not a " +
            "theme's value, and move theme specifics to the theme packages:\n" + string.Join("\n", offenders));
    }

    // The compile-time path of THIS test file, walked up until the folder that contains "src".
    private static string FindRepoRoot([CallerFilePath] string thisFile = "")
    {
        var dir = Path.GetDirectoryName(thisFile);
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src")))
            dir = Path.GetDirectoryName(dir);
        Assert.False(dir is null, "Could not locate the repository root (no ancestor 'src' folder).");
        return dir!;
    }
}
