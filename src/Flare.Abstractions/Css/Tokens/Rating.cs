namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlareRating</c>.</summary>
public static class Rating
{
    private const string FlareRating = $"{Vars.Flare}-rating";

    /// <summary>CSS custom-property name for the star glyph size token.</summary>
    public const string Size = $"{FlareRating}-size";
    /// <summary>CSS custom-property name for the unfilled (empty) star color token.</summary>
    public const string EmptyColor = $"{FlareRating}-empty-color";
    /// <summary>CSS custom-property name for the default filled-star color token.</summary>
    public const string FilledColor = $"{FlareRating}-filled-color";
    /// <summary>CSS custom-property name for the hover scale token.</summary>
    public const string HoverScale = $"{FlareRating}-hover-scale";
}
