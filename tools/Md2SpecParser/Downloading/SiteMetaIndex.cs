using System.Text.Json;

namespace Flare.Tools.Md2SpecParser.Downloading;

/// <summary>
/// The site's route index. m2.material.io serves every page's content as JSON under
/// <c>/page-data/&lt;collection&gt;/&lt;document&gt;.json</c>, and only that numeric form answers - the route
/// itself returns the shell HTML. The mapping is public in <c>site_meta.js</c>, which is one
/// <c>window.site_meta = { ... }</c> assignment, so the tool reads routes from there rather than
/// asking the config to carry ids nobody can check by eye.
/// </summary>
public sealed class SiteMetaIndex
{
    private readonly Dictionary<string, string> _pageDataPaths;

    private SiteMetaIndex(Dictionary<string, string> pageDataPaths) => _pageDataPaths = pageDataPaths;

    /// <summary>Number of routes the index resolved.</summary>
    public int Count => _pageDataPaths.Count;

    /// <summary>Downloads and parses the route index.</summary>
    public static async Task<SiteMetaIndex> LoadAsync(HttpClient http, string siteMetaUrl, CancellationToken ct = default)
    {
        var script = await http.GetStringAsync(siteMetaUrl, ct);
        return Parse(script);
    }

    /// <summary>Parses the <c>window.site_meta = {...};</c> assignment.</summary>
    public static SiteMetaIndex Parse(string script)
    {
        var open = script.IndexOf('{');
        if (open < 0) throw new InvalidOperationException("site_meta.js carries no object literal.");

        var json = script[open..].TrimEnd();
        if (json.EndsWith(';')) json = json[..^1];

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("routes", out var routes))
            throw new InvalidOperationException("site_meta.js has no 'routes' map.");

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var route in routes.EnumerateObject())
        {
            if (!route.Value.TryGetProperty("reference", out var reference)) continue;
            if (!reference.TryGetProperty("collection_id", out var collection)) continue;
            if (!reference.TryGetProperty("document_id", out var document)) continue;

            var collectionId = collection.GetString();
            if (string.IsNullOrEmpty(collectionId)) continue;

            // document_id is a JSON number well inside long range; ToString keeps it exact.
            var documentId = document.ValueKind == JsonValueKind.Number
                ? document.GetInt64().ToString()
                : document.GetString();
            if (string.IsNullOrEmpty(documentId)) continue;

            map[Normalize(route.Name)] = $"/page-data/{collectionId}/{documentId}.json";
        }

        return new SiteMetaIndex(map);
    }

    /// <summary>The page-data path for a route, or null when the site does not publish that route.</summary>
    public string? PageDataPath(string route) =>
        _pageDataPaths.TryGetValue(Normalize(route), out var path) ? path : null;

    // Routes are stored with a leading slash and no trailing one; ".html" tails are how the
    // pre-2021 urls were written and still appear in older notes.
    private static string Normalize(string route)
    {
        var value = route.Trim();
        if (value.EndsWith(".html", StringComparison.OrdinalIgnoreCase)) value = value[..^5];
        if (!value.StartsWith('/')) value = '/' + value;
        if (value.Length > 1) value = value.TrimEnd('/');
        return value;
    }
}
