using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens;

/// <summary>Corner-radius values for the shape scale (none, extra-small .. full).</summary>
public sealed record ShapeTokens
{
    /// <summary>None token.</summary>
    [CssVar(Shape.None)] public required string None { get; init; }
    /// <summary>Extra small token.</summary>
    [CssVar(Shape.ExtraSmall)] public required string ExtraSmall { get; init; }
    /// <summary>Small token.</summary>
    [CssVar(Shape.Small)] public required string Small { get; init; }
    /// <summary>Medium token.</summary>
    [CssVar(Shape.Medium)] public required string Medium { get; init; }
    /// <summary>Large token.</summary>
    [CssVar(Shape.Large)] public required string Large { get; init; }
    /// <summary>Extra large token.</summary>
    [CssVar(Shape.ExtraLarge)] public required string ExtraLarge { get; init; }
    /// <summary>Full token.</summary>
    [CssVar(Shape.Full)] public required string Full { get; init; }

    /// <summary>
    /// How long a component takes to travel between the corners of one state and the next - pressed,
    /// selected, hovered. <b>This is what decides whether a design language reshapes its components
    /// on interaction at all:</b> a language that does gives the travel a duration, and one whose
    /// components hold their shape parks it at an instant so each state's corners simply apply.
    /// </summary>
    /// <remarks>
    /// Deliberately one token for the whole library rather than one per component. Reshaping on
    /// interaction is a single statement a design language makes, so the button, the toggle button
    /// and the split button read the same value; a theme needing a component to differ overrides
    /// that component's transition in its own stylesheet. The corner values themselves stay with
    /// each component, because those are per-component geometry rather than a shared decision.
    /// </remarks>
    [CssVar(Shape.MorphDuration)] public required string MorphDuration { get; init; }
    /// <summary>
    /// Easing of the corner travel described by <see cref="MorphDuration"/>. A theme that parks that
    /// duration at an instant never reaches this.
    /// </summary>
    [CssVar(Shape.MorphEasing)] public required string MorphEasing { get; init; }
}
