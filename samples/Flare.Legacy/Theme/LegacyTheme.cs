using Flare.Abstractions.Tokens;
using Flare.Theming;
using Flare.Theme.FluentUI2;

namespace Flare.Legacy;

/// <summary>
/// The "Legacy" theme: the flat / square / gray "old desktop (1C / classic Windows)" look. It is
/// <b>derived</b> from Fluent UI 2 (so all corner/state geometry, style assets and palette generator
/// come from Fluent) and then layers the Tahoma typeface, dense spacing and gray Filled cards on top -
/// a worked example of <see cref="ThemeDerivation.Derive"/> instead of implementing <c>ITheme</c> by hand.
/// </summary>
public static class LegacyTheme
{
    /// <summary>The stable theme id - use this constant to select the Legacy theme.</summary>
    public const string ThemeId = "legacy";

    /// <summary>The Legacy theme, derived from <see cref="Fluent2Theme"/>.</summary>
    public static readonly Flare.Abstractions.ITheme Instance = new Fluent2Theme().Derive(
        id: ThemeId,
        displayName: "Legacy",
        design: BuildDesign,
        defaultPaletteId: LegacyPalettes.Legacy.Id,
        // The Legacy theme ships no palettes of its own (the gray palette is registered separately);
        // override Fluent's palette list with an empty one rather than inheriting it.
        palettes: []);

    // Fluent geometry + the classic Tahoma face, dense 1C spacing, and gray Filled cards. The base
    // Fluent DesignTokens arrive as the parameter (Derive passes them in).
    private static DesignTokens BuildDesign(DesignTokens g)
    {
        const string font = "Tahoma, 'Segoe UI', sans-serif";
        TypeStyle Tahoma(TypeStyle s) => s with { FontFamily = font };
        // Button labels use their own --flare-btn-label-* tokens, so set the face + a smaller size here.
        TypeStyle BtnLabel(TypeStyle s, string size) => s with { FontFamily = font, FontSize = size };
        var t = g.Typography;

        // All cards are Filled and share the gray "desktop" background, delineated only by a dark border.
        var ext = new Dictionary<string, string>(g.Extended)
        {
            ["--flare-card-bg"] = "var(--flare-color-surface-container-highest)",
            ["--flare-card-border"] = "1px solid var(--flare-color-outline)",
        };

        return g with
        {
            Extended = ext,
            // Tighten the small spacing steps so the dense field grids sit closer together (1C density).
            Spacing = g.Spacing with { S1 = "0.0625rem", S2 = "0.125rem", S3 = "0.1875rem" },
            // Dense 6px card padding (overrides Fluent's 12px) for the raw-content demo panels.
            // Outlined cards blend into the gray desktop background (matching --flare-color-background),
            // delineated only by their border, as in the classic 1C look.
            Card = g.Card with
            {
                PaddingTop = "0.375rem",
                PaddingRight = "0.375rem",
                PaddingBottom = "0.375rem",
                PaddingLeft = "0.375rem",
                OutlinedBg = "var(--flare-color-background)",
            },
            Button = g.Button with
            {
                LabelXs = BtnLabel(g.Button.LabelXs, "0.6875rem"),
                LabelSm = BtnLabel(g.Button.LabelSm, "0.6875rem"),
                LabelMd = BtnLabel(g.Button.LabelMd, "0.75rem"),
                LabelLg = BtnLabel(g.Button.LabelLg, "0.8125rem"),
                LabelXl = BtnLabel(g.Button.LabelXl, "0.875rem"),
            },
            Typography = t with
            {
                DisplayLarge = Tahoma(t.DisplayLarge),
                DisplayMedium = Tahoma(t.DisplayMedium),
                DisplaySmall = Tahoma(t.DisplaySmall),
                HeadlineLarge = Tahoma(t.HeadlineLarge),
                HeadlineMedium = Tahoma(t.HeadlineMedium),
                HeadlineSmall = Tahoma(t.HeadlineSmall),
                TitleLarge = Tahoma(t.TitleLarge),
                TitleMedium = Tahoma(t.TitleMedium),
                TitleSmall = Tahoma(t.TitleSmall),
                BodyLarge = Tahoma(t.BodyLarge),
                BodyMedium = Tahoma(t.BodyMedium),
                BodySmall = Tahoma(t.BodySmall),
                LabelLarge = Tahoma(t.LabelLarge),
                LabelMedium = Tahoma(t.LabelMedium),
                LabelSmall = Tahoma(t.LabelSmall),
            },
        };
    }
}
