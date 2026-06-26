namespace Flare.Tools.CssAudit;

/// <summary>
/// Outcome of a CSS / <c>CssClasses</c> / theme synchronization audit. <see cref="InSync"/> is
/// <c>true</c> only when all three reports are empty. Each list holds human-readable findings
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

    /// <summary>True when CssClasses, Flare.Components CSS and the themes are fully in sync.</summary>
    public bool InSync =>
        ClassesMissingConstant.Count == 0 && ConstantsMissingCss.Count == 0 && ThemeOnlyClasses.Count == 0;

    /// <summary>All findings across the three reports, for a single combined failure message.</summary>
    public IEnumerable<string> AllFindings =>
        ClassesMissingConstant.Concat(ConstantsMissingCss).Concat(ThemeOnlyClasses);
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
        };
    }
}
