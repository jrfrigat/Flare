using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsCollapsibleTests : FlareTestContext
{
    private static RenderFragment Tabs() => b =>
    {
        foreach (var (label, seq) in new[] { ("First", 0), ("Second", 2), ("Third", 4) })
        {
            b.OpenComponent<FlareTab>(seq);
            b.AddAttribute(seq + 1, "Label", label);
            b.CloseComponent();
        }
    };

    [Fact]
    public void Collapsible_RendersTheOverflowControlBesideTheTablist()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.Collapsible, true)
            .Add(x => x.ChildContent, Tabs()));

        Assert.Single(cut.FindAll($".{Css.Classes.Tabs.More}"));
        // A tablist holds only tabs: the overflow control is a sibling of it, never a child.
        Assert.Empty(cut.FindAll($"[role=tablist] .{Css.Classes.Tabs.More}"));
    }

    [Fact]
    public void WithoutCollapsible_ThereIsNoOverflowControl()
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.ChildContent, Tabs()));

        Assert.Empty(cut.FindAll($".{Css.Classes.Tabs.More}"));
    }

    [Fact]
    public void OverflowLabel_NamesTheOverflowButton()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.Collapsible, true)
            .Add(x => x.OverflowLabel, "More tabs")
            .Add(x => x.ChildContent, Tabs()));

        var button = cut.Find($".{Css.Classes.Tabs.More} button");
        Assert.Equal("More tabs", button.GetAttribute("aria-label"));
    }
}
