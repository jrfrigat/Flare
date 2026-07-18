using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareTabs vertical orientation  (6 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlareTabsVerticalTests : FlareTestContext
{
    private static RenderFragment TwoTabsVertical() => b =>
    {
        b.OpenComponent<FlareTabs>(0);
        b.AddAttribute(1, "Placement", TabsPlacement.Left);
        b.AddAttribute(2, "ChildContent", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareTab>(3);
            inner.AddAttribute(4, "Label", "Settings");
            inner.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(6, "<p>Settings Content</p>")));
            inner.CloseComponent();

            inner.OpenComponent<FlareTab>(7);
            inner.AddAttribute(8, "Label", "Profile");
            inner.AddAttribute(9, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(10, "<p>Profile Content</p>")));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void RendersVerticalOrientation()
    {
        var cut = Render(TwoTabsVertical());

        Assert.Contains("flare-tabs--vertical", cut.Find(".flare-tabs").ClassName);
    }

    [Fact]
    public void RendersHorizontalOrientation_Default()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        Assert.DoesNotContain("flare-tabs--vertical", cut.Find(".flare-tabs").ClassName);
    }

    [Fact]
    public void KeyboardNavVertical_ArrowDownMovesToNextTab()
    {
        var cut = Render(TwoTabsVertical());

        cut.Find("[role='tablist']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        var tabs = cut.FindAll(".flare-tabs__tab");
        Assert.Contains("flare-tabs__tab--active", tabs[1].ClassName);
    }

    [Fact]
    public void KeyboardNavVertical_ArrowUpMovesToPreviousTab()
    {
        var cut = Render(TwoTabsVertical());

        cut.FindAll(".flare-tabs__tab")[1].Click();

        cut.Find("[role='tablist']").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });

        var tabs = cut.FindAll(".flare-tabs__tab");
        Assert.Contains("flare-tabs__tab--active", tabs[0].ClassName);
    }

    [Fact]
    public void RendersVerticalActiveTab_HasActiveClass()
    {
        var cut = Render(TwoTabsVertical());

        var tabs = cut.FindAll(".flare-tabs__tab");
        Assert.Contains("flare-tabs__tab--active", tabs[0].ClassName);
    }

    [Fact]
    public void RendersVerticalTabContent_FirstTabByDefault()
    {
        var cut = Render(TwoTabsVertical());

        // All panels always rendered (no state loss); inactive panel has hidden class
        Assert.Contains("Settings Content", cut.Markup);
        var panels = cut.FindAll(".flare-tabs__panel");
        Assert.Equal(2, panels.Count);
        Assert.DoesNotContain("flare-tab-panel--hidden", panels[0].ClassName);
        Assert.Contains("flare-tab-panel--hidden", panels[1].ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareAccordion  (7 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareAccordionTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareAccordion>();

        Assert.NotEmpty(cut.FindAll(".flare-accordion"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddChildContent("<p id=\"inner\">Hello</p>"));

        Assert.NotEmpty(cut.FindAll("#inner"));
    }

    [Fact]
    public void AllowMultipleFalse_IsDefault()
    {
        var cut = Render<FlareAccordion>();

        Assert.NotEmpty(cut.FindAll(".flare-accordion"));
    }

    [Fact]
    public void AllowMultipleTrue_RendersComponent()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.AllowMultiple, true));

        Assert.NotEmpty(cut.FindAll(".flare-accordion"));
    }

    [Fact]
    public void RendersWithAdditionalAttributes()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddUnmatched("data-testid", "accordion-root"));

        Assert.Equal("accordion-root", cut.Find(".flare-accordion").GetAttribute("data-testid"));
    }

    [Fact]
    public void ProvidesCascadingValue()
    {
        var cut = Render<FlareAccordion>(p => p
            .AddChildContent<FlareAccordionPanel>(pp => pp
                .Add(x => x.Header, "Panel A")));

        Assert.NotEmpty(cut.FindAll(".flare-accordion"));
    }

    [Fact]
    public void RendersWithStyleParam()
    {
        var cut = Render<FlareAccordion>(p => p
            .Add(x => x.Style, "border:1px solid red"));

        var div = cut.Find(".flare-accordion");
        Assert.Contains("border", div.GetAttribute("style") ?? "");
    }
}

// ------------------------------------------------------------------------------
// FlareAccordionPanel  (8 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareAccordionPanelTests : FlareTestContext
{
    private IRenderedComponent<FlareAccordionPanel> RenderPanel(
        Action<ComponentParameterCollectionBuilder<FlareAccordionPanel>>? configure = null)
    {
        return Render<FlareAccordion>(p => p
            .AddChildContent<FlareAccordionPanel>(configure ?? (_ => { })))
            .FindComponent<FlareAccordionPanel>();
    }

    [Fact]
    public void RendersRootDiv()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "My Header"));

        Assert.NotEmpty(cut.FindAll(".flare-accordion-panel"));
    }

    [Fact]
    public void RendersHeaderText()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Section Title"));

        Assert.Contains("Section Title", cut.Markup);
    }

    [Fact]
    public void RendersHeaderButton()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Clickable Header"));

        Assert.NotEmpty(cut.FindAll("button.flare-accordion-panel__header"));
    }

    [Fact]
    public void CollapsedByDefault_AriaExpandedFalse()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Panel"));

        var btn = cut.Find("button.flare-accordion-panel__header");
        Assert.Equal("false", btn.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ExpandedParam_True_AriaExpandedTrue()
    {
        var cut = RenderPanel(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, true));

        var btn = cut.Find("button.flare-accordion-panel__header");
        Assert.Equal("true", btn.GetAttribute("aria-expanded"));
    }

    [Fact]
    public void ExpandedParam_True_HasExpandedClass()
    {
        var cut = RenderPanel(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, true));

        Assert.NotEmpty(cut.FindAll(".flare-accordion-panel--expanded"));
    }

    [Fact]
    public void CollapsedByDefault_NoExpandedClass()
    {
        var cut = RenderPanel(p => p.Add(x => x.Header, "Panel"));

        Assert.Empty(cut.FindAll(".flare-accordion-panel--expanded"));
    }

    [Fact]
    public void ToggleExpandsPanel_ClickHeader()
    {
        var cut = RenderPanel(p => p
            .Add(x => x.Header, "Panel")
            .Add(x => x.Expanded, false));

        cut.Find("button.flare-accordion-panel__header").Click();

        Assert.NotEmpty(cut.FindAll(".flare-accordion-panel--expanded"));
    }
}

// ------------------------------------------------------------------------------
// FlareStepper  (8 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareStepperTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareStepper>();

        Assert.NotEmpty(cut.FindAll(".flare-stepper"));
    }

    [Fact]
    public void DefaultOrientation_HasHorizontalClass()
    {
        var cut = Render<FlareStepper>();

        Assert.NotEmpty(cut.FindAll(".flare-stepper--horizontal"));
    }

    [Fact]
    public void VerticalOrientation_HasVerticalClass()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.Orientation, StepperOrientation.Vertical));

        Assert.NotEmpty(cut.FindAll(".flare-stepper--vertical"));
    }

    [Fact]
    public void RendersStepHeader()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(sp => sp
                .Add(x => x.Label, "Step One")));

        Assert.NotEmpty(cut.FindAll(".flare-stepper__header"));
    }

    [Fact]
    public void SingleStep_IsActive()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent<FlareStep>(sp => sp
                .Add(x => x.Label, "First")));

        Assert.NotEmpty(cut.FindAll(".flare-stepper__step--active"));
    }

    [Fact]
    public void MultipleSteps_FirstIsActive_SecondIsUpcoming()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
                b.OpenComponent<FlareStep>(2);
                b.AddAttribute(3, "Label", "Two");
                b.CloseComponent();
            }));

        var active = cut.FindAll(".flare-stepper__step--active");
        var upcoming = cut.FindAll(".flare-stepper__step--upcoming");

        Assert.Single(active);
        Assert.Single(upcoming);
    }

    [Fact]
    public void LinearTrue_IsDefault()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.Linear, true));

        Assert.NotEmpty(cut.FindAll(".flare-stepper"));
    }

    [Fact]
    public void RendersConnectorBetweenSteps()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "A");
                b.CloseComponent();
                b.OpenComponent<FlareStep>(2);
                b.AddAttribute(3, "Label", "B");
                b.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll(".flare-stepper__connector"));
    }

    // Builds three labelled steps; the middle one's Skippable flag is parameterised.
    private static RenderFragment ThreeSteps(string a, string b, string c, bool middleSkippable = false) => builder =>
    {
        builder.OpenComponent<FlareStep>(0);
        builder.AddAttribute(1, "Label", a);
        builder.CloseComponent();
        builder.OpenComponent<FlareStep>(2);
        builder.AddAttribute(3, "Label", b);
        builder.AddAttribute(4, "Skippable", middleSkippable);
        builder.CloseComponent();
        builder.OpenComponent<FlareStep>(5);
        builder.AddAttribute(6, "Label", c);
        builder.CloseComponent();
    };

    private static string? ActiveLabel(Bunit.IRenderedComponent<FlareStepper> cut) =>
        cut.FindAll(".flare-stepper__step--active .flare-stepper__label").FirstOrDefault()?.TextContent;

    [Fact]
    public void ActiveIndexParameter_SelectsThatStep()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        Assert.Equal("Two", ActiveLabel(cut));
    }

    [Fact]
    public void Navigation_FiresActiveIndexChanged()
    {
        var captured = -1;
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => captured = i))
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        cut.InvokeAsync(() => cut.Instance.Next());

        Assert.Equal(1, captured);
        Assert.Equal("Two", ActiveLabel(cut));
    }

    [Fact]
    public void ActionContent_ReplacesBuiltInButtons()
    {
        var cut = Render<FlareStepper>(p => p
            .Add(x => x.ActionContent, (RenderFragment<StepperContext>)(ctx => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-nav");
                builder.AddContent(2, $"{ctx.ActiveIndex + 1}/{ctx.Count}");
                builder.CloseElement();
            }))
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        Assert.Empty(cut.FindAll(".flare-stepper__nav-btn"));
        var nav = cut.Find(".custom-nav");
        Assert.Equal("1/3", nav.TextContent);
    }

    [Fact]
    public void DefaultButtons_RenderWhenNoActionContent()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareStep>(0);
                b.AddAttribute(1, "Label", "One");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(3, "<p>body</p>")));
                b.CloseComponent();
                b.OpenComponent<FlareStep>(4);
                b.AddAttribute(5, "Label", "Two");
                b.CloseComponent();
            }));

        // First step shows a primary "Next" button and no "Back".
        Assert.NotEmpty(cut.FindAll(".flare-stepper__nav-btn--primary"));
    }

    [Fact]
    public void Skippable_GoToPastSkippableStep_Advances()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(ThreeSteps("One", "Two", "Three", middleSkippable: true)));

        cut.InvokeAsync(() => cut.Instance.GoTo(2));

        Assert.Equal("Three", ActiveLabel(cut));
    }

    [Fact]
    public void NonSkippable_GoToPastStep_Blocked()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(ThreeSteps("One", "Two", "Three", middleSkippable: false)));

        cut.InvokeAsync(() => cut.Instance.GoTo(2));

        // Linear stepper refuses to jump over a non-skippable step.
        Assert.Equal("One", ActiveLabel(cut));
    }

    [Fact]
    public void GoTo_ImmediateNextStep_Advances_InLinearMode()
    {
        var cut = Render<FlareStepper>(p => p
            .AddChildContent(ThreeSteps("One", "Two", "Three")));

        cut.InvokeAsync(() => cut.Instance.GoTo(1));

        Assert.Equal("Two", ActiveLabel(cut));
    }
}

// ------------------------------------------------------------------------------
// FlareBreadcrumb  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareBreadcrumbTests : FlareTestContext
{
    private static readonly BreadcrumbItem[] _items =
    [
        new("Home", "/"),
        new("Products", "/products"),
        new("Details"),
    ];

    [Fact]
    public void RendersRootNav()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        Assert.NotEmpty(cut.FindAll(".flare-breadcrumb"));
    }

    [Fact]
    public void RendersAllItems()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        Assert.Equal(3, cut.FindAll(".flare-breadcrumb__item").Count);
    }

    [Fact]
    public void LastItem_HasCurrentClass()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        var items = cut.FindAll(".flare-breadcrumb__item");
        Assert.Contains("flare-breadcrumb__item--current", items[^1].ClassName);
    }

    [Fact]
    public void NonLastItems_HaveLinks()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items));

        Assert.NotEmpty(cut.FindAll("a.flare-breadcrumb__link"));
    }

    [Fact]
    public void SeparatorRendered_BetweenItems()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Separator, ">"));

        var seps = cut.FindAll(".flare-breadcrumb__separator");
        Assert.Equal(2, seps.Count);
    }

    [Fact]
    public void CustomSeparator_Rendered()
    {
        var cut = Render<FlareBreadcrumb>(p => p
            .Add(x => x.Items, _items)
            .Add(x => x.Separator, "»"));

        var sep = cut.Find(".flare-breadcrumb__separator");
        Assert.Contains("»", sep.TextContent);
    }
}

// ------------------------------------------------------------------------------
// FlarePagination  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlarePaginationTests : FlareTestContext
{
    [Fact]
    public void RendersRootNav()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5));

        Assert.NotEmpty(cut.FindAll(".flare-pagination"));
    }

    [Fact]
    public void RendersPageButtons()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 3)
            .Add(x => x.Current, 1));

        var buttons = cut.FindAll("button");
        Assert.True(buttons.Count >= 3);
    }

    [Fact]
    public void PreviousButton_DisabledOnFirstPage()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Current, 1));

        var prevBtn = cut.Find("button[aria-label='Previous page']");
        Assert.True(prevBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void NextButton_DisabledOnLastPage()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Current, 5));

        var nextBtn = cut.Find("button[aria-label='Next page']");
        Assert.True(nextBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void ActivePage_HasActiveClass()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 5)
            .Add(x => x.Current, 2));

        var activeBtn = cut.Find("button[aria-current='page']");
        Assert.Contains("flare-pagination__btn--active", activeBtn.ClassName);
    }

    [Fact]
    public void LargePageCount_ShowsEllipsis()
    {
        var cut = Render<FlarePagination>(p => p
            .Add(x => x.TotalPages, 20)
            .Add(x => x.Current, 10));

        Assert.NotEmpty(cut.FindAll(".flare-pagination__ellipsis"));
    }
}

// ------------------------------------------------------------------------------
// FlareNavLink  (6 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareNavLinkTests : FlareTestContext
{
    [Fact]
    public void RendersAnchorTag()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/home")
            .AddChildContent("Home"));

        Assert.NotEmpty(cut.FindAll("a.flare-nav-link"));
    }

    [Fact]
    public void RendersChildContentInTextSpan()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/about")
            .AddChildContent("About Us"));

        Assert.Contains("About Us", cut.Find(".flare-nav-link__text").TextContent);
    }

    [Fact]
    public void HrefAttribute_AppliedToAnchor()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/dashboard"));

        Assert.Equal("/dashboard", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void Active_True_HasActiveClass()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/page")
            .Add(x => x.Active, true)
            .AddChildContent("Page"));

        Assert.Contains("flare-nav-link--active", cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void Disabled_True_HasDisabledClass()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/locked")
            .Add(x => x.Disabled, true)
            .AddChildContent("Locked"));

        Assert.Contains("flare-nav-link--disabled", cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void Icon_RendersIconSpan()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/settings")
            .Add(x => x.Icon, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "nav-icon");
                b.CloseElement();
            }))
            .AddChildContent("Settings"));

        Assert.NotEmpty(cut.FindAll(".flare-nav-link__icon"));
    }
}

// ------------------------------------------------------------------------------
// FlareDrawer  (8 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlareDrawerTests : FlareTestContext
{
    [Fact]
    public void RendersTemporary_DefaultVariant()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, false));

        var drawer = cut.Find(".flare-drawer");
        Assert.DoesNotContain("flare-drawer--permanent", drawer.ClassName);
        Assert.DoesNotContain("flare-drawer--mini", drawer.ClassName);
    }

    [Fact]
    public void RendersPermanent()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Permanent));

        Assert.Contains("flare-drawer--permanent", cut.Find(".flare-drawer").ClassName);
    }

    [Fact]
    public void RendersMini()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Mini));

        Assert.Contains("flare-drawer--mini", cut.Find(".flare-drawer").ClassName);
    }

    [Fact]
    public void RendersScrimForTemporaryWhenOpen()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true));

        Assert.NotEmpty(cut.FindAll(".flare-drawer-scrim"));
    }

    [Fact]
    public void NoScrimForPermanent()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Permanent)
            .Add(x => x.Open, true));

        Assert.Empty(cut.FindAll(".flare-drawer-scrim"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Permanent)
            .AddChildContent("<nav class=\"nav-content\">Nav</nav>"));

        Assert.NotNull(cut.Find(".nav-content"));
    }

    [Fact]
    public void RendersOpenState()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true));

        Assert.Contains("flare-drawer--open", cut.Find(".flare-drawer").ClassName);
    }

    [Fact]
    public void RendersClosedState_NoOpenClass()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, false));

        Assert.DoesNotContain("flare-drawer--open", cut.Find(".flare-drawer").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareMenu  (8 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareMenuTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareMenu>();

        Assert.NotEmpty(cut.FindAll(".flare-menu"));
    }

    [Fact]
    public void RendersActivatorDiv()
    {
        var cut = Render<FlareMenu>();

        Assert.NotEmpty(cut.FindAll(".flare-menu__activator"));
    }

    [Fact]
    public void MenuPanelHiddenInitially()
    {
        var cut = Render<FlareMenu>();

        Assert.Empty(cut.FindAll(".flare-menu__panel"));
    }

    [Fact]
    public void ClickActivator_OpensMenuPanel()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button id=\"act\">Open</button>")));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll(".flare-menu__panel"));
    }

    [Fact]
    public void MenuPanel_HasRoleMenu()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>")));

        cut.Find(".flare-menu__activator").Click();

        Assert.Equal("menu", cut.Find(".flare-menu__panel").GetAttribute("role"));
    }

    [Fact]
    public void RendersMenuItems_WhenOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Item One")));

        cut.Find(".flare-menu__activator").Click();

        Assert.Contains("Item One", cut.Markup);
    }

    [Fact]
    public void DefaultAnchor_HasBottomLeftClass()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>")));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll(".flare-menu__panel--bottom-left"));
    }

    [Fact]
    public void RendersBackdrop_WhenOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>")));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll(".flare-menu__backdrop"));
    }
}

// ------------------------------------------------------------------------------
// FlareNavGroup  (8 tests from Wave7)
// ------------------------------------------------------------------------------

public class C_FlareNavGroupTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareNavGroup>();

        Assert.NotEmpty(cut.FindAll(".flare-nav-group"));
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Navigation"));

        Assert.Contains("Navigation", cut.Find(".flare-nav-group__title").TextContent);
    }

    [Fact]
    public void Icon_RendersIconSpan()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Icon, FlareIcons.Home));

        // "home" is built in, so the icon renders as inline SVG (no Material font).
        Assert.Equal(FlareIcons.Home.Data, cut.Find(".flare-nav-group__icon path").GetAttribute("d"));
    }

    [Fact]
    public void Expanded_False_HidesChildren()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, false)
            .AddChildContent("<a id=\"nav-child\">Link</a>"));

        // Items always rendered in DOM (no state loss); hidden via CSS class when collapsed
        Assert.NotEmpty(cut.FindAll("#nav-child"));
        var items = cut.Find(".flare-nav-group__items");
        Assert.DoesNotContain("flare-nav-group__items--open", items.ClassName);
    }

    [Fact]
    public void Expanded_True_ShowsChildren()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, true)
            .AddChildContent("<a id=\"nav-child-visible\">Link</a>"));

        Assert.NotEmpty(cut.FindAll("#nav-child-visible"));
    }

    [Fact]
    public void HeaderButton_IsClickable()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, false)
            .AddChildContent("<a id=\"nav-link\">Link</a>"));

        cut.Find("button.flare-nav-group__header").Click();

        Assert.NotEmpty(cut.FindAll("#nav-link"));
    }

    [Fact]
    public void RendersChevronElement()
    {
        var cut = Render<FlareNavGroup>();

        Assert.NotEmpty(cut.FindAll(".flare-nav-group__chevron"));
    }

    [Fact]
    public void Expanded_True_RendersChildContent()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Expanded, true)
            .AddChildContent("<span id=\"group-content\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#group-content"));
    }
}

// ------------------------------------------------------------------------------
// FlareNavGroup auto-expand + nesting (active child reveals the group chain)
// ------------------------------------------------------------------------------

public class C_FlareNavAutoExpandTests : FlareTestContext
{
    [Fact]
    public void NavLink_OnActiveChanged_FiresWhenActive()
    {
        bool? reported = null;
        Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/x")
            .Add(x => x.Active, true)
            .Add(x => x.OnActiveChanged, (bool a) => reported = a)
            .AddChildContent("X"));

        Assert.True(reported);
    }

    [Fact]
    public void NavGroup_AutoExpands_WhenChildLinkActive()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Group")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareNavLink>(link => link
                .Add(l => l.Href, "/deep")
                .Add(l => l.Active, true)
                .AddChildContent("Deep")));

        Assert.NotEmpty(cut.FindAll(".flare-nav-group__items--open"));
        Assert.Contains("flare-nav-group--expanded", cut.Find(".flare-nav-group").ClassName);
    }

    [Fact]
    public void NavGroup_StaysCollapsed_WhenNoChildActive()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Group")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareNavLink>(link => link
                .Add(l => l.Href, "/inactive")
                .AddChildContent("Inactive")));

        Assert.Empty(cut.FindAll(".flare-nav-group__items--open"));
    }

    [Fact]
    public void NestedGroups_BothExpand_WhenDeepLinkActive()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Outer")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareNavGroup>(inner => inner
                .Add(g => g.Label, "Inner")
                .Add(g => g.Expanded, false)
                .AddChildContent<FlareNavLink>(link => link
                    .Add(l => l.Href, "/deep")
                    .Add(l => l.Active, true)
                    .AddChildContent("Deep"))));

        // both the outer and inner groups end up expanded
        Assert.Equal(2, cut.FindAll(".flare-nav-group__items--open").Count);
        Assert.Equal(2, cut.FindAll(".flare-nav-group--expanded").Count);
    }
}
