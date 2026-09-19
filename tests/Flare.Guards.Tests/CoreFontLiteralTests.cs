using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// Guards the font families: a core stylesheet names a token, never a family. Every run of prose already
/// took its family from a typescale step; the code-like runs did not, and four rules said
/// <c>font-family: monospace</c> outright - <c>FlareText</c> in mono, inline <c>FlareCode</c>, the code
/// block and code inside markdown.
/// </summary>
/// <remarks>
/// <c>monospace</c> looks like the absence of a choice, which is why it survived. It is not: it is the
/// user agent's choice, and it is the one a design language most often wants to overrule, because a code
/// face is a visible part of an engineering tool's identity. A theme that set JetBrains Mono in its
/// typography tokens watched every mono run ignore it and had to add a rule of its own against a core
/// class - the token mandate inverted, and a rule that breaks the moment core renames the class.
/// </remarks>
public sealed class CoreFontLiteralTests
{
    // font-family, or the family part of the `font` shorthand, set to anything that is not a var().
    private static readonly Regex LiteralFamily = new(
        @"font-family\s*:\s*(?<value>[^;}]+)", RegexOptions.Compiled);

    [Fact]
    public void NoCoreStylesheetNamesAFontFamily()
    {
        var offenders = new List<string>();
        foreach (var path in Directory.EnumerateFiles(CssDir, "*.css"))
        {
            var name = Path.GetFileName(path);
            if (name == "flare-components.css") continue;   // generated from the parts

            var css = StripComments(File.ReadAllText(path));
            foreach (Match m in LiteralFamily.Matches(css))
            {
                var value = m.Groups["value"].Value.Trim();
                // `inherit` takes whatever the host already resolved, so it names nothing.
                if (value.StartsWith("var(", StringComparison.Ordinal) || value == "inherit") continue;
                offenders.Add($"{name}: font-family: {value}");
            }
        }

        Assert.True(offenders.Count == 0,
            "These core rules name a font family instead of a token, so no theme can change the face:\n  "
            + string.Join("\n  ", offenders)
            + $"\nUse var({Css.Tokens.Typography.MonoFont}) for code-like runs, or a typescale step's font.");
    }

    private static string CssDir =>
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

    private static string StripComments(string css) =>
        Regex.Replace(css, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

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
