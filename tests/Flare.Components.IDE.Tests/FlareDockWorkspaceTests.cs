using Microsoft.AspNetCore.Components.Web;
using System.Reflection;
using Flare.Components;
using Flare.Components.IDE;
using Flare.Css;
using Flare.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public sealed class FlareDockWorkspaceTests : FlareTestContext
{
    private static readonly DockItem[] Catalog = [new("a", "A", true), new("b", "B"), new("c", "C")];

    [Fact]
    public void SplittingAndCollapsingPreservesTheSameActivePane()
    {
        var original = new DockSplit("split", DockAxis.Vertical,
            new DockGroup("first", ["a", "b"], "a"), new DockGroup("last", ["c"], "c"));
        DockLayoutChange? change = null;
        var cut = RenderWorkspace(original, requested => change = requested);
        var before = cut.FindComponents<Probe>().Single(x => x.Instance.ItemKey == "a").Instance;

        FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!.Click();
        cut.FindAll(".flare-dock__actions button").Single(x => x.TextContent.Trim() == "Split beside A").Click();
        Assert.Equal(DockAction.Split, change!.Action);
        Assert.Same(original, change.Previous);
        var proposed = Assert.IsType<DockSplit>(change.Proposed);
        var nested = Assert.IsType<DockSplit>(proposed.First);
        Assert.Equal(DockAxis.Horizontal, nested.Axis);
        Assert.Same(original.Second, proposed.Second);
        Assert.Single(cut.FindAll("[role=separator]"));

        cut.Render(p => p.Add(x => x.Layout, proposed));
        Assert.Equal(2, cut.FindAll("[role=separator]").Count);
        Assert.Same(before, cut.FindComponents<Probe>().Single(x => x.Instance.ItemKey == "a").Instance);

        FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!.Click();
        cut.FindAll(".flare-dock__actions button").Single(x => x.TextContent.Trim() == "Move to C").Click();
        Assert.Equal(DockAction.Move, change.Action);
        cut.Render(p => p.Add(x => x.Layout, change.Proposed));
        Assert.Single(cut.FindAll("[role=separator]"));
        Assert.Same(before, cut.FindComponents<Probe>().Single(x => x.Instance.ItemKey == "a").Instance);
    }

    [Fact]
    public void RetainsOnlyVisitedItemsAndDisposesAfterAcceptedClose()
    {
        var group = new DockGroup("first", ["a", "b"], "a");
        DockLayoutChange? change = null;
        var cut = RenderWorkspace(group, requested => change = requested);
        var before = cut.FindComponents<Probe>().Single().Instance;
        cut.Find("button[role=tab][aria-label=B]").Click();
        cut.Render(p => p.Add(x => x.Layout, change!.Proposed));
        Assert.Equal(2, cut.FindComponents<Probe>().Count);
        Assert.Single(cut.FindAll("[role=tabpanel][hidden]"));
        Assert.False(before.Disposed);

        cut.Find("button[role=tab][aria-label=A]").Click();
        var selectA = change!.Proposed;
        cut.Render(p => p.Add(x => x.Layout, selectA));
        cut.Find("button[aria-label='Close A']").Click();
        Assert.False(before.Disposed);
        cut.Render(p => p.Add(x => x.Layout, change.Proposed));
        Assert.True(before.Disposed);
        Assert.Equal("B", cut.Find("button[aria-selected=true]").GetAttribute("aria-label"));
    }

    [Fact]
    public void SplittersLeaveDirectionToTheSharedSplitter()
    {
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a"], "a"), new DockGroup("last", ["b"], "b"));
        var cut = RenderWorkspace(layout, _ => { });

        var splitter = Assert.Single(cut.FindComponents<FlareSplitter>());
        Assert.False(splitter.Instance.Reverse);
    }

    [Fact]
    public void TabsExposeAPointerDragHandle()
    {
        var group = new DockGroup("first", ["a", "b"], "a");
        var cut = RenderWorkspace(group, _ => { });

        var tabs = cut.FindAll("button[role=tab]");
        Assert.Equal(2, tabs.Count);
        Assert.All(tabs, tab => Assert.NotNull(tab.QuerySelector($"[data-{Css.Classes.Drag.Handle}]")));
        Assert.All(tabs, tab => Assert.NotNull(tab.QuerySelector($".{Css.Classes.Drag.Handle}")));
    }

    [Fact]
    public void RejectsDuplicateOrMissingKeysAndKeepsTheAcceptedLayout()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a"], "a"), _ => { });
        var before = cut.FindComponents<Probe>().Single().Instance;
        cut.Render(p => p.Add(x => x.Layout,
            new DockSplit("split", DockAxis.Horizontal,
                new DockGroup("first", ["a"], "a"), new DockGroup("last", ["a"], "a"))));
        Assert.Single(cut.FindAll("[role=tab]"));
        Assert.Contains("invalid", cut.Find("[role=status]").TextContent, StringComparison.OrdinalIgnoreCase);
        cut.Render(p => p.Add(x => x.Layout, new DockGroup("first", ["missing"])));
        Assert.Same(before, cut.FindComponents<Probe>().Single().Instance);
    }

    [Fact]
    public async Task DropsATabBeforeATabInAnotherGroup()
    {
        DockLayoutChange? change = null;
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a", "b"], "a"), new DockGroup("last", ["c"], "c"));
        var cut = RenderWorkspace(layout, requested => change = requested);
        var context = cut.FindComponent<FlareDragContext<string>>();

        var source = cut.FindAll("[data-flare-drag]")
            .Single(x => x.QuerySelector("button[aria-label=A]") is not null)
            .GetAttribute("data-flare-drag");
        var header = FindHeader(cut, "C");
        var row = header.QuerySelector("[data-flare-drop]")!.GetAttribute("data-flare-drop");
        var over = header.QuerySelectorAll("[data-flare-drag]")
            .Single(x => x.QuerySelector("button[aria-label=C]") is not null)
            .GetAttribute("data-flare-drag");

        await cut.InvokeAsync(() => context.Instance.OnDragStartAsync(source!));
        await cut.InvokeAsync(() => context.Instance.OnDropAsync(source!, row!, 0, "before", over));

        Assert.Equal(DockAction.Move, change!.Action);
        var proposed = Assert.IsType<DockSplit>(change.Proposed);
        Assert.Equal(new[] { "a", "c" }, Assert.IsType<DockGroup>(proposed.Second).Items.ToArray());
    }

    [Fact]
    public async Task DropsATabOntoTheStartZoneOfAnotherGroup()
    {
        DockLayoutChange? change = null;
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a", "b"], "a"), new DockGroup("last", ["c"], "c"));
        var cut = RenderWorkspace(layout, requested => change = requested);
        var context = cut.FindComponent<FlareDragContext<string>>();

        var source = cut.FindAll("[data-flare-drag]")
            .Single(x => x.QuerySelector("button[aria-label=A]") is not null)
            .GetAttribute("data-flare-drag");
        var header = FindHeader(cut, "C");
        const string marker = "-group-";
        var headerId = header.Id!;
        var suffix = headerId[(headerId.IndexOf(marker, StringComparison.Ordinal) + marker.Length)..];
        var zone = cut.FindAll("[data-flare-drop]")
            .Single(x => x.GetAttribute("data-flare-drop")!.EndsWith($"-{suffix}-start", StringComparison.Ordinal))
            .GetAttribute("data-flare-drop");

        await cut.InvokeAsync(() => context.Instance.OnDragStartAsync(source!));
        await cut.InvokeAsync(() => context.Instance.OnDropAsync(source!, zone!, -1, "into", null));

        Assert.Equal(DockAction.Split, change!.Action);
        var proposed = Assert.IsType<DockSplit>(change.Proposed);
        var target = Assert.IsType<DockSplit>(proposed.Second);
        Assert.Equal(new[] { "a" }, Assert.IsType<DockGroup>(target.First).Items.ToArray());
        Assert.Equal(new[] { "c" }, Assert.IsType<DockGroup>(target.Second).Items.ToArray());
    }

    [Fact]
    public async Task DeleteClosesTheFocusedTab()
    {
        DockLayoutChange? change = null;
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b"], "a"), requested => change = requested);

        await cut.Find("button[role=tab][aria-label=B]")
            .TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Delete" });

        Assert.Equal(DockAction.Close, change!.Action);
        Assert.Equal("b", change.ItemKey);
        Assert.Equal(new[] { "a" }, Assert.IsType<DockGroup>(change.Proposed).Items.ToArray());
    }

    [Fact]
    public async Task ArrowKeysMoveFocusBetweenTabs()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components.IDE/js/flare-dock.js");
        module.SetupVoid("register");
        module.SetupVoid("focusTab");
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b", "c"], "a"), _ => { });
        var tabA = cut.Find("button[role=tab][aria-label=A]");
        var tabB = cut.Find("button[role=tab][aria-label=B]");

        await tabA.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "ArrowRight" });

        cut.WaitForAssertion(() => Assert.Contains(module.Invocations,
            invocation => invocation.Identifier == "focusTab" && (string?)invocation.Arguments[1] == tabB.Id));
    }

    [Fact]
    public void HeaderKeepsTheThemesHeightAndThePaneReadsTheMeasuredValue()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a"], "a"), _ => { });

        var header = cut.Find(".flare-dock__header");
        Assert.NotNull(header.GetAttribute("data-flare-dock-header"));
        Assert.Contains("block-size:auto", header.GetAttribute("style"));

        var pane = cut.Find(".flare-dock__pane");
        Assert.Contains("-header", pane.GetAttribute("style"));
        Assert.DoesNotContain("flare-dock-header-height", cut.Markup);
    }

    [Fact]
    public void MovesATabBeforeASpecificTabInAnotherGroup()
    {
        DockLayoutChange? change = null;
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a", "b"], "a"), new DockGroup("last", ["c"], "c"));
        var cut = RenderWorkspace(layout, requested => change = requested);

        FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!.Click();
        cut.FindAll(".flare-dock__actions button").Single(x => x.TextContent.Trim() == "Move before C").Click();

        Assert.Equal(DockAction.Move, change!.Action);
        Assert.Equal("last", change.TargetKey);
        var proposed = Assert.IsType<DockSplit>(change.Proposed);
        var last = Assert.IsType<DockGroup>(proposed.Second);
        Assert.Equal(new[] { "a", "c" }, last.Items.ToArray());
        Assert.Equal("a", last.ActiveKey);
    }

    [Fact]
    public void TheActionsMenuIsLinkedToItsButtonAndClosesOnEscape()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b"], "a"), _ => { });
        var toggle = FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!;
        toggle.Click();

        var menu = cut.Find("[data-flare-dock-actions]");
        Assert.Equal(toggle.GetAttribute("aria-controls"), menu.Id);

        menu.KeyDown(Key.Escape);
        Assert.Empty(cut.FindAll("[data-flare-dock-actions]"));
    }

    [Fact]
    public void EscapeReturnsFocusToTheActionsButton()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components.IDE/js/flare-dock.js");
        module.SetupVoid("register");
        module.SetupVoid("focusElement");
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b"], "a"), _ => { });
        var toggle = FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!;
        var toggleId = toggle.Id;
        toggle.Click();

        cut.Find("[data-flare-dock-actions]").KeyDown(Key.Escape);

        cut.WaitForAssertion(() => Assert.Contains(module.Invocations,
            invocation => invocation.Identifier == "focusElement" && (string?)invocation.Arguments[1] == toggleId));
    }

    [Fact]
    public void RendersOnlyTheRequestedBodyZones()
    {
        var group = new DockGroup("first", ["a"], "a");

        var configured = RenderWorkspace(group, _ => { }, dropZones: DockDropZones.Start | DockDropZones.End);
        Assert.Equal(new[] { "end", "start" },
            configured.FindAll("[data-flare-dock-zone]")
                .Select(zone => zone.GetAttribute("data-flare-dock-zone")!)
                .OrderBy(zone => zone, StringComparer.Ordinal));

        var none = RenderWorkspace(group, _ => { }, dropZones: DockDropZones.None);
        Assert.Empty(none.FindAll("[data-flare-dock-zone]"));

        var all = RenderWorkspace(group, _ => { });
        Assert.Equal(5, all.FindAll("[data-flare-dock-zone]").Count);
    }

    [Fact]
    public void TheActionsMenuFollowsTheEnabledZones()
    {
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a", "b"], "a"), new DockGroup("last", ["c"], "c"));
        var cut = RenderWorkspace(layout, _ => { },
            dropZones: DockDropZones.Start | DockDropZones.End);

        FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!.Click();
        var items = cut.FindAll(".flare-dock__actions button").Select(button => button.TextContent.Trim()).ToArray();

        Assert.Contains("Split before C", items);
        Assert.Contains("Split beside C", items);
        Assert.DoesNotContain("Split above C", items);
        Assert.DoesNotContain("Split below C", items);
        Assert.DoesNotContain("Move to C", items);
        Assert.Contains("Move before C", items);
        Assert.Contains("Move after C", items);
    }

    [Fact]
    public void TheCenterZoneControlsMoveToButNotRowMoves()
    {
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a", "b"], "a"), new DockGroup("last", ["c"], "c"));
        var cut = RenderWorkspace(layout, _ => { }, dropZones: DockDropZones.Into);

        FindHeader(cut, "A").QuerySelector("button[aria-expanded]")!.Click();
        var items = cut.FindAll(".flare-dock__actions button").Select(button => button.TextContent.Trim()).ToArray();

        Assert.Contains("Move to C", items);
        Assert.Contains("Move before C", items);
        Assert.Contains("Move after C", items);
        Assert.DoesNotContain(items, item => item.StartsWith("Split ", StringComparison.Ordinal));
    }

    [Fact]
    public void NoneKeepsTheTabRowAndNoBodyZones()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b"], "a"), _ => { },
            dropZones: DockDropZones.None);

        Assert.Single(cut.FindAll("[data-flare-dock-tabs]"));
        Assert.Empty(cut.FindAll("[data-flare-dock-zone]"));
        Assert.NotEmpty(cut.FindAll("[data-flare-drop]"));
    }

    [Fact]
    public void RejectsANodeTypeDerivedOutsideTheLayoutModel()
    {
        var cut = RenderWorkspace(new CustomNode(new DockGroup("first", ["a"], "a")), _ => { });

        Assert.Contains("invalid", cut.Find("[role=status]").TextContent, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record CustomNode : DockNode
    {
        public CustomNode(DockNode original) : base(original) { }
    }

    [Fact]
    public void RemovingAnItemThatIsNotInTheGroupKeepsIt()
    {
        var remove = typeof(FlareDockWorkspace).Assembly
            .GetType("Flare.Components.IDE.DockLayoutOperations")!
            .GetMethod("Remove", BindingFlags.NonPublic | BindingFlags.Static)!;
        var group = new DockGroup("g", ["a"]);

        Assert.Same(group, remove.Invoke(null, [group, "missing"]));
    }

    [Fact]
    public void NamesAGroupByItsFirstTabAndNotByTheSelection()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b"]), _ => { });
        Assert.Equal("A", cut.Find("[role=tablist]").GetAttribute("aria-label"));

        cut.Render(p => p.Add(x => x.Layout, new DockGroup("first", ["a", "b"], "b")));
        Assert.Equal("A", cut.Find("[role=tablist]").GetAttribute("aria-label"));
    }

    [Fact]
    public void OmitsTheResizeHandleWhenResizeCannotBeRequested()
    {
        var layout = new DockSplit("split", DockAxis.Horizontal,
            new DockGroup("first", ["a"], "a"), new DockGroup("last", ["b"], "b"));

        var denied = RenderWorkspace(layout, _ => { }, (action, _, _) => action != DockAction.Resize);
        Assert.Empty(denied.FindAll("[role=separator]"));

        var readOnly = RenderWorkspace(layout, null);
        Assert.Empty(readOnly.FindAll("[role=separator]"));
    }

    [Fact]
    public void TabMetadata_RendersIconAndBadgesInsideTheButton()
    {
        var items = new DockItem[]
        {
            new("a", "A", Icon: FlareIcons.Folder, Badge: "3"),
            new("b", "B", BadgeDot: true)
        };
        var cut = RenderWorkspace(new DockGroup("first", ["a", "b"], "a"), _ => { }, items: items);

        var tabA = cut.Find("button[role=tab][aria-label=A]");
        Assert.NotEmpty(tabA.QuerySelectorAll($".{Css.Classes.Icon.Root}"));
        Assert.Contains("3", tabA.TextContent);
        Assert.NotEmpty(tabA.QuerySelectorAll($".{Css.Classes.Tabs.Badge}"));

        var tabB = cut.Find("button[role=tab][aria-label=B]");
        Assert.NotEmpty(tabB.QuerySelectorAll($".{Css.Classes.Tabs.BadgeDot}"));
    }

    [Fact]
    public void TabActions_RenderBesideTheTabButtonAsRealControls()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a"], "a"), _ => { },
            tabActions: item => builder =>
            {
                builder.OpenElement(0, "button");
                builder.AddAttribute(1, "id", "action-" + item.Key);
                builder.CloseElement();
            });

        var action = cut.Find("#action-a");
        Assert.Equal("button", action.TagName.ToLowerInvariant());
        Assert.Null(action.Closest("button[role=tab]"));
        Assert.Contains("flare-dock__tab-actions", action.ParentElement!.ClassName ?? "");
    }

    [Fact]
    public void TabMarkers_RenderInsideTheTabButton()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a"], "a"), _ => { },
            tabMarkers: item => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "id", "marker-" + item.Key);
                builder.CloseElement();
            });

        var marker = cut.Find("#marker-a");
        Assert.NotNull(marker.Closest("button[role=tab]"));
    }

    [Fact]
    public void ShowDropGuideFalse_KeepsTheZonesAndSuppressesTheBaseGuide()
    {
        var cut = RenderWorkspace(new DockGroup("first", ["a"], "a"), _ => { }, showDropGuide: false);

        Assert.Contains("flare-dock--no-guide", cut.Find(".flare-dock").ClassName);
        Assert.NotEmpty(cut.FindAll(".flare-dock__target"));
    }

    private IRenderedComponent<FlareDockWorkspace> RenderWorkspace(DockNode layout, Action<DockLayoutChange>? onChange,
        Func<DockAction, string?, string, bool>? canRequest = null,
        DockDropZones dropZones = DockDropZones.All,
        IReadOnlyList<DockItem>? items = null,
        RenderFragment<DockItem>? tabTemplate = null,
        RenderFragment<DockItem>? tabActions = null,
        RenderFragment<DockItem>? tabMarkers = null,
        bool showDropGuide = true) =>
        Render<FlareDockWorkspace>(p =>
        {
            p.Add(x => x.Layout, layout)
                .Add(x => x.Items, items ?? Catalog)
                .Add(x => x.DropZones, dropZones)
                .Add(x => x.ShowDropGuide, showDropGuide)
                .Add(x => x.ContentTemplate, (RenderFragment<DockItem>)(item => builder =>
                {
                    builder.OpenComponent<Probe>(0);
                    builder.AddAttribute(1, nameof(Probe.ItemKey), item.Key);
                    builder.CloseComponent();
                }));
            if (tabTemplate is not null) p.Add(x => x.TabTemplate, tabTemplate);
            if (tabActions is not null) p.Add(x => x.TabActionsTemplate, tabActions);
            if (tabMarkers is not null) p.Add(x => x.TabMarkerTemplate, tabMarkers);
            if (onChange is not null)
                p.Add(x => x.OnChangeRequested, EventCallback.Factory.Create(this, onChange));
            if (canRequest is not null)
                p.Add(x => x.CanRequest, canRequest);
        });

    private static AngleSharp.Dom.IElement FindHeader(IRenderedComponent<FlareDockWorkspace> cut, string label) =>
        cut.FindAll(".flare-dock__header").Single(x => x.QuerySelector($"[role=tablist][aria-label={label}]") is not null);

    public sealed class Probe : ComponentBase, IDisposable
    {
        [Parameter] public string ItemKey { get; set; } = "";
        public bool Disposed { get; private set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ItemKey);
        public void Dispose() => Disposed = true;
    }
}
