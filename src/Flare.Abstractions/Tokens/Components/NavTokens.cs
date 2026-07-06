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

    /// <summary>Left accent bar drawn on the active item; set to none for no bar.</summary>
    [CssVar(NavField.ActiveLeftBar)] public required string ActiveLeftBar { get; init; }
}
