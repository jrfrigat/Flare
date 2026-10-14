using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsLifecycleTests : FlareTestContext
{
    private static RenderFragment Panels(bool disabled = false) => b =>
    {
        b.OpenComponent<FlareTab>(0);
        b.AddAttribute(1, "Label", "One");
        b.AddAttribute(2, "Disabled", disabled);
        b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<span>First body</span>")));
        b.CloseComponent();
        b.OpenComponent<FlareTab>(4);
        b.AddAttribute(5, "Label", "Two");
        b.AddAttribute(6, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<span>Second body</span>")));
        b.CloseComponent();
    };

    [Fact]
    public void LazyChangesWithoutChangingActiveIndex()
    {
        var content = Panels();
        var cut = Render<FlareTabs>(p => p.AddChildContent(content));
        Assert.Contains("Second body", cut.Markup);
        cut.Render(p => p.Add(x => x.Lazy, true).AddChildContent(content));
        Assert.DoesNotContain("Second body", cut.Markup);
        cut.Render(p => p.Add(x => x.Lazy, false).AddChildContent(content));
        Assert.Contains("Second body", cut.Markup);
    }

    [Fact]
    public void InitialAndDynamicallyDisabledTabNeverOwnTheTabStop()
    {
        var cut = Render<FlareTabs>(p => p.AddChildContent(Panels()));
        cut.Render(p => p.AddChildContent(Panels(true)));
        var tabs = cut.FindAll("[role=tab]");
        Assert.True(tabs[0].HasAttribute("disabled"));
        Assert.Equal("-1", tabs[0].GetAttribute("tabindex"));
        Assert.Equal("0", tabs[1].GetAttribute("tabindex"));
    }

    [Fact]
    public void PanelEscapeHatchKeepsItsInternalIdentity()
    {
        var cut = Render<FlareTabs>(p => p.AddChildContent<FlareTab>(t => t
            .Add(x => x.Label, "One")
            .Add(x => x.Class, "consumer-panel")
            .Add(x => x.Style, "overflow:auto")
            .AddUnmatched("data-testid", "consumer")
            .AddUnmatched("id", "wrong-id")));
        var panel = cut.Find("[role=tabpanel]");
        Assert.Contains("consumer-panel", panel.ClassName);
        Assert.Contains("overflow:auto", panel.GetAttribute("style"));
        Assert.Equal("consumer", panel.GetAttribute("data-testid"));
        Assert.Equal(cut.Find("[role=tab]").GetAttribute("aria-controls"), panel.Id);
    }
}
