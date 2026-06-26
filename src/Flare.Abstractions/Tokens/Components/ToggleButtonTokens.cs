namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareToggleButton</c> / <c>FlareToggleGroup</c> (MD3 Segmented Button).
/// Toggle has 3 sizes (S/M/L). The selected state may morph the corner radius (MD3) or only change
/// colour (Fluent) - both are expressed purely through these tokens, so the shared CSS stays theme-agnostic.</summary>
public sealed record ToggleButtonTokens
{
    // --- 1. ВЫСОТА КОНТЕЙНЕРА (с меткой) под 3 размера ---
    /// <summary>Height sm token (<c>2rem</c>).</summary>
    public string HeightSm { get; init; } = "2rem";      // 32dp
    /// <summary>Height md token (<c>2.5rem</c>).</summary>
    public string HeightMd { get; init; } = "2.5rem";    // 40dp
    /// <summary>Height lg token (<c>3rem</c>).</summary>
    public string HeightLg { get; init; } = "3rem";      // 48dp

    // --- 2. БОКОВОЙ PADDING (с меткой) ---
    /// <summary>Padding sm token (<c>0.75rem</c>).</summary>
    public string PaddingSm { get; init; } = "0.75rem";
    /// <summary>Padding md token (<c>1rem</c>).</summary>
    public string PaddingMd { get; init; } = "1rem";
    /// <summary>Padding lg token (<c>1.5rem</c>).</summary>
    public string PaddingLg { get; init; } = "1.5rem";

    // --- 3. ЗАЗОР иконка-метка ---
    /// <summary>Gap token (<c>0.375rem</c>).</summary>
    public string Gap { get; init; } = "0.375rem";

    // --- 4. РАДИУС покоя и выбранного состояния (morph) ---
    /// <summary>Radius token (<c>var(--flare-shape-full)</c>).</summary>
    public string Radius { get; init; } = "var(--flare-shape-full)";
    /// <summary>Radius selected sm token (<c>var(--flare-shape-medium)</c>).</summary>
    public string RadiusSelectedSm { get; init; } = "var(--flare-shape-medium)"; // ~12dp
    /// <summary>Radius selected md token (<c>var(--flare-shape-medium)</c>).</summary>
    public string RadiusSelectedMd { get; init; } = "var(--flare-shape-medium)"; // ~12dp
    /// <summary>Radius selected lg token (<c>1rem</c>).</summary>
    public string RadiusSelectedLg { get; init; } = "1rem";                       // ~16dp

    // --- 5. ЦВЕТА покоя / выбора ---
    /// <summary>Rest bg token (<c>var(--flare-color-surface-container-highest)</c>).</summary>
    public string RestBg { get; init; } = "var(--flare-color-surface-container-highest)";
    /// <summary>Rest color token (<c>var(--flare-color-on-surface-variant)</c>).</summary>
    public string RestColor { get; init; } = "var(--flare-color-on-surface-variant)";
    /// <summary>Selected bg token (<c>var(--flare-color-secondary-container)</c>).</summary>
    public string SelectedBg { get; init; } = "var(--flare-color-secondary-container)";
    /// <summary>Selected color token (<c>var(--flare-color-on-secondary-container)</c>).</summary>
    public string SelectedColor { get; init; } = "var(--flare-color-on-secondary-container)";

    // --- 6. ГРУППА (Segmented) ---
    /// <summary>Group border token (<c>1px solid var(--flare-color-outline)</c>).</summary>
    public string GroupBorder { get; init; } = "1px solid var(--flare-color-outline)";
    /// <summary>Group radius token (<c>var(--flare-shape-full)</c>).</summary>
    public string GroupRadius { get; init; } = "var(--flare-shape-full)";
    /// <summary>Group radius vertical token (<c>var(--flare-shape-medium)</c>).</summary>
    public string GroupRadiusVertical { get; init; } = "var(--flare-shape-medium)";
    /// <summary>Group divider token (<c>var(--flare-color-outline)</c>).</summary>
    public string GroupDivider { get; init; } = "var(--flare-color-outline)";
}
