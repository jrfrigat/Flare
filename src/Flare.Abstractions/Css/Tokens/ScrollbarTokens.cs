namespace Flare.Css.Tokens;

/// <summary>
/// CSS variable tokens for the scrollbar - the one surface in a Flare application that the browser used
/// to paint on its own.
/// <para>
/// Two mechanisms have to be fed from these, because neither covers every engine: the standard
/// <c>scrollbar-color</c> / <c>scrollbar-width</c> pair (Firefox, Chromium 121+) and the older
/// <c>::-webkit-scrollbar</c> pseudo-elements (Safari). <see cref="Width"/> is a keyword because the
/// standard property takes one; <see cref="Size"/> is the length the pseudo-element path needs, and the
/// two must be kept consistent by the theme.
/// </para>
/// </summary>
public static class Scrollbar
{
    /// <summary>CSS custom-property name for the standard <c>scrollbar-width</c> keyword
    /// (<c>auto</c> | <c>thin</c> | <c>none</c>).</summary>
    public const string Width = "--flare-scrollbar-width";
    /// <summary>CSS custom-property name for the scrollbar thickness used by the WebKit pseudo-element
    /// path, as a length.</summary>
    public const string Size = "--flare-scrollbar-size";
    /// <summary>CSS custom-property name for the thumb colour at rest.</summary>
    public const string Thumb = "--flare-scrollbar-thumb";
    /// <summary>CSS custom-property name for the thumb colour while the pointer is over the scroll
    /// container.</summary>
    public const string ThumbHover = "--flare-scrollbar-thumb-hover";
    /// <summary>CSS custom-property name for the track colour. <c>transparent</c> gives the overlay
    /// look most current design languages use.</summary>
    public const string Track = "--flare-scrollbar-track";
    /// <summary>CSS custom-property name for the thumb's corner radius.</summary>
    public const string Radius = "--flare-scrollbar-radius";
}
