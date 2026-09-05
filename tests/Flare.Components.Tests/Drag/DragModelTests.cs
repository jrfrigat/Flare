using Flare.Components.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// Four surfaces implemented drag-and-drop independently and three of them did not work on a touch
/// screen at all, because native HTML5 drag-and-drop fires no event there. This is the one model that
/// replaces them, built on pointer events.
///
/// What is asserted here is the C# half: the contract the DOM is described with, and the two questions
/// the browser asks. The gesture itself - which element the pointer is over, where the insertion line
/// goes - lives in the browser by design (a call per pointermove would be a network round trip per
/// pointermove on Blazor Server), so it is verified in a real browser rather than here.
/// </summary>
public sealed class DragModelTests : FlareTestContext
{
    private sealed record Card(string Id, string Title);

    private static readonly Card _first = new("c1", "First");
    private static readonly Card _second = new("c2", "Second");

    // A board: two columns, the first holding both cards. Ids are explicit so a test can name an item
    // the way the browser would.
    private static RenderFragment Board(DropPlacement placement = DropPlacement.Between) => b =>
    {
        b.OpenComponent<FlareDropZone>(0);
        b.AddAttribute(1, nameof(FlareDropZone.Target), "todo");
        b.AddAttribute(2, nameof(FlareDropZone.Placement), placement);
        b.AddAttribute(3, nameof(FlareDropZone.ChildContent), (RenderFragment)(inner =>
        {
            Item(inner, 0, _first);
            Item(inner, 10, _second);
        }));
        b.CloseComponent();

        b.OpenComponent<FlareDropZone>(20);
        b.AddAttribute(21, nameof(FlareDropZone.Target), "done");
        b.AddAttribute(22, nameof(FlareDropZone.Placement), placement);
        b.CloseComponent();
    };

    private static void Item(RenderTreeBuilder b, int seq, Card card)
    {
        b.OpenComponent<FlareDraggable>(seq);
        b.AddAttribute(seq + 1, nameof(FlareDraggable.Id), card.Id);
        b.AddAttribute(seq + 2, nameof(FlareDraggable.Payload), card);
        b.AddAttribute(seq + 3, nameof(FlareDraggable.ChildContent),
            (RenderFragment)(inner => inner.AddContent(0, card.Title)));
        b.CloseComponent();
    }

    private IRenderedComponent<FlareDragContext<Card>> RenderBoard(
        Action<FlareDropEventArgs<Card>>? onDrop = null,
        Func<Card, string, bool>? canDrop = null,
        DropPlacement placement = DropPlacement.Between)
    {
        return Render<FlareDragContext<Card>>(p =>
        {
            p.Add(x => x.ChildContent, Board(placement));
            if (onDrop is not null) p.Add(x => x.OnDrop, onDrop);
            if (canDrop is not null) p.Add(x => x.CanDrop, canDrop);
        });
    }

    // The browser finds every draggable and every zone through these attributes - there is one gesture
    // for the whole context, not one registration per item, so the DOM is the whole interface.
    [Fact]
    public void TheDomDescribesWhatCanBeDraggedAndWhereItCanLand()
    {
        var cut = RenderBoard();

        var items = cut.FindAll("[data-flare-drag]");
        Assert.Equal(2, items.Count);
        Assert.Equal("c1", items[0].GetAttribute("data-flare-drag"));

        var zones = cut.FindAll("[data-flare-drop]");
        Assert.Equal(2, zones.Count);
        Assert.Equal("todo", zones[0].GetAttribute("data-flare-drop"));
        Assert.Equal("between", zones[0].GetAttribute("data-flare-drop-placement"));
    }

    // The context is not a box. A board that is a flex row must not gain a block between it and its
    // columns, which is the whole reason it renders display:contents rather than a wrapper.
    [Fact]
    public void TheContextRootCarriesItsOwnClassAndNothingElse()
    {
        var cut = RenderBoard();
        Assert.Equal(Css.Classes.Drag.Context, cut.Find($"div.{Css.Classes.Drag.Context}").ClassName);
    }

    [Fact]
    public async Task EveryZoneAcceptsByDefault()
    {
        var cut = RenderBoard();

        // null is the answer for "all of them" - the list would say the same thing and cost a marshal.
        Assert.Null(await cut.Instance.OnDragStartAsync("c1"));
    }

    [Fact]
    public async Task CanDropNarrowsTheTargetsBeforeTheDragIsVisible()
    {
        var cut = RenderBoard(canDrop: (card, target) => target != "done" || card.Id == "c2");

        var forFirst = await cut.Instance.OnDragStartAsync("c1");
        Assert.Equal(new[] { "todo" }, forFirst!.Allow);
        Assert.Null(await cut.Instance.OnDragStartAsync("c2"));
    }

    // The per-zone predicate and the context-wide one say the same kind of thing, and a zone that
    // refuses is refused whichever of the two said so.
    [Fact]
    public async Task AZoneCanRefuseOnItsOwn()
    {
        var cut = Render<FlareDragContext<Card>>(p => p
            .Add(x => x.ChildContent, b =>
            {
                b.OpenComponent<FlareDropZone>(0);
                b.AddAttribute(1, nameof(FlareDropZone.Target), "todo");
                b.AddAttribute(2, nameof(FlareDropZone.ChildContent),
                    (RenderFragment)(inner => Item(inner, 0, _first)));
                b.CloseComponent();

                b.OpenComponent<FlareDropZone>(20);
                b.AddAttribute(21, nameof(FlareDropZone.Target), "locked");
                b.AddAttribute(22, nameof(FlareDropZone.Accepts), (Func<object?, bool>)(_ => false));
                b.CloseComponent();
            }));

        var allowed = await cut.Instance.OnDragStartAsync("c1");
        Assert.Equal(new[] { "todo" }, allowed!.Allow);
    }

    // An unknown id is what a drag of an item that has just been removed looks like. It must refuse
    // every target rather than fall through to "all of them".
    [Fact]
    public async Task AnItemTheModelDoesNotKnowLandsNowhere()
    {
        var cut = RenderBoard();
        var allowed = await cut.Instance.OnDragStartAsync("gone");
        Assert.Empty(allowed!.Allow!);
    }

    [Fact]
    public async Task ADropReportsWhereItLandedAndWhereItCameFrom()
    {
        FlareDropEventArgs<Card>? drop = null;
        var cut = RenderBoard(onDrop: e => drop = e);

        await cut.InvokeAsync(() => cut.Instance.OnDropAsync("c1", "done", 0, "before", "c2"));

        Assert.NotNull(drop);
        Assert.Equal(_first, drop!.Payload);
        Assert.Equal("done", drop.TargetId);
        Assert.Equal("todo", drop.SourceTargetId);
        Assert.Equal(0, drop.Index);
        Assert.Equal(DropEdge.Before, drop.Edge);
        Assert.Equal(_second, drop.OverPayload);
        Assert.True(drop.HasOverPayload);
    }

    // A drop into a zone rather than between items: there is no item under the pointer, and the index
    // is the one place the caller can read.
    [Fact]
    public async Task ADropIntoAZoneCarriesNoItem()
    {
        FlareDropEventArgs<Card>? drop = null;
        var cut = RenderBoard(onDrop: e => drop = e, placement: DropPlacement.Into);

        await cut.InvokeAsync(() => cut.Instance.OnDropAsync("c1", "done", -1, "into", null));

        Assert.Equal(DropEdge.Into, drop!.Edge);
        Assert.Null(drop.OverPayload);
        Assert.False(drop.HasOverPayload);
        Assert.Equal(-1, drop.Index);
    }

    // An item declared with a payload of a different type is not this context's item. Handing the
    // caller a default in its place would be a silent lie, so the drag is refused instead.
    [Fact]
    public async Task AnItemOfAnotherTypeIsNotThisContextsItem()
    {
        var dropped = false;
        var cut = Render<FlareDragContext<Card>>(p => p
            .Add(x => x.OnDrop, _ => dropped = true)
            .Add(x => x.ChildContent, b =>
            {
                b.OpenComponent<FlareDropZone>(0);
                b.AddAttribute(1, nameof(FlareDropZone.Target), "todo");
                b.AddAttribute(2, nameof(FlareDropZone.ChildContent), (RenderFragment)(inner =>
                {
                    inner.OpenComponent<FlareDraggable>(0);
                    inner.AddAttribute(1, nameof(FlareDraggable.Id), "stranger");
                    inner.AddAttribute(2, nameof(FlareDraggable.Payload), 42);
                    inner.CloseComponent();
                }));
                b.CloseComponent();
            }));

        var allowed = await cut.Instance.OnDragStartAsync("stranger");
        Assert.Empty(allowed!.Allow!);
        await cut.InvokeAsync(() => cut.Instance.OnDropAsync("stranger", "todo", 0, "before", null));
        Assert.False(dropped);
    }

    // Group is the escape hatch for one context holding two unrelated sets of things: the browser reads
    // the group off the ITEM and only looks at zones that match it.
    [Fact]
    public void ItemsAndZonesInheritTheContextsGroupAndCanOverrideIt()
    {
        var cut = Render<FlareDragContext<Card>>(p => p
            .Add(x => x.Group, "cards")
            .Add(x => x.ChildContent, b =>
            {
                b.OpenComponent<FlareDropZone>(0);
                b.AddAttribute(1, nameof(FlareDropZone.Target), "todo");
                b.AddAttribute(2, nameof(FlareDropZone.ChildContent), (RenderFragment)(inner =>
                {
                    Item(inner, 0, _first);
                    inner.OpenComponent<FlareDraggable>(10);
                    inner.AddAttribute(11, nameof(FlareDraggable.Id), "note");
                    inner.AddAttribute(12, nameof(FlareDraggable.Group), "notes");
                    inner.CloseComponent();
                }));
                b.CloseComponent();
            }));

        Assert.Equal("cards", cut.Find("[data-flare-drop]").GetAttribute("data-flare-drag-group"));
        Assert.Equal("cards", cut.Find("[data-flare-drag='c1']").GetAttribute("data-flare-drag-group"));
        Assert.Equal("notes", cut.Find("[data-flare-drag='note']").GetAttribute("data-flare-drag-group"));
    }

    [Fact]
    public void ADisabledItemSaysSoInTheDom()
    {
        var cut = Render<FlareDragContext<Card>>(p => p
            .Add(x => x.ChildContent, b =>
            {
                b.OpenComponent<FlareDraggable>(0);
                b.AddAttribute(1, nameof(FlareDraggable.Id), "c1");
                b.AddAttribute(2, nameof(FlareDraggable.Disabled), true);
                b.CloseComponent();
            }));

        var item = cut.Find("[data-flare-drag]");
        Assert.Equal("true", item.GetAttribute("data-flare-drag-disabled"));
        Assert.Contains(Css.Classes.Drag.ItemDisabled, item.ClassName);
    }

    // An item removed from the board must not stay droppable: the registration is what the two interop
    // calls resolve against, and a stale entry would report a payload nobody can see any more.
    [Fact]
    public async Task RemovingAnItemUnregistersIt()
    {
        var show = true;
        var cut = Render<FlareDragContext<Card>>(p => p
            .Add(x => x.ChildContent, b =>
            {
                b.OpenComponent<FlareDropZone>(0);
                b.AddAttribute(1, nameof(FlareDropZone.Target), "todo");
                b.AddAttribute(2, nameof(FlareDropZone.ChildContent), (RenderFragment)(inner =>
                {
                    if (show) Item(inner, 0, _first);
                }));
                b.CloseComponent();
            }));

        Assert.Null(await cut.Instance.OnDragStartAsync("c1"));

        show = false;
        cut.Render();

        var afterRemoval = await cut.Instance.OnDragStartAsync("c1");
        Assert.Empty(afterRemoval!.Allow!);
    }

    // -- Keyboard reorder ----------------------------------------------------
    // None of the four drag surfaces had one, and a reorder only a pointer can perform is a control
    // half the readers cannot use. The browser is asked for the DOM order, because registration order
    // on this side is not render order once a list has been reordered.

    private const string Module = "./_content/Flare.Components/js/flare-dragdrop.js";

    private Bunit.BunitJSModuleInterop Board()
    {
        var module = JSInterop.SetupModule(Module);
        module.Setup<DragZoneOrder[]>("dragItemOrder", _ => true).SetResult(
        [
            new DragZoneOrder("todo", ["c1", "c2"]),
            new DragZoneOrder("done", []),
        ]);
        return module;
    }

    private static void Press(IRenderedComponent<FlareDragContext<Card>> cut, string id, string key) =>
        cut.Find($"[data-flare-drag='{id}']").KeyDown(new KeyboardEventArgs { Key = key });

    [Fact]
    public void EveryItemIsATabStopAndSaysWhatItIs()
    {
        var cut = RenderBoard();
        var item = cut.Find("[data-flare-drag='c1']");

        Assert.Equal("0", item.GetAttribute("tabindex"));
        Assert.Equal("false", item.GetAttribute("aria-grabbed"));
        Assert.False(string.IsNullOrWhiteSpace(item.GetAttribute("aria-roledescription")));
    }

    // The escape hatch for a list long enough that a tab stop per item is worse than no keyboard path.
    [Fact]
    public void KeyboardReorderCanBeTurnedOff()
    {
        var cut = Render<FlareDragContext<Card>>(p => p
            .Add(x => x.KeyboardReorder, false)
            .Add(x => x.ChildContent, Board(DropPlacement.Between)));

        Assert.Null(cut.Find("[data-flare-drag='c1']").GetAttribute("tabindex"));
    }

    [Fact]
    public void SpacePicksTheItemUp()
    {
        Board();
        var cut = RenderBoard();

        Press(cut, "c1", " ");

        var item = cut.Find("[data-flare-drag='c1']");
        Assert.Contains(Css.Classes.Drag.ItemPicked, item.ClassName);
        Assert.Equal("true", item.GetAttribute("aria-grabbed"));
    }

    // The arrows walk positions, and the position is announced because there is no preview to watch.
    [Fact]
    public void TheArrowsWalkThePositionsAndSayWhereTheyAre()
    {
        Board();
        var cut = RenderBoard();

        Press(cut, "c1", " ");
        Press(cut, "c1", "ArrowDown");

        // c1 out of the way leaves one other item in "todo", so "todo" holds two slots and "done" one.
        Assert.Contains("2", cut.Find("[role=status]").TextContent);
        Assert.Contains("3", cut.Find("[role=status]").TextContent);
    }

    [Fact]
    public async Task SpaceAgainDropsItWhereTheArrowsLeftIt()
    {
        Board();
        FlareDropEventArgs<Card>? drop = null;
        var cut = RenderBoard(onDrop: e => drop = e);

        Press(cut, "c1", " ");
        Press(cut, "c1", "ArrowDown");
        Press(cut, "c1", " ");
        await Task.Yield();

        // "todo" holds c2 once c1 is out of it, so it offers two positions: before c2 and after it.
        // One arrow from where c1 started is the second, which is past every item - hence no item to
        // land beside, and an index that is simply the length of what is left.
        Assert.NotNull(drop);
        Assert.Equal(_first, drop!.Payload);
        Assert.Equal("todo", drop.TargetId);
        Assert.Equal("todo", drop.SourceTargetId);
        Assert.Equal(1, drop.Index);
        Assert.False(drop.HasOverPayload);
        Assert.DoesNotContain(Css.Classes.Drag.ItemPicked, cut.Find("[data-flare-drag='c1']").ClassName);
    }

    // The last slot of the last zone is past every item, which is how an empty zone is reachable at all.
    [Fact]
    public async Task TheItemCanWalkIntoAnEmptyZone()
    {
        Board();
        FlareDropEventArgs<Card>? drop = null;
        var cut = RenderBoard(onDrop: e => drop = e);

        Press(cut, "c1", " ");
        for (var i = 0; i < 5; i++) Press(cut, "c1", "ArrowDown");
        Press(cut, "c1", " ");
        await Task.Yield();

        Assert.Equal("done", drop!.TargetId);
        Assert.Equal(0, drop.Index);
        Assert.False(drop.HasOverPayload);
    }

    [Fact]
    public void EscapeLetsItGoWithoutMovingAnything()
    {
        Board();
        var dropped = false;
        var cut = RenderBoard(onDrop: _ => dropped = true);

        Press(cut, "c1", " ");
        Press(cut, "c1", "ArrowDown");
        Press(cut, "c1", "Escape");

        Assert.False(dropped);
        Assert.DoesNotContain(Css.Classes.Drag.ItemPicked, cut.Find("[data-flare-drag='c1']").ClassName);
    }

    // A zone that refuses the payload is not a place the arrows can walk to either - the keyboard path
    // asks the same question the pointer path asks.
    [Fact]
    public async Task ARefusedZoneIsNotReachableByKeyboard()
    {
        Board();
        FlareDropEventArgs<Card>? drop = null;
        var cut = RenderBoard(onDrop: e => drop = e, canDrop: (_, target) => target != "done");

        Press(cut, "c1", " ");
        for (var i = 0; i < 5; i++) Press(cut, "c1", "ArrowDown");
        Press(cut, "c1", " ");
        await Task.Yield();

        Assert.Equal("todo", drop!.TargetId);
    }

}
