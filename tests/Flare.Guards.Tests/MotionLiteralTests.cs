using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// Every transition and animation takes its pace and its curve from the motion tokens, so a theme sets the
/// character of movement in one place and <c>prefers-reduced-motion</c> reaches all of it through one rule.
/// A literal - <c>0.3s ease</c>, <c>150ms</c>, <c>linear</c> - is a timing the theme cannot change and the
/// reduced-motion rule cannot stop; 21 of them and 20 keyword curves had accumulated before this guard.
/// Zero is allowed (no delay, an instant flip), and so is anything inside <c>var()</c>, including a fallback.
/// Blocks under <c>@media (prefers-reduced-motion</c> are skipped: switching motion off there is their job.
/// Proved on 2026-09-25 against a deliberate <c>transition: color 150ms ease</c> in button.css.
/// </summary>
public sealed class MotionLiteralTests
{
    // Generated bundles repeat their parts; the parts are what is read.
    private static readonly string[] _bundles = ["flare-components.css", "components.css"];

    private static readonly Regex _comment = new(@"/\*.*?\*/", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex _reducedMotion = new(@"@media\s*\(\s*prefers-reduced-motion[^{]*\{", RegexOptions.Compiled);
    private static readonly Regex _innermostVar = new(@"var\([^()]*\)", RegexOptions.Compiled);
    private static readonly Regex _declaration = new(
        @"(?<![-\w])(?<prop>(transition|animation)(-duration|-delay|-timing-function)?)\s*:\s*(?<value>[^;{}]+)",
        RegexOptions.Compiled);
    private static readonly Regex _time = new(@"(?<![\w.-])(?<n>\d*\.?\d+)(ms|s)(?![\w-])", RegexOptions.Compiled);
    private static readonly Regex _curve = new(
        @"(?<![\w-])(ease|ease-in|ease-out|ease-in-out|linear)(?![\w-])|cubic-bezier\(|steps\(", RegexOptions.Compiled);

    [Fact]
    public void NoStylesheetTimesOrEasesMotionWithALiteral()
    {
        var src = Path.Combine(FindRepoRoot(), "src");
        var sheets = Directory.EnumerateDirectories(src, "Flare.Components*")
            .Concat(Directory.EnumerateDirectories(src, "Flare.Theme.*"))
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.css", SearchOption.AllDirectories))
            .Where(f => !_bundles.Contains(Path.GetFileName(f), StringComparer.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToList();

        Assert.True(sheets.Count > 50, "Found almost no stylesheets - the scan is looking in the wrong place.");

        var offenders = new List<string>();
        foreach (var file in sheets)
        {
            var css = WithoutReducedMotionBlocks(_comment.Replace(File.ReadAllText(file), ""));
            foreach (Match m in _declaration.Matches(css))
            {
                var value = m.Groups["value"].Value;
                while (_innermostVar.IsMatch(value)) value = _innermostVar.Replace(value, "");

                var literalTime = _time.Matches(value).Any(t => double.Parse(t.Groups["n"].Value,
                    System.Globalization.CultureInfo.InvariantCulture) != 0);
                if (literalTime || _curve.IsMatch(value))
                    offenders.Add($"{Path.GetRelativePath(src, file)}  {m.Groups["prop"].Value}: {m.Groups["value"].Value.Trim()}");
            }
        }

        Assert.True(offenders.Count == 0,
            "Motion timed or eased with a literal instead of a --flare-motion-* token, so no theme can change it "
            + "and prefers-reduced-motion cannot stop it:\n  " + string.Join("\n  ", offenders));
    }

    // Removes each @media (prefers-reduced-motion ...) { ... } block, braces balanced.
    private static string WithoutReducedMotionBlocks(string css)
    {
        for (var m = _reducedMotion.Match(css); m.Success; m = _reducedMotion.Match(css))
        {
            var depth = 1;
            var i = m.Index + m.Length;
            for (; i < css.Length && depth > 0; i++)
            {
                if (css[i] == '{') depth++;
                else if (css[i] == '}') depth--;
            }
            css = css.Remove(m.Index, i - m.Index);
        }
        return css;
    }

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
        throw new InvalidOperationException("Could not locate the repository root.");
    }
}
