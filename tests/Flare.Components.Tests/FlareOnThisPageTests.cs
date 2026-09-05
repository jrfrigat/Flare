using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareOnThisPageTests : FlareTestContext
{
    private static TocHeading[] TwoHeadings() =>
    [
        new TocHeading { Id = "intro", Text = "Intro", Level = 2 },
        new TocHeading { Id = "details", Text = "Details", Level = 3 },
    ];

    [Fact]
    public void Empty_RendersNothing_ByDefault()
    {
        var cut = Render<FlareOnThisPage>();
        Assert.Empty(cut.FindAll($".{Css.Classes.TableOfContents.Root}"));
    }

    [Fact]
    public void ShowWhenEmpty_RendersContainer()
    {
        var cut = Render<FlareOnThisPage>(p => p.Add(x => x.ShowWhenEmpty, true));
        Assert.Single(cut.FindAll($".{Css.Classes.TableOfContents.Root}"));
    }

    [Fact]
    public void SetHeadings_RendersLinksWithHrefs()
    {
        var cut = Render<FlareOnThisPage>();
        cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));

        var links = cut.FindAll($".{Css.Classes.TableOfContents.Link}");
        Assert.Equal(2, links.Count);
        // href is anchored to the current page path, ending in the fragment.
        Assert.EndsWith("#intro", links[0].GetAttribute("href"));
        Assert.Contains("Intro", links[0].TextContent);
        Assert.EndsWith("#details", links[1].GetAttribute("href"));
    }

    [Fact]
    public void SetActive_MarksAllVisibleLinksActive()
    {
        var cut = Render<FlareOnThisPage>();
        cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));
        cut.InvokeAsync(() => cut.Instance.SetActive(["intro", "details"]));

        // Both visible headings are marked active simultaneously.
        var active = cut.FindAll($".{Css.Classes.TableOfContents.LinkActive}");
        Assert.Equal(2, active.Count);
        Assert.All(active, a => Assert.Equal("true", a.GetAttribute("aria-current")));
    }

    [Fact]
    public void SetActive_OnlyMarksVisibleSubset()
    {
        var cut = Render<FlareOnThisPage>();
        cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));
        cut.InvokeAsync(() => cut.Instance.SetActive(["details"]));

        var active = cut.Find($".{Css.Classes.TableOfContents.LinkActive}");
        Assert.EndsWith("#details", active.GetAttribute("href"));
        Assert.Single(cut.FindAll($".{Css.Classes.TableOfContents.LinkActive}"));
    }

    [Fact]
    public void DefaultTitle_IsLocalized()
    {
        var cut = Render<FlareOnThisPage>(p => p.Add(x => x.ShowWhenEmpty, true));
        Assert.Equal("On this page", cut.Find($".{Css.Classes.TableOfContents.Title}").TextContent);
    }

    [Fact]
    public void CustomTitle_Overrides()
    {
        var cut = Render<FlareOnThisPage>(p => p
            .Add(x => x.ShowWhenEmpty, true)
            .Add(x => x.Title, "Contents"));
        Assert.Equal("Contents", cut.Find($".{Css.Classes.TableOfContents.Title}").TextContent);
    }

    [Fact]
    public void ActiveIdsChanged_FiresOnSetActive()
    {
        IReadOnlyList<string>? fired = null;
        var cut = Render<FlareOnThisPage>(p => p
            .Add(x => x.ActiveIdsChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, ids => fired = ids)));
        cut.InvokeAsync(() => cut.Instance.SetActive(["intro", "details"]));
        Assert.Equal(["intro", "details"], fired);
    }
}

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
}

public class FlareTextAnchorTests : FlareTestContext
{
    [Fact]
    public void AnchorId_SetsIdAndAnchorLink()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.HeadlineSmall)
            .Add(x => x.AnchorId, "getting-started")
            .AddChildContent("Getting started"));

        var heading = cut.Find("h4");
        Assert.Equal("getting-started", heading.GetAttribute("id"));

        var anchor = cut.Find($"a.{Css.Classes.Text.Anchor}");
        Assert.EndsWith("#getting-started", anchor.GetAttribute("href"));
        Assert.Equal("true", anchor.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void NoAnchorId_RendersNoIdOrAnchor()
    {
        var cut = Render<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.HeadlineSmall)
            .AddChildContent("Plain"));

        Assert.False(cut.Find("h4").HasAttribute("id"));
        Assert.Empty(cut.FindAll($"a.{Css.Classes.Text.Anchor}"));
    }
}
