using System.Text.RegularExpressions;
using Flare.Abstractions;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign2;
using Flare.Theme.MaterialDesign3;
using Flare.Theme.MaterialDesign3Expressive;
using Flare.Theme.VisualStudio;
using Flare.Theming;

namespace Flare.Guards.Tests;

/// <summary>
/// A token value may name another theme variable - <c>2px solid var(--flare-fluent-focus-stroke-color)</c> -
/// and when that variable is not defined the reference is "guaranteed-invalid": every declaration that
/// substitutes the token is dropped at computed-value time, silently. Visual Studio inherits its checkbox,
/// radio and menu focus rings from Fluent 2 but supplies its own <c>Extended</c> map, which did not carry the
/// Fluent focus-stroke variables those rings name, so keyboard focus drew nothing on those controls.
///
/// The check reads the theme as the runtime injects it (design tokens, the colours of its default palette, and
/// the dark-mode extras) and only follows references without a fallback: <c>var(--x, #F5F5F5)</c> is safe by
/// construction. Proved on 2026-09-13 against the Visual Studio defect above before it was fixed.
/// </summary>
public sealed class TokenReferenceTests
{
    private static readonly Func<ITheme>[] Factories =
    [
        () => new AeroTheme(),
        () => new FluentUI2Theme(),
        () => new LiquidGlassTheme(),
        () => new MaterialDesign2Theme(),
        () => new MaterialDesign3Theme(),
        () => new MaterialDesign3ExpressiveTheme(),
        () => new VisualStudioTheme(),
    ];

    public static TheoryData<int> ThemeIndexes()
    {
        var d = new TheoryData<int>();
        for (var i = 0; i < Factories.Length; i++) d.Add(i);
        return d;
    }

    // var(--flare-name) closed straight away: no fallback to fall back on.
    private static readonly Regex _bareReference = new(@"var\(\s*(--flare-[a-z0-9-]+)\s*\)", RegexOptions.Compiled);

    [Theory]
    [MemberData(nameof(ThemeIndexes))]
    public void EveryVariableATokenNamesWithoutAFallbackIsDefined(int index)
    {
        var theme = Factories[index]();
        var palette = theme.Palettes.First(p => p.Id == theme.DefaultPaletteId);

        var light = theme.Design.Flatten(palette.Light);
        var dark = theme.Design.Flatten(palette.Dark);
        if (theme.ExtendedDarkOverride is { } extras)
            foreach (var (k, v) in extras) dark[k] = v;

        var offenders = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var (mode, vars) in new[] { ("light", light), ("dark", dark) })
            foreach (var (name, value) in vars)
                foreach (Match m in _bareReference.Matches(value ?? string.Empty))
                    if (!vars.ContainsKey(m.Groups[1].Value))
                        offenders.Add($"{name} -> {m.Groups[1].Value} ({mode})");

        Assert.True(offenders.Count == 0,
            $"The '{theme.Id}' theme has tokens naming a variable it never defines. Each declaration that uses "
            + "such a token is dropped by the browser. Define the variable, or give the reference a fallback:\n  "
            + string.Join("\n  ", offenders));
    }

    // Generated bundles repeat their parts; the parts are what is read.
    private static readonly string[] _bundles = ["flare-components.css", "components.css"];

    private static readonly Regex _comment = new(@"/\*.*?\*/", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex _declared = new(@"(--flare-[a-z0-9-]+)\s*:", RegexOptions.Compiled);

    /// <summary>
    /// The same failure one level up: a component stylesheet naming a theme variable no theme defines.
    /// <c>--flare-motion-easing-emphasized-decelerate</c> was read by the chart, gauge and pull-to-refresh
    /// transitions in every theme and defined by none, so none of them ever animated. Variables a stylesheet
    /// declares itself, and the per-instance channels in <c>LocalVars</c> that a component writes on its own
    /// element, are not theme variables and are not checked. Proved on 2026-09-13 against that easing and
    /// the Query package's undefined focus-ring and spacing names before they were fixed.
    /// </summary>
    [Theory]
    [MemberData(nameof(ThemeIndexes))]
    public void EveryThemeVariableAComponentStylesheetNamesWithoutAFallbackIsDefined(int index)
    {
        var theme = Factories[index]();
        var palette = theme.Palettes.First(p => p.Id == theme.DefaultPaletteId);
        var vars = theme.Design.Flatten(palette.Light);

        var local = typeof(Flare.Css.Tokens.LocalVars)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(f => (string)f.GetValue(null)!)
            .ToHashSet(StringComparer.Ordinal);

        var sheets = Directory.EnumerateDirectories(Path.Combine(FindRepoRoot(), "src"), "Flare.Components*")
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.css", SearchOption.AllDirectories))
            .Where(f => !_bundles.Contains(Path.GetFileName(f), StringComparer.OrdinalIgnoreCase))
            .ToDictionary(f => f, f => _comment.Replace(File.ReadAllText(f), ""));

        Assert.True(sheets.Count > 50, "Found almost no component stylesheets - the scan is looking in the wrong place.");

        var declared = sheets.Values.SelectMany(css => _declared.Matches(css).Select(m => m.Groups[1].Value))
            .ToHashSet(StringComparer.Ordinal);

        var offenders = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var (file, css) in sheets)
            foreach (Match m in _bareReference.Matches(css))
            {
                var name = m.Groups[1].Value;
                if (vars.ContainsKey(name) || declared.Contains(name) || local.Contains(name)) continue;
                offenders.Add($"{name} in {Path.GetFileName(file)}");
            }

        Assert.True(offenders.Count == 0,
            $"Component stylesheets name variables the '{theme.Id}' theme never defines, so every declaration "
            + "reading them is dropped:\n  " + string.Join("\n  ", offenders));
    }

    /// <summary>
    /// A theme's own stylesheets, read against that theme alone - light and dark, since a dark-only extra
    /// is still defined when the rule runs in dark mode. No theme fails it today; proved on 2026-09-13 with a
    /// deliberately undefined name added to <c>aero-base.css</c>.
    /// </summary>
    [Theory]
    [MemberData(nameof(ThemeIndexes))]
    public void EveryVariableAThemeStylesheetNamesWithoutAFallbackIsDefined(int index)
    {
        var theme = Factories[index]();
        var cssDir = Path.Combine(FindRepoRoot(), "src", theme.GetType().Assembly.GetName().Name!, "wwwroot", "css");
        if (!Directory.Exists(cssDir)) return;

        var palette = theme.Palettes.First(p => p.Id == theme.DefaultPaletteId);
        var defined = theme.Design.Flatten(palette.Light).Keys.ToHashSet(StringComparer.Ordinal);
        defined.UnionWith(theme.Design.Flatten(palette.Dark).Keys);
        if (theme.ExtendedDarkOverride is { } extras) defined.UnionWith(extras.Keys);

        var sheets = Directory.EnumerateDirectories(Path.Combine(FindRepoRoot(), "src"), "Flare.Components*")
            .Select(d => Path.Combine(d, "wwwroot", "css"))
            .Append(cssDir)
            .Where(Directory.Exists)
            .SelectMany(d => Directory.EnumerateFiles(d, "*.css", SearchOption.AllDirectories))
            .Where(f => !_bundles.Contains(Path.GetFileName(f), StringComparer.OrdinalIgnoreCase))
            .ToDictionary(f => f, f => _comment.Replace(File.ReadAllText(f), ""));
        foreach (var css in sheets.Values)
            defined.UnionWith(_declared.Matches(css).Select(m => m.Groups[1].Value));

        var offenders = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var (file, css) in sheets.Where(s => s.Key.StartsWith(cssDir, StringComparison.OrdinalIgnoreCase)))
            foreach (Match m in _bareReference.Matches(css))
                if (!defined.Contains(m.Groups[1].Value))
                    offenders.Add($"{m.Groups[1].Value} in {Path.GetFileName(file)}");

        Assert.True(offenders.Count == 0,
            $"The '{theme.Id}' theme's stylesheets name variables it never defines, so every declaration "
            + "reading them is dropped:\n  " + string.Join("\n  ", offenders));
    }

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
        throw new InvalidOperationException("Could not locate the repository root.");
    }
}
