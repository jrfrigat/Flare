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
    [CssVar(AvatarField.SurfaceColor)] public required string SurfaceColor { get; init; }

    /// <summary>Color of the avatar text/initials.</summary>
    [CssVar(AvatarField.TextColor)] public required string TextColor { get; init; }

    /// <summary>Color of the avatar icon.</summary>
    [CssVar(AvatarField.IconColor)] public required string IconColor { get; init; }

    /// <summary>Border radius for the rounded shape.</summary>
    [CssVar(AvatarField.RoundedRadius)] public required string RoundedRadius { get; init; }

    /// <summary>Border radius for the square shape.</summary>
    [CssVar(AvatarField.SquareRadius)] public required string SquareRadius { get; init; }

    /// <summary>Size of the xs variant.</summary>
    [CssVar(AvatarField.SizeXs)] public required string SizeXs { get; init; }

    /// <summary>Size of the sm variant.</summary>
    [CssVar(AvatarField.SizeSm)] public required string SizeSm { get; init; }

    /// <summary>Size of the md variant (default).</summary>
    [CssVar(AvatarField.SizeMd)] public required string SizeMd { get; init; }

    /// <summary>Size of the lg variant.</summary>
    [CssVar(AvatarField.SizeLg)] public required string SizeLg { get; init; }

    /// <summary>Size of the xl variant.</summary>
    [CssVar(AvatarField.SizeXl)] public required string SizeXl { get; init; }

    /// <summary>Font family of the avatar initials.</summary>
    [CssVar(AvatarField.FontFamily)] public required string FontFamily { get; init; }

    /// <summary>Font size of the avatar initials.</summary>
    [CssVar(AvatarField.FontSize)] public required string FontSize { get; init; }

    /// <summary>Font weight of the avatar initials.</summary>
    [CssVar(AvatarField.FontWeight)] public required string FontWeight { get; init; }

    /// <summary>Border width of the avatar group overlap.</summary>
    [CssVar(AvatarField.GroupBorderWidth)] public required string GroupBorderWidth { get; init; }

    /// <summary>Border color of the avatar group overlap.</summary>
    [CssVar(AvatarField.GroupBorderColor)] public required string GroupBorderColor { get; init; }

    /// <summary>Background color of the overflow badge.</summary>
    [CssVar(AvatarField.GroupOverflowBg)] public required string GroupOverflowBg { get; init; }

    /// <summary>Color of the overflow badge text.</summary>
    [CssVar(AvatarField.GroupOverflowColor)] public required string GroupOverflowColor { get; init; }
}
