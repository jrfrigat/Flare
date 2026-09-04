using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// A sticky table header cannot draw its own edge with a border.
///
/// `.flare-datagrid__table` is `border-collapse: collapse`, and under collapse the TABLE owns and paints
/// every collapsed border - not the cell it was declared on. A sticky cell moves; the table does not. So
/// a scrolling grid kept its opaque header and left the divider behind with the first row, and the data
/// ran up flush against the column titles - or, with the filter row on, against the filter inputs.
///
/// Measured in Chrome on two identical sticky-header tables side by side: with `collapse` the rule under
/// the last header row vanishes the moment the body scrolls; with `separate` it stays. Switching the
/// grid to `separate` is the wrong fix - it doubles every cell edge and changes how the frozen columns
/// paint - so the edges are redrawn as inset shadows, which belong to the element and travel with it.
///
/// This guards the declarations rather than the pixels, which need a browser. It is short on purpose:
/// it names the three edges that were measured, so deleting one fails here instead of silently
/// returning a header that dissolves into its own data.
/// </summary>
public class StickyTableEdgeTests
{
    // The selector that redraws the edge, the shadow offset it redraws it with, and the selector whose
    // rule drops the border being replaced - the same one, except for the footer, whose divider is
    // declared on the row while the cells are what stick.
    public static TheoryData<string, string, string> Edges => new()
    {
        // The header's bottom edge.
        {
            ".flare-datagrid__wrapper--scroll thead th",
            "inset 0 calc(-1 * var(--flare-border-width)) 0",
            ".flare-datagrid__wrapper--scroll thead th"
        },
        // The column dividers between header cells on a bordered grid.
        {
            ".flare-datagrid--bordered .flare-datagrid__wrapper--scroll thead th:not(:last-child)",
            "inset calc(-1 * var(--flare-border-width)) 0 0",
            ".flare-datagrid--bordered .flare-datagrid__wrapper--scroll thead th:not(:last-child)"
        },
        // The aggregate footer's top edge, from the footer's own width token.
        {
            ".flare-datagrid__wrapper--scroll tfoot .flare-datagrid__aggregate-row > .flare-datagrid__td",
            "inset 0 var(--flare-datagrid-aggregate-divider-width) 0",
            ".flare-datagrid__wrapper--scroll tfoot .flare-datagrid__aggregate-row"
        },
    };

    [Theory]
    [MemberData(nameof(Edges))]
    public void StickyEdge_IsPaintedByTheCellAndNotByACollapsedBorder(string selector, string shadow, string dropsBorder)
    {
        var css = StripComments(File.ReadAllText(Path.Combine(CssDir, "datagrid.css")));
        var body = RuleBody(css, selector);

        Assert.True(body is not null,
            $"`{selector}` is missing. It is what redraws the edge a sticky cell loses to "
            + "`border-collapse: collapse`.");

        Assert.True(Normalize(body!).Contains(Normalize(shadow)),
            $"`{selector}` must draw its edge with `box-shadow: {shadow} ...` - a border there is painted "
            + "by the table and stays behind when the header sticks.");

        var dropper = RuleBody(css, dropsBorder);
        Assert.True(dropper is not null && Regex.IsMatch(dropper, @"border-(bottom|right|top)\s*:\s*none"),
            $"`{dropsBorder}` must drop the border this shadow replaces, or the two draw the same line "
            + "twice at rest and only one of them survives a scroll.");
    }

    private static string CssDir =>
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

    private static string? RuleBody(string css, string selector) =>
        Regex.Matches(css, @"(?<selectors>[^{}]+)\{(?<body>[^{}]*)\}")
            .Where(m => m.Groups["selectors"].Value.Split(',').Any(s => Normalize(s) == Normalize(selector)))
            .Select(m => m.Groups["body"].Value)
            .FirstOrDefault();

    private static string Normalize(string s) => Regex.Replace(s, @"\s+", " ").Trim();

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
