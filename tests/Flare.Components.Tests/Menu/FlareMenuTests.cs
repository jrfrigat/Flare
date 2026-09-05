using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareMenuTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareMenu>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Root}"));
    }

    [Fact]
    public void RendersActivatorDiv()
    {
        var cut = Render<FlareMenu>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Activator}"));
    }

    [Fact]
    public void MenuPanelHiddenInitially()
    {
        var cut = Render<FlareMenu>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public void ClickActivator_OpensMenuPanel()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button id=\"act\">Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public void MenuPanel_HasRoleMenu()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Equal("menu", cut.Find($".{Css.Classes.Menu.Panel}").GetAttribute("role"));
    }

    [Fact]
    public void RendersMenuItems_WhenOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Item One")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Contains("Item One", cut.Markup);
    }

    [Fact]
    public void DefaultAnchor_HasBottomLeftClass()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.BottomLeft}"));
    }

    [Fact]
    public void RendersBackdrop_WhenOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
    }
}

// ------------------------------------------------------------------------------
// FlareNavGroup auto-expand + nesting (active child reveals the group chain)
// ------------------------------------------------------------------------------
