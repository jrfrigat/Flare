using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Dialog / ConfirmDialog / MessageBox: the panel surface, its text and shadow, and the
/// two dialog-specific geometry knobs. Scrim, padding, title/content fonts, motion and the per-size widths
/// are NOT tokens here - the CSS reuses the shared color/spacing/typescale scales (and hardcoded size
/// classes) directly.
/// </summary>
public sealed record DialogTokens
{
    /// <summary>Corner radius of the dialog panel.</summary>
    [CssVar(DialogPanel.Radius)] public required string Radius { get; init; }

    /// <summary>Size of the dialog header/close icon.</summary>
    [CssVar(DialogPanel.IconSize)] public required string IconSize { get; init; }

    /// <summary>Background of the dialog panel.</summary>
    [CssVar(DialogPanel.Bg)] public required string Bg { get; init; }

    /// <summary>Text color of the dialog panel, inherited by the title and body unless they set their own.</summary>
    [CssVar(DialogPanel.Color)] public required string Color { get; init; }

    /// <summary>Shadow (<c>box-shadow</c>) lifting the dialog panel off the page; <c>none</c> for a flat panel.</summary>
    [CssVar(DialogPanel.Shadow)] public required string Shadow { get; init; }
}
