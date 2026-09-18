using System.Reflection;
using System.Text.RegularExpressions;
using Flare.Abstractions;

namespace Flare.Guards.Tests;

/// <summary>
/// The stacking ladder is a contract about what may cover what, and a contract made of numbers spread
/// over twenty stylesheets is one nobody can check. It was not checkable before: the bottom navigation
/// bar was pinned at 1100 while the dialog scrim painted at 1000, so on a phone the actions of a
/// bottom-anchored dialog were drawn under the bar and could not be pressed - measured in a consuming
/// application at 375x812, which shipped a `z-index:999` override to get them back. A select's listbox
/// tied with that same scrim at 1000, where the winner is decided by document order rather than intent.
///
/// A theme owns where the ladder sits, so these guards hold the parts a theme must not get wrong: the
/// rungs stay in order with room between them, and no stylesheet goes back to naming a number instead of
/// asking for a rung.
/// </summary>
public class LayerLadderTests
{
    // In the order they must appear, lowest first. This is the ladder's whole meaning: a rung may only
    // be covered by the rungs after it in this list.
    private static readonly (string Name, Func<Flare.Abstractions.Tokens.LayerTokens, string> Value)[] Ladder =
    [
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Chrome), l => l.Chrome),
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Drawer), l => l.Drawer),
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Dropdown), l => l.Dropdown),
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Modal), l => l.Modal),
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Toast), l => l.Toast),
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Tooltip), l => l.Tooltip),
        (nameof(Flare.Abstractions.Tokens.LayerTokens.Drag), l => l.Drag),
    ];

    [Theory]
    [MemberData(nameof(Themes))]
    public void RungsAreStrictlyAscending(string themeId)
    {
        var ladder = LadderOf(themeId);

        for (var i = 1; i < Ladder.Length; i++)
        {
            Assert.True(ladder[i - 1].Value < ladder[i].Value,
                $"Theme '{themeId}' puts {Ladder[i].Name} at {ladder[i].Value}, which does not sit above "
                + $"{Ladder[i - 1].Name} at {ladder[i - 1].Value}. The order is what every component relies "
                + "on when it asks for a rung by name.");
        }
    }

    // Room for a component to lift one of its own parts over another - a drawer panel over its own scrim,
    // a submenu over its parent menu - with `calc(var(--rung) + n)` and without reaching the rung above.
    [Theory]
    [MemberData(nameof(Themes))]
    public void RungsLeaveRoomForAComponentsOwnParts(string themeId)
    {
        var ladder = LadderOf(themeId);
        var deepest = DeepestOffsetInUse();

        for (var i = 1; i < Ladder.Length; i++)
        {
            Assert.True(ladder[i].Value - ladder[i - 1].Value > deepest,
                $"Theme '{themeId}' leaves {ladder[i].Value - ladder[i - 1].Value} between {Ladder[i - 1].Name} "
                + $"and {Ladder[i].Name}, which is not more than the deepest offset a stylesheet already "
                + $"takes (+{deepest}), so a component's own upper part would reach the rung above.");
        }
    }

    // The point of the ladder is that nothing opts out of it. A detached surface - one taken out of the
    // flow with `position: fixed`, or an `absolute` panel escaping its parent - is exactly the kind that
    // collides across components. Local stacking inside a component's own context is fine as a number and
    // is not matched here: this looks only at values large enough to be reaching for an app-level layer.
    [Fact]
    public void NoStylesheetNamesAnAppLevelNumber()
    {
        var offenders = new List<string>();
        foreach (var path in Directory.EnumerateFiles(CssDir, "*.css"))
        {
            var name = Path.GetFileName(path);
            if (name == "flare-components.css") continue;   // generated from the parts below

            var css = StripComments(File.ReadAllText(path));
            foreach (Match m in Regex.Matches(css, @"z-index\s*:\s*(\d+)"))
            {
                if (int.Parse(m.Groups[1].Value) >= AppLevelFloor)
                    offenders.Add($"{name} (z-index: {m.Groups[1].Value})");
            }
        }

        Assert.True(offenders.Count == 0,
            "These stylesheets place a surface by number instead of asking the ladder for a rung: "
            + string.Join(", ", offenders)
            + ". A number here cannot be moved by a theme and cannot be reasoned about from any other file.");
    }

    // Below this a value is plainly local: the largest in-component ladder in the repository is the data
    // grid's frozen-column stack, which reaches 10.
    private const int AppLevelFloor = 50;

    public static IEnumerable<object[]> Themes() =>
        ThemeInstances().Select(t => new object[] { t.Id });

    private static List<ITheme> ThemeInstances()
    {
        // Touch one type per theme package so the assembly is loaded before it is reflected over.
        _ = new[]
        {
            typeof(Flare.Theme.Aero.AeroTheme),
            typeof(Flare.Theme.FluentUI2.FluentUI2Theme),
            typeof(Flare.Theme.LiquidGlass.LiquidGlassTheme),
            typeof(Flare.Theme.MaterialDesign2.MaterialDesign2Theme),
            typeof(Flare.Theme.MaterialDesign3.MaterialDesign3Theme),
            typeof(Flare.Theme.MaterialDesign3Expressive.MaterialDesign3ExpressiveTheme),
            typeof(Flare.Theme.VisualStudio.VisualStudioTheme),
        };

        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith("Flare.Theme.", StringComparison.Ordinal) == true)
            .SelectMany(SafeTypes)
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                && typeof(ITheme).IsAssignableFrom(t)
                && t.GetConstructor(Type.EmptyTypes) is not null)
            .Select(t => (ITheme)Activator.CreateInstance(t)!)
            .GroupBy(t => t.Id, StringComparer.Ordinal)
            .Select(g => g.First())
            .OrderBy(t => t.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static IEnumerable<Type> SafeTypes(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t is not null)!; }
    }

    private static (string Name, int Value)[] LadderOf(string themeId)
    {
        var theme = ThemeInstances().Single(t => t.Id == themeId);
        return Ladder.Select(rung =>
        {
            var raw = rung.Value(theme.Design.Layer);
            Assert.True(int.TryParse(raw, out var value),
                $"Theme '{themeId}' sets {rung.Name} to '{raw}', which is not a plain number - the ladder's "
                + "order cannot be checked, and neither can a component's `calc(var(--rung) + 1)`.");
            return (rung.Name, value);
        }).ToArray();
    }

    private static int DeepestOffsetInUse()
    {
        var deepest = 0;
        foreach (var path in Directory.EnumerateFiles(CssDir, "*.css"))
        {
            if (Path.GetFileName(path) == "flare-components.css") continue;
            var css = StripComments(File.ReadAllText(path));
            foreach (Match m in Regex.Matches(css, @"calc\(\s*var\(--flare-z-[a-z-]+\)\s*\+\s*(\d+)\s*\)"))
                deepest = Math.Max(deepest, int.Parse(m.Groups[1].Value));
        }
        return deepest;
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
