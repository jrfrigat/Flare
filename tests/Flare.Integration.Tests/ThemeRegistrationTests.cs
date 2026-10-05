using Flare.Abstractions;
using Flare.Extensions;
using Flare.Theme.MaterialDesign3;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Integration.Tests;

/// <summary>Exercises explicit theme registration through the real composition root.</summary>
public sealed class ThemeRegistrationTests : BunitContext
{
    [Fact]
    public void ReferencedPackage_DoesNotRegisterATheme()
    {
        Services.AddFlare();
        var error = Assert.Throws<InvalidOperationException>(() => Services.GetRequiredService<IThemeService>());
        Assert.Contains("no theme is registered", error.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExplicitTheme_IsRegisteredBeforeOrAfterAddFlare(bool before)
    {
        var theme = new MaterialDesign3Theme();
        if (before) Services.AddFlareTheme(theme);
        Services.AddFlare(options => options.DefaultThemeId = theme.Id);
        if (!before) Services.AddFlareTheme(theme);

        var service = Services.GetRequiredService<IThemeService>();
        Assert.Same(theme, service.CurrentTheme);
        Assert.Same(theme, Assert.Single(service.Themes));
        Assert.Contains(service.Palettes, palette => palette.Id == theme.DefaultPaletteId);
    }
}
