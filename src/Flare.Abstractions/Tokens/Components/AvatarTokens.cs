using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Avatar component.
/// These control the geometry, sizing, and appearance of avatars.
/// </summary>
public sealed record AvatarTokens
{
    /// <summary>Background color of the avatar.</summary>
    [CssVar(AvatarField.SurfaceColor)] public string SurfaceColor { get; init; } = "var(--flare-color-primary-container)";

    /// <summary>Color of the avatar text/initials.</summary>
    [CssVar(AvatarField.TextColor)] public string TextColor { get; init; } = "var(--flare-color-on-primary-container)";

    /// <summary>Color of the avatar icon.</summary>
    [CssVar(AvatarField.IconColor)] public string IconColor { get; init; } = "var(--flare-color-on-primary-container)";

    /// <summary>Border radius for the rounded shape.</summary>
    [CssVar(AvatarField.RoundedRadius)] public string RoundedRadius { get; init; } = "var(--flare-shape-full)";

    /// <summary>Border radius for the square shape.</summary>
    [CssVar(AvatarField.SquareRadius)] public string SquareRadius { get; init; } = "var(--flare-shape-small)";

    /// <summary>Size of the xs variant.</summary>
    [CssVar(AvatarField.SizeXs)] public string SizeXs { get; init; } = "24px";

    /// <summary>Size of the sm variant.</summary>
    [CssVar(AvatarField.SizeSm)] public string SizeSm { get; init; } = "32px";

    /// <summary>Size of the md variant (default).</summary>
    [CssVar(AvatarField.SizeMd)] public string SizeMd { get; init; } = "40px";

    /// <summary>Size of the lg variant.</summary>
    [CssVar(AvatarField.SizeLg)] public string SizeLg { get; init; } = "48px";

    /// <summary>Size of the xl variant.</summary>
    [CssVar(AvatarField.SizeXl)] public string SizeXl { get; init; } = "64px";

    /// <summary>Font family of the avatar initials.</summary>
    [CssVar(AvatarField.FontFamily)] public string FontFamily { get; init; } = "var(--flare-typescale-label-large-font)";

    /// <summary>Font size of the avatar initials.</summary>
    [CssVar(AvatarField.FontSize)] public string FontSize { get; init; } = "var(--flare-typescale-label-large-size)";

    /// <summary>Font weight of the avatar initials.</summary>
    [CssVar(AvatarField.FontWeight)] public string FontWeight { get; init; } = "var(--flare-typescale-label-large-weight, 500)";

    /// <summary>Border width of the avatar group overlap.</summary>
    [CssVar(AvatarField.GroupBorderWidth)] public string GroupBorderWidth { get; init; } = "2px";

    /// <summary>Border color of the avatar group overlap.</summary>
    [CssVar(AvatarField.GroupBorderColor)] public string GroupBorderColor { get; init; } = "var(--flare-color-surface)";

    /// <summary>Background color of the overflow badge.</summary>
    [CssVar(AvatarField.GroupOverflowBg)] public string GroupOverflowBg { get; init; } = "var(--flare-color-surface-container-highest)";

    /// <summary>Color of the overflow badge text.</summary>
    [CssVar(AvatarField.GroupOverflowColor)] public string GroupOverflowColor { get; init; } = "var(--flare-color-on-surface-variant)";
}
