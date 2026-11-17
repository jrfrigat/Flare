using Flare.Components.IDE;
using Flare.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// In <see cref="IdeToolRailMode.PerTool"/> an auto-hide side region lists one rail entry per hosted
/// tool group tab. Selecting an entry opens the tool in the flyout, the active entry is marked, and
/// selecting it again closes the flyout - without moving the document area. The default region rail
/// and regions without a tool group keep today's behavior.
/// </summary>
public class IdeLayoutToolRailTests : FlareTestContext
{
    public IdeLayoutToolRailTests() => Services.AddFlareIde();

    private sealed record Tool(string Title, string? Key, FlareIcon? Icon = null,
        IdePanelMode Mode = IdePanelMode.AutoHide);

    private static RenderFragment Panels(params Tool[] tools) => builder =>
    {
        var sequence = 0;
        foreach (var tool in tools)
        {
            builder.OpenComponent<FlareToolPanel>(sequence++);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Title), tool.Title);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Mode), tool.Mode);
            if (tool.Key is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Key), tool.Key);
            if (tool.Icon is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Icon), tool.Icon);
            builder.CloseComponent();
        }
    };

    private static RenderFragment Group(ToolPanelPosition position, string? activeKey,
        bool showTabStripWhenFloating, params Tool[] tools) => builder =>
    {
        builder.OpenComponent<FlareToolPanelGroup>(0);
        builder.AddAttribute(1, nameof(FlareToolPanelGroup.Position), position);
        builder.AddAttribute(2, nameof(FlareToolPanelGroup.ShowTabStripWhenFloating), showTabStripWhenFloating);
        if (activeKey is not null)
            builder.AddAttribute(3, nameof(FlareToolPanelGroup.ActiveKey), activeKey);
        builder.AddAttribute(4, nameof(FlareToolPanelGroup.ChildContent), Panels(tools));
        builder.CloseComponent();
    };

    private static string EntrySelector => $".{Css.Classes.Ide.Layout.ToolRailEntry}";
    private static string ToolRailSelector => $".{Css.Classes.Ide.Layout.ToolRail}";
    private static string LeftToolRailSelector =>
        $".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.ToolRail}";
    private static string RightEntrySelector =>
        $".{Css.Classes.Ide.Layout.PanelRight} .{Css.Classes.Ide.Layout.ToolRailEntry}";

    private static string[] Pressed(IRenderedComponent<FlareIdeLayout> cut) =>
        cut.FindAll(EntrySelector).Select(entry => entry.GetAttribute("aria-pressed")!).ToArray();

    private static Tool[] TwoTools() =>
    [
        new("Explorer", "explorer", FlareIcons.Folder),
        new("Properties", "properties", FlareIcons.Settings)
    ];

    [Fact]
    public void PerToolRail_ListsEveryToolWithItsIconAndLabel()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(EntrySelector).Count));

        var entries = cut.FindAll(EntrySelector);
        Assert.Equal(["Explorer", "Properties"], entries.Select(entry => entry.TextContent.Trim()));
        Assert.All(entries, entry => Assert.Equal("button", entry.LocalName));
        Assert.All(entries, entry => Assert.Equal("button", entry.GetAttribute("type")));
        Assert.Equal("explorer", entries[0].GetAttribute("data-flare-tool-key"));
        Assert.Equal("properties", entries[1].GetAttribute("data-flare-tool-key"));
        Assert.Single(entries[0].QuerySelectorAll("svg"));
        Assert.Single(entries[1].QuerySelectorAll("svg"));
        Assert.Single(cut.FindAll(ToolRailSelector));
    }

    [Fact]
    public void DefaultRegionMode_KeepsTheSingleRailButton()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, null, true, TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.Rail}")));
        Assert.Empty(cut.FindAll(EntrySelector));
    }

    [Fact]
    public void RegionWithoutAGroup_KeepsTheSingleRailButton()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel, Panels(new Tool("Explorer", "explorer"))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.Rail}")));
        Assert.Empty(cut.FindAll(EntrySelector));
    }

    [Fact]
    public async Task PerToolRail_ClickingAToolSelectsItAndOpensTheFlyout()
    {
        string? selected = null;
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftActiveToolKey, "explorer")
            .Add(x => x.LeftActiveToolKeyChanged,
                EventCallback.Factory.Create<string?>(this, key => selected = key))
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Equal(["true", "false"], Pressed(cut)));
        await cut.FindAll(EntrySelector)[1].ClickAsync(new());

        Assert.Equal("properties", selected);
        Assert.False(collapsed);
        Assert.Equal(["false", "true"], Pressed(cut));
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}"));
        Assert.Single(cut.FindAll(ToolRailSelector));
    }

    [Fact]
    public void PerToolRail_TheControlledKeyDrivesTheGroupAndHidesTheTabsWhileFloating()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftActiveToolKey, "explorer")
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}")));
        Assert.Empty(cut.FindAll("[role=tablist]"));
        Assert.Equal("Explorer", cut.Find($".{Css.Classes.Ide.ToolPanelGroup.TitleText}").TextContent.Trim());

        cut.Render(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftActiveToolKey, "properties")
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "properties", false, TwoTools())));

        Assert.Equal(["false", "true"], Pressed(cut));
        Assert.Equal("Properties", cut.Find($".{Css.Classes.Ide.ToolPanelGroup.TitleText}").TextContent.Trim());
        Assert.True(cut.FindAll("[role=tabpanel]")[0].HasAttribute("hidden"));
        Assert.False(cut.FindAll("[role=tabpanel]")[1].HasAttribute("hidden"));
    }

    [Fact]
    public async Task PerToolRail_ClickingAnotherToolWhileFloatingSwitchesAndStaysOpen()
    {
        string? selected = null;
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftActiveToolKey, "explorer")
            .Add(x => x.LeftActiveToolKeyChanged,
                EventCallback.Factory.Create<string?>(this, key => selected = key))
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(EntrySelector).Count));
        await cut.FindAll(EntrySelector)[1].ClickAsync(new());

        Assert.Equal("properties", selected);
        Assert.Null(collapsed);
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}"));
        Assert.Single(cut.FindAll(ToolRailSelector));
    }

    [Fact]
    public async Task PerToolRail_ClickingTheActiveToolClosesTheFlyout()
    {
        bool? collapsed = null;
        var keyChanges = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftActiveToolKey, "explorer")
            .Add(x => x.LeftActiveToolKeyChanged,
                EventCallback.Factory.Create<string?>(this, _ => keyChanges++))
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}")));
        await cut.FindAll(EntrySelector)[0].ClickAsync(new());

        Assert.True(collapsed);
        Assert.Equal(0, keyChanges);
        Assert.Single(cut.FindAll(ToolRailSelector));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}"));
    }

    [Fact]
    public void PerToolRail_MarksTheGroupsOwnSelectionWhenTheHostDoesNotControlTheKey()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "properties", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Equal(["false", "true"], Pressed(cut)));
    }

    [Fact]
    public void PerToolRail_SuppressesTheTabsOnlyWhileFloating()
    {
        RenderFragment PanelSet() => Group(ToolPanelPosition.Left, "explorer", false, TwoTools());

        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftActiveToolKey, "explorer")
            .Add(x => x.LeftToolPanel, PanelSet()));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}")));
        Assert.Empty(cut.FindAll("[role=tablist]"));

        cut.Render(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftActiveToolKey, "explorer")
            .Add(x => x.LeftToolPanel, PanelSet()));

        Assert.Single(cut.FindAll("[role=tablist]"));
        Assert.Single(cut.FindAll(ToolRailSelector));
    }

    [Fact]
    public void DefaultTabStripStaysWhileFloating()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftToolPanel,
                Group(ToolPanelPosition.Left, "explorer", true, TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}")));
        Assert.Single(cut.FindAll("[role=tablist]"));
    }

    [Fact]
    public async Task PerToolRail_WorksOnTheRightRegion()
    {
        string? selected = null;
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.RightCollapsed, true)
            .Add(x => x.RightActiveToolKeyChanged,
                EventCallback.Factory.Create<string?>(this, key => selected = key))
            .Add(x => x.RightCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.RightToolPanel,
                Group(ToolPanelPosition.Right, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(RightEntrySelector).Count));
        await cut.FindAll(RightEntrySelector)[1].ClickAsync(new());

        Assert.Equal("properties", selected);
        Assert.False(collapsed);
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelRight}.{Css.Classes.Ide.Layout.PanelFloating}"));
    }

    [Fact]
    public async Task PerToolRail_KeepsTheDropTargetAndExpandsOnDrop()
    {
        IdeDocumentMove? moved = null;
        bool? expanded = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.Documents, [new("a", "A.cs")])
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => expanded = value))
            .Add(x => x.OnDocumentMove,
                EventCallback.Factory.Create<IdeDocumentMove>(this, value => moved = value))
            .Add(x => x.LeftToolPanel, Group(ToolPanelPosition.Left, "explorer", false, TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(LeftToolRailSelector)));

        await cut.Find($".{Css.Classes.Ide.Layout.DockSelect}").TriggerEventAsync("ondragstart", new DragEventArgs());
        await cut.Find(LeftToolRailSelector).TriggerEventAsync("ondragenter", new DragEventArgs());
        Assert.Contains(Css.Classes.Ide.Layout.RailDropTarget, cut.Find(LeftToolRailSelector).ClassList);

        // Crossing into a child button fires another enter before the leave: the highlight must stay.
        await cut.Find(LeftToolRailSelector).TriggerEventAsync("ondragenter", new DragEventArgs());
        await cut.Find(LeftToolRailSelector).TriggerEventAsync("ondragleave", new DragEventArgs());
        Assert.Contains(Css.Classes.Ide.Layout.RailDropTarget, cut.Find(LeftToolRailSelector).ClassList);

        await cut.Find(LeftToolRailSelector).TriggerEventAsync("ondragleave", new DragEventArgs());
        Assert.DoesNotContain(Css.Classes.Ide.Layout.RailDropTarget, cut.Find(LeftToolRailSelector).ClassList);

        await cut.Find(LeftToolRailSelector).TriggerEventAsync("ondrop", new DragEventArgs());
        cut.WaitForAssertion(() => Assert.NotNull(moved));
        Assert.Equal(IdeDocumentRegion.Left, moved!.Destination);
        Assert.False(expanded);
    }
}
