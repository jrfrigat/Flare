using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsCollapsibleTests : FlareTestContext
{
    private static RenderFragment Tabs(EventCallback secondClick = default) => b =>
    {
        foreach (var (label, seq) in new[] { ("First", 0), ("Second", 3), ("Third", 6) })
        {
            b.OpenComponent<FlareTab>(seq);
            b.AddAttribute(seq + 1, "Label", label);
            if (label == "Second") b.AddAttribute(seq + 2, nameof(FlareTab.OnClick), secondClick);
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

    [Fact]
    public async Task ChoosingAFoldedTabRunsItsClickBeforeActivation()
    {
        var calls = new List<string>();
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.Collapsible, true)
            .Add(x => x.ChildContent, Tabs(EventCallback.Factory.Create(this, () => calls.Add("click"))))
            .Add(x => x.OnPreviewInteraction, _ => calls.Add("preview"))
            .Add(x => x.ActiveIndexChanged, _ => calls.Add("active")));

        await cut.InvokeAsync(() => cut.Instance.OnTabsOverflowChanged(0, 1));
        await cut.Find($".{Css.Classes.Tabs.More} button").ClickAsync(new());
        calls.Clear();
        await cut.Find($".{Css.Classes.Tabs.OverflowTab}").ClickAsync(new());

        Assert.Equal(["click", "preview", "active"], calls);
        Assert.Equal("Second", cut.Find("[role=tab][aria-selected=true]").GetAttribute("aria-label"));
    }
}
