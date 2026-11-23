namespace Flare.Components;

/// <summary>
/// The one size scale every Flare control uses (Xs..Xl; Md is the default unless a component says
/// otherwise). Each theme uses the same height for buttons and form fields at a given step.
/// Other families, including avatars, chips, switches and tracks, use the same size names
/// with dimensions from their own theme tokens.
///
/// The height itself is theme-owned: Md and larger meet the WCAG 2.5.8 target minimum of 24x24 CSS px
/// in every built-in theme. Xs is the smallest target a theme's design system defines rather than an
/// invented step below it; where the result is below 24px the spacing exception applies, so use Sm or
/// larger where the minimum has to hold. The 44x44 (iOS) and 48x48 (Android) targets are platform
/// guidance for touch, not a web requirement: use Lg or larger where a finger is the primary input.
/// Components whose value set is not this scale keep their own enum (FabSize, CardSize, DialogSize).
/// </summary>
public enum FlareSize
{
    /// <summary>Extra small - dense, for toolbars and inline elements.</summary>
    Xs,
    /// <summary>Small - compact.</summary>
    Sm,
    /// <summary>Medium - default size.</summary>
    Md,
    /// <summary>Large.</summary>
    Lg,
    /// <summary>Extra large - prominent.</summary>
    Xl,
}
