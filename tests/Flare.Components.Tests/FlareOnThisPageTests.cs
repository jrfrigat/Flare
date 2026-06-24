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
        var cut = RenderComponent<FlareOnThisPage>();
        Assert.Empty(cut.FindAll(".flare-toc"));
    }

    [Fact]
    public void ShowWhenEmpty_RendersContainer()
    {
        var cut = RenderComponent<FlareOnThisPage>(p => p.Add(x => x.ShowWhenEmpty, true));
        Assert.Single(cut.FindAll(".flare-toc"));
    }

    [Fact]
    public void SetHeadings_RendersLinksWithHrefs()
    {
        var cut = RenderComponent<FlareOnThisPage>();
        cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));

        var links = cut.FindAll(".flare-toc__link");
        Assert.Equal(2, links.Count);
        // href is anchored to the current page path, ending in the fragment.
        Assert.EndsWith("#intro", links[0].GetAttribute("href"));
        Assert.Contains("Intro", links[0].TextContent);
        Assert.EndsWith("#details", links[1].GetAttribute("href"));
    }

    [Fact]
    public void SetActive_MarksAllVisibleLinksActive()
    {
        var cut = RenderComponent<FlareOnThisPage>();
        cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));
        cut.InvokeAsync(() => cut.Instance.SetActive(["intro", "details"]));

        // Both visible headings are marked active simultaneously.
        var active = cut.FindAll(".flare-toc__link--active");
        Assert.Equal(2, active.Count);
        Assert.All(active, a => Assert.Equal("true", a.GetAttribute("aria-current")));
    }

    [Fact]
    public void SetActive_OnlyMarksVisibleSubset()
    {
        var cut = RenderComponent<FlareOnThisPage>();
        cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));
        cut.InvokeAsync(() => cut.Instance.SetActive(["details"]));

        var active = cut.Find(".flare-toc__link--active");
        Assert.EndsWith("#details", active.GetAttribute("href"));
        Assert.Single(cut.FindAll(".flare-toc__link--active"));
    }

    [Fact]
    public void DefaultTitle_IsLocalized()
    {
        var cut = RenderComponent<FlareOnThisPage>(p => p.Add(x => x.ShowWhenEmpty, true));
        Assert.Equal("On this page", cut.Find(".flare-toc__title").TextContent);
    }

    [Fact]
    public void CustomTitle_Overrides()
    {
        var cut = RenderComponent<FlareOnThisPage>(p => p
            .Add(x => x.ShowWhenEmpty, true)
            .Add(x => x.Title, "Contents"));
        Assert.Equal("Contents", cut.Find(".flare-toc__title").TextContent);
    }

    [Fact]
    public void ActiveIdsChanged_FiresOnSetActive()
    {
        IReadOnlyList<string>? fired = null;
        var cut = RenderComponent<FlareOnThisPage>(p => p
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
        var cut = RenderComponent<FlareTableOfContents>(p => p
            .Add(x => x.Title, "Contents")
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#a").Add(x => x.ChildContent, "Alpha").Add(x => x.Active, true))
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#b").Add(x => x.ChildContent, "Bravo")));

        Assert.Equal("Contents", cut.Find(".flare-toc__title").TextContent);
        var links = cut.FindAll(".flare-toc__link");
        Assert.Equal(2, links.Count);
        Assert.Equal("#a", links[0].GetAttribute("href"));
        Assert.Contains("flare-toc__link--active", links[0].ClassName);
        Assert.Equal("true", links[0].GetAttribute("aria-current"));
        Assert.DoesNotContain("flare-toc__link--active", links[1].ClassName);
    }

    [Fact]
    public void NoTitle_RendersListOnly()
    {
        var cut = RenderComponent<FlareTableOfContents>(p => p
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#a").Add(x => x.ChildContent, "Alpha")));
        Assert.Empty(cut.FindAll(".flare-toc__title"));
        Assert.Single(cut.FindAll(".flare-toc__link"));
    }

    [Fact]
    public void TocLink_Level_SetsDepthVariable()
    {
        var cut = RenderComponent<FlareTableOfContents>(p => p
            .AddChildContent<FlareTocLink>(l => l.Add(x => x.Href, "#a").Add(x => x.Level, 2).Add(x => x.ChildContent, "Deep")));
        var li = cut.Find(".flare-toc__item");
        Assert.Contains("--flare-toc-depth:2", li.GetAttribute("style"));
    }
}

public class FlareTextAnchorTests : FlareTestContext
{
    [Fact]
    public void AnchorId_SetsIdAndAnchorLink()
    {
        var cut = RenderComponent<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.HeadlineSmall)
            .Add(x => x.AnchorId, "getting-started")
            .AddChildContent("Getting started"));

        var heading = cut.Find("h4");
        Assert.Equal("getting-started", heading.GetAttribute("id"));

        var anchor = cut.Find("a.flare-text__anchor");
        Assert.EndsWith("#getting-started", anchor.GetAttribute("href"));
        Assert.Equal("true", anchor.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void NoAnchorId_RendersNoIdOrAnchor()
    {
        var cut = RenderComponent<FlareText>(p => p
            .Add(x => x.Typo, TypographyScale.HeadlineSmall)
            .AddChildContent("Plain"));

        Assert.False(cut.Find("h4").HasAttribute("id"));
        Assert.Empty(cut.FindAll("a.flare-text__anchor"));
    }
}
