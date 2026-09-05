using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// A CSS name spelled out in a test is a name nothing checks. It survives a rename in the registry -
/// the test then looks for a class the library no longer emits and quietly finds nothing - and it can
/// name a class that never existed, which is how an absence assertion passes forever.
///
/// So a test names what the registry already owns THROUGH the registry: <c>$".{Css.Classes.X.Y}"</c>.
/// The compiler then refuses a name that is gone, and `Flare.CssAudit` proves the name exists in the
/// stylesheets - the same argument that took the literals out of the components.
///
/// Two things stay literal on purpose. A name the registry does NOT own is not covered here at all,
/// which is what an assertion like "this class must never be emitted" needs - there is no constant for
/// a class that must not exist. And verbatim strings are skipped: they are the regexes the stylesheet
/// guards match with, where a name is a pattern rather than a name.
/// </summary>
public class TestsNameCssFromTheRegistryTests
{
    [Fact]
    public void NoTestSpellsOutANameTheRegistryOwns()
    {
        var known = RegistryNames();
        Assert.True(known.Count > 500, "The registry scan found almost nothing, so this guard proves nothing.");

        var pattern = new Regex(
            "(" + string.Join("|", known.OrderByDescending(n => n.Length).Select(Regex.Escape)) + ")(?![a-z0-9-])",
            RegexOptions.Compiled);

        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(TestsDir, "*.cs", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                || file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                continue;

            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
                foreach (var literal in PlainStringLiterals(lines[i]))
                    foreach (Match m in pattern.Matches(literal))
                        offenders.Add($"{Path.GetFileName(file)}:{i + 1} \"{m.Value}\"");
        }

        Assert.True(offenders.Count == 0,
            "These tests spell out a CSS name the registry already owns; use the constant so a rename "
            + "cannot leave the test looking for something nobody emits any more: "
            + string.Join(", ", offenders.Take(20))
            + (offenders.Count > 20 ? $" (+{offenders.Count - 20} more)" : ""));
    }

    // Every "..." on the line, minus the verbatim ones. Interpolated strings are included: their holes
    // are code, and a name inside one is still a name spelled out.
    private static IEnumerable<string> PlainStringLiterals(string line)
    {
        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] != '"') continue;
            var verbatim = i > 0 && (line[i - 1] == '@' || (i > 1 && line[i - 1] == '$' && line[i - 2] == '@'));
            var end = -1;
            for (var k = i + 1; k < line.Length; k++)
            {
                if (!verbatim && line[k] == '\\') { k++; continue; }
                if (line[k] != '"') continue;
                if (verbatim && k + 1 < line.Length && line[k + 1] == '"') { k++; continue; }
                end = k;
                break;
            }
            if (end < 0) yield break;
            if (!verbatim) yield return line[(i + 1)..end];
            i = end;
        }
    }

    private static List<string> RegistryNames()
    {
        var names = new List<string>();
        var dir = Path.Combine(FindRepoRoot(), "src", "Flare.Abstractions", "Css");
        foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
            foreach (Match m in Regex.Matches(File.ReadAllText(file), @"public const string \w+\s*=\s*""([^""]+)"""))
                names.Add(m.Groups[1].Value);
        return names;
    }

    private static string TestsDir => Path.Combine(FindRepoRoot(), "tests");

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Abstractions")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from " + AppContext.BaseDirectory);
    }
}
