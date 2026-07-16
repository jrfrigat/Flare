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

namespace Flare.Core.Tests;

/// <summary>
/// A token that lands in a <c>calc()</c> addition must carry a unit: inside <c>calc()</c> a bare <c>0</c> is a
/// <c>&lt;number&gt;</c>, not a <c>&lt;length&gt;</c> - the "0 needs no unit" leniency exists only for a
/// literal written straight into a property. So <c>calc(100% - 46.79% + 0)</c> is
/// <c>&lt;percentage&gt; + &lt;number&gt;</c>, invalid at computed-value time, and the whole declaration is
/// dropped.
///
/// This is the third shape of the same bug the other two guards cover: a custom property that is PRESENT but
/// not substitutable, quietly invalidating the declaration that reads it. <see cref="ParkedTokenFallbackTests"/>
/// catches <c>initial</c> and <see cref="DeadFallbackTests"/> catches a core fallback - both ask "is a value
/// supplied". This one asks the next question down: "is the supplied value the CSS TYPE the declaration
/// substitutes it into". MaterialDesign2 shipped <c>SliderTokens.Gap = "0"</c> and its slider rail was
/// invisible at every size, through every release that had the theme.
///
/// The type is DERIVED from the consuming expression rather than declared on the token. Hand-annotating every
/// <c>[CssVar]</c> with a CSS type would be hundreds of edits that then rot as the CSS moves; the calc sites
/// already state the requirement unambiguously, and reading them keeps the guard true for calc sites nobody
/// has written yet.
///
/// Two things this has to get right, both learned the hard way:
/// 1. <b>Component C# counts.</b> The slider builds its rail geometry inline in .razor, not in a stylesheet;
///    a CSS-only scan misses precisely the case that shipped.
/// 2. <b>The indirection has to be followed.</b> That inline calc reads <c>var(--_gap)</c>, a private alias
///    the stylesheet defines as <c>var(--flare-slider-gap)</c>. Matching on the theme token's own name finds
///    nothing.
///
/// KNOWN GAP: this covers additions only. A token that is merely SCALED into a length property -
/// <c>margin-inline-start: calc(-1 * var(--flare-toc-marker-width))</c> - is the same class of bug, but
/// deciding it needs the property's own type, not just the expression's shape, and the property list would be
/// a second thing to keep true. The one in-box instance is inert (at <c>0</c> the declaration drops to a
/// margin of zero, which is what it would have computed anyway), so the cost was not worth paying yet.
/// </summary>
public sealed class LengthTokenUnitTests
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

    // "0", "-1", "1.5" - a value with no unit and no var()/function around it.
    private static readonly Regex _bareNumber = new(@"^-?\d+(\.\d+)?$", RegexOptions.Compiled);

    // `--_gap: var(--flare-slider-gap)` - a private alias standing in for a theme token.
    private static readonly Regex _alias =
        new(@"(--_[a-z0-9-]+)\s*:\s*var\(\s*(--flare-[a-z0-9-]+)", RegexOptions.Compiled);

    private static readonly Regex _varRef = new(@"var\(\s*(--[a-z0-9_-]+)", RegexOptions.Compiled);

    // An ADDITION operator, not a hyphen. CSS requires whitespace on both sides of + and - inside calc()
    // precisely so they cannot be confused with the hyphens in an identifier (--flare-state-hover-opacity is
    // full of them) or with the sign of a number (-1). That rule is what makes this test cheap and exact.
    private static readonly Regex _additionOperator = new(@"\s[+-]\s", RegexOptions.Compiled);

    [Theory]
    [MemberData(nameof(ThemeIndexes))]
    public void TokensUsedInACalcAddition_CarryAUnit(int index)
    {
        var theme = Factories[index]();
        var vars = theme.Design.FlattenDesign();
        var mustBeLength = TokensReachingACalcAddition();

        Assert.True(mustBeLength.Count > 0,
            "Found no calc() sites at all - the scan is not looking where it thinks it is.");

        var offenders = new List<string>();
        foreach (var token in mustBeLength.OrderBy(t => t, StringComparer.Ordinal))
        {
            if (!vars.TryGetValue(token, out var value) || value is null) continue;
            if (!_bareNumber.IsMatch(value.Trim())) continue;
            offenders.Add($"{token} = \"{value}\"   -> write \"{value.Trim()}px\"");
        }

        Assert.True(offenders.Count == 0,
            $"The '{theme.Id}' theme gives these tokens a unitless value, but each one is substituted into a "
            + "calc() that adds or subtracts it from a length or a percentage. A bare number is not a length "
            + "there, so the whole declaration is invalid at computed-value time and is dropped - the control "
            + "renders wrong, with no error anywhere. Add the unit:\n  "
            + string.Join("\n  ", offenders));
    }

    /// <summary>
    /// The theme tokens that end up as an operand of <c>+</c> or <c>-</c> inside some <c>calc()</c>, whether
    /// the expression lives in a stylesheet or is built inline in component code, and whether it names the
    /// token directly or through a private <c>--_alias</c>.
    /// </summary>
    private static HashSet<string> TokensReachingACalcAddition()
    {
        var root = FindRepoRoot();
        var cssDir = Path.Combine(root, "src", "Flare.Components", "wwwroot", "css");
        var componentsDir = Path.Combine(root, "src", "Flare.Components");

        var cssFiles = Directory.EnumerateFiles(cssDir, "*.css").ToList();
        var sources = cssFiles
            .Concat(Directory.EnumerateFiles(componentsDir, "*.razor", SearchOption.AllDirectories))
            .Select(File.ReadAllText)
            .ToList();

        // --_alias -> --flare-token. Only stylesheets declare these.
        var alias = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var css in cssFiles.Select(File.ReadAllText))
            foreach (Match m in _alias.Matches(css))
                alias[m.Groups[1].Value] = m.Groups[2].Value;

        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var text in sources)
        {
            foreach (var body in CalcBodies(text))
            {
                // Only an addition can clash this way, and only + / - can force two operands to agree on a
                // type. `calc(var(--opacity) * 100%)` is exactly how the opacity tokens are meant to be
                // used - multiplication takes a plain number, and demanding a unit there would be wrong.
                if (!_additionOperator.IsMatch(body)) continue;

                foreach (Match v in _varRef.Matches(body))
                {
                    if (!CarriesItsOwnTypeIntoTheAddition(body, v.Index)) continue;

                    var name = v.Groups[1].Value;
                    if (alias.TryGetValue(name, out var resolved)) name = resolved;
                    if (name.StartsWith("--flare-", StringComparison.Ordinal)) result.Add(name);
                }
            }
        }
        return result;
    }

    // Scaled by something that already carries a unit: `var(--x) * 100%`. Then the PRODUCT is what enters the
    // addition, and it is the literal that gives it a type - so the token is meant to be a plain number and
    // demanding a unit would be exactly backwards. This is how every opacity token is consumed.
    private static readonly Regex _scaledByAUnitAfter =
        new(@"^\s*\*\s*-?\d+(\.\d+)?(%|px|rem|em|vh|vw)", RegexOptions.Compiled);
    private static readonly Regex _scaledByAUnitBefore =
        new(@"-?\d+(\.\d+)?(%|px|rem|em|vh|vw)\s*\*\s*$", RegexOptions.Compiled);

    /// <summary>
    /// Whether this <c>var()</c> hands its OWN type to the surrounding addition. Multiplying or dividing by a
    /// plain number preserves the type, so those still count - <c>calc(50% + var(--h) / 2)</c> needs
    /// <c>--h</c> to be a length just as much as a bare operand would. Only being scaled by a unit-bearing
    /// literal takes the token out of the picture, because then the literal is what types the product.
    /// </summary>
    private static bool CarriesItsOwnTypeIntoTheAddition(string body, int varStart)
    {
        var end = EndOfVar(body, varStart);
        if (end < 0) return false;
        if (_scaledByAUnitAfter.IsMatch(body[end..])) return false;
        if (_scaledByAUnitBefore.IsMatch(body[..varStart])) return false;
        return true;
    }

    /// <summary>Index just past the <c>)</c> that closes the <c>var(</c> starting at <paramref name="start"/>.</summary>
    private static int EndOfVar(string body, int start)
    {
        var i = body.IndexOf('(', start);
        if (i < 0) return -1;
        var depth = 1;
        for (var j = i + 1; j < body.Length; j++)
        {
            if (body[j] == '(') depth++;
            else if (body[j] == ')' && --depth == 0) return j + 1;
        }
        return -1;
    }

    /// <summary>Every <c>calc(...)</c> body in the text, paren-balanced so nested var() fallbacks stay whole.</summary>
    private static IEnumerable<string> CalcBodies(string text)
    {
        const string open = "calc(";
        var i = text.IndexOf(open, StringComparison.Ordinal);
        while (i >= 0)
        {
            var start = i + open.Length;
            var depth = 1;
            var j = start;
            while (j < text.Length && depth > 0)
            {
                if (text[j] == '(') depth++;
                else if (text[j] == ')') depth--;
                j++;
            }
            if (depth == 0) yield return text[start..(j - 1)];
            i = text.IndexOf(open, start, StringComparison.Ordinal);
        }
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate the repo root (a folder containing src/Flare.Components).");
    }
}
