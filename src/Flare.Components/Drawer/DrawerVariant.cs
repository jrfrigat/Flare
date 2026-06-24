namespace Flare.Components;

/// <summary>Display variant for <see cref="FlareDrawer"/>.</summary>
public enum DrawerVariant
{
    /// <summary>Slides in over content with a scrim backdrop. Default.</summary>
    Temporary,
    /// <summary>Always visible alongside content; no scrim, no transform animation.</summary>
    Permanent,
    /// <summary>Pushes content aside when open; collapses to zero width when closed.</summary>
    Persistent,
    /// <summary>Icon-only width by default; expands to full width on hover.</summary>
    Mini,
    /// <summary>Behaves as Persistent on large screens and Temporary on small screens.</summary>
    Responsive,
}
