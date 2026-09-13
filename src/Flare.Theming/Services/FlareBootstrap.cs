using Flare.Abstractions.Tokens;
using System.Text;

namespace Flare.Theming;

/// <summary>
/// Generates the synchronous anti-FOUC bootstrap script that sets the initial theme/palette/mode
/// classes on &lt;html&gt; before first paint (from localStorage, validated against the registered
/// ids, falling back to the configured defaults). The script body is built from C# so it never
/// drifts from the registered themes/palettes. For SSR use the <c>FlareBootstrap</c> component;
/// for static WASM, place the output in <c>index.html</c> &lt;head&gt; before the Blazor script.
/// Keys: <c>flare-theme</c>, <c>flare-palette</c>, <c>flare-mode</c>.
/// </summary>
public static class FlareBootstrap
{
    /// <summary>Builds the bootstrap JavaScript (no &lt;script&gt; wrapper).</summary>
    public static string GenerateScript(
        IEnumerable<string> themeIds,
        IEnumerable<string> paletteIds,
        string defaultTheme,
        string defaultPalette,
        ThemeMode defaultMode)
        => GenerateScript(themeIds, paletteIds, defaultTheme, defaultPalette, defaultMode, styleFamilies: null);

    /// <summary>
    /// Builds the bootstrap JavaScript (no &lt;script&gt; wrapper), also adding the style-family class of a
    /// theme whose stylesheets come from another theme, so a saved derived theme is styled from the first
    /// paint rather than only once .NET has started.
    /// </summary>
    /// <param name="themeIds">Ids of the registered themes; a saved id outside this list falls back to the default.</param>
    /// <param name="paletteIds">Ids of the registered palettes; a saved id outside this list falls back to the default.</param>
    /// <param name="defaultTheme">Theme applied when nothing valid is saved.</param>
    /// <param name="defaultPalette">Palette applied when nothing valid is saved.</param>
    /// <param name="defaultMode">Mode applied when nothing is saved.</param>
    /// <param name="styleFamilies">
    /// Theme id to <see cref="Flare.Abstractions.ITheme.StyleFamilyId"/> for the themes whose family differs
    /// from their id. Null or empty emits no family lookup.
    /// </param>
    public static string GenerateScript(
        IEnumerable<string> themeIds,
        IEnumerable<string> paletteIds,
        string defaultTheme,
        string defaultPalette,
        ThemeMode defaultMode,
        IReadOnlyDictionary<string, string>? styleFamilies)
    {
        var themes = JsArray(themeIds);
        var palettes = JsArray(paletteIds);
        var mode = defaultMode.ToString().ToLowerInvariant();

        var sb = new StringBuilder();
        sb.Append("(function(){var d=document.documentElement,s=localStorage;");
        sb.Append("var T=").Append(themes).Append(",P=").Append(palettes).Append(';');
        sb.Append("var t=s.getItem('flare-theme');if(T.indexOf(t)<0)t='").Append(Esc(defaultTheme)).Append("';");
        sb.Append("var p=s.getItem('flare-palette');if(P.indexOf(p)<0)p='").Append(Esc(defaultPalette)).Append("';");
        sb.Append("var m=s.getItem('flare-mode')||'").Append(mode).Append("';");
        sb.Append("d.classList.add('flare-theme-'+t,'flare-palette-'+p);");
        var families = styleFamilies?.Where(f => f.Key != f.Value).ToList();
        if (families is { Count: > 0 })
        {
            sb.Append("var F={").Append(string.Join(",", families.Select(f => "'" + Esc(f.Key) + "':'" + Esc(f.Value) + "'"))).Append("};");
            sb.Append("if(Object.prototype.hasOwnProperty.call(F,t))d.classList.add('flare-theme-'+F[t]);");
        }
        // Both classes, not just the dark one: flare-mode-light declares color-scheme:light, which is
        // what stops native controls rendering dark inside a light Flare page.
        sb.Append("var dk=(m==='dark'||(m==='auto'&&matchMedia('(prefers-color-scheme: dark)').matches));");
        sb.Append("d.classList.add(dk?'flare-mode-dark':'flare-mode-light');");
        sb.Append("})();");
        return sb.ToString();
    }

    private static string JsArray(IEnumerable<string> ids) =>
        "[" + string.Join(",", ids.Select(i => "'" + Esc(i) + "'")) + "]";

    private static string Esc(string s) => s.Replace("\\", "\\\\").Replace("'", "\\'");
}
