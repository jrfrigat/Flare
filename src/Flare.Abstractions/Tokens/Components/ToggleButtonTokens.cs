using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareToggleGroup</c>, the segmented control that seams several toggle
/// buttons into one bordered container. The buttons inside it are ordinary <c>FlareButton</c>s and read
/// <see cref="ButtonTokens"/> for their height, padding, typography, corners and selected paint - a toggle
/// is a button with a selected state, not a control of its own - so what is left here is only what the
/// container adds: its border, its corners and the rule between its segments.</summary>
public sealed record ToggleButtonTokens
{
    /// <summary>Border around a segmented group, as a CSS <c>border</c> shorthand.</summary>
    [CssVar(ToggleButton.GroupBorder)] public string GroupBorder { get; init; } = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline)";
    /// <summary>Corner radius of a horizontal segmented group.</summary>
    [CssVar(ToggleButton.GroupRadius)] public string GroupRadius { get; init; } = "var(--flare-shape-full)";
    /// <summary>Corner radius of a vertical segmented group, which a theme may want to differ from the
    /// horizontal one because the group is taller than it is wide.</summary>
    [CssVar(ToggleButton.GroupRadiusVertical)] public string GroupRadiusVertical { get; init; } = "var(--flare-shape-medium)";
    /// <summary>Colour of the rule between adjacent segments in a group.</summary>
    [CssVar(ToggleButton.GroupDivider)] public string GroupDivider { get; init; } = "var(--flare-color-outline)";
}
