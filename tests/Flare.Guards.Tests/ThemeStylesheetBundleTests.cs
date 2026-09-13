using System.Reflection;
using Flare.Abstractions;

namespace Flare.Guards.Tests;

/// <summary>
/// An application that writes a theme's links into its own head (<c>ThemeStylesheets.Manual</c>) repeats
/// the theme's <c>StyleAssets</c> by hand, so an in-box theme ships its own CSS as exactly one stylesheet
/// at one predictable address: <c>_content/Flare.Theme.Name/css/components.css</c>, which the build
/// concatenates from <c>components.imports.css</c>. A second local entry would put every such app one
/// link short without any error.
/// </summary>
public sealed class ThemeStylesheetBundleTests
{
    public static IEnumerable<object[]> ThemePackages() =>
        Directory.GetDirectories(Path.Combine(FindRepoRoot(), "src"), "Flare.Theme.*")
            .Select(Path.GetFileName)
            .Where(name => !name!.EndsWith(".Tokens", StringComparison.Ordinal))
            .OrderBy(name => name, StringComparer.Ordinal)
            .Select(name => new object[] { name! });

    [Theory]
    [MemberData(nameof(ThemePackages))]
    public void AnInBoxThemeShipsItsOwnCssAsOneBundledStylesheet(string package)
    {
        var projectDir = Path.Combine(FindRepoRoot(), "src", package);
        var themes = Assembly.Load(new AssemblyName(package)).GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true }
                        && typeof(ITheme).IsAssignableFrom(t)
                        && t.GetConstructor(Type.EmptyTypes) is not null)
            .Select(t => (ITheme)Activator.CreateInstance(t)!)
            .ToList();
        Assert.NotEmpty(themes);

        var hasCss = Directory.Exists(Path.Combine(projectDir, "wwwroot", "css"));
        var bundle = $"_content/{package}/css/components.css";

        foreach (var theme in themes)
        {
            var local = theme.StyleAssets.Where(a => a.StartsWith("_content/", StringComparison.Ordinal)).ToList();
            if (hasCss)
                Assert.True(local.Count == 1 && local[0] == bundle,
                    $"{theme.GetType().Name}.StyleAssets must name its own CSS once, as {bundle}; it lists: "
                    + string.Join(", ", local));
            else
                Assert.True(local.Count == 0,
                    $"{theme.GetType().Name}.StyleAssets names local stylesheets, but {package} has no wwwroot/css: "
                    + string.Join(", ", local));
        }

        if (!hasCss) return;
        Assert.True(File.Exists(Path.Combine(projectDir, "components.imports.css")),
            $"{package} has CSS but no components.imports.css, so nothing builds {bundle}.");
        Assert.True(File.Exists(Path.Combine(projectDir, "wwwroot", "css", "components.css")),
            $"{package} did not produce wwwroot/css/components.css: set <FlareCssBundle>components</FlareCssBundle> "
            + "in its project file.");
    }

    private static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src", "Flare.Components")))
            dir = Path.GetDirectoryName(dir);
        return dir ?? throw new DirectoryNotFoundException("Could not locate the repo root.");
    }
}
