using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareMenuTests : FlareTestContext
{
    // The activator slot is a template: the menu hands it the ARIA a menu button has to carry. These
    // tests only need something clickable in the wrapper, so they ignore the context they are given -
    // FlareMenuActivatorAriaTests is where the attributes themselves are checked.
    private static RenderFragment<FlareMenuActivatorContext> MenuActivator(string html) =>
        _ => b => b.AddMarkupContent(0, html);

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
            .Add(x => x.Activator, MenuActivator("<button id=\"act\">Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public void MenuPanel_HasRoleMenu()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Equal("menu", cut.Find($".{Css.Classes.Menu.Panel}").GetAttribute("role"));
    }

    [Fact]
    public void RendersMenuItems_WhenOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Item One")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Contains("Item One", cut.Markup);
    }

    [Fact]
    public void DefaultAnchor_HasBottomLeftClass()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.BottomLeft}"));
    }

    [Fact]
    public void RendersBackdrop_WhenOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
    }
}
