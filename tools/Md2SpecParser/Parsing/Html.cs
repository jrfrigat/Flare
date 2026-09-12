using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Tools.Md2SpecParser.Parsing;

/// <summary>
/// The little of HTML the guideline content uses, turned into markdown. The body arrives as
/// hand-authored HTML with the site's own <c>&lt;mio-*&gt;</c> pseudo-elements double-escaped inside
/// it, so decoding has to happen first and the pseudo-elements have to be recognised afterwards.
/// </summary>
public static partial class Html
{
    /// <summary>Converts a guideline content fragment to markdown.</summary>
    public static string ToMarkdown(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        var text = WebUtility.HtmlDecode(html);

        // The site's own elements: a link carries its target, everything else is layout scaffolding
        // (columns, image placeholders, video) that means nothing in a text document.
        text = MioLink().Replace(text, "[$2]($1)");
        text = MioImage().Replace(text, m => $"\n(image {m.Groups[1].Value})\n");
        text = MioTag().Replace(text, string.Empty);

        // Tables carry the values worth auditing - the baseline shape scheme, the dark-theme
        // elevation overlays - so they become markdown tables rather than a column of loose lines.
        text = Table().Replace(text, m => TableToMarkdown(m.Groups[1].Value));

        text = Headings().Replace(text, m => $"\n{new string('#', int.Parse(m.Groups[1].Value) + 1)} {m.Groups[2].Value.Trim()}\n");
        text = Regex.Replace(text, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"</p>|</li>|</ul>|</ol>|</div>", "\n", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"<li[^>]*>", "- ", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"<(b|strong)>(.*?)</\1>", "**$2**", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        text = Regex.Replace(text, @"<(i|em)>(.*?)</\1>", "*$2*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        text = Regex.Replace(text, @"<code>(.*?)</code>", "`$1`", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        text = Regex.Replace(text, @"<a [^>]*href=""([^""]*)""[^>]*>(.*?)</a>", "[$2]($1)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        text = Regex.Replace(text, "<[^>]+>", string.Empty);

        return Collapse(text);
    }

    /// <summary>Normalises whitespace: trims every line and collapses runs of blank lines to one.</summary>
    public static string Collapse(string text)
    {
        var sb = new StringBuilder();
        var blank = true; // suppress leading blank lines
        foreach (var raw in text.Replace("\r\n", "\n").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0)
            {
                if (!blank) sb.Append('\n');
                blank = true;
                continue;
            }
            sb.Append(line).Append('\n');
            blank = false;
        }
        return sb.ToString().TrimEnd();
    }

    // A table whose cells hold their own paragraphs and <br>s; everything collapses to one line per
    // cell, because a markdown cell cannot hold a line break either.
    private static string TableToMarkdown(string body)
    {
        var rows = new List<List<string>>();
        foreach (Match row in TableRow().Matches(body))
        {
            var cells = TableCell().Matches(row.Groups[1].Value)
                .Select(c => Cell(c.Groups[2].Value))
                .ToList();
            if (cells.Count > 0) rows.Add(cells);
        }
        if (rows.Count == 0) return string.Empty;

        var width = rows.Max(r => r.Count);
        var sb = new StringBuilder("\n");
        for (var i = 0; i < rows.Count; i++)
        {
            var cells = rows[i];
            sb.Append("| ");
            for (var c = 0; c < width; c++) sb.Append(c < cells.Count ? cells[c] : string.Empty).Append(" | ");
            sb.Append('\n');
            if (i == 0) sb.Append("|").Append(string.Concat(Enumerable.Repeat("---|", width))).Append('\n');
        }
        return sb.Append('\n').ToString();
    }

    private static string Cell(string html)
    {
        var text = Regex.Replace(html, @"<br\s*/?>", " ", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"</p>\s*<p[^>]*>", " ", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, "<[^>]+>", string.Empty);
        text = WebUtility.HtmlDecode(text).Replace('\n', ' ').Replace("|", "\\|");
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    [GeneratedRegex(@"<table[^>]*>(.*?)</table>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Table();

    [GeneratedRegex(@"<tr[^>]*>(.*?)</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TableRow();

    [GeneratedRegex(@"<(td|th)[^>]*>(.*?)</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TableCell();

    [GeneratedRegex(@"<mio-link\s+link=""([^""]*)""[^>]*>(.*?)</mio-link>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex MioLink();

    [GeneratedRegex(@"<mio-image\s+index=(\d+)\s*>\s*</mio-image>", RegexOptions.IgnoreCase)]
    private static partial Regex MioImage();

    [GeneratedRegex(@"</?mio-[a-z-]+[^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex MioTag();

    [GeneratedRegex(@"<h([1-6])[^>]*>(.*?)</h\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex Headings();
}
