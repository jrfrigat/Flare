using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareBottomNavItemTests : FlareTestContext
{
    [Fact]
    public void RendersAnchorTag()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/home")
            .AddChildContent("Home"));

        Assert.NotEmpty(cut.FindAll($"a.{Css.Classes.BottomNav.Item}"));
    }

    [Fact]
    public void RendersLabelText()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/about")
            .AddChildContent("About Us"));

        Assert.Contains("About Us", cut.Find($".{Css.Classes.BottomNav.ItemLabel}").TextContent);
    }

    [Fact]
    public void RendersIcon()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/settings")
            .Add(x => x.IconContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "bn-icon");
                b.CloseElement();
            }))
            .AddChildContent("Settings"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.BottomNav.ItemIcon}"));
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
        Assert.Contains(Css.Classes.BottomNav.ItemActive, a.ClassName ?? "");
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

        Assert.Contains(Css.Classes.BottomNav.ItemDisabled, cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void Disabled_True_IsNotFocusableAndSuppressesHref()
    {
        var cut = Render<FlareBottomNavItem>(p => p
            .Add(x => x.Href, "/locked")
            .Add(x => x.Disabled, true)
            .AddChildContent("Locked"));

        var a = cut.Find("a");
        Assert.False(a.HasAttribute("href"));
        Assert.Equal("true", a.GetAttribute("aria-disabled"));
        Assert.Equal("-1", a.GetAttribute("tabindex"));
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

        Assert.DoesNotContain(Css.Classes.BottomNav.ItemActive, cut.Find("a").ClassName ?? "");
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

        Assert.Contains(Css.Classes.BottomNav.ItemActive, cut.Find("a").ClassName ?? "");
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

        Assert.DoesNotContain(Css.Classes.BottomNav.ItemActive, cut.Find("a").ClassName ?? "");

        cut.InvokeAsync(() => nav.NavigateTo("/settings"));
        cut.WaitForState(() => cut.Find("a").ClassName?.Contains(Css.Classes.BottomNav.ItemActive) ?? false);

        Assert.Contains(Css.Classes.BottomNav.ItemActive, cut.Find("a").ClassName ?? "");
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

        Assert.Contains(Css.Classes.BottomNav.ItemActive, cut.Find("a").ClassName ?? "");

        cut.InvokeAsync(() => nav.NavigateTo("/home"));
        cut.WaitForState(() => !(cut.Find("a").ClassName?.Contains(Css.Classes.BottomNav.ItemActive) ?? false));

        Assert.DoesNotContain(Css.Classes.BottomNav.ItemActive, cut.Find("a").ClassName ?? "");
    }
}
