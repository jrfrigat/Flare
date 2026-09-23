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

    [Fact]
    public void UnchangedHeader_DoesNotRenderTheBarAgain()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.ChildContent, OneTab("Same")));
        var before = cut.RenderCount;

        cut.Render(p => p.Add(x => x.ChildContent, OneTab("Same")));

        Assert.Equal(before + 1, cut.RenderCount);
    }
}
