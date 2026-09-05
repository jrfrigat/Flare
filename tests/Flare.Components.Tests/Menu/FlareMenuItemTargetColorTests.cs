using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareMenuItem Target + IconColor
// ------------------------------------------------------------------------------
public class FlareMenuItemTargetColorTests : FlareTestContext
{
    private static RenderFragment Activator => b => b.AddMarkupContent(0, "<button>Open</button>");

    [Fact]
    public void Target_Blank_AddsTargetAndRel()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, Activator)
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Href, "https://example.com")
                .Add(x => x.Target, "_blank")
                .AddChildContent("External")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var anchor = cut.Find($"a.{Css.Classes.Menu.Item}");
        Assert.Equal("_blank", anchor.GetAttribute("target"));
        Assert.Equal("noopener noreferrer", anchor.GetAttribute("rel"));
    }

    [Fact]
    public void IconColor_AddsColorClassOnIcon()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, Activator)
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, FlareIcons.Edit)
                .Add(x => x.IconColor, FlareColor.Primary)
                .AddChildContent("Edit")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Contains(Css.Classes.Color.Primary, cut.Find($".{Css.Classes.Menu.ItemIcon}").ClassName);
    }

    [Fact]
    public void LeadingIconColor_OverridesIconColor()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(x => x.Activator, Activator)
            .AddChildContent<FlareMenuItem>(mi => mi
                .Add(x => x.Icon, FlareIcons.Edit)
                .Add(x => x.IconColor, FlareColor.Primary)
                .Add(x => x.LeadingIconColor, FlareColor.Error)
                .AddChildContent("Edit")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        var cls = cut.Find($".{Css.Classes.Menu.ItemIcon}").ClassName;
        Assert.Contains(Css.Classes.Color.Error, cls);
        Assert.DoesNotContain(Css.Classes.Color.Primary, cls);
    }
}
