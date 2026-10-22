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

    // Disabled used to keep the href, so Enter still navigated without the callback and the link
    // stayed in the tab order.
    [Fact]
    public void Disabled_RemovesHrefAndLeavesTheTabOrder()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/locked")
            .Add(x => x.Disabled, true)
            .AddChildContent("Locked"));

        var link = cut.Find("a");
        Assert.Null(link.GetAttribute("href"));
        Assert.Equal("true", link.GetAttribute("aria-disabled"));
        Assert.Equal("-1", link.GetAttribute("tabindex"));
    }

    [Fact]
    public void Disabled_ClickDoesNotInvokeOnClick()
    {
        var clicks = 0;
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/locked")
            .Add(x => x.Disabled, true)
            .Add(x => x.OnClick, () => clicks++)
            .AddChildContent("Locked"));

        cut.Find("a").Click();

        Assert.Equal(0, clicks);
        Assert.Null(cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void ReenabledLink_RestoresHrefAndTabOrder()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/page")
            .Add(x => x.Disabled, true));

        Assert.Null(cut.Find("a").GetAttribute("href"));

        cut.Render(p => p
            .Add(x => x.Href, "/page")
            .Add(x => x.Disabled, false));

        var link = cut.Find("a");
        Assert.Equal("/page", link.GetAttribute("href"));
        Assert.Null(link.GetAttribute("aria-disabled"));
        Assert.Null(link.GetAttribute("tabindex"));
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

    [Fact]
    public void Badge_DefaultColorKeepsTheErrorRole()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/inbox")
            .Add(x => x.BadgeContent, (RenderFragment)(b => b.AddContent(0, "3")))
            .AddChildContent("Inbox"));

        var badge = cut.Find($".{Css.Classes.Navigation.NavLinkBadge}");
        Assert.Contains(FlareColor.Error.CssClass, badge.ClassName);
        Assert.Null(badge.GetAttribute("style"));
    }

    [Fact]
    public void Badge_RoleColorAddsTheSharedColorClass()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/inbox")
            .Add(x => x.BadgeColor, FlareColor.Primary)
            .Add(x => x.BadgeContent, (RenderFragment)(b => b.AddContent(0, "3")))
            .AddChildContent("Inbox"));

        var badge = cut.Find($".{Css.Classes.Navigation.NavLinkBadge}");
        Assert.Contains(FlareColor.Primary.CssClass, badge.ClassName);
    }

    [Fact]
    public void Badge_CustomColorPublishesTheLocalPair()
    {
        var cut = Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/inbox")
            .Add(x => x.BadgeColor, FlareColor.Custom("#123456"))
            .Add(x => x.BadgeContent, (RenderFragment)(b => b.AddContent(0, "3")))
            .AddChildContent("Inbox"));

        var style = cut.Find($".{Css.Classes.Navigation.NavLinkBadge}").GetAttribute("style");
        Assert.Contains($"{Css.Tokens.LocalColor.Main}:#123456", style);
        Assert.Contains($"{Css.Tokens.LocalColor.On}:", style);
    }
}
