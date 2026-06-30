namespace Flare.Components;

/// <summary>
/// Navigation context handed to <see cref="FlareStepper.ActionContent"/> so a consumer can render
/// bespoke navigation (custom icon buttons, wheel/keyboard handlers, ...) in place of the built-in
/// Back/Next buttons. It exposes the active step's position plus the same navigation operations the
/// built-in controls use, so a custom control behaves identically (each operation runs the
/// <see cref="FlareStepper.OnStepChanging"/> guard and updates <see cref="FlareStepper.ActiveIndex"/>).
/// </summary>
public sealed class StepperContext
{
    private readonly FlareStepper _stepper;

    internal StepperContext(FlareStepper stepper, int activeIndex, int count, FlareStep? step)
    {
        _stepper = stepper;
        ActiveIndex = activeIndex;
        Count = count;
        Step = step;
    }

    /// <summary>Zero-based index of the currently active step.</summary>
    public int ActiveIndex { get; }

    /// <summary>Total number of registered steps.</summary>
    public int Count { get; }

    /// <summary>The currently active <see cref="FlareStep"/>, or <c>null</c> when no steps are registered.</summary>
    public FlareStep? Step { get; }

    /// <summary>True when the active step is the first one (no previous step to go back to).</summary>
    public bool IsFirst => ActiveIndex <= 0;

    /// <summary>True when the active step is the last one (no next step to advance to).</summary>
    public bool IsLast => ActiveIndex >= Count - 1;

    /// <summary>Advances to the next step. No-op on the last step. Runs the navigation guard.</summary>
    public Task NextAsync() => _stepper.Next();

    /// <summary>Returns to the previous step. No-op on the first step. Runs the navigation guard.</summary>
    public Task BackAsync() => _stepper.Back();

    /// <summary>
    /// Navigates to <paramref name="index"/> when permitted by the linear/skippable rules. Runs the
    /// navigation guard. Use <see cref="CanGoTo"/> to test a target up-front (e.g. to disable a button).
    /// </summary>
    /// <param name="index">Zero-based target step index.</param>
    public Task GoToAsync(int index) => _stepper.GoTo(index);

    /// <summary>
    /// Whether navigation to <paramref name="index"/> is currently allowed by the linear/skippable
    /// rules (the same check used for step-indicator clicks).
    /// </summary>
    /// <param name="index">Zero-based target step index.</param>
    public bool CanGoTo(int index) => _stepper.CanGoTo(index);
}
