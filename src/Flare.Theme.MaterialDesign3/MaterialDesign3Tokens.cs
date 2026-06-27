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

        // Baseline MD3 reverts two Expressive-only Extended behaviors (the Extended dictionary is
        // applied last in CssVarMap.FlattenDesign, so these win over the inherited typed tokens):
        //   1. Progress: a flat track (no Expressive wavy/amplitude line).
        //   2. Menus: a classic single 4dp surface with square items, not the Expressive 16dp
        //      rounded panel with floating rounded "island" group sections.
        var extended = new Dictionary<string, string>(reference.Extended)
        {
            // Flat (non-wavy) progress.
            ["--flare-progress-wavy-enabled"] = "0",
            ["--flare-progress-wavy-height"] = "4px",
            ["--flare-progress-wave-amplitude"] = "0px",

            // Classic MD3 menu: 4dp container, square items, no island grouping.
            ["--flare-menu-panel-radius"] = "var(--flare-shape-extra-small)",
            ["--flare-menu-item-radius"] = "0",
            ["--flare-menu-item-radius-end"] = "0",
            ["--flare-menu-item-gap-between"] = "0",
            ["--flare-menu-group-bg"] = "transparent",
            ["--flare-menu-group-radius"] = "0",
            ["--flare-menu-group-padding"] = "0",
            ["--flare-menu-group-gap"] = "0",
            ["--flare-menu-group-shadow"] = "none",
            ["--flare-menu-grouped-panel-bg"] = "var(--flare-color-surface-container)",
            ["--flare-menu-grouped-panel-shadow"] = "var(--flare-elevation-2)",
        };

        return reference with
        {
            Button = button,
            Extended = extended,
        };
    }
}
