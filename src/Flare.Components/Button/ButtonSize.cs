namespace Flare.Components;

/// <summary>
/// Size of a FlareButton on the shared Xs..Xl scale (Md is the default). The height is theme-owned:
/// Md and larger meet the WCAG 2.5.8 target minimum of 24x24 CSS px in every built-in theme, while Xs
/// is 20px in the classic desktop themes (Aero, FluentUI 2, Visual Studio) and relies on that rule's
/// spacing exception; use Sm or larger where the minimum has to hold. The 44x44 (iOS) and 48x48
/// (Android) targets are platform guidance for touch, not a web requirement: use Lg or larger where a
/// finger is the primary input. An icon-only button has no visible label, so it must carry <c>AriaLabel</c>.
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
