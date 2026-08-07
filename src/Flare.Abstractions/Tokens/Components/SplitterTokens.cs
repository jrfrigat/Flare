using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareSplitter</c>: how wide the draggable gutter is, how the grip
/// inside it is drawn, and what the gutter and its optional centre icon are coloured. The gutter is the
/// hit target and the grip is the mark inside it, so the two are sized independently - a theme can keep a
/// comfortable drag area while drawing only a hairline.</summary>
public sealed record SplitterTokens
{
    /// <summary>Thickness of the draggable gutter: its width when the splitter is vertical, its height
    /// when horizontal. This is the pointer target, so it is usually larger than the visible grip.</summary>
    [CssVar(Splitter.GutterSize)] public required string GutterSize { get; init; }

    /// <summary>Thickness of the grip bar drawn inside the gutter, across the drag axis.</summary>
    [CssVar(Splitter.GripThickness)] public required string GripThickness { get; init; }

    /// <summary>Length of the grip bar along the gutter. A theme that wants no visible grip sets this to
    /// zero rather than hiding the element.</summary>
    [CssVar(Splitter.GripLength)] public required string GripLength { get; init; }

    /// <summary>Gutter background at rest.</summary>
    [CssVar(Splitter.Color)] public required string Color { get; init; }

    /// <summary>Gutter background while hovered or keyboard-focused.</summary>
    [CssVar(Splitter.HoverColor)] public required string HoverColor { get; init; }

    /// <summary>Size of the optional collapse/expand icon centred in the gutter.</summary>
    [CssVar(Splitter.IconSize)] public required string IconSize { get; init; }

    /// <summary>Colour of the centre icon, which must stay legible on <see cref="Color"/> and
    /// <see cref="HoverColor"/> alike.</summary>
    [CssVar(Splitter.IconColor)] public required string IconColor { get; init; }
}
