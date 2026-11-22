using Flare.Components.IDE;
using Flare.Icons;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// <see cref="FlareToolPanelGroup.TabStripPlacement"/> puts the one tab strip either in the header
/// (the default, unchanged) or below the body, where the header carries the active tool's title and
/// icon. Selection, ARIA and keyboard behavior stay identical in both placements.
/// </summary>
public class IdeToolPanelGroupPlacementTests : FlareTestContext
{
    public IdeToolPanelGroupPlacementTests() => Services.AddFlareIde();

    private sealed record Panel(string Title, string Text, string? Key = null, FlareIcon? Icon = null);

    private static RenderFragment Panels(params Panel[] panels) => builder =>
    {
        var sequence = 0;
        foreach (var panel in panels)
        {
            builder.OpenComponent<FlareToolPanel>(sequence++);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Title), panel.Title);
            if (panel.Key is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Key), panel.Key);
            if (panel.Icon is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Icon), panel.Icon);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.ChildContent),
                (RenderFragment)(content => content.AddContent(0, panel.Text)));
            builder.CloseComponent();
        }
    };

    private static RenderFragment Group(TabStripPlacement placement, RenderFragment panels,
        RenderFragment? toolbar = null, string? activeKey = null,
        EventCallback<string?> activeKeyChanged = default) => builder =>
    {
        builder.OpenComponent<FlareToolPanelGroup>(0);
        builder.AddAttribute(1, nameof(FlareToolPanelGroup.ChildContent), panels);
        builder.AddAttribute(2, nameof(FlareToolPanelGroup.TabStripPlacement), placement);
        if (toolbar is not null)
            builder.AddAttribute(3, nameof(FlareToolPanelGroup.ToolbarContent), toolbar);
        if (activeKey is not null)
            builder.AddAttribute(4, nameof(FlareToolPanelGroup.ActiveKey), activeKey);
        if (activeKeyChanged.HasDelegate)
            builder.AddAttribute(5, nameof(FlareToolPanelGroup.ActiveKeyChanged), activeKeyChanged);
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

    private IRenderedComponent<FlareToolPanelGroup> RenderGroup(TabStripPlacement placement,
        params Panel[] panels) => Render<FlareToolPanelGroup>(p => p
            .Add(x => x.TabStripPlacement, placement)
            .Add(x => x.ChildContent, Panels(panels)));

    private static string[] Selected(IRenderedComponent<FlareToolPanelGroup> cut) =>
        cut.FindAll("[role=tab]").Select(tab => tab.GetAttribute("aria-selected")!).ToArray();

    private static string TitleText(IRenderedComponent<FlareToolPanelGroup> cut) =>
        cut.Find($".{Css.Classes.Ide.ToolPanelGroup.TitleText}").TextContent.Trim();

    [Fact]
    public void BottomPlacement_PutsTheOneTabStripAfterTheBody()
    {
        var cut = RenderGroup(TabStripPlacement.Bottom,
            new("Explorer", "Files"), new("Properties", "Details"));

        var root = cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Root}");
        Assert.Contains(Css.Classes.Ide.ToolPanelGroup.TabsBottomRoot, root.ClassList);
        Assert.Equal(3, root.Children.Length);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Header, root.Children[0].ClassName);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Body, root.Children[1].ClassName);
        var bottomTabs = root.Children[2].QuerySelector("[role=tablist]");
        Assert.NotNull(bottomTabs);
        Assert.Contains(Css.Classes.Ide.ToolPanelGroup.TabStripBottom, bottomTabs.ClassList);

        Assert.Single(cut.FindAll("[role=tablist]"));
        Assert.Empty(cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Header}")
            .QuerySelectorAll("[role=tablist]"));
    }

    [Fact]
    public void TopPlacement_KeepsTheStripInTheHeaderAndRendersNoTitle()
    {
        var cut = RenderGroup(TabStripPlacement.Top,
            new("Explorer", "Files"), new("Properties", "Details"));

        var root = cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Root}");
        Assert.DoesNotContain(Css.Classes.Ide.ToolPanelGroup.TabsBottomRoot, root.ClassList);
        Assert.Equal(2, root.Children.Length);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Header, root.Children[0].ClassName);
        Assert.NotNull(root.Children[0].QuerySelector("[role=tablist]"));
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Body, root.Children[1].ClassName);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.ToolPanelGroup.Title}"));
        Assert.Single(cut.FindAll("[role=tablist]"));
    }

    [Fact]
    public async Task BottomPlacement_ShowsTheActiveTitleAndFollowsATabClick()
    {
        var cut = RenderGroup(TabStripPlacement.Bottom,
            new("Explorer", "Files"), new("Properties", "Details"));

        Assert.Equal("Explorer", TitleText(cut));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new());

        Assert.Equal("Properties", TitleText(cut));
        Assert.Equal(["false", "true"], Selected(cut));
        Assert.True(cut.FindAll("[role=tabpanel]")[0].HasAttribute("hidden"));
        Assert.False(cut.FindAll("[role=tabpanel]")[1].HasAttribute("hidden"));
    }

    [Fact]
    public void BottomPlacement_TitleFollowsAControlledKey()
    {
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ActiveKey, "explorer")
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files", Key: "explorer"),
                    new("Properties", "Details", Key: "properties"))));

        Assert.Equal("Explorer", TitleText(cut));

        cut.Render(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ActiveKey, "properties")
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files", Key: "explorer"),
                    new("Properties", "Details", Key: "properties"))));

        Assert.Equal("Properties", TitleText(cut));
        Assert.Equal(["false", "true"], Selected(cut));
    }

    [Fact]
    public async Task BottomPlacement_TitleFallsBackWhenTheActivePanelIsRemoved()
    {
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files"), new("Properties", "Details"))));
        await cut.FindAll("[role=tab]")[1].ClickAsync(new());
        Assert.Equal("Properties", TitleText(cut));

        cut.Render(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ChildContent, Panels(new Panel("Explorer", "Files"))));

        Assert.Equal("Explorer", TitleText(cut));
        Assert.Equal(["true"], Selected(cut));
    }

    [Fact]
    public void BottomPlacement_TitleFollowsAChangedTitle()
    {
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ChildContent, Panels(new Panel("Explorer", "Files"))));

        Assert.Equal("Explorer", TitleText(cut));

        cut.Render(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ChildContent, Panels(new Panel("Solution Explorer", "Files"))));

        Assert.Equal("Solution Explorer", TitleText(cut));
        Assert.Equal("Solution Explorer", cut.Find("[role=tab]").TextContent.Trim());
    }

    [Fact]
    public void BottomPlacement_ShowsTheActiveToolIcon()
    {
        var cut = RenderGroup(TabStripPlacement.Bottom,
            new Panel("Explorer", "Files", Icon: FlareIcons.Folder));

        Assert.Single(cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Title}")
            .QuerySelectorAll("svg"));
    }

    [Fact]
    public void BottomPlacement_RendersNoIconWhenTheToolHasNone()
    {
        var cut = RenderGroup(TabStripPlacement.Bottom, new Panel("Explorer", "Files"));

        Assert.Empty(cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Title}")
            .QuerySelectorAll("svg"));
    }

    [Fact]
    public void BottomPlacement_KeepsActionsInTheHeader()
    {
        var cut = Render(Group(TabStripPlacement.Bottom,
            Panels(new Panel("Explorer", "Files")), toolbar: Actions()));

        var header = cut.Find($".{Css.Classes.Ide.ToolPanelGroup.Header}");
        Assert.Equal(2, header.Children.Length);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Title, header.Children[0].ClassName);
        Assert.Equal(Css.Classes.Ide.ToolPanelGroup.Actions, header.Children[1].ClassName);
        Assert.Single(header.Children[1].QuerySelectorAll("button[aria-label='Pin panel']"));
        Assert.Empty(header.QuerySelectorAll("[role=tablist]"));
    }

    [Fact]
    public async Task BottomPlacement_KeyboardNavigationSelectsAndMovesTheTitle()
    {
        var cut = RenderGroup(TabStripPlacement.Bottom,
            new("One", "", Key: "one"), new("Two", "", Key: "two"), new("Three", "", Key: "three"));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("ArrowRight", rtl: false));

        Assert.Equal(["false", "true", "false"], Selected(cut));
        Assert.Equal("Two", TitleText(cut));

        await cut.InvokeAsync(() => cut.Instance.OnTabNavigation("End", rtl: false));

        Assert.Equal(["false", "false", "true"], Selected(cut));
        Assert.Equal("Three", TitleText(cut));
    }

    [Fact]
    public async Task BottomPlacement_KeepsTheTabAndPanelAriaRelationship()
    {
        var cut = RenderGroup(TabStripPlacement.Bottom,
            new("Explorer", "Files"), new("Properties", "Details"));

        var tabs = cut.FindAll("[role=tab]");
        var panels = cut.FindAll("[role=tabpanel]");
        Assert.Equal(2, tabs.Count);
        Assert.Equal(2, panels.Count);
        Assert.Equal(tabs[0].Id, panels[0].GetAttribute("aria-labelledby"));
        Assert.Equal(tabs[0].GetAttribute("aria-controls"), panels[0].Id);
        Assert.Equal("0", tabs[0].GetAttribute("tabindex"));
        Assert.Equal("-1", tabs[1].GetAttribute("tabindex"));
        Assert.Equal("horizontal", cut.Find("[role=tablist]").GetAttribute("aria-orientation"));
        Assert.Equal("Tool panels", cut.Find("[role=tablist]").GetAttribute("aria-label"));
    }

    [Fact]
    public async Task PlacementSwitch_KeepsOneTablistAndTheSelection()
    {
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Top)
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files"), new("Properties", "Details"))));
        await cut.FindAll("[role=tab]")[1].ClickAsync(new());

        cut.Render(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files"), new("Properties", "Details"))));

        Assert.Single(cut.FindAll("[role=tablist]"));
        Assert.Equal(["false", "true"], Selected(cut));
        Assert.Equal("Properties", TitleText(cut));
    }

    [Fact]
    public async Task BottomPlacement_ReportsTheSameSelectionContract()
    {
        string? reportedKey = null;
        string? reportedTitle = null;
        var cut = Render<FlareToolPanelGroup>(p => p
            .Add(x => x.TabStripPlacement, TabStripPlacement.Bottom)
            .Add(x => x.ActiveKey, "explorer")
            .Add(x => x.ActiveKeyChanged, key => reportedKey = key)
            .Add(x => x.OnActiveChanged, title => reportedTitle = title)
            .Add(x => x.ChildContent,
                Panels(new("Explorer", "Files", Key: "explorer"),
                    new("Properties", "Details", Key: "properties"))));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new());

        Assert.Equal("properties", reportedKey);
        Assert.Equal("Properties", reportedTitle);
        Assert.Equal("Explorer", TitleText(cut));
    }
}
