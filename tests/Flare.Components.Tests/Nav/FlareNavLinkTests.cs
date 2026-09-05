using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareNavLinkTests : FlareTestContext
{
    [Fact]
    public void RendersAnchorTag()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/home")
            .AddChildContent("Home"));

        Assert.NotEmpty(cut.FindAll($"a.{Css.Classes.Navigation.NavLink}"));
    }

    [Fact]
    public void RendersChildContentInTextSpan()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/about")
            .AddChildContent("About Us"));

        Assert.Contains("About Us", cut.Find($".{Css.Classes.Navigation.NavLinkText}").TextContent);
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

        Assert.Contains(Css.Classes.Navigation.NavLinkActive, cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void Disabled_True_HasDisabledClass()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/locked")
            .Add(x => x.Disabled, true)
            .AddChildContent("Locked"));

        Assert.Contains(Css.Classes.Navigation.NavLinkDisabled, cut.Find("a").ClassName ?? "");
    }

    [Fact]
    public void Icon_RendersIconSpan()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/settings")
            .Add(x => x.IconContent, (RenderFragment)(b =>
            {
                b.OpenElement(0, "span");
                b.AddAttribute(1, "id", "nav-icon");
                b.CloseElement();
            }))
            .AddChildContent("Settings"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Navigation.NavLinkIcon}"));
    }
}

// ------------------------------------------------------------------------------
// FlareDrawer  (8 tests from Wave1)
// ------------------------------------------------------------------------------
