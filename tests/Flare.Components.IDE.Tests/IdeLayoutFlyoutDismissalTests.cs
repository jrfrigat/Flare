using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// An auto-hide flyout is dismissed like a tool window: a pointer press outside the floating body
/// and its rail collapses the region, Escape collapses it and returns focus to the active tool's
/// rail entry, and dismissal goes through the controlled collapsed state. Docked regions never
/// take part, and the JS listeners exist only while a flyout is open.
/// </summary>
public class IdeLayoutFlyoutDismissalTests : FlareTestContext
{
    private const string ModulePath = "./_content/Flare.Components.IDE/js/flare-ide.js";

    public IdeLayoutFlyoutDismissalTests() => Services.AddFlareIde();

    private sealed record Tool(string Title, string? Key, IdePanelMode Mode = IdePanelMode.AutoHide,
        ToolPanelPosition Position = ToolPanelPosition.Left);

    private static RenderFragment Panels(params Tool[] tools) => builder =>
    {
        var sequence = 0;
        foreach (var tool in tools)
        {
            builder.OpenComponent<FlareToolPanel>(sequence++);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Title), tool.Title);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Mode), tool.Mode);
            builder.AddAttribute(sequence++, nameof(FlareToolPanel.Position), tool.Position);
            if (tool.Key is not null)
                builder.AddAttribute(sequence++, nameof(FlareToolPanel.Key), tool.Key);
            builder.CloseComponent();
        }
    };

    private static RenderFragment Group(string? activeKey = "explorer", params Tool[] tools) => builder =>
    {
        builder.OpenComponent<FlareToolPanelGroup>(0);
        builder.AddAttribute(1, nameof(FlareToolPanelGroup.Position), ToolPanelPosition.Left);
        builder.AddAttribute(2, nameof(FlareToolPanelGroup.ShowTabStripWhenFloating), false);
        if (activeKey is not null)
            builder.AddAttribute(3, nameof(FlareToolPanelGroup.ActiveKey), activeKey);
        builder.AddAttribute(4, nameof(FlareToolPanelGroup.ChildContent), Panels(tools));
        builder.CloseComponent();
    };

    private static Tool[] TwoTools() =>
    [
        new("Explorer", "explorer"),
        new("Properties", "properties")
    ];

    private static string FloatingClass => $".{Css.Classes.Ide.Layout.PanelFloating}";
    private static string EntrySelector => $".{Css.Classes.Ide.Layout.ToolRailEntry}";

    [Fact]
    public async Task OutsidePress_CollapsesTheFloatingLeftRegionOnce()
    {
        bool? collapsed = null;
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => { collapsed = value; changes++; }))
            .Add(x => x.LeftToolPanel, Group(tools: TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Left"));

        Assert.True(collapsed);
        Assert.Equal(1, changes);
        Assert.Empty(cut.FindAll(FloatingClass));

        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Left"));

        Assert.Equal(1, changes);
    }

    [Fact]
    public async Task OutsidePress_CollapsesADefaultModeFlyout()
    {
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.LeftToolPanel, Panels(new Tool("Properties", "properties"))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Left"));

        Assert.True(collapsed);
        Assert.Empty(cut.FindAll(FloatingClass));
    }

    [Fact]
    public async Task OutsidePress_LeavesADockedRegion()
    {
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, _ => changes++))
            .Add(x => x.LeftToolPanel,
                Panels(new Tool("Explorer", "explorer", IdePanelMode.Docked))));

        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Left"));

        Assert.Equal(0, changes);
        Assert.Empty(cut.FindAll(FloatingClass));
    }

    [Fact]
    public async Task OutsidePress_LeavesARegionTheHostAlreadyCollapsed()
    {
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, _ => changes++))
            .Add(x => x.LeftToolPanel, Group(tools: TwoTools())));

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(EntrySelector).Count));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Left"));

        Assert.Equal(0, changes);
    }

    [Fact]
    public async Task OutsidePress_IgnoresAnUnknownSide()
    {
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, _ => changes++))
            .Add(x => x.LeftToolPanel, Panels(new Tool("Properties", "properties"))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Bottom"));

        Assert.Equal(0, changes);
    }

    [Fact]
    public async Task OutsidePress_CollapsesTheFloatingRightRegion()
    {
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.RightCollapsed, false)
            .Add(x => x.RightCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.RightToolPanel,
                Panels(new Tool("Properties", "properties", Position: ToolPanelPosition.Right))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Right"));

        Assert.True(collapsed);
        Assert.Empty(cut.FindAll(FloatingClass));
    }

    [Fact]
    public async Task Escape_CollapsesAndReturnsFocusToTheActiveRailEntry()
    {
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ToolRailMode, IdeToolRailMode.PerTool)
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftActiveToolKey, "properties")
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.LeftToolPanel, Group("properties", TwoTools())));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutEscapeAsync("Left"));

        Assert.True(collapsed);
        Assert.Empty(cut.FindAll(FloatingClass));
        cut.WaitForAssertion(() =>
        {
            var entries = cut.FindAll(EntrySelector);
            Assert.Equal(2, entries.Count);
            Assert.Equal(["false", "true"], entries.Select(entry => entry.GetAttribute("aria-pressed")));
        });
        cut.WaitForAssertion(() => Assert.Contains(JSInterop.Invocations,
            invocation => invocation.Identifier == "Blazor._internal.domWrapper.focus"));
    }

    [Fact]
    public async Task Escape_CollapsesOnlyOnce()
    {
        bool? collapsed = null;
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => { collapsed = value; changes++; }))
            .Add(x => x.LeftToolPanel, Panels(new Tool("Properties", "properties"))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutEscapeAsync("Left"));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutEscapeAsync("Left"));

        Assert.True(collapsed);
        Assert.Equal(1, changes);
    }

    [Fact]
    public async Task Escape_LeavesADockedRegion()
    {
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, _ => changes++))
            .Add(x => x.LeftToolPanel,
                Panels(new Tool("Explorer", "explorer", IdePanelMode.Docked))));

        await cut.InvokeAsync(() => cut.Instance.OnFlyoutEscapeAsync("Left"));

        Assert.Equal(0, changes);
    }

    [Fact]
    public async Task Escape_CollapsesTheFloatingRightRegion()
    {
        bool? collapsed = null;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.RightCollapsed, false)
            .Add(x => x.RightCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, value => collapsed = value))
            .Add(x => x.RightToolPanel,
                Panels(new Tool("Properties", "properties", Position: ToolPanelPosition.Right))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(FloatingClass)));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutEscapeAsync("Right"));

        Assert.True(collapsed);
        Assert.Empty(cut.FindAll(FloatingClass));
    }

    [Fact]
    public void FloatingRegion_RegistersAndClearsTheFlyoutInTheModule()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupVoid("setFlyout", _ => true).SetVoidResult();
        module.SetupVoid("clearFlyout", _ => true).SetVoidResult();

        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, false)
            .Add(x => x.LeftToolPanel, Panels(new Tool("Properties", "properties"))));

        cut.WaitForAssertion(() => module.VerifyInvoke("setFlyout"));

        cut.Render(p => p
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel, Panels(new Tool("Properties", "properties"))));

        cut.WaitForAssertion(() => module.VerifyInvoke("clearFlyout"));
    }

    [Fact]
    public void CollapsedRegion_DoesNotRegisterAFlyout()
    {
        var module = JSInterop.SetupModule(ModulePath);

        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.LeftCollapsed, true)
            .Add(x => x.LeftToolPanel, Panels(new Tool("Properties", "properties"))));

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.Rail}")));
        Assert.Empty(module.Invocations);
    }
}
