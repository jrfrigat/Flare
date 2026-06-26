namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for side.</summary>
public static class Side
{
    /// <summary>CSS custom-property name for the t token.</summary>
    public const string T = $"top";
    /// <summary>CSS custom-property name for the b token.</summary>
    public const string B = $"bottom";
    /// <summary>CSS custom-property name for the l token.</summary>
    public const string L = $"left";
    /// <summary>CSS custom-property name for the r token.</summary>
    public const string R = $"right";

    /// <summary>CSS custom-property name for the tl token.</summary>
    public const string TL = $"{T}-{L}";
    /// <summary>CSS custom-property name for the tr token.</summary>
    public const string TR = $"{T}-{R}";
    /// <summary>CSS custom-property name for the bl token.</summary>
    public const string BL = $"{B}-{L}";
    /// <summary>CSS custom-property name for the br token.</summary>
    public const string BR = $"{B}-{R}";
}
