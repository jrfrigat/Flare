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
}
