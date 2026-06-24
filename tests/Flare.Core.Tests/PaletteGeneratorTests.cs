using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Core.Tests;

public class PaletteGeneratorTests
{
    [Fact]
    public void FromColors_SetsBrandAndIsModeDistinct()
    {
        var p = Palette.FromColors("brand", "Brand", "#2B579A", source: "Generated");

        Assert.Equal("brand", p.Id);
        Assert.Equal("Generated", p.Source);
        Assert.Equal("#2B579A", p.Light.Primary);
        Assert.Equal("#2B579A", p.Light.Info);
        Assert.Equal("#FFFFFF", p.Light.OnPrimary);     // readable on a mid-dark blue
        Assert.NotEqual(p.Light.Primary, p.Dark.Primary); // dark mode brightens the brand
    }

    [Fact]
    public void FromColors_LightSurfacesAreLight_DarkSurfacesAreDark()
    {
        var p = Palette.FromColors("x", "X", "#1E8E3E");

        Assert.True(ColorMath.Luminance(p.Light.Surface) > 0.8, "light surface should be light");
        Assert.True(ColorMath.Luminance(p.Light.OnSurface) < 0.2, "light on-surface should be dark");
        Assert.True(ColorMath.Luminance(p.Dark.Surface) < 0.2, "dark surface should be dark");
        Assert.True(ColorMath.Luminance(p.Dark.OnSurface) > 0.8, "dark on-surface should be light");
    }

    [Fact]
    public void FromColors_BackgroundTintsSurface()
    {
        var p = Palette.FromColors("y", "Y", "#0B57D0", background: "#FAFAF5");
        Assert.Equal("#FAFAF5", p.Light.Surface);
    }

    [Fact]
    public void Generated_Palette_Flattens_To_Full_ColorVarSet()
    {
        var p = Palette.FromColors("z", "Z", "#00897B");
        var lightVars = p.Light.FlattenColors();

        // a representative spread of roles is present and non-empty
        foreach (var key in new[]
        {
            "--flare-color-primary", "--flare-color-on-surface", "--flare-color-surface-container",
            "--flare-color-outline", "--flare-color-error", "--flare-shadow-umbra",
        })
        {
            Assert.True(lightVars.ContainsKey(key), $"missing {key}");
            Assert.False(string.IsNullOrWhiteSpace(lightVars[key]));
        }
    }
}
