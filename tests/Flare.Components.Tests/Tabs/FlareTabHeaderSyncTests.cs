using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The tab bar is drawn by FlareTabs from its FlareTab children, and a parent render reaches FlareTabs
/// before it reaches the tabs - so what a tab puts in the bar has to be pushed back up when it changes,
/// or a count in a label shows the previous render's value.
/// </summary>
public sealed class FlareTabHeaderSyncTests : FlareTestContext
{
    private static RenderFragment OneTab(string label, string? badge = null) => b =>
    {
        b.OpenComponent<FlareTab>(0);
        b.AddAttribute(1, nameof(FlareTab.Label), label);
        b.AddAttribute(2, nameof(FlareTab.Badge), badge);
        b.CloseComponent();
    };

    [Fact]
    public void ChangedLabel_ReachesTheBarInTheSameRender()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.ChildContent, OneTab("Artifacts (1)")));

        cut.Render(p => p.Add(x => x.ChildContent, OneTab("Artifacts (2)")));

        Assert.Equal("Artifacts (2)", cut.Find($".{Css.Classes.Tabs.Label}").TextContent);
    }

    [Fact]
    public void ChangedBadge_ReachesTheBarInTheSameRender()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.ChildContent, OneTab("Inbox", "3")));

        cut.Render(p => p.Add(x => x.ChildContent, OneTab("Inbox", "4")));

        Assert.Equal("4", cut.Find($".{Css.Classes.Tabs.Badge}").TextContent);
    }

    // Each bar render hands the tab its parameters again, so a label that is different on every
    // evaluation would keep the two re-rendering each other if nothing capped it.
    [Fact(Timeout = 10000)]
    public async Task LabelThatChangesOnEveryEvaluation_DoesNotRenderForever()
    {
        var n = 0;
        RenderFragment ticking = b =>
        {
            b.OpenComponent<FlareTab>(0);
            b.AddAttribute(1, nameof(FlareTab.Label), $"tick {n++}");
            b.CloseComponent();
        };

        var cut = await Task.Run(() => Render<FlareTabs>(p => p.Add(x => x.ChildContent, ticking)),
            Xunit.TestContext.Current.CancellationToken);
        await Task.Run(() => cut.Render(p => p.Add(x => x.ChildContent, ticking)),
            Xunit.TestContext.Current.CancellationToken);

        Assert.StartsWith("tick ", cut.Find($".{Css.Classes.Tabs.Label}").TextContent);
    }

    [Fact]
    public void UnchangedHeader_DoesNotRenderTheBarAgain()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.ChildContent, OneTab("Same")));
        var before = cut.RenderCount;

        cut.Render(p => p.Add(x => x.ChildContent, OneTab("Same")));

        Assert.Equal(before + 1, cut.RenderCount);
    }
}
