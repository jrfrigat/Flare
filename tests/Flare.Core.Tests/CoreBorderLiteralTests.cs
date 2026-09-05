using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Core.Tests;

/// <summary>
/// Guards the border scale: a rule drawn between or around things in a core stylesheet gets its width
/// and style from <c>--flare-border-*</c>, never from a literal.
/// </summary>
/// <remarks>
/// A hardcoded <c>1px solid</c> reads as harmless because the colour beside it is a token, and that is
/// exactly why 86 of them accumulated. It leaves a design language unable to say that its separators are
/// heavier, or dashed, or absent - core has already decided, and the theme can only recolour the
/// decision. The shipped app bar and drawer rules were the visible end of it: M3 draws no divider under
/// a top app bar, and core drew one anyway.
///
/// Three shapes are deliberately allowed through, because none of them is a rule between things:
/// a border reserved as <c>transparent</c> so a variant swap cannot shift layout, the button spinner's
/// <c>currentColor</c> ring, which is geometry, and the markdown blockquote's accent bar.
/// </remarks>
public sealed class CoreBorderLiteralTests
{
    // border / border-top / border-inline-end / ... followed by a literal width and a line style.
    private static readonly Regex LiteralBorder = new(
        @"border(?:-(?:top|right|bottom|left|block|inline)(?:-(?:start|end))?)?\s*:\s*[0-9.]+(?:px|rem|em)\s+(?:solid|dashed|dotted|double)\b[^;}]*",
        RegexOptions.Compiled);

    // Allowed by shape, not by file: each is a width reserved for layout or a drawn figure, not a rule.
    private static bool IsStructural(string declaration) =>
        declaration.Contains("transparent", StringComparison.Ordinal)
        || declaration.Contains("currentColor", StringComparison.OrdinalIgnoreCase);

    // The single decorative accent bar. Named explicitly so a second one has to argue for itself.
    private const string BlockquoteAccent = $"border-left: 4px solid var({Css.Tokens.Color.Primary})";

    [Fact]
    public void NoCoreStylesheet_DrawsARuleFromALiteralWidth()
    {
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css").OrderBy(f => f, StringComparer.Ordinal))
        {
            var css = StripComments(File.ReadAllText(file));
            foreach (Match m in LiteralBorder.Matches(css))
            {
                var declaration = Compact(m.Value);
                if (IsStructural(declaration) || declaration.StartsWith(BlockquoteAccent, StringComparison.Ordinal))
                    continue;
                offenders.Add($"{Path.GetFileName(file)}  {declaration}");
            }
        }

        Assert.True(offenders.Count == 0,
            "A rule drawn in core CSS must take its width and style from the border scale - " +
            $"var({Css.Tokens.Border.Divider}) between things, var({Css.Tokens.Border.Outline}) around a surface, " +
            $"or var({Css.Tokens.Border.Width}[-emphasis]) var({Css.Tokens.Border.Style}) with the component's own " +
            "colour token. A literal width decides on behalf of every theme:" +
            Environment.NewLine + string.Join(Environment.NewLine, offenders));
    }

    private static string Compact(string s) => string.Join(' ', s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

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
