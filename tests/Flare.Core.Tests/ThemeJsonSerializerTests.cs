using Flare.Abstractions.Tokens;
using Flare.Theme.MaterialDesign3;
using Flare.Theming;
using System.Text.Json.Nodes;

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

    [Fact]
    public void ImportsAThemeSavedWithoutCaptionTokens()
    {
        var theme = new MaterialDesign3Theme();
        var json = JsonNode.Parse(ThemeJsonSerializer.ExportTheme(theme))!.AsObject();
        json["design"]!.AsObject().Remove("caption");

        var restored = ThemeJsonSerializer.ImportTheme(json.ToJsonString());

        Assert.Equal(theme.Id, restored.Id);
        Assert.Equal(theme.Design.FocusRing, restored.Design.FocusRing);
        var exported = JsonNode.Parse(ThemeJsonSerializer.ExportTheme(restored))!.AsObject();
        Assert.False(exported["design"]!.AsObject().ContainsKey("caption"));
    }
}
