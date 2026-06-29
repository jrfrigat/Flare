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
