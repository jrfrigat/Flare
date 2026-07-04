namespace Flare.Css.Tokens;

/// <summary>CSS variable tokens for <c>FlarePagination</c>.</summary>
public static class Pagination
{
    private const string FlarePagination = $"{Vars.Flare}-pagination";

    /// <summary>CSS custom-property name for the page-button square size token.</summary>
    public const string Size = $"{FlarePagination}-size";
    /// <summary>CSS custom-property name for the page-button corner radius token.</summary>
    public const string Radius = $"{FlarePagination}-radius";
    /// <summary>CSS custom-property name for the page-button border color token.</summary>
    public const string BorderColor = $"{FlarePagination}-border-color";
    /// <summary>CSS custom-property name for the default active-page color token.</summary>
    public const string ActiveColor = $"{FlarePagination}-active-color";
}
