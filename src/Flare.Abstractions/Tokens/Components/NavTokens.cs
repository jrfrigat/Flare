using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for navigation items (drawer / nav-rail entries) and the active indicator.
/// A theme styles the item hover shape and the active indicator (a pill fill and/or a left accent
/// bar) entirely through these tokens.
/// </summary>
public sealed record NavTokens
{
    /// <summary>Active weight.</summary>
    [CssVar(NavField.ActiveWeight)] public required string ActiveWeight { get; init; }
    /// <summary>Badge weight.</summary>
    [CssVar(NavField.BadgeWeight)] public required string BadgeWeight { get; init; }
    /// <summary>Rail label line height.</summary>
    [CssVar(NavField.RailLabelLineHeight)] public required string RailLabelLineHeight { get; init; }
    /// <summary>Hover/focus corner radius of a nav item.</summary>
    [CssVar(NavField.ItemRadius)] public required string ItemRadius { get; init; }

    /// <summary>Corner radius of the active indicator.</summary>
    [CssVar(NavField.IndicatorRadius)] public required string IndicatorRadius { get; init; }

    /// <summary>Background of the active indicator.</summary>
    [CssVar(NavField.ActiveIndicator)] public required string ActiveIndicator { get; init; }

    /// <summary>
    /// Text and icon colour of the active item. The other half of <see cref="ActiveIndicator"/>: the
    /// theme picks what the active row is painted on, so it has to pick what is read against it. These
    /// two were split before - the indicator was a token and the foreground was fixed in core to the
    /// on-secondary-container role - which left a theme with a tinted pill it could choose and a text
    /// colour it could not, and a theme whose indicator is <c>none</c> with a container foreground on a
    /// plain surface.
    /// </summary>
    [CssVar(NavField.ActiveColor)] public required string ActiveColor { get; init; }

    /// <summary>Resting text and icon colour of a nav item.</summary>
    [CssVar(NavField.ItemColor)] public required string ItemColor { get; init; }

    /// <summary>Text and icon colour of a hovered nav item, which a language may lift above its resting
    /// colour to answer the hover state layer underneath it.</summary>
    [CssVar(NavField.ItemHoverColor)] public required string ItemHoverColor { get; init; }

    /// <summary>Colour of a nav group's header, separate from the items so a language can set the
    /// section heading quieter or louder than the rows under it.</summary>
    [CssVar(NavField.GroupColor)] public required string GroupColor { get; init; }

    /// <summary>Colour of the supplementary caption pinned in a nav menu's header or footer - a build
    /// version, an account line. Reads as a caption rather than as a row.</summary>
    [CssVar(NavField.MetaColor)] public required string MetaColor { get; init; }

    /// <summary>Left accent bar drawn on the active item; set to none for no bar.</summary>
    [CssVar(NavField.ActiveLeftBar)] public required string ActiveLeftBar { get; init; }

    /// <summary>How far a disabled nav link fades. A language that repaints disabled controls in a flat
    /// palette leaves this opaque and carries the change in its own stylesheet, since a foreground
    /// colour has no value meaning "leave this as painted".</summary>
    [CssVar(NavField.LinkDisabledOpacity)] public required string LinkDisabledOpacity { get; init; }

    /// <summary>Size of the leading icon on a nav link and a nav group header, in the menu and in the rail.</summary>
    [CssVar(NavField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Minimum height of a nav link and a nav group header; a longer label still grows the row.</summary>
    [CssVar(NavField.ItemHeight)] public required string ItemHeight { get; init; }
}
