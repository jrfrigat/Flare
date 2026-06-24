namespace Flare.Components.Tests;

public class FlareColorTests
{
    [Fact]
    public void Random_ReturnsCustomHexColor()
    {
        for (var i = 0; i < 50; i++)
        {
            var c = FlareColor.Random();
            Assert.True(c.IsCustom);
            Assert.NotNull(c.Value);
            Assert.Matches("^#[0-9A-Fa-f]{6}$", c.Value!);
        }
    }

    [Fact]
    public void Next_ReturnsCustomHex_DifferentFromInput()
    {
        var a = FlareColor.Custom("#3366CC");
        var b = FlareColor.Next(a);

        Assert.True(b.IsCustom);
        Assert.Matches("^#[0-9A-Fa-f]{6}$", b.Value!);
        Assert.NotEqual(a.Value, b.Value);
    }

    [Fact]
    public void Next_IsDeterministic()
    {
        var a = FlareColor.Custom("#3366CC");
        Assert.Equal(FlareColor.Next(a).Value, FlareColor.Next(a).Value);
    }

    [Fact]
    public void InstanceNext_EqualsStaticNext()
    {
        var a = FlareColor.Custom("#10B981");
        Assert.Equal(FlareColor.Next(a).Value, a.Next().Value);
    }

    [Fact]
    public void Next_FromDefaultOrRole_StartsFromSeed_NotPalette()
    {
        // Not tied to the palette: a role/default input yields a custom hex, not a role class.
        var fromDefault = FlareColor.Default.Next();
        var fromRole = FlareColor.Primary.Next();

        Assert.True(fromDefault.IsCustom);
        Assert.True(fromRole.IsCustom);
        Assert.Null(fromDefault.CssClass);
        // both start from the same seed, so they match
        Assert.Equal(fromDefault.Value, fromRole.Value);
    }

    [Fact]
    public void Next_SequenceProducesDistinctColors()
    {
        var seen = new List<string>();
        var c = FlareColor.Custom("#3366CC");
        for (var i = 0; i < 6; i++)
        {
            c = c.Next();
            seen.Add(c.Value!);
        }
        // golden-angle hue rotation keeps consecutive colors distinct
        Assert.Equal(6, seen.Distinct().Count());
    }

    [Fact]
    public void Dynamic_EmitsHarmonizedLightDarkTokenSet()
    {
        var c = FlareColor.Dynamic("#3F51B5");
        Assert.True(c.IsCustom);
        Assert.True(c.IsDynamic);
        var full = c.StyleFull();
        Assert.NotNull(full);
        Assert.Contains("--fc-main:", full);
        Assert.Contains("--fc-on:", full);
        Assert.Contains("--fc-container:", full);
        Assert.Contains("--fc-on-container:", full);
        Assert.Contains("light-dark(", full);   // mode-adaptive
    }

    [Fact]
    public void Dynamic_InvalidSource_FallsBackToDefault()
    {
        var c = FlareColor.Dynamic("not-a-color");
        Assert.True(c.IsDefault);
        Assert.False(c.IsDynamic);
    }

    [Fact]
    public void Dynamic_Monochrome_IsDynamicWithMainToken()
    {
        var c = FlareColor.Dynamic("#3F51B5", DynamicVariant.Monochrome);
        Assert.True(c.IsDynamic);
        Assert.NotNull(c.StyleMain());
    }
}
