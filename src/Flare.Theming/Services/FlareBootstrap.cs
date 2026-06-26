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
        sb.Append("if(m==='dark'||(m==='auto'&&matchMedia('(prefers-color-scheme: dark)').matches))d.classList.add('flare-mode-dark');");
        sb.Append("})();");
        return sb.ToString();
    }

    private static string JsArray(IEnumerable<string> ids) =>
        "[" + string.Join(",", ids.Select(i => "'" + Esc(i) + "'")) + "]";

    private static string Esc(string s) => s.Replace("\\", "\\\\").Replace("'", "\\'");
}
