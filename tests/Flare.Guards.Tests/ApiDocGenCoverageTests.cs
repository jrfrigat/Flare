using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// The generated API reference covers exactly the assemblies that sit next to the generator, so an
/// add-on package the Gallery shows but the generator does not reference is documented nowhere: the
/// components render in the Gallery, the API tab has no page for them, and nothing fails. Adding
/// <c>Flare.Components.Barcode</c> to the Gallery and forgetting the generator did precisely that.
/// </summary>
public sealed class ApiDocGenCoverageTests
{
    [Fact]
    public void TheGeneratorReferencesEveryAddOnPackageTheGalleryShows()
    {
        var root = FindRepoRoot();
        var gallery = AddOnReferences(Path.Combine(root, "samples", "Flare.Gallery", "Flare.Gallery.csproj"));
        var generator = AddOnReferences(Path.Combine(root, "tools", "Flare.ApiDocGen", "Flare.ApiDocGen.csproj"));

        Assert.NotEmpty(gallery);

        var missing = gallery.Except(generator, StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
        Assert.True(missing.Count == 0,
            "tools/Flare.ApiDocGen/Flare.ApiDocGen.csproj does not reference: " + string.Join(", ", missing) +
            ". Without the reference the package's assembly and XML docs never reach the probe directory, " +
            "so its components are missing from ComponentApiRegistry.g.cs.");
    }

    // Component add-ons only. Themes and icon packs are deliberately outside the registry, and the
    // Gallery also references generators and the umbrella package, which the generator gets anyway.
    private static HashSet<string> AddOnReferences(string csprojPath)
    {
        var text = File.ReadAllText(csprojPath);
        var matches = Regex.Matches(text, @"Include=""[^""]*[\\/](Flare\.Components\.[A-Za-z0-9]+)\.csproj""");
        return matches.Select(m => m.Groups[1].Value).ToHashSet(StringComparer.Ordinal);
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from " + AppContext.BaseDirectory);
    }
}
