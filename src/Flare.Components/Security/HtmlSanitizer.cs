using System.Text.RegularExpressions;

namespace Flare.Components.Security;

internal static partial class HtmlSanitizer
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
        { "b", "i", "em", "strong", "u", "s", "br", "span", "p" };

    /// <summary>Strips all HTML tags not in the AllowedTags allowlist.</summary>
    public static string StripTags(string? html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;
        return AllTagsRegex().Replace(html, m =>
        {
            // Group 1 captures the tag name (without leading /), so TrimStart is not needed
            var tag = m.Groups[1].Value.Split(' ', '\t')[0];
            return AllowedTags.Contains(tag) ? m.Value : string.Empty;
        });
    }

    /// <summary>
    /// Full sanitize: removes script/on* handlers, javascript:/vbscript: URIs,
    /// data:(non-image) URIs, CSS expressions, dangerous tags (iframe/object/embed/form/base/meta/link).
    /// </summary>
    public static string Sanitize(string? html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;
        html = DangerousTagsRegex().Replace(html, string.Empty);
        html = ScriptTagRegex().Replace(html, string.Empty);
        html = OnHandlerAttrRegex().Replace(html, string.Empty);
        html = JavascriptHrefRegex().Replace(html, "about:blank");
        html = VbscriptHrefRegex().Replace(html, "about:blank");
        html = DataNonImageHrefRegex().Replace(html, "about:blank");
        html = CssExpressionRegex().Replace(html, string.Empty);
        return html;
    }

    // Matches opening and closing tags: <tag ...> and </tag ...>
    [GeneratedRegex(@"</?(\w[^>]*)>", RegexOptions.IgnoreCase)]
    private static partial Regex AllTagsRegex();

    [GeneratedRegex(@"<(iframe|object|embed|form|base|meta|link)[\s\S]*?>", RegexOptions.IgnoreCase)]
    private static partial Regex DangerousTagsRegex();

    [GeneratedRegex(@"<script[\s\S]*?</script>", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptTagRegex();

    // Strips on* event handlers with double-quoted, single-quoted, or unquoted values
    [GeneratedRegex(@"\s+on\w+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]*)", RegexOptions.IgnoreCase)]
    private static partial Regex OnHandlerAttrRegex();

    [GeneratedRegex(@"(?i)(href|src|action)\s*=\s*[""']?\s*javascript:", RegexOptions.IgnoreCase)]
    private static partial Regex JavascriptHrefRegex();

    [GeneratedRegex(@"(?i)(href|src|action)\s*=\s*[""']?\s*vbscript:", RegexOptions.IgnoreCase)]
    private static partial Regex VbscriptHrefRegex();

    [GeneratedRegex(@"(?i)(href|src|action)\s*=\s*[""']?\s*data:(?!image/)", RegexOptions.IgnoreCase)]
    private static partial Regex DataNonImageHrefRegex();

    [GeneratedRegex(@"(?i)style\s*=\s*[""'][^""']*expression\s*\(", RegexOptions.IgnoreCase)]
    private static partial Regex CssExpressionRegex();
}
