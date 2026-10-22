namespace Flare.Components;

/// <summary>
/// Size of a FlareButton on the shared Xs..Xl scale (Md is the default). Every step is at least 32 CSS px
/// tall in the built-in themes, so the WCAG 2.5.8 target minimum of 24x24 CSS px is met at every size.
/// The 44x44 (iOS) and 48x48 (Android) targets are platform guidance for touch, not a web requirement:
/// use Md or larger where a finger is the primary input. An icon-only button has no visible label, so
/// it must carry <c>AriaLabel</c>.
/// </summary>
public enum ButtonSize
{
    /// <summary>Extra small</summary>
    Xs,
    /// <summary>Small</summary>
    Sm,
    /// <summary>Medium</summary>
    Md,
    /// <summary>Large</summary>
    Lg,
    /// <summary>Extra large</summary>
    Xl,
}
