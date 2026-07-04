using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Card component. These control the geometry, color, and motion of cards.
/// Defaults follow Material Design 3 Expressive; other themes override the fields they need so each
/// variant looks native (see <c>FluentUI2Tokens</c>, <c>AeroTokens</c>).
/// </summary>
public sealed record CardTokens
{
    /// <summary>Background color of the elevated card variant.</summary>
    [CssVar(CardField.ElevatedBg)] public required string ElevatedBg { get; init; }

    /// <summary>Background color of the filled card variant.</summary>
    [CssVar(CardField.FilledBg)] public required string FilledBg { get; init; }

    /// <summary>Border of the filled card variant. Defaults to <c>none</c> (a borderless filled panel);
    /// set it to draw a delineating border around filled cards (e.g. the dense gray "1C" look).</summary>
    [CssVar(CardField.FilledBorder)] public required string FilledBorder { get; init; }

    /// <summary>Background color of the outlined card variant.</summary>
    [CssVar(CardField.OutlinedBg)] public required string OutlinedBg { get; init; }

    /// <summary>Border of the outlined card variant.</summary>
    [CssVar(CardField.OutlinedBorder)] public required string OutlinedBorder { get; init; }

    /// <summary>Background color of the tonal card variant.</summary>
    [CssVar(CardField.TonalBg)] public required string TonalBg { get; init; }

    /// <summary>Text color of the tonal card variant.</summary>
    [CssVar(CardField.TonalColor)] public required string TonalColor { get; init; }

    /// <summary>Text color of the text (transparent) card variant.</summary>
    [CssVar(CardField.TextColor)] public required string TextColor { get; init; }

    /// <summary>Border radius of the card (medium size). Small/large sizes scale from the shape scale.</summary>
    [CssVar(CardField.Radius)] public required string Radius { get; init; }

    /// <summary>Elevation (box-shadow) of the elevated variant.</summary>
    [CssVar(CardField.Elevation)] public required string Elevation { get; init; }

    /// <summary>Elevation on hover for clickable/elevated cards.</summary>
    [CssVar(CardField.ElevationHover)] public required string ElevationHover { get; init; }

    /// <summary>Border applied to a selected card (accent ring). Used when Selectable + Selected.</summary>
    [CssVar(CardField.SelectedBorder)] public required string SelectedBorder { get; init; }

    /// <summary>Background tint applied to a selected card.</summary>
    [CssVar(CardField.SelectedBg)] public required string SelectedBg { get; init; }

    /// <summary>Color of the hover/press state layer for interactive cards.</summary>
    [CssVar(CardField.StateLayer)] public required string StateLayer { get; init; }

    /// <summary>Inner padding at the top of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingTop)] public required string PaddingTop { get; init; }

    /// <summary>Inner padding at the right of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingRight)] public required string PaddingRight { get; init; }

    /// <summary>Inner padding at the bottom of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingBottom)] public required string PaddingBottom { get; init; }

    /// <summary>Inner padding at the left of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingLeft)] public required string PaddingLeft { get; init; }

    /// <summary>Padding inside the card content.</summary>
    [CssVar(CardField.ContentPadding)] public required string ContentPadding { get; init; }

    /// <summary>Padding inside the card header.</summary>
    [CssVar(CardField.HeaderPadding)] public required string HeaderPadding { get; init; }

    /// <summary>Padding inside the card footer.</summary>
    [CssVar(CardField.FooterPadding)] public required string FooterPadding { get; init; }

    /// <summary>Padding inside the card actions.</summary>
    [CssVar(CardField.ActionsPadding)] public required string ActionsPadding { get; init; }

    /// <summary>Gap between action buttons.</summary>
    [CssVar(CardField.ActionsGap)] public required string ActionsGap { get; init; }

    /// <summary>Border radius of the card media.</summary>
    [CssVar(CardField.MediaRadius)] public required string MediaRadius { get; init; }

    /// <summary>Color of the card title text.</summary>
    [CssVar(CardField.TitleColor)] public required string TitleColor { get; init; }

    /// <summary>Font family of the card title.</summary>
    [CssVar(CardField.TitleFontFamily)] public required string TitleFontFamily { get; init; }

    /// <summary>Font size of the card title.</summary>
    [CssVar(CardField.TitleFontSize)] public required string TitleFontSize { get; init; }

    /// <summary>Color of the card subtitle text.</summary>
    [CssVar(CardField.SubtitleColor)] public required string SubtitleColor { get; init; }

    /// <summary>Font family of the card subtitle.</summary>
    [CssVar(CardField.SubtitleFontFamily)] public required string SubtitleFontFamily { get; init; }

    /// <summary>Font size of the card subtitle.</summary>
    [CssVar(CardField.SubtitleFontSize)] public required string SubtitleFontSize { get; init; }

    /// <summary>Transition duration for hover effects.</summary>
    [CssVar(CardField.TransitionDuration)] public required string TransitionDuration { get; init; }

    /// <summary>Transition easing for hover effects.</summary>
    [CssVar(CardField.TransitionEasing)] public required string TransitionEasing { get; init; }
}
