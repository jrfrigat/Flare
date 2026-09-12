using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// Anything positioned along the writing direction has to use logical properties. A physical
/// <c>right</c> or <c>margin-right</c> puts a trailing affordance - the progress stop dot, the space
/// reserved for it - at the wrong end of the track under RTL, where the bar finishes on the left.
/// </summary>
public sealed class LogicalDirectionTests
{
    // Files whose inline-direction geometry is load-bearing. Kept explicit rather than repo-wide:
    // a physical offset is legitimate in plenty of places (a decorative corner, an LTR-only glyph),
    // so this guard covers the components whose ends carry meaning.
    private static readonly string[] Guarded = ["progress.css", "meter.css", "slider.css"];

    private static readonly string[] Physical =
    [
        "margin-right", "margin-left", "padding-right", "padding-left",
        "border-right", "border-left",
    ];

    [Fact]
    public void GuardedComponentsUseLogicalInlineProperties()
    {
        var offenders = new List<string>();

        foreach (var file in GuardedFiles())
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var code = lines[i].Split("/*")[0];

                foreach (var property in Physical)
                    if (Regex.IsMatch(code, $@"(^|[;{{\s]){Regex.Escape(property)}\s*:"))
                        offenders.Add($"{Rel(file)}:{i + 1}: {property} - use its inline-* form");

                // Bare `right:`/`left:` offsets, but not `inset-inline-end`, `text-align: right`
                // or a `background-position: right` keyword.
                foreach (Match m in Regex.Matches(code, @"(^|[;{\s])(right|left)\s*:"))
                    offenders.Add($"{Rel(file)}:{i + 1}: {m.Groups[2].Value}: - use inset-inline-start/end");
            }
        }

        Assert.True(offenders.Count == 0, string.Join("\n", offenders));
    }

    private static IEnumerable<string> GuardedFiles()
    {
        var root = FindRepoRoot();
        var dirs = new List<string> { Path.Combine(root, "src", "Flare.Components", "wwwroot", "css") };
        dirs.AddRange(Directory.GetDirectories(Path.Combine(root, "src"), "Flare.Theme.*")
            .Select(d => Path.Combine(d, "wwwroot", "css", "components"))
            .Where(Directory.Exists));

        return dirs.Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.css", SearchOption.AllDirectories))
            .Where(f => Guarded.Contains(Path.GetFileName(f), StringComparer.Ordinal))
            .OrderBy(p => p, StringComparer.Ordinal);
    }

    private static string Rel(string path) =>
        Path.GetRelativePath(FindRepoRoot(), path).Replace('\\', '/');

    private static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src", "Flare.Components")))
            dir = Path.GetDirectoryName(dir);
        return dir ?? throw new DirectoryNotFoundException("Could not locate the repo root.");
    }
}
