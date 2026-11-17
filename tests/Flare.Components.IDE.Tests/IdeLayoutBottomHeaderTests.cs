using System.Globalization;
using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The bottom region's tool-window contract: an opt-in header with typed pin and collapse controls,
/// a controlled <see cref="FlareIdeLayout.BottomPinned"/> state that turns the region into an
/// auto-hide flyout, the existing collapse/rail contract, and no breakage without a bottom panel.
/// </summary>
public class IdeLayoutBottomHeaderTests : FlareTestContext
{
    public IdeLayoutBottomHeaderTests() => Services.AddFlareIde();

    private static RenderFragment Content() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "id", "bottom-content");
        builder.AddContent(2, "Output");
        builder.CloseElement();
    };

    private static RenderFragment HeaderContent() => builder =>
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "id", "bottom-label");
        builder.AddContent(2, "Output");
        builder.CloseElement();
    };

    private static string Header => $".{Css.Classes.Ide.Layout.BottomHeader}";
    private static string Actions => $".{Css.Classes.Ide.Layout.BottomHeaderActions}";
    private static string Rail => $".{Css.Classes.Ide.Layout.Rail}";
    private static string Floating => $".{Css.Classes.Ide.Layout.PanelFloating}";
    private static string BottomResizer => $"[role=separator][data-region=\"Bottom\"]";

    private IRenderedComponent<FlareIdeLayout> RenderBottom(
        bool showHeader = false, bool pinned = true, bool collapsed = false,
        EventCallback<bool>? pinnedChanged = null, EventCallback<bool>? collapsedChanged = null,
        bool showPin = true, bool showCollapse = true, RenderFragment? headerContent = null,
        string height = "200px", RenderFragment? bottom = null) =>
        Render<FlareIdeLayout>(p => p
            .Add(x => x.BottomPanel, bottom ?? Content())
            .Add(x => x.ShowBottomHeader, showHeader)
            .Add(x => x.ShowBottomPin, showPin)
            .Add(x => x.ShowBottomCollapse, showCollapse)
            .Add(x => x.BottomHeaderContent, headerContent)
            .Add(x => x.BottomPinned, pinned)
            .Add(x => x.BottomPinnedChanged, pinnedChanged ?? default)
            .Add(x => x.BottomCollapsed, collapsed)
            .Add(x => x.BottomCollapsedChanged, collapsedChanged ?? default)
            .Add(x => x.BottomPanelHeight, height));

    [Fact]
    public void BottomHeader_IsOffByDefaultAndKeepsTheHistoricalBody()
    {
        var cut = RenderBottom();

        Assert.Empty(cut.FindAll(Header));
        var body = cut.Find($".{Css.Classes.Ide.Layout.Bottom} .{Css.Classes.Ide.Layout.PanelBody}");
        Assert.Equal(Css.Classes.Ide.Layout.PanelBody, body.ClassName);
        Assert.Equal("Output", cut.Find("#bottom-content").TextContent);
    }

    [Fact]
    public void BottomHeader_RendersHostContentAndTypedControls()
    {
        var cut = RenderBottom(showHeader: true, headerContent: HeaderContent());

        var header = cut.Find(Header);
        Assert.Equal(2, header.Children.Length);
        Assert.Equal(Css.Classes.Ide.Layout.BottomHeaderContent, header.Children[0].ClassName);
        Assert.Equal("bottom-label", header.Children[0].FirstElementChild!.Id);
        Assert.Equal(Css.Classes.Ide.Layout.BottomHeaderActions, header.Children[1].ClassName);

        var actions = cut.Find(Actions);
        Assert.Equal("toolbar", actions.GetAttribute("role"));
        Assert.Equal("Bottom panel actions", actions.GetAttribute("aria-label"));

        var pin = actions.QuerySelector("button[aria-label='Pin panel']")!;
        Assert.Equal("true", pin.GetAttribute("aria-pressed"));
        Assert.Single(pin.QuerySelectorAll("svg"));
        Assert.Contains(Css.Classes.Ide.Layout.BottomPinIcon, pin.QuerySelector("svg")!.ClassList);
        Assert.DoesNotContain(Css.Classes.Ide.Layout.BottomPinUnpinned,
            pin.QuerySelector("svg")!.ClassList);

        var collapse = actions.QuerySelector("button[aria-label='Collapse']")!;
        Assert.Single(collapse.QuerySelectorAll("svg"));
        Assert.Equal("Output", cut.Find("#bottom-label").TextContent);
        Assert.Equal("Output", cut.Find("#bottom-content").TextContent);
    }

    [Fact]
    public void BottomHeader_UsesTheLocalizedAccessibleNames()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            var cut = RenderBottom(showHeader: true);

            var actions = cut.Find(Actions);
            Assert.Equal("Действия нижней панели", actions.GetAttribute("aria-label"));
            Assert.NotNull(actions.QuerySelector("button[aria-label='Закрепить панель']"));
            Assert.NotNull(actions.QuerySelector("button[aria-label='Свернуть']"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public void BottomHeader_CanHideEachTypedControlAndTheWholeHeader()
    {
        var noControls = RenderBottom(showHeader: true, showPin: false, showCollapse: false,
            headerContent: HeaderContent());
        Assert.NotNull(noControls.Find("#bottom-label"));
        Assert.Empty(noControls.FindAll(Actions));

        var empty = RenderBottom(showHeader: true, showPin: false, showCollapse: false);
        Assert.Empty(empty.FindAll(Header));
    }

    [Fact]
    public void BottomHeader_IsNotRenderedWhileCollapsed()
    {
        var cut = RenderBottom(showHeader: true, collapsed: true);

        Assert.Empty(cut.FindAll(Header));
        Assert.Single(cut.FindAll(Rail));
        Assert.Empty(cut.FindAll("#bottom-content"));
    }

    [Fact]
    public async Task BottomPin_UnpinsToAnAutoHideFlyoutAndReports()
    {
        bool? reported = null;
        var cut = RenderBottom(showHeader: true,
            pinnedChanged: EventCallback.Factory.Create<bool>(this, value => reported = value));

        await cut.Find("button[aria-label='Pin panel']").ClickAsync(new());

        Assert.False(reported);
        Assert.Single(cut.FindAll(Floating));
        Assert.Empty(cut.FindAll(BottomResizer));
        Assert.Single(cut.FindAll(Rail));
        Assert.NotNull(cut.Find(Header));
        Assert.Equal("false",
            cut.Find("button[aria-label='Pin panel']").GetAttribute("aria-pressed"));
        Assert.Contains(Css.Classes.Ide.Layout.BottomPinUnpinned,
            cut.Find("button[aria-label='Pin panel'] svg").ClassList);
        Assert.Equal("Output", cut.Find("#bottom-content").TextContent);
    }

    [Fact]
    public async Task BottomPin_ReportsTheChangeAndHonoursAChangedParameter()
    {
        bool? reported = null;
        var cut = RenderBottom(showHeader: true, pinned: true,
            pinnedChanged: EventCallback.Factory.Create<bool>(this, value => reported = value));

        await cut.Find("button[aria-label='Pin panel']").ClickAsync(new());

        Assert.False(reported);
        Assert.Single(cut.FindAll(Floating));
        Assert.Empty(cut.FindAll(BottomResizer));

        cut.Render(p => p
            .Add(x => x.BottomPanel, Content())
            .Add(x => x.ShowBottomHeader, true)
            .Add(x => x.BottomPinned, true)
            .Add(x => x.BottomPinnedChanged,
                EventCallback.Factory.Create<bool>(this, value => reported = value)));

        Assert.Empty(cut.FindAll(Floating));
        Assert.Single(cut.FindAll(BottomResizer));

        cut.Render(p => p
            .Add(x => x.BottomPanel, Content())
            .Add(x => x.ShowBottomHeader, true)
            .Add(x => x.BottomPinned, false)
            .Add(x => x.BottomPinnedChanged,
                EventCallback.Factory.Create<bool>(this, value => reported = value)));

        Assert.Single(cut.FindAll(Floating));
        Assert.Empty(cut.FindAll(BottomResizer));
    }

    [Fact]
    public void BottomPin_KeepsTheConfiguredHeightAndDockedResizer()
    {
        var cut = RenderBottom(showHeader: true, height: "240px");

        var root = cut.Find($".{Css.Classes.Ide.Layout.Root}");
        Assert.Contains($"{Css.Tokens.Ide.Layout.BottomHeight}:240px", root.GetAttribute("style"));
        Assert.Single(cut.FindAll(BottomResizer));
    }

    [Fact]
    public async Task BottomCollapse_HidesToTheRailAndTheRailReopens()
    {
        bool? reported = null;
        var cut = RenderBottom(showHeader: true, pinned: true,
            collapsedChanged: EventCallback.Factory.Create<bool>(this, value => reported = value));

        await cut.Find("button[aria-label='Collapse']").ClickAsync(new());

        Assert.True(reported);
        Assert.Empty(cut.FindAll(Header));
        Assert.Empty(cut.FindAll("#bottom-content"));
        Assert.Single(cut.FindAll(Rail));
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelCollapsed}"));

        await cut.Find(Rail).ClickAsync(new());

        Assert.False(reported);
        Assert.NotNull(cut.Find(Header));
        Assert.Equal("Output", cut.Find("#bottom-content").TextContent);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelCollapsed}"));
    }

    [Fact]
    public async Task BottomFlyout_OutsidePressAndEscapeCollapseTheUnpinnedRegion()
    {
        bool? outside = null;
        var cut = RenderBottom(showHeader: true, pinned: false,
            collapsedChanged: EventCallback.Factory.Create<bool>(this, value => outside = value));
        Assert.Single(cut.FindAll(Floating));

        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Bottom"));

        Assert.True(outside);
        Assert.Empty(cut.FindAll(Floating));
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelCollapsed}"));

        bool? escape = null;
        var second = RenderBottom(showHeader: true, pinned: false,
            collapsedChanged: EventCallback.Factory.Create<bool>(this, value => escape = value));
        Assert.Single(second.FindAll(Floating));

        await second.InvokeAsync(() => second.Instance.OnFlyoutEscapeAsync("Bottom"));

        Assert.True(escape);
        Assert.Empty(second.FindAll(Floating));
    }

    [Fact]
    public async Task BottomFlyout_OutsidePressLeavesAPinnedRegion()
    {
        var changes = 0;
        var cut = RenderBottom(showHeader: true, pinned: true,
            collapsedChanged: EventCallback.Factory.Create<bool>(this, _ => changes++));

        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Bottom"));

        Assert.Equal(0, changes);
        Assert.Empty(cut.FindAll(Floating));
        Assert.NotNull(cut.Find(Header));
    }

    [Fact]
    public async Task BottomFlyout_DoesNothingWithoutABottomPanel()
    {
        var changes = 0;
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.ShowBottomHeader, true)
            .Add(x => x.BottomPinned, false)
            .Add(x => x.BottomCollapsedChanged,
                EventCallback.Factory.Create<bool>(this, _ => changes++)));

        await cut.InvokeAsync(() => cut.Instance.OnFlyoutOutsidePressAsync("Bottom"));
        await cut.InvokeAsync(() => cut.Instance.OnFlyoutEscapeAsync("Bottom"));

        Assert.Equal(0, changes);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.Bottom}"));
        Assert.Empty(cut.FindAll(Header));
        Assert.Empty(cut.FindAll(Rail));
    }

    [Fact]
    public async Task BottomPin_RepinsAnOpenFlyoutBackToDocked()
    {
        var cut = RenderBottom(showHeader: true, pinned: false);
        Assert.Single(cut.FindAll(Floating));

        await cut.Find("button[aria-label='Pin panel']").ClickAsync(new());

        Assert.Empty(cut.FindAll(Floating));
        Assert.Single(cut.FindAll(BottomResizer));
        Assert.Equal("true",
            cut.Find("button[aria-label='Pin panel']").GetAttribute("aria-pressed"));
        Assert.Equal("Output", cut.Find("#bottom-content").TextContent);
    }
}
