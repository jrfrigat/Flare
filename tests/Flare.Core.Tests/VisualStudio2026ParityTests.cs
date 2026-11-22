using System.Runtime.CompilerServices;
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
        Assert.Equal("#333333", dark[VisualStudioCssVars.ChromeCommandWell]);
        Assert.Equal("#353535", dark[VisualStudioCssVars.ChromeCommandWellBorder]);
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
    public void BaseTabsReadTheChromeRoles()
    {
        var light = VisualStudio.DesignReference.Extended;

        Assert.Equal("var(--flare-vs-chrome-tab-strip)", light[VisualStudioCssVars.TabStripBg]);
        Assert.Equal("var(--flare-vs-chrome-tab-active)", light[VisualStudioCssVars.TabActiveBg]);
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
    public void CommandWell_IsOwnedByTheIdePackage()
    {
        var light = VisualStudio.DesignReference.Extended;
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        // The optional IDE package publishes these hooks with base-token defaults; the theme does not
        // know about the IDE and must not declare them.
        foreach (var map in new[] { light, dark })
        {
            Assert.False(map.ContainsKey("--flare-ide-toolbar-well-surface"));
            Assert.False(map.ContainsKey("--flare-ide-toolbar-well-border-color"));
            Assert.False(map.ContainsKey("--flare-ide-toolbar-well-height"));
        }

        Assert.Equal($"var({Css.Tokens.Color.Surface})", light[VisualStudioCssVars.ChromeCommandWell]);
        Assert.Equal($"var({Css.Tokens.Color.Outline})", light[VisualStudioCssVars.ChromeCommandWellBorder]);
        Assert.Equal("#333333", dark[VisualStudioCssVars.ChromeCommandWell]);
        Assert.Equal("#353535", dark[VisualStudioCssVars.ChromeCommandWellBorder]);

        // The form-field well keeps its measured #353535 fill and #454545 stroke.
        var input = VisualStudio.DesignReference.Input;
        Assert.Equal($"var({VisualStudioCssVars.ChromeField})", input.FilledBg);
        Assert.Equal($"var({Css.Tokens.Color.Outline})", input.BorderColor);
        Assert.Equal("1.4375rem", input.HeightMd);
    }

    [Fact]
    public void ThemeCss_LeavesTheCommandWellToTheIdePackage()
    {
        var css = TokensToCss.ThemeCss(Theme);

        Assert.DoesNotContain("--flare-ide-toolbar-well-", css);
        Assert.Contains("--flare-vs-chrome-command-well:#333333;", css);
        Assert.Contains("--flare-vs-chrome-command-well-border:#353535;", css);
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
        Assert.DoesNotContain("--flare-caption-", css);
    }

    [Fact]
    public void ThemeCss_EmitsTheNeutralButtonAndDisabledRoles()
    {
        var css = TokensToCss.ThemeCss(Theme);

        // A bare Filled command is chrome, a Text command is chrome text, and neither is the accent.
        Assert.Contains($"{Css.Tokens.Button.FilledContainer}:var({Css.Tokens.Color.SurfaceContainerHigh});", css);
        Assert.Contains($"{Css.Tokens.Button.TextContent}:var({VisualStudioCssVars.ChromeText});", css);
        Assert.Contains($"{Css.Tokens.Button.DisabledContainer}:var({Css.Tokens.Color.SurfaceContainer});", css);
        Assert.Contains($"{Css.Tokens.Button.DisabledContent}:var({VisualStudioCssVars.ChromeTextDisabled});", css);
        Assert.Contains($"{Css.Tokens.InputField.DisabledBg}:var({Css.Tokens.Color.SurfaceContainer});", css);
        // The light Fluent disabled palette must never leak into the VS shell through a token default.
        Assert.DoesNotContain("--flare-fluent-disabled-", css);
    }

    [Fact]
    public void Extended_DoesNotDeclareIdeTokens()
    {
        var light = VisualStudio.DesignReference.Extended;
        var dark = Theme.ExtendedDarkOverride;
        Assert.NotNull(dark);

        foreach (var map in new[] { light, dark })
        {
            var ideKeys = map.Keys
                .Where(key => key.StartsWith("--flare-ide-", StringComparison.Ordinal))
                .ToArray();
            Assert.True(ideKeys.Length == 0,
                "The Visual Studio theme must not declare the optional IDE package's tokens: "
                + string.Join(", ", ideKeys));
        }
    }

    [Fact]
    public void ThemeCss_DoesNotReferenceIdeClassesOrTokens()
    {
        var css = TokensToCss.ThemeCss(Theme);

        Assert.DoesNotContain("--flare-ide-", css);
        Assert.DoesNotContain("flare-doc-tabs", css);
        Assert.DoesNotContain("flare-toolbar", css);
        Assert.DoesNotContain("flare-menubar", css);
        Assert.DoesNotContain("flare-statusbar", css);
        Assert.DoesNotContain("flare-sheet-tabs", css);
        Assert.DoesNotContain("flare-toolpanel", css);
    }

    [Fact]
    public void ThemeStylesheet_DoesNotReferenceTheIdePackage()
    {
        var cssPath = Path.Combine(FindRepoRoot(), "src", "Flare.Theme.VisualStudio",
            "wwwroot", "css", "components.css");
        Assert.True(File.Exists(cssPath), $"Missing generated theme stylesheet: {cssPath}");
        var css = File.ReadAllText(cssPath);

        Assert.DoesNotContain("--flare-ide-", css);
        Assert.DoesNotContain("flare-doc-tabs", css);
        Assert.DoesNotContain(".flare-toolbar", css);
        Assert.DoesNotContain(".flare-menubar", css);
        Assert.DoesNotContain(".flare-statusbar", css);
        Assert.DoesNotContain(".flare-sheet-tabs", css);
        Assert.DoesNotContain(".flare-toolpanel", css);
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

    // The compile-time path of this test file, walked up until the folder that contains "src".
    private static string FindRepoRoot([CallerFilePath] string thisFile = "")
    {
        var dir = Path.GetDirectoryName(thisFile);
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src")))
            dir = Path.GetDirectoryName(dir);
        Assert.False(dir is null, "Could not locate the repository root (no ancestor 'src' folder).");
        return dir!;
    }
}
