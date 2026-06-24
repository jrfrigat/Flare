// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareMenuItem  (6 tests)
// -----------------------------------------------------------------------------

public class FlareMenuItemTests : FlareTestContext
{
    [Fact]
    public void RendersButton()
    {
        var cut = RenderComponent<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Save")));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll("button.flare-menu-item"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = RenderComponent<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Delete")));

        cut.Find(".flare-menu__activator").Click();

        Assert.Contains("Delete", cut.Markup);
    }

    [Fact]
    public void DisabledMenuItem_HasDisabledAttribute()
    {
        var cut = RenderComponent<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Disabled, true)
                .AddChildContent("Disabled Action")));

        cut.Find(".flare-menu__activator").Click();

        var btn = cut.Find("button.flare-menu-item");
        Assert.True(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void DisabledMenuItem_HasDisabledClass()
    {
        var cut = RenderComponent<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Disabled, true)
                .AddChildContent("Disabled Action")));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll(".flare-menu-item--disabled"));
    }

    [Fact]
    public void MenuItemHasRoleMenuitem()
    {
        var cut = RenderComponent<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Item")));

        cut.Find(".flare-menu__activator").Click();

        Assert.Equal("menuitem", cut.Find("button.flare-menu-item").GetAttribute("role"));
    }

    [Fact]
    public void ItemWithIcon_RendersIconSpan()
    {
        var cut = RenderComponent<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, "settings")
                .AddChildContent("Settings")));

        cut.Find(".flare-menu__activator").Click();

        Assert.NotEmpty(cut.FindAll(".flare-menu-item__icon"));
    }
}
