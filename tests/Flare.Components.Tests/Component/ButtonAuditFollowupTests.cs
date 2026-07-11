using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;

namespace Flare.Components;

// ------------------------------------------------------------------------------
// FlareButton audit follow-ups: auto rel=noopener on target=_blank links,
// FocusAsync(), and LoadingTemplate.
// ------------------------------------------------------------------------------

public class C_FlareButtonAuditFollowupTests : FlareTestContext
{
    [Fact]
    public void LinkWithBlankTarget_DefaultsRelToNoopener()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Href, "https://example.com")
            .Add(x => x.Target, "_blank")
            .AddChildContent("Open"));

        Assert.Equal("noopener noreferrer", cut.Find("a").GetAttribute("rel"));
    }

    [Fact]
    public void LinkWithBlankTarget_ExplicitRel_Wins()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Href, "https://example.com")
            .Add(x => x.Target, "_blank")
            .Add(x => x.Rel, "author")
            .AddChildContent("Open"));

        Assert.Equal("author", cut.Find("a").GetAttribute("rel"));
    }

    [Fact]
    public void LinkWithoutBlankTarget_HasNoRel()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Href, "/local")
            .Add(x => x.Target, "_self")
            .AddChildContent("Go"));

        Assert.False(cut.Find("a").HasAttribute("rel"));
    }

    [Fact]
    public void LoadingTemplate_ReplacesDefaultSpinner()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true)
            .Add(x => x.LoadingTemplate, b => b.AddMarkupContent(0, "<span class=\"my-loader\">wait</span>"))
            .AddChildContent("Save"));

        Assert.Contains("my-loader", cut.Markup);
        Assert.DoesNotContain(Css.Classes.Button.Spinner, cut.Markup);
    }

    [Fact]
    public void LoadingWithoutTemplate_ShowsDefaultSpinner()
    {
        var cut = Render<FlareButton>(p => p
            .Add(x => x.Loading, true)
            .AddChildContent("Save"));

        Assert.Contains(Css.Classes.Button.Spinner, cut.Markup);
    }

    [Fact]
    public async Task FocusAsync_DoesNotThrow()
    {
        var cut = Render<FlareButton>(p => p.AddChildContent("Focus me"));

        // Loose JSInterop: the element focus round-trip completes without a configured handler.
        await cut.Instance.FocusAsync();
    }
}
