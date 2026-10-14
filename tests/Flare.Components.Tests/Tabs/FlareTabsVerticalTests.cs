using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareTabsVerticalTests : FlareTestContext
{
    private static RenderFragment TwoTabsVertical() => b =>
    {
        b.OpenComponent<FlareTabs>(0);
        b.AddAttribute(1, "Placement", TabsPlacement.Left);
        b.AddAttribute(2, "ChildContent", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareTab>(3);
            inner.AddAttribute(4, "Label", "Settings");
            inner.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(6, "<p>Settings Content</p>")));
            inner.CloseComponent();

            inner.OpenComponent<FlareTab>(7);
            inner.AddAttribute(8, "Label", "Profile");
            inner.AddAttribute(9, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(10, "<p>Profile Content</p>")));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void RendersVerticalOrientation()
    {
        var cut = Render(TwoTabsVertical());

        Assert.Contains(Css.Classes.Tabs.Vertical, cut.Find($".{Css.Classes.Tabs.Root}").ClassName);
    }

    [Fact]
    public void RendersHorizontalOrientation_Default()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        Assert.DoesNotContain(Css.Classes.Tabs.Vertical, cut.Find($".{Css.Classes.Tabs.Root}").ClassName);
    }

    [Fact]
    public async Task KeyboardNavVertical_ArrowDownMovesToNextTab()
    {
        var cut = Render(TwoTabsVertical());

        await cut.InvokeAsync(() => cut.FindComponent<FlareTabs>().Instance.OnTabNavigation("ArrowDown", false));

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[1].ClassName);
    }

    [Fact]
    public async Task KeyboardNavVertical_ArrowUpMovesToPreviousTab()
    {
        var cut = Render(TwoTabsVertical());

        cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].Click();

        await cut.InvokeAsync(() => cut.FindComponent<FlareTabs>().Instance.OnTabNavigation("ArrowUp", false));

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[0].ClassName);
    }

    [Fact]
    public void RendersVerticalActiveTab_HasActiveClass()
    {
        var cut = Render(TwoTabsVertical());

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[0].ClassName);
    }

    [Theory]
    [InlineData(TabsPlacement.Top, false, "ArrowRight", 2)]
    [InlineData(TabsPlacement.Top, true, "ArrowLeft", 2)]
    [InlineData(TabsPlacement.Top, false, "ArrowDown", 0)]
    [InlineData(TabsPlacement.Left, true, "ArrowDown", 2)]
    [InlineData(TabsPlacement.Left, false, "ArrowRight", 0)]
    [InlineData(TabsPlacement.Top, false, "End", 2)]
    public async Task NavigationHonorsOrientationDirectionAndDisabledTabs(TabsPlacement placement, bool rtl, string key, int expected)
    {
        var cut = Render<FlareTabs>(p => p.Add(x => x.Placement, placement).AddChildContent(b =>
        {
            for (var i = 0; i < 3; i++)
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", $"Tab {i}");
                b.AddAttribute(2, "Disabled", i == 1);
                b.CloseComponent();
            }
        }));
        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation(key, rtl));
        var tabs = cut.FindAll("[role=tab]");
        Assert.Equal("0", tabs[expected].GetAttribute("tabindex"));
        Assert.Single(tabs, t => t.GetAttribute("tabindex") == "0");
    }

    [Fact]
    public async Task CancelledKeyboardActivationKeepsSelection()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.OnPreviewInteraction, args => args.Cancel = true)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0); b.AddAttribute(1, "Label", "One"); b.CloseComponent();
                b.OpenComponent<FlareTab>(2); b.AddAttribute(3, "Label", "Two"); b.CloseComponent();
            }));
        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("ArrowRight", false));
        Assert.Equal("0", cut.FindAll("[role=tab]")[0].GetAttribute("tabindex"));
    }

    [Fact]
    public void RendersVerticalTabContent_FirstTabByDefault()
    {
        var cut = Render(TwoTabsVertical());

        // All panels always rendered (no state loss); inactive panel has hidden class
        Assert.Contains("Settings Content", cut.Markup);
        var panels = cut.FindAll($".{Css.Classes.Tabs.Panel}");
        Assert.Equal(2, panels.Count);
        Assert.DoesNotContain(Css.Classes.Tabs.PanelHidden, panels[0].ClassName);
        Assert.Contains(Css.Classes.Tabs.PanelHidden, panels[1].ClassName);
    }

    [Fact]
    public async Task CloseButtonsAreSiblingsAndDeleteUsesThePreviewGuard()
    {
        var closed = new List<string>();
        var cancel = true;
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.OnTabClose, label => closed.Add(label))
            .Add(x => x.OnPreviewInteraction, args => args.Cancel = cancel)
            .AddChildContent<FlareTab>(p => p.Add(x => x.Label, "Document").Add(x => x.Closeable, true)));
        Assert.Empty(cut.FindAll("button button"));
        Assert.Equal("-1", cut.Find($".{Css.Classes.Tabs.TabClose}").GetAttribute("tabindex"));
        Assert.Equal("Document", cut.Find("[role=tab]").GetAttribute("aria-label"));
        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("Delete", false));
        Assert.Empty(closed);
        cancel = false;
        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("Delete", false));
        Assert.Equal(["Document"], closed);
    }
}
