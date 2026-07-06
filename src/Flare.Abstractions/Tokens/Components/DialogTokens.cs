using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for Dialog / ConfirmDialog / MessageBox. Surface, scrim, elevation, padding, title/content
/// font+color, motion and the per-size widths are NOT tokens here - the CSS reuses the shared color/
/// elevation/spacing/typescale scales (and hardcoded size classes) directly. Only the two dialog-specific
/// geometry knobs the CSS reads are tokens.
/// </summary>
public sealed record DialogTokens
{
    /// <summary>Corner radius of the dialog panel.</summary>
    [CssVar(DialogPanel.Radius)] public required string Radius { get; init; }

    /// <summary>Size of the dialog header/close icon.</summary>
    [CssVar(DialogPanel.IconSize)] public required string IconSize { get; init; }
}
