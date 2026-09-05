using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// Every floating panel in the library is positioned by the placement engine, which writes `position`,
/// `top` and `left` on the element - and nothing else. So a stylesheet rule that ALSO places the panel
/// is not overridden, it is combined with those coordinates, and the result is a box nobody asked for:
///
/// - `right` or `bottom` alongside the engine's `left`/`top` constrains a fixed box from both sides,
///   and it stretches between them instead of sitting at one;
/// - a `translate` transform shifts the panel away from the coordinates the engine measured, which is
///   how a "smart positioned" tooltip used to land half its own width to the left of its trigger.
///
/// Both are legitimate as the RESTING form - where the panel sits for the frame before it is placed,
/// and where it stays in a browser that never ran the script - so the rule is not "never declare these"
/// but "declare them only when the panel has not been placed". `[data-flare-placed]` is the engine's
/// mark, and `:not([data-flare-placed])` is how a resting rule steps aside.
///
/// Pseudo-element rules are exempt: an arrow drawn with `::before` is positioned inside the panel and
/// has nothing to do with where the panel is.
/// </summary>
public class FloatingPanelPlacementTests
{
    // Stylesheet, and the panel class in it that the placement engine owns.
    public static TheoryData<string, string> Panels => new()
    {
        { "popover.css", Css.Classes.Popover.Paper },
        { "tooltip.css", Css.Classes.Tooltip.Content },
        { "menu.css", Css.Classes.Menu.Panel },
        { "menu.css", Css.Classes.Menu.SubmenuPanel },
        { "datagrid.css", Css.Classes.DataGrid.FilterMenu },
        { "datagrid.css", Css.Classes.DataGrid.ColumnPicker },
        { "datagrid.css", Css.Classes.DataGrid.FilterBuilderPanel },
    };

    [Theory]
    [MemberData(nameof(Panels))]
    public void RestingPlacement_StepsAsideOnceTheEngineHasPlacedThePanel(string file, string panelClass)
    {
        var css = StripComments(File.ReadAllText(Path.Combine(CssDir, file)));

        foreach (var (selectors, body) in Rules(css))
        {
            if (!MentionsPanel(selectors, panelClass)) continue;
            if (selectors.Contains("::before") || selectors.Contains("::after")) continue;
            if (selectors.Contains("[data-flare-placed]")) continue;

            var placing = PlacingDeclaration(body);
            if (placing is null) continue;

            Assert.Fail(
                $"{file}: `{selectors.Trim()}` declares `{placing}`, which the placement engine cannot "
                + "override - it writes only position/top/left. Guard the rule with "
                + "`:not([data-flare-placed])` so it applies while the panel is at rest and steps aside "
                + "once the engine has placed it.");
        }
    }

    // The declarations the engine cannot undo: the two edges it never writes, and a transform that moves
    // the box away from the coordinates it did write. `transform: rotate(...)` and the like are fine.
    private static string? PlacingDeclaration(string body)
    {
        foreach (var decl in body.Split(';'))
        {
            var text = decl.Trim();
            if (text.Length == 0) continue;
            var name = text.Split(':')[0].Trim().ToLowerInvariant();
            if (name is "right" or "bottom" or "inset-inline-start" or "inset-inline-end"
                or "inset-block-start" or "inset-block-end" or "inset")
            {
                return text;
            }

            if (name == "transform" && text.Contains("translate", StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }
        }

        return null;
    }

    // The class as a whole word: `.flare-datagrid__filter-menu-list` is a different element from
    // `.flare-datagrid__filter-menu`, and a substring match would confuse the two.
    private static bool MentionsPanel(string selectors, string panelClass) =>
        Regex.IsMatch(selectors, Regex.Escape("." + panelClass) + @"(?![\w-])");

    private static string CssDir =>
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

    private static IEnumerable<(string Selectors, string Body)> Rules(string css) =>
        Regex.Matches(css, @"(?<selectors>[^{}]+)\{(?<body>[^{}]*)\}")
            .Select(m => (m.Groups["selectors"].Value, m.Groups["body"].Value));

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
