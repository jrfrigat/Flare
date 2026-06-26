using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareToggleButton</c> / <c>FlareToggleGroup</c> (MD3 Segmented Button).
/// Toggle has 3 sizes (S/M/L). The selected state may morph the corner radius (MD3) or only change
/// colour (Fluent) - both are expressed purely through these tokens, so the shared CSS stays theme-agnostic.</summary>
public sealed record ToggleButtonTokens
{
    // --- 1. ВЫСОТА КОНТЕЙНЕРА (с меткой) под 3 размера ---
    /// <summary>Height sm token (<c>2rem</c>).</summary>
    [CssVar(ToggleButton.Height.Sm)] public string HeightSm { get; init; } = "2rem";      // 32dp
    /// <summary>Height md token (<c>2.5rem</c>).</summary>
    [CssVar(ToggleButton.Height.Md)] public string HeightMd { get; init; } = "2.5rem";    // 40dp
    /// <summary>Height lg token (<c>3rem</c>).</summary>
    [CssVar(ToggleButton.Height.Lg)] public string HeightLg { get; init; } = "3rem";      // 48dp

    // --- 2. БОКОВОЙ PADDING (с меткой) ---
    /// <summary>Padding sm token (<c>0.75rem</c>).</summary>
    [CssVar(ToggleButton.Padding.Sm)] public string PaddingSm { get; init; } = "0.75rem";
    /// <summary>Padding md token (<c>1rem</c>).</summary>
    [CssVar(ToggleButton.Padding.Md)] public string PaddingMd { get; init; } = "1rem";
    /// <summary>Padding lg token (<c>1.5rem</c>).</summary>
    [CssVar(ToggleButton.Padding.Lg)] public string PaddingLg { get; init; } = "1.5rem";

    // --- 3. ЗАЗОР иконка-метка ---
    /// <summary>Gap token (<c>0.375rem</c>).</summary>
    [CssVar(ToggleButton.Gap)] public string Gap { get; init; } = "0.375rem";

    // --- 4. РАДИУС покоя и выбранного состояния (morph) ---
    /// <summary>Radius token (<c>var(--flare-shape-full)</c>).</summary>
    [CssVar(ToggleButton.Radius)] public string Radius { get; init; } = Vars.Var(Shape.Full);
    /// <summary>Radius selected sm token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(ToggleButton.RadiusSelected.Sm)] public string RadiusSelectedSm { get; init; } = Vars.Var(Shape.Medium); // ~12dp
    /// <summary>Radius selected md token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(ToggleButton.RadiusSelected.Md)] public string RadiusSelectedMd { get; init; } = Vars.Var(Shape.Medium); // ~12dp
    /// <summary>Radius selected lg token (<c>1rem</c>).</summary>
    [CssVar(ToggleButton.RadiusSelected.Lg)] public string RadiusSelectedLg { get; init; } = "1rem";                       // ~16dp

    // --- 5. ЦВЕТА покоя / выбора ---
    /// <summary>Rest bg token (<c>var(--flare-color-surface-container-highest)</c>).</summary>
    [CssVar(ToggleButton.RestBg)] public string RestBg { get; init; } = Vars.Var(Color.SurfaceContainerHighest);
    /// <summary>Rest color token (<c>var(--flare-color-on-surface-variant)</c>).</summary>
    [CssVar(ToggleButton.RestColor)] public string RestColor { get; init; } = Vars.Var(Color.OnSurfaceVariant);
    /// <summary>Selected bg token (<c>var(--flare-color-secondary-container)</c>).</summary>
    [CssVar(ToggleButton.SelectedBg)] public string SelectedBg { get; init; } = Vars.Var(Color.SecondaryContainer);
    /// <summary>Selected color token (<c>var(--flare-color-on-secondary-container)</c>).</summary>
    [CssVar(ToggleButton.SelectedColor)] public string SelectedColor { get; init; } = Vars.Var(Color.OnSecondaryContainer);

    // --- 6. ГРУППА (Segmented) ---
    /// <summary>Group border token (<c>1px solid var(--flare-color-outline)</c>).</summary>
    [CssVar(ToggleButton.GroupBorder)] public string GroupBorder { get; init; } = "1px solid var(--flare-color-outline)";
    /// <summary>Group radius token (<c>var(--flare-shape-full)</c>).</summary>
    [CssVar(ToggleButton.GroupRadius)] public string GroupRadius { get; init; } = Vars.Var(Shape.Full);
    /// <summary>Group radius vertical token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(ToggleButton.GroupRadiusVertical)] public string GroupRadiusVertical { get; init; } = Vars.Var(Shape.Medium);
    /// <summary>Group divider token (<c>var(--flare-color-outline)</c>).</summary>
    [CssVar(ToggleButton.GroupDivider)] public string GroupDivider { get; init; } = Vars.Var(Color.Outline);
}
