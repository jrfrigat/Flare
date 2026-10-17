using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Components;

namespace Flare.Guards.Tests;

/// <summary>
/// Architecture-boundary guard: Flare.Abstractions, Flare.Theming and Flare.Components must never
/// reference a theme package (Flare.Theme.*). Themes are external, optional add-ons registered
/// explicitly through <c>ITheme</c> - any developer can ship their own, so the core
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

    /// <summary>
    /// A theme must not build on another design language's tokens. Material 2 did: it took the
    /// Material 3 baseline and overrode the parts somebody noticed, so everything nobody noticed
    /// arrived as Material 3 - the dark on-surface colour, the checkbox size, the switch's whole
    /// silhouette, all found by an audit against the published Material 2 specification. Inheriting
    /// across languages hides those; owning the values makes each one visible.
    /// </summary>
    [Fact]
    public void MaterialDesign2_DoesNotBuildOnMaterialDesign3Tokens()
    {
        var md2 = typeof(Flare.Theme.MaterialDesign2.MaterialDesign2Theme).Assembly;

        var borrowed = md2.GetReferencedAssemblies()
            .Where(a => a.Name is not null && a.Name.StartsWith("Flare.Theme.", StringComparison.Ordinal))
            .Select(a => a.Name!)
            .ToArray();

        Assert.True(borrowed.Length == 0,
            "Flare.Theme.MaterialDesign2 references another theme package: " + string.Join(", ", borrowed) +
            ". Material 2 predates Material 3 and shares none of its opinions - it ships its own complete " +
            "token set (MaterialDesign2Tokens) instead of inheriting one.");
    }

    // "Flare.Theme." (with the trailing dot) so this doesn't false-match "Flare.Theming" itself.
    private static bool IsThemePackage(AssemblyName name) =>
        name.Name is not null && name.Name.StartsWith("Flare.Theme.", StringComparison.Ordinal);

    /// <summary>
    /// Token-default guard: a token record in Flare.Abstractions may give a member a default only when
    /// that default is built from other Flare tokens. Flare is theme-agnostic, so the core never bakes a
    /// theme's concrete literal: a component token that a base token expresses defaults to it
    /// (<c>var(--flare-color-surface)</c>, or a <c>color-mix</c> of such references), and every other
    /// <c>string</c> member stays <c>required</c> so each theme supplies it. A default carrying a length,
    /// time or colour literal ("16px", "200ms", "#fff") or a reference outside <c>--flare-*</c> fails here.
    /// </summary>
    [Fact]
    public void AbstractionsTokenRecords_DefaultOnlyToFlareTokens()
    {
        var nullability = new NullabilityInfoContext();
        var offenders = typeof(DesignTokens).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsPublic: true, IsAbstract: false, Namespace: not null }
                        && t.Namespace.StartsWith("Flare.Abstractions.Tokens", StringComparison.Ordinal))
            .SelectMany(t =>
            {
                // Only the leaf, NON-NULLABLE string members can carry a default: nullable metadata
                // (string? Source/StyleAsset) defaults to null, nested records are covered by their own
                // members, and non-string members (the Extended extras map) are not values.
                var optional = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType == typeof(string) && p.CanWrite)
                    .Where(p => nullability.Create(p).WriteState != NullabilityState.Nullable)
                    .Where(p => p.GetCustomAttribute<RequiredMemberAttribute>() is null)
                    .ToArray();
                if (optional.Length == 0) return [];
                // Reflection does not enforce 'required', and the initializers run: the defaults are real.
                var instance = Activator.CreateInstance(t)!;
                return optional
                    .Where(p => !IsFlareTokenDefault((string?)p.GetValue(instance)))
                    .Select(p => $"{t.Name}.{p.Name} = \"{p.GetValue(instance)}\"");
            })
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.True(offenders.Length == 0,
            "Flare.Abstractions token member(s) default to something other than Flare tokens - a default " +
            "may only reference --flare-* variables (with percentages and keywords); make the member " +
            "'required' and move the value to the theme: " + string.Join(", ", offenders));
    }

    // A default is made of Flare tokens when it references at least one --flare-* variable, references
    // nothing else, and outside those references carries no length, time or colour literal.
    private static bool IsFlareTokenDefault(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var references = Regex.Matches(value, @"var\(\s*(--[a-zA-Z0-9_-]+)");
        if (references.Count == 0 || references.Any(m => !m.Groups[1].Value.StartsWith("--flare-", StringComparison.Ordinal)))
            return false;
        var rest = Regex.Replace(value, @"var\(\s*--flare-[a-z0-9-]+\s*\)", " ");
        return !Regex.IsMatch(rest,
            @"\d(px|rem|em|vh|vw|ms|s)\b|#[0-9a-fA-F]{3,8}\b|\b(rgb|rgba|hsl|hsla)\(", RegexOptions.IgnoreCase);
    }

    // Concrete theme identifiers (design-system product + font names) that must never appear in the
    // theme-agnostic core source. The core describes a token's ROLE, not a specific theme's value -
    // theme names and their concrete values (e.g. "MD3 = 80dp") belong in the theme packages. These
    // are product-name tokens, not English words, so they never false-match: "Material Design" and
    // "Material You" (NOT bare "material", which is the allowed "material-symbols" icon font or
    // "materialize"), "Fluent UI" / "FluentUI" / "Fluent2" (NOT the "fluent API" idiom), and the
    // theme-specific fonts "Roboto"/"Segoe". Uppercase "MD3"/"MD2"/"FUI2" cannot occur in lowercase-hex
    // GUIDs or SVG "M3"/"M2" moveto path data (which is why bare "M3"/"M2" is intentionally NOT listed).
    // The lowercase ids ("md3-expressive", "md2-indigo", "fui2-...") are how a theme names itself in
    // CSS classes and in stored settings, and they are what the uppercase entries above miss: a
    // hardcoded 'md3-expressive' default sat in flare-bootstrap.js for months because the scan was
    // case-sensitive. The trailing dash keeps them from matching hex digests or ordinary prose.
    private static readonly string[] ForbiddenThemeNames =
    [
        "MD3", "MD2", "FUI2", "Material Design", "Material You", "Fluent UI", "FluentUI", "Fluent2",
        "Cupertino", "Roboto", "Segoe",
        "md3-", "md2-", "fui2-",
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
