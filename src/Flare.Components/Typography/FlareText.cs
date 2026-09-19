using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>
/// Renders a semantic HTML element (h1-h5, p) styled with the active theme's type scale.
/// Uses BuildRenderTree to support dynamic element names - standard Razor cannot do this.
/// </summary>
public sealed class FlareText : FlareComponentBase
{
    /// <summary>Text content rendered inside the typography element.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>Type-scale role applied to the text (defaults to <see cref="TypographyScale.BodyMedium"/>).</summary>
    [Parameter] public TypographyScale Typo { get; set; } = TypographyScale.BodyMedium;
    /// <summary>HTML element to render (e.g. "h1".."h5", "p", "span"). When null a sensible default for the <see cref="Typo"/> is used.</summary>
    [Parameter] public string? Element { get; set; }
    /// <summary>Text color. Role (<c>FlareColor.Primary</c>) -> shared class; custom (<c>FlareColor.Custom("#...")</c>) -> inline token. Default inherits.</summary>
    [Parameter] public FlareColor Color { get; set; } = FlareColor.Default;
    /// <summary>Font weight override. <see cref="FontWeight.Default"/> keeps the type scale's weight.</summary>
    [Parameter] public FontWeight Weight { get; set; } = FontWeight.Default;
    /// <summary>Horizontal text alignment. <see cref="TextAlign.Default"/> inherits the surrounding alignment.</summary>
    [Parameter] public TextAlign Align { get; set; } = TextAlign.Default;

    /// <summary>When true, renders in the monospace font - for code-like runs and keystrokes (pair with
    /// <c>Element="code"</c>/<c>"kbd"</c>). It only swaps the font; for the tonal inline-code chip use
    /// <see cref="FlareCode"/>.</summary>
    [Parameter] public bool Mono { get; set; }

    /// <summary>Letter-casing applied on top of the type-scale step. <see cref="TextTransform.None"/>
    /// (the default) leaves the text as written.</summary>
    [Parameter] public TextTransform Transform { get; set; } = TextTransform.None;

    /// <summary>
    /// Caps the text at this many lines and ends it with an ellipsis. <c>0</c> (the default) leaves it
    /// to wrap as far as it likes.
    /// <para>
    /// One parameter rather than a <c>Truncate</c> flag beside a <c>MaxLines</c> count, because they are
    /// the same request at different depths and two knobs would have to explain which wins. The
    /// mechanism underneath is not the same, though, and that is why this is a count and not a bool: at
    /// one line the text stops wrapping and ends with an ellipsis, which keeps the element's own display
    /// type; past one it needs a clamped flex box, which does not. Asking for the count lets the
    /// component pick, where a caller writing the CSS by hand had to know.
    /// </para>
    /// </summary>
    [Parameter] public int MaxLines { get; set; }

    /// <summary>The component's root CSS class.</summary>
    protected override string ComponentCssClass => Css.Classes.Text.Root;

    /// <summary>Renders the configured semantic element (h1-h5/p) with the resolved type-scale classes.</summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, _safeElement);
        builder.AddAttribute(1, "class", BuildCssClass(_scaleClass, Color.CssClass, _weightClass, _alignClass, _monoClass, _transformClass, _clampClass));
        if (_inlineStyle is not null)
            builder.AddAttribute(2, "style", _inlineStyle);
        if (AdditionalAttributes is not null)
            builder.AddMultipleAttributes(4, AdditionalAttributes);
        builder.AddContent(5, ChildContent);
        builder.CloseElement();
    }

    // Text always paints a scale, so an unset Typo falls back to body-medium here rather than in the map.
    private string _scaleClass => FlareTypography.CssClass(Typo) ?? Css.Classes.Text.BodyMedium;

    private string? _weightClass => Weight switch
    {
        FontWeight.Thin => Css.Classes.Text.WeightThin,
        FontWeight.ExtraLight => Css.Classes.Text.WeightExtraLight,
        FontWeight.Light => Css.Classes.Text.WeightLight,
        FontWeight.Regular => Css.Classes.Text.WeightRegular,
        FontWeight.Medium => Css.Classes.Text.WeightMedium,
        FontWeight.SemiBold => Css.Classes.Text.WeightSemiBold,
        FontWeight.Bold => Css.Classes.Text.WeightBold,
        FontWeight.ExtraBold => Css.Classes.Text.WeightExtraBold,
        FontWeight.Black => Css.Classes.Text.WeightBlack,
        _ => null,
    };

    private string? _alignClass => Align switch
    {
        TextAlign.Start => Css.Classes.Text.AlignStart,
        TextAlign.Left => Css.Classes.Text.AlignLeft,
        TextAlign.Center => Css.Classes.Text.AlignCenter,
        TextAlign.Right => Css.Classes.Text.AlignRight,
        TextAlign.End => Css.Classes.Text.AlignEnd,
        TextAlign.Justify => Css.Classes.Text.AlignJustify,
        _ => null,
    };

    private string? _monoClass => Mono ? Css.Classes.Text.Mono : null;

    private string? _transformClass => Transform switch
    {
        TextTransform.Uppercase => Css.Classes.Text.TransformUppercase,
        TextTransform.Lowercase => Css.Classes.Text.TransformLowercase,
        TextTransform.Capitalize => Css.Classes.Text.TransformCapitalize,
        _ => null,
    };

    // One line is not a one-line clamp: nowrap + ellipsis leaves the element's display type alone,
    // while the clamp needs -webkit-box and would turn a heading into a flex box to say the same thing.
    private string? _clampClass => MaxLines switch
    {
        <= 0 => null,
        1 => Css.Classes.Text.TruncateLine,
        _ => Css.Classes.Text.Clamp,
    };

    // The line count is per instance, so it travels as a local channel rather than as a class per depth.
    private string? _clampStyle => MaxLines >= 2
        ? $"{Css.Tokens.LocalVars.TextMaxLines}:{MaxLines.ToString(System.Globalization.CultureInfo.InvariantCulture)};"
        : null;

    private string _htmlElement => Typo switch
    {
        TypographyScale.DisplayLarge
            or TypographyScale.DisplayMedium
            or TypographyScale.DisplaySmall => "h1",
        TypographyScale.HeadlineLarge => "h2",
        TypographyScale.HeadlineMedium => "h3",
        TypographyScale.HeadlineSmall => "h4",
        TypographyScale.TitleLarge
            or TypographyScale.TitleMedium
            or TypographyScale.TitleSmall => "h5",
        _ => "p",
    };

    private static readonly HashSet<string> AllowedElements = new(StringComparer.OrdinalIgnoreCase)
        { "h1", "h2", "h3", "h4", "h5", "h6", "p", "span", "div", "strong", "em", "small", "b", "i", "code", "kbd", "samp", "pre" };

    private string _safeElement => Element is not null && AllowedElements.Contains(Element) ? Element : _htmlElement;

    // Custom color -> inline the shared --fc-main token; role/default need no inline. The clamp depth
    // joins it as a second channel, so a clamped run with a custom colour carries both.
    private string? _inlineStyle
    {
        get
        {
            var color = Color.IsCustom ? $"{Css.Tokens.LocalColor.Main}:{Color.Value};" : null;
            var combined = color + _clampStyle + Style;
            return string.IsNullOrEmpty(combined) ? null : combined;
        }
    }
}
