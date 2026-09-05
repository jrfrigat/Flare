using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareBottomNavTests : FlareTestContext
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

        Assert.NotEmpty(cut.FindAll($"nav.{Css.Classes.BottomNav.Root}"));
    }

    [Fact]
    public void RendersChildItems()
    {
        var cut = Render<FlareBottomNav>(p => p
            .AddChildContent(TwoItems()));

        Assert.Equal(2, cut.FindAll($".{Css.Classes.BottomNav.Item}").Count);
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

        Assert.Equal("bottom-nav-root", cut.Find($".{Css.Classes.BottomNav.Root}").GetAttribute("data-testid"));
    }
}
