using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for <c>FlareColorPicker</c>'s chrome that isn't already covered by semantic color
/// tokens: the transparency checkerboard and the native hue/alpha range-input thumb (styled via
/// <c>::-webkit-slider-thumb</c> / <c>::-moz-range-thumb</c>, which cannot inherit <c>currentColor</c>
/// or an ancestor's custom property the way normal elements can). Defaults reproduce the previous
/// fixed appearance; a theme may override any field (e.g. a dark checkerboard or an accent-ring thumb).
/// </summary>
public sealed record ColorPickerTokens
{
    /// <summary>Color of the light squares in the transparency checkerboard (preview swatch and alpha track).</summary>
    [CssVar(ColorPickerField.CheckerColor)] public string CheckerColor { get; init; } = "#ccc";

    /// <summary>Background of the hue/alpha slider's native thumb.</summary>
    [CssVar(ColorPickerField.ThumbBg)] public string ThumbBg { get; init; } = "#fff";

    /// <summary>Border color of the hue/alpha slider's native thumb.</summary>
    [CssVar(ColorPickerField.ThumbBorderColor)] public string ThumbBorderColor { get; init; } = "rgba(0,0,0,0.3)";
}
