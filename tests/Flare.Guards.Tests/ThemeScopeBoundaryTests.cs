using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// A theme stylesheet wrapped in <c>@scope</c> with no lower boundary keeps applying all the way
/// down, so a <c>FlareThemeScope</c> of another theme nested inside it inherits the outer theme's
/// component rules. Every theme sheet has to stop at the first nested root that is not its own.
/// </summary>
public sealed class ThemeScopeBoundaryTests
{
    [Fact]
    public void EveryThemeScopeStopsAtAForeignRoot()
    {
        var offenders = new List<string>();

        foreach (var file in ThemeCssFiles())
        {
            var css = File.ReadAllText(file);
            foreach (Match at in Regex.Matches(css, @"@scope\s*\(([^)]*)\)\s*(to\s*\(([^)]*)\))?"))
            {
                var roots = at.Groups[1].Value.Trim();
                var limit = at.Groups[3].Success ? at.Groups[3].Value.Trim() : null;

                if (limit is null)
                {
                    offenders.Add($"{Rel(file)}: @scope ({roots}) has no 'to (...)' boundary");
                    continue;
                }

                // The boundary has to be "any nested root that is not this theme", not just any root:
                // nesting the same theme must keep working, since it opens a scope of its own.
                if (!limit.Contains($".{Css.Classes.Theme.Root}", StringComparison.Ordinal) ||
                    !limit.Contains(":not(", StringComparison.Ordinal))
                    offenders.Add($"{Rel(file)}: boundary 'to ({limit})' does not exempt a nested same-theme root");
            }
        }

        Assert.True(offenders.Count == 0, string.Join("\n", offenders));
    }

    /// <summary>
    /// The scoping root is the theme's own class. A sheet that names a second, unrelated root - an
    /// opt-in component class, say - applies under every theme once that sheet is in the document,
    /// which makes what a page renders depend on which themes were visited earlier.
    /// </summary>
    [Fact]
    public void AThemeSheetScopesOnItsOwnThemeClassOnly()
    {
        var offenders = new List<string>();

        foreach (var file in ThemeCssFiles())
            foreach (Match at in Regex.Matches(File.ReadAllText(file), @"@scope\s*\(([^)]*)\)"))
                foreach (var root in at.Groups[1].Value.Split(',', StringSplitOptions.TrimEntries))
                    if (!root.StartsWith($".{Css.Classes.Theme.ThemePrefix}", StringComparison.Ordinal))
                        offenders.Add($"{Rel(file)}: @scope root '{root}' is not a theme class");

        Assert.True(offenders.Count == 0, string.Join("\n", offenders));
    }

    private static IEnumerable<string> ThemeCssFiles() =>
        Directory.GetDirectories(Path.Combine(FindRepoRoot(), "src"), "Flare.Theme.*")
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.css", SearchOption.AllDirectories))
            .OrderBy(p => p, StringComparer.Ordinal);

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
