using Flare.Abstractions.Tokens;

namespace Flare.Core.Tests;

/// <summary>
/// A complete <see cref="MotionTokens"/> for tests that build a theme by hand and do not care what its motion
/// is - one place to add a member to, instead of one copy per test.
/// </summary>
internal static class TestMotionTokens
{
    private static readonly MotionPhaseTokens Phase = new()
    {
        Delay = "0ms",
        Duration = "200ms",
        Easing = "ease",
        Offset = "0px",
        Scale = "1",
        Opacity = "0",
        Reveal = "100%",
        Blur = "0px",
        FadeDelay = "0ms",
        FadeDuration = "200ms",
        FadeEasing = "linear",
    };

    private static readonly SurfaceMotionTokens Surface = new() { Enter = Phase, Exit = Phase };

    public static readonly MotionTokens Minimal = new()
    {
        EasingStandard = "ease",
        EasingDecelerate = "ease-out",
        EasingAccelerate = "ease-in",
        EasingEmphasized = "ease",
        EasingEmphasizedDecelerate = "ease-out",
        EasingEmphasizedAccelerate = "ease-in",
        EasingSubtle = "ease",
        EasingSubtleDecelerate = "ease-out",
        EasingSubtleAccelerate = "ease-in",
        EasingLinear = "linear",
        DurationStateChange = "100ms",
        DurationSmallMove = "200ms",
        DurationEnterSmall = "250ms",
        DurationEnterMedium = "400ms",
        DurationEnterLarge = "500ms",
        DurationExitSmall = "150ms",
        DurationExitMedium = "200ms",
        DurationExitLarge = "250ms",
        DurationCycle = "1000ms",
        DurationCycleLong = "2000ms",
        EasingSpringFast = "ease",
        EasingSpring = "ease",
        EasingSpringSlow = "ease",
        DurationSpringFast = "300ms",
        DurationSpring = "350ms",
        DurationSpringSlow = "500ms",
        Dialog = Surface,
        Sheet = Surface,
        Menu = Surface,
        Popover = Surface,
        Tooltip = Surface,
        Snackbar = Surface,
        Drawer = Surface,
    };
}
