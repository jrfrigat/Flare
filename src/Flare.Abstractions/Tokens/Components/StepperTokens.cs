using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareStepper</c> (step indicator circle and the connector between steps).
/// The step-state fills (active / completed / upcoming / error) reference the theme's semantic color
/// roles directly, so they stay coherent with the color system and are not duplicated here; label and
/// description typography reuse the shared typescale tokens.
/// </summary>
public sealed record StepperTokens
{
    /// <summary>Focus ring thickness.</summary>
    [CssVar(Stepper.FocusRingThickness)] public required string FocusRingThickness { get; init; }
    /// <summary>Focus ring color.</summary>
    [CssVar(Stepper.FocusRingColor)] public required string FocusRingColor { get; init; }
    /// <summary>Diameter of the step indicator circle.</summary>
    [CssVar(Stepper.CircleSize)] public required string CircleSize { get; init; }

    /// <summary>Border width of the step circle (upcoming / error states).</summary>
    [CssVar(Stepper.CircleBorderWidth)] public required string CircleBorderWidth { get; init; }

    /// <summary>Glyph size of an icon rendered inside the step circle.</summary>
    [CssVar(Stepper.CircleIconSize)] public required string CircleIconSize { get; init; }

    /// <summary>Thickness of the connector between steps (line height / width).</summary>
    [CssVar(Stepper.ConnectorThickness)] public required string ConnectorThickness { get; init; }

    /// <summary>Minimum length of the connector between steps.</summary>
    [CssVar(Stepper.ConnectorMinLength)] public required string ConnectorMinLength { get; init; }

    /// <summary>Color of an incomplete connector.</summary>
    [CssVar(Stepper.ConnectorColor)] public required string ConnectorColor { get; init; }

    /// <summary>Color of a completed connector.</summary>
    [CssVar(Stepper.ConnectorActiveColor)] public required string ConnectorActiveColor { get; init; }

    /// <summary>Minimum width reserved for each step (keeps labels from crowding).</summary>
    [CssVar(Stepper.StepMinWidth)] public required string StepMinWidth { get; init; }
}
