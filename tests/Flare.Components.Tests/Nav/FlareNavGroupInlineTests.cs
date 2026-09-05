using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareNavGroup renders an inline accordion: a header button that toggles a child-items region.
// It stays an inline accordion regardless of the layout's collapsed/mini-rail state.
public class FlareNavGroupInlineTests : FlareTestContext
{
    [Fact]
    public void RendersInlineGroup_HeaderTogglesItems()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(g => g.Label, "Components")
            .Add(g => g.Icon, FlareIcons.Folder)
            .AddChildContent($"<a class=\"{Css.Classes.Navigation.NavLink}\">Buttons</a>"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Navigation.GroupItems}"));
        var header = cut.Find($"button.{Css.Classes.Navigation.NavGroupHeader}");
        Assert.Equal("false", header.GetAttribute("aria-expanded"));
        Assert.Contains("Buttons", cut.Find($".{Css.Classes.Navigation.GroupItems}").TextContent);

        header.Click();
        Assert.Equal("true", cut.Find($"button.{Css.Classes.Navigation.NavGroupHeader}").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void RendersInlineGroup_NoFlyoutMarkup()
    {
        // The group is always an inline accordion: no flyout panel. (The two-pane secondary column
        // replaced the old per-group flyout; FlareNavGroup no longer reads the layout context.)
        var cut = Render<FlareNavGroup>(p => p
            .Add(g => g.Label, "Components")
            .AddChildContent("<a>x</a>"));

        Assert.Empty(cut.FindAll(".flare-nav-group--flyout"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Navigation.GroupItems}"));
    }
}
