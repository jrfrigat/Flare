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
    public void KeyboardNavVertical_ArrowDownMovesToNextTab()
    {
        var cut = Render(TwoTabsVertical());

        cut.Find("[role='tablist']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[1].ClassName);
    }

    [Fact]
    public void KeyboardNavVertical_ArrowUpMovesToPreviousTab()
    {
        var cut = Render(TwoTabsVertical());

        cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].Click();

        cut.Find("[role='tablist']").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });

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
}

// ------------------------------------------------------------------------------
// FlareAccordion  (7 tests from Wave5)
// ------------------------------------------------------------------------------
