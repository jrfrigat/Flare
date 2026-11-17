using Flare.Css;
using Flare.Css.Tokens;
using static Flare.Css.Tokens.Vars;

namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme geometry and state tokens for the title-bar caption control. The four members a theme
/// must state are the ones a literal would otherwise bake into the core - the control's box and its
/// resting paint; every other state expresses a base Flare token and stays optional.
/// </summary>
/// <remarks>
/// The control is drawn inside a title-bar row, so its width and height are independent of the button
/// size ramp: a theme states the measured row height rather than inheriting a general control size.
/// The close control is the only danger state; its hover and pressed paints are separate tokens so a
/// theme can keep the platform's red without repainting every caption control.
/// </remarks>
public sealed record CaptionButtonTokens
{
    /// <summary>Width of one caption control in the title-bar row.</summary>
    [CssVar(CaptionButton.Width)] public required string Width { get; init; }
    /// <summary>Height of the caption control, normally the full title-bar row.</summary>
    [CssVar(CaptionButton.Height)] public required string Height { get; init; }
    /// <summary>Size of the drawn glyph inside the control. A caller-supplied icon descriptor that
    /// states its own size still wins, because it is an explicit request.</summary>
    [CssVar(CaptionButton.GlyphSize)] public required string GlyphSize { get; init; }
    /// <summary>Corner radius of the control. The default is the shape scale's none, which is what a
    /// row of adjacent caption controls needs to avoid gaps between them.</summary>
    [CssVar(CaptionButton.Radius)] public string Radius { get; init; } = Var(Css.Tokens.Shape.None);
    /// <summary>Resting background. A title-bar control normally sits on the title-bar surface, so a
    /// theme usually states transparent rather than naming a surface role here.</summary>
    [CssVar(CaptionButton.Background)] public required string Background { get; init; }
    /// <summary>Resting glyph color.</summary>
    [CssVar(CaptionButton.Color)] public string Color { get; init; } = Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>Background under the pointer.</summary>
    [CssVar(CaptionButton.HoverBackground)] public string HoverBackground { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHigh);
    /// <summary>Glyph color under the pointer.</summary>
    [CssVar(CaptionButton.HoverColor)] public string HoverColor { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Background while the control is held down.</summary>
    [CssVar(CaptionButton.PressedBackground)] public string PressedBackground { get; init; } = Var(Css.Tokens.Color.SurfaceContainerHighest);
    /// <summary>Glyph color while the control is held down.</summary>
    [CssVar(CaptionButton.PressedColor)] public string PressedColor { get; init; } = Var(Css.Tokens.Color.OnSurface);
    /// <summary>Keyboard focus outline. Pointer presses do not draw it: the control answers
    /// <c>:focus-visible</c>, so a click leaves the row clean.</summary>
    [CssVar(CaptionButton.FocusOutline)] public string FocusOutline { get; init; } = Var(Css.Tokens.Vars.FocusRing);
    /// <summary>Distance the focus outline sits from the control edge. The default pulls it inside by
    /// one spacing step, because an outline outside a full-height title-bar control would be clipped
    /// by the row or overlap its neighbours.</summary>
    [CssVar(CaptionButton.FocusOffset)] public string FocusOffset { get; init; } = $"calc({Var(Css.Tokens.Spacing.S1)} * -1)";
    /// <summary>Opacity of a disabled control. The disabled state reserves the measured title-bar box
    /// and never draws its hover or pressed surface.</summary>
    [CssVar(CaptionButton.DisabledOpacity)] public string DisabledOpacity { get; init; } = Var(Css.Tokens.State.DisabledOpacity);
    /// <summary>Hover background of the close control - the danger paint of the title bar.</summary>
    [CssVar(CaptionButton.CloseHoverBackground)] public string CloseHoverBackground { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Hover glyph color of the close control, which must stay legible on the danger paint.</summary>
    [CssVar(CaptionButton.CloseHoverColor)] public string CloseHoverColor { get; init; } = Var(Css.Tokens.Color.OnError);
    /// <summary>Pressed background of the close control. It defaults to the same danger paint as hover;
    /// a theme with a darker held-down red states it here.</summary>
    [CssVar(CaptionButton.ClosePressedBackground)] public string ClosePressedBackground { get; init; } = Var(Css.Tokens.Color.Error);
    /// <summary>Pressed glyph color of the close control.</summary>
    [CssVar(CaptionButton.ClosePressedColor)] public string ClosePressedColor { get; init; } = Var(Css.Tokens.Color.OnError);
}
