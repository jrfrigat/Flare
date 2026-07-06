using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme tokens for <c>FlareToggleButton</c> / <c>FlareToggleGroup</c> (segmented button).
/// Toggle has 3 sizes (S/M/L). The selected state may morph the corner radius or only change the
/// colour - both are expressed purely through these tokens, so the shared CSS stays theme-agnostic.</summary>
public sealed record ToggleButtonTokens
{
    /// <summary>Height xs.</summary>
    [CssVar(ToggleButton.HeightXs)] public required string HeightXs { get; init; }
    /// <summary>Height xl.</summary>
    [CssVar(ToggleButton.HeightXl)] public required string HeightXl { get; init; }
    /// <summary>Padding xs.</summary>
    [CssVar(ToggleButton.PaddingXs)] public required string PaddingXs { get; init; }
    /// <summary>Padding xl.</summary>
    [CssVar(ToggleButton.PaddingXl)] public required string PaddingXl { get; init; }
    // --- 1. CONTAINER HEIGHT (with label) for 3 sizes ---
    /// <summary>Height sm token (<c>2rem</c>).</summary>
    [CssVar(ToggleButton.Height.Sm)] public required string HeightSm { get; init; }      // 32dp
    /// <summary>Height md token (<c>2.5rem</c>).</summary>
    [CssVar(ToggleButton.Height.Md)] public required string HeightMd { get; init; }    // 40dp
    /// <summary>Height lg token (<c>3rem</c>).</summary>
    [CssVar(ToggleButton.Height.Lg)] public required string HeightLg { get; init; }      // 48dp

    // --- 2. SIDE PADDING (with label) ---
    /// <summary>Padding sm token (<c>0.75rem</c>).</summary>
    [CssVar(ToggleButton.Padding.Sm)] public required string PaddingSm { get; init; }
    /// <summary>Padding md token (<c>1rem</c>).</summary>
    [CssVar(ToggleButton.Padding.Md)] public required string PaddingMd { get; init; }
    /// <summary>Padding lg token (<c>1.5rem</c>).</summary>
    [CssVar(ToggleButton.Padding.Lg)] public required string PaddingLg { get; init; }

    // --- 3. GAP between icon and label ---
    /// <summary>Gap token (<c>0.375rem</c>).</summary>
    [CssVar(ToggleButton.Gap)] public required string Gap { get; init; }

    // --- 4. RADIUS for rest and selected states (morph) ---
    /// <summary>Radius token (<c>var(--flare-shape-full)</c>).</summary>
    [CssVar(ToggleButton.Radius)] public required string Radius { get; init; }
    /// <summary>Radius selected sm token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(ToggleButton.RadiusSelected.Sm)] public required string RadiusSelectedSm { get; init; } // ~12dp
    /// <summary>Radius selected md token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(ToggleButton.RadiusSelected.Md)] public required string RadiusSelectedMd { get; init; } // ~12dp
    /// <summary>Radius selected lg token (<c>1rem</c>).</summary>
    [CssVar(ToggleButton.RadiusSelected.Lg)] public required string RadiusSelectedLg { get; init; }                       // ~16dp

    // --- 5. COLORS for rest / selected ---
    /// <summary>Rest bg token (<c>var(--flare-color-surface-container-highest)</c>).</summary>
    [CssVar(ToggleButton.RestBg)] public required string RestBg { get; init; }
    /// <summary>Rest color token (<c>var(--flare-color-on-surface-variant)</c>).</summary>
    [CssVar(ToggleButton.RestColor)] public required string RestColor { get; init; }
    /// <summary>Selected bg token (<c>var(--flare-color-secondary-container)</c>).</summary>
    [CssVar(ToggleButton.SelectedBg)] public required string SelectedBg { get; init; }
    /// <summary>Selected color token (<c>var(--flare-color-on-secondary-container)</c>).</summary>
    [CssVar(ToggleButton.SelectedColor)] public required string SelectedColor { get; init; }

    // --- 6. GROUP (Segmented) ---
    /// <summary>Group border token (<c>1px solid var(--flare-color-outline)</c>).</summary>
    [CssVar(ToggleButton.GroupBorder)] public required string GroupBorder { get; init; }
    /// <summary>Group radius token (<c>var(--flare-shape-full)</c>).</summary>
    [CssVar(ToggleButton.GroupRadius)] public required string GroupRadius { get; init; }
    /// <summary>Group radius vertical token (<c>var(--flare-shape-medium)</c>).</summary>
    [CssVar(ToggleButton.GroupRadiusVertical)] public required string GroupRadiusVertical { get; init; }
    /// <summary>Group divider token (<c>var(--flare-color-outline)</c>).</summary>
    [CssVar(ToggleButton.GroupDivider)] public required string GroupDivider { get; init; }
}
