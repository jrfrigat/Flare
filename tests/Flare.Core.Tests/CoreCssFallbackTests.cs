using System.Text;

namespace Flare.Core.Tests;

/// <summary>
/// Guards the last hole in the core/theme split: a <c>var(--flare-x, &lt;fallback&gt;)</c> in a core
/// stylesheet whose fallback is a LOOK rather than an identity value.
/// </summary>
/// <remarks>
/// The sibling guards cover the other shapes of this problem and none of them can see this one.
/// <c>AbstractionsTokenRecords_ShipNoLiteralDefaults</c> polices the token RECORDS, not the CSS.
/// <c>DeadFallbackTests</c> removes fallbacks on tokens every theme emits. <c>ParkedTokenFallbackTests</c>
/// insists on keeping them where a theme parks the token at <c>initial</c>. CssAudit's own
/// <c>LiteralFallbackRx</c> deliberately exempts the <c>--flare-&lt;component&gt;-*</c> families, because
/// there a fallback is normally that parked sentinel - which is exactly why nine literal fallbacks sat
/// in core CSS unflagged, and why the two this test found had been invisible for longer still.
///
/// The rule is drawn from what the tree actually contains rather than invented: 44 reads across 19
/// tokens, and they split cleanly. A per-instance var a theme never emits (<c>--flare-col-span</c>,
/// <c>--flare-z-dropdown</c>, <c>--flare-dial-angle</c>) needs a fallback, and its fallback is an
/// identity - <c>1</c>, <c>auto</c>, <c>0deg</c>, <c>minmax(0, 1fr)</c>, or a chain of other vars.
/// Nothing about it is a design decision. A fallback that names a COLOUR is the opposite: it decides
/// what the component looks like on behalf of every theme, and no theme can reach it.
/// </remarks>
public sealed class CoreCssFallbackTests
{
    // A fallback naming a colour is a look. Everything else in core CSS today is an identity value, so
    // this is the whole rule; widen it only with a case that argues for itself.
    private static readonly string[] ColourFunctions =
        ["color-mix(", "rgb(", "rgba(", "hsl(", "hsla(", "oklch(", "oklab(", "lch(", "lab("];

    [Fact]
    public void NoCoreStylesheet_FallsBackToAVisualOpinion()
    {
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f, StringComparer.Ordinal))
        {
            var css = StripComments(File.ReadAllText(file));

            foreach (var (token, fallback) in ReadFallbacks(css))
            {
                if (IsIdentityValue(fallback)) continue;
                offenders.Add($"{Path.GetFileName(file)}  var({token}, {Compact(fallback)})");
            }
        }

        Assert.True(offenders.Count == 0,
            "A core stylesheet falls back to a value that decides how the component LOOKS. The core owns "
            + "no look: register the token in Css.Tokens, add it to the component's token record as a "
            + "required member, let each theme state it, and read it without a fallback. See "
            + $"{Css.Tokens.SwitchField.FocusShadowOn} for the shape of the fix:\n  "
            + string.Join("\n  ", offenders));
    }

    /// <summary>
    /// True for a fallback that carries no design decision: a number, a keyword, a zero angle, a grid
    /// track, or a chain that defers to other <c>--flare-*</c> vars.
    /// </summary>
    private static bool IsIdentityValue(string fallback)
    {
        var value = fallback.Trim();
        if (value.Length == 0) return true;

        // A hex colour is a look wherever it appears.
        if (value.Contains('#', StringComparison.Ordinal)) return false;

        foreach (var fn in ColourFunctions)
            if (value.Contains(fn, StringComparison.OrdinalIgnoreCase))
                return false;

        return true;
    }

    /// <summary>Yields every <c>var(--flare-*, fallback)</c> read, with the fallback's parens balanced.</summary>
    private static IEnumerable<(string Token, string Fallback)> ReadFallbacks(string css)
    {
        const string open = "var(";
        var i = 0;

        while ((i = css.IndexOf(open, i, StringComparison.Ordinal)) >= 0)
        {
            var cursor = i + open.Length;
            while (cursor < css.Length && char.IsWhiteSpace(css[cursor])) cursor++;

            var nameStart = cursor;
            while (cursor < css.Length && (css[cursor] == '-' || char.IsLetterOrDigit(css[cursor]))) cursor++;
            var token = css[nameStart..cursor];

            while (cursor < css.Length && char.IsWhiteSpace(css[cursor])) cursor++;

            // No comma: the read has no fallback and nothing to judge.
            if (cursor >= css.Length || css[cursor] != ',' || !token.StartsWith("--flare-", StringComparison.Ordinal))
            {
                i += open.Length;
                continue;
            }

            cursor++;
            var depth = 1;
            var fallback = new StringBuilder();
            while (cursor < css.Length && depth > 0)
            {
                var c = css[cursor];
                if (c == '(') depth++;
                else if (c == ')' && --depth == 0) break;
                fallback.Append(c);
                cursor++;
            }

            yield return (token, fallback.ToString());
            i = cursor;
        }
    }

    private static string StripComments(string css)
    {
        var sb = new StringBuilder(css.Length);
        var inComment = false;

        for (var i = 0; i < css.Length; i++)
        {
            if (!inComment && css[i] == '/' && i + 1 < css.Length && css[i + 1] == '*') { inComment = true; i++; continue; }
            if (inComment && css[i] == '*' && i + 1 < css.Length && css[i + 1] == '/') { inComment = false; i++; continue; }
            if (!inComment) sb.Append(css[i]);
        }

        return sb.ToString();
    }

    private static string Compact(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    // Walk up to the folder that contains src/Flare.Components (the test runs from bin/).
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
