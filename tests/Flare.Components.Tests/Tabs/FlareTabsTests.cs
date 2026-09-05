using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTabsTests : FlareTestContext
{
    private static RenderFragment TwoTabLayout() => b =>
    {
        b.OpenComponent<FlareTabs>(0);
        b.AddAttribute(1, "ChildContent", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareTab>(2);
            inner.AddAttribute(3, "Label", "Tab One");
            inner.AddAttribute(4, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(5, "<p>Content One</p>")));
            inner.CloseComponent();

            inner.OpenComponent<FlareTab>(6);
            inner.AddAttribute(7, "Label", "Tab Two");
            inner.AddAttribute(8, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(9, "<p>Content Two</p>")));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Renders_TabsContainer_WithCorrectCssClass()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "First");
                b.CloseComponent();
            }));

        Assert.Single(cut.FindAll($".{Css.Classes.Tabs.Root}"));
    }

    [Fact]
    public void Renders_TabBar_WithTabButtons()
    {
        var cut = Render(TwoTabLayout());

        var tabButtons = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Equal(2, tabButtons.Count);
    }

    [Fact]
    public void TabButtons_HaveCorrectLabels()
    {
        var cut = Render(TwoTabLayout());

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains("Tab One", tabs[0].TextContent);
        Assert.Contains("Tab Two", tabs[1].TextContent);
    }

    [Fact]
    public void FirstTab_IsActiveByDefault()
    {
        var cut = Render(TwoTabLayout());

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[0].ClassName);
        Assert.DoesNotContain(Css.Classes.Tabs.TabActive, tabs[1].ClassName);
    }

    [Fact]
    public void FirstTab_ContentVisible_ByDefault()
    {
        var cut = Render(TwoTabLayout());

        // First tab panel should be rendered
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Tabs.Panel}"));
        Assert.Contains("Content One", cut.Markup);
    }

    [Fact]
    public void SecondTab_ContentNotVisible_Initially()
    {
        var cut = Render(TwoTabLayout());

        // Panels are always rendered (no state loss), inactive panel has hidden class
        var panels = cut.FindAll($".{Css.Classes.Tabs.Panel}");
        Assert.Equal(2, panels.Count);
        Assert.Contains(Css.Classes.Tabs.PanelHidden, panels[1].ClassName);
    }

    [Fact]
    public void ClickingSecondTab_MakesItActive()
    {
        var cut = Render(TwoTabLayout());

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        tabs[1].Click();

        var updatedTabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.DoesNotContain(Css.Classes.Tabs.TabActive, updatedTabs[0].ClassName);
        Assert.Contains(Css.Classes.Tabs.TabActive, updatedTabs[1].ClassName);
    }

    [Fact]
    public void UserSelection_SurvivesParentReRender()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
                b.OpenComponent<FlareTab>(2);
                b.AddAttribute(3, "Label", "Two");
                b.CloseComponent();
            }));

        cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].Click();
        Assert.Contains(Css.Classes.Tabs.TabActive, cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].ClassName);

        // A parent re-render (params re-supplied with ActiveIndex unchanged) must NOT snap the
        // selection back to ActiveIndex (the default 0).
        cut.Render(p => p.Add(x => x.ActiveIndex, 0));

        Assert.Contains(Css.Classes.Tabs.TabActive, cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].ClassName);
        Assert.DoesNotContain(Css.Classes.Tabs.TabActive, cut.FindAll($".{Css.Classes.Tabs.TabButton}")[0].ClassName);
    }

    [Fact]
    public void ClickingSecondTab_ShowsSecondTabContent()
    {
        var cut = Render(TwoTabLayout());

        cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].Click();

        // After click: second panel is active (no hidden class), first is hidden
        var panels = cut.FindAll($".{Css.Classes.Tabs.Panel}");
        Assert.DoesNotContain(Css.Classes.Tabs.PanelHidden, panels[1].ClassName);
        Assert.Contains(Css.Classes.Tabs.PanelHidden, panels[0].ClassName);
    }

    [Fact]
    public void DisabledTab_CannotBeActivated()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "Active Tab");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(3, "<p>Active</p>")));
                b.CloseComponent();

                b.OpenComponent<FlareTab>(4);
                b.AddAttribute(5, "Label", "Disabled Tab");
                b.AddAttribute(6, "Disabled", true);
                b.AddAttribute(7, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(8, "<p>Disabled Content</p>")));
                b.CloseComponent();
            }));

        // Click the disabled tab
        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        tabs[1].Click();

        // First tab should still be active
        Assert.Contains(Css.Classes.Tabs.TabActive, cut.FindAll($".{Css.Classes.Tabs.TabButton}")[0].ClassName);
        Assert.DoesNotContain(Css.Classes.Tabs.TabActive, cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].ClassName);
    }

    [Fact]
    public void DisabledTab_HasDisabledAttribute()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "Normal");
                b.CloseComponent();

                b.OpenComponent<FlareTab>(2);
                b.AddAttribute(3, "Label", "Disabled");
                b.AddAttribute(4, "Disabled", true);
                b.CloseComponent();
            }));

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.True(tabs[1].HasAttribute("disabled"));
    }

    [Fact]
    public void TabBar_HasRoleTablist()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        Assert.NotNull(cut.Find("[role='tablist']"));
    }

    [Fact]
    public void TabButton_HasRoleTab()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        var tabBtn = cut.Find($".{Css.Classes.Tabs.TabButton}");
        Assert.Equal("tab", tabBtn.GetAttribute("role"));
    }

    [Fact]
    public void ActiveTabButton_HasAriaSelectedAttribute()
    {
        var cut = Render(TwoTabLayout());

        // Blazor renders bool true as aria-selected="" (attribute present)
        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.True(tabs[0].HasAttribute("aria-selected"));
    }

    [Fact]
    public void InactiveTabButton_HasNoAriaSelectedAttribute()
    {
        var cut = Render(TwoTabLayout());

        // Blazor removes the attribute when the bool value is false
        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.False(tabs[1].HasAttribute("aria-selected"));
    }

    [Fact]
    public void ActiveIndex_SelectsBoundTab()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
                b.OpenComponent<FlareTab>(2);
                b.AddAttribute(3, "Label", "Two");
                b.CloseComponent();
            }));

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.DoesNotContain(Css.Classes.Tabs.TabActive, tabs[0].ClassName);
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[1].ClassName);
    }

    [Fact]
    public void ClickingTab_RaisesActiveIndexChanged()
    {
        var changed = -1;
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => changed = i))
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
                b.OpenComponent<FlareTab>(2);
                b.AddAttribute(3, "Label", "Two");
                b.CloseComponent();
            }));

        cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].Click();
        Assert.Equal(1, changed);
    }

    [Fact]
    public void OnPreviewInteraction_Cancel_BlocksActivation()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.OnPreviewInteraction, EventCallback.Factory.Create<TabInteractionEventArgs>(this, a => a.Cancel = true))
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
                b.OpenComponent<FlareTab>(2);
                b.AddAttribute(3, "Label", "Two");
                b.CloseComponent();
            }));

        cut.FindAll($".{Css.Classes.Tabs.TabButton}")[1].Click();

        var tabs = cut.FindAll($".{Css.Classes.Tabs.TabButton}");
        Assert.Contains(Css.Classes.Tabs.TabActive, tabs[0].ClassName);
        Assert.DoesNotContain(Css.Classes.Tabs.TabActive, tabs[1].ClassName);
    }

    [Fact]
    public void BadgeDot_RendersDotModifier()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.AddAttribute(2, "BadgeDot", true);
                b.CloseComponent();
            }));

        Assert.NotNull(cut.Find($".{Css.Classes.Tabs.BadgeDot}"));
    }

    [Fact]
    public void Tooltip_RendersTitleAttribute()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.AddAttribute(2, "Tooltip", "Hello");
                b.CloseComponent();
            }));

        Assert.Equal("Hello", cut.Find($".{Css.Classes.Tabs.TabButton}").GetAttribute("title"));
    }

    [Fact]
    public void HeaderStartAndEnd_RenderCustomContent()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.HeaderStart, (RenderFragment)(b => b.AddMarkupContent(0, "<button id=\"start-btn\">S</button>")))
            .Add(x => x.HeaderEnd, (RenderFragment)(b => b.AddMarkupContent(0, "<button id=\"end-btn\">E</button>")))
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        Assert.NotNull(cut.Find($".{Css.Classes.Tabs.HeaderStart} #start-btn"));
        Assert.NotNull(cut.Find($".{Css.Classes.Tabs.HeaderEnd} #end-btn"));
    }

    [Fact]
    public void HeaderZones_NotRendered_WhenUnset()
    {
        var cut = Render<FlareTabs>(p => p
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        Assert.Empty(cut.FindAll($".{Css.Classes.Tabs.HeaderStart}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Tabs.HeaderEnd}"));
    }

    [Fact]
    public void Typo_AppliesLabelFontTokens()
    {
        var cut = Render<FlareTabs>(p => p
            .Add(x => x.Typo, TypographyScale.TitleSmall)
            .AddChildContent(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.CloseComponent();
            }));

        var style = cut.Find($".{Css.Classes.Tabs.Root}").GetAttribute("style") ?? "";
        Assert.Contains($"{Css.Tokens.Tabs.LabelSize}:var({Css.Tokens.Typography.TitleSmall.Size})", style);
    }
}
