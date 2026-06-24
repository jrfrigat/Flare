using Flare.Gallery.Api;
using Flare.Gallery.Models;
using Flare.Gallery.Resources;
using System.Globalization;

namespace Flare.Gallery.Services;

/// <summary>The kind of destination a <see cref="SearchResult"/> points to.</summary>
public enum SearchKind
{
    /// <summary>A top-level gallery page (Getting Started, Theming, ...).</summary>
    Page,
    /// <summary>A component demo page.</summary>
    Component,
    /// <summary>An API reference page for a type.</summary>
    Api,
}

/// <summary>One global-search hit, resolved for display in the current UI language.</summary>
/// <param name="Href">Route to navigate to.</param>
/// <param name="Title">Display title in the current language.</param>
/// <param name="Kind">The destination kind.</param>
/// <param name="KindLabel">Localized label for <paramref name="Kind"/> (used as the group header).</param>
/// <param name="Icon">Material Symbols icon name.</param>
public sealed record SearchResult(string Href, string Title, SearchKind Kind, string KindLabel, string Icon);

/// <summary>
/// Builds and queries the gallery's global search index. Every entry carries both its English and
/// Russian names (pulled from the <c>GalleryStrings</c> resources for both cultures), so a query in
/// either language matches regardless of the current UI language. The index is built once from
/// <see cref="ComponentCatalog"/> (components), a fixed set of top-level pages, and the generated
/// <see cref="ComponentApiRegistry"/> (API types).
/// </summary>
public sealed class GallerySearchService
{
    private sealed record Entry(string Href, string? TitleKey, string Name, SearchKind Kind, string Icon, string Haystack);

    private static readonly CultureInfo _en = new("en");
    private static readonly CultureInfo _ru = new("ru");

    private readonly IReadOnlyList<Entry> _entries = Build();

    /// <summary>Returns up to <paramref name="max"/> matches for <paramref name="query"/>, best first.</summary>
    public IReadOnlyList<SearchResult> Search(string query, int max = 12)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<SearchResult>();

        var q = query.Trim().ToLowerInvariant();

        return _entries
            .Select(e => (e, score: Score(e, q)))
            .Where(t => t.score > 0)
            .OrderByDescending(t => t.score)
            .ThenBy(t => Display(t.e), StringComparer.CurrentCultureIgnoreCase)
            .Take(max)
            .Select(t => new SearchResult(t.e.Href, Display(t.e), t.e.Kind, KindLabel(t.e.Kind), t.e.Icon))
            .ToList();
    }

    // Higher score = better match. Prefix beats word-prefix beats substring; falls back to the
    // bilingual haystack (covers the route and the non-display language).
    private static int Score(Entry e, string q)
    {
        var name = e.Name.ToLowerInvariant();
        if (name.StartsWith(q, StringComparison.Ordinal)) return 4;
        if (e.Haystack.Contains(" " + q, StringComparison.Ordinal)) return 3; // word-start in either language
        if (name.Contains(q, StringComparison.Ordinal)) return 2;
        if (e.Haystack.Contains(q, StringComparison.Ordinal)) return 1;
        return 0;
    }

    // Resolved in the current UI language (GalleryStrings.Culture); falls back to the English name.
    private static string Display(Entry e) =>
        e.TitleKey is not null
            ? GalleryStrings.ResourceManager.GetString(e.TitleKey) ?? e.Name
            : e.Name;

    private static string KindLabel(SearchKind kind) => kind switch
    {
        SearchKind.Page => GalleryStrings.Search_KindPage,
        SearchKind.Api => GalleryStrings.Search_KindApi,
        _ => GalleryStrings.Search_KindComponent,
    };

    private static string Loc(string key, CultureInfo ci) =>
        GalleryStrings.ResourceManager.GetString(key, ci) ?? string.Empty;

    private static List<Entry> Build()
    {
        var list = new List<Entry>();

        // -- Top-level pages (English name = the resource key resolved for en). ----
        AddKeyed(list, "/", "Nav_Overview", SearchKind.Page, "home");
        AddKeyed(list, "/getting-started", "Nav_GettingStarted", SearchKind.Page, "rocket_launch");
        AddKeyed(list, "/theming", "Nav_Theming", SearchKind.Page, "palette");
        AddKeyed(list, "/color", "Nav_Color", SearchKind.Page, "format_color_fill");
        // Custom Theme has no localized nav label (English-only literal in the menu).
        AddLiteral(list, "/custom-theme", "Custom Theme", SearchKind.Page, "tune");
        AddKeyed(list, "/about", "Nav_About", SearchKind.Page, "info");

        // -- Components (English name from the catalog; RU/EN titles from resources). --
        foreach (var c in ComponentCatalog.All)
        {
            var titleEn = c.TitleKey is not null ? Loc(c.TitleKey, _en) : null;
            var titleRu = c.TitleKey is not null ? Loc(c.TitleKey, _ru) : null;
            var haystack = Hay(c.Name, titleEn, titleRu, c.Href);
            list.Add(new Entry(c.Href, c.TitleKey, c.Name, SearchKind.Component, "widgets", haystack));
        }

        // -- API type pages (type names only; English). ---------------------------
        foreach (var name in ComponentApiRegistry.Components.Keys)
            list.Add(new Entry($"/api/{name}", null, name, SearchKind.Api, "data_object", Hay(name, null, null, name)));

        return list;
    }

    private static void AddKeyed(List<Entry> list, string href, string key, SearchKind kind, string icon)
    {
        var nameEn = Loc(key, _en);
        var nameRu = Loc(key, _ru);
        list.Add(new Entry(href, key, string.IsNullOrEmpty(nameEn) ? key : nameEn, kind, icon, Hay(nameEn, nameEn, nameRu, href)));
    }

    private static void AddLiteral(List<Entry> list, string href, string name, SearchKind kind, string icon) =>
        list.Add(new Entry(href, null, name, kind, icon, Hay(name, null, null, href)));

    // Lower-cased " name en ru href " with leading/trailing spaces so " " + q can detect word starts.
    private static string Hay(string name, string? en, string? ru, string href) =>
        (" " + string.Join(' ', new[] { name, en, ru, href }.Where(s => !string.IsNullOrEmpty(s))) + " ")
            .ToLowerInvariant();
}
