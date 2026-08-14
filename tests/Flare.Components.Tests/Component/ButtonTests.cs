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
    public void CollapsibleRendersAnOverflowControlAndASecondCopy()
    {
        // The fold itself is a measurement and only a browser can make it, so what is testable here is
        // the contract the measurer needs: the trailing overflow control exists, and the panel holds a
        // second copy of the same content for it to reveal. Without the copy there would be nothing to
        // fold INTO, and the group would just clip.
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Collapsible, true)
            .AddChildContent("<button class=\"flare-btn\">One</button><button class=\"flare-btn\">Two</button>"));

        var root = cut.Find(".flare-btn-group");
        Assert.Contains("flare-btn-group--collapsible", root.ClassName);
        Assert.Single(cut.FindAll($".{Css.Classes.ButtonGroup.More}"));

        // The copies live behind the menu, so they exist only once it is open - which is also why
        // the measurer treats the panel as optional rather than assuming it is there.
        Assert.Empty(cut.FindAll($".{Css.Classes.ButtonGroup.OverflowList}"));
        cut.Find($".{Css.Classes.ButtonGroup.More} .flare-btn").Click();
        Assert.Single(cut.FindAll($".{Css.Classes.ButtonGroup.OverflowList}"));
        Assert.Equal(2, cut.FindAll($".{Css.Classes.ButtonGroup.OverflowList} .flare-btn").Count);
    }

    [Fact]
    public void AToggleIsAButtonAndCarriesTheButtonFamily()
    {
        // The whole point of the rebuild: a toggle segment and a plain segment are the same element with
        // the same classes, so every group rule reaches both with the same token family. While the toggle
        // was a control of its own, the group's press rule found it holding a different padding token and
        // GREW the neighbours it was supposed to shrink.
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.Variant, ButtonVariant.Outlined)
            .Add(x => x.Size, ButtonSize.Lg)
            .Add(x => x.Toggled, true)
            .AddChildContent("Bold"));

        var btn = cut.Find("button");
        Assert.Contains(Css.Classes.Button.Root, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Outlined, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Lg, btn.ClassName);
        Assert.Contains(Css.Classes.Button.Selected, btn.ClassName);
        // An unselected toggle must still say it is a toggle: an absent aria-pressed reads as a plain
        // command, so "false" is the state and not the absence of one.
        Assert.Equal("true", btn.GetAttribute("aria-pressed"));

        var off = Render<FlareToggleButton>(p => p.AddChildContent("Bold"));
        Assert.Equal("false", off.Find("button").GetAttribute("aria-pressed"));
        Assert.DoesNotContain(Css.Classes.Button.Selected, off.Find("button").ClassName);
    }

    [Fact]
    public void TheLabelCanChangeWithTheState()
    {
        // A toggle whose two states are different verbs says so in the label, not only in the icon.
        var cut = Render<FlareToggleButton>(p => p
            .Add(x => x.OnLabel, (RenderFragment)(b => b.AddContent(0, "Following")))
            .AddChildContent("Follow"));

        Assert.Contains("Follow", cut.Find("button").TextContent);
        Assert.DoesNotContain("Following", cut.Find("button").TextContent);

        cut.Find("button").Click();
        Assert.Contains("Following", cut.Find("button").TextContent);
    }

    [Fact]
    public void TheLabelStaysPutWhenOnlyOneIsGiven()
    {
        // Falling back keeps a toggle that changes only colour and shape from having to say its label
        // twice.
        var cut = Render<FlareToggleButton>(p => p.AddChildContent("Bold"));
        cut.Find("button").Click();
        Assert.Contains("Bold", cut.Find("button").TextContent);
    }

    [Fact]
    public void APlainButtonIsNotAToggle()
    {
        // The tri-state is what keeps that promise in the other direction: an ordinary button must not
        // grow an aria-pressed just because the parameter exists.
        var cut = Render<FlareButton>(p => p.AddChildContent("Save"));
        Assert.False(cut.Find("button").HasAttribute("aria-pressed"));
    }

    [Fact]
    public void OverflowPanelIsAGroupAndNotAMenu()
    {
        // The folded segments are buttons, not menu items: they take the focus themselves. A menu panel
        // keeps the focus and swallows the keys that would move it, which is right for items that cannot
        // be focused and wrong here - it would leave every folded button reachable by nothing but Escape.
        var cut = Render<FlareButtonGroup>(p => p
            .Add(x => x.Collapsible, true)
            .AddChildContent("<button class=\"flare-btn\">One</button>"));

        cut.Find($".{Css.Classes.ButtonGroup.More} .flare-btn").Click();
        Assert.Equal("group", cut.Find($".{Css.Classes.Menu.Panel}").GetAttribute("role"));
    }

    [Fact]
    public void OverflowEllipsisTurnsWithTheGroup()
    {
        // The dots run ACROSS the bar they fold, so a row gets the vertical ellipsis and a column the
        // horizontal one - the same convention an app bar and a navigation rail follow.
        var horizontal = Render<FlareButtonGroup>(p => p.Add(x => x.Collapsible, true));
        Assert.Contains(FlareIcons.MoreVert.Data, horizontal.Markup);

        var vertical = Render<FlareButtonGroup>(p => p
            .Add(x => x.Collapsible, true)
            .Add(x => x.Vertical, true));
        Assert.Contains(FlareIcons.MoreHoriz.Data, vertical.Markup);
    }

    [Fact]
    public void NotCollapsibleRendersNoOverflowControl()
    {
        var cut = Render<FlareButtonGroup>(p => p.AddChildContent("<button class=\"flare-btn\">One</button>"));
        Assert.DoesNotContain("flare-btn-group--collapsible", cut.Find(".flare-btn-group").ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.ButtonGroup.More}"));
    }

    [Fact]
    public void NamesItsModel()
    {
        // The two models behave differently under a press - a standard group's segments trade width
        // with each other, a connected group's only change shape - so the markup has to say which one
        // it is rather than one being the absence of the other. Standard is the default.
        foreach (var (connected, expected, notExpected) in new[]
                 {
                     (false, "flare-btn-group--standard", "flare-btn-group--connected"),
                     (true, "flare-btn-group--connected", "flare-btn-group--standard"),
                 })
        {
            var cut = Render<FlareButtonGroup>(p => p.Add(x => x.Connected, connected));
            var cls = cut.Find(".flare-btn-group").ClassName;
            Assert.Contains(expected, cls);
            Assert.DoesNotContain(notExpected, cls);
        }
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

        Assert.NotEmpty(cut.FindAll(".flare-btn"));
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

        Assert.Contains("Bookmark", cut.Find($".{Css.Classes.Button.Label}").TextContent);
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
