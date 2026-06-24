namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for motion.</summary>
public static class Motion
{
    private const string DurationPrefix = $"{Vars.Flare}-motion-duration";
    private const string EasingPrefix = $"{Vars.Flare}-motion-easing";

    /// <summary>CSS custom-property name for the duration short 1 token.</summary>
    public const string DurationShort1 = $"{DurationPrefix}-short1";
    /// <summary>CSS custom-property name for the duration short 2 token.</summary>
    public const string DurationShort2 = $"{DurationPrefix}-short2";
    /// <summary>CSS custom-property name for the duration medium 1 token.</summary>
    public const string DurationMedium1 = $"{DurationPrefix}-medium1";
    /// <summary>CSS custom-property name for the duration medium 2 token.</summary>
    public const string DurationMedium2 = $"{DurationPrefix}-medium2";
    /// <summary>CSS custom-property name for the duration long 1 token.</summary>
    public const string DurationLong1 = $"{DurationPrefix}-long1";
    /// <summary>CSS custom-property name for the duration long 2 token.</summary>
    public const string DurationLong2 = $"{DurationPrefix}-long2";

    /// <summary>CSS custom-property name for the easing standard token.</summary>
    public const string EasingStandard = $"{EasingPrefix}-standard";
    /// <summary>CSS custom-property name for the easing decelerate token.</summary>
    public const string EasingDecelerate = $"{EasingPrefix}-decelerate";
    /// <summary>CSS custom-property name for the easing accelerate token.</summary>
    public const string EasingAccelerate = $"{EasingPrefix}-accelerate";
    /// <summary>CSS custom-property name for the easing emphasized token.</summary>
    public const string EasingEmphasized = $"{EasingPrefix}-emphasized";
}
