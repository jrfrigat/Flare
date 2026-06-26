namespace Flare.Css.Tokens;

/// <summary>CSS variable names for <c>FlareTableOfContents</c> / <c>FlareOnThisPage</c>.</summary>
public static class TableOfContents
{
    private const string FlareToc = $"{Vars.Flare}-toc";

    /// <summary>CSS custom-property name for the active color token.</summary>
    public const string ActiveColor = $"{FlareToc}-active-color";
    /// <summary>CSS custom-property name for the inactive color token.</summary>
    public const string InactiveColor = $"{FlareToc}-inactive-color";
    /// <summary>CSS custom-property name for the title color token.</summary>
    public const string TitleColor = $"{FlareToc}-title-color";
    /// <summary>CSS custom-property name for the rail color token.</summary>
    public const string RailColor = $"{FlareToc}-rail-color";
    /// <summary>CSS custom-property name for the rail width token.</summary>
    public const string RailWidth = $"{FlareToc}-rail-width";
    /// <summary>CSS custom-property name for the active bg token.</summary>
    public const string ActiveBg = $"{FlareToc}-active-bg";
    /// <summary>CSS custom-property name for the active radius token.</summary>
    public const string ActiveRadius = $"{FlareToc}-active-radius";
    /// <summary>CSS custom-property name for the marker width token.</summary>
    public const string MarkerWidth = $"{FlareToc}-marker-width";
    /// <summary>CSS custom-property name for the link pad x token.</summary>
    public const string LinkPadX = $"{FlareToc}-link-pad-x";
    /// <summary>CSS custom-property name for the indent token.</summary>
    public const string Indent = $"{FlareToc}-indent";
}
