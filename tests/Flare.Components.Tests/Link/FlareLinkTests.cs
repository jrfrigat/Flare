using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareLinkTests : FlareTestContext
{
    [Fact]
    public void WithHref_RendersAnchor()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "/docs")
            .AddChildContent("Docs"));
        var a = cut.Find($"a.{Css.Classes.Link.Root}");
        Assert.Equal("/docs", a.GetAttribute("href"));
        Assert.Contains("Docs", a.TextContent);
    }

    [Fact]
    public void Disabled_AddsDisabledClass()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "/x")
            .Add(x => x.Disabled, true));
        Assert.Contains(Css.Classes.Link.Disabled, cut.Find($".{Css.Classes.Link.Root}").ClassName);
    }
}
