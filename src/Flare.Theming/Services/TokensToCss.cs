using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using System.Text;
using System.Text.RegularExpressions;

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
        Rule(sb, $".flare-palette-{palette.Id}.flare-mode-dark", palette.Dark.FlattenColors());
        if (palette.HighContrast is { } hc)
            Rule(sb, $".flare-palette-{palette.Id}.flare-mode-high-contrast", hc.FlattenColors());
        return sb.ToString();
    }

    /// <summary>Concatenated static CSS for all given themes and palettes (strategy A bundle).</summary>
    public static string Bundle(IEnumerable<ITheme> themes, IEnumerable<Palette> palettes)
    {
        var sb = new StringBuilder();
        foreach (var t in themes) sb.Append(ThemeCss(t));
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

        // Remove comments (/* ... */)
        var result = Regex.Replace(css, @"/\*.*?\*/", "", RegexOptions.Singleline);

        // Remove newlines and excess whitespace
        result = Regex.Replace(result, @"\s+", " ");

        // Remove space after { and before }
        result = Regex.Replace(result, @"\s*\{\s*", "{");
        result = Regex.Replace(result, @"\s*\}", "}");

        // Remove space after : and before value (but keep in values like "cubic-bezier(...)")
        result = Regex.Replace(result, @":\s+", ":");

        // Remove trailing semicolons before }
        result = Regex.Replace(result, ";}", "}");

        // Remove leading/trailing whitespace
        return result.Trim();
    }

    private static void Rule(StringBuilder sb, string selector, IReadOnlyDictionary<string, string> vars)
    {
        sb.Append(selector).Append('{');
        foreach (var (k, val) in vars)
            sb.Append(k).Append(':').Append(val).Append(';');
        sb.Append('}');
    }
}
