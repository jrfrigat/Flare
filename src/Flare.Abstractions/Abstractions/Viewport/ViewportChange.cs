namespace Flare.Components;

/// <summary>
/// Payload delivered to a viewport subscriber when the window is resized (throttled) or when the
/// active breakpoint changes.
/// </summary>
/// <param name="Size">The viewport size at the moment of the change.</param>
/// <param name="Breakpoint">The active breakpoint tier for <see cref="Size"/>, resolved with the
/// subscription's breakpoint definitions.</param>
/// <param name="BreakpointChanged">True when this notification crossed a breakpoint boundary (the tier
/// differs from the previous notification); false when only the pixel size changed within the same tier.</param>
/// <param name="IsImmediate">True when this is the synthetic first notification delivered right after
/// subscribing (see <see cref="ViewportSubscribeOptions.FireImmediately"/>), not a real resize event.</param>
public readonly record struct ViewportChange(
    ViewportSize Size,
    Breakpoint Breakpoint,
    bool BreakpointChanged,
    bool IsImmediate);
