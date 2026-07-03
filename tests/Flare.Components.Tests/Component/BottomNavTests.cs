using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareBottomNav / FlareBottomNavItem
// ------------------------------------------------------------------------------

public class C_FlareBottomNavTests : FlareTestContext
{
    private static RenderFragment TwoItems(string firstHref = "/", string secondHref = "/settings") => b =>
    {
        b.OpenComponent<FlareBottomNavItem>(0);
        b.AddAttribute(1, "Href", firstHref);
        b.AddAttribute(2, "Match", NavMatchMode.Exact);
        b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Home")));
        b.CloseComponent();

        b.OpenComponent<FlareBottomNavItem>(4);
        b.AddAttribute(5, "Href", secondHref);
        b.AddAttribute(6, "ChildContent", (RenderFragment)(c => c.AddContent(0, "Settings")));
        b.CloseComponent();
    };

    [Fact]
    public void RendersRootNav()
    {
        var cut = Render<FlareBottomNav>();

        Assert.NotEmpty(cut.FindAll("nav.flare-bottom-nav"));
    }

    [Fact]
    public void RendersChildItems()
    {
        var cut = Render<FlareBottomNav>(p => p
            .AddChildContent(TwoItems()));

        Assert.Equal(2, cut.FindAll(".flare-bottom-nav-item").Count);
    }

    [Fact]
    public void DefaultAriaLabel_IsBottomNavigation()
    {
        var cut = Render<FlareBottomNav>();

        Assert.Equal("Bottom navigation", cut.Find("nav").GetAttribute("aria-label"));
    }

    [Fact]
    public void CustomAriaLabel_IsApplied()
    {
        var cut = Render<FlareBottomNav>(p => p
            .Add(x => x.AriaLabel, "Main sections"));

        Assert.Equal("Main sections", cut.Find("nav").GetAttribute("aria-label"));
    }

    [Fact]
    public void RendersWithAdditionalAttributes()
    {
        var cut = Render<FlareBottomNav>(p => p
            .AddUnmatched("data-testid", "bottom-nav-root"));

        Assert.Equal("bottom-nav-root", cut.Find(".flare-bottom-nav").GetAttribute("data-testid"));
    }
}

public class C_FlareBottomNavItemTests : FlareTestContext
{
    [Fact]
    public void RendersAnchorTag()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/home")
            .AddChildContent("Home"));

        Assert.NotEmpty(cut.FindAll("a.flare-bottom-nav-item"));
    }

    [Fact]
    public void RendersLabelText()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/about")
            .AddChildContent("About Us"));

        Assert.Contains("About Us", cut.Find(".flare-bottom-nav-item__label").TextContent);
    }

    [Fact]
    public void RendersIcon()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/settings")
            .Add(x => x.Icon, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "bn-icon");
                b.CloseElement();
            }))
            .AddChildContent("Settings"));

        Assert.NotEmpty(cut.FindAll(".flare-bottom-nav-item__icon"));
        Assert.NotEmpty(cut.FindAll("#bn-icon"));
    }

    [Fact]
    public void HrefAttribute_AppliedToAnchor()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/dashboard"));

        Assert.Equal("/dashboard", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void Active_True_HasActiveClassAndAriaCurrent()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/page")
            .Add(x => x.Active, true)
            .AddChildContent("Page"));

        var a = cut.Find("a");
        Assert.Contains("flare-bottom-nav-item--active", a.ClassName ?? "");
        Assert.Equal("page", a.GetAttribute("aria-current"));
    }

    [Fact]
    public void Inactive_NoAriaCurrent()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/page")
            .AddChildContent("Page"));

        Assert.False(cut.Find("a").HasAttribute("aria-current"));
    }

    [Fact]
    public void Disabled_True_HasDisabledClass()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/locked")
            .Add(x => x.Disabled, true)
            .AddChildContent("Locked"));

        Assert.Contains("flare-bottom-nav-item--disabled", cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void Click_InvokesOnClick()
    {
        var clicked = false;
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/x")
            .Add(x => x.OnClick, () => clicked = true)
            .AddChildContent("X"));

        cut.Find("a").Click();

        Assert.True(clicked);
    }

    [Fact]
    public void OnActiveChanged_FiresWhenActive()
    {
        bool? reported = null;
        Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/x")
            .Add(x => x.Active, true)
            .Add(x => x.OnActiveChanged, (bool a) => reported = a)
            .AddChildContent("X"));

        Assert.True(reported);
    }

    [Fact]
    public void ExactMatch_ActiveOnlyOnExactRoute()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/products/details");

        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/products")
            .Add(x => x.Match, NavMatchMode.Exact)
            .AddChildContent("Products"));

        Assert.DoesNotContain("flare-bottom-nav-item--active", cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void PrefixMatch_ActiveOnChildRoute()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/products/details");

        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/products")
            .Add(x => x.Match, NavMatchMode.Prefix)
            .AddChildContent("Products"));

        Assert.Contains("flare-bottom-nav-item--active", cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void LocationChanged_UpdatesActiveStateReactively()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/home");

        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/settings")
            .Add(x => x.Match, NavMatchMode.Exact)
            .AddChildContent("Settings"));

        Assert.DoesNotContain("flare-bottom-nav-item--active", cut.Find("a").ClassName ?? "");

        cut.InvokeAsync(() => nav.NavigateTo("/settings"));
        cut.WaitForState(() => cut.Find("a").ClassName?.Contains("flare-bottom-nav-item--active") ?? false);

        Assert.Contains("flare-bottom-nav-item--active", cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void LocationChanged_DeactivatesWhenNavigatingAway()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("/settings");

        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/settings")
            .Add(x => x.Match, NavMatchMode.Exact)
            .AddChildContent("Settings"));

        Assert.Contains("flare-bottom-nav-item--active", cut.Find("a").ClassName ?? "");

        cut.InvokeAsync(() => nav.NavigateTo("/home"));
        cut.WaitForState(() => !(cut.Find("a").ClassName?.Contains("flare-bottom-nav-item--active") ?? false));

        Assert.DoesNotContain("flare-bottom-nav-item--active", cut.Find("a").ClassName ?? "");
    }
}
