using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests.Component;

// Coverage for the small generic API additions surfaced by the PlaylistShared migration.

// ------------------------------------------------------------------------------
// FlareIconButton  (icon-only wrapper over FlareButton)
// ------------------------------------------------------------------------------
public class C_FlareIconButtonTests : FlareTestContext
{
    [Fact]
    public void Icon_RendersIconOnlyButton()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.Settings)
            .Add(x => x.AriaLabel, "Settings"));

        var btn = cut.Find($"button.{Css.Classes.Button.Root}");
        Assert.Contains(Css.Classes.Button.IconOnly, btn.ClassName);
        Assert.Equal("Settings", btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DefaultVariant_IsText()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.Add)
            .Add(x => x.AriaLabel, "Add"));

        Assert.Contains(Css.Classes.Button.Text, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void Href_RendersAnchor()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.OpenInNew)
            .Add(x => x.Href, "https://example.com")
            .Add(x => x.AriaLabel, "Open"));

        Assert.NotEmpty(cut.FindAll($"a.{Css.Classes.Button.Root}"));
    }
}

// ------------------------------------------------------------------------------
// FlareChip Variant
// ------------------------------------------------------------------------------
public class C_FlareChipVariantTests : FlareTestContext
{
    [Fact]
    public void FilledVariant_AddsFilledClass()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Variant, ChipVariant.Filled));

        Assert.Contains(Css.Classes.Chip.Filled, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }

    [Fact]
    public void ElevatedVariant_AddsElevatedClass()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Variant, ChipVariant.Elevated));

        Assert.Contains(Css.Classes.Chip.Elevated, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }

    [Fact]
    public void OutlinedVariant_IsDefault_NoVariantModifier()
    {
        var cut = Render<FlareChip>(p => p.Add(x => x.Label, "Tag"));

        var cls = cut.Find($".{Css.Classes.Chip.Root}").ClassName;
        Assert.DoesNotContain(Css.Classes.Chip.Filled, cls);
        Assert.DoesNotContain(Css.Classes.Chip.Elevated, cls);
    }

    [Fact]
    public void ElevatedBool_StillMapsToElevated()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Elevated, true));

        Assert.Contains(Css.Classes.Chip.Elevated, cut.Find($".{Css.Classes.Chip.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareAvatar FallbackIcon / FallbackContent
// ------------------------------------------------------------------------------
public class C_FlareAvatarFallbackTests : FlareTestContext
{
    [Fact]
    public void NoImageNoText_DefaultsToPersonIcon()
    {
        var cut = Render<FlareAvatar>();

        // The default fallback is now the built-in person SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Icon} path"));
    }

    [Fact]
    public void FallbackIcon_OverridesDefault()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.FallbackIcon, FlareIcons.Group));

        // "group" is built in, overriding the default person icon with inline SVG.
        Assert.Equal(FlareIcons.Group.Data, cut.Find($".{Css.Classes.Avatar.Icon} path").GetAttribute("d"));
    }

    [Fact]
    public void FallbackContent_ReplacesIcon()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.FallbackContent, b => b.AddMarkupContent(0, "<span class=\"custom-fb\">x</span>")));

        Assert.NotEmpty(cut.FindAll(".custom-fb"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Avatar.Icon}"));
    }
}

// ------------------------------------------------------------------------------
// FlareField / FlareTextField Error + FullWidth + Margin
// ------------------------------------------------------------------------------
public class C_FlareFieldErrorLayoutTests : FlareTestContext
{
    [Fact]
    public void Error_AddsErrorState_WithoutMessage()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Error, true));

        Assert.Contains(Css.Classes.Input.Error, cut.Find($".{Css.Classes.Input.Root}").ClassName);
        // No error message row is forced when there is no ErrorText.
        Assert.Empty(cut.FindAll($".{Css.Classes.Input.HelperError}"));
        Assert.Equal("true", cut.Find($"input.{Css.Classes.Input.Control}").GetAttribute("aria-invalid"));
    }

    [Fact]
    public void Invalid_Alias_AddsErrorState()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Invalid, true));

        Assert.Contains(Css.Classes.Input.Error, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void FullWidthFalse_AddsAutoClass()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.FullWidth, false));

        Assert.Contains(Css.Classes.Input.Auto, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void FullWidthTrue_IsDefault_NoAutoClass()
    {
        var cut = Render<FlareTextField>();

        Assert.DoesNotContain(Css.Classes.Input.Auto, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }

    [Fact]
    public void MarginDense_AddsMarginClass()
    {
        var cut = Render<FlareTextField>(p => p.Add(x => x.Margin, FieldMargin.Dense));

        Assert.Contains(Css.Classes.Input.MarginDense, cut.Find($".{Css.Classes.Input.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareCard numeric Elevation
// ------------------------------------------------------------------------------
public class C_FlareCardElevationTests : FlareTestContext
{
    [Fact]
    public void Elevation_EmitsInlineElevationVariable()
    {
        var cut = Render<FlareCard>(p => p.Add(x => x.Elevation, 3));

        // Driven through the --flare-card-elevation variable (not the final box-shadow) so the
        // clickable :hover lift rule can still override the resting shadow.
        Assert.Contains($"{Css.Tokens.CardField.Elevation}:var({Css.Tokens.Elevation.Level3})", cut.Find($".{Css.Classes.Card.Root}").GetAttribute("style"));
    }

    [Fact]
    public void ElevationZero_IsFlat()
    {
        var cut = Render<FlareCard>(p => p.Add(x => x.Elevation, 0));

        Assert.Contains($"{Css.Tokens.CardField.Elevation}:var({Css.Tokens.Elevation.Level0})", cut.Find($".{Css.Classes.Card.Root}").GetAttribute("style"));
    }

    [Fact]
    public void Elevation_ClampedToScale()
    {
        var cut = Render<FlareCard>(p => p.Add(x => x.Elevation, 99));

        Assert.Contains($"{Css.Tokens.CardField.Elevation}:var({Css.Tokens.Elevation.Level5})", cut.Find($".{Css.Classes.Card.Root}").GetAttribute("style"));
    }
}

// ------------------------------------------------------------------------------
// FlareStack StretchItems / StretchFirst
// ------------------------------------------------------------------------------
public class C_FlareStackStretchTests : FlareTestContext
{
    [Fact]
    public void StretchItems_AddsStretchClass()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.StretchItems, true));

        Assert.Contains(Css.Classes.Stack.Stretch, cut.Find($".{Css.Classes.Stack.Root}").ClassName);
    }

    [Fact]
    public void StretchFirst_AddsStretchFirstClass()
    {
        var cut = Render<FlareStack>(p => p.Add(x => x.StretchFirst, true));

        Assert.Contains(Css.Classes.Stack.StretchFirst, cut.Find($".{Css.Classes.Stack.Root}").ClassName);
    }

    [Fact]
    public void StretchItems_WinsOverStretchFirst()
    {
        var cut = Render<FlareStack>(p => p
            .Add(x => x.StretchItems, true)
            .Add(x => x.StretchFirst, true));

        var cls = cut.Find($".{Css.Classes.Stack.Root}").ClassName;
        Assert.Contains($"{Css.Classes.Stack.Stretch} ", cls + " ");
        Assert.DoesNotContain(Css.Classes.Stack.StretchFirst, cls);
    }
}

// ------------------------------------------------------------------------------
// FlareMenuItem Target + IconColor
// ------------------------------------------------------------------------------
public class C_FlareMenuItemTargetColorTests : FlareTestContext
{
    private static RenderFragment Activator => b => b.AddMarkupContent(0, "<button>Open</button>");

    [Fact]
    public void Target_Blank_AddsTargetAndRel()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, Activator)
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Href, "https://example.com")
                .Add(x => x.Target, "_blank")
                .AddChildContent("External")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var anchor = cut.Find($"a.{Css.Classes.Menu.Item}");
        Assert.Equal("_blank", anchor.GetAttribute("target"));
        Assert.Equal("noopener noreferrer", anchor.GetAttribute("rel"));
    }

    [Fact]
    public void IconColor_AddsColorClassOnIcon()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, Activator)
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, FlareIcons.Edit)
                .Add(x => x.IconColor, FlareColor.Primary)
                .AddChildContent("Edit")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Contains(Css.Classes.Color.Primary, cut.Find($".{Css.Classes.Menu.ItemIcon}").ClassName);
    }

    [Fact]
    public void LeadingIconColor_OverridesIconColor()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, Activator)
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, FlareIcons.Edit)
                .Add(x => x.IconColor, FlareColor.Primary)
                .Add(x => x.LeadingIconColor, FlareColor.Error)
                .AddChildContent("Edit")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var cls = cut.Find($".{Css.Classes.Menu.ItemIcon}").ClassName;
        Assert.Contains(Css.Classes.Color.Error, cls);
        Assert.DoesNotContain(Css.Classes.Color.Primary, cls);
    }
}

// ------------------------------------------------------------------------------
// FlareToggleGroup cascade (Size / Color / Disabled)
// ------------------------------------------------------------------------------
public class C_FlareToggleGroupCascadeTests : FlareTestContext
{
    [Fact]
    public void GroupSize_CascadesToButtons()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Size, ButtonSize.Lg)
            .AddChildContent<FlareToggleButton>(b => b
                .Add(x => x.Value, (object?)"a")
                .AddChildContent("A")));

        Assert.Contains(Css.Classes.Button.Lg, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void GroupColor_CascadesColorClassToButtons()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Color, FlareColor.Tertiary)
            .AddChildContent<FlareToggleButton>(b => b
                .Add(x => x.Value, (object?)"a")
                .AddChildContent("A")));

        Assert.Contains(Css.Classes.Color.Tertiary, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void GroupDisabled_DisablesButtons()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Disabled, true)
            .AddChildContent<FlareToggleButton>(b => b
                .Add(x => x.Value, (object?)"a")
                .AddChildContent("A")));

        Assert.True(cut.Find($"button.{Css.Classes.Button.Root}").HasAttribute("disabled"));
    }
}

// ------------------------------------------------------------------------------
// FlareSelect declarative <option> child content
// ------------------------------------------------------------------------------
public class C_FlareSelectDeclarativeTests : FlareTestContext
{
    private static RenderFragment Options => b =>
    {
        b.OpenElement(0, "option");
        b.AddAttribute(1, "value", "a");
        b.AddContent(2, "Apple");
        b.CloseElement();
        b.OpenElement(3, "option");
        b.AddAttribute(4, "value", "b");
        b.AddContent(5, "Banana");
        b.CloseElement();
    };

    [Fact]
    public void DeclarativeOptions_RenderInDropdown()
    {
        var cut = Render<FlareSelect<string>>(p => p.Add(x => x.ChildContent, Options));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void DeclarativeOptions_SelectedLabelShown()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Value, "b")
            .Add(x => x.ChildContent, Options));

        Assert.Contains("Banana", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }

    // Static <option> markup is compiled by Razor into a single Markup frame (raw HTML), not element
    // frames - bUnit's AddChildContent(string) reproduces that shape, which the parser must handle.
    [Fact]
    public void StaticOptionMarkup_RendersOptions()
    {
        var cut = Render<FlareSelect<string>>(p => p.AddChildContent(
            "<option value=\"a\">Apple</option><option value=\"b\">Banana</option><option value=\"c\">Cherry</option>"));

        cut.Find($".{Css.Classes.Select.Control}").Click();

        Assert.Equal(3, cut.FindAll($".{Css.Classes.Select.Option}").Count);
    }

    [Fact]
    public void StaticOptionMarkup_SelectedLabelShown()
    {
        var cut = Render<FlareSelect<string>>(p => p
            .Add(x => x.Value, "b")
            .AddChildContent("<option value=\"a\">Apple</option><option value=\"b\">Banana</option>"));

        Assert.Contains("Banana", cut.Find($".{Css.Classes.Select.Value}").TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareLink Typo
// ------------------------------------------------------------------------------
public class C_FlareLinkTypoTests : FlareTestContext
{
    [Fact]
    public void Typo_AddsTypeScaleClass()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "#")
            .Add(x => x.Typo, TypographyScale.TitleMedium)
            .AddChildContent("Link"));

        Assert.Contains(Css.Classes.Text.TitleMedium, cut.Find("a").ClassName);
    }

    [Fact]
    public void NoTypo_NoTypeScaleClass()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "#")
            .AddChildContent("Link"));

        Assert.DoesNotContain("flare-text--", cut.Find("a").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareCollapse (standalone expand/collapse)
// ------------------------------------------------------------------------------
public class C_FlareCollapseTests : FlareTestContext
{
    [Fact]
    public void Collapsed_ByDefault()
    {
        var cut = Render<FlareCollapse>(p => p
            .AddChildContent("<p class=\"inner\">Body</p>"));

        Assert.DoesNotContain(Css.Classes.Collapse.Expanded, cut.Find($".{Css.Classes.Collapse.Root}").ClassName);
    }

    [Fact]
    public void Expanded_AddsExpandedClass()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Expanded, true)
            .AddChildContent("<p class=\"inner\">Body</p>"));

        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Find($".{Css.Classes.Collapse.Root}").ClassName);
    }

    [Fact]
    public void Header_RendersToggleButton_AndExpands()
    {
        var cut = Render<FlareCollapse>(p => p
            .Add(x => x.Header, "More")
            .AddChildContent("<p class=\"inner\">Body</p>"));

        var btn = cut.Find($"button.{Css.Classes.Collapse.Header}");
        Assert.Equal("false", btn.GetAttribute("aria-expanded"));

        btn.Click();

        Assert.Contains(Css.Classes.Collapse.Expanded, cut.Find($".{Css.Classes.Collapse.Root}").ClassName);
        Assert.Equal("true", cut.Find($"button.{Css.Classes.Collapse.Header}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Headerless_RendersNoToggleButton()
    {
        var cut = Render<FlareCollapse>(p => p
            .AddChildContent("<p class=\"inner\">Body</p>"));

        Assert.Empty(cut.FindAll($"button.{Css.Classes.Collapse.Header}"));
    }
}

// ------------------------------------------------------------------------------
// ISnackbarService options overload
// ------------------------------------------------------------------------------
public class C_SnackbarOptionsTests
{
    [Fact]
    public void Show_WithOptions_MapsAllFields()
    {
        var service = new SnackbarService();
        SnackbarMessage? captured = null;
        service.OnShow += m => captured = m;

        service.Show("Saved", new SnackbarOptions
        {
            Severity = SnackbarSeverity.Warning,
            DurationMs = 0,
            ShowClose = false,
            ShowProgress = true,
            CssClass = "my-snackbar",
            CloseAfterNavigation = true,
        });

        Assert.NotNull(captured);
        Assert.Equal("Saved", captured!.Text);
        Assert.Equal(SnackbarSeverity.Warning, captured.Severity);
        Assert.Equal(0, captured.DurationMs);
        Assert.False(captured.ShowClose);
        Assert.True(captured.ShowProgress);
        Assert.Equal("my-snackbar", captured.CssClass);
        Assert.True(captured.CloseAfterNavigation);
    }

    [Fact]
    public void Show_WithNullOptions_Throws()
    {
        var service = new SnackbarService();

        Assert.Throws<ArgumentNullException>(() => service.Show("x", (SnackbarOptions)null!));
    }
}

// ------------------------------------------------------------------------------
// FlareSnackbarProvider per-message CssClass + CloseAfterNavigation
// ------------------------------------------------------------------------------
public class C_SnackbarProviderOptionsTests : FlareTestContext
{
    public C_SnackbarProviderOptionsTests()
    {
        Services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    [Fact]
    public void CssClass_AppliedToSnackbarElement()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Hi", new SnackbarOptions { DurationMs = 0, CssClass = "promo-snackbar" });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.NotEmpty(cut.FindAll(".promo-snackbar"));
    }

    [Fact]
    public void CloseAfterNavigation_DismissesOnNavigate()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();
        var nav = Services.GetRequiredService<NavigationManager>();

        service.Show("Hi", new SnackbarOptions { DurationMs = 0, CloseAfterNavigation = true });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        nav.NavigateTo("/other");

        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);
        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }
}
