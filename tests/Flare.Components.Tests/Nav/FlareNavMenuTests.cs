using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareNavMenuTests : FlareTestContext
{
    [Fact]
    public void RendersNavWithChildContent()
    {
        var cut = Render<FlareNavMenu>(p => p
            .AddChildContent("<a class=\"link\">Home</a>"));
        Assert.NotEmpty(cut.FindAll($"nav.{Css.Classes.Navigation.NavMenu}"));
        Assert.NotEmpty(cut.FindAll(".link"));
    }

    [Fact]
    public void HideScrollbar_AddsModifier()
    {
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.HideScrollbar, true));
        Assert.Contains(Css.Classes.Navigation.NavMenuNoScrollbar, cut.Find($"nav.{Css.Classes.Navigation.NavMenu}").ClassName);
    }

    [Fact]
    public void Mode_Rail_AddsRailModifier()
    {
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.Mode, NavMenuMode.Rail));
        Assert.Contains(Css.Classes.Navigation.NavMenuRail, cut.Find($"nav.{Css.Classes.Navigation.NavMenu}").ClassName);
    }

    [Fact]
    public void Mode_Full_OverridesRailFlag()
    {
        // An explicit Full mode wins over the legacy Rail flag.
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.Mode, NavMenuMode.Full).Add(x => x.Rail, true));
        Assert.DoesNotContain(Css.Classes.Navigation.NavMenuRail, cut.Find($"nav.{Css.Classes.Navigation.NavMenu}").ClassName);
    }

    [Fact]
    public void Mode_RailLabeled_AddsBothRailModifiers()
    {
        var cut = Render<FlareNavMenu>(p => p.Add(x => x.Mode, NavMenuMode.RailLabeled));
        var cls = cut.Find($"nav.{Css.Classes.Navigation.NavMenu}").ClassName;
        Assert.Contains(Css.Classes.Navigation.NavMenuRail, cls);
        Assert.Contains(Css.Classes.Navigation.NavMenuRailLabeled, cls);
    }
}
