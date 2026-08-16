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

    /// <summary>
    /// Returns true if href is safe to render in an anchor href. Relative references - "about",
    /// "", "./x", "../x", "/x", "#x", "?q=1" - are always safe; an absolute URL is safe only when
    /// its scheme cannot execute script.
    /// </summary>
    /// <remarks>
    /// This deliberately blocks by SCHEME rather than allow-listing URL shapes. An allow-list that
    /// required a leading '/' rejected every base-relative link, which is how a Blazor app must
    /// write its internal links to survive being hosted under a sub-path (GitHub Pages serves the
    /// gallery from /Flare/, and "/x" resolves against the origin, ignoring &lt;base href&gt;).
    /// </remarks>
    public static bool IsHrefSafe(string? href) => IsUrlSafe(href, AllowedHrefSchemes);

    /// <summary>Returns true if image src is safe (no javascript:/vbscript:, allowed data:image/*).</summary>
    public static bool IsImageSrcSafe(string? src) =>
        src is not null && IsUrlSafe(src, AllowedImageSchemes) &&
        // "data:" is allowed only for images, never for the script-bearing payload types.
        (!StartsWithScheme(src, "data") || src.TrimStart().StartsWith("data:image/", StringComparison.OrdinalIgnoreCase));

    private static readonly string[] AllowedHrefSchemes = ["http", "https", "mailto", "tel"];
    private static readonly string[] AllowedImageSchemes = ["http", "https", "data", "blob"];

    // Longest scheme we accept is "mailto" (6); anything longer cannot match the allow-list.
    private const int MaxSchemeLength = 8;

    /// <summary>
    /// True when <paramref name="url"/> is a relative reference, or an absolute URL whose scheme is
    /// in <paramref name="allowed"/>. Null is treated as "not set" and is safe.
    /// </summary>
    private static bool IsUrlSafe(string? url, string[] allowed)
    {
        if (url is null) return true;

        var value = url.AsSpan();
        // Browsers ignore leading whitespace and control characters when parsing a URL's scheme, so
        // "\n javascript:alert(1)" still runs. Skip them before looking for the ':' as well.
        var start = 0;
        while (start < value.Length && IsIgnorable(value[start])) start++;
        value = value[start..];
        if (value.IsEmpty) return true;             // "" resolves to the base URL - a valid home link

        // A ':' only introduces a scheme when it comes before any '/', '?' or '#'; in "a/b:c" the
        // colon belongs to the path, so the whole thing is a relative reference.
        var colon = -1;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (ch is '/' or '?' or '#') break;
            if (ch == ':') { colon = i; break; }
        }
        if (colon < 0) return true;                 // no scheme -> relative reference -> safe
        if (colon > MaxSchemeLength) return false;  // absolute, and too long to be an allowed scheme

        Span<char> scheme = stackalloc char[MaxSchemeLength];
        var length = 0;
        for (var i = 0; i < colon; i++)
        {
            // Interior whitespace/control characters are stripped too: "java\tscript:" is a
            // javascript: URL as far as the browser is concerned.
            if (IsIgnorable(value[i])) continue;
            if (length == scheme.Length) return false;
            scheme[length++] = value[i];
        }

        ReadOnlySpan<char> parsed = scheme[..length];
        foreach (var candidate in allowed)
            if (parsed.Equals(candidate.AsSpan(), StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }

    /// <summary>True if <paramref name="url"/> begins with the given scheme followed by ':'.</summary>
    private static bool StartsWithScheme(string url, string scheme)
    {
        var value = url.AsSpan().TrimStart();
        return value.Length > scheme.Length && value[scheme.Length] == ':' &&
               value[..scheme.Length].Equals(scheme, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsIgnorable(char c) => char.IsWhiteSpace(c) || char.IsControl(c);

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
