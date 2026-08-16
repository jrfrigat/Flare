using System.Globalization;

namespace Flare.Gallery.Services;

/// <summary>
/// Serves each theme package's README - the design-system -> Flare component mapping - to the
/// <c>/themes/{id}</c> page.
/// </summary>
/// <remarks>
/// The README file itself is embedded, not a copy of it, so the page in the gallery and the README
/// shown on nuget.org are the same text and cannot drift. Read once and cached; the files never
/// change at runtime.
/// </remarks>
public sealed class ThemeDocService
{
    private readonly Dictionary<string, string?> _docs = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the theme's markdown in the given culture, or null when the package ships no README.
    /// A theme with no translation falls back to English rather than showing nothing.
    /// </summary>
    public string? Doc(string themeId, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(themeId)) return null;

        var ru = (culture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName == "ru";
        return ru
            ? Read($"theme-{themeId}.ru.md") ?? Read($"theme-{themeId}.md")
            : Read($"theme-{themeId}.md");
    }

    /// <summary>True when a theme has a mapping document to show (in any language).</summary>
    public bool Has(string themeId) => Read($"theme-{themeId}.md") is not null;

    /// <summary>
    /// The markdown with its leading H1 and install/registration block removed - the page draws its own
    /// title and the reader is already inside the app, so "dotnet add package" is not the first thing
    /// they need. Everything from the first H2 onward is kept verbatim.
    /// </summary>
    public string? Body(string themeId, CultureInfo? culture = null)
    {
        var doc = Doc(themeId, culture);
        if (doc is null) return null;

        var firstSection = doc.IndexOf("\n## ", StringComparison.Ordinal);
        return firstSection < 0 ? doc : doc[(firstSection + 1)..];
    }

    // Cached by logical name, misses included: a theme with no README must not re-probe the manifest
    // on every render.
    private string? Read(string logicalName)
    {
        if (_docs.TryGetValue(logicalName, out var cached)) return cached;

        using var stream = typeof(ThemeDocService).Assembly.GetManifestResourceStream(logicalName);
        string? text = null;
        if (stream is not null)
        {
            using var reader = new StreamReader(stream);
            text = reader.ReadToEnd();
        }

        _docs[logicalName] = text;
        return text;
    }
}
