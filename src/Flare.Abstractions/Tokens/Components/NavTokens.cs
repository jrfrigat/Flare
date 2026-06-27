using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Per-theme tokens for navigation items (drawer / nav-rail entries) and the active indicator.
/// Defaults follow Material Design 3 (rounded pill indicator on a secondary-container fill);
/// other themes override (e.g. Fluent uses a left accent bar).
/// </summary>
public sealed record NavTokens
{
    /// <summary>Hover/focus corner radius of a nav item. MD3 = extra-small.</summary>
    [CssVar(NavField.ItemRadius)] public string ItemRadius { get; init; } = Vars.Var(Shape.ExtraSmall);

    /// <summary>Corner radius of the active indicator. MD3 = full (pill).</summary>
    [CssVar(NavField.IndicatorRadius)] public string IndicatorRadius { get; init; } = Vars.Var(Shape.Full);

    /// <summary>Background of the active indicator. MD3 = secondary-container.</summary>
    [CssVar(NavField.ActiveIndicator)] public string ActiveIndicator { get; init; } = Vars.Var(Color.SecondaryContainer);

    /// <summary>Left accent bar drawn on the active item (e.g. Fluent rail). MD3 = none.</summary>
    [CssVar(NavField.ActiveLeftBar)] public string ActiveLeftBar { get; init; } = "none";
}
