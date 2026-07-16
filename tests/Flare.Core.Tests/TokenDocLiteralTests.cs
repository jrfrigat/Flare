using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// Enforces the token mandate <b>in prose</b>: a core token's XML doc must not state what a theme sets it to.
///
/// The sibling guards cover the code - <see cref="DeadFallbackTests"/> forbids a theme default hiding in a CSS
/// fallback, and the token-record guard forbids a literal default on the member itself. The docs were the hole
/// left over: nothing stopped <c>/// &lt;summary&gt;Gap xs token (&lt;c&gt;0.25rem&lt;/c&gt;).&lt;/summary&gt;</c>,
/// which asserts a value the core does not own and cannot keep true. It did not keep true: when this guard was
/// written, 27 of the 84 value-quoting docs contradicted the shipping MD3 theme (that one said <c>0.25rem</c>
/// while the theme used <c>0.5rem</c>), because a doc and a theme drift apart the moment either is edited.
///
/// Two shapes are banned, because the pattern showed up in both:
/// 1. a CSS dimension literal anywhere in the summary - the theme's number stated as the token's meaning;
/// 2. a trailing <c>(&lt;c&gt;...&lt;/c&gt;).</c> parenthetical - the "this token is X" claim shape, which catches
///    the <c>(&lt;c&gt;var(--flare-elevation-2)&lt;/c&gt;)</c> quotes that carry no digits at all. Only the
///    trailing position counts: mid-sentence, the same markup is a pointer rather than an assertion (naming
///    the selector a token applies at, or telling a theme author what to reference).
///
/// What is still allowed, deliberately: naming a SPECIAL value semantically ("a theme with flat buttons parks
/// this at <c>none</c>", "parks this at <c>0</c>"). That describes the token's own contract rather than one
/// theme's taste, and a reader needs it. Hence the digit-plus-unit rule rather than a blanket ban on numerals.
/// </summary>
public sealed class TokenDocLiteralTests
{
    // A CSS dimension: digits glued to a unit. No whitespace allowed between them, so prose like
    // "3 sizes" or "5 steps" does not trip it - only "0.25rem", "2px", "150ms".
    private static readonly Regex _cssDimension =
        new(@"\d+(\.\d+)?(px|rem|em|ch|ex|vh|vw|vmin|vmax|pt|dp|deg|ms|s)\b", RegexOptions.Compiled);

    // The value-claim shape: a parenthetical holding nothing but a <c>..</c> run, sitting at the very END of
    // the summary - "Panel shadow token (<c>var(--flare-elevation-2)</c>)." That trailing position is what
    // makes it an assertion about the value rather than prose. Mid-sentence uses are left alone, because they
    // are pointers, not claims: naming the selector a token applies at ("on focus (<c>:focus-visible</c>),
    // not on a pointer press"), or telling a theme author what to reference.
    private static readonly Regex _bareValueClaim =
        new(@"\(<c>[^<]*</c>\)\s*\.?\s*</summary>", RegexOptions.Compiled);

    private static readonly Regex _cssVarMember = new(@"\[CssVar\(", RegexOptions.Compiled);
    private static readonly Regex _docLine = new(@"^\s*///(.*)$", RegexOptions.Compiled);

    [Fact]
    public void TokenRecordDocs_DoNotStateAThemesValue()
    {
        var tokensDir = Path.Combine(FindRepoRoot(), "src", "Flare.Abstractions", "Tokens");
        Assert.True(Directory.Exists(tokensDir), $"Token model folder not found: {tokensDir}");

        var offenders = new List<string>();
        var scanned = 0;

        foreach (var file in Directory.EnumerateFiles(tokensDir, "*.cs", SearchOption.AllDirectories)
                     .OrderBy(f => f, StringComparer.Ordinal))
        {
            var lines = File.ReadAllLines(file);
            var doc = new StringBuilder();
            var docStart = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var m = _docLine.Match(lines[i]);
                if (m.Success)
                {
                    if (doc.Length == 0) docStart = i + 1;
                    doc.Append(m.Groups[1].Value).Append(' ');
                    continue;
                }

                // Not a doc line: if a doc block just ended, judge it against the member it introduces.
                if (doc.Length > 0)
                {
                    if (_cssVarMember.IsMatch(lines[i]))
                    {
                        scanned++;
                        var text = doc.ToString();
                        var rel = Path.GetFileName(file);
                        foreach (Match d in _cssDimension.Matches(text))
                            offenders.Add($"{rel}:{docStart}  quotes the CSS value '{d.Value}'");
                        foreach (Match v in _bareValueClaim.Matches(text))
                            offenders.Add($"{rel}:{docStart}  states a value: '{v.Value}'");
                    }
                    doc.Clear();
                }
            }
        }

        Assert.True(scanned > 0, "Scanned no [CssVar] members - the guard is not looking where it thinks it is.");
        Assert.True(offenders.Count == 0,
            "A core token doc must say what the token is FOR, not what one theme sets it to - the core owns no "
            + "value, so such a doc is unowned and goes stale (27 of them were already wrong when this guard "
            + "landed). Describe the purpose instead; to name a special value, describe its meaning ('a theme "
            + "with no island treatment parks this at <c>transparent</c>'):\n  "
            + string.Join("\n  ", offenders));
    }

    // Walk up to the folder that contains src/Flare.Abstractions (the test runs from bin/).
    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Abstractions")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate the repo root (a folder containing src/Flare.Abstractions).");
    }
}
