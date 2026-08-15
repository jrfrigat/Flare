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
    private readonly Dictionary<string, string> _docs = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Returns the theme's markdown, or null when the package ships no README.</summary>
    public string? Doc(string themeId)
    {
        if (string.IsNullOrWhiteSpace(themeId)) return null;
        if (_docs.TryGetValue(themeId, out var cached)) return cached;

        using var stream = typeof(ThemeDocService).Assembly.GetManifestResourceStream($"theme-{themeId}.md");
        if (stream is null) return null;

        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        _docs[themeId] = text;
        return text;
    }

    /// <summary>True when a theme has a mapping document to show.</summary>
    public bool Has(string themeId) => Doc(themeId) is not null;

    /// <summary>
    /// The markdown with its leading H1 and install/registration block removed - the page draws its own
    /// title and the reader is already inside the app, so "dotnet add package" is not the first thing
    /// they need. Everything from the first H2 onward is kept verbatim.
    /// </summary>
    public string? Body(string themeId)
    {
        var doc = Doc(themeId);
        if (doc is null) return null;

        var firstSection = doc.IndexOf("\n## ", StringComparison.Ordinal);
        return firstSection < 0 ? doc : doc[(firstSection + 1)..];
    }
}
