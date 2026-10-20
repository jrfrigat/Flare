using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Dialog / ConfirmDialog / MessageBox: the panel surface, its text and shadow, the
/// size presets, the title typography and the close button's focus ring. Scrim, padding, motion and the
/// content typography still come from the shared color/spacing/typescale scales.
/// </summary>
public sealed record DialogTokens
{
    /// <summary>Corner radius of the dialog panel.</summary>
    [CssVar(DialogPanel.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.ExtraLarge);

    /// <summary>Size of the dialog header/close icon.</summary>
    [CssVar(DialogPanel.IconSize)] public required string IconSize { get; init; }

    /// <summary>Background of the dialog panel.</summary>
    [CssVar(DialogPanel.Bg)] public string Bg { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHigh);

    /// <summary>Text color of the dialog panel, inherited by the title and body unless they set their own.</summary>
    [CssVar(DialogPanel.Color)] public string Color { get; init; } = Var(Css.Tokens.Color.OnSurface);

    /// <summary>Shadow (<c>box-shadow</c>) lifting the dialog panel off the page; <c>none</c> for a flat panel.</summary>
    [CssVar(DialogPanel.Shadow)] public string Shadow { get; init; } = Var(Css.Tokens.Elevation.Level3);

    // The widths are lengths for which no base scale exists, so they carry no default: a theme that does
    // not set them would leave the panel unconstrained. Each size preset reads its own token.
    /// <summary>Minimum width of the dialog panel (a CSS length).</summary>
    [CssVar(DialogPanel.MinWidth)] public required string MinWidth { get; init; }
    /// <summary>Maximum width of the <see cref="DialogSize.Xs"/> preset.</summary>
    [CssVar(DialogPanel.MaxWidthXs)] public required string MaxWidthXs { get; init; }
    /// <summary>Maximum width of the <see cref="DialogSize.Sm"/> preset.</summary>
    [CssVar(DialogPanel.MaxWidthSm)] public required string MaxWidthSm { get; init; }
    /// <summary>Maximum width of the <see cref="DialogSize.Md"/> preset.</summary>
    [CssVar(DialogPanel.MaxWidthMd)] public required string MaxWidthMd { get; init; }
    /// <summary>Maximum width of the <see cref="DialogSize.Lg"/> preset.</summary>
    [CssVar(DialogPanel.MaxWidthLg)] public required string MaxWidthLg { get; init; }
    /// <summary>Maximum width of the <see cref="DialogSize.Xl"/> preset.</summary>
    [CssVar(DialogPanel.MaxWidthXl)] public required string MaxWidthXl { get; init; }
    /// <summary>Width and height of the built-in close button.</summary>
    [CssVar(DialogPanel.CloseSize)] public required string CloseSize { get; init; }

    /// <summary>Width of the close button's focus ring.</summary>
    [CssVar(DialogPanel.FocusRingWidth)] public string FocusRingWidth { get; init; } = Var(Css.Tokens.Border.WidthEmphasis);
    /// <summary>Color of the close button's focus ring.</summary>
    [CssVar(DialogPanel.FocusRingColor)] public string FocusRingColor { get; init; } = Var(Css.Tokens.Color.Primary);
    /// <summary>Distance between the close button and its focus ring.</summary>
    [CssVar(DialogPanel.FocusRingOffset)] public string FocusRingOffset { get; init; } = Var(Css.Tokens.Spacing.S1);

    /// <summary>Font family of the dialog title.</summary>
    [CssVar(DialogPanel.TitleFont)] public string TitleFont { get; init; } = Var(Css.Tokens.Typography.HeadlineSmall.Font);
    /// <summary>Font size of the dialog title.</summary>
    [CssVar(DialogPanel.TitleSize)] public string TitleSize { get; init; } = Var(Css.Tokens.Typography.HeadlineSmall.Size);
    /// <summary>Font weight of the dialog title.</summary>
    [CssVar(DialogPanel.TitleWeight)] public string TitleWeight { get; init; } = Var(Css.Tokens.Typography.HeadlineSmall.Weight);
    /// <summary>Line height of the dialog title.</summary>
    [CssVar(DialogPanel.TitleLineHeight)] public string TitleLineHeight { get; init; } = Var(Css.Tokens.Typography.HeadlineSmall.Height);
    /// <summary>Letter spacing of the dialog title.</summary>
    [CssVar(DialogPanel.TitleSpacing)] public string TitleSpacing { get; init; } = Var(Css.Tokens.Typography.HeadlineSmall.Spacing);
}
