namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Avatar component.
/// These control the geometry, sizing, and appearance of avatars.
/// </summary>
public sealed record AvatarTokens
{
    /// <summary>Background color of the avatar.</summary>
    public string SurfaceColor { get; init; } = "var(--flare-color-primary-container)";

    /// <summary>Color of the avatar text/initials.</summary>
    public string TextColor { get; init; } = "var(--flare-color-on-primary-container)";

    /// <summary>Color of the avatar icon.</summary>
    public string IconColor { get; init; } = "var(--flare-color-on-primary-container)";

    /// <summary>Border radius for the rounded shape.</summary>
    public string RoundedRadius { get; init; } = "var(--flare-shape-full)";

    /// <summary>Border radius for the square shape.</summary>
    public string SquareRadius { get; init; } = "var(--flare-shape-small)";

    /// <summary>Size of the xs variant.</summary>
    public string SizeXs { get; init; } = "24px";

    /// <summary>Size of the sm variant.</summary>
    public string SizeSm { get; init; } = "32px";

    /// <summary>Size of the md variant (default).</summary>
    public string SizeMd { get; init; } = "40px";

    /// <summary>Size of the lg variant.</summary>
    public string SizeLg { get; init; } = "48px";

    /// <summary>Size of the xl variant.</summary>
    public string SizeXl { get; init; } = "64px";

    /// <summary>Font family of the avatar initials.</summary>
    public string FontFamily { get; init; } = "var(--flare-typescale-label-large-font)";

    /// <summary>Font size of the avatar initials.</summary>
    public string FontSize { get; init; } = "var(--flare-typescale-label-large-size)";

    /// <summary>Font weight of the avatar initials.</summary>
    public string FontWeight { get; init; } = "var(--flare-typescale-label-large-weight, 500)";

    /// <summary>Border width of the avatar group overlap.</summary>
    public string GroupBorderWidth { get; init; } = "2px";

    /// <summary>Border color of the avatar group overlap.</summary>
    public string GroupBorderColor { get; init; } = "var(--flare-color-surface)";

    /// <summary>Background color of the overflow badge.</summary>
    public string GroupOverflowBg { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Color of the overflow badge text.</summary>
    public string GroupOverflowColor { get; init; } = "var(--flare-color-on-surface-variant)";
}
