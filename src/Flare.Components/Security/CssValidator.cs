using System.Text.RegularExpressions;

namespace Flare.Components.Security;

internal static partial class CssValidator
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Strips dangerous CSS: expression(), url(javascript:), url(data:), behavior:, -moz-binding:.
    /// Returns null if input is null.
    /// </summary>
    public static string? StripDangerous(string? css)
    {
        if (css is null) return null;
        try
        {
            css = ExpressionRegex().Replace(css, string.Empty);
            css = UrlJavascriptRegex().Replace(css, "url(about:blank)");
            css = UrlDataRegex().Replace(css, "url(about:blank)");
            css = BehaviorRegex().Replace(css, string.Empty);
            css = MozBindingRegex().Replace(css, string.Empty);
            return css;
        }
        catch (RegexMatchTimeoutException) { return string.Empty; }
    }

    /// <summary>
    /// Sanitizes a CSS color value for safe insertion into an inline custom property.
    /// Allows hex, named colors, rgb()/hsl()/color-mix() and var(--flare-color-*) expressions;
    /// rejects values containing declaration/rule separators or dangerous functions. Returns null
    /// when empty or unsafe.
    /// </summary>
    public static string? SanitizeColor(string? color)
    {
        if (string.IsNullOrWhiteSpace(color)) return null;
        var c = color.Trim();
        try
        {
            if (!SafeColorRegex().IsMatch(c)) return null;       // whitelist chars only (no ; { } < > ")
        }
        catch (RegexMatchTimeoutException) { return null; }
        var stripped = StripDangerous(c);                         // belt-and-suspenders
        return string.IsNullOrWhiteSpace(stripped) ? null : stripped;
    }

    /// <summary>Returns true if href is safe to render in an anchor href or image src.</summary>
    public static bool IsHrefSafe(string? href) =>
        href is null || href.StartsWith('/') || href.StartsWith('#') ||
        href.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
        href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
        href.StartsWith("tel:", StringComparison.OrdinalIgnoreCase);

    /// <summary>Returns true if image src is safe (no javascript:/vbscript:, allowed data:image/*).</summary>
    public static bool IsImageSrcSafe(string? src) =>
        src is not null && (
            src.StartsWith('/') ||
            src.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            src.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            src.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) ||
            src.StartsWith("blob:", StringComparison.OrdinalIgnoreCase));

    [GeneratedRegex(@"^[#a-zA-Z0-9(),.%\s\-]+$")]
    private static partial Regex SafeColorRegex();

    [GeneratedRegex(@"expression\s*\(", RegexOptions.IgnoreCase)]
    private static partial Regex ExpressionRegex();

    [GeneratedRegex(@"url\s*\(\s*[""']?\s*javascript:", RegexOptions.IgnoreCase)]
    private static partial Regex UrlJavascriptRegex();

    [GeneratedRegex(@"url\s*\(\s*[""']?\s*data:(?!image/)", RegexOptions.IgnoreCase)]
    private static partial Regex UrlDataRegex();

    [GeneratedRegex(@"behavior\s*:", RegexOptions.IgnoreCase)]
    private static partial Regex BehaviorRegex();

    [GeneratedRegex(@"-moz-binding\s*:", RegexOptions.IgnoreCase)]
    private static partial Regex MozBindingRegex();
}
