using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareMenuItemTests : FlareTestContext
{
    // These tests are about the items, not about the button that opens them: the activator context is
    // taken and dropped.
    private static RenderFragment<FlareMenuActivatorContext> MenuActivator(string html) =>
        _ => b => b.AddMarkupContent(0, html);

    [Fact]
    public void RendersButton()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Save")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Menu.Item}"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Delete")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Contains("Delete", cut.Markup);
    }

    [Fact]
    public void DisabledMenuItem_HasDisabledAttribute()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Disabled, true)
                .AddChildContent("Disabled Action")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var btn = cut.Find($"button.{Css.Classes.Menu.Item}");
        Assert.True(btn.HasAttribute("disabled"));
    }

    [Fact]
    public void DisabledMenuItem_HasDisabledClass()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Disabled, true)
                .AddChildContent("Disabled Action")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.ItemDisabled}"));
    }

    [Fact]
    public void MenuItemHasRoleMenuitem()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Item")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Equal("menuitem", cut.Find($"button.{Css.Classes.Menu.Item}").GetAttribute("role"));
    }

    [Fact]
    public void ItemWithIcon_RendersIconSpan()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, FlareIcons.Settings)
                .AddChildContent("Settings")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.ItemIcon}"));
    }

    [Fact]
    public void Shortcut_RendersTrailingHintAndDescribesTheItem()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Shortcut, "Ctrl+S")
                .AddChildContent("Save")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var shortcut = cut.Find($".{Css.Classes.Menu.ItemShortcut}");
        Assert.Equal("Ctrl+S", shortcut.TextContent);
        // Hidden from the accessible name; referenced as the description instead.
        Assert.Equal("true", shortcut.GetAttribute("aria-hidden"));
        Assert.Equal(shortcut.Id, cut.Find($"button.{Css.Classes.Menu.Item}").GetAttribute("aria-describedby"));
    }

    [Fact]
    public void CheckedTrue_RendersMenuitemcheckboxWithCheckMark()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Checked, true)
                .AddChildContent("Word wrap")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var item = cut.Find($"button.{Css.Classes.Menu.Item}");
        Assert.Equal("menuitemcheckbox", item.GetAttribute("role"));
        Assert.Equal("true", item.GetAttribute("aria-checked"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.ItemCheck}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.ItemCheckIcon}"));
    }

    [Fact]
    public void CheckedFalse_KeepsTheCheckSlotButDrawsNoMark()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Checked, false)
                .AddChildContent("Word wrap")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var item = cut.Find($"button.{Css.Classes.Menu.Item}");
        Assert.Equal("menuitemcheckbox", item.GetAttribute("role"));
        Assert.Equal("false", item.GetAttribute("aria-checked"));
        // The slot stays reserved, which is what keeps checked and unchecked labels aligned.
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.ItemCheck}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.ItemCheckIcon}"));
    }

    [Fact]
    public void PlainItem_KeepsMenuitemRoleWithoutCheckSlot()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi.AddChildContent("Save")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var item = cut.Find($"button.{Css.Classes.Menu.Item}");
        Assert.Equal("menuitem", item.GetAttribute("role"));
        Assert.Null(item.GetAttribute("aria-checked"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.ItemCheck}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.ItemShortcut}"));
    }

    [Fact]
    public void CheckedItem_StillRaisesClickAndClosesTheMenu()
    {
        var clicked = false;
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Checked, true)
                .Add(x => x.OnClick, () => clicked = true)
                .AddChildContent("Word wrap")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        cut.Find($"button.{Css.Classes.Menu.Item}").Click();

        Assert.True(clicked);
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public void AnchorItem_KeepsHrefWhileReportingTheCheckboxState()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, MenuActivator("<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Href, "/docs")
                .Add(x => x.Checked, false)
                .AddChildContent("Documentation")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var item = cut.Find($"a.{Css.Classes.Menu.Item}");
        Assert.Equal("/docs", item.GetAttribute("href"));
        Assert.Equal("menuitemcheckbox", item.GetAttribute("role"));
        Assert.Equal("false", item.GetAttribute("aria-checked"));
    }
}
