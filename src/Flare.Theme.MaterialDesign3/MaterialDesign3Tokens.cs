using Flare.Abstractions.Tokens;

namespace Flare.Theme.MaterialDesign3;

/// <summary>
/// Baseline Material Design 3 design tokens: the published reference set, used as it stands.
/// Expressive behaviours - the button label ramp, the island menu, the size ramp - belong to the
/// Expressive theme, which derives from this same reference and overrides only what it changes.
/// </summary>
internal static class MaterialDesign3Tokens
{
    /// <summary>The complete baseline MD3 design tokens.</summary>
    public static readonly DesignTokens Design = Flare.Theme.MaterialDesign3.Tokens.MaterialDesign3Tokens.Design;
}
