using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// An overlay that is hidden by CSS must not still occupy the room it would occupy when shown.
/// `visibility: hidden` and a faded-out item both leave the box in the layout, and an anchored overlay's
/// box sits wherever its PLACEMENT puts it - which for a right-hand placement on an anchor near the
/// window edge is off the edge. Measured at 375px before the fix: a hidden tooltip bubble reached 13px
/// past the content edge and a closed speed dial 91px, and both gave the page real horizontal scroll
/// while nothing was open. Size containment is what removes the contribution; `allow-discrete` is what
/// keeps the closing animation, by holding the collapse until the fade has finished.
///
/// This guards the declarations rather than the geometry, because the geometry needs a browser. It is
/// deliberately a short list: it names the two overlays whose hidden footprint was measured, so deleting
/// a line fails here instead of silently returning a sideways scrollbar to a phone.
/// </summary>
public class HiddenOverlayFootprintTests
{
    // Component, the rule that hides it, and the rule that shows it again.
    public static TheoryData<string, string, string> Overlays => new()
    {
        { "tooltip.css", $".{Css.Classes.Tooltip.Content}", $".{Css.Classes.Tooltip.ContentVisible}" },
        { "fabmenu.css", $".{Css.Classes.FabMenu.List}", $".{Css.Classes.FabMenu.ListOpen}" },
    };

    [Theory]
    [MemberData(nameof(Overlays))]
    public void HiddenOverlay_IsSizeContainedAndRestoredWhenShown(string file, string restSelector, string shownSelector)
    {
        var css = StripComments(File.ReadAllText(Path.Combine(CssDir, file)));

        var atRest = RuleBody(css, restSelector);
        Assert.True(
            atRest is not null && Regex.IsMatch(atRest, @"content-visibility\s*:\s*hidden"),
            $"{restSelector} must declare `content-visibility: hidden` so a hidden overlay contributes "
            + "nothing to its scroll container. Without it the box keeps its placement offset and can "
            + "reach past the viewport while nobody can see it.");

        Assert.True(
            atRest is not null && Regex.IsMatch(atRest, @"transition-behavior\s*:\s*allow-discrete"),
            $"{restSelector} must declare `transition-behavior: allow-discrete`, or the collapse lands at "
            + "the START of the hide transition and the overlay disappears instead of fading out.");

        Assert.True(
            atRest is not null && Regex.IsMatch(atRest, @"transition:[^;]*content-visibility"),
            $"{restSelector} must transition `content-visibility`, or `allow-discrete` has nothing to "
            + "apply to and the collapse is immediate again.");

        var shown = Rules(css).Where(r => Subjects(r.Selectors).Contains(shownSelector))
            .Select(r => r.Body).FirstOrDefault();
        Assert.True(
            shown is not null && Regex.IsMatch(shown, @"content-visibility\s*:\s*visible"),
            $"{shownSelector} must restore `content-visibility: visible`. A size-contained box measures "
            + "as if it were empty, so the collision engine and the first painted frame would both see "
            + "the wrong size.");
    }

    private static string CssDir =>
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

    // The base rule for a selector: the one whose selector list is exactly that single selector, so a
    // modifier or a descendant rule is never mistaken for it.
    private static string? RuleBody(string css, string selector) =>
        Rules(css).Where(r => r.Selectors.Trim() == selector).Select(r => r.Body).FirstOrDefault();

    private static IEnumerable<(string Selectors, string Body)> Rules(string css) =>
        Regex.Matches(css, @"(?<selectors>[^{}]+)\{(?<body>[^{}]*)\}")
            .Select(m => (m.Groups["selectors"].Value, m.Groups["body"].Value));

    private static string[] Subjects(string selectors) =>
        [.. selectors.Split(',').Select(s => s.Trim())];

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
