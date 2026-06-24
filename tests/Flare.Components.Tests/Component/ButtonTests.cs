using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

// ------------------------------------------------------------------------------
// FlareButton icon-only  (icon without text -> square button; replaces FlareIconButton)
// ------------------------------------------------------------------------------

public class C_FlareButtonIconOnlyTests : FlareTestContext
{
    private static RenderFragment Icon => b => b.AddMarkupContent(0, "<i class=\"icon\"></i>");

    [Fact]
    public void IconWithoutText_AddsIconOnlyClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "default"));

        Assert.Contains("flare-btn--icon-only", cut.Find("button").ClassName);
    }

    [Fact]
    public void IconWithText_DoesNotAddIconOnlyClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.LeadingIcon, Icon)
            .AddChildContent("Label"));

        Assert.DoesNotContain("flare-btn--icon-only", cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersWithFilledVariant()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Filled)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "filled"));

        Assert.Contains("flare-btn--filled", cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersWithTonalVariant()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Tonal)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "tonal"));

        Assert.Contains("flare-btn--tonal", cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersWithOutlinedVariant()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Outlined)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "outlined"));

        Assert.Contains("flare-btn--outlined", cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersSmallSize()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Size, ButtonSize.Sm)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "small"));

        Assert.Contains("flare-btn--sm", cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersLargeSize()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "large"));

        Assert.Contains("flare-btn--lg", cut.Find("button").ClassName);
    }

    [Fact]
    public void RendersDisabled()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "disabled"));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersAriaLabel()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.LeadingIcon, Icon)
            .Add(x => x.AriaLabel, "close dialog"));

        Assert.Equal("close dialog", cut.Find("button").GetAttribute("aria-label"));
    }
}

// ------------------------------------------------------------------------------
// FlareButtonGroup  (6 tests)
// ------------------------------------------------------------------------------

public class C_FlareButtonGroupTests : FlareTestContext
{
    [Fact]
    public void RendersHorizontally()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Vertical, false));

        var div = cut.Find(".flare-btn-group");
        Assert.NotNull(div);
        Assert.DoesNotContain("flare-btn-group--vertical", div.ClassName);
    }

    [Fact]
    public void RendersVertically()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Vertical, true));

        Assert.Contains("flare-btn-group--vertical", cut.Find(".flare-btn-group").ClassName);
    }

    [Fact]
    public void RendersFullWidth()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.FullWidth, true));

        Assert.Contains("flare-btn-group--full", cut.Find(".flare-btn-group").ClassName);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .AddChildContent("<button class=\"child-btn\">A</button><button class=\"child-btn\">B</button>"));

        Assert.Equal(2, cut.FindAll(".child-btn").Count);
    }

    [Fact]
    public void RendersDefaultVariant_NoVerticalModifier()
    {
        var cut = Render<FlareButtonGroup>();

        var div = cut.Find(".flare-btn-group");
        Assert.DoesNotContain("flare-btn-group--vertical", div.ClassName);
        Assert.DoesNotContain("flare-btn-group--full", div.ClassName);
    }

    [Fact]
    public void RendersFlareButtonsInside()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save")));
                b.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll(".flare-btn--filled"));
    }

    [Fact]
    public void CascadesSizeAndVariantToChildButtons()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.Variant, ButtonVariant.Tonal)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save")));
                b.CloseComponent();
            }));

        var btn = cut.Find(".flare-btn");
        Assert.Contains("flare-btn--lg", btn.ClassName);
        Assert.Contains("flare-btn--tonal", btn.ClassName);
    }

    [Fact]
    public void ButtonKeepsOwnVariantWhenGroupDoesNotOverride()
    {
        var cut = Render<FlareButtonGroup>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareButton>(0);
                b.AddAttribute(1, "Variant", ButtonVariant.Outlined);
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(3, "Save")));
                b.CloseComponent();
            }));

        Assert.Contains("flare-btn--outlined", cut.Find(".flare-btn").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareFloatingActionButton  (8 tests)
// ------------------------------------------------------------------------------

public class C_FlareFloatingActionButtonTests : FlareTestContext
{
    [Fact]
    public void RendersDefault()
    {
        var cut = Render<FlareFloatingActionButton>();

        Assert.NotEmpty(cut.FindAll(".flare-fab"));
    }

    [Fact]
    public void RendersSmall()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Size, FabSize.Sm));

        Assert.Contains("flare-fab--sm", cut.Find(".flare-fab").ClassName);
    }

    [Fact]
    public void RendersLarge()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Size, FabSize.Lg));

        Assert.Contains("flare-fab--lg", cut.Find(".flare-fab").ClassName);
    }

    [Fact]
    public void RendersSecondaryColorClass()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Color, FlareColor.Secondary));

        Assert.Contains("flare-color-secondary", cut.Find(".flare-fab").ClassName);
    }

    [Fact]
    public void RendersCustomFabColorInline()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Color, FlareColor.Custom("#123456")));

        Assert.Contains("--fc-container", cut.Find(".flare-fab").GetAttribute("style"));
    }

    [Fact]
    public void DefaultFabColorHasNoColorClass()
    {
        var cut = Render<FlareFloatingActionButton>();

        Assert.DoesNotContain("flare-color-", cut.Find(".flare-fab").ClassName);
    }

    [Fact]
    public void RendersAnchorBottomRight()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Position, FabPosition.BottomRight));

        Assert.NotEmpty(cut.FindAll(".flare-fab-anchor--bottom-right"));
    }

    [Fact]
    public void RendersLabelSlot()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.Label, "Create")
            .Add(x => x.Position, FabPosition.Static));

        var label = cut.Find(".flare-fab__label");
        Assert.Equal("Create", label.TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareToggleButton  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareToggleButtonTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareToggleButton>();

        Assert.NotEmpty(cut.FindAll(".flare-toggle-btn"));
    }

    [Fact]
    public void UnpressedState_AriaPressedFalse()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Toggled, false));

        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void PressedState_AriaPressedTrue()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Toggled, true));

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void DisabledState_ButtonHasDisabledAttribute()
    {
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareToggleButton>(p => p
            .AddChildContent("Bookmark"));

        Assert.Contains("Bookmark", cut.Find(".flare-toggle-btn__label").TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlareButton Loading  (8 tests from Wave7)
// ------------------------------------------------------------------------------

public class C_FlareButtonLoadingTests : FlareTestContext
{
    [Fact]
    public void Loading_False_RendersChildContent()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, false)
            .AddChildContent("Click Me"));

        Assert.Contains("Click Me", cut.Find(".flare-btn__label").TextContent);
    }

    [Fact]
    public void Loading_True_RendersSpinner()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.NotEmpty(cut.FindAll("span.flare-btn__spinner"));
    }

    [Fact]
    public void Loading_True_AddsLoadingClass()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.Contains("flare-btn--loading", cut.Find("button").ClassName ?? "");
    }

    [Fact]
    public void Loading_True_DisablesButton()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Loading_True_SetsAriaBusy()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true));

        Assert.Equal("true", cut.Find("button").GetAttribute("aria-busy"));
    }

    [Fact]
    public void Loading_True_WithLoadingText_ShowsLoadingTextInLabel()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.LoadingText, "Saving..."));

        Assert.Contains("Saving...", cut.Find(".flare-btn__label").TextContent);
    }

    [Fact]
    public void Loading_False_DoesNotShowSpinner()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, false));

        Assert.Empty(cut.FindAll("span.flare-btn__spinner"));
    }

    [Fact]
    public void DefaultState_RendersNormally()
    {
        var cut = Render<FlareButton>(p => p
            .AddChildContent("Submit"));

        Assert.NotEmpty(cut.FindAll("button.flare-btn"));
        Assert.False(cut.Find("button").HasAttribute("disabled"));
    }
}

// ------------------------------------------------------------------------------
// FlareSplitButton  (4 tests)
// ------------------------------------------------------------------------------

public class C_FlareSplitButtonTests : FlareTestContext
{
    [Fact]
    public void RendersMainLabelAndTrigger()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save")));

        Assert.NotEmpty(cut.FindAll(".flare-split-btn__main"));
        Assert.NotEmpty(cut.FindAll(".flare-split-btn__trigger"));
        Assert.Contains("Save", cut.Find(".flare-split-btn__main").TextContent);
    }

    [Fact]
    public void Menu_IsClosedInitially()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save"))
            .Add(x => x.MenuItems, b =>
            {
                b.OpenComponent<FlareMenuItem>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save as...")));
                b.CloseComponent();
            }));

        Assert.Empty(cut.FindAll(".flare-menu__panel"));
    }

    [Fact]
    public void Menu_OpensOnTriggerClick_AndShowsItems()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save"))
            .Add(x => x.MenuItems, b =>
            {
                b.OpenComponent<FlareMenuItem>(0);
                b.AddAttribute(1, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(2, "Save as...")));
                b.CloseComponent();
            }));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll(".flare-menu__panel"));
        Assert.Contains("Save as...", cut.Markup);
    }

    [Fact]
    public void Disabled_DisablesBothButtons()
    {
        var cut = Render<FlareSplitButton>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "Save")));

        Assert.All(cut.FindAll(".flare-split-btn button"),
            btn => Assert.True(btn.HasAttribute("disabled")));
    }
}
