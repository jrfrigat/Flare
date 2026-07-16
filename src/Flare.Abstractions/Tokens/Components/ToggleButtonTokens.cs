using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareToggleButton</c> / <c>FlareToggleGroup</c> (segmented button)
/// across the five button sizes. The selected state may morph the corner radius or only change the
/// colour - both are expressed purely through these tokens, so the shared CSS stays theme-agnostic.</summary>
public sealed record ToggleButtonTokens
{
    // --- 1. CONTAINER HEIGHT (with label) ---
    /// <summary>Container height at the xs size.</summary>
    [CssVar(ToggleButton.HeightXs)] public required string HeightXs { get; init; }
    /// <summary>Container height at the sm size.</summary>
    [CssVar(ToggleButton.Height.Sm)] public required string HeightSm { get; init; }
    /// <summary>Container height at the md size.</summary>
    [CssVar(ToggleButton.Height.Md)] public required string HeightMd { get; init; }
    /// <summary>Container height at the lg size.</summary>
    [CssVar(ToggleButton.Height.Lg)] public required string HeightLg { get; init; }
    /// <summary>Container height at the xl size.</summary>
    [CssVar(ToggleButton.HeightXl)] public required string HeightXl { get; init; }

    // --- 2. SIDE PADDING (with label) ---
    /// <summary>Space between the container edge and the content at the xs size.</summary>
    [CssVar(ToggleButton.PaddingXs)] public required string PaddingXs { get; init; }
    /// <summary>Space between the container edge and the content at the sm size.</summary>
    [CssVar(ToggleButton.Padding.Sm)] public required string PaddingSm { get; init; }
    /// <summary>Space between the container edge and the content at the md size.</summary>
    [CssVar(ToggleButton.Padding.Md)] public required string PaddingMd { get; init; }
    /// <summary>Space between the container edge and the content at the lg size.</summary>
    [CssVar(ToggleButton.Padding.Lg)] public required string PaddingLg { get; init; }
    /// <summary>Space between the container edge and the content at the xl size.</summary>
    [CssVar(ToggleButton.PaddingXl)] public required string PaddingXl { get; init; }

    // --- 3. GAP between icon and label ---
    /// <summary>Space between the icon and the label. The toggle uses one gap at every size.</summary>
    [CssVar(ToggleButton.Gap)] public required string Gap { get; init; }

    // --- 4. RADIUS for rest and selected states (morph) ---
    /// <summary>Corner radius in the unselected state, at every size.</summary>
    [CssVar(ToggleButton.Radius)] public required string Radius { get; init; }
    /// <summary>Corner radius in the selected state at the sm size. A theme that signals selection with
    /// colour alone sets this to the same value as <see cref="Radius"/>; a theme that morphs the shape on
    /// selection makes them differ.</summary>
    [CssVar(ToggleButton.RadiusSelected.Sm)] public required string RadiusSelectedSm { get; init; }
    /// <summary>Corner radius in the selected state at the md size.</summary>
    [CssVar(ToggleButton.RadiusSelected.Md)] public required string RadiusSelectedMd { get; init; }
    /// <summary>Corner radius in the selected state at the lg size.</summary>
    [CssVar(ToggleButton.RadiusSelected.Lg)] public required string RadiusSelectedLg { get; init; }

    // --- 5. COLORS for rest / selected ---
    /// <summary>Container background in the unselected state.</summary>
    [CssVar(ToggleButton.RestBg)] public required string RestBg { get; init; }
    /// <summary>Icon and label colour in the unselected state.</summary>
    [CssVar(ToggleButton.RestColor)] public required string RestColor { get; init; }
    /// <summary>Container background in the selected state.</summary>
    [CssVar(ToggleButton.SelectedBg)] public required string SelectedBg { get; init; }
    /// <summary>Icon and label colour in the selected state, which must stay legible on
    /// <see cref="SelectedBg"/>.</summary>
    [CssVar(ToggleButton.SelectedColor)] public required string SelectedColor { get; init; }

    // --- 6. GROUP (Segmented) ---
    /// <summary>Border around a segmented group, as a CSS <c>border</c> shorthand.</summary>
    [CssVar(ToggleButton.GroupBorder)] public required string GroupBorder { get; init; }
    /// <summary>Corner radius of a horizontal segmented group.</summary>
    [CssVar(ToggleButton.GroupRadius)] public required string GroupRadius { get; init; }
    /// <summary>Corner radius of a vertical segmented group, which a theme may want to differ from the
    /// horizontal one because the group is taller than it is wide.</summary>
    [CssVar(ToggleButton.GroupRadiusVertical)] public required string GroupRadiusVertical { get; init; }
    /// <summary>Colour of the rule between adjacent segments in a group.</summary>
    [CssVar(ToggleButton.GroupDivider)] public required string GroupDivider { get; init; }
}
