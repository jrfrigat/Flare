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
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Save")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($"button.{Css.Classes.Menu.Item}"));
    }

    [Fact]
    public void RendersLabel()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Delete")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Contains("Delete", cut.Markup);
    }

    [Fact]
    public void DisabledMenuItem_HasDisabledAttribute()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
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
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
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
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi =>
                mi.AddChildContent("Item")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Equal("menuitem", cut.Find($"button.{Css.Classes.Menu.Item}").GetAttribute("role"));
    }

    [Fact]
    public void ItemWithIcon_RendersIconSpan()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, b =>
                b.AddMarkupContent(0, "<button>Open</button>"))
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, FlareIcons.Settings)
                .AddChildContent("Settings")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.ItemIcon}"));
    }
}
