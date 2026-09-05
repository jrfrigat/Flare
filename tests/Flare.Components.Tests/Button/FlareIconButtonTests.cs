using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareIconButton  (icon-only wrapper over FlareButton)
// ------------------------------------------------------------------------------
public class FlareIconButtonTests : FlareTestContext
{
    [Fact]
    public void Icon_RendersIconOnlyButton()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.Settings)
            .Add(x => x.AriaLabel, "Settings"));

        var btn = cut.Find($"button.{Css.Classes.Button.Root}");
        Assert.Contains(Css.Classes.Button.IconOnly, btn.ClassName);
        Assert.Equal("Settings", btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void DefaultVariant_IsText()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.Add)
            .Add(x => x.AriaLabel, "Add"));

        Assert.Contains(Css.Classes.Button.Text, cut.Find($"button.{Css.Classes.Button.Root}").ClassName);
    }

    [Fact]
    public void Href_RendersAnchor()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.OpenInNew)
            .Add(x => x.Href, "https://example.com")
            .Add(x => x.AriaLabel, "Open"));

        Assert.NotEmpty(cut.FindAll($"a.{Css.Classes.Button.Root}"));
    }
}
