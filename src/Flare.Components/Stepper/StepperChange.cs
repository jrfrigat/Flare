namespace Flare.Components;

/// <summary>
/// Describes a pending change of the active step in a <see cref="FlareStepper"/>, passed to the
/// <see cref="FlareStepper.OnStepChanging"/> guard so it can validate (and allow or veto) the move.
/// </summary>
/// <param name="From">The currently active step index.</param>
/// <param name="To">The step index being navigated to. A backward move has <c>To &lt; From</c>.</param>
public readonly record struct StepperChange(int From, int To);
