using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareThemeScopeTests : FlareTestContext
{
    [Fact]
    public void Mode_Dark_AppliesThemePaletteAndDarkModeClasses()
    {
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService())
            .Add(x => x.Mode, ThemeMode.Dark)
            .AddChildContent("<span class=\"kid\">x</span>"));
        var cls = cut.Find("div").ClassName;
        Assert.Contains(Css.Classes.Theme.Root, cls);
        Assert.Contains("flare-theme-stub", cls);
        Assert.Contains("flare-palette-stub", cls);
        Assert.Contains(Css.Classes.Theme.ModeDark, cls);
        Assert.NotEmpty(cut.FindAll(".kid"));
    }

    [Fact]
    public void UnsetMode_InheritsOuterMode()
    {
        // Stub outer is light -> an unset Mode inherits light (explicit light class re-asserts it).
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService())
            .AddChildContent("<span>x</span>"));
        var cls = cut.Find("div").ClassName;
        Assert.Contains(Css.Classes.Theme.ModeLight, cls);
        Assert.DoesNotContain(Css.Classes.Theme.ModeDark, cls);
    }
}
