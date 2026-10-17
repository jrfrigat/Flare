using Flare.Abstractions;
using Flare.Theming;
using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Core.Tests;

public class TokensToCssTests
{
    // Reuse the builders from ThemeServiceTests via small local copies.
    private static TypeStyle TS() => new() { FontFamily = "Test", FontWeight = "400", FontSize = "1rem", LineHeight = "1.5rem", LetterSpacing = "0" };

    private static DesignTokens Design(IReadOnlyDictionary<string, string>? extended = null) => MaterialDesign3Tokens.Design with
    {
        FocusRing = "2px solid #000",
        Typography = new()
        {
            MonoFont = "monospace, monospace",
            WeightMedium = "500", WeightBold = "700",
            TrackingWide = "0.05em", Size2Xs = "0.5625rem", SizeXs = "0.625rem",
            DisplayLarge = TS(),
            DisplayMedium = TS(),
            DisplaySmall = TS(),
            HeadlineLarge = TS(),
            HeadlineMedium = TS(),
            HeadlineSmall = TS(),
            TitleLarge = TS(),
            TitleMedium = TS(),
            TitleSmall = TS(),
            BodyLarge = TS(),
            BodyMedium = TS(),
            BodySmall = TS(),
            LabelLarge = TS(),
            LabelMedium = TS(),
            LabelSmall = TS(),
        },
        Shape = new() { None = "0", ExtraSmall = "4px", Small = "8px", Medium = "12px", Large = "16px", ExtraLarge = "28px", Full = "9999px", MorphDuration = "0s", MorphEasing = "ease" },
        Elevation = new() { Level0 = "none", Level1 = $"0 1px 2px var({Css.Tokens.Color.ShadowUmbra})", Level2 = "x", Level3 = "x", Level4 = "x", Level5 = "x" },
        Motion = TestMotionTokens.Minimal,
        State = new() { HoverOpacity = "0.08", SelectedOpacity = "0.12", FocusOpacity = "0.12", PressedOpacity = "0.12", DraggedOpacity = "0.16", DisabledOpacity = "0.38", DisabledContainerOpacity = "0.12", HoverLayer = "currentColor", FocusLayer = "currentColor", PressedLayer = "currentColor", DraggedLayer = "currentColor", FocusHoverLayer = "currentColor", SelectedLayer = "currentColor", SelectedHoverLayer = "currentColor" },
        Extended = extended ?? new Dictionary<string, string> { ["--x"] = "1" },
    };

    private static ColorScheme Colors(string primary, string umbra = "rgba(0,0,0,0.2)") => new()
    {
        Primary = primary,
        OnPrimary = "#FFF",
        PrimaryContainer = "#FFF",
        OnPrimaryContainer = "#000",
        Secondary = "#000",
        OnSecondary = "#FFF",
        SecondaryContainer = "#FFF",
        OnSecondaryContainer = "#000",
        Tertiary = "#000",
        OnTertiary = "#FFF",
        TertiaryContainer = "#FFF",
        OnTertiaryContainer = "#000",
        Error = "#F00",
        OnError = "#FFF",
        ErrorContainer = "#FFF",
        OnErrorContainer = "#000",
        Success = "#0A0",
        OnSuccess = "#FFF",
        SuccessContainer = "#DFD",
        OnSuccessContainer = "#020",
        Warning = "#A60",
        OnWarning = "#FFF",
        WarningContainer = "#FED",
        OnWarningContainer = "#210",
        Info = "#06A",
        OnInfo = "#FFF",
        InfoContainer = "#DEF",
        OnInfoContainer = "#013",
        Surface = "#FFF",
        OnSurface = "#000",
        SurfaceVariant = "#EEE",
        OnSurfaceVariant = "#333",
        OnSurfaceVariant2 = "#444",
        SurfaceContainer = "#EEE",
        SurfaceContainerLowest = "#ffffff",
        SurfaceContainerLow = "#F5F5F5",
        SurfaceContainerHigh = "#DDD",
        SurfaceContainerHighest = "#CCC",
        SurfaceBright = "#CCC",
        SurfaceDim = "#CCC",
        PrimaryFixed = "#CCC",
        PrimaryFixedDim = "#CCC",
        OnPrimaryFixed = "#CCC",
        OnPrimaryFixedVariant = "#CCC",
        SecondaryFixed = "#CCC",
        SecondaryFixedDim = "#CCC",
        OnSecondaryFixed = "#CCC",
        OnSecondaryFixedVariant = "#CCC",
        TertiaryFixed = "#CCC",
        TertiaryFixedDim = "#CCC",
        OnTertiaryFixed = "#CCC",
        OnTertiaryFixedVariant = "#CCC",
        Background = "#FFF",
        OnBackground = "#000",
        Outline = "#999",
        OutlineVariant = "#CCC",
        InverseSurface = "#000",
        InverseOnSurface = "#FFF",
        InversePrimary = "#FFF",
        Scrim = "#000",
        Shadow = "#000",
        ShadowUmbra = umbra,
        ShadowPenumbra = "rgba(0,0,0,0.1)",
    };

    private sealed class FakeTheme(string id, DesignTokens design, IReadOnlyDictionary<string, string>? darkExt = null) : ITheme
    {
        public string Id => id;
        public string DisplayName => id;
        public DesignTokens Design => design;
        public string DefaultPaletteId => "p";
        public IReadOnlyList<string> StyleAssets => [];
        public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => darkExt;
    }

    [Fact]
    public void ColorsPlusDesign_Equals_FlattenOfComposite_NoDrift()
    {
        var design = Design();
        var colors = Colors("#123456");

        var split = new Dictionary<string, string>(colors.FlattenColors());
        foreach (var (k, v) in design.FlattenDesign()) split[k] = v;

        var composite = design.Flatten(colors);

        Assert.Equal(composite.Count, split.Count);
        foreach (var (k, v) in composite)
            Assert.Equal(v, split[k]);
    }

    [Fact]
    public void PaletteCss_EmitsLightAndDarkRules()
    {
        var css = TokensToCss.PaletteCss(new Palette
        {
            Id = "ocean",
            Name = "Ocean",
            Light = Colors("#0000FF"),
            Dark = Colors("#88AAFF"),
        });

        Assert.Contains(".flare-palette-ocean{", css);
        Assert.Contains($".flare-palette-ocean.{Css.Classes.Theme.ModeDark}", css);
        Assert.Contains($"{Css.Tokens.Color.Primary}:#0000FF;", css);
        Assert.Contains($"{Css.Tokens.Color.Primary}:#88AAFF;", css);
    }

    // Palette.ForMode falls back to Dark for high contrast; the CSS has to do the same, or an app root in
    // high-contrast mode matches no palette rule but the light one.
    [Fact]
    public void PaletteCss_WithoutHighContrast_DarkRuleAlsoCoversHighContrast()
    {
        var css = TokensToCss.PaletteCss(new Palette { Id = "ocean", Name = "Ocean", Light = Colors("#0000FF"), Dark = Colors("#88AAFF") });

        Assert.Contains($".flare-palette-ocean.{Css.Classes.Theme.ModeDark}, .flare-palette-ocean.{Css.Classes.Theme.ModeHighContrast}{{", css);
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(css, "#88AAFF"));
    }

    [Fact]
    public void PaletteCss_WithHighContrast_KeepsItsOwnRuleAfterDark()
    {
        var css = TokensToCss.PaletteCss(new Palette
        {
            Id = "ocean", Name = "Ocean", Light = Colors("#0000FF"), Dark = Colors("#88AAFF"), HighContrast = Colors("#FFFF00"),
        });

        var dark = css.IndexOf($".flare-palette-ocean.{Css.Classes.Theme.ModeDark}{{", StringComparison.Ordinal);
        var hc = css.IndexOf($".flare-palette-ocean.{Css.Classes.Theme.ModeHighContrast}{{", StringComparison.Ordinal);
        Assert.True(dark >= 0 && hc > dark, "the high-contrast rule must follow the dark rule it overrides");
    }

    // The progress track used to be painted with the secondary-container role directly, so a theme could only
    // recolor it by changing that role for everything else too.
    [Fact]
    public void ThemeCss_CarriesTheProgressTrackColor()
    {
        var css = TokensToCss.ThemeCss(new Flare.Theme.MaterialDesign3.MaterialDesign3Theme());

        Assert.Contains($"{Css.Tokens.ProgressField.TrackColor}:var({Css.Tokens.Color.SecondaryContainer});", css);
    }

    [Theory]
    [InlineData(false, false, Css.Classes.Theme.ModeLight)]
    [InlineData(true, false, Css.Classes.Theme.ModeDark)]
    [InlineData(true, true, Css.Classes.Theme.ModeHighContrast + " " + Css.Classes.Theme.ModeDark)]
    public void ModeClasses_HighContrastIsDarkToo(bool dark, bool highContrast, string expected) =>
        Assert.Equal(expected, Css.Classes.Theme.ModeClasses(dark, highContrast));

    [Fact]
    public void ThemeCss_EmitsDesignRule_AndDarkOverrideDiffOnly()
    {
        var design = Design(new Dictionary<string, string> { ["--a"] = "light", ["--b"] = "keep" });
        var darkExt = new Dictionary<string, string> { ["--a"] = "dark", ["--b"] = "keep" };
        var css = TokensToCss.ThemeCss(new FakeTheme("md3", design, darkExt));

        Assert.Contains(".flare-theme-md3{", css);
        Assert.Contains($".flare-theme-md3.{Css.Classes.Theme.ModeDark}{{", css);
        Assert.Contains("--a:dark;", css);   // changed key present
        Assert.DoesNotContain("--b:keep;.flare", css); // unchanged key not duplicated in the dark rule
    }
}
