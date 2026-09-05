using System.Text.RegularExpressions;

namespace Flare.Guards.Tests;

/// <summary>
/// A dialog has to stay reachable. It is centred on a scrim that is `position: fixed` and does not
/// scroll, so a panel taller than the window does not push a scrollbar anywhere - it simply hangs off
/// both ends of the screen with its title and its buttons on the wrong side of the edge.
///
/// Measured in Chrome on Flare's own stylesheet before the fix: at a viewport of 1274px, a dialog of
/// eighty paragraphs came out 2862px tall, its top 794px ABOVE the window and its actions the same
/// distance below, with the content region reporting `overflow-y: visible` and no scroll anywhere. The
/// same probe after: the panel is 1226px, its top is at 24px, header and actions are both on screen,
/// and the content region scrolls 2758px of content in 1122px.
///
/// This guards the declarations rather than the pixels, which need a browser. The small-screen rule had
/// the cap for phones only, which is why it went unnoticed on a desktop.
/// </summary>
public class DialogReachabilityTests
{
    [Fact]
    public void Panel_IsCappedToTheViewport()
    {
        var body = RuleBody($".{Css.Classes.Dialog.Root}");

        Assert.True(body is not null, $"`.{Css.Classes.Dialog.Root}` is missing.");
        Assert.Matches(@"max-block-size\s*:\s*calc\(100dvh", Normalize(body!));
        Assert.Matches(@"flex-direction\s*:\s*column", Normalize(body!));
    }

    // Dynamic viewport units, not `vh`, ANYWHERE a cap exists so that something fits the screen. On a
    // phone `vh` counts the space behind the browser chrome, so the one place a cap is needed is the one
    // place it is too generous - the dialog left its top and bottom under the URL bar, and the grid's
    // filter menu made the same promise about its actions row in the same comment that broke it.
    // NOTE: this guard was written with a mangled pattern and asserted nothing at all until it was
    // repaired - a stray control character in the regex meant it could never match anything. A guard
    // that passes for the wrong reason is worse than none, because it gets reported as cover.
    [Fact]
    public void NoStylesheetCapsWithStaticViewportHeight()
    {
        // "100vh" but not "100dvh": in the static form the digits sit directly before the unit.
        AssertNoMatch(@"\d+vh\b",
            "size something in static vh, which overshoots the visible area on a phone; use dvh");
    }

    // A number subtracted from the viewport is a theme's opinion about how much air a surface keeps
    // at the screen edge, and a theme cannot repoint a number. It also spreads: the 0.31.0 dialog cap
    // took its 3rem from the rule next to it, because copying a literal is easier than adding a token.
    [Fact]
    public void NoLiteralInsetSubtractedFromTheViewport()
    {
        AssertNoMatch(@"calc\(\s*100d?v[hw]\s*-\s*[\d.]",
            $"subtract a literal length from the viewport; use {Css.Tokens.Overlay.ViewportInset}");
    }

    // A share of the screen is the same kind of opinion. 100 is not - that IS the viewport.
    [Fact]
    public void NoLiteralShareOfTheViewport()
    {
        AssertNoMatch(@"(?<![\d.])(?!100d?v[hw]\b)\d+(\.\d+)?d?v[hw]\b",
            $"take a literal share of the viewport; use {Css.Tokens.Overlay.PanelMaxBlockSize}");
    }

    // A percentage inside color-mix is how strongly one colour is laid over another - a state layer,
    // a veil, a tint. Written as a number it is core deciding what "hover" or "selected" LOOKS like
    // for every theme. Every one of them reads a token now; this keeps it that way. The one literal
    // that stays legitimate is the `* 100%` that turns a 0..1 opacity token into a percentage.
    [Fact]
    public void NoLiteralPercentageInAColorMix()
    {
        AssertNoMatch(@"color-mix\([^;{}]*?(?<![\d.])(?!100%)\d+(\.\d+)?%",
            "mix a colour at a literal strength; the percentage belongs in a token");
    }

    private static void AssertNoMatch(string pattern, string what)
    {
        var offenders = Directory.EnumerateFiles(CssDir, "*.css")
            .Select(path => (Name: Path.GetFileName(path), Css: StripComments(File.ReadAllText(path))))
            .Where(f => Regex.IsMatch(f.Css, pattern))
            .Select(f => f.Name)
            .ToList();

        Assert.True(offenders.Count == 0,
            "These stylesheets " + what + ": " + string.Join(", ", offenders));
    }

    [Fact]
    public void ContentRegion_IsWhatScrolls()
    {
        var body = RuleBody($".{Css.Classes.Dialog.Content}");

        Assert.True(body is not null, $"`.{Css.Classes.Dialog.Content}` is missing.");
        Assert.Matches(@"overflow\s*:\s*auto", Normalize(body!));
        // A flex item refuses to shrink below its content without this, so the panel would grow past
        // its own cap instead of the content scrolling inside it.
        Assert.Matches(@"min-block-size\s*:\s*0", Normalize(body!));
    }

    [Fact]
    public void HeaderAndActions_KeepTheirSize()
    {
        var css = StripComments(File.ReadAllText(Path.Combine(CssDir, "dialog.css")));
        var rule = Regex.Matches(css, @"(?<selectors>[^{}]+)\{(?<body>[^{}]*)\}")
            .FirstOrDefault(m => m.Groups["selectors"].Value.Contains($".{Css.Classes.Dialog.Actions}", StringComparison.Ordinal)
                && Regex.IsMatch(m.Groups["body"].Value, @"flex\s*:\s*0 0 auto"));

        Assert.True(rule is not null,
            "The header and the actions must be `flex: 0 0 auto`, or the panel's cap is paid for by "
            + "squashing the title and the buttons instead of by scrolling the content.");
        Assert.Contains($".{Css.Classes.Dialog.Header}", rule!.Groups["selectors"].Value, StringComparison.Ordinal);
    }

    private static string CssDir =>
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

    private static string? RuleBody(string selector)
    {
        var css = StripComments(File.ReadAllText(Path.Combine(CssDir, "dialog.css")));
        return Regex.Matches(css, @"(?<selectors>[^{}]+)\{(?<body>[^{}]*)\}")
            .Where(m => m.Groups["selectors"].Value.Split(',').Any(s => Normalize(s) == Normalize(selector)))
            .Select(m => m.Groups["body"].Value)
            .FirstOrDefault();
    }

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
