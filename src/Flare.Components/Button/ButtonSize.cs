namespace Flare.Components;

/// <summary>
/// Size of a FlareButton on the shared Xs..Xl scale (Md is the default). The height is theme-owned:
/// Md and larger meet the WCAG 2.5.8 target minimum of 24x24 CSS px in every built-in theme. Xs is
/// the smallest target a theme's design system defines rather than an invented step below it - a
/// theme whose primary control is 24px maps Xs there, and a compact desktop theme whose toolbar
/// spec stops at 22px maps Xs to that height; where the result is below 24px the spacing exception
/// applies, so use Sm or larger where the minimum has to hold. The 44x44 (iOS) and 48x48
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
