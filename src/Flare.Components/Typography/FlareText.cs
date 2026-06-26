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

    /// <summary>
    /// When set, gives the element a stable <c>id</c> (a link target) and renders a hover "#" deep-link.
    /// Use on headings so <c>FlareOnThisPage</c> and shareable URLs can reference the section.
    /// </summary>
    [Parameter] public string? AnchorId { get; set; }

    [Inject] private NavigationManager Nav { get; set; } = default!;

    /// <summary>The component's root CSS class.</summary>
    protected override string ComponentCssClass => Css.Classes.Text.Root;

    // Fragment href anchored to the CURRENT page path, so Blazor does not resolve a bare "#id"
    // against the base href and navigate to the site root.
    private string _anchorHref
    {
        get
        {
            var uri = Nav.Uri;
            var hash = uri.IndexOf('#');
            var path = hash >= 0 ? uri[..hash] : uri;
            return $"{path}#{AnchorId}";
        }
    }

    /// <summary>Renders the configured semantic element (h1-h5/p) with the resolved type-scale classes.</summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, _safeElement);
        builder.AddAttribute(1, "class", BuildCssClass(_scaleClass, Color.CssClass, _weightClass, _alignClass));
        if (_inlineStyle is not null)
            builder.AddAttribute(2, "style", _inlineStyle);
        if (AnchorId is not null)
            builder.AddAttribute(3, "id", AnchorId);
        if (AdditionalAttributes is not null)
            builder.AddMultipleAttributes(4, AdditionalAttributes);
        builder.AddContent(5, ChildContent);
        if (AnchorId is not null)
        {
            // Hover-revealed deep link to this heading (kept out of the a11y tree / tab order).
            builder.OpenElement(6, "a");
            builder.AddAttribute(7, "class", Css.Classes.Text.Anchor);
            builder.AddAttribute(8, "href", _anchorHref);
            builder.AddAttribute(9, "aria-hidden", "true");
            builder.AddAttribute(10, "tabindex", "-1");
            builder.AddContent(11, "#");
            builder.CloseElement();
        }
        builder.CloseElement();
    }

    private string _scaleClass => Typo switch
    {
        TypographyScale.DisplayLarge => Css.Classes.Text.DisplayLarge,
        TypographyScale.DisplayMedium => Css.Classes.Text.DisplayMedium,
        TypographyScale.DisplaySmall => Css.Classes.Text.DisplaySmall,
        TypographyScale.HeadlineLarge => Css.Classes.Text.HeadlineLarge,
        TypographyScale.HeadlineMedium => Css.Classes.Text.HeadlineMedium,
        TypographyScale.HeadlineSmall => Css.Classes.Text.HeadlineSmall,
        TypographyScale.TitleLarge => Css.Classes.Text.TitleLarge,
        TypographyScale.TitleMedium => Css.Classes.Text.TitleMedium,
        TypographyScale.TitleSmall => Css.Classes.Text.TitleSmall,
        TypographyScale.BodyLarge => Css.Classes.Text.BodyLarge,
        TypographyScale.BodyMedium => Css.Classes.Text.BodyMedium,
        TypographyScale.BodySmall => Css.Classes.Text.BodySmall,
        TypographyScale.LabelLarge => Css.Classes.Text.LabelLarge,
        TypographyScale.LabelMedium => Css.Classes.Text.LabelMedium,
        TypographyScale.LabelSmall => Css.Classes.Text.LabelSmall,
        _ => Css.Classes.Text.BodyMedium,
    };

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
        { "h1", "h2", "h3", "h4", "h5", "h6", "p", "span", "div", "strong", "em", "small", "b", "i" };

    private string _safeElement => Element is not null && AllowedElements.Contains(Element) ? Element : _htmlElement;

    // Custom color -> inline the shared --fc-main token; role/default need no inline.
    private string? _inlineStyle => Color.IsCustom
        ? $"{Css.Tokens.LocalColor.Main}:{Color.Value};{Style}"
        : Style;
}
