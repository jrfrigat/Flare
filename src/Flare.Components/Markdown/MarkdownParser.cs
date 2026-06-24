using System.Text;
using System.Text.RegularExpressions;

namespace Flare.Components;

/// <summary>Pure C# line-by-line Markdown -> HTML renderer. No JavaScript, no external packages.</summary>
public static partial class MarkdownParser
{
    #region Public API

    /// <summary>Converts <paramref name="markdown"/> to an HTML string.</summary>
    /// <param name="markdown">Markdown source text.</param>
    /// <param name="sanitize">When <c>true</c>, strips dangerous patterns (script, javascript:, on*= handlers).</param>
    public static string ToHtml(string? markdown, bool sanitize = true)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var lines = markdown.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var html = RenderLines(lines);

        return sanitize ? Sanitize(html) : html;
    }

    #endregion

    #region Line Rendering

    private static string RenderLines(string[] lines)
    {
        var sb = new StringBuilder();
        int i = 0;

        while (i < lines.Length)
        {
            var line = lines[i];

            // Fenced code block
            if (line.TrimStart().StartsWith("```"))
            {
                i = RenderFencedCode(lines, i, sb);
                continue;
            }

            // Heading
            if (TryRenderHeading(line, sb))
            {
                i++;
                continue;
            }

            // Horizontal rule
            if (IsHorizontalRule(line))
            {
                sb.AppendLine("<hr>");
                i++;
                continue;
            }

            // Blockquote
            if (line.TrimStart().StartsWith("> ") || line.TrimStart() == ">")
            {
                i = RenderBlockquote(lines, i, sb);
                continue;
            }

            // Unordered list
            if (IsUnorderedListItem(line))
            {
                i = RenderUnorderedList(lines, i, sb);
                continue;
            }

            // Ordered list
            if (IsOrderedListItem(line))
            {
                i = RenderOrderedList(lines, i, sb);
                continue;
            }

            // Table
            if (i + 1 < lines.Length && IsTableSeparator(lines[i + 1]))
            {
                i = RenderTable(lines, i, sb);
                continue;
            }

            // Blank line - paragraph separator
            if (string.IsNullOrWhiteSpace(line))
            {
                i++;
                continue;
            }

            // Paragraph
            i = RenderParagraph(lines, i, sb);
        }

        return sb.ToString();
    }

    #endregion

    #region Block Elements

    private static bool TryRenderHeading(string line, StringBuilder sb)
    {
        var trimmed = line.TrimStart();
        int level = 0;
        while (level < trimmed.Length && trimmed[level] == '#') level++;
        if (level is < 1 or > 4) return false;
        if (level >= trimmed.Length || trimmed[level] != ' ') return false;

        var text = trimmed[(level + 1)..].Trim();
        sb.AppendLine($"<h{level}>{RenderInline(text)}</h{level}>");
        return true;
    }

    private static bool IsHorizontalRule(string line)
    {
        var t = line.Trim();
        return t is "---" or "***" or "___"
            || (t.Length >= 3 && t.All(c => c == '-'))
            || (t.Length >= 3 && t.All(c => c == '*'))
            || (t.Length >= 3 && t.All(c => c == '_'));
    }

    private static int RenderFencedCode(string[] lines, int start, StringBuilder sb)
    {
        var fence = lines[start].TrimStart();
        var lang = fence.Length > 3 ? fence[3..].Trim() : string.Empty;
        var langAttr = string.IsNullOrEmpty(lang) ? string.Empty : $" class=\"language-{HtmlEncode(lang)}\"";

        sb.Append($"<pre><code{langAttr}>");
        int i = start + 1;
        bool first = true;
        while (i < lines.Length)
        {
            var l = lines[i];
            if (l.TrimStart().StartsWith("```"))
            {
                i++;
                break;
            }
            if (!first) sb.Append('\n');
            sb.Append(HtmlEncode(l));
            first = false;
            i++;
        }
        sb.AppendLine("</code></pre>");
        return i;
    }

    private static int RenderBlockquote(string[] lines, int start, StringBuilder sb)
    {
        var content = new List<string>();
        int i = start;
        while (i < lines.Length)
        {
            var l = lines[i].TrimStart();
            if (l.StartsWith("> "))
                content.Add(l[2..]);
            else if (l == ">")
                content.Add(string.Empty);
            else
                break;
            i++;
        }

        sb.AppendLine("<blockquote>");
        var inner = RenderLines(content.ToArray());
        sb.Append(inner);
        sb.AppendLine("</blockquote>");
        return i;
    }

    private static bool IsUnorderedListItem(string line)
    {
        var t = line.TrimStart();
        return (t.StartsWith("- ") || t.StartsWith("* ")) && t.Length > 2;
    }

    private static int RenderUnorderedList(string[] lines, int start, StringBuilder sb)
    {
        sb.AppendLine("<ul>");
        int i = start;
        while (i < lines.Length && IsUnorderedListItem(lines[i]))
        {
            var t = lines[i].TrimStart()[2..];
            sb.AppendLine($"  <li>{RenderInline(t)}</li>");
            i++;
        }
        sb.AppendLine("</ul>");
        return i;
    }

    private static bool IsOrderedListItem(string line)
    {
        var t = line.TrimStart();
        var dot = t.IndexOf(". ", StringComparison.Ordinal);
        if (dot <= 0) return false;
        return t[..dot].All(char.IsDigit);
    }

    private static int RenderOrderedList(string[] lines, int start, StringBuilder sb)
    {
        sb.AppendLine("<ol>");
        int i = start;
        while (i < lines.Length && IsOrderedListItem(lines[i]))
        {
            var t = lines[i].TrimStart();
            var dot = t.IndexOf(". ", StringComparison.Ordinal);
            var text = t[(dot + 2)..];
            sb.AppendLine($"  <li>{RenderInline(text)}</li>");
            i++;
        }
        sb.AppendLine("</ol>");
        return i;
    }

    private static bool IsTableSeparator(string line)
    {
        var t = line.Trim();
        if (!t.StartsWith('|') || !t.EndsWith('|')) return false;
        return t.Replace("|", string.Empty).Replace("-", string.Empty).Replace(":", string.Empty).Trim().Length == 0;
    }

    private static int RenderTable(string[] lines, int start, StringBuilder sb)
    {
        // Header row
        var headers = SplitTableRow(lines[start]);
        // Skip separator (start+1)
        sb.AppendLine("<table>");
        sb.AppendLine("  <thead><tr>");
        foreach (var h in headers)
            sb.AppendLine($"    <th>{RenderInline(h.Trim())}</th>");
        sb.AppendLine("  </tr></thead>");
        sb.AppendLine("  <tbody>");

        int i = start + 2;
        while (i < lines.Length && lines[i].Trim().StartsWith('|'))
        {
            var cells = SplitTableRow(lines[i]);
            sb.AppendLine("    <tr>");
            foreach (var c in cells)
                sb.AppendLine($"      <td>{RenderInline(c.Trim())}</td>");
            sb.AppendLine("    </tr>");
            i++;
        }

        sb.AppendLine("  </tbody>");
        sb.AppendLine("</table>");
        return i;
    }

    private static string[] SplitTableRow(string line)
    {
        var t = line.Trim();
        if (t.StartsWith('|')) t = t[1..];
        if (t.EndsWith('|')) t = t[..^1];
        return t.Split('|');
    }

    private static int RenderParagraph(string[] lines, int start, StringBuilder sb)
    {
        var paragraphLines = new List<string>();
        int i = start;
        while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i])
               && !TryRenderHeading(lines[i], new StringBuilder())
               && !IsHorizontalRule(lines[i])
               && !IsUnorderedListItem(lines[i])
               && !IsOrderedListItem(lines[i])
               && !lines[i].TrimStart().StartsWith("```")
               && !lines[i].TrimStart().StartsWith("> "))
        {
            // Stop if next line looks like a table separator
            if (i + 1 < lines.Length && IsTableSeparator(lines[i + 1]))
                break;
            paragraphLines.Add(lines[i]);
            i++;
        }

        if (paragraphLines.Count > 0)
        {
            var text = string.Join(" ", paragraphLines);
            sb.AppendLine($"<p>{RenderInline(text)}</p>");
        }
        return i;
    }

    #endregion

    #region Inline Rendering

    private static string RenderInline(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // Process inline elements in order of precedence (longest patterns first)
        var result = new StringBuilder();
        int pos = 0;

        while (pos < text.Length)
        {
            // Image: ![alt](src)
            if (pos < text.Length - 1 && text[pos] == '!' && text[pos + 1] == '[')
            {
                int end = TryMatchImageOrLink(text, pos + 1, out var inner, out var href);
                if (end > pos)
                {
                    var safeSrc = IsImageSrcSafe(href) ? href : string.Empty;
                    result.Append($"<img alt=\"{HtmlEncode(inner)}\" src=\"{HtmlEncode(safeSrc)}\">");
                    pos = end;
                    continue;
                }
            }

            // Link: [text](url)
            if (text[pos] == '[')
            {
                int end = TryMatchImageOrLink(text, pos, out var linkText, out var href);
                if (end > pos)
                {
                    result.Append($"<a href=\"{HtmlEncode(href)}\" target=\"_blank\" rel=\"noopener noreferrer\">{RenderInline(linkText)}</a>");
                    pos = end;
                    continue;
                }
            }

            // Inline code: `code`
            if (text[pos] == '`')
            {
                int close = text.IndexOf('`', pos + 1);
                if (close > pos)
                {
                    result.Append($"<code>{HtmlEncode(text[(pos + 1)..close])}</code>");
                    pos = close + 1;
                    continue;
                }
            }

            // Bold+Italic: ***text*** or ___text___
            if (pos + 2 < text.Length &&
                ((text[pos] == '*' && text[pos + 1] == '*' && text[pos + 2] == '*') ||
                 (text[pos] == '_' && text[pos + 1] == '_' && text[pos + 2] == '_')))
            {
                char marker = text[pos];
                string tag3 = new string(marker, 3);
                int close = text.IndexOf(tag3, pos + 3, StringComparison.Ordinal);
                if (close > pos)
                {
                    result.Append($"<strong><em>{HtmlEncode(text[(pos + 3)..close])}</em></strong>");
                    pos = close + 3;
                    continue;
                }
            }

            // Bold: **text** or __text__
            if (pos + 1 < text.Length &&
                ((text[pos] == '*' && text[pos + 1] == '*') ||
                 (text[pos] == '_' && text[pos + 1] == '_')))
            {
                char marker = text[pos];
                string tag2 = new string(marker, 2);
                int close = text.IndexOf(tag2, pos + 2, StringComparison.Ordinal);
                if (close > pos)
                {
                    result.Append($"<strong>{HtmlEncode(text[(pos + 2)..close])}</strong>");
                    pos = close + 2;
                    continue;
                }
            }

            // Italic: *text* or _text_
            if (text[pos] == '*' || text[pos] == '_')
            {
                char marker = text[pos];
                int close = text.IndexOf(marker, pos + 1);
                if (close > pos)
                {
                    result.Append($"<em>{HtmlEncode(text[(pos + 1)..close])}</em>");
                    pos = close + 1;
                    continue;
                }
            }

            result.Append(HtmlEncode(text[pos].ToString()));
            pos++;
        }

        return result.ToString();
    }

    /// <summary>
    /// Tries to match [text](url) starting at <paramref name="start"/>.
    /// Returns the position after the closing ')' on success, or <paramref name="start"/> on failure.
    /// </summary>
    private static int TryMatchImageOrLink(string text, int start, out string inner, out string href)
    {
        inner = string.Empty;
        href = string.Empty;

        int bracketClose = text.IndexOf(']', start + 1);
        if (bracketClose < 0) return start;
        if (bracketClose + 1 >= text.Length || text[bracketClose + 1] != '(') return start;

        int parenClose = text.IndexOf(')', bracketClose + 2);
        if (parenClose < 0) return start;

        inner = text[(start + 1)..bracketClose];
        href = text[(bracketClose + 2)..parenClose];
        return parenClose + 1;
    }

    #endregion

    #region Security

    private static string Sanitize(string html)
    {
        // Remove <script> blocks
        html = ScriptTagRegex().Replace(html, string.Empty);

        // Remove javascript: in href/src
        html = JavascriptProtocolRegex().Replace(html, "about:blank");

        // Remove on* event attributes (onclick, onload, etc.)
        html = OnEventAttributeRegex().Replace(html, string.Empty);

        // Block vbscript: protocol
        html = VbscriptProtocolRegex().Replace(html, "about:blank");

        // Block dangerous data: URIs (allow data:image/* but block data:text/html, data:application/*)
        html = DangerousDataUriRegex().Replace(html, "about:blank");

        // Strip CSS expression() (IE legacy XSS vector)
        html = CssExpressionRegex().Replace(html, string.Empty);

        // Remove dangerous HTML tags
        html = DangerousTagsRegex().Replace(html, string.Empty);

        return html;
    }

    [GeneratedRegex(@"<script[\s\S]*?</script>", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptTagRegex();

    [GeneratedRegex(@"(?i)(href|src)\s*=\s*[""']?\s*javascript:", RegexOptions.IgnoreCase)]
    private static partial Regex JavascriptProtocolRegex();

    [GeneratedRegex(@"\s+on\w+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]*)", RegexOptions.IgnoreCase)]
    private static partial Regex OnEventAttributeRegex();

    [GeneratedRegex(@"(?i)(href|src)\s*=\s*[""']?\s*vbscript:", RegexOptions.IgnoreCase)]
    private static partial Regex VbscriptProtocolRegex();

    [GeneratedRegex(@"(?i)(href|src)\s*=\s*[""']?\s*data:\s*(?!image/)", RegexOptions.IgnoreCase)]
    private static partial Regex DangerousDataUriRegex();

    [GeneratedRegex(@"(?i)expression\s*\(", RegexOptions.IgnoreCase)]
    private static partial Regex CssExpressionRegex();

    [GeneratedRegex(@"<(?:iframe|object|embed|form|base|meta|link)\b[^>]*>[\s\S]*?</(?:iframe|object|embed|form|base|meta|link)>|<(?:iframe|object|embed|form|base|meta|link)\b[^>]*/?>", RegexOptions.IgnoreCase)]
    private static partial Regex DangerousTagsRegex();

    #endregion

    #region Helpers

    private static string HtmlEncode(string text) =>
        System.Web.HttpUtility.HtmlEncode(text);

    private static bool IsImageSrcSafe(string src)
    {
        if (string.IsNullOrEmpty(src)) return true;
        var lower = src.TrimStart().ToLowerInvariant();
        return !lower.StartsWith("javascript:") && !lower.StartsWith("vbscript:")
            && (!lower.StartsWith("data:") || lower.StartsWith("data:image/"));
    }

    #endregion
}
