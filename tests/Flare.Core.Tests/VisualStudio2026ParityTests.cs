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
        Assert.Equal("#2E2E2E", dark[VisualStudioCssVars.ChromeBreadcrumbEdge]);
        Assert.Equal("#282828", dark[VisualStudioCssVars.ChromeToolWindow]);
        Assert.Equal("#282828", dark[VisualStudioCssVars.ChromeToolTab]);
        Assert.Equal("#353535", dark[VisualStudioCssVars.ChromeField]);
        Assert.Equal("#1C1C1C", dark[VisualStudioCssVars.ChromeBottomPanel]);
        Assert.Equal("#404040", dark[VisualStudioCssVars.ChromeBottomTabUnderline]);
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
    public void CommandBar_CarriesTheMeasuredStripSurfacesAndMetrics()
    {
        var light = VisualStudio.DesignReference.Extended;
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        foreach (var map in new[] { light, dark })
        {
            Assert.Equal("var(--flare-vs-chrome-title-bar)", map["--flare-ide-toolbar-surface"]);
            Assert.Equal("var(--flare-vs-chrome-command-bar)", map["--flare-ide-toolbar-group-surface"]);
            Assert.Equal("var(--flare-vs-chrome-bar-edge)", map["--flare-ide-toolbar-edge-color"]);
            Assert.Equal("0.25rem", map["--flare-ide-toolbar-gap"]);
            Assert.Equal("0.25rem", map["--flare-ide-toolbar-dense-gap"]);
            Assert.Equal("1.75rem", map["--flare-ide-toolbar-height"]);
            Assert.Equal("1.75rem", map["--flare-ide-toolbar-dense-height"]);
            Assert.Equal("0", map["--flare-ide-toolbar-padding"]);
            Assert.Equal("0", map["--flare-ide-toolbar-dense-padding"]);
        }
    }

    [Fact]
    public void ThemeCss_EmitsTheMeasuredCommandBar()
    {
        var css = TokensToCss.ThemeCss(Theme);

        Assert.Contains("--flare-ide-toolbar-surface:var(--flare-vs-chrome-title-bar);", css);
        Assert.Contains("--flare-ide-toolbar-group-surface:var(--flare-vs-chrome-command-bar);", css);
        Assert.Contains("--flare-ide-toolbar-height:1.75rem;", css);
        Assert.Contains("--flare-ide-toolbar-edge-color:var(--flare-vs-chrome-bar-edge);", css);
    }

    [Fact]
    public void Menus_TakeTheMeasuredPanelItemAndPillMetrics()
    {
        var menu = VisualStudio.DesignReference.Menu;

        Assert.Equal("var(--flare-vs-chrome-menu)", menu.PanelBg);
        Assert.Equal($"var({Css.Tokens.Shape.Large})", menu.PanelRadius);
        Assert.Equal("0.0625rem", menu.PanelPaddingBlock);
        Assert.Equal("0.0625rem", menu.PanelPaddingInline);
        Assert.Equal("1.375rem", menu.ItemHeight);
        Assert.Equal("0", menu.ItemPaddingBlock);
        Assert.Equal("0.75rem", menu.ItemPaddingInline);
        Assert.Equal("0.125rem", menu.ItemGapBetween);
        Assert.Equal("0.125rem", menu.ItemGap);
        Assert.Equal("1rem", menu.ItemIconSize);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", menu.ItemRadius);
        Assert.Equal($"var({Css.Tokens.Shape.Small})", menu.ItemRadiusEnd);
        Assert.Equal("var(--flare-vs-chrome-text-disabled)", menu.ItemShortcutColor);
        Assert.Equal("var(--flare-vs-chrome-accent)", menu.ItemCheckColor);
        Assert.Equal("1", menu.ItemDisabledOpacity);
        Assert.Equal("1px solid var(--flare-vs-chrome-divider)", menu.SeparatorRule);
        Assert.Equal("0.3125rem", menu.SeparatorSpacing);
    }

    [Fact]
    public void MenuBar_TakesTheMeasuredThirtyPixelBar()
    {
        var extended = VisualStudio.DesignReference.Extended;

        Assert.Equal("1.875rem", extended["--flare-ide-menubar-height"]);
    }

    [Fact]
    public void BottomSheetTabs_TakeTheMeasuredStripAndUnderline()
    {
        var light = VisualStudio.DesignReference.Extended;
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        foreach (var map in new[] { light, dark })
        {
            Assert.Equal("var(--flare-vs-chrome-bottom-panel)", map["--flare-ide-sheet-tabs-surface"]);
            Assert.Equal("var(--flare-vs-chrome-text-secondary)", map["--flare-ide-sheet-tabs-color"]);
            Assert.Equal("transparent", map["--flare-ide-sheet-tabs-active-bg"]);
            Assert.Equal("transparent", map["--flare-ide-sheet-tabs-active-border-color"]);
            Assert.Equal("var(--flare-vs-chrome-text)", map["--flare-ide-sheet-tabs-active-color"]);
            Assert.Equal("var(--flare-vs-chrome-bottom-tab-underline)", map["--flare-ide-sheet-tabs-underline-color"]);
            Assert.Equal("var(--flare-vs-chrome-accent)", map["--flare-ide-sheet-tabs-divider"]);
            Assert.Equal("2.0625rem", map["--flare-ide-sheet-tabs-height"]);
            Assert.Equal("0.875rem", map["--flare-ide-sheet-tabs-gap"]);
            Assert.Equal("0 0.4375rem 0.25rem", map["--flare-ide-sheet-tabs-padding"]);
            Assert.Equal("3px", map["--flare-ide-sheet-tabs-underline-height"]);
        }

        Assert.Equal($"var({Css.Tokens.Color.OutlineVariant})", light[VisualStudioCssVars.ChromeBottomTabUnderline]);
        Assert.Equal("#404040", dark[VisualStudioCssVars.ChromeBottomTabUnderline]);
    }

    [Fact]
    public void ThemeCss_EmitsTheMeasuredBottomSheetTabs()
    {
        var css = TokensToCss.ThemeCss(Theme);

        Assert.Contains("--flare-ide-sheet-tabs-surface:var(--flare-vs-chrome-bottom-panel);", css);
        Assert.Contains("--flare-ide-sheet-tabs-height:2.0625rem;", css);
        Assert.Contains("--flare-ide-sheet-tabs-gap:0.875rem;", css);
        Assert.Contains("--flare-ide-sheet-tabs-padding:0 0.4375rem 0.25rem;", css);
        Assert.Contains("--flare-ide-sheet-tabs-underline-height:3px;", css);
        Assert.Contains("--flare-vs-chrome-bottom-tab-underline:#404040;", css);
    }

    [Fact]
    public void EditorBreadcrumb_TakesTheMeasuredRowAndActionArea()
    {
        var light = VisualStudio.DesignReference.Extended;
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        foreach (var map in new[] { light, dark })
        {
            Assert.Equal("var(--flare-vs-chrome-breadcrumb)", map["--flare-ide-breadcrumb-surface"]);
            Assert.Equal("var(--flare-vs-chrome-text-secondary)", map["--flare-ide-breadcrumb-color"]);
            Assert.Equal("var(--flare-vs-chrome-breadcrumb-edge)", map["--flare-ide-breadcrumb-edge-color"]);
            Assert.Equal("var(--flare-vs-chrome-divider)", map["--flare-ide-breadcrumb-separator-color"]);
            Assert.Equal("var(--flare-vs-chrome-editor)", map["--flare-ide-breadcrumb-active-surface"]);
            Assert.Equal("var(--flare-vs-chrome-text)", map["--flare-ide-breadcrumb-active-color"]);
            Assert.Equal("var(--flare-vs-chrome-breadcrumb-edge)", map["--flare-ide-breadcrumb-actions-surface"]);
            Assert.Equal("1.375rem", map["--flare-ide-breadcrumb-bar-height"]);
            Assert.Equal("0.5rem", map["--flare-ide-breadcrumb-bar-padding"]);
            Assert.Equal("0.5rem", map["--flare-ide-breadcrumb-segment-padding"]);
            Assert.Equal("0.25rem", map["--flare-ide-breadcrumb-segment-gap"]);
            Assert.Equal("1rem", map["--flare-ide-breadcrumb-icon-size"]);
            Assert.Equal("0.75rem", map["--flare-ide-breadcrumb-chevron-size"]);
            Assert.Equal("0.5rem", map["--flare-ide-breadcrumb-actions-padding"]);
            Assert.Equal("0.25rem", map["--flare-ide-breadcrumb-actions-gap"]);
        }

        Assert.Equal($"var({Css.Tokens.Color.OutlineVariant})", light[VisualStudioCssVars.ChromeBreadcrumbEdge]);
        Assert.Equal("#2E2E2E", dark[VisualStudioCssVars.ChromeBreadcrumbEdge]);
    }

    [Fact]
    public void ThemeCss_EmitsTheMeasuredEditorBreadcrumb()
    {
        var css = TokensToCss.ThemeCss(Theme);

        Assert.Contains("--flare-ide-breadcrumb-surface:var(--flare-vs-chrome-breadcrumb);", css);
        Assert.Contains("--flare-ide-breadcrumb-edge-color:var(--flare-vs-chrome-breadcrumb-edge);", css);
        Assert.Contains("--flare-ide-breadcrumb-actions-surface:var(--flare-vs-chrome-breadcrumb-edge);", css);
        Assert.Contains("--flare-ide-breadcrumb-bar-height:1.375rem;", css);
        Assert.Contains("--flare-ide-breadcrumb-icon-size:1rem;", css);
        Assert.Contains("--flare-vs-chrome-breadcrumb-edge:#2E2E2E;", css);
    }

    [Fact]
    public void StatusBar_TakesTheMeasuredSurfacePaddingAndGaps()
    {
        var extended = VisualStudio.DesignReference.Extended;

        Assert.Equal("var(--flare-vs-chrome-status-bar)", extended["--flare-ide-statusbar-surface"]);
        Assert.Equal("var(--flare-vs-chrome-text)", extended["--flare-ide-statusbar-color"]);
        Assert.Equal("0 0.5rem", extended["--flare-ide-statusbar-padding"]);
        Assert.Equal("0.75rem", extended["--flare-ide-statusbar-gap"]);
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
    public void TreeMetrics_AreSixteenPixelGlyphsInTwentyFourPixelTargets()
    {
        var tree = VisualStudio.DesignReference.Tree;

        Assert.Equal("1rem", tree.IconSize);
        Assert.Equal("1.5rem", tree.ToggleSize);
    }

    [Fact]
    public void ChromeTypeHooks_ResolveToTheTwelvePixelBodyMedium()
    {
        var extended = VisualStudio.DesignReference.Extended;

        Assert.Equal($"var({Css.Tokens.Typography.BodyMedium.Font})", extended["--flare-ide-typescale-body-small-font"]);
        Assert.Equal($"var({Css.Tokens.Typography.BodyMedium.Size})", extended["--flare-ide-typescale-body-small-size"]);
        Assert.Equal($"var({Css.Tokens.Typography.BodyMedium.Weight})", extended["--flare-ide-typescale-body-small-weight"]);
        Assert.Equal($"var({Css.Tokens.Typography.BodyMedium.Height})", extended["--flare-ide-typescale-body-small-height"]);
        Assert.Equal($"var({Css.Tokens.Typography.BodyMedium.Spacing})", extended["--flare-ide-typescale-body-small-spacing"]);
        Assert.Equal($"var({Css.Tokens.Typography.BodyMedium.Weight})", extended["--flare-ide-typescale-label-small-weight"]);
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
        Assert.Contains($"{Css.Tokens.Tree.IconSize}:1rem;", css);
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
