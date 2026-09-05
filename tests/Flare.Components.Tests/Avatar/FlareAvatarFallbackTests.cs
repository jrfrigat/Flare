using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// ------------------------------------------------------------------------------
// FlareAvatar FallbackIcon / FallbackContent
// ------------------------------------------------------------------------------
public class FlareAvatarFallbackTests : FlareTestContext
{
    [Fact]
    public void NoImageNoText_DefaultsToPersonIcon()
    {
        var cut = Render<FlareAvatar>();

        // The default fallback is now the built-in person SVG (no Material Symbols font dependency).
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Avatar.Icon} path"));
    }

    [Fact]
    public void FallbackIcon_OverridesDefault()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.FallbackIcon, FlareIcons.Group));

        // "group" is built in, overriding the default person icon with inline SVG.
        Assert.Equal(FlareIcons.Group.Data, cut.Find($".{Css.Classes.Avatar.Icon} path").GetAttribute("d"));
    }

    [Fact]
    public void FallbackContent_ReplacesIcon()
    {
        var cut = Render<FlareAvatar>(p => p
            .Add(x => x.FallbackContent, b => b.AddMarkupContent(0, "<span class=\"custom-fb\">x</span>")));

        Assert.NotEmpty(cut.FindAll(".custom-fb"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Avatar.Icon}"));
    }
}
