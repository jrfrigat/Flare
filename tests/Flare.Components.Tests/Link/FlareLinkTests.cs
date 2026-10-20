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
        Assert.Equal("link", cut.Find($".{Css.Classes.Link.Root}").GetAttribute("role"));
        Assert.Equal("true", cut.Find($".{Css.Classes.Link.Root}").GetAttribute("aria-disabled"));
    }

    [Fact]
    public void NoHref_RendersPlainTextWithoutLinkSemantics()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Title, "No destination")
            .AddChildContent("Label"));

        var span = cut.Find($".{Css.Classes.Link.Root}");
        Assert.Equal("SPAN", span.TagName);
        Assert.Null(span.GetAttribute("role"));
        Assert.Null(span.GetAttribute("aria-disabled"));
        Assert.Equal("No destination", span.GetAttribute("title"));
        Assert.Empty(cut.FindAll("a"));
    }

    [Fact]
    public void AnUnsafeHref_IsNotRenderedAsAWorkingLink()
    {
        var cut = Render<FlareLink>(p => p
            .Add(x => x.Href, "javascript:alert(1)")
            .AddChildContent("Label"));

        Assert.Empty(cut.FindAll("a"));
        Assert.Null(cut.Find($".{Css.Classes.Link.Root}").GetAttribute("role"));
    }
}
