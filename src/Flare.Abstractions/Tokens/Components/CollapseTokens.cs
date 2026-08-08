using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareCollapse</c> (a standalone expandable region).
///
/// Deliberately separate from <see cref="AccordionTokens"/> even though the two headers share their
/// markup and behaviour: an accordion header is a filled section inside a bordered container, a
/// collapse header is a transparent control standing on its own, and a theme is entitled to size and
/// weight them differently. Sharing one record would have made that impossible.</summary>
public sealed record CollapseTokens
{
    /// <summary>Background of the header. A theme that lets the surface behind show through parks this
    /// at <c>transparent</c>.</summary>
    [CssVar(CollapseField.HeaderBg)] public required string HeaderBg { get; init; }
    /// <summary>Foreground of the header.</summary>
    [CssVar(CollapseField.HeaderColor)] public required string HeaderColor { get; init; }
    /// <summary>Corner radius of the header, which carries the hover shape since the control has no
    /// container of its own.</summary>
    [CssVar(CollapseField.HeaderRadius)] public required string HeaderRadius { get; init; }
    /// <summary>Space above and below the header's content.</summary>
    [CssVar(CollapseField.HeaderPaddingBlock)] public required string HeaderPaddingBlock { get; init; }
    /// <summary>Space between the header's side edges and its content.</summary>
    [CssVar(CollapseField.HeaderPaddingInline)] public required string HeaderPaddingInline { get; init; }
    /// <summary>Space between the parts of the header's inline content (icons, badges, the title).</summary>
    [CssVar(CollapseField.HeaderGap)] public required string HeaderGap { get; init; }
    /// <summary>Font family of the header. Each theme decides which step of its own type scale a
    /// standalone expander maps to.</summary>
    [CssVar(CollapseField.HeaderLabelFont)] public required string HeaderLabelFont { get; init; }
    /// <summary>Font size of the header.</summary>
    [CssVar(CollapseField.HeaderLabelSize)] public required string HeaderLabelSize { get; init; }
    /// <summary>Font weight of the header.</summary>
    [CssVar(CollapseField.HeaderLabelWeight)] public required string HeaderLabelWeight { get; init; }
    /// <summary>How far a disabled header fades. A language that repaints disabled headers in a flat
    /// palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(CollapseField.HeaderDisabledOpacity)] public required string HeaderDisabledOpacity { get; init; }
    /// <summary>Foreground of the chevron that turns as the region expands.</summary>
    [CssVar(CollapseField.IconColor)] public required string IconColor { get; init; }
}
