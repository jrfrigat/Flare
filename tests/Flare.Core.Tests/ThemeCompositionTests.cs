using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Core.Tests;

public class ThemeCompositionTests
{
    private static DesignTokens MakeDesign() => new()
    {
        FocusRing = "2px solid #000",
        Typography = new()
        {
            DisplayLarge = T(),
            DisplayMedium = T(),
            DisplaySmall = T(),
            HeadlineLarge = T(),
            HeadlineMedium = T(),
            HeadlineSmall = T(),
            TitleLarge = T(),
            TitleMedium = T(),
            TitleSmall = T(),
            BodyLarge = T(),
            BodyMedium = T(),
            BodySmall = T(),
            LabelLarge = T(),
            LabelMedium = T(),
            LabelSmall = T(),
        },
        Shape = new() { None = "0", ExtraSmall = "4px", Small = "8px", Medium = "12px", Large = "16px", ExtraLarge = "28px", Full = "9999px" },
        Elevation = new()
        {
            Level0 = "none",
            Level1 = "0 1px 2px var(--flare-shadow-umbra), 0 1px 3px var(--flare-shadow-penumbra)",
            Level2 = "0 2px 4px var(--flare-shadow-umbra)",
            Level3 = "0 4px 8px var(--flare-shadow-umbra)",
            Level4 = "0 6px 10px var(--flare-shadow-umbra)",
            Level5 = "0 8px 12px var(--flare-shadow-umbra)",
        },
        Motion = new() { DurationShort1 = "50ms", DurationShort2 = "100ms", DurationMedium1 = "200ms", DurationMedium2 = "300ms", DurationLong1 = "450ms", DurationLong2 = "600ms", EasingStandard = "ease", EasingDecelerate = "ease-out", EasingAccelerate = "ease-in", EasingEmphasized = "ease" },
        State = new() { HoverOpacity = "0.08", FocusOpacity = "0.12", PressedOpacity = "0.12", DraggedOpacity = "0.16", DisabledOpacity = "0.38", DisabledContainerOpacity = "0.12" },
        Badge = new(),
        Alert = new(),
        Button = new(),
        SplitButton = new(),
        ToggleButton = new(),
        Fab = new(),
        Menu = new(),
        Checkbox = new(),
        Radio = new(),
        Chip = new(),
        Tabs = new(),
    };

    private static TypeStyle T() => new() { FontFamily = "Test", FontWeight = "400", FontSize = "1rem", LineHeight = "1.5rem", LetterSpacing = "0" };

    private static ColorScheme MakeColors(string primary, string umbra = "rgba(0,0,0,0.25)") => new()
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
        SurfaceContainer = "#EEE",
        SurfaceContainerLow = "#F5F5F5",
        SurfaceContainerHigh = "#DDD",
        SurfaceContainerHighest = "#CCC",
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
        ShadowPenumbra = "rgba(0,0,0,0.10)",
    };

    [Fact]
    public void Flatten_MapsColorsAndDesign()
    {
        var vars = MakeDesign().Flatten(MakeColors("#123456"));

        Assert.Equal("#123456", vars[Css.Tokens.Color.Primary]);
        Assert.Equal("2px solid #000", vars[Css.Tokens.Vars.FocusRing]);
        Assert.Equal("12px", vars[Css.Tokens.Shape.Medium]);
        Assert.Equal("rgba(0,0,0,0.25)", vars[Css.Tokens.Color.ShadowUmbra]);
        Assert.Equal("rgba(0,0,0,0.10)", vars[Css.Tokens.Color.ShadowPenumbra]);
    }

    [Fact]
    public void Flatten_LightVsDark_DiffersOnlyByScheme()
    {
        var design = MakeDesign();
        var light = design.Flatten(MakeColors("#111111", "rgba(0,0,0,0.14)"));
        var dark = design.Flatten(MakeColors("#EEEEEE", "rgba(0,0,0,0.30)"));

        Assert.Equal("#111111", light[Css.Tokens.Color.Primary]);
        Assert.Equal("#EEEEEE", dark[Css.Tokens.Color.Primary]);
        Assert.Equal("rgba(0,0,0,0.14)", light[Css.Tokens.Color.ShadowUmbra]);
        Assert.Equal("rgba(0,0,0,0.30)", dark[Css.Tokens.Color.ShadowUmbra]);
        // the design half is identical between the two modes
        foreach (var (key, value) in design.FlattenDesign())
            Assert.Equal(value, dark[key]);
    }

    [Fact]
    public void Flatten_EmitsShadowLayerVars()
    {
        var vars = MakeDesign().Flatten(MakeColors("#000000"));

        Assert.Equal("rgba(0,0,0,0.25)", vars[Css.Tokens.Color.ShadowUmbra]);
        Assert.Equal("rgba(0,0,0,0.10)", vars[Css.Tokens.Color.ShadowPenumbra]);
        // elevation geometry references the layer vars
        Assert.Contains("var(--flare-shadow-umbra)", vars[Css.Tokens.Elevation.Level1]);
    }
}
