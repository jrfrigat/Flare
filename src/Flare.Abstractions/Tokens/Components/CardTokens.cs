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
    [CssVar(CardField.ElevatedBg)] public string ElevatedBg { get; init; } = Vars.Var(Color.SurfaceContainerLow);

    /// <summary>Background color of the filled card variant.</summary>
    [CssVar(CardField.FilledBg)] public string FilledBg { get; init; } = Vars.Var(Color.SurfaceContainerHighest);

    /// <summary>Border of the filled card variant. Defaults to <c>none</c> (a borderless filled panel);
    /// set it to draw a delineating border around filled cards (e.g. the dense gray "1C" look).</summary>
    [CssVar(CardField.FilledBorder)] public string FilledBorder { get; init; } = "none";

    /// <summary>Background color of the outlined card variant.</summary>
    [CssVar(CardField.OutlinedBg)] public string OutlinedBg { get; init; } = Vars.Var(Color.Surface);

    /// <summary>Border of the outlined card variant.</summary>
    [CssVar(CardField.OutlinedBorder)] public string OutlinedBorder { get; init; } = "1px solid var(--flare-color-outline-variant)";

    /// <summary>Background color of the tonal card variant.</summary>
    [CssVar(CardField.TonalBg)] public string TonalBg { get; init; } = Vars.Var(Color.SecondaryContainer);

    /// <summary>Text color of the tonal card variant.</summary>
    [CssVar(CardField.TonalColor)] public string TonalColor { get; init; } = Vars.Var(Color.OnSecondaryContainer);

    /// <summary>Text color of the text (transparent) card variant.</summary>
    [CssVar(CardField.TextColor)] public string TextColor { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Border radius of the card (medium size). Small/large sizes scale from the shape scale.</summary>
    [CssVar(CardField.Radius)] public string Radius { get; init; } = Vars.Var(Shape.Medium);

    /// <summary>Elevation (box-shadow) of the elevated variant.</summary>
    [CssVar(CardField.Elevation)] public string Elevation { get; init; } = Vars.Var(Flare.Css.Tokens.Elevation.Level1);

    /// <summary>Elevation on hover for clickable/elevated cards.</summary>
    [CssVar(CardField.ElevationHover)] public string ElevationHover { get; init; } = Vars.Var(Flare.Css.Tokens.Elevation.Level2);

    /// <summary>Border applied to a selected card (accent ring). Used when Selectable + Selected.</summary>
    [CssVar(CardField.SelectedBorder)] public string SelectedBorder { get; init; } = "2px solid var(--flare-color-primary)";

    /// <summary>Background tint applied to a selected card.</summary>
    [CssVar(CardField.SelectedBg)] public string SelectedBg { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)";

    /// <summary>Color of the hover/press state layer for interactive cards.</summary>
    [CssVar(CardField.StateLayer)] public string StateLayer { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Inner padding at the top of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingTop)] public string PaddingTop { get; init; } = "0";

    /// <summary>Inner padding at the right of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingRight)] public string PaddingRight { get; init; } = "0";

    /// <summary>Inner padding at the bottom of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingBottom)] public string PaddingBottom { get; init; } = "0";

    /// <summary>Inner padding at the left of the card root (raw-content cards). Default 0.</summary>
    [CssVar(CardField.PaddingLeft)] public string PaddingLeft { get; init; } = "0";

    /// <summary>Padding inside the card content.</summary>
    [CssVar(CardField.ContentPadding)] public string ContentPadding { get; init; } = "16px";

    /// <summary>Padding inside the card header.</summary>
    [CssVar(CardField.HeaderPadding)] public string HeaderPadding { get; init; } = "16px 16px 0 16px";

    /// <summary>Padding inside the card footer.</summary>
    [CssVar(CardField.FooterPadding)] public string FooterPadding { get; init; } = "8px 16px 16px 16px";

    /// <summary>Padding inside the card actions.</summary>
    [CssVar(CardField.ActionsPadding)] public string ActionsPadding { get; init; } = "8px 16px 16px 16px";

    /// <summary>Gap between action buttons.</summary>
    [CssVar(CardField.ActionsGap)] public string ActionsGap { get; init; } = "8px";

    /// <summary>Border radius of the card media.</summary>
    [CssVar(CardField.MediaRadius)] public string MediaRadius { get; init; } = "0";

    /// <summary>Color of the card title text.</summary>
    [CssVar(CardField.TitleColor)] public string TitleColor { get; init; } = Vars.Var(Color.OnSurface);

    /// <summary>Font family of the card title.</summary>
    [CssVar(CardField.TitleFontFamily)] public string TitleFontFamily { get; init; } = Vars.Var(Typography.Font("title-medium"));

    /// <summary>Font size of the card title.</summary>
    [CssVar(CardField.TitleFontSize)] public string TitleFontSize { get; init; } = Vars.Var(Typography.Size("title-medium"));

    /// <summary>Color of the card subtitle text.</summary>
    [CssVar(CardField.SubtitleColor)] public string SubtitleColor { get; init; } = Vars.Var(Color.OnSurfaceVariant);

    /// <summary>Font family of the card subtitle.</summary>
    [CssVar(CardField.SubtitleFontFamily)] public string SubtitleFontFamily { get; init; } = Vars.Var(Typography.Font("body-medium"));

    /// <summary>Font size of the card subtitle.</summary>
    [CssVar(CardField.SubtitleFontSize)] public string SubtitleFontSize { get; init; } = Vars.Var(Typography.Size("body-medium"));

    /// <summary>Transition duration for hover effects.</summary>
    [CssVar(CardField.TransitionDuration)] public string TransitionDuration { get; init; } = Vars.Var(Motion.DurationShort2);

    /// <summary>Transition easing for hover effects.</summary>
    [CssVar(CardField.TransitionEasing)] public string TransitionEasing { get; init; } = Vars.Var(Motion.EasingStandard);
}
