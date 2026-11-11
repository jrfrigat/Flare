using Flare.Abstractions.Tokens;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// The serializer writes through generated type metadata rather than reflection, so a type the
/// generation missed would throw at run time rather than at build time. A round trip is what
/// proves the palette graph is whole.
/// </summary>
public sealed class ThemeJsonSerializerTests
{
    [Fact]
    public void APaletteSurvivesARoundTrip()
    {
        var palette = PaletteFactory.FromColors("brand", "Brand", "#6750A4", source: "test");

        var restored = ThemeJsonSerializer.ImportPalette(ThemeJsonSerializer.ExportPalette(palette));

        Assert.Equal(palette, restored);
    }

    [Fact]
    public void PaletteJsonKeepsTheCamelCaseNames()
    {
        var palette = PaletteFactory.FromColors("brand", "Brand", "#6750A4");

        var json = ThemeJsonSerializer.ExportPalette(palette);

        Assert.Contains("\"light\":", json);
        Assert.DoesNotContain("\"Light\":", json);
    }
}
