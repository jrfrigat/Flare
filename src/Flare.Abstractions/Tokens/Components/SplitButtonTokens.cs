using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Per-theme geometry and seam tokens for <c>FlareSplitButton</c>.</summary>
public sealed record SplitButtonTokens
{
    // --- 1. SEAM / GAP BETWEEN BUTTONS ---
    /// <summary>Width of the seam between the main button and the trigger. A theme that fuses the two into
    /// one container parks this at <c>0</c>; a theme with two separated pills opens it up.</summary>
    [CssVar(SplitButton.Gap)] public required string Gap { get; init; }

    /// <summary>Width of the trigger button. A theme may track the button height for a square trigger, or
    /// pin a fixed width.</summary>
    [CssVar(SplitButton.TriggerWidth)] public required string TriggerWidth { get; init; }

    // (Trigger side padding tokens removed: the trigger is a fixed icon-only square with
    //  padding-inline:0 in splitbutton.css, so these had no CSS reader.)

    // --- 3. CARET ICON SIZES (Caret Sizes) ---
    /// <summary>Caret glyph size on the trigger at the xs size.</summary>
    [CssVar(SplitButton.CaretSize.Xs)] public required string CaretSizeXs { get; init; }
    /// <summary>Caret glyph size on the trigger at the sm size.</summary>
    [CssVar(SplitButton.CaretSize.Sm)] public required string CaretSizeSm { get; init; }
    /// <summary>Caret glyph size on the trigger at the md size.</summary>
    [CssVar(SplitButton.CaretSize.Md)] public required string CaretSizeMd { get; init; }
    /// <summary>Caret glyph size on the trigger at the lg size.</summary>
    [CssVar(SplitButton.CaretSize.Lg)] public required string CaretSizeLg { get; init; }
    /// <summary>Caret glyph size on the trigger at the xl size.</summary>
    [CssVar(SplitButton.CaretSize.Xl)] public required string CaretSizeXl { get; init; }

    // --- 4. PER-CORNER RADII FOR THE MAIN BUTTON (Main Button) ---
    // The main button and the trigger are radiused independently so a theme can round the outer edges of
    // the pair while keeping the two sides of the seam square (or not).
    /// <summary>Corner radii of the main button at the xs size, one value per corner.</summary>
    public required CornerRadiusTokens MainRadiusXs { get; init; }
    /// <summary>Corner radii of the main button at the sm size, one value per corner.</summary>
    public required CornerRadiusTokens MainRadiusSm { get; init; }
    /// <summary>Corner radii of the main button at the md size, one value per corner.</summary>
    public required CornerRadiusTokens MainRadiusMd { get; init; }
    /// <summary>Corner radii of the main button at the lg size, one value per corner.</summary>
    public required CornerRadiusTokens MainRadiusLg { get; init; }
    /// <summary>Corner radii of the main button at the xl size, one value per corner.</summary>
    public required CornerRadiusTokens MainRadiusXl { get; init; }

    // --- 5. PER-CORNER RADII FOR THE TRIGGER BUTTON (Trigger Button) ---
    /// <summary>Corner radii of the trigger button at the xs size, one value per corner.</summary>
    public required CornerRadiusTokens TriggerRadiusXs { get; init; }
    /// <summary>Corner radii of the trigger button at the sm size, one value per corner.</summary>
    public required CornerRadiusTokens TriggerRadiusSm { get; init; }
    /// <summary>Corner radii of the trigger button at the md size, one value per corner.</summary>
    public required CornerRadiusTokens TriggerRadiusMd { get; init; }
    /// <summary>Corner radii of the trigger button at the lg size, one value per corner.</summary>
    public required CornerRadiusTokens TriggerRadiusLg { get; init; }
    /// <summary>Corner radii of the trigger button at the xl size, one value per corner.</summary>
    public required CornerRadiusTokens TriggerRadiusXl { get; init; }
}
