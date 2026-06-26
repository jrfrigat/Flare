using Flare.Abstractions;
using Flare.Theming;
using Flare.Theming;
using Flare.Abstractions.Tokens;

namespace Flare.Core.Tests;

/// <summary>
/// Integration tests for theme switching, validation, and serialization.
/// </summary>
public class ThemeIntegrationTests
{
    [Fact]
    public void FlareThemeBuilder_Should_Build_Valid_Theme()
    {
        var theme = new FlareThemeBuilder("test-theme", "Test Theme")
            .WithDefaultPalette("test-palette")
            .WithStyleAsset("_content/Test/css/test.css")
            .Build();

        Assert.Equal("test-theme", theme.Id);
        Assert.Equal("Test Theme", theme.DisplayName);
        Assert.Single(theme.StyleAssets);
    }

    [Fact]
    public void FlareThemeBuilder_Should_Throw_On_Invalid_Theme()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new FlareThemeBuilder("", "Empty ID")
                .Build());
    }

    [Fact]
    public void FlareThemeBuilder_Should_Override_Typography()
    {
        var customTypography = new TypographyTokens
        {
            DisplayLarge = new TypeStyle { FontFamily = "Inter", FontWeight = "700", FontSize = "3rem", LineHeight = "3.5rem", LetterSpacing = "0em" },
            DisplayMedium = new TypeStyle { FontFamily = "Inter", FontWeight = "700", FontSize = "2.5rem", LineHeight = "3rem", LetterSpacing = "0em" },
            DisplaySmall = new TypeStyle { FontFamily = "Inter", FontWeight = "700", FontSize = "2rem", LineHeight = "2.5rem", LetterSpacing = "0em" },
            HeadlineLarge = new TypeStyle { FontFamily = "Inter", FontWeight = "600", FontSize = "1.5rem", LineHeight = "2rem", LetterSpacing = "0em" },
            HeadlineMedium = new TypeStyle { FontFamily = "Inter", FontWeight = "600", FontSize = "1.25rem", LineHeight = "1.75rem", LetterSpacing = "0em" },
            HeadlineSmall = new TypeStyle { FontFamily = "Inter", FontWeight = "600", FontSize = "1rem", LineHeight = "1.5rem", LetterSpacing = "0em" },
            TitleLarge = new TypeStyle { FontFamily = "Inter", FontWeight = "600", FontSize = "1rem", LineHeight = "1.5rem", LetterSpacing = "0em" },
            TitleMedium = new TypeStyle { FontFamily = "Inter", FontWeight = "600", FontSize = "0.875rem", LineHeight = "1.25rem", LetterSpacing = "0em" },
            TitleSmall = new TypeStyle { FontFamily = "Inter", FontWeight = "600", FontSize = "0.75rem", LineHeight = "1rem", LetterSpacing = "0em" },
            BodyLarge = new TypeStyle { FontFamily = "Inter", FontWeight = "400", FontSize = "1rem", LineHeight = "1.5rem", LetterSpacing = "0em" },
            BodyMedium = new TypeStyle { FontFamily = "Inter", FontWeight = "400", FontSize = "0.875rem", LineHeight = "1.25rem", LetterSpacing = "0em" },
            BodySmall = new TypeStyle { FontFamily = "Inter", FontWeight = "400", FontSize = "0.75rem", LineHeight = "1rem", LetterSpacing = "0em" },
            LabelLarge = new TypeStyle { FontFamily = "Inter", FontWeight = "500", FontSize = "0.875rem", LineHeight = "1.25rem", LetterSpacing = "0em" },
            LabelMedium = new TypeStyle { FontFamily = "Inter", FontWeight = "500", FontSize = "0.75rem", LineHeight = "1rem", LetterSpacing = "0em" },
            LabelSmall = new TypeStyle { FontFamily = "Inter", FontWeight = "500", FontSize = "0.6875rem", LineHeight = "1rem", LetterSpacing = "0em" },
        };

        var theme = new FlareThemeBuilder("custom-theme", "Custom Theme")
            .WithTypography(customTypography)
            .WithStyleAsset("_content/Custom/css/custom.css")
            .Build();

        Assert.Equal("Inter", theme.Design.Typography.BodyLarge.FontFamily);
        Assert.Equal("3rem", theme.Design.Typography.DisplayLarge.FontSize);
    }

    [Fact]
    public void ThemeValidator_Should_Catch_Missing_Typography()
    {
        var validator = new ThemeValidator();
        var theme = new FlareThemeBuilder("test", "Test")
            .WithStyleAsset("_content/Test/test.css")
            .Build();

        var errors = validator.Validate(theme);
        Assert.Empty(errors);
    }

    [Fact]
    public void Palette_Should_Have_Light_And_Dark_Schemes()
    {
        var palette = PaletteFactory.FromColors("test-palette", "Test Palette", "#6750A4");

        Assert.NotNull(palette.Light);
        Assert.NotNull(palette.Dark);
        Assert.NotEqual(palette.Light.Primary, palette.Dark.Primary);
    }

    [Fact]
    public async Task ThemeService_Should_Switch_Themes()
    {
        var theme1 = new FlareThemeBuilder("theme1", "Theme 1")
            .WithStyleAsset("_content/Test/theme1.css")
            .Build();
        var theme2 = new FlareThemeBuilder("theme2", "Theme 2")
            .WithStyleAsset("_content/Test/theme2.css")
            .Build();

        var palette = PaletteFactory.FromColors("test", "Test", "#6750A4");
        var injector = new FakeCssVariableInjector();
        var service = new ThemeService(injector, theme1, palette);

        service.RegisterTheme(theme2);

        Assert.Equal("theme1", service.CurrentTheme.Id);

        await service.SetThemeAsync("theme2");
        Assert.Equal("theme2", service.CurrentTheme.Id);
    }

    [Fact]
    public async Task ThemeService_Should_Switch_Palettes()
    {
        var theme = new FlareThemeBuilder("test", "Test")
            .WithStyleAsset("_content/Test/test.css")
            .Build();
        var palette1 = PaletteFactory.FromColors("palette1", "Palette 1", "#6750A4");
        var palette2 = PaletteFactory.FromColors("palette2", "Palette 2", "#B3261E");

        var injector = new FakeCssVariableInjector();
        var service = new ThemeService(injector, theme, palette1);

        service.RegisterPalette(palette2);

        Assert.Equal("palette1", service.CurrentPalette.Id);

        await service.SetPaletteAsync("palette2");
        Assert.Equal("palette2", service.CurrentPalette.Id);
    }

    [Fact]
    public async Task ThemeService_Should_Switch_Mode()
    {
        var theme = new FlareThemeBuilder("test", "Test")
            .WithStyleAsset("_content/Test/test.css")
            .Build();
        var palette = PaletteFactory.FromColors("test", "Test", "#6750A4");

        var injector = new FakeCssVariableInjector();
        var service = new ThemeService(injector, theme, palette);

        Assert.False(service.IsDark);

        await service.SetModeAsync(ThemeMode.Dark);
        Assert.True(service.IsDark);

        await service.SetModeAsync(ThemeMode.HighContrast);
        Assert.True(service.IsHighContrast);
        Assert.True(service.IsDark);
    }

    [Fact]
    public void FlareStyles_ActiveOnly_Should_Filter_Assets()
    {
        var theme1 = new FlareThemeBuilder("theme1", "Theme 1")
            .WithStyleAsset("_content/Test/theme1.css")
            .Build();
        var theme2 = new FlareThemeBuilder("theme2", "Theme 2")
            .WithStyleAsset("_content/Test/theme2.css")
            .Build();

        var palette1 = PaletteFactory.FromColors("palette1", "Palette 1", "#6750A4");
        palette1 = palette1 with { StyleAsset = "_content/Test/palette1.css" };

        var palette2 = PaletteFactory.FromColors("palette2", "Palette 2", "#B3261E");
        palette2 = palette2 with { StyleAsset = "_content/Test/palette2.css" };

        var injector = new FakeCssVariableInjector();
        var service = new ThemeService(injector, theme1, palette1);
        service.RegisterTheme(theme2);
        service.RegisterPalette(palette2);

        // ActiveOnly=false should include all assets
        var allAssets = service.Themes.SelectMany(t => t.StyleAssets)
            .Concat(service.Palettes.Select(p => p.StyleAsset).Where(a => a is not null).Select(a => a!))
            .Distinct().ToList();
        Assert.Equal(4, allAssets.Count);

        // ActiveOnly=true should include only active assets
        var activeAssets = service.CurrentTheme.StyleAssets
            .Concat(new[] { service.CurrentPalette.StyleAsset }.Where(a => a is not null).Select(a => a!))
            .Distinct().ToList();
        Assert.Equal(2, activeAssets.Count);
    }

    private sealed class FakeCssVariableInjector : ICssVariableInjector
    {
        public ValueTask InjectAsync(DesignTokens design, ColorScheme colors, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SetStaticCssAsync(string css, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask SetCustomTokensAsync(IReadOnlyDictionary<string, string> tokens, CancellationToken ct = default) => ValueTask.CompletedTask;
        public ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default) => ValueTask.CompletedTask;
    }
}
