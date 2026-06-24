namespace Flare.Components;

/// <summary>
/// Unified size scale for Flare components. Use this for new components.
/// Existing components may have their own size enums for backward compatibility.
/// </summary>
public enum FlareSize
{
    /// <summary>Extra small - dense, for toolbars and inline elements.</summary>
    Xs,
    /// <summary>Small - compact.</summary>
    Sm,
    /// <summary>Medium - default size.</summary>
    Md,
    /// <summary>Large.</summary>
    Lg,
    /// <summary>Extra large - prominent.</summary>
    Xl,
}
