namespace Flare.Css.Tokens;

/// <summary>
/// Local (per-instance) color CSS variables - the <c>--fc-*</c> set. A <c>FlareColor</c> with a custom
/// value inlines these on the element, and the component CSS reads them, falling back to the theme
/// color role. They are element-scoped overrides, not theme tokens, hence the short <c>--fc-</c> prefix.
/// </summary>
public static class LocalColor
{
    /// <summary>The accent / main color (<c>--fc-main</c>).</summary>
    public const string Main = "--fc-main";
    /// <summary>The contrast color on top of <see cref="Main"/> (<c>--fc-on</c>).</summary>
    public const string On = "--fc-on";
    /// <summary>The tonal container background (<c>--fc-container</c>).</summary>
    public const string Container = "--fc-container";
    /// <summary>The contrast color on top of <see cref="Container"/> (<c>--fc-on-container</c>).</summary>
    public const string OnContainer = "--fc-on-container";
}
