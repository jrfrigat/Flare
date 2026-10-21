using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTableOfContentsTests : FlareTestContext
{
    [Fact]
    public void RendersTitleAndChildLinks()
    {
        var cut = Render<FlareTableOfContents>(p => p
            .Add(x => x.Title, "Contents")
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#a").Add(x => x.ChildContent, "Alpha").Add(x => x.Active, true))
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#b").Add(x => x.ChildContent, "Bravo")));

        Assert.Equal("Contents", cut.Find($".{Css.Classes.TableOfContents.Title}").TextContent);
        var links = cut.FindAll($".{Css.Classes.TableOfContents.Link}");
        Assert.Equal(2, links.Count);
        Assert.Equal("#a", links[0].GetAttribute("href"));
        Assert.Contains(Css.Classes.TableOfContents.LinkActive, links[0].ClassName);
        Assert.Equal("true", links[0].GetAttribute("aria-current"));
        Assert.DoesNotContain(Css.Classes.TableOfContents.LinkActive, links[1].ClassName);
    }

    [Fact]
    public void NoTitle_RendersListOnly()
    {
        var cut = Render<FlareTableOfContents>(p => p
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#a").Add(x => x.ChildContent, "Alpha")));
        Assert.Empty(cut.FindAll($".{Css.Classes.TableOfContents.Title}"));
        Assert.Single(cut.FindAll($".{Css.Classes.TableOfContents.Link}"));
    }

    [Fact]
    public void TocLink_Level_SetsDepthVariable()
    {
        var cut = Render<FlareTableOfContents>(p => p
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#a").Add(x => x.Level, 2).Add(x => x.ChildContent, "Deep")));
        var li = cut.Find($".{Css.Classes.TableOfContents.Item}");
        Assert.Contains($"{Css.Tokens.LocalVars.TocDepth}:2", li.GetAttribute("style"));
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("  javascript:alert(1)")]
    [InlineData("vbscript:msgbox(1)")]
    public void TocLink_RefusesAScriptScheme(string href)
    {
        var cut = Render<FlareTocLink>(p => p
            .Add(x => x.Href, href)
            .Add(x => x.ChildContent, "Unsafe"));

        Assert.Equal("about:blank", cut.Find("a").GetAttribute("href"));
    }

    [Theory]
    [InlineData("#section")]
    [InlineData("/components/pagination")]
    [InlineData("https://example.com/page")]
    [InlineData("mailto:a@example.com")]
    public void TocLink_KeepsAnAllowedHref(string href)
    {
        var cut = Render<FlareTocLink>(p => p
            .Add(x => x.Href, href)
            .Add(x => x.ChildContent, "Safe"));

        Assert.Equal(href, cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void TocLink_WithoutHref_OmitsTheAttribute()
    {
        var cut = Render<FlareTocLink>(p => p.Add(x => x.ChildContent, "Plain"));

        Assert.False(cut.Find("a").HasAttribute("href"));
    }
}
