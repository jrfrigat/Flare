using Flare.Abstractions;
using Flare.Theme.VisualStudio;
using Flare.Theming;

namespace Flare.Core.Tests;

/// <summary>
/// The Visual Studio 2026 dark shell was measured from a 2559x1439 screenshot at 100% scale. These
/// assertions pin the measured values: unlike a generated palette, the shell palette is a fact from
/// the product, so a silent drift back to the VS 2022 layers or the blue accent is a regression.
/// </summary>
public sealed class VisualStudio2026ParityTests
{
    private static VisualStudioTheme Theme => new();

    [Fact]
    public void DarkScheme_CarriesTheMeasuredShellSurfaces()
    {
        var dark = VisualStudio.DarkColors;

        Assert.Equal("#1C1C1C", dark.Background);
        Assert.Equal("#1E1E1E", dark.Surface);
        Assert.Equal("#282828", dark.SurfaceVariant);
        Assert.Equal("#262626", dark.SurfaceContainerLow);
        Assert.Equal("#2C2C2C", dark.SurfaceContainer);
        Assert.Equal("#383838", dark.SurfaceContainerHigh);
        Assert.Equal("#3D3D3D", dark.SurfaceContainerHighest);
        Assert.Equal("#141414", dark.SurfaceContainerLowest);
    }

    [Fact]
    public void DarkScheme_CarriesTheMeasuredTextOutlineAndAccent()
    {
        var dark = VisualStudio.DarkColors;

        Assert.Equal("#FFFFFF", dark.OnSurface);
        Assert.Equal("#CCCCCC", dark.OnSurfaceVariant);
        Assert.Equal("#858585", dark.OnSurfaceVariant2);
        Assert.Equal("#454545", dark.Outline);
        Assert.Equal("#3D3D3D", dark.OutlineVariant);
        Assert.Equal("#9184EE", dark.Primary);
        Assert.Equal("#9184EE", dark.Info);
        Assert.Equal("#1C1C1C", dark.OnPrimary);
        Assert.NotEqual("#3794FF", dark.Primary);
    }

    [Fact]
    public void DarkExtended_CarriesTheMeasuredChromeRoles()
    {
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        Assert.Equal("#1C1C1C", dark[VisualStudioCssVars.ChromeTitleBar]);
        Assert.Equal("#262626", dark[VisualStudioCssVars.ChromeCommandBar]);
        Assert.Equal("#262626", dark[VisualStudioCssVars.ChromeTabStrip]);
        Assert.Equal("#282828", dark[VisualStudioCssVars.ChromeTabActive]);
        Assert.Equal("#1E1E1E", dark[VisualStudioCssVars.ChromeEditor]);
        Assert.Equal("#2B2B2B", dark[VisualStudioCssVars.ChromeBreadcrumb]);
        Assert.Equal("#282828", dark[VisualStudioCssVars.ChromeToolWindow]);
        Assert.Equal("#282828", dark[VisualStudioCssVars.ChromeToolTab]);
        Assert.Equal("#353535", dark[VisualStudioCssVars.ChromeField]);
        Assert.Equal("#1C1C1C", dark[VisualStudioCssVars.ChromeBottomPanel]);
        Assert.Equal("#141414", dark[VisualStudioCssVars.ChromeStatusBar]);
        Assert.Equal("#0E0E0E", dark[VisualStudioCssVars.ChromeBarEdge]);
        Assert.Equal("#3D3D3D", dark[VisualStudioCssVars.ChromeDivider]);
        Assert.Equal("#454545", dark[VisualStudioCssVars.ChromeOutline]);
        Assert.Equal("#2C2C2C", dark[VisualStudioCssVars.ChromeMenu]);
        Assert.Equal("#383838", dark[VisualStudioCssVars.ChromeHover]);
        Assert.Equal("#292929", dark[VisualStudioCssVars.ChromeMenuBarHover]);
        Assert.Equal("#9184EE", dark[VisualStudioCssVars.ChromeAccent]);
        Assert.Equal("#FFFFFF", dark[VisualStudioCssVars.ChromeText]);
        Assert.Equal("#CCCCCC", dark[VisualStudioCssVars.ChromeTextSecondary]);
        Assert.Equal("#858585", dark[VisualStudioCssVars.ChromeTextDisabled]);
        Assert.Equal("#9D9D9D", dark[VisualStudioCssVars.ChromeScrollbarThumb]);
        Assert.Equal("#282828", dark[VisualStudioCssVars.ChromeScrollbarTrack]);
    }

    [Fact]
    public void BothModes_DefineEveryChromeRole()
    {
        var light = VisualStudio.DesignReference.Extended;
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        var chrome = typeof(VisualStudioCssVars)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .Where(v => v.StartsWith("--flare-vs-chrome-", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(chrome);
        Assert.All(chrome, name =>
        {
            Assert.True(light.ContainsKey(name), $"light map is missing {name}");
            Assert.True(dark.ContainsKey(name), $"dark map is missing {name}");
        });
    }

    [Fact]
    public void TheDocumentTabsAndIdeShellReadTheChromeRoles()
    {
        var light = VisualStudio.DesignReference.Extended;

        Assert.Equal("var(--flare-vs-chrome-tab-strip)", light[VisualStudioCssVars.TabStripBg]);
        Assert.Equal("var(--flare-vs-chrome-tab-active)", light[VisualStudioCssVars.TabActiveBg]);

        Assert.Equal("var(--flare-vs-chrome-tool-window)", light["--flare-ide-surface"]);
        Assert.Equal("var(--flare-vs-chrome-command-bar)", light["--flare-ide-surface-variant"]);
        Assert.Equal("var(--flare-vs-chrome-divider)", light["--flare-ide-outline-variant"]);
        Assert.Equal("var(--flare-vs-chrome-accent)", light["--flare-ide-primary"]);
        Assert.Equal("var(--flare-vs-chrome-title-bar)", light["--flare-ide-menubar-bg"]);
        Assert.Equal("var(--flare-vs-chrome-command-bar)", light["--flare-ide-commandbar-bg"]);
        Assert.Equal("var(--flare-vs-chrome-tab-strip)", light["--flare-ide-tab-strip-bg"]);
        Assert.Equal("var(--flare-vs-chrome-tab-active)", light["--flare-ide-tab-active-bg"]);
        Assert.Equal("var(--flare-vs-chrome-editor)", light["--flare-ide-editor-bg"]);
        Assert.Equal("var(--flare-vs-chrome-bottom-panel)", light["--flare-ide-bottom-tab-strip-bg"]);
        Assert.Equal("var(--flare-vs-chrome-status-bar)", light["--flare-ide-statusbar-bg"]);
        Assert.Equal("var(--flare-vs-chrome-tool-window)", light["--flare-ide-panel-bg"]);
    }

    [Fact]
    public void ThemeCss_EmitsTheMeasuredDarkChrome()
    {
        var css = TokensToCss.ThemeCss(Theme);
        var darkRule = css[css.IndexOf($".{Css.Classes.Theme.ForTheme(VisualStudioTheme.ThemeId)}.{Css.Classes.Theme.ModeDark}", StringComparison.Ordinal)..];

        Assert.Contains($"{VisualStudioCssVars.ChromeTitleBar}:#1C1C1C;", darkRule);
        Assert.Contains($"{VisualStudioCssVars.ChromeTabStrip}:#262626;", darkRule);
        Assert.Contains($"{VisualStudioCssVars.ChromeToolWindow}:#282828;", darkRule);
        Assert.Contains($"{VisualStudioCssVars.ChromeStatusBar}:#141414;", darkRule);
        Assert.Contains($"{VisualStudioCssVars.ChromeAccent}:#9184EE;", darkRule);
    }

    [Fact]
    public void PaletteCss_EmitsThePurpleAccentInDark()
    {
        var css = TokensToCss.PaletteCss(VisualStudioPalettes.Blue);
        var darkRule = css[css.IndexOf($".{Css.Classes.Theme.ForPalette(VisualStudioPalettes.BlueId)}.{Css.Classes.Theme.ModeDark}", StringComparison.Ordinal)..];

        Assert.Contains("#9184EE", darkRule);
    }

    [Fact]
    public void FieldMetrics_MatchTheMeasuredVs2026Well()
    {
        var input = VisualStudio.DesignReference.Input;

        Assert.Equal("1.4375rem", input.HeightMd); // 23px
        Assert.Equal("1.25rem", input.HeightXs);
        Assert.Equal("1.375rem", input.HeightSm);
        Assert.Equal("1.625rem", input.HeightLg);
        Assert.Equal("1.875rem", input.HeightXl);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", input.OutlinedRadius);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", input.FilledVariantRadius);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", input.OutlinedVariantRadius);
        Assert.Equal($"var({VisualStudioCssVars.ChromeField})", input.FilledBg);
        Assert.Equal($"var({VisualStudioCssVars.ChromeField})", input.FilledVariantBg);
        Assert.Equal($"var({VisualStudioCssVars.ChromeField})", input.OutlinedVariantBg);
        Assert.Equal($"var({Css.Tokens.Color.Outline})", input.BorderColor);
        Assert.Equal("0.125rem 0.5rem", input.PaddingMd);
        Assert.Equal("1rem", input.IconSize);
    }

    [Fact]
    public void FieldFocus_IsOneAccentLineWithoutAGlow()
    {
        var input = VisualStudio.DesignReference.Input;

        Assert.Equal("none", input.FocusRing);
        Assert.Equal($"1px solid var({Css.Tokens.LocalColor.Main}, var({Css.Tokens.Color.Primary}))", input.FocusOutline);
        Assert.Equal("0", input.FocusOutlineOffset);
        Assert.Equal("none", input.FilledVariantFocusRing);
        Assert.Equal("none", input.OutlinedVariantFocusRing);
        Assert.Equal($"var({Css.Tokens.LocalColor.Main}, var({Css.Tokens.Color.Primary}))", input.FilledVariantFocusBorderColor);
        Assert.Equal($"var({Css.Tokens.LocalColor.Main}, var({Css.Tokens.Color.Primary}))", input.OutlinedVariantFocusBorderColor);
    }

    [Fact]
    public void CommandButton_IsTwentyFourPixelsWithASixteenPixelIcon()
    {
        var button = VisualStudio.DesignReference.Button;

        Assert.Equal("1.5rem", button.HeightXs);
        Assert.Equal("1rem", button.IconSizeXs);
        Assert.Equal("none", button.FocusShadow);
        Assert.Equal($"1px solid var({Css.Tokens.Color.Primary})", button.FocusOutline);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", button.RadiusXs.TopLeft);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", button.RadiusXs.BottomRight);
    }

    [Fact]
    public void CheckboxAndRadio_FocusWithOneAccentLine()
    {
        var checkbox = VisualStudio.DesignReference.Checkbox;
        var radio = VisualStudio.DesignReference.Radio;

        Assert.Equal($"1px solid var({Css.Tokens.Color.Primary})", checkbox.FocusOutline);
        Assert.Equal("none", checkbox.FocusShadow);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", checkbox.Radius);
        Assert.Equal($"1px solid var({Css.Tokens.Color.Primary})", radio.FocusOutline);
        Assert.Equal("none", radio.FocusShadow);
        Assert.Equal("0", radio.FocusOutlineOffset);
    }

    [Fact]
    public void Scrollbar_IsTwoPixelsSquareOnThePanelSurface()
    {
        var scrollbar = VisualStudio.DesignReference.Scrollbar;

        Assert.Equal("auto", scrollbar.Width);
        Assert.Equal("2px", scrollbar.Size);
        Assert.Equal("#9D9D9D", scrollbar.Thumb);
        Assert.Equal("var(--flare-vs-chrome-scrollbar-track)", scrollbar.Track);
        Assert.Equal("0px", scrollbar.Radius);
    }

    [Fact]
    public void DropdownsAndSearch_TakeSixPixelCorners()
    {
        Assert.Equal($"var({Css.Tokens.Shape.Large})", VisualStudio.DesignReference.Menu.PanelRadius);
        Assert.Equal($"var({Css.Tokens.Shape.Large})", VisualStudio.DesignReference.Popover.Radius);
        Assert.Equal(
            $"var({Css.Tokens.Shape.Large})",
            VisualStudio.DesignReference.Extended[VisualStudioCssVars.ChromeSearchRadius]);
    }

    [Fact]
    public void ThemeCss_EmitsTheMeasuredComponentMetrics()
    {
        var css = TokensToCss.ThemeCss(Theme);

        Assert.Contains($"{Css.Tokens.Button.Height.Xs}:1.5rem;", css);
        Assert.Contains($"{Css.Tokens.Button.IconSize.Xs}:1rem;", css);
        Assert.Contains($"{Css.Tokens.InputField.HeightMd}:1.4375rem;", css);
        Assert.Contains($"{Css.Tokens.Scrollbar.Size}:2px;", css);
        Assert.Contains($"{Css.Tokens.Scrollbar.Radius}:0px;", css);
        Assert.Contains($"{Css.Tokens.MenuPanel.Radius}:var({Css.Tokens.Shape.Large});", css);
    }

    [Fact]
    public void LightScheme_KeepsItsExistingValues()
    {
        var light = VisualStudio.LightColors;

        Assert.Equal("#FFFFFF", light.Surface);
        Assert.Equal("#1E1E1E", light.OnSurface);
        Assert.Equal("#005FB8", light.Primary);
        Assert.Equal("#CCCEDB", light.Outline);
        Assert.Equal("#E4E4E8", light.OutlineVariant);
        Assert.Equal("#F5F5F7", light.Background);
    }

    [Fact]
    public void LightChromeRoles_NamePaletteRolesInsteadOfMeasuredDarkLiterals()
    {
        var light = VisualStudio.DesignReference.Extended;

        Assert.Contains(Css.Tokens.Color.SurfaceContainerLow, light[VisualStudioCssVars.ChromeTitleBar]);
        Assert.Contains(Css.Tokens.Color.Surface, light[VisualStudioCssVars.ChromeEditor]);
        Assert.StartsWith($"var({Css.Tokens.Color.Primary}", light[VisualStudioCssVars.ChromeAccent]);
        Assert.DoesNotContain("#1C1C1C", light[VisualStudioCssVars.ChromeTitleBar]);
    }
}
