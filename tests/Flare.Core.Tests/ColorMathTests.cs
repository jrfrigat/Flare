using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Core.Tests;

public class ColorMathTests
{
    [Theory]
    [InlineData("#FFFFFF", "#1A1A1A")] // dark text on white
    [InlineData("#000000", "#FFFFFF")] // white text on black
    [InlineData("#0B57D0", "#FFFFFF")] // white on a mid-dark blue
    public void OnColor_PicksReadableForeground(string bg, string expected)
    {
        Assert.Equal(expected, ColorMath.OnColor(bg));
    }

    [Fact]
    public void Lighten_And_Darken_MoveTowardWhiteAndBlack()
    {
        Assert.Equal("#FFFFFF", ColorMath.Lighten("#808080", 1.0));
        Assert.Equal("#000000", ColorMath.Darken("#808080", 1.0));
        Assert.Equal("#808080", ColorMath.Lighten("#808080", 0.0));
    }

    [Fact]
    public void Parse_Handles3And6Digit()
    {
        Assert.Equal((255, 0, 0), ColorMath.Parse("#F00"));
        Assert.Equal((11, 87, 208), ColorMath.Parse("#0B57D0"));
    }

    [Fact]
    public void PaletteFactory_Brand_AppliesSeedToPrimaryAndInfo()
    {
        var baseScheme = MakeColors("#111111");
        var p = PaletteFactory.Brand("brand-x", "Brand X", baseScheme, baseScheme, "#2B579A");

        Assert.Equal("brand-x", p.Id);
        Assert.Equal("#2B579A", p.Light.Primary);
        Assert.Equal("#2B579A", p.Light.Info);
        Assert.Equal("#FFFFFF", p.Light.OnPrimary);          // readable on the dark-ish blue
        // neutrals stay from the base scheme
        Assert.Equal(baseScheme.Surface, p.Light.Surface);
        Assert.Equal(baseScheme.Outline, p.Light.Outline);
        // dark mode brightens the brand and keeps it distinct from light
        Assert.NotEqual(p.Light.Primary, p.Dark.Primary);
    }

    private static ColorScheme MakeColors(string primary) => new()
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
        Surface = "#FAFAFA",
        OnSurface = "#000",
        SurfaceVariant = "#EEE",
        OnSurfaceVariant = "#333",
        SurfaceContainer = "#EEE",
        SurfaceContainerLow = "#F5F5F5",
        SurfaceContainerHigh = "#DDD",
        SurfaceContainerHighest = "#CCC",
        Background = "#FFF",
        OnBackground = "#000",
        Outline = "#999999",
        OutlineVariant = "#CCC",
        InverseSurface = "#000",
        InverseOnSurface = "#FFF",
        InversePrimary = "#FFF",
        Scrim = "#000",
        Shadow = "#000",
        ShadowUmbra = "rgba(0,0,0,0.3)",
        ShadowPenumbra = "rgba(0,0,0,0.15)",
    };
}
