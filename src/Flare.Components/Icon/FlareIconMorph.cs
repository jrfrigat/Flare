namespace Flare.Components;

/// <summary>
/// How <c>FlareIconView</c> transitions from one glyph to the next when its value changes. The outgoing
/// and incoming glyphs are cross-faded in place - what differs between the modes is the movement layered
/// on top of that fade, and how far it travels is the theme's call (the <c>--flare-icon-morph-*</c> tokens).
/// </summary>
/// <remarks>
/// This is a transition between two icons, not an interpolation of one outline into another: the pair is
/// chosen by the caller at runtime, so no shared path structure can be assumed. See
/// <c>docs/issues/icon-morph-transition.md</c>.
/// </remarks>
public enum FlareIconMorph
{
    /// <summary>No transition; the glyph is replaced in place. The view renders no wrapper and keeps no history.</summary>
    None = 0,

    /// <summary>The outgoing glyph fades out as the incoming one fades in, neither of them moving.</summary>
    Fade,

    /// <summary>The outgoing glyph shrinks away while the incoming one grows in, both fading.</summary>
    Scale,

    /// <summary>The outgoing glyph turns away while the incoming one arrives from the opposite angle, both fading.</summary>
    Rotate,
}
