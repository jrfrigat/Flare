using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for the Card component. These control the geometry, color, and motion of cards.
/// A theme supplies every value and overrides the fields it needs so each variant looks native.
/// </summary>
public sealed record CardTokens
{
    /// <summary>Background color of the elevated card variant.</summary>
    [CssVar(CardField.ElevatedBg)] public string ElevatedBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerLow);

    /// <summary>Background color of the filled card variant.</summary>
    [CssVar(CardField.FilledBg)] public string FilledBg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHighest);

    /// <summary>Border of the filled card variant. The theme sets it: <c>none</c> for a borderless filled
    /// panel, or a border to delineate filled cards (e.g. the dense gray "1C" look).</summary>
    [CssVar(CardField.FilledBorder)] public required string FilledBorder { get; init; }

    /// <summary>Background color of the outlined card variant.</summary>
    [CssVar(CardField.OutlinedBg)] public string OutlinedBg { get; init; } = Var(Css.Tokens.Color.Surface);

    /// <summary>Border of the outlined card variant.</summary>
    [CssVar(CardField.OutlinedBorder)] public required string OutlinedBorder { get; init; }

    /// <summary>Background color of the tonal card variant.</summary>
    [CssVar(CardField.TonalBg)] public string TonalBg { get; init; } = Var(Css.Tokens.Color.SecondaryContainer);

    /// <summary>Text color of the tonal card variant.</summary>
    [CssVar(CardField.TonalColor)] public string TonalColor { get; init; } = Var(Css.Tokens.Color.OnSecondaryContainer);

    /// <summary>Text color of the text (transparent) card variant.</summary>
    [CssVar(CardField.TextColor)] public string TextColor { get; init; } = Var(Css.Tokens.Color.OnSurface);

    /// <summary>Border radius of the card (medium size). Small/large sizes scale from the shape scale.</summary>
    [CssVar(CardField.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.Medium);

    /// <summary>Elevation (box-shadow) of the elevated variant.</summary>
    [CssVar(CardField.Elevation)] public string Elevation { get; init; } = Var(Css.Tokens.Elevation.Level1);

    /// <summary>Elevation of a clickable elevated card on hover.</summary>
    [CssVar(CardField.ElevationHover)] public string ElevationHover { get; init; } = Var(Css.Tokens.Elevation.Level2);

    /// <summary>Elevation of a clickable filled card on hover; set it to the resting value for a card that
    /// does not lift.</summary>
    [CssVar(CardField.FilledElevationHover)] public string FilledElevationHover { get; init; } = Var(Css.Tokens.Elevation.Level1);

    /// <summary>Elevation of a clickable outlined card on hover; set it to the resting value for a card that
    /// does not lift.</summary>
    [CssVar(CardField.OutlinedElevationHover)] public string OutlinedElevationHover { get; init; } = Var(Css.Tokens.Elevation.Level1);

    /// <summary>Elevation of a clickable tonal card on hover; set it to the resting value for a card that
    /// does not lift.</summary>
    [CssVar(CardField.TonalElevationHover)] public required string TonalElevationHover { get; init; }

    /// <summary>Keyboard focus indicator of a clickable card, as an <c>outline</c> shorthand. Point it at the
    /// shared focus ring for a card that focuses like every other control.</summary>
    [CssVar(CardField.FocusRing)] public required string FocusRing { get; init; }

    /// <summary>Elevation (box-shadow) of the filled variant at rest.</summary>
    [CssVar(CardField.FilledElevation)] public required string FilledElevation { get; init; }

    /// <summary>Elevation (box-shadow) of the outlined variant at rest; a language whose outline replaces
    /// the shadow sets <c>none</c>.</summary>
    [CssVar(CardField.OutlinedElevation)] public string OutlinedElevation { get; init; } = Var(Css.Tokens.Elevation.Level0);

    /// <summary>Elevation (box-shadow) of the tonal variant at rest.</summary>
    [CssVar(CardField.TonalElevation)] public required string TonalElevation { get; init; }

    /// <summary>Elevation (box-shadow) of the text variant, which has no container of its own.</summary>
    [CssVar(CardField.TextElevation)] public string TextElevation { get; init; } = Var(Css.Tokens.Elevation.Level0);

    /// <summary>Border applied to a selected card (accent ring). Used when Selectable + Selected.</summary>
    [CssVar(CardField.SelectedBorder)] public string SelectedBorder { get; init; } = $"{Var(Css.Tokens.Border.WidthEmphasis)} {Var(Css.Tokens.Border.Style)} {Var(Css.Tokens.Color.Primary)}";

    /// <summary>Background tint applied to a selected card.</summary>
    [CssVar(CardField.SelectedBg)] public string SelectedBg { get; init; } = $"color-mix(in srgb, {Var(Css.Tokens.Color.Primary)} 8%, transparent)";

    /// <summary>Whole hover state layer for interactive cards - colour including alpha, or an image.
    /// Point it at the shared hover layer to make a card hover like every other surface, or give it
    /// its own value to make a card the exception.</summary>
    [CssVar(CardField.StateLayer)] public string StateLayer { get; init; } = Var(Css.Tokens.State.HoverLayer);

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
    [CssVar(CardField.TitleColor)] public string TitleColor { get; init; } = Var(Css.Tokens.Color.OnSurface);

    /// <summary>Font family of the card title.</summary>
    [CssVar(CardField.TitleFontFamily)] public string TitleFontFamily { get; init; } = Var(Css.Tokens.Typography.TitleMedium.Font);

    /// <summary>Font size of the card title.</summary>
    [CssVar(CardField.TitleFontSize)] public string TitleFontSize { get; init; } = Var(Css.Tokens.Typography.TitleMedium.Size);

    /// <summary>Color of the card subtitle text.</summary>
    [CssVar(CardField.SubtitleColor)] public string SubtitleColor { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);

    /// <summary>Font family of the card subtitle.</summary>
    [CssVar(CardField.SubtitleFontFamily)] public string SubtitleFontFamily { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Font);

    /// <summary>Font size of the card subtitle.</summary>
    [CssVar(CardField.SubtitleFontSize)] public string SubtitleFontSize { get; init; } = Var(Css.Tokens.Typography.BodyMedium.Size);

    /// <summary>Transition duration for hover effects.</summary>
    [CssVar(CardField.TransitionDuration)] public string TransitionDuration { get; init; } = Var(Css.Tokens.Motion.DurationStateChange);

    /// <summary>Transition easing for hover effects.</summary>
    [CssVar(CardField.TransitionEasing)] public string TransitionEasing { get; init; } = Var(Css.Tokens.Motion.EasingStandard);
}
