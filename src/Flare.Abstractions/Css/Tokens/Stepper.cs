namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlareStepper</c>.</summary>
public static class Stepper
{
    private const string FlareStepper = $"{Vars.Flare}-stepper";

    /// <summary>CSS custom-property name for the step indicator circle diameter token.</summary>
    public const string CircleSize = $"{FlareStepper}-circle-size";
    /// <summary>CSS custom-property name for the step circle border width token.</summary>
    public const string CircleBorderWidth = $"{FlareStepper}-circle-border-width";
    /// <summary>CSS custom-property name for the icon glyph size inside the circle token.</summary>
    public const string CircleIconSize = $"{FlareStepper}-circle-icon-size";
    /// <summary>CSS custom-property name for the connector thickness token.</summary>
    public const string ConnectorThickness = $"{FlareStepper}-connector-thickness";
    /// <summary>CSS custom-property name for the connector minimum length token.</summary>
    public const string ConnectorMinLength = $"{FlareStepper}-connector-min-length";
    /// <summary>CSS custom-property name for the incomplete connector color token.</summary>
    public const string ConnectorColor = $"{FlareStepper}-connector-color";
    /// <summary>CSS custom-property name for the completed connector color token.</summary>
    public const string ConnectorActiveColor = $"{FlareStepper}-connector-active-color";
    /// <summary>CSS custom-property name for the per-step minimum width token.</summary>
    public const string StepMinWidth = $"{FlareStepper}-step-min-width";
}
