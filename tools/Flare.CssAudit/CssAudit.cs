using System.Text.RegularExpressions;

namespace Flare.Tools.CssAudit;

/// <summary>
/// Outcome of a CSS / <c>CssClasses</c> / theme synchronization audit. <see cref="InSync"/> is
/// <c>true</c> only when all reports are empty. Each list holds human-readable findings
/// (the same lines the CLI prints), so a failing test can dump them verbatim.
/// </summary>
public sealed class CssAuditReport
{
    /// <summary><c>[+]</c> classes present in Flare.Components CSS but with no <c>CssClasses</c> constant.</summary>
    public required IReadOnlyList<string> ClassesMissingConstant { get; init; }
    /// <summary><c>[-]</c> <c>CssClasses</c> constants with no matching Flare.Components CSS rule.</summary>
    public required IReadOnlyList<string> ConstantsMissingCss { get; init; }
    /// <summary><c>[~]</c> classes a theme defines that the Flare.Components base does not.</summary>
    public required IReadOnlyList<string> ThemeOnlyClasses { get; init; }
    /// <summary><c>[!]</c> dead literal fallbacks on always-emitted semantic tokens (see <see cref="CssAudit"/>).</summary>
    public required IReadOnlyList<string> LiteralTokenFallbacks { get; init; }
    /// <summary><c>[=]</c> CSS-class values declared by more than one <c>CssClasses</c> constant (redundant duplicates to consolidate).</summary>
    public required IReadOnlyList<string> DuplicateConstants { get; init; }

    /// <summary>True when CssClasses, Flare.Components CSS and the themes are fully in sync, no
    /// component CSS bakes a theme literal as a fallback on an always-emitted semantic token, and no
    /// CSS class is declared by more than one constant.</summary>
    public bool InSync =>
        ClassesMissingConstant.Count == 0 && ConstantsMissingCss.Count == 0
        && ThemeOnlyClasses.Count == 0 && LiteralTokenFallbacks.Count == 0
        && DuplicateConstants.Count == 0;

    /// <summary>All findings across the reports, for a single combined failure message.</summary>
    public IEnumerable<string> AllFindings =>
        ClassesMissingConstant.Concat(ConstantsMissingCss).Concat(ThemeOnlyClasses)
            .Concat(LiteralTokenFallbacks).Concat(DuplicateConstants);
}

/// <summary>
/// Programmatic entry point for the CssClasses/CSS/theme sync check. The CLI <c>check</c> command
/// and the test suite both run through <see cref="Run"/> so they can never disagree.
/// </summary>
public static class CssAudit
{
    /// <summary>
    /// Runs the audit against the repository source tree.
    /// </summary>
    /// <param name="repoRoot">
    /// Repository root (the folder containing <c>src/Flare.Components</c>). When null it is located
    /// by walking up from the current and base directories.
    /// </param>
    /// <exception cref="DirectoryNotFoundException">The repo root could not be located.</exception>
    public static CssAuditReport Run(string? repoRoot = null)
    {
        var root = repoRoot ?? Program.FindRepoRoot()
            ?? throw new DirectoryNotFoundException(
                "Could not locate the repo root (a folder containing src/Flare.Components).");

        var cssDir = Path.Combine(root, "src", "Flare.Components", "wwwroot", "css");
        var cssClassesDir = Path.Combine(root, "src", "Flare.Abstractions", "Css", "Classes");
        var themeDirs = Directory.GetDirectories(Path.Combine(root, "src"), "Flare.Theme.*")
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .ToArray();

        var css = Program.CollectCssClasses(cssDir);
        var constants = Program.CollectConstants(cssClassesDir);
        var themeCss = Program.CollectCssClasses(themeDirs);

        var (plus, minus, tilde) = Program.Compare(css, constants, themeCss);

        return new CssAuditReport
        {
            ClassesMissingConstant = plus.Select(c => $"[+] {c}  (in {string.Join(", ", css[c])})").ToList(),
            ConstantsMissingCss = minus.Select(v => $"[-] {v}  ({constants.LocationOf(v)})").ToList(),
            ThemeOnlyClasses = tilde.Select(c => $"[~] {c}  (in {string.Join(", ", themeCss[c])})").ToList(),
            LiteralTokenFallbacks = ScanLiteralTokenFallbacks(cssDir),
            DuplicateConstants = constants.Duplicates.Select(d => $"[=] {d}").ToList(),
        };
    }

    // Matches a var() on an always-emitted SEMANTIC token family (color/shape/spacing/typescale/
    // motion/state/elevation) whose fallback STARTS WITH A THEME LITERAL: a hex color, a number
    // (dimension / opacity / box-shadow offset), or a color function (rgb/hsl/.../cubic-bezier). The
    // atomic (?>\s*) stops whitespace backtracking from defeating the lookahead. This deliberately
    // does NOT flag: semantic->semantic chains (fallback starts with var(), theme-agnostic keyword or
    // currentColor/color-mix fallbacks, or component-family tokens (--flare-<component>-*) whose
    // fallback is the intentional "initial"-sentinel mechanism.
    private static readonly Regex LiteralFallbackRx = new(
        @"var\(--flare-(?:color|shape|spacing|typescale|motion|state|elevation)-[a-z0-9-]+,(?>\s*)(?=#|[0-9]|rgb|hsl|hwb|lab|lch|oklab|oklch|cubic-bezier)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Flags dead literal fallbacks on always-emitted semantic tokens in the Flare.Components CSS
    /// bundle. Because every <c>--flare-color/shape/spacing/typescale/motion/state/elevation-*</c>
    /// token is <c>required</c> and always emitted, such a fallback never applies (and can even wrongly
    /// override a theme that sets the token to a flat value like <c>none</c>). This keeps the M2 "no
    /// literal CSS fallbacks" mandate enforced and prevents regressions like a decimal opacity token
    /// being pasted into a percentage context via its literal fallback.
    /// </summary>
    internal static IReadOnlyList<string> ScanLiteralTokenFallbacks(string cssDir)
    {
        var findings = new List<string>();
        if (!Directory.Exists(cssDir)) return findings;
        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f, StringComparer.Ordinal))
        {
            var name = Path.GetFileName(file);
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
                if (LiteralFallbackRx.IsMatch(lines[i]))
                    findings.Add($"[!] literal token fallback  ({name}:{i + 1})  {lines[i].Trim()}");
        }
        return findings;
    }
}
