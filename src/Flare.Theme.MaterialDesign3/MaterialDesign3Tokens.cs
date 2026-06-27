using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3Expressive;

namespace Flare.Theme.MaterialDesign3;

/// <summary>
/// Baseline Material Design 3 design tokens. Derived from the published MD3 Expressive reference
/// (<see cref="Md3.DesignReference"/>) so the color system, shape scale, typography, motion, state
/// and elevation stay identical to MD3; only the Expressive-only behaviors are reverted to their
/// calmer baseline form.
/// </summary>
internal static class MaterialDesign3Tokens
{
    /// <summary>The complete baseline MD3 design tokens.</summary>
    public static readonly DesignTokens Design = BuildDesign();

    private static DesignTokens BuildDesign()
    {
        var reference = Md3.DesignReference;

        // Baseline MD3 buttons use Label Large at every size. The Expressive theme ramps the label
        // up to Title Medium / Headline Small / Headline Large for its larger button sizes; baseline
        // keeps the single classic button type style across all sizes.
        var label = reference.Typography.LabelLarge;
        var button = reference.Button with
        {
            LabelXs = label,
            LabelSm = label,
            LabelMd = label,
            LabelLg = label,
            LabelXl = label,
        };

        // Baseline MD3 reverts two Expressive-only behaviors via the typed records:
        //   1. Progress: a flat track (no Expressive wavy/amplitude line).
        var progress = reference.Progress with
        {
            WavyEnabled = "0",
            WavyHeight = "4px",
            WaveAmplitude = "0px",
        };

        //   2. Menus: a classic single 4dp surface with square items, not the Expressive 16dp rounded
        //      panel with floating rounded "island" group sections.
        var menu = reference.Menu with
        {
            PanelRadius = "var(--flare-shape-extra-small)",
            ItemRadius = "0",
            ItemRadiusEnd = "0",
            ItemGapBetween = "0",
            GroupBg = "transparent",
            GroupRadius = "0",
            GroupPadding = "0",
            GroupGap = "0",
            GroupShadow = "none",
            GroupedPanelBg = "var(--flare-color-surface-container)",
            GroupedPanelShadow = "var(--flare-elevation-2)",
        };

        return reference with
        {
            Button = button,
            Progress = progress,
            Menu = menu,
            // Extended carries only the inherited theme-specific extras (e.g. datetimepicker gap).
            Extended = reference.Extended,
        };
    }
}
