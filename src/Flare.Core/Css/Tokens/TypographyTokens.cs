namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for typography.</summary>
public static class Typography
{
    private const string Prefix = $"{Vars.Flare}-typescale";

    /// <summary>CSS variable name for the font-family of the given type scale.</summary>
    public static string Font(string scale) => $"{Prefix}-{scale}-font";
    /// <summary>CSS variable name for the font-weight of the given type scale.</summary>
    public static string Weight(string scale) => $"{Prefix}-{scale}-weight";
    /// <summary>CSS variable name for the font-size of the given type scale.</summary>
    public static string Size(string scale) => $"{Prefix}-{scale}-size";
    /// <summary>CSS variable name for the line-height of the given type scale.</summary>
    public static string Height(string scale) => $"{Prefix}-{scale}-height";
    /// <summary>CSS variable name for the letter-spacing of the given type scale.</summary>
    public static string Spacing(string scale) => $"{Prefix}-{scale}-spacing";
}
