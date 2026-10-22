using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using System.Text;

namespace Flare.Theming;

/// <summary>
/// Serializes themes and palettes to static, class-scoped CSS. This is the single source of
/// truth shared by both delivery strategies: runtime-once injection (A) and build-time files (B).
///   theme   -> <c>.flare-theme-{id} { design vars }</c> (+ optional dark Extended override)
///   palette -> <c>.flare-palette-{id} { light colors }</c> and
///              <c>.flare-palette-{id}.flare-mode-dark { dark colors }</c>
/// The theme/palette/mode classes are applied to the same root element, so the dark rules use
/// a compound selector. With those classes on a root element, component CSS resolves every
/// <c>var(--flare-*)</c> via the cascade -- switching theme/palette/mode is then just a class swap.
/// </summary>
public static class TokensToCss
{
    /// <summary>Static CSS for one theme's design tokens (and its dark Extended override, if any).</summary>
    public static string ThemeCss(ITheme theme)
    {
        var sb = new StringBuilder();
        Rule(sb, $".flare-theme-{theme.Id}", theme.Design.FlattenDesign());

        if (theme.ExtendedDarkOverride is { } dark)
        {
            var baseExt = theme.Design.Extended;
            var diff = dark
                .Where(kv => !baseExt.TryGetValue(kv.Key, out var b) || b != kv.Value)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            if (diff.Count > 0)
                Rule(sb, $".flare-theme-{theme.Id}.flare-mode-dark", diff);
        }
        return sb.ToString();
    }

    /// <summary>Static CSS for one palette (light + dark + optional high-contrast color schemes).</summary>
    public static string PaletteCss(Palette palette)
    {
        var sb = new StringBuilder();
        Rule(sb, $".flare-palette-{palette.Id}", palette.Light.FlattenColors());
        // Without its own high-contrast scheme a palette falls back to dark (Palette.ForMode); the dark rule
        // then names the high-contrast class as well, so the fallback costs a selector, not a copy of the tokens.
        var dark = $".flare-palette-{palette.Id}.flare-mode-dark";
        var highContrast = $".flare-palette-{palette.Id}.flare-mode-high-contrast";
        Rule(sb, palette.HighContrast is null ? $"{dark}, {highContrast}" : dark, palette.Dark.FlattenColors());
        if (palette.HighContrast is { } hc)
            Rule(sb, highContrast, hc.FlattenColors());
        return sb.ToString();
    }

    /// <summary>
    /// Concatenated static CSS for all given themes and palettes (strategy A bundle). A theme's rules come
    /// after those of every theme it is built on: its root carries each ancestor's class too, and at equal
    /// specificity the later rule is the one whose tokens apply.
    /// </summary>
    public static string Bundle(IEnumerable<ITheme> themes, IEnumerable<Palette> palettes)
    {
        var sb = new StringBuilder();
        foreach (var t in themes.OrderBy(t => ThemeLineage.Ids(t).Count)) sb.Append(ThemeCss(t));
        foreach (var p in palettes) sb.Append(PaletteCss(p));
        return sb.ToString();
    }

    /// <summary>
    /// Minifies CSS output by removing unnecessary whitespace, comments, and redundant characters.
    /// Safe for generated CSS (no user content).
    /// </summary>
    public static string Minify(string css)
    {
        if (string.IsNullOrEmpty(css)) return css;

        var result = new StringBuilder(css.Length);
        var pendingSpace = false;
        var i = 0;
        while (i < css.Length)
        {
            var c = css[i];

            // Comments are dropped.
            if (c == '/' && i + 1 < css.Length && css[i + 1] == '*')
            {
                var end = css.IndexOf("*/", i + 2, StringComparison.Ordinal);
                i = end < 0 ? css.Length : end + 2;
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                pendingSpace = result.Length > 0;
                i++;
                continue;
            }

            if (pendingSpace)
            {
                pendingSpace = false;
                var previous = result[^1];
                if (previous is not '{' and not '}' and not ':' and not ';' &&
                    c is not '{' and not '}' and not ':' and not ';')
                    result.Append(' ');
            }

            // A quoted string is copied as written: its spaces and comment-looking text are content.
            if (c is '\'' or '"')
            {
                var quote = c;
                result.Append(c);
                i++;
                while (i < css.Length)
                {
                    result.Append(css[i]);
                    if (css[i] == '\\' && i + 1 < css.Length)
                    {
                        result.Append(css[i + 1]);
                        i += 2;
                        continue;
                    }
                    if (css[i] == quote)
                    {
                        i++;
                        break;
                    }
                    i++;
                }
                continue;
            }

            // A semicolon directly before } is redundant, whitespace between them included.
            if (c == ';')
            {
                var next = i + 1;
                while (next < css.Length && char.IsWhiteSpace(css[next])) next++;
                if (next < css.Length && css[next] == '}')
                {
                    i++;
                    continue;
                }
            }

            result.Append(c);
            i++;
        }

        return result.ToString().Trim();
    }

    private static void Rule(StringBuilder sb, string selector, IReadOnlyDictionary<string, string> vars)
    {
        sb.Append(selector).Append('{');
        foreach (var (k, val) in vars)
            sb.Append(k).Append(':').Append(val).Append(';');
        sb.Append('}');
    }
}
