using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// A control that a hover reveals does not exist on a phone. There is no hover to uncover it with, and
/// a transparent element is still a tap target - so it is worse than absent: invisible, unfindable, and
/// hittable by accident.
///
/// Flare's answer is not to drop the fade-in but to gate it on a fine pointer: the data grid's column
/// resize handle and the slider's hover thumb both hide themselves inside <c>@media (hover: hover)</c>
/// and stay visible everywhere else. This holds the whole library to that, because the failure is
/// silent - the desktop it was written on shows exactly what the author intended.
///
/// Pseudo-elements are excluded. A <c>::before</c> that fades in on hover is a state layer painted over
/// something already visible, not an affordance of its own; nothing is hidden from a touch reader by
/// its absence.
/// </summary>
public class TouchAffordanceTests
{
    [Fact]
    public void AHoverRevealedControlIsGatedOnAFinePointer()
    {
        var offenders = new List<string>();

        foreach (var path in Directory.EnumerateFiles(CssDir, "*.css"))
        {
            var css = StripComments(File.ReadAllText(path));

            // Everything that any rule makes visible, and whether a reader without a hover can get
            // there. A second trigger is the usual answer and it is enough: the slider's value bubble
            // comes up on `:focus-within`, which a finger produces by dragging the slider, and a
            // tooltip has `--open` and a focus variant. `:focus-visible` is NOT one of those - it is
            // the keyboard's, and a tap does not raise it.
            var reveals = Rules(css)
                .Where(r => OpacityAbove(r.Body, 0))
                .SelectMany(r => r.Selectors.Split(',')
                    .SelectMany(part => Targets(part).Select(t => (Target: t, Reachable: !NeedsHover(part)))))
                .ToList();

            var hoverOnly = reveals.Select(x => x.Target).ToHashSet(StringComparer.Ordinal);
            hoverOnly.ExceptWith(reveals.Where(x => x.Reachable).Select(x => x.Target));

            // What is transparent for everyone, including the readers who have no hover. A gated block
            // and a keyframe are both fine: one only applies where a hover exists, the other is a
            // frame of an animation rather than a resting state.
            var hidden = Rules(WithoutBlocks(WithoutBlocks(css, HoverGate), Keyframes))
                .Where(r => Opacity(r.Body) == 0)
                .SelectMany(r => Targets(r.Selectors))
                .ToHashSet(StringComparer.Ordinal);

            offenders.AddRange(hidden.Intersect(hoverOnly, StringComparer.Ordinal)
                .Select(sel => Path.GetFileName(path) + ": " + sel));
        }

        Assert.True(offenders.Count == 0,
            "These are invisible until a pointer that a phone does not have hovers them. Put the "
            + "`opacity: 0` inside `@media (hover: hover)` so a coarse pointer keeps them visible: "
            + string.Join(", ", offenders));
    }

    private const string HoverGate = @"@media[^{]*\(\s*hover\s*:\s*hover\s*\)[^{]*";
    private const string Keyframes = @"@(-\w+-)?keyframes[^{]*";

    private static string CssDir =>
        Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");

    private sealed record Rule(string Selectors, string Body);

    // Nested rules come out of this correctly and at-rule headers do not: a selector cannot contain a
    // brace, so `@media (...) {` never matches as one.
    private static IEnumerable<Rule> Rules(string css) =>
        Regex.Matches(css, @"(?<sel>[^{}]+)\{(?<body>[^{}]*)\}")
            .Select(m => new Rule(m.Groups["sel"].Value, m.Groups["body"].Value));

    // The element a rule is about: the last simple selector, without its pseudo-classes. A pseudo-
    // ELEMENT is not one, and returns nothing.
    private static IEnumerable<string> Targets(string selectors) =>
        selectors.Split(',')
            .Select(part => part.Trim())
            .Where(part => !part.Contains("::", StringComparison.Ordinal))
            .Select(part => part.Split([' ', '>', '+', '~'], StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "")
            .Select(last => last.Split(':')[0].Trim())
            .Where(name => name.StartsWith('.') || name.StartsWith('#'));

    private static double? Opacity(string body)
    {
        var m = Regex.Match(body, @"(?<![-\w])opacity\s*:\s*(?<v>\d+(\.\d+)?)\s*(!important)?\s*[;}]|(?<![-\w])opacity\s*:\s*(?<v2>\d+(\.\d+)?)\s*(!important)?\s*$");
        if (!m.Success) return null;
        var raw = m.Groups["v"].Success ? m.Groups["v"].Value : m.Groups["v2"].Value;
        return double.Parse(raw, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static bool OpacityAbove(string body, double floor) => Opacity(body) is { } v && v > floor;

    // A trigger a finger cannot produce. `:focus-visible` is checked first because it contains
    // `:focus`, and unlike `:focus` it is the keyboard's alone.
    private static bool NeedsHover(string selectorPart) =>
        selectorPart.Contains(":hover", StringComparison.Ordinal)
        || selectorPart.Contains(":focus-visible", StringComparison.Ordinal);

    // Cuts every block whose header matches, braces balanced, so what is left is the CSS that applies
    // unconditionally.
    private static string WithoutBlocks(string css, string headerPattern)
    {
        var kept = new StringBuilder();
        var cursor = 0;

        foreach (Match m in Regex.Matches(css, headerPattern))
        {
            if (m.Index < cursor) continue;
            var open = css.IndexOf('{', m.Index);
            if (open < 0) break;

            var depth = 0;
            var end = open;
            for (; end < css.Length; end++)
            {
                if (css[end] == '{') depth++;
                else if (css[end] == '}' && --depth == 0) break;
            }

            kept.Append(css, cursor, m.Index - cursor);
            cursor = Math.Min(end + 1, css.Length);
        }

        if (cursor < css.Length) kept.Append(css, cursor, css.Length - cursor);
        return kept.ToString();
    }

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
