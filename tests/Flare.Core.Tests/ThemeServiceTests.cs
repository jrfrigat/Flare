using Flare.Core.Abstractions;
using Flare.Core.Services;
using Flare.Core.Tokens;

namespace Flare.Core.Tests;

public class ThemeServiceTests
{
    private static TypeStyle TS() => new() { FontFamily = "Test", FontWeight = "400", FontSize = "1rem", LineHeight = "1.5rem", LetterSpacing = "0" };

    private static DesignTokens Design() => new()
    {
        FocusRing = "2px solid #000",
        Typography = new()
        {
            DisplayLarge = TS(),
            DisplayMedium = TS(),
            DisplaySmall = TS(),
            HeadlineLarge = TS(),
            HeadlineMedium = TS(),
            HeadlineSmall = TS(),
            TitleLarge = TS(),
            TitleMedium = TS(),
            TitleSmall = TS(),
            BodyLarge = TS(),
            BodyMedium = TS(),
            BodySmall = TS(),
            LabelLarge = TS(),
            LabelMedium = TS(),
            LabelSmall = TS(),
        },
        Shape = new() { None = "0", ExtraSmall = "4px", Small = "8px", Medium = "12px", Large = "16px", ExtraLarge = "28px", Full = "9999px" },
        Elevation = new() { Level0 = "none", Level1 = "0 1px 2px var(--flare-shadow-umbra)", Level2 = "x", Level3 = "x", Level4 = "x", Level5 = "x" },
        Motion = new() { DurationShort1 = "50ms", DurationShort2 = "100ms", DurationMedium1 = "200ms", DurationMedium2 = "300ms", DurationLong1 = "450ms", DurationLong2 = "600ms", EasingStandard = "ease", EasingDecelerate = "ease-out", EasingAccelerate = "ease-in", EasingEmphasized = "ease" },
        State = new() { HoverOpacity = "0.08", FocusOpacity = "0.12", PressedOpacity = "0.12", DraggedOpacity = "0.16", DisabledOpacity = "0.38", DisabledContainerOpacity = "0.12" },
        Badge = new(),
        Alert = new(),
        Button = new(),
        SplitButton = new(),
        ToggleButton = new(),
        Fab = new(),
        Menu = new(),
        Checkbox = new(),
        Radio = new(),
        Chip = new(),
        Tabs = new(),
    };

    private static ColorScheme Colors(string primary) => new()
    {
        Primary = primary,
        OnPrimary = "#FFF",
        PrimaryContainer = "#FFF",
        OnPrimaryContainer = "#000",
        Secondary = "#000",
        OnSecondary = "#FFF",
        SecondaryContainer = "#FFF",
        OnSecondaryContainer = "#000",
        Tertiary = "#000",
        OnTertiary = "#FFF",
        TertiaryContainer = "#FFF",
        OnTertiaryContainer = "#000",
        Error = "#F00",
        OnError = "#FFF",
        ErrorContainer = "#FFF",
        OnErrorContainer = "#000",
        Success = "#0A0",
        OnSuccess = "#FFF",
        SuccessContainer = "#DFD",
        OnSuccessContainer = "#020",
        Warning = "#A60",
        OnWarning = "#FFF",
        WarningContainer = "#FED",
        OnWarningContainer = "#210",
        Info = "#06A",
        OnInfo = "#FFF",
        InfoContainer = "#DEF",
        OnInfoContainer = "#013",
        Surface = "#FFF",
        OnSurface = "#000",
        SurfaceVariant = "#EEE",
        OnSurfaceVariant = "#333",
        SurfaceContainer = "#EEE",
        SurfaceContainerLow = "#F5F5F5",
        SurfaceContainerHigh = "#DDD",
        SurfaceContainerHighest = "#CCC",
        Background = "#FFF",
        OnBackground = "#000",
        Outline = "#999",
        OutlineVariant = "#CCC",
        InverseSurface = "#000",
        InverseOnSurface = "#FFF",
        InversePrimary = "#FFF",
        Scrim = "#000",
        Shadow = "#000",
    };

    private static Palette Pal(string id, string light = "#111111", string dark = "#EEEEEE") =>
        new() { Id = id, Name = id, Light = Colors(light), Dark = Colors(dark) };

    private sealed class FakeTheme(string id, IPaletteGenerator? gen = null) : ITheme
    {
        public string Id => id;
        public string DisplayName => id;
        public DesignTokens Design { get; } = ThemeServiceTests.Design();
        public string DefaultPaletteId => "p1";
        public IReadOnlyList<string> StyleAssets => [];
        public IPaletteGenerator? PaletteGenerator => gen;
    }

    private sealed class FakeGen : IPaletteGenerator
    {
        public Palette Generate(string id, string name, PaletteSeed seed, string? source = null) =>
            new() { Id = id, Name = "FAKE", Source = source, Light = Colors("#ABCDEF"), Dark = Colors("#123456") };
    }

    private sealed class FakeInjector : ICssVariableInjector
    {
        public DesignTokens? LastDesign { get; private set; }
        public ColorScheme? LastColors { get; private set; }
        public int InjectCount { get; private set; }
        public string? LastStaticCss { get; private set; }
        public ValueTask InjectAsync(DesignTokens design, ColorScheme colors, CancellationToken ct = default)
        {
            LastDesign = design; LastColors = colors; InjectCount++; return ValueTask.CompletedTask;
        }
        public ValueTask SetStaticCssAsync(string css, CancellationToken ct = default)
        {
            LastStaticCss = css; return ValueTask.CompletedTask;
        }
        public Dictionary<string, string> Custom { get; } = new();
        public ValueTask SetCustomTokensAsync(IReadOnlyDictionary<string, string> tokens, CancellationToken ct = default)
        {
            foreach (var (k, v) in tokens) Custom[k] = v;
            return ValueTask.CompletedTask;
        }
        public ValueTask ClearCustomTokensAsync(IEnumerable<string> tokenNames, CancellationToken ct = default)
        {
            foreach (var n in tokenNames) Custom.Remove(n);
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>A theme whose Extended dict shadows a component token's CSS var (like Fluent2 does).</summary>
    private sealed class ExtShadowTheme : ITheme
    {
        public string Id => "ext";
        public string DisplayName => "ext";
        public DesignTokens Design { get; } = ThemeServiceTests.Design() with
        {
            Extended = new Dictionary<string, string> { ["--flare-card-radius"] = "var(--flare-shape-medium)" },
        };
        public string DefaultPaletteId => "p1";
        public IReadOnlyList<string> StyleAssets => [];
    }

    private static ThemeService Make(out FakeInjector injector, ThemeDelivery delivery = ThemeDelivery.ClassToggle)
    {
        injector = new FakeInjector();
        return new ThemeService(injector, new FakeTheme("t1"), Pal("p1"), ThemeMode.Light, delivery);
    }

    [Fact]
    public void CustomizeColors_PushesOnlyChangedRoles()
    {
        var injector = new FakeInjector();
        var s = new ThemeService(injector, new FakeTheme("t1"), Pal("p1"), ThemeMode.Light);

        s.CustomizeColors(c => c with { Primary = "#123456" });

        Assert.Equal("#123456", injector.Custom["--flare-color-primary"]);
        Assert.False(injector.Custom.ContainsKey("--flare-color-secondary"));
    }

    [Fact]
    public void CustomizeDesign_EditedComponentToken_WinsOverShadowingExtended()
    {
        var injector = new FakeInjector();
        var s = new ThemeService(injector, new ExtShadowTheme(), Pal("p1"), ThemeMode.Light);

        // The theme's Extended shadows --flare-card-radius; editing the typed Card.Radius must still win.
        s.CustomizeDesign(d => d with { Card = d.Card with { Radius = "2px" } });

        Assert.Equal("2px", injector.Custom["--flare-card-radius"]);
    }

    [Fact]
    public void Constructor_SetsDefaults()
    {
        var s = Make(out _);
        Assert.Equal("t1", s.CurrentTheme.Id);
        Assert.Equal("p1", s.CurrentPalette.Id);
        Assert.Equal(ThemeMode.Light, s.Mode);
        Assert.False(s.IsDark);
        Assert.Equal(ThemeDelivery.ClassToggle, s.Delivery);
    }

    [Fact]
    public async Task ClassToggle_StaticBundle_ContainsActiveThemeAndPalette()
    {
        var s = Make(out var inj);
        s.RegisterPalette(Pal("p2", light: "#222222"));
        await s.EnsureStaticCssAsync();

        // Only the active theme/palette are emitted up front -- not every registered palette.
        Assert.Contains(".flare-theme-t1{", inj.LastStaticCss);
        Assert.Contains(".flare-palette-p1{", inj.LastStaticCss);
        Assert.DoesNotContain(".flare-palette-p2{", inj.LastStaticCss);
    }

    [Fact]
    public async Task ClassToggle_RequireThemeAssets_AddsNonActivePaletteToBundle()
    {
        var s = Make(out var inj);
        s.RegisterPalette(Pal("p2", light: "#222222"));
        await s.EnsureStaticCssAsync();
        Assert.DoesNotContain(".flare-palette-p2{", inj.LastStaticCss);

        // A FlareThemeScope requesting p2 makes its class rules available without switching.
        await s.RequireThemeAssetsAsync(null, "p2");

        Assert.Contains(".flare-palette-p1{", inj.LastStaticCss);   // active stays
        Assert.Contains(".flare-palette-p2{", inj.LastStaticCss);
        Assert.Contains("--flare-color-primary:#222222;", inj.LastStaticCss);
    }

    [Fact]
    public async Task ClassToggle_SetPalette_SwitchesWithoutVarInjection()
    {
        var s = Make(out var inj);
        s.RegisterPalette(Pal("p2", light: "#222222"));
        await s.SetPaletteAsync("p2");

        Assert.Equal("p2", s.CurrentPalette.Id);
        Assert.Equal(0, inj.InjectCount);            // no full var injection
        Assert.NotNull(inj.LastStaticCss);            // static bundle applied
    }

    [Fact]
    public async Task Inject_SetPalette_InjectsLightColors()
    {
        var s = Make(out var inj, ThemeDelivery.Inject);
        s.RegisterPalette(Pal("p2", light: "#222222"));
        await s.SetPaletteAsync("p2");

        Assert.Equal("#222222", inj.LastColors!.Primary);
    }

    [Fact]
    public async Task Inject_SetMode_Dark_InjectsDarkColors()
    {
        var s = Make(out var inj, ThemeDelivery.Inject);
        await s.SetModeAsync(ThemeMode.Dark);

        Assert.True(s.IsDark);
        Assert.Equal("#EEEEEE", inj.LastColors!.Primary);
    }

    [Fact]
    public async Task AutoMode_FollowsSystemDark()
    {
        var s = Make(out _);
        await s.SetModeAsync(ThemeMode.Auto);
        Assert.False(s.IsDark);

        await s.SetSystemDarkAsync(true);
        Assert.True(s.IsDark);
    }

    [Fact]
    public async Task SetTheme_SwitchesAndFiresEvent()
    {
        var s = Make(out _);
        s.RegisterTheme(new FakeTheme("t2"));
        var fired = false;
        s.OnThemeChanged += () => { fired = true; return Task.CompletedTask; };

        await s.SetThemeAsync("t2");

        Assert.Equal("t2", s.CurrentTheme.Id);
        Assert.True(fired);
    }

    [Fact]
    public async Task SetTheme_ThrowsForUnregistered()
    {
        var s = Make(out _);
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.SetThemeAsync("nope"));
    }

    [Fact]
    public void GeneratePalette_UsesDefaultGenerator_WhenThemeHasNone()
    {
        var s = Make(out _);
        var p = s.GeneratePalette("g", "G", new PaletteSeed("#2B579A"));
        Assert.Equal("#2B579A", p.Light.Primary); // default generator keeps the seed as primary
    }

    [Fact]
    public async Task GeneratePalette_UsesCurrentThemeGenerator_WhenPresent()
    {
        var injector = new FakeInjector();
        var s = new ThemeService(injector, new FakeTheme("t1", new FakeGen()), Pal("p1"), ThemeMode.Light);
        var p = s.GeneratePalette("g", "G", new PaletteSeed("#2B579A"));
        Assert.Equal("FAKE", p.Name); // routed to the theme's generator

        // switching to a theme without a generator falls back to the default
        s.RegisterTheme(new FakeTheme("t2"));
        await s.SetThemeAsync("t2");
        Assert.Equal("#2B579A", s.GeneratePalette("g2", "G2", new PaletteSeed("#2B579A")).Light.Primary);
    }

    [Fact]
    public void RegisterTheme_And_Palette_NoDuplicates()
    {
        var s = Make(out _);
        s.RegisterTheme(new FakeTheme("t1"));
        s.RegisterPalette(Pal("p1"));
        Assert.Single(s.Themes);
        Assert.Single(s.Palettes);
    }
}
