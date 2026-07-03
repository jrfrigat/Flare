using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareLinkTabsTests : FlareTestContext
{
    private static RenderFragment TwoLinkTabs(NavMatchMode match = NavMatchMode.Prefix) => b =>
    {
        b.OpenComponent<FlareLinkTabs>(0);
        b.AddAttribute(1, "ChildContent", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareLinkTab>(2);
            inner.AddAttribute(3, "Label", "Login");
            inner.AddAttribute(4, "Href", "/login");
            inner.AddAttribute(5, "Match", match);
            inner.CloseComponent();

            inner.OpenComponent<FlareLinkTab>(6);
            inner.AddAttribute(7, "Label", "Register");
            inner.AddAttribute(8, "Href", "/register");
            inner.AddAttribute(9, "Match", match);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Renders_LinkTabsContainer_WithCorrectCssClass()
    {
        var cut = Render(TwoLinkTabs());

        Assert.Single(cut.FindAll(".flare-link-tabs"));
    }

    [Fact]
    public void Renders_AllItems_AsAnchors()
    {
        var cut = Render(TwoLinkTabs());

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.Equal(2, anchors.Count);
    }

    [Fact]
    public void Items_HaveCorrectLabelsAndHrefs()
    {
        var cut = Render(TwoLinkTabs());

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.Contains("Login", anchors[0].TextContent);
        Assert.Equal("/login", anchors[0].GetAttribute("href"));
        Assert.Contains("Register", anchors[1].TextContent);
        Assert.Equal("/register", anchors[1].GetAttribute("href"));
    }

    [Fact]
    public void RootElement_IsNavigationLandmark_NotTablist()
    {
        var cut = Render(TwoLinkTabs());

        // Cross-route links are a navigation landmark, not an in-page tablist: a tablist owning
        // plain <a> children (no role=tab) is an invalid ARIA relationship.
        Assert.NotEmpty(cut.FindAll("nav.flare-link-tabs"));
        Assert.Empty(cut.FindAll("[role='tablist']"));
    }

    [Fact]
    public void AriaLabel_AppliedToNavLandmark()
    {
        var cut = Render<FlareLinkTabs>(p => p
            .Add(x => x.AriaLabel, "Account sections")
            .AddChildContent<FlareLinkTab>(t => t
                .Add(x => x.Label, "One")
                .Add(x => x.Href, "/one")));

        Assert.Equal("Account sections", cut.Find("nav.flare-link-tabs").GetAttribute("aria-label"));
    }

    // ------------------------------------------------------------------
    // Active-state derivation from NavigationManager (FlareNavLink pattern)
    // ------------------------------------------------------------------

    [Fact]
    public void ExactMatch_OnlyExactRoute_IsActive()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/login");

        var cut = Render(TwoLinkTabs(NavMatchMode.Exact));

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.Contains("flare-link-tab--active", anchors[0].ClassName);
        Assert.DoesNotContain("flare-link-tab--active", anchors[1].ClassName);
    }

    [Fact]
    public void ExactMatch_SubRoute_IsNotActive()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/login/sub");

        var cut = Render(TwoLinkTabs(NavMatchMode.Exact));

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.DoesNotContain("flare-link-tab--active", anchors[0].ClassName);
    }

    [Fact]
    public void PrefixMatch_SubRoute_IsActive()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/login/sub");

        var cut = Render(TwoLinkTabs(NavMatchMode.Prefix));

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.Contains("flare-link-tab--active", anchors[0].ClassName);
        Assert.DoesNotContain("flare-link-tab--active", anchors[1].ClassName);
    }

    [Fact]
    public void PrefixMatch_DoesNotFalsePositive_OnSegmentBoundary()
    {
        // "/login" must not match "/login-history" - segment-boundary check.
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/login-history");

        var cut = Render(TwoLinkTabs(NavMatchMode.Prefix));

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.DoesNotContain("flare-link-tab--active", anchors[0].ClassName);
    }

    [Fact]
    public void ActiveState_UpdatesOnLocationChanged()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/login");

        var cut = Render(TwoLinkTabs());

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.Contains("flare-link-tab--active", anchors[0].ClassName);
        Assert.DoesNotContain("flare-link-tab--active", anchors[1].ClassName);

        nav.NavigateTo("/register");
        cut.WaitForState(() => cut.FindAll("a.flare-link-tab")[1].ClassName?.Contains("flare-link-tab--active") == true);

        anchors = cut.FindAll("a.flare-link-tab");
        Assert.DoesNotContain("flare-link-tab--active", anchors[0].ClassName);
        Assert.Contains("flare-link-tab--active", anchors[1].ClassName);
    }

    [Fact]
    public void NoMatchingRoute_NoItemIsActive()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/somewhere-else");

        var cut = Render(TwoLinkTabs());

        var anchors = cut.FindAll("a.flare-link-tab");
        Assert.DoesNotContain("flare-link-tab--active", anchors[0].ClassName);
        Assert.DoesNotContain("flare-link-tab--active", anchors[1].ClassName);
    }

    // ------------------------------------------------------------------
    // Variant CSS classes (FlareTabs variant-testing pattern)
    // ------------------------------------------------------------------

    [Fact]
    public void DefaultVariant_HasNoVariantModifierClass()
    {
        var cut = Render(TwoLinkTabs());

        var root = cut.Find(".flare-link-tabs");
        Assert.DoesNotContain("flare-link-tabs--tonal", root.ClassName);
        Assert.DoesNotContain("flare-link-tabs--filled", root.ClassName);
    }

    [Fact]
    public void TonalVariant_AppliesTonalClass()
    {
        var cut = Render<FlareLinkTabs>(p => p
            .Add(x => x.Variant, LinkTabsVariant.Tonal)
            .AddChildContent<FlareLinkTab>(t => t
                .Add(x => x.Label, "One")
                .Add(x => x.Href, "/one")));

        Assert.Contains("flare-link-tabs--tonal", cut.Find(".flare-link-tabs").ClassName);
    }

    [Fact]
    public void FilledVariant_AppliesFilledClass()
    {
        var cut = Render<FlareLinkTabs>(p => p
            .Add(x => x.Variant, LinkTabsVariant.Filled)
            .AddChildContent<FlareLinkTab>(t => t
                .Add(x => x.Label, "One")
                .Add(x => x.Href, "/one")));

        Assert.Contains("flare-link-tabs--filled", cut.Find(".flare-link-tabs").ClassName);
    }

    // ------------------------------------------------------------------
    // Misc FlareLinkTab behavior
    // ------------------------------------------------------------------

    [Fact]
    public void Disabled_True_HasDisabledClass()
    {
        var cut = Render<FlareLinkTabs>(p => p
            .AddChildContent<FlareLinkTab>(t => t
                .Add(x => x.Label, "One")
                .Add(x => x.Href, "/one")
                .Add(x => x.Disabled, true)));

        Assert.Contains("flare-link-tab--disabled", cut.Find("a.flare-link-tab").ClassName);
    }

    [Fact]
    public void Disabled_True_IsNotFocusableAndSuppressesHref()
    {
        var cut = Render<FlareLinkTabs>(p => p
            .AddChildContent<FlareLinkTab>(t => t
                .Add(x => x.Label, "One")
                .Add(x => x.Href, "/one")
                .Add(x => x.Disabled, true)));

        var a = cut.Find("a.flare-link-tab");
        Assert.False(a.HasAttribute("href"));
        Assert.Equal("true", a.GetAttribute("aria-disabled"));
        Assert.Equal("-1", a.GetAttribute("tabindex"));
    }

    [Fact]
    public void LeadingIcon_RendersIconSpan()
    {
        var cut = Render<FlareLinkTabs>(p => p
            .AddChildContent<FlareLinkTab>(t => t
                .Add(x => x.Label, "One")
                .Add(x => x.Href, "/one")
                .Add(x => x.LeadingIcon, (RenderFragment)(b =>
                {
                    b.OpenElement(0, "span");
                    b.AddAttribute(1, "id", "tab-icon");
                    b.CloseElement();
                }))));

        Assert.NotEmpty(cut.FindAll(".flare-link-tab__icon"));
    }
}
