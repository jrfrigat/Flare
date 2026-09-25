namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for motion.</summary>
public static partial class Motion
{
    /// <summary>CSS custom-property name for the duration of hover, press and focus feedback.</summary>
    public const string DurationStateChange = "--flare-motion-duration-state-change";
    /// <summary>CSS custom-property name for the duration of a small element moving in place.</summary>
    public const string DurationSmallMove = "--flare-motion-duration-small-move";
    /// <summary>CSS custom-property name for the duration of a small in-page region appearing.</summary>
    public const string DurationEnterSmall = "--flare-motion-duration-enter-small";
    /// <summary>CSS custom-property name for the duration of a medium in-page region appearing.</summary>
    public const string DurationEnterMedium = "--flare-motion-duration-enter-medium";
    /// <summary>CSS custom-property name for the duration of a large in-page region appearing.</summary>
    public const string DurationEnterLarge = "--flare-motion-duration-enter-large";
    /// <summary>CSS custom-property name for the duration of a small in-page region going away.</summary>
    public const string DurationExitSmall = "--flare-motion-duration-exit-small";
    /// <summary>CSS custom-property name for the duration of a medium in-page region going away.</summary>
    public const string DurationExitMedium = "--flare-motion-duration-exit-medium";
    /// <summary>CSS custom-property name for the duration of a large in-page region going away.</summary>
    public const string DurationExitLarge = "--flare-motion-duration-exit-large";
    /// <summary>CSS custom-property name for the period of one spinner turn.</summary>
    public const string DurationCycle = "--flare-motion-duration-cycle";
    /// <summary>CSS custom-property name for the period of one pass of a long loop.</summary>
    public const string DurationCycleLong = "--flare-motion-duration-cycle-long";

    /// <summary>CSS custom-property name for the curve of a prominent element arriving.</summary>
    public const string EasingEmphasizedDecelerate = "--flare-motion-easing-emphasized-decelerate";
    /// <summary>CSS custom-property name for the curve of a prominent element leaving.</summary>
    public const string EasingEmphasizedAccelerate = "--flare-motion-easing-emphasized-accelerate";
    /// <summary>CSS custom-property name for the curve of a quiet on-screen movement.</summary>
    public const string EasingSubtle = "--flare-motion-easing-subtle";
    /// <summary>CSS custom-property name for the curve of a quiet element arriving.</summary>
    public const string EasingSubtleDecelerate = "--flare-motion-easing-subtle-decelerate";
    /// <summary>CSS custom-property name for the curve of a quiet element leaving.</summary>
    public const string EasingSubtleAccelerate = "--flare-motion-easing-subtle-accelerate";
    /// <summary>CSS custom-property name for the constant-rate curve.</summary>
    public const string EasingLinear = "--flare-motion-easing-linear";


    /// <summary>CSS custom-property name for the fast spring duration token.</summary>
    public const string DurationSpringFast = "--flare-motion-duration-spring-fast";
    /// <summary>CSS custom-property name for the default spring duration token.</summary>
    public const string DurationSpring = "--flare-motion-duration-spring";
    /// <summary>CSS custom-property name for the slow spring duration token.</summary>
    public const string DurationSpringSlow = "--flare-motion-duration-spring-slow";

    /// <summary>CSS custom-property name for the easing standard token.</summary>
    public const string EasingStandard = "--flare-motion-easing-standard";
    /// <summary>CSS custom-property name for the easing decelerate token.</summary>
    public const string EasingDecelerate = "--flare-motion-easing-decelerate";
    /// <summary>CSS custom-property name for the easing accelerate token.</summary>
    public const string EasingAccelerate = "--flare-motion-easing-accelerate";
    /// <summary>CSS custom-property name for the easing emphasized token.</summary>
    public const string EasingEmphasized = "--flare-motion-easing-emphasized";

    /// <summary>CSS custom-property name for the fast spring easing token.</summary>
    public const string EasingSpringFast = "--flare-motion-easing-spring-fast";
    /// <summary>CSS custom-property name for the default spring easing token.</summary>
    public const string EasingSpring = "--flare-motion-easing-spring";
    /// <summary>CSS custom-property name for the slow spring easing token.</summary>
    public const string EasingSpringSlow = "--flare-motion-easing-spring-slow";
}
