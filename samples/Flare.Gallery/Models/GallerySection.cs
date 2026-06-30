namespace Flare.Gallery.Models;

/// <summary>
/// A top-level navigation section in the gallery's two-pane nav. Selecting a section (other than
/// <see cref="None"/>) opens the secondary drawer with that section's pages.
/// </summary>
public enum GallerySection
{
    /// <summary>No section selected; the secondary drawer is closed.</summary>
    None,
    /// <summary>Theming pages (overview, color, custom theme).</summary>
    Themes,
    /// <summary>Component demo pages, grouped by category.</summary>
    Components,
    /// <summary>Service / live-demo pages.</summary>
    Services,
    /// <summary>API reference pages.</summary>
    Api,
}

/// <summary>Maps a route to the navigation section it belongs to, so the primary rail can highlight
/// the section of the current page.</summary>
public static class GallerySections
{
    /// <summary>The section a path belongs to (<see cref="GallerySection.None"/> for top-level pages).</summary>
    public static GallerySection FromPath(string? path)
    {
        var p = "/" + (path ?? string.Empty).Trim('/').ToLowerInvariant();
        if (p.StartsWith("/components")) return GallerySection.Components;
        if (p.StartsWith("/services")) return GallerySection.Services;
        if (p == "/api" || p.StartsWith("/api/")) return GallerySection.Api;
        return p is "/theming" or "/color" or "/custom-theme" ? GallerySection.Themes : GallerySection.None;
    }
}
