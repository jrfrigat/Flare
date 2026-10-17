using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareToggleGroup</c>, the segmented control that seams several toggle
/// buttons into one bordered container. The buttons inside it are ordinary <c>FlareButton</c>s and read
/// <see cref="ButtonTokens"/> for their height, padding, typography, corners and selected paint - a toggle
/// is a button with a selected state, not a control of its own - so what is left here is only what the
/// container adds: its border, its corners and the rule between its segments.</summary>
public sealed record ToggleButtonTokens
{
    /// <summary>Border around a segmented group, as a CSS <c>border</c> shorthand.</summary>
    [CssVar(ToggleButton.GroupBorder)] public string GroupBorder { get; init; } = $"{Var(Css.Tokens.Border.Width)} {Var(Css.Tokens.Border.Style)} {Var(Css.Tokens.Color.Outline)}";
    /// <summary>Corner radius of a horizontal segmented group.</summary>
    [CssVar(ToggleButton.GroupRadius)] public string GroupRadius { get; init; } = Var(Css.Tokens.Shape.Full);
    /// <summary>Corner radius of a vertical segmented group, which a theme may want to differ from the
    /// horizontal one because the group is taller than it is wide.</summary>
    [CssVar(ToggleButton.GroupRadiusVertical)] public string GroupRadiusVertical { get; init; } = Var(Css.Tokens.Shape.Medium);
    /// <summary>Colour of the rule between adjacent segments in a group.</summary>
    [CssVar(ToggleButton.GroupDivider)] public string GroupDivider { get; init; } = Var(Css.Tokens.Color.Outline);
}
