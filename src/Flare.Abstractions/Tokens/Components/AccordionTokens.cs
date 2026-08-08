using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareAccordion</c> / <c>FlareAccordionPanel</c> (the container, its
/// panel headers and their bodies).</summary>
public sealed record AccordionTokens
{
    // --- 1. CONTAINER ---
    /// <summary>Border around the accordion container, as a CSS <c>border</c> shorthand. A theme with a
    /// borderless stack of sections parks this at <c>none</c>.</summary>
    [CssVar(AccordionField.Border)] public required string Border { get; init; }
    /// <summary>Corner radius of the accordion container. This used to read the CARD's radius, which
    /// meant a theme could not reshape one without reshaping the other.</summary>
    [CssVar(AccordionField.Radius)] public required string Radius { get; init; }
    /// <summary>Rule drawn between adjacent panels, as a CSS <c>border</c> shorthand.</summary>
    [CssVar(AccordionField.PanelDivider)] public required string PanelDivider { get; init; }

    // --- 2. PANEL HEADER ---
    /// <summary>Background of a panel header.</summary>
    [CssVar(AccordionField.HeaderBg)] public required string HeaderBg { get; init; }
    /// <summary>Foreground of a panel header.</summary>
    [CssVar(AccordionField.HeaderColor)] public required string HeaderColor { get; init; }
    /// <summary>Space above and below a panel header's content.</summary>
    [CssVar(AccordionField.HeaderPaddingBlock)] public required string HeaderPaddingBlock { get; init; }
    /// <summary>Space between a panel header's side edges and its content.</summary>
    [CssVar(AccordionField.HeaderPaddingInline)] public required string HeaderPaddingInline { get; init; }
    /// <summary>Space between the parts of a header's inline content (icons, badges, the title).</summary>
    [CssVar(AccordionField.HeaderGap)] public required string HeaderGap { get; init; }
    /// <summary>Font family of a panel header. Each theme decides which step of its own type scale a
    /// section title maps to.</summary>
    [CssVar(AccordionField.HeaderLabelFont)] public required string HeaderLabelFont { get; init; }
    /// <summary>Font size of a panel header.</summary>
    [CssVar(AccordionField.HeaderLabelSize)] public required string HeaderLabelSize { get; init; }
    /// <summary>Font weight of a panel header.</summary>
    [CssVar(AccordionField.HeaderLabelWeight)] public required string HeaderLabelWeight { get; init; }
    /// <summary>How far a disabled panel header fades. A language that repaints disabled headers in a
    /// flat palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(AccordionField.HeaderDisabledOpacity)] public required string HeaderDisabledOpacity { get; init; }
    /// <summary>Size of the chevron that turns as a panel expands.</summary>
    [CssVar(AccordionField.IconSize)] public required string IconSize { get; init; }

    // --- 3. PANEL BODY ---
    /// <summary>Space above and below a panel body's content.</summary>
    [CssVar(AccordionField.BodyPaddingBlock)] public required string BodyPaddingBlock { get; init; }
    /// <summary>Space between a panel body's side edges and its content.</summary>
    [CssVar(AccordionField.BodyPaddingInline)] public required string BodyPaddingInline { get; init; }
    /// <summary>Foreground of a panel body.</summary>
    [CssVar(AccordionField.BodyColor)] public required string BodyColor { get; init; }
    /// <summary>Ceiling the expanded panel's height animates to. The panel opens by growing
    /// <c>max-height</c>, which needs a concrete target, so this has to clear the tallest body a theme
    /// expects to hold; raising it slows the visible part of the open, which is why it belongs to the
    /// theme rather than to the core.</summary>
    [CssVar(AccordionField.ContentMaxHeight)] public required string ContentMaxHeight { get; init; }
}
