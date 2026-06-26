namespace Flare.Abstractions.Tokens;

/// <summary>How theme/palette CSS reaches the document.</summary>
public enum ThemeDelivery
{
    /// <summary>
    /// Strategy A: one static, class-scoped stylesheet is injected once; switching
    /// theme/palette/mode is a class swap on the root element (no per-switch JS, fastest).
    /// </summary>
    ClassToggle,

    /// <summary>
    /// Legacy strategy: every switch writes the full composed variable set onto :root via
    /// JS interop. Simpler mental model; heavier on each switch.
    /// </summary>
    Inject,
}
