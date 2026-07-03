namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for colorpicker.</summary>
public static class ColorPickerField
{
    private const string Prefix = $"{Vars.Flare}-colorpicker";

    /// <summary>CSS custom-property name for the transparency-checkerboard square color token.</summary>
    public const string CheckerColor = $"{Prefix}-checker-color";
    /// <summary>CSS custom-property name for the hue/alpha slider thumb background token.</summary>
    public const string ThumbBg = $"{Prefix}-thumb-bg";
    /// <summary>CSS custom-property name for the hue/alpha slider thumb border color token.</summary>
    public const string ThumbBorderColor = $"{Prefix}-thumb-border-color";
}
