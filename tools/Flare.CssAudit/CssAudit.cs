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
    /// <summary>
    /// <c>[L]</c> names a component writes as text: a name the registry owns, spelled out anyway, or a
    /// class attribute holding a bare <c>flare-*</c> name - which is what a name assembled from a stem
    /// looks like. The other five reports cannot see either, because a name that is never written whole
    /// is a name this audit cannot read.
    /// </summary>
    public required IReadOnlyList<string> NamesSpelledOut { get; init; }

    /// <summary>True when CssClasses, Flare.Components CSS and the themes are fully in sync, no
    /// component CSS bakes a theme literal as a fallback on an always-emitted semantic token, no
    /// CSS class is declared by more than one constant, and every name comes from the registry.</summary>
    public bool InSync =>
        ClassesMissingConstant.Count == 0 && ConstantsMissingCss.Count == 0
        && ThemeOnlyClasses.Count == 0 && LiteralTokenFallbacks.Count == 0
        && DuplicateConstants.Count == 0 && NamesSpelledOut.Count == 0;

    /// <summary>All findings across the reports, for a single combined failure message.</summary>
    public IEnumerable<string> AllFindings =>
        ClassesMissingConstant.Concat(ConstantsMissingCss).Concat(ThemeOnlyClasses)
            .Concat(LiteralTokenFallbacks).Concat(DuplicateConstants).Concat(NamesSpelledOut);
}

/// <summary>
/// Outcome of a <c>--flare-*</c> token synchronization audit (the token counterpart of
/// <see cref="CssAuditReport"/>). Token usage and <c>Css.Tokens</c> are now fully reconciled, so this is
/// both printed by the <c>tokens</c> CLI verb and enforced as a build gate by
/// <c>CssAuditTests.CssTokens_Components_And_Themes_StayInSync</c>. Each list holds human-readable findings.
/// </summary>
public sealed class TokenAuditReport
{
    /// <summary><c>[T+]</c> tokens read in Flare.Components CSS with no <c>Css.Tokens</c> constant.</summary>
    public required IReadOnlyList<string> TokensMissingConstant { get; init; }
    /// <summary><c>[T-]</c> <c>Css.Tokens</c> constants referenced by no CSS at all (dead or aliased).</summary>
    public required IReadOnlyList<string> ConstantsMissingCss { get; init; }
    /// <summary><c>[T~]</c> tokens a theme references that are neither declared nor in the base CSS.</summary>
    public required IReadOnlyList<string> ThemeOnlyTokens { get; init; }

    /// <summary>True when CSS token usage and Css.Tokens fully agree (no drift in any direction).</summary>
    public bool InSync =>
        TokensMissingConstant.Count == 0 && ConstantsMissingCss.Count == 0 && ThemeOnlyTokens.Count == 0;

    /// <summary>All findings across the reports, for a single combined message.</summary>
    public IEnumerable<string> AllFindings =>
        TokensMissingConstant.Concat(ConstantsMissingCss).Concat(ThemeOnlyTokens);
}

/// <summary>
/// Outcome of the optional-package audit. Every package under <c>src/Flare.Components.*</c> ships its
/// own stylesheet and its own <c>Css/Classes</c> registry, so each is judged on its own terms: a class
/// the package styles has a constant somewhere (its own registry or the core one), a constant it
/// declares has a rule, and no name is spelled out where a constant belongs.
/// </summary>
public sealed class PackageAuditReport
{
    /// <summary>The packages examined, in path order.</summary>
    public required IReadOnlyList<string> Packages { get; init; }
    /// <summary><c>[+]</c> classes a package's CSS defines with no constant in its registry or the core one.</summary>
    public required IReadOnlyList<string> ClassesMissingConstant { get; init; }
    /// <summary><c>[-]</c> constants nothing styles AND nothing emits - a name that is simply dead.</summary>
    public required IReadOnlyList<string> ConstantsMissingCss { get; init; }
    /// <summary><c>[L]</c> names written as text in a package's components instead of coming from a registry.</summary>
    public required IReadOnlyList<string> NamesSpelledOut { get; init; }
    /// <summary>
    /// <c>[h]</c> classes a component emits that no rule styles. Reported, never failed: on an element
    /// whose layout its parent owns, such a class is the hook a theme needs in order to reach it, and a
    /// package stylesheet is thin on purpose. Worth reading when a variant looks like it does nothing.
    /// </summary>
    public required IReadOnlyList<string> StylingHooks { get; init; }

    /// <summary>True when every package's CSS, registry and markup agree.</summary>
    public bool InSync =>
        ClassesMissingConstant.Count == 0 && ConstantsMissingCss.Count == 0 && NamesSpelledOut.Count == 0;

    /// <summary>All findings across the reports, for a single combined failure message.</summary>
    public IEnumerable<string> AllFindings =>
        ClassesMissingConstant.Concat(ConstantsMissingCss).Concat(NamesSpelledOut);
}

/// <summary>
/// Programmatic entry point for the CssClasses/CSS/theme sync check. The CLI <c>check</c> command
/// and the test suite both run through <see cref="Run"/> so they can never disagree.
/// </summary>
public static class CssAudit
{
    // Token families whose NAMES are generated at runtime from a prefix rather than declared as a 1:1
    // const, so a CSS reference such as --flare-typescale-body-large-font is "declared" via its
    // generator (Typography.Font/Weight/Size/Height/Spacing), not via a literal constant.
    internal static readonly string[] KnownTokenPrefixes = { "--flare-typescale" };

    /// <summary>
    /// Runs the token audit (<c>--flare-*</c> CSS usage vs <c>Css.Tokens</c> constants) against the
    /// repository source tree. Report-only: the caller decides what to do with the findings; nothing
    /// here fails a build.
    /// </summary>
    /// <param name="repoRoot">Repo root; located by walking up when null (see <see cref="Run"/>).</param>
    public static TokenAuditReport RunTokens(string? repoRoot = null)
    {
        var root = repoRoot ?? Program.FindRepoRoot()
            ?? throw new DirectoryNotFoundException(
                "Could not locate the repo root (a folder containing src/Flare.Components).");

        var cssDir = Path.Combine(root, "src", "Flare.Components", "wwwroot", "css");
        var tokensDir = Path.Combine(root, "src", "Flare.Abstractions", "Css", "Tokens");
        var themeDirs = Directory.GetDirectories(Path.Combine(root, "src"), "Flare.Theme.*")
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .ToArray();

        var css = Program.CollectCssTokens(cssDir);
        Program.AddComponentCodeTokens(css, Path.Combine(root, "src", "Flare.Components"));
        // Core token constants + each theme's OWN private token constants (theme tokens live in the theme
        // project, not the core - so a theme-private token declared there is not reported as undeclared).
        var themeProjectDirs = Directory.GetDirectories(Path.Combine(root, "src"), "Flare.Theme.*");
        var constants = Program.CollectTokenConstants(new[] { tokensDir }.Concat(themeProjectDirs).ToArray());
        // A component that reads a token through its Css.Tokens CONSTANT (the practice the conventions ask
        // for) leaves no token string in its source, so the two scans above cannot see the usage. Resolve
        // the constant references too, plus the ones a token class consumes through its own accessor.
        Program.AddConstantReferenceTokens(css, constants, Path.Combine(root, "src", "Flare.Components"));
        Program.AddIntraClassTokenReferences(css, tokensDir);
        var themeCss = Program.CollectCssTokens(themeDirs);
        // Theme-private tokens are often defined + read entirely in the theme's C# (its extra-var dict and
        // DesignTokens value strings), never in a .css file - count those references so they are not [T-].
        foreach (var td in themeProjectDirs) Program.AddComponentCodeTokens(themeCss, td);

        var (plus, minus, tilde) = Program.CompareTokens(css, constants, themeCss, KnownTokenPrefixes);

        return new TokenAuditReport
        {
            TokensMissingConstant = plus.Select(t => $"[T+] {t}  (in {string.Join(", ", css[t])})").ToList(),
            ConstantsMissingCss = minus.Select(v => $"[T-] {v}  ({constants.LocationOf(v)})").ToList(),
            ThemeOnlyTokens = tilde.Select(t => $"[T~] {t}  (in {string.Join(", ", themeCss[t])})").ToList(),
        };
    }

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
            NamesSpelledOut = Program.ScanSpelledOutNames(
                Path.Combine(root, "src", "Flare.Components"), "Flare.Components", constants),
        };
    }

    /// <summary>
    /// Runs the audit over every optional component package (<c>src/Flare.Components.*</c>). The core
    /// audit cannot see these: it reads one stylesheet folder and one registry, while each package has
    /// its own pair. Without this pass a package could style a class nobody emits, emit a class nobody
    /// styles, or spell a name out - and nothing would say so.
    /// </summary>
    /// <param name="repoRoot">Repo root; located by walking up when null (see <see cref="Run"/>).</param>
    /// <exception cref="DirectoryNotFoundException">The repo root could not be located.</exception>
    public static PackageAuditReport RunPackages(string? repoRoot = null)
    {
        var root = repoRoot ?? Program.FindRepoRoot()
            ?? throw new DirectoryNotFoundException(
                "Could not locate the repo root (a folder containing src/Flare.Components).");

        // A package legitimately uses core classes (a button inside a carousel) and core CSS
        // legitimately has no opinion about a package class, so both directions consult the core.
        var coreCss = Program.CollectCssClasses(Path.Combine(root, "src", "Flare.Components", "wwwroot", "css"));
        var coreConstants = Program.CollectConstants(Path.Combine(root, "src", "Flare.Abstractions", "Css", "Classes"));

        var packages = new List<string>();
        var plus = new List<string>();
        var minus = new List<string>();
        var spelledOut = new List<string>();
        var hooks = new List<string>();

        foreach (var dir in Program.PackageDirs(root))
        {
            var name = Path.GetFileName(dir);
            packages.Add(name);

            var css = Program.CollectCssClasses(Path.Combine(dir, "wwwroot", "css"));
            var classesDir = Path.Combine(dir, "Css", "Classes");
            var constants = Directory.Exists(classesDir) ? Program.CollectConstants(classesDir) : new ConstSet();

            foreach (var cls in css.Keys.Where(c => !constants.Values.Contains(c) && !coreConstants.Values.Contains(c)))
                plus.Add($"[+] {name}: {cls}  (in {string.Join(", ", css[cls])}, no constant)");

            // A constant with no rule is only dead if nothing emits it either; while a component puts it
            // on an element it is a hook, which is how a theme reaches an element the base CSS leaves alone.
            var emitted = Program.CollectClassReferences(dir);
            foreach (var v in constants.Values
                         .Where(v => !css.ContainsKey(v) && !coreCss.ContainsKey(v))
                         .OrderBy(v => v, StringComparer.Ordinal))
            {
                var live = constants.Declarations()
                    .Any(d => d.Value == v && emitted.Contains($"{d.Owner}.{d.Field}"));
                if (live)
                    hooks.Add($"[h] {name}: {v}  ({constants.LocationOf(v)}, emitted, no rule)");
                else
                    minus.Add($"[-] {name}: {v}  ({constants.LocationOf(v)}, nothing styles or emits it)");
            }

            spelledOut.AddRange(Program.ScanSpelledOutNames(dir, name, coreConstants, constants));
        }

        return new PackageAuditReport
        {
            Packages = packages,
            ClassesMissingConstant = plus,
            ConstantsMissingCss = minus,
            NamesSpelledOut = spelledOut,
            StylingHooks = hooks,
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
