using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Flare.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public class FlareThemeScopeTests : FlareTestContext
{
    private sealed class Probe : ComponentBase
    {
        [CascadingParameter] public ThemeSnapshot? Snapshot { get; set; }
        [CascadingParameter] public IThemeService? Theme { get; set; }
        public int Changed { get; private set; }

        protected override void OnInitialized()
        {
            if (Theme is not null) Theme.OnThemeChanged += OnThemeChanged;
        }

        private Task OnThemeChanged()
        {
            Changed++;
            return Task.CompletedTask;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder) { }
    }

    private static RenderFragment ProbeContent() => builder =>
    {
        builder.OpenComponent<Probe>(0);
        builder.CloseComponent();
    };

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
    public void ADerivedTheme_CarriesTheClassOfEveryGeneration()
    {
        var gold = new StubTheme().Derive("better").Derive("gold");
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService(gold))
            .AddChildContent("<span>x</span>"));

        var classes = cut.Find("div").ClassList;
        Assert.Contains("flare-theme-gold", classes);
        Assert.Contains("flare-theme-better", classes);
        Assert.Contains("flare-theme-stub", classes);
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

    [Fact]
    public void HighContrastMode_ReachesClassesSnapshotAndScopedService()
    {
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService())
            .Add(x => x.Mode, ThemeMode.HighContrast)
            .AddChildContent(ProbeContent()));

        var classes = cut.Find("div").ClassList;
        Assert.Contains(Css.Classes.Theme.ModeHighContrast, classes);
        Assert.Contains(Css.Classes.Theme.ModeDark, classes);
        var probe = cut.FindComponent<Probe>().Instance;
        Assert.True(probe.Snapshot!.IsHighContrast);
        Assert.True(probe.Theme!.IsHighContrast);
    }

    [Fact]
    public void OuterRtl_ReachesClassesSnapshotAndScopedService()
    {
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(new StubThemeService { Rtl = true })
            .AddChildContent(ProbeContent()));

        Assert.Contains(Css.Classes.Theme.Rtl, cut.Find("div").ClassList);
        var probe = cut.FindComponent<Probe>().Instance;
        Assert.True(probe.Snapshot!.IsRtl);
        Assert.True(probe.Theme!.IsRtl);
    }

    [Fact]
    public void ChangedParameters_NotifyScopedSubscribers()
    {
        var service = new StubThemeService();
        var cut = Render<FlareThemeScope>(p => p
            .AddCascadingValue<IThemeService>(service)
            .Add(x => x.Mode, ThemeMode.Dark)
            .AddChildContent(ProbeContent()));
        var probe = cut.FindComponent<Probe>().Instance;
        Assert.Equal(0, probe.Changed);

        cut.Render(p => p
            .Add(x => x.Mode, ThemeMode.Light)
            .AddChildContent(ProbeContent()));

        Assert.True(cut.FindComponent<Probe>().Instance.Changed > 0);
    }
}
