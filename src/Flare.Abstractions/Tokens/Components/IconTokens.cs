using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for icon - the motion of a glyph swap, read by icon.css.</summary>
public sealed record IconTokens
{
    /// <summary>
    /// How long a glyph swap takes when <c>FlareIconView.Morph</c> is on. A design language that treats an
    /// icon change as a repaint rather than a transition parks this at an instant, which makes every swap
    /// immediate without any call site opting out.
    /// </summary>
    [CssVar(Icon.MorphDuration)] public required string MorphDuration { get; init; }

    /// <summary>
    /// Curve of the glyph's MOVEMENT over the span of <see cref="MorphDuration"/> - the scale or rotation
    /// the mode adds, not the cross-fade underneath it, which rides the theme's standard easing instead.
    /// A theme that parks that duration at an instant never reaches this.
    /// </summary>
    /// <remarks>
    /// The split is what makes a spring usable here: a spring overshoots, and an opacity driven past its
    /// endpoint finishes early, which turns the hand-off between the two glyphs into a pop.
    /// </remarks>
    [CssVar(Icon.MorphEasing)] public required string MorphEasing { get; init; }

    /// <summary>
    /// Scale factor the glyph travels from and to in the scale morph mode: the incoming glyph grows from it,
    /// the outgoing one shrinks to it. A theme parks this at unity to make that mode a plain cross-fade.
    /// </summary>
    /// <remarks>
    /// Unitless, and read only by the scale mode - the fade mode is opacity alone and never touches it.
    /// </remarks>
    [CssVar(Icon.MorphScale)] public required string MorphScale { get; init; }

    /// <summary>
    /// Angle the glyph travels through in the rotate morph mode: the outgoing glyph turns to it, the
    /// incoming one arrives from its negation. A theme parks this at no rotation to make that mode a plain
    /// cross-fade.
    /// </summary>
    /// <remarks>Read only by the rotate mode.</remarks>
    [CssVar(Icon.MorphRotate)] public required string MorphRotate { get; init; }
}
