using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// Every Gallery component file must be reachable: either it is a routable page, or something names it.
/// A demo nobody references still compiles and still passes every test, so it rots unseen - eight
/// DataGrid demos had drifted that way, four of them written against parameters that no longer existed,
/// including three export demos that would have rendered no export control at all. The Gallery is the
/// documentation, so a demo that renders nowhere is a documentation gap that looks like coverage.
/// </summary>
public sealed class GalleryDeadMarkupTests
{
    [Fact]
    public void EveryGalleryComponentIsReachable()
    {
        var gallery = Path.Combine(FindRepoRoot(), "samples", "Flare.Gallery");
        var files = Directory
            .EnumerateFiles(gallery, "*.*", SearchOption.AllDirectories)
            .Where(f => Path.GetExtension(f) is ".razor" or ".cs")
            .Where(f => !IsBuildArtifact(f))
            .ToList();

        var sources = files.ToDictionary(f => f, File.ReadAllText);
        var orphans = new List<string>();

        foreach (var (file, text) in sources)
        {
            if (Path.GetExtension(file) != ".razor") continue;
            // A routable page needs no referrer, and the layout is wired up by Blazor itself.
            if (text.Contains("@page ", StringComparison.Ordinal)) continue;
            if (file.Contains($"{Path.DirectorySeparatorChar}Layout{Path.DirectorySeparatorChar}", StringComparison.Ordinal)) continue;

            var name = Path.GetFileNameWithoutExtension(file);
            var mentioned = sources.Any(other =>
                other.Key != file && Regex.IsMatch(other.Value, $@"\b{Regex.Escape(name)}\b"));

            if (!mentioned) orphans.Add(Path.GetRelativePath(gallery, file));
        }

        Assert.True(orphans.Count == 0,
            "Gallery components nothing references - they render nowhere:\n  " + string.Join("\n  ", orphans));
    }

    private static bool IsBuildArtifact(string path) =>
        path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
        || path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal);

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
