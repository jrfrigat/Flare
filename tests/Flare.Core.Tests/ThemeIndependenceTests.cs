using System.Reflection;
using Flare.Abstractions;
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
}
