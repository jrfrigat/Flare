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
    public void ShowWhenEmpty_ForwardsTheBoxParametersToTheRenderedRoot()
    {
        var cut = Render<FlareOnThisPage>(p => p
            .Add(x => x.ShowWhenEmpty, true)
            .Add(x => x.Sticky, true)
            .Add(x => x.StickyTop, "1rem")
            .Add(x => x.Width, "14rem")
            .Add(x => x.MaxHeight, "calc(100vh - 6rem)")
            .Add(x => x.Overflow, "auto"));

        var style = cut.Find($".{Css.Classes.TableOfContents.Root}").GetAttribute("style") ?? string.Empty;
        Assert.Contains("position:sticky", style);
        Assert.Contains("top:1rem", style);
        Assert.Contains("width:14rem", style);
        Assert.Contains("max-height:calc(100vh - 6rem)", style);
        Assert.Contains("overflow:auto", style);
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

    [Fact]
    public void AdditionalAttributes_ReachTheNav()
    {
        var cut = Render<FlareOnThisPage>(p => p
            .Add(x => x.ShowWhenEmpty, true)
            .Add(x => x.AdditionalAttributes, new Dictionary<string, object>
            {
                ["id"] = "toc",
                ["aria-labelledby"] = "heading",
                ["data-test"] = "value",
            }));

        var nav = cut.Find("nav");
        Assert.Equal("toc", nav.GetAttribute("id"));
        Assert.Equal("heading", nav.GetAttribute("aria-labelledby"));
        Assert.Equal("value", nav.GetAttribute("data-test"));
    }

    private int InitCalls() => JSInterop.Invocations.Count(i => i.Identifier == "FlareToc.init");

    [Fact]
    public void FirstRender_RegistersOnceWithParameters()
    {
        Render<FlareOnThisPage>(p => p
            .Add(x => x.Root, "#content")
            .Add(x => x.HeadingSelector, "h2")
            .Add(x => x.ScrollRoot, "#panel"));

        Assert.Equal(1, InitCalls());
        var call = JSInterop.Invocations.Last(i => i.Identifier == "FlareToc.init");
        Assert.Equal("#content", call.Arguments[2]);
        Assert.Equal("h2", call.Arguments[3]);
        Assert.Equal("#panel", call.Arguments[4]);
    }

    [Fact]
    public async Task RefreshAsync_RescansWithCurrentParameters()
    {
        var cut = Render<FlareOnThisPage>(p => p.Add(x => x.Root, "#content"));

        await cut.InvokeAsync(() => cut.Instance.RefreshAsync());

        var calls = JSInterop.Invocations.Where(i => i.Identifier == "FlareToc.init").ToList();
        Assert.Equal(2, calls.Count);
        Assert.Equal("#content", calls[1].Arguments[2]);
    }

    [Fact]
    public void ChangedParameters_ReRegister()
    {
        var cut = Render<FlareOnThisPage>(p => p.Add(x => x.HeadingSelector, "h2"));

        cut.Render(p => p
            .Add(x => x.Root, "#content")
            .Add(x => x.HeadingSelector, "h2, h3"));

        var calls = JSInterop.Invocations.Where(i => i.Identifier == "FlareToc.init").ToList();
        Assert.Equal(2, calls.Count);
        Assert.Equal("#content", calls[1].Arguments[2]);
        Assert.Equal("h2, h3", calls[1].Arguments[3]);
    }

    [Fact]
    public void UnchangedParameters_OnRerender_DoNotReRegister()
    {
        var cut = Render<FlareOnThisPage>(p => p.Add(x => x.Root, "#content"));

        cut.Render(p => p.Add(x => x.Title, "Contents"));

        Assert.Equal(1, InitCalls());
    }

    [Fact]
    public async Task Dispose_RemovesTheRegistration()
    {
        var cut = Render<FlareOnThisPage>();

        await cut.InvokeAsync(() => cut.Instance.DisposeAsync().AsTask());

        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "FlareToc.dispose");
    }

    [Fact]
    public async Task SetHeadings_UnchangedSet_DoesNotRerender()
    {
        var cut = Render<FlareOnThisPage>(p => p.Add(x => x.ShowWhenEmpty, true));
        await cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));
        var renders = cut.RenderCount;

        await cut.InvokeAsync(() => cut.Instance.SetHeadings(TwoHeadings()));

        Assert.Equal(renders, cut.RenderCount);
    }
}
