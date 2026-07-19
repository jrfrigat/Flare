using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Icons;

/// <summary>The Material Icons (Material Design 2) webfont style; each maps to a font family.</summary>
public enum MaterialDesign2Style
{
    /// <summary>Filled (<c>material-icons</c>). The default classic family.</summary>
    Filled,
    /// <summary>Outlined (<c>material-icons-outlined</c>).</summary>
    Outlined,
    /// <summary>Rounded (<c>material-icons-round</c>).</summary>
    Round,
    /// <summary>Sharp (<c>material-icons-sharp</c>).</summary>
    Sharp,
    /// <summary>Two-tone (<c>material-icons-two-tone</c>).</summary>
    TwoTone,
}

/// <summary>
/// A Material Design 2 icon from the classic Material Icons webfont, selected by <see cref="Name"/> (the
/// ligature name, e.g. <c>"home"</c>) and <see cref="Style"/>. Renders a <c>&lt;span class="material-icons"&gt;</c>
/// glyph, so the host app must load the corresponding Material Icons font family. Any
/// <see cref="FlareMaterialDesign2Icon"/> drops into any parameter typed <see cref="FlareIcon"/>. Ships in the
/// optional <c>Flare.Icons.MaterialDesign2.Symbols</c> package. For a font-free icon prefer the SVG package
/// <c>Flare.Icons.MaterialDesign2.Svg</c> (<c>MaterialDesign2Icons.*</c>) or the built-in <see cref="FlareIcons"/> set.
/// </summary>
public sealed record FlareMaterialDesign2Icon : FlareIcon
{
    /// <summary>The Material Icons ligature name (snake_case), e.g. <c>chevron_left</c>.</summary>
    public required string Name { get; init; }

    /// <summary>The font family / style. Default <see cref="MaterialDesign2Style.Filled"/>.</summary>
    public MaterialDesign2Style Style { get; init; } = MaterialDesign2Style.Filled;

    /// <inheritdoc/>
    protected override void Build(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", BuildClass(FontClass()));
        var style = BuildStyle(null);
        if (style is not null) builder.AddAttribute(2, "style", style);
        AddAccessibility(builder, 3);
        AddExtraAttributes(builder, 6);
        builder.AddContent(7, Name);
        builder.CloseElement();
    }

    private string FontClass() => Style switch
    {
        MaterialDesign2Style.Outlined => "material-icons-outlined",
        MaterialDesign2Style.Round => "material-icons-round",
        MaterialDesign2Style.Sharp => "material-icons-sharp",
        MaterialDesign2Style.TwoTone => "material-icons-two-tone",
        _ => "material-icons",
    };
}
