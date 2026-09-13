using Flare.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public sealed class FlareThemeProviderStylesheetsTests : FlareTestContext
{
    public FlareThemeProviderStylesheetsTests() =>
        Services.AddSingleton<IThemeStorageService, NothingSaved>();

    [Fact]
    public void ByDefaultTheProviderWritesTheThemeLinks()
    {
        var cut = Render<FlareThemeProvider>();

        Assert.Single(cut.FindComponents<FlareStyles>());
    }

    /// <summary>
    /// An app that links the theme stylesheets in its own head must not get a second set: the same
    /// sheet would be applied twice, the later copy moving the theme after the app's own overrides.
    /// </summary>
    [Fact]
    public void InManualModeTheProviderWritesNoLinks()
    {
        var cut = Render<FlareThemeProvider>(p => p.Add(x => x.Stylesheets, ThemeStylesheets.Manual));

        Assert.Empty(cut.FindComponents<FlareStyles>());
    }

    private sealed class NothingSaved : IThemeStorageService
    {
        public Task<ThemeSelection?> GetAsync() => Task.FromResult<ThemeSelection?>(null);
        public Task SaveAsync(ThemeSelection selection) => Task.CompletedTask;
    }
}
