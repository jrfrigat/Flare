using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// A component that reads a design token by name must name it with the registry constant, never with a
/// string literal.
/// </summary>
/// <remarks>
/// This is the corner of the CSS-name problem where being wrong is completely silent. Everywhere else a
/// misspelled <c>--flare-*</c> shows up: a rule that does not apply leaves a visibly unstyled element.
/// A misspelled lookup key does not - <c>ReadTokenNum</c> returns the fallback, the component behaves
/// exactly as if the theme had not set the token, and the only symptom is a feature that quietly never
/// turns on. <c>FlareProgress</c> read all eight of its wave tokens by literal, so renaming any of them
/// in a theme would have silently disabled the wavy progress bar with nothing to show for it.
///
/// The broader pass over the remaining literals is docs/issues/css-name-literals.md. This guard covers
/// the part that cannot be caught by looking at the screen.
/// </remarks>
public sealed class TokenLookupKeyTests
{
    // ReadTokenNum("--flare-x", ...) / ReadTokenStr("--flare-x", ...)
    private static readonly Regex LiteralLookup =
        new(@"ReadToken(?:Num|Str)\s*\(\s*""", RegexOptions.Compiled);

    [Fact]
    public void NoComponent_ReadsATokenByStringLiteral()
    {
        var root = Path.Combine(FindRepoRoot(), "src", "Flare.Components");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
                     .Where(f => f.EndsWith(".cs", StringComparison.Ordinal) || f.EndsWith(".razor", StringComparison.Ordinal))
                     .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                     .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                     .OrderBy(f => f, StringComparer.Ordinal))
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (LiteralLookup.IsMatch(lines[i]))
                    offenders.Add($"{Path.GetFileName(file)}:{i + 1}  {lines[i].Trim()}");
            }
        }

        Assert.True(offenders.Count == 0,
            "A token read by name must use its Flare.Css.Tokens constant, not a literal. A literal that " +
            "stops matching the registry fails silently: the read returns its fallback and the feature " +
            "simply never switches on." + Environment.NewLine + string.Join(Environment.NewLine, offenders));
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
