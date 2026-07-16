using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme geometry tokens for <c>FlareButtonGroup</c>. The base <c>buttongroup.css</c> carries no
/// visual opinion - it reads these tokens (all defaulting to a plain, un-connected button row) - so a
/// theme SETS the model it wants instead of a theme UNSETTING another's baked defaults. A connected look
/// = zero <see cref="Gap"/> + a small negative <see cref="Overlap"/> (collapses adjacent borders) + flat
/// <see cref="InnerRadius"/>; a separated look = positive <see cref="Gap"/> + zero <see cref="Overlap"/> +
/// rounded <see cref="InnerRadius"/>. <see cref="OuterRadius"/> may be a size-adaptive capsule
/// (<c>calc(var(--_flare-btn-height) / 2)</c>) since the button's size class sets that height variable.
/// </summary>
public sealed record ButtonGroupTokens
{
    /// <summary>Gap between segments (positive = separated). Connected themes use <c>0</c>.</summary>
    [CssVar(ButtonGroup.Gap)] public required string Gap { get; init; }

    /// <summary>Segment overlap: a (usually negative) margin on non-first segments that pulls each one back
    /// onto its neighbour, collapsing the two adjacent borders into a single shared seam. <c>0</c> = no
    /// overlap (separated segments, each keeping its own border).</summary>
    [CssVar(ButtonGroup.Overlap)] public required string Overlap { get; init; }

    /// <summary>Outer (leading/trailing group-end) corner radius.</summary>
    [CssVar(ButtonGroup.OuterRadius)] public required string OuterRadius { get; init; }

    /// <summary>Inner (interior seam) corner radius. <c>0</c> = flat/segmented; a shape token = rounded.</summary>
    [CssVar(ButtonGroup.InnerRadius)] public required string InnerRadius { get; init; }

    /// <summary>Z-index applied to a hovered/focused segment so its border and ring are not clipped by an
    /// overlapping neighbour.</summary>
    [CssVar(ButtonGroup.ZActive)] public required string ZActive { get; init; }
}
