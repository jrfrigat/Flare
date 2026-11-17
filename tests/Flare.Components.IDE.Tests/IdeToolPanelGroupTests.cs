using System.Globalization;
using Flare.Components.IDE;
using Flare.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// Several tool panels in one region share a tab strip and show one body at a time, while the panels
/// keep reporting to the layout so the rail, collapse and auto-hide behavior do not change.
/// </summary>
public class IdeToolPanelGroupTests : FlareTestContext
{
    public IdeToolPanelGroupTests() => Services.AddFlareIde();

    private const string UiModule = "./_content/Flare.Components/js/flare-ui.js";

    private sealed record Panel(string Title, string Text, IdePanelMode Mode = IdePanelMode.Docked,
        string? Key = null, FlareIcon? Icon = null);

    private static RenderFragment Panels(params Panel[] panels) => builder =>
    {
        var sequence = 0;
        foreach (var panel in panels)
        {
            builder.OpenComponent<FlareToolPanel>(sequence++);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Title), panel.Title);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Mode), panel.Mode);
            if (panel.Key is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Key), panel.Key);
            if (panel.Icon is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Icon), panel.Icon);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.ChildContent),
                (RenderFragment)(content => content.AddContent(0, panel.Text)));
            builder.CloseComponent();
        }
    };

    private static RenderFragment Group(RenderFragment panels) => builder =>
    {
        builder.OpenComponent<FlareToolPanelGroup>(0);
        builder.AddAttribute(1, nameof(FlareToolPanelGroup.ChildContent), panels);
        builder.CloseComponent();
    };

    private static RenderFragment Actions() => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "aria-label", "Pin panel");
        builder.AddContent(3, "Pin");
        builder.CloseElement();
    };

    private static RenderFragment GroupWithActions(RenderFragment panels, bool showCollapseButton = true) => builder =>
    {
        builder.OpenComponent<FlareToolPanelGroup>(0);
        builder.AddAttribute(1, nameof(FlareToolPanelGroup.ChildContent), panels);
        builder.AddAttribute(2, nameof(FlareToolPanelGroup.ToolbarContent), Actions());
        builder.AddAttribute(3, nameof(FlareToolPanelGroup.ShowCollapseButton), showCollapseButton);
        builder.CloseComponent();
    };

    private IRenderedComponent<FlareToolPanelGroup> RenderGroup(params Panel[] panels) =>
        Render<FlareToolPanelGroup>(p => p.Add(x => x.ChildContent, Panels(panels)));

    private IRenderedComponent<FlareToolPanelGroup> RenderControlled(string activeKey, params Panel[] panels) =>
        Render<FlareToolPanelGroup>(p => p
            .Add(x => x.ActiveKey, activeKey)
            .Add(x => x.ChildContent, Panels(panels)));

    private IRenderedComponent<FlareToolPanelGroup> RenderWithActions(params Panel[] panels) =>
        Render<FlareToolPanelGroup>(p => p
            .Add(x => x.ChildContent, Panels(panels))
            .Add(x => x.ToolbarContent, Actions()));

    private static string[] Selected(IRenderedComponent<FlareToolPanelGroup> cut) =>
        cut.FindAll("[role=tab]").Select(tab => tab.GetAttribute("aria-selected")!).ToArray();

    [Fact]
    public async Task Group_ShowsOnePanelAndKeepsTheOthersMounted()
    {
        var cut = RenderGroup(new("Explorer", "Files"), new("Properties", "Details"));

        Assert.Equal(["true", "false"], Selected(cut));
        Assert.Equal(2, cut.FindAll("[role=tabpanel]").Count);

        await cut.FindAll("[role=tab]")[1].ClickAsync(new());

        Assert.Equal(["false", "true"], Selected(cut));
        Assert.True(cut.FindAll("[role=tabpanel]")[0].HasAttribute("hidden"));
        Assert.False(cut.FindAll("[role=tabpanel]")[1].HasAttribute("hidden"));
        Assert.Contains("Details", cut.FindAll("[role=tabpanel]")[1].TextContent);
    }

    [Fact]
    public async Task Group_NavigationKeysMoveSelectionAndHonourRtl()
    {
        var cut = RenderGroup(new("One", ""), new("Two", ""), new("Three", ""));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("ArrowRight", rtl: false));
        Assert.Equal(["false", "true", "false"], Selected(cut));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("ArrowLeft", rtl: true));
        Assert.Equal(["false", "false", "true"], Selected(cut));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("Home", rtl: false));
        Assert.Equal(["true", "false", "false"], Selected(cut));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("End", rtl: false));
        Assert.Equal(["false", "false", "true"], Selected(cut));
    }

    [Fact]
    public void Group_ControlledKeySelectsTheMatchingPanel()
    {
        var cut = RenderControlled("properties",
            new("Explorer", "Files", Key: "explorer"), new("Properties", "Details", Key: "properties"));

        Assert.Equal(["false", "true"], Selected(cut));
        Assert.Contains("Details", cut.FindAll("[role=tabpanel]")[1].TextContent);
    }

    [Fact]
    public void Group_ControlledKeyChangeMovesTheSelection()
    {
        var cut = RenderControlled("explorer",
            new("Explorer", "Files", Key: "explorer"), new("Properties", "Details", Key: "properties"));
        Assert.Equal(["true", "false"], Selected(cut));

        cut.Render(p => p.Add(x => x.ActiveKey, "properties"));

        Assert.Equal(["false", "true"], Selected(cut));
    }

    [Fact]
    public void Group_UnknownControlledKeyKeepsTheCurrentPanel()
    {
        var cut = RenderControlled("missing",
            new("Explorer", "Files", Key: "explorer"), new("Properties", "Details", Key: "properties"));

        Assert.Equal(["true", "false"], Selected(cut));
        Assert.False(cut.FindAll("[role=tabpanel]")[0].HasAttribute("hidden"));

        cut.Render(p => p.Add(x => x.ActiveKey, "also-missing"));

        Assert.Equal(["true", "false"], Selected(cut));
    }

    [Fact]
    public void Group_KeylessPanelIsSelectedByItsTitle()
    {
        var cut = RenderControlled("Properties",
            new("Explorer", "Files"), new("Properties", "Details"));

        Assert.Equal(["false", "true"], Selected(cut));
    }

    [Fact]
    public async Task Group_TabClickReportsKeyAndTitle()
    {
        string? reportedKey = null;
        string? reportedTitle = null;
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.ActiveKey, "explorer")
            .Add(x => x.ActiveKeyChanged, key => reportedKey = key)
            .Add(x => x.OnActiveChanged, title => reportedTitle = title)
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files", Key: "explorer"), new("Properties", "Details", Key: "properties"))));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new());

        Assert.Equal("properties", reportedKey);
        Assert.Equal("Properties", reportedTitle);
    }

    [Fact]
    public async Task Group_ControlledClickActsWhenTheParentSetsTheKey()
    {
        string? reportedKey = null;
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.ActiveKey, "explorer")
            .Add(x => x.ActiveKeyChanged, key => reportedKey = key)
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files", Key: "explorer"), new("Properties", "Details", Key: "properties"))));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new());

        Assert.Equal("properties", reportedKey);
        Assert.Equal(["true", "false"], Selected(cut));

        cut.Render(p => p.Add(x => x.ActiveKey, "properties"));

        Assert.Equal(["false", "true"], Selected(cut));
    }

    [Fact]
    public async Task Group_KeyboardNavigationReportsTheActivatedPanel()
    {
        string? reportedKey = null;
        string? reportedTitle = null;
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.ActiveKeyChanged, key => reportedKey = key)
            .Add(x => x.OnActiveChanged, title => reportedTitle = title)
            .Add(x => x.ChildContent, Panels(new("One", "", Key: "one"), new("Two", "", Key: "two"))));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("ArrowRight", rtl: false));

        Assert.Equal("two", reportedKey);
        Assert.Equal("Two", reportedTitle);
        Assert.Equal(["false", "true"], Selected(cut));
    }

    [Fact]
    public void Group_ToolbarActionsRenderAfterTheTabsAndBeforeTheCollapseButton()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.LeftToolPanel,
            GroupWithActions(Panels(new Panel("Explorer", "Files")))));

        var header = cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Header}");
        Assert.Equal(3, header.Children.Length);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Tabs, header.Children[0].ClassName);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Actions, header.Children[1].ClassName);
        Assert.Equal("button", header.Children[2].LocalName);
        Assert.Single(header.Children[1].QuerySelectorAll("button[aria-label='Pin panel']"));
    }

    [Fact]
    public void Group_ToolbarHasALocalizedAccessibleName()
    {
        Assert.Equal("Panel actions", RenderWithActions(new Panel("Explorer", ""))
            .Find($".{Css.Classes.Ide.ToolPanelGroup.Actions}").GetAttribute("aria-label"));

        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

            Assert.Equal("Действия панели", RenderWithActions(new Panel("Explorer", ""))
                .Find($".{Css.Classes.Ide.ToolPanelGroup.Actions}").GetAttribute("aria-label"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public async Task Group_ToolbarKeepsTabSelectionAndKeyboardNavigation()
    {
        var cut = RenderWithActions(new("One", ""), new("Two", ""), new("Three", ""));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new());
        Assert.Equal(["false", "true", "false"], Selected(cut));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("End", rtl: false));
        Assert.Equal(["false", "false", "true"], Selected(cut));
    }

    [Fact]
    public async Task Layout_GroupToolbarKeepsTheCollapseButton()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.LeftToolPanel,
            GroupWithActions(Panels(new("Explorer", ""), new("Properties", "")))));

        await cut.InvokeAsync(() => cut.Find(
            $".{Css.Classes.Ide.ToolPanelGroup.Header} > button").ClickAsync(new()));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.ToolPanelGroup.Root}"));
        Assert.Equal(["Explorer", "Properties"],
            cut.FindAll($".{Css.Classes.Ide.Layout.RailTitle}").Select(title => title.TextContent));
    }

    // The tab strip disconnects with the circuit; the keyboard unregistration in DisposeAsync then throws
    // JSDisconnectedException, which is not a JSException and must not fault the disposal.
    [Fact]
    public async Task Group_DisposingAfterTheCircuitDisconnected_DoesNotThrow()
    {
        var module = JSInterop.SetupModule(UiModule);
        module.SetupVoid("registerTabKeyboard", _ => true).SetVoidResult();
        module.SetupVoid("removeTabKeyboard", _ => true)
            .SetException(new JSDisconnectedException("circuit gone"));

        var cut = RenderGroup(new("Explorer", "Files"), new("Properties", "Details"));
        // The disposal unregisters only after the asynchronous registration has completed; wait for
        // that state, or the test would pass without the removal call ever being made.
        var registered = typeof(FlareToolPanelGroup).GetField("_keyboardRegistered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        cut.WaitForAssertion(() => Assert.True((bool)registered.GetValue(cut.Instance)!));

        await cut.Instance.DisposeAsync();

        Assert.Contains(module.Invocations, invocation => invocation.Identifier == "removeTabKeyboard");
        Assert.NotEmpty(cut.FindAll("[role=tab]"));
    }

    [Fact]
    public void Group_TabShowsItsIconAndKeepsTheLabelInOneElement()
    {
        var cut = RenderGroup(new("Explorer", "", Icon: FlareIcons.Folder), new("Properties", ""));

        var tabs = cut.FindAll("[role=tab]");
        Assert.Single(tabs[0].QuerySelectorAll("svg"));
        Assert.Empty(tabs[1].QuerySelectorAll("svg"));
        Assert.Equal("true", tabs[0].GetAttribute("data-flare-tab-icon"));
        Assert.Null(tabs[1].GetAttribute("data-flare-tab-icon"));
        Assert.Equal("Explorer",
            tabs[0].QuerySelector($".{Css.Classes.Ide.ToolPanelGroup.TabLabel}")!.TextContent.Trim());
        Assert.Equal("Explorer", tabs[0].TextContent.Trim());
        Assert.False(tabs[0].HasAttribute("data-flare-tab-compact"));
    }

    [Fact]
    public void Layout_GroupWithoutCollapseButtonKeepsTabsAndActions()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.LeftToolPanel,
            GroupWithActions(Panels(new("Explorer", ""), new("Properties", "")), showCollapseButton: false)));

        var header = cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Header}");
        Assert.Equal(2, header.Children.Length);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Tabs, header.Children[0].ClassName);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Actions, header.Children[1].ClassName);
        Assert.Single(header.Children[1].QuerySelectorAll("button[aria-label='Pin panel']"));
        Assert.Empty(header.QuerySelectorAll(":scope > button"));
    }

    [Fact]
    public void Layout_GroupWithoutCollapseButtonStillCollapsesThroughTheRegionState()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.LeftToolPanel,
            GroupWithActions(Panels(new("Explorer", ""), new("Properties", "")), showCollapseButton: false)));
        Assert.NotNull(cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Root}"));

        cut.Render(p => p
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel,
                GroupWithActions(Panels(new("Explorer", ""), new("Properties", "")), showCollapseButton: false)));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.ToolPanelGroup.Root}"));
        Assert.Equal(["Explorer", "Properties"],
            cut.FindAll($".{Css.Classes.Ide.Layout.RailTitle}").Select(title => title.TextContent));
    }

    [Fact]
    public void Layout_TwoGroupsWithToolbarsKeepUniqueIds()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftToolPanel, GroupWithActions(Panels(new Panel("Explorer", ""))))
            .Add(x => x.RightToolPanel, GroupWithActions(Panels(new Panel("Properties", "")))));

        var ids = cut.FindAll("[id]").Select(element => element.Id).ToArray();
        Assert.NotEmpty(ids);
        Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Group_UsesLocalizedTabListAndUntitledLabels()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            var cut = RenderGroup(new Panel("", "Body"));

            Assert.Equal("Панели инструментов",
                cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Tabs}").GetAttribute("aria-label"));
            Assert.Equal("Без названия", cut.Find("[role=tab]").TextContent.Trim());
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public async Task Layout_GroupHeaderCollapsesTheRegionAndLabelsEveryPanel()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.LeftToolPanel,
            Group(Panels(new("Explorer", ""), new("Properties", "")))));

        await cut.InvokeAsync(() => cut.Find(
            $".{Css.Classes.Ide.ToolPanelGroup.Header} > button").ClickAsync(new()));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.ToolPanelGroup.Root}"));
        Assert.Equal(["Explorer", "Properties"],
            cut.FindAll($".{Css.Classes.Ide.Layout.RailTitle}").Select(title => title.TextContent));
    }

    [Fact]
    public void Layout_GroupedAutoHidePanelsFloatTheRegion()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.LeftToolPanel,
            Group(Panels(new("Explorer", "", IdePanelMode.AutoHide),
                new("Properties", "", IdePanelMode.AutoHide)))));

        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}"));
    }

    [Fact]
    public async Task Layout_RegionButtonReturnsToTheGroupTabChosenBeforeTheDocument()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [])
            .Add(x => x.LeftToolPanel, Group(Panels(new("Explorer", "Files"), new("Properties", "Details")))));

        await cut.InvokeAsync(() => cut.FindAll("[role=tab]")[1].ClickAsync(new()));

        cut.Render(p => p.Add(x => x.Documents, [new("a", "A.cs", IdeDocumentRegion.Left)]));
        var leftBody = $".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.PanelBody}";

        Assert.True(cut.Find(leftBody).HasAttribute("inert"));
        await cut.InvokeAsync(() => cut.Find(
            $".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Left\"] button").ClickAsync(new()));

        Assert.False(cut.Find(leftBody).HasAttribute("inert"));
        Assert.Equal("true", cut.FindAll("[role=tab]")[1].GetAttribute("aria-selected"));
        Assert.False(cut.FindAll("[role=tabpanel]")[1].HasAttribute("hidden"));
    }
}
