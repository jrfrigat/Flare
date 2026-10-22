using AngleSharp.Dom;
using Bunit;
using Flare.Components.Gantt;
using Flare.Components.Gantt.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Xunit;

namespace Flare.Components.Gantt.Tests;

public sealed class GanttInteractionTests : GanttTestBase
{
    private static readonly Node[] TwoRows = [new Node("r1"), new Node("r2")];
    private static readonly Work[] Plan = [new Work("a", "r1", Day(0), Day(1)), new Work("b", "r1", Day(2), Day(3))];

    private IRenderedComponent<FlareGantt> Editable(List<GanttChangeEventArgs> seen, bool cancel = false, GanttLink[]? links = null,
        Action<ComponentParameterCollectionBuilder<FlareGanttPanel<Node, Work>>>? more = null) =>
        Chart(p =>
        {
            p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan).Add(x => x.Editable, true)
             .Add(x => x.Links, links ?? [new GanttLink("a", "b")])
             .Add(x => x.OnChange, (GanttChangeEventArgs e) => { seen.Add(e); e.Cancel = cancel; });
            more?.Invoke(p);
        });

    private static IElement Bar(IRenderedComponent<FlareGantt> cut, string key) => cut.Find($".flare-gantt__bar[data-key='{key}']");

    [Fact]
    public void Ctrl_arrow_moves_one_unit_and_pushes_successors()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen);

        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });

        var change = Assert.Single(seen);
        Assert.Equal(GanttEditAction.Move, change.Action);
        Assert.Equal([new GanttChange("a", GanttSpan.FromDays(Day(1), Day(2))), new GanttChange("b", GanttSpan.FromDays(Day(3), Day(4)))], change.Changes);
        Assert.Contains("a \u043F\u0435\u0440\u0435\u043D\u0435\u0441\u0435\u043D: 13.10.2026 \u2013 14.10.2026.", cut.Find("[role=status]").TextContent);
    }

    [Fact]
    public void Ctrl_shift_arrow_resizes_the_end_and_never_inverts()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen, links: []);

        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true, ShiftKey = true });
        Bar(cut, "b").KeyDown(new KeyboardEventArgs { Key = "ArrowLeft", CtrlKey = true, ShiftKey = true });
        Drag(cut, "a", GanttEditAction.ResizeStart, 5);

        Assert.Equal(GanttEditAction.ResizeEnd, seen[0].Action);
        Assert.Equal(GanttSpan.FromDays(Day(0), Day(2)), seen[0].Changes[0].Span);
        Assert.Equal(GanttSpan.FromDays(Day(2), Day(2)), seen[1].Changes[0].Span);
        Assert.Equal(GanttEditAction.ResizeStart, seen[2].Action);
        Assert.Equal(new GanttSpan(Day(2).ToDateTime(TimeOnly.MinValue), Day(2).ToDateTime(TimeOnly.MinValue)), seen[2].Changes[0].Span); // start stops at the end
    }

    [Fact]
    public void Ctrl_down_moves_to_the_next_row()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen);

        Bar(cut, "b").KeyDown(new KeyboardEventArgs { Key = "ArrowDown", CtrlKey = true });
        Bar(cut, "b").KeyDown(new KeyboardEventArgs { Key = "ArrowUp", CtrlKey = true }); // already on the first row

        var change = Assert.Single(seen);
        Assert.Equal("r2", change.Changes[0].RowKey);
        Assert.Single(change.Changes);
    }

    [Fact]
    public void A_cancelled_or_unapplied_change_leaves_the_chart_as_it_was()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen, cancel: true);
        string Geometry() => string.Join("|", cut.FindAll(".flare-gantt__bar-slot, path.flare-gantt__link").Select(e => e.GetAttribute("style") ?? e.GetAttribute("d")));
        var before = Geometry();

        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });

        Assert.Single(seen);
        Assert.Equal(before, Geometry());
        Assert.Empty(cut.Find("[role=status]").TextContent);
    }

    [Fact]
    public void A_pointer_drag_commits_whole_units()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen, links: []);

        Drag(cut, "a", GanttEditAction.Move, 2);
        Bar(cut, "a").Click(); // the click that follows a drag does not select

        var change = Assert.Single(seen);
        Assert.Equal(GanttSpan.FromDays(Day(2), Day(3)), change.Changes[0].Span);
        Assert.Empty(cut.FindAll(".flare-gantt__bar--selected"));
        Assert.Empty(cut.FindAll(".flare-gantt__bar--preview"));
    }

    [Fact]
    public void Pointer_drag_down_changes_the_row_and_the_end_handle_resizes()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen, links: []);

        Drag(cut, "a", GanttEditAction.Move, 0, "r2");
        Assert.Equal("r2", seen[0].Changes[0].RowKey);

        Drag(cut, "b", GanttEditAction.ResizeEnd, 2);
        Assert.Equal(GanttEditAction.ResizeEnd, seen[1].Action);
        Assert.Equal(GanttSpan.FromDays(Day(2), Day(5)), seen[1].Changes[0].Span);
    }

    [Fact]
    public async Task A_gesture_for_another_panel_an_unknown_action_or_a_row_change_that_is_not_allowed_moves_nothing_across_rows()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cut = Editable(seen, more: p => p.Add(x => x.AllowRowChange, false));

        await cut.InvokeAsync(() => cut.Instance.BarDragged("elsewhere", "a", "Move", 1, null));
        await cut.InvokeAsync(() => cut.Instance.BarDragged(cut.Find(".flare-gantt__body").GetAttribute("data-panel")!, "a", "Spin", 1, null));
        Assert.Empty(seen);

        Drag(cut, "a", GanttEditAction.Move, 1, "r2");
        Assert.Null(Assert.Single(seen).Changes[0].RowKey);
    }

    // The same rules as the keyboard: a link drawn with the pointer is checked for cycles before it is offered.
    [Fact]
    public async Task A_link_drawn_with_the_pointer_is_created_or_refused()
    {
        var created = new List<GanttLink>();
        var rejected = new List<GanttLinkRejectedEventArgs>();
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan).Add(x => x.Editable, true)
            .Add(x => x.Links, new[] { new GanttLink("a", "b") })
            .Add(x => x.OnLinkCreate, (GanttLinkEventArgs e) => created.Add(e.Link))
            .Add(x => x.OnLinkRejected, (GanttLinkRejectedEventArgs e) => rejected.Add(e)));
        var panel = cut.Find(".flare-gantt__body").GetAttribute("data-panel")!;

        await cut.InvokeAsync(() => cut.Instance.LinkDragged(panel, "b", "a"));
        Assert.Equal(GanttValidationCode.Cycle, Assert.Single(rejected).Validation.Code);

        await cut.InvokeAsync(() => cut.Instance.LinkDragged(panel, "a", "b"));
        Assert.Equal(GanttValidationCode.Duplicate, rejected[1].Validation.Code);
        Assert.Empty(created);
        Assert.Equal(2, cut.FindAll(".flare-gantt__link-handle").Count);
    }

    [Fact]
    public void Links_are_created_from_the_keyboard_and_cycles_are_refused()
    {
        var created = new List<GanttLink>();
        var rejected = new List<GanttLinkRejectedEventArgs>();
        var items = new[] { new Work("a", "r1", Day(0), Day(1)), new Work("b", "r1", Day(2), Day(3)), new Work("c", "r2", Day(4), Day(5)) };
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, items).Add(x => x.Editable, true)
            .Add(x => x.Links, new[] { new GanttLink("a", "b"), new GanttLink("b", "c") })
            .Add(x => x.OnLinkCreate, (GanttLinkEventArgs e) => created.Add(e.Link))
            .Add(x => x.OnLinkRejected, (GanttLinkRejectedEventArgs e) => rejected.Add(e)));

        Bar(cut, "c").KeyDown(new KeyboardEventArgs { Key = "l" });
        Assert.Contains("flare-gantt__bar--link-source", Bar(cut, "c").ClassList);
        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        var refusal = Assert.Single(rejected);
        Assert.Equal(GanttValidationCode.Cycle, refusal.Validation.Code);
        Assert.Equal("\u0421\u0432\u044F\u0437\u044C \u0441\u043E\u0437\u0434\u0430\u0435\u0442 \u0446\u0438\u043A\u043B: c \u2192 a \u2192 b \u2192 c.", refusal.Message);
        Assert.Equal("\u0421\u0432\u044F\u0437\u044C \u0441\u043E\u0437\u0434\u0430\u0435\u0442 \u0446\u0438\u043A\u043B: C \u2192 A \u2192 B \u2192 C.", new GanttTexts().Describe(refusal.Validation, key => key.ToUpperInvariant(), new System.Globalization.CultureInfo("ru-RU")));
        Assert.Equal(refusal.Message, cut.Find("[role=status]").TextContent);

        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "L" });
        Bar(cut, "c").Click();
        Assert.Equal([new GanttLink("a", "c")], created);
        Assert.Equal("Связь создана: a → c.", cut.Find("[role=status]").TextContent);
        Assert.Empty(cut.FindAll(".flare-gantt__bar--link-source"));
    }

    [Fact]
    public void A_link_the_handler_refuses_is_not_announced()
    {
        var created = new List<GanttLink>();
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows)
            .Add(x => x.Items, new[] { new Work("a", "r1", Day(0), Day(1)), new Work("c", "r2", Day(4), Day(5)) })
            .Add(x => x.Editable, true)
            .Add(x => x.OnLinkCreate, (GanttLinkEventArgs e) => { created.Add(e.Link); e.Cancel = true; }));

        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "l" });
        Bar(cut, "c").Click();

        Assert.Equal([new GanttLink("a", "c")], created);
        // The refusal leaves the previous announcement alone instead of confirming a link the handler refused.
        Assert.Equal("Связь от a. Выберите последователя и нажмите Enter.", cut.Find("[role=status]").TextContent);
        Assert.Empty(cut.FindAll(".flare-gantt__bar--link-source"));
    }

    [Fact]
    public void Read_only_and_compact_charts_raise_nothing()
    {
        var seen = new List<GanttChangeEventArgs>();
        var readOnly = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan)
            .Add(x => x.OnChange, (GanttChangeEventArgs e) => seen.Add(e)));
        Bar(readOnly, "a").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });
        Drag(readOnly, "a", GanttEditAction.Move, 1);
        Assert.Empty(readOnly.FindAll(".flare-gantt__handle"));
        Assert.Empty(readOnly.FindAll(".flare-gantt__link-handle"));
        Assert.Null(readOnly.Find(".flare-gantt__body").GetAttribute("data-editable"));

        var compact = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan).Add(x => x.Editable, true)
            .Add(x => x.OnChange, (GanttChangeEventArgs e) => seen.Add(e)), g => g.Add(x => x.Compact, true));
        Bar(compact, "a").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });

        Assert.Empty(seen);
    }

    [Fact]
    public void Drop_zones_are_one_per_row_and_resolve_the_unit_under_the_pointer()
    {
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.DropGroup, "pool"));

        var zones = cut.FindAll(".flare-gantt__drop");
        Assert.Equal(2, zones.Count);
        var rows = zones.Select(z => GanttRowDropTarget.TryParse(z.GetAttribute("data-flare-drop"), out var t) ? t!.RowKey : null);
        Assert.Equal(["r1", "r2"], rows);
        Assert.All(zones, z => Assert.NotNull(z.QuerySelector(".flare-gantt__drop-unit")));

        // 40 px per day: 130 px from the start of the axis is the fourth day.
        Assert.Equal(new GanttDropTarget("r2", GanttSpan.Day(Day(3))), cut.Instance.UnitAt("r2", 130));
        Assert.Null(cut.Instance.UnitAt("r2", 10_000));
    }

    [Fact]
    public async Task Resolve_drop_reads_unit_targets_and_refuses_anything_else()
    {
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows));
        var unit = GanttDropTarget.Format("r2", GanttSpan.Day(Day(3)));

        Assert.Equal(new GanttDropTarget("r2", GanttSpan.Day(Day(3))), await cut.InvokeAsync(() => cut.Instance.ResolveDrop(unit)));
        Assert.Null(await cut.InvokeAsync(() => cut.Instance.ResolveDrop("something else")));
        Assert.False(GanttRowDropTarget.TryParse(unit, out _));
        Assert.False(GanttRowDropTarget.TryParse(GanttItemDropTarget.Format("a"), out _));
        Assert.False(GanttDropTarget.TryParse(GanttRowDropTarget.Format("r1"), out _));
    }

    [Fact]
    public void Bars_become_drop_zones_only_with_an_item_drop_group()
    {
        var plain = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan));
        Assert.Empty(plain.FindAll(".flare-gantt__item-drop"));

        var cut = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan).Add(x => x.ItemDropGroup, "people"));

        var zones = cut.FindAll(".flare-gantt__item-drop");
        Assert.Equal(2, zones.Count);
        var keys = zones.Select(z => GanttItemDropTarget.TryParse(z.GetAttribute("data-flare-drop"), out var t) ? t!.ItemKey : null);
        Assert.Equal(["a", "b"], keys);
        Assert.All(zones, z => Assert.NotNull(z.QuerySelector(".flare-gantt__bar")));
    }

    [Fact]
    public void Item_targets_parse_and_never_pass_for_unit_targets()
    {
        Assert.True(GanttItemDropTarget.TryParse(GanttItemDropTarget.Format("task-1"), out var item));
        Assert.Equal(new GanttItemDropTarget("task-1"), item);
        Assert.False(GanttItemDropTarget.TryParse(GanttDropTarget.Format("r1", GanttSpan.Day(Day(0))), out _));
        Assert.False(GanttDropTarget.TryParse(GanttItemDropTarget.Format("task-1"), out _));
        Assert.False(GanttItemDropTarget.TryParse("task-1", out _));
    }

    [Fact]
    public void Suggestions_are_never_drop_zones()
    {
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan).Add(x => x.ItemDropGroup, "people")
            .Add(x => x.Suggested, new[] { new GanttChange("a", GanttSpan.FromDays(Day(4), Day(5)), "r2") }));

        Assert.Equal(2, cut.FindAll(".flare-gantt__item-drop").Count);
    }

    [Fact]
    public void Accepts_reach_the_unit_and_bar_zones()
    {
        var cut = Chart(p => p.Add(x => x.Rows, TwoRows).Add(x => x.Items, Plan)
            .Add(x => x.DropGroup, "plan").Add(x => x.DropAccepts, (object? o) => o is string)
            .Add(x => x.ItemDropGroup, "plan").Add(x => x.ItemDropAccepts, (object? o) => o is int));

        var zones = cut.FindComponents<FlareDropZone>();
        var units = zones.Where(z => GanttRowDropTarget.TryParse(z.Instance.Target, out _)).ToArray();
        var bars = zones.Where(z => GanttItemDropTarget.TryParse(z.Instance.Target, out _)).ToArray();
        Assert.Equal(2, units.Length);
        Assert.Equal(2, bars.Length);
        Assert.All(units, z => { Assert.True(z.Instance.Accepts!("task")); Assert.False(z.Instance.Accepts!(7)); });
        Assert.All(bars, z => { Assert.True(z.Instance.Accepts!(7)); Assert.False(z.Instance.Accepts!("task")); });
    }

    [Fact]
    public async Task A_bar_moved_to_another_row_before_the_module_attached_gets_the_focus_once_it_has()
    {
        var attached = new TaskCompletionSource<IGanttView?>();
        var view = new RecordingView();
        Services.AddSingleton<IGanttJsService>(new SlowJs(attached.Task));
        var cut = Editable([]);

        Bar(cut, "a").KeyDown(new KeyboardEventArgs { Key = "ArrowDown", CtrlKey = true });
        Assert.Empty(view.Focused);

        await cut.InvokeAsync(() => attached.SetResult(view));

        cut.WaitForAssertion(() => Assert.Equal(["a"], view.Focused));
    }

    private sealed class SlowJs(Task<IGanttView?> attached) : IGanttJsService
    {
        public async ValueTask<IGanttView?> AttachAsync(ElementReference root, DotNetObjectReference<FlareGantt> dotNetRef, bool virtualize, bool endless) =>
            await attached;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private sealed class RecordingView : IGanttView
    {
        public List<string> Focused { get; } = [];

        public ValueTask ScrollToAsync(double x) => ValueTask.CompletedTask;

        public ValueTask ShiftAsync(double pixels) => ValueTask.CompletedTask;

        public ValueTask<double?> PointerXAsync() => ValueTask.FromResult<double?>(null);

        public ValueTask FocusItemAsync(string key)
        {
            Focused.Add(key);
            return ValueTask.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
