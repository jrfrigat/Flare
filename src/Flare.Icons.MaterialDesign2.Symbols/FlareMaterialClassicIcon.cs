using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

/// <summary>The Material Icons (Material Design 2) webfont style; each maps to a font family.</summary>
public enum MaterialClassicStyle
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
/// <see cref="FlareMaterialClassicIcon"/> drops into any parameter typed <see cref="FlareIcon"/>. Ships in the
/// optional <c>Flare.Icons.MaterialDesign2.Symbols</c> package. For a font-free icon prefer the SVG package
/// <c>Flare.Icons.MaterialDesign2.Svg</c> (<c>MaterialIcons.*</c>) or the built-in <see cref="FlareIcons"/> set.
/// </summary>
public sealed record FlareMaterialClassicIcon : FlareIcon
{
    /// <summary>The Material Icons ligature name (snake_case), e.g. <c>chevron_left</c>.</summary>
    public required string Name { get; init; }

    /// <summary>The font family / style. Default <see cref="MaterialClassicStyle.Filled"/>.</summary>
    public MaterialClassicStyle Style { get; init; } = MaterialClassicStyle.Filled;

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
        MaterialClassicStyle.Outlined => "material-icons-outlined",
        MaterialClassicStyle.Round => "material-icons-round",
        MaterialClassicStyle.Sharp => "material-icons-sharp",
        MaterialClassicStyle.TwoTone => "material-icons-two-tone",
        _ => "material-icons",
    };
}
