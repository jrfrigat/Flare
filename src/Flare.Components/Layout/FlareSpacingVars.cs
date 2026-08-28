using Flare.Css.Tokens;

namespace Flare.Components;

/// <summary>
/// Maps a <see cref="FlareSpacing"/> step onto the spacing token behind it. For components that carry a
/// gap as an inline custom property rather than as a modifier class - the token stays the theme's either
/// way, so a component never writes a length of its own.
/// </summary>
public static class FlareSpacingVars
{
    /// <summary>The <c>--flare-spacing-*</c> custom-property NAME for a spacing step.</summary>
    /// <param name="spacing">The step to map. <see cref="FlareSpacing.Custom"/> has no token and maps to 0.</param>
    public static string Name(FlareSpacing spacing) => spacing switch
    {
        FlareSpacing.XXSmall => Spacing.S2,
        FlareSpacing.XSmall => Spacing.S4,
        FlareSpacing.Small => Spacing.S6,
        FlareSpacing.Medium => Spacing.S8,
        FlareSpacing.Large => Spacing.S12,
        FlareSpacing.XLarge => Spacing.S16,
        _ => Spacing.S0,
    };

    /// <summary>The <c>var(--flare-spacing-*)</c> REFERENCE for a spacing step, ready for a style value.</summary>
    /// <param name="spacing">The step to map.</param>
    public static string Var(FlareSpacing spacing) => $"var({Name(spacing)})";
}
