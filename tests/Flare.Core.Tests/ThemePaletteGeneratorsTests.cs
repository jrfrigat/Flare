using System.Reflection;
using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Flare.Theme.Aero;
using Flare.Theme.FluentUI2;
using Flare.Theme.LiquidGlass;
using Flare.Theme.MaterialDesign3.Tokens;
using Flare.Theme.VisualStudio;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// Every theme ships a palette generator: the seed a user or the OS accent provides becomes a full
/// light and dark scheme. These are property checks - a valid role contract, determinism and the
/// light/dark relation - not snapshots of individual colors, which would freeze a palette nobody
/// asked to freeze.
/// </summary>
public class ThemePaletteGeneratorsTests
{
    private const string Seed = "#2B579A";

    public static IEnumerable<object[]> Generators =>
    [
        ["Fluent2", Fluent2RampGenerator.Instance],
        ["MD3", Md3TonalGenerator.Instance],
        ["Aero", AeroRampGenerator.Instance],
        ["LiquidGlass", LiquidGlassRampGenerator.Instance],
        ["VisualStudio", VisualStudioRampGenerator.Instance],
    ];

    private static readonly PropertyInfo[] RoleProperties = typeof(ColorScheme)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.PropertyType == typeof(string))
        .ToArray();

    private static string? Role(ColorScheme scheme, string name)
        => (string?)typeof(ColorScheme).GetProperty(name)!.GetValue(scheme);

    [Theory]
    [MemberData(nameof(Generators))]
    public void Generate_CarriesTheMetadataItWasGiven(string name, IPaletteGenerator generator)
    {
        var palette = generator.Generate("brand", "Brand", new PaletteSeed(Seed), "Generated");

        Assert.Equal("brand", palette.Id);
        Assert.Equal("Brand", palette.Name);
        Assert.Equal("Generated", palette.Source);
    }

    [Theory]
    [MemberData(nameof(Generators))]
    public void Generate_IsDeterministicForTheSameSeed(string name, IPaletteGenerator generator)
    {
        var first = generator.Generate("brand", "Brand", new PaletteSeed(Seed));
        var second = generator.Generate("brand", "Brand", new PaletteSeed(Seed));

        Assert.Equal(first, second);
    }

    [Theory]
    [MemberData(nameof(Generators))]
    public void Generate_ChangesWhenTheSeedChanges(string name, IPaletteGenerator generator)
    {
        var first = generator.Generate("brand", "Brand", new PaletteSeed(Seed));
        var second = generator.Generate("brand", "Brand", new PaletteSeed("#C2185B"));

        Assert.NotEqual(first, second);
    }

    [Theory]
    [MemberData(nameof(Generators))]
    public void Generate_FillsEveryRoleWithAValidColor(string name, IPaletteGenerator generator)
    {
        var palette = generator.Generate("brand", "Brand", new PaletteSeed(Seed));

        foreach (var (schemeName, scheme) in new[] { ("Light", palette.Light), ("Dark", palette.Dark) })
        {
            foreach (var role in RoleProperties)
            {
                var value = (string?)role.GetValue(scheme);
                Assert.True(IsColor(value), $"{name}/{schemeName}: role {role.Name} is '{value ?? "null"}'");
            }
        }
    }

    [Theory]
    [MemberData(nameof(Generators))]
    public void Generate_KeepsLightLightAndDarkDark(string name, IPaletteGenerator generator)
    {
        var palette = generator.Generate("brand", "Brand", new PaletteSeed(Seed));
        var lightSurface = Role(palette.Light, nameof(ColorScheme.Surface))!;
        var darkSurface = Role(palette.Dark, nameof(ColorScheme.Surface))!;

        Assert.NotEqual(lightSurface, darkSurface);
        Assert.True(ColorMath.Luminance(lightSurface) > ColorMath.Luminance(darkSurface),
            $"{name}: the light surface must be lighter than the dark one");

        Assert.True(ColorMath.Luminance(Role(palette.Light, nameof(ColorScheme.OnSurface))!)
                    < ColorMath.Luminance(lightSurface),
            $"{name}: on-surface must be darker than the light surface");
        Assert.True(ColorMath.Luminance(Role(palette.Dark, nameof(ColorScheme.OnSurface))!)
                    > ColorMath.Luminance(darkSurface),
            $"{name}: on-surface must be lighter than the dark surface");
    }

    [Fact]
    public void Md3_UsesTheBackgroundHintDeterministically()
    {
        var plain = Md3TonalGenerator.Instance.Generate("brand", "Brand", new PaletteSeed(Seed));
        var hinted = Md3TonalGenerator.Instance.Generate("brand", "Brand", new PaletteSeed(Seed, Background: "#FAFAF5"));
        var hintedAgain = Md3TonalGenerator.Instance.Generate("brand", "Brand", new PaletteSeed(Seed, Background: "#FAFAF5"));

        Assert.NotEqual(plain, hinted);
        Assert.Equal(hinted, hintedAgain);
    }

    private static bool IsColor(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value[0] == '#')
        {
            if (value.Length is not (4 or 7 or 9)) return false;
            for (var i = 1; i < value.Length; i++)
                if (!char.IsAsciiHexDigit(value[i])) return false;
            return true;
        }

        return value.StartsWith("rgb(", StringComparison.Ordinal)
            || value.StartsWith("rgba(", StringComparison.Ordinal)
            || value.StartsWith("hsl(", StringComparison.Ordinal)
            || value.StartsWith("hsla(", StringComparison.Ordinal);
    }
}
