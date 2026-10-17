using Bunit;
using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Flare.Components.Gantt.Tests;

/// <summary>Axis width, the row-header column and the cover stop while resizing.</summary>
public sealed class GanttLayoutTests : GanttTestBase
{
    private static readonly Node[] OneRow = [new Node("r1")];
    private static readonly Work[] Parent = [new Work("p", "r1", Day(2), Day(5))];

    [Fact]
    public void Fill_width_stretches_units_to_the_visible_width()
    {
        var cut = Chart(p => p.Add(x => x.Rows, OneRow).Add(x => x.Items, Parent), g => g.Add(x => x.FillWidth, true));

        // 14 days of 40 px = 560 px; 1000 px are visible, 160 px of them the row-header column.
        cut.InvokeAsync(() => cut.Instance.Viewport(1000, 160));

        Assert.Equal(59.92, cut.Instance.EffectiveUnitWidth, 2);
        Assert.StartsWith("left:119.84px;", cut.Find(".flare-gantt__bar-slot").GetAttribute("style"));
    }

    [Fact]
    public void Unit_width_is_the_floor_and_fill_is_opt_in()
    {
        var narrow = Chart(p => p.Add(x => x.Rows, OneRow), g => g.Add(x => x.FillWidth, true));
        narrow.InvokeAsync(() => narrow.Instance.Viewport(300, 160));
        var plain = Chart(p => p.Add(x => x.Rows, OneRow));
        plain.InvokeAsync(() => plain.Instance.Viewport(2000, 160));

        Assert.Equal(40, narrow.Instance.EffectiveUnitWidth);
        Assert.Equal(40, plain.Instance.EffectiveUnitWidth);
    }

    [Fact]
    public void A_new_width_that_keeps_the_unit_does_not_rerender()
    {
        var cut = Chart(p => p.Add(x => x.Rows, OneRow).Add(x => x.Items, Parent), g => g.Add(x => x.FillWidth, true));
        var renders = cut.RenderCount;

        // The axis is wider than the view: a day stays 40 px however the window width changes (a menu animation).
        foreach (var width in new[] { 500d, 480, 460, 440 }) cut.InvokeAsync(() => cut.Instance.Viewport(width, 160));

        Assert.Equal(40, cut.Instance.EffectiveUnitWidth);
        Assert.Equal(renders, cut.RenderCount);
    }

    [Fact]
    public void Virtualized_panels_draw_only_rows_near_the_visible_part()
    {
        var rows = Enumerable.Range(0, 200).Select(i => new Node($"r{i}")).ToArray();
        var cut = Chart(p => p.Add(x => x.Rows, rows), g => g.Add(x => x.Virtualize, true));
        int Drawn() => cut.FindAll(".flare-gantt__row").Count;

        // Rows of 30 px: before the browser reports what is visible, the first screen with a margin is drawn, not every row.
        Assert.InRange(Drawn(), 30, 90);
        Assert.Empty(cut.FindAll(".flare-gantt__spacer"));
        Assert.Equal("6000px", cut.Find(".flare-gantt__body").GetAttribute("style")!.Split(':')[1]);

        var panel = cut.Find(".flare-gantt__body").GetAttribute("data-panel")!;
        cut.InvokeAsync(() => cut.Instance.Visible([panel], [3000], [3600]));

        Assert.InRange(Drawn(), 20, 80);
        Assert.Equal("height:2400px", cut.Find(".flare-gantt__spacer").GetAttribute("style"));
        Assert.Equal("r80", cut.Find(".flare-gantt__row").GetAttribute("data-row"));
    }

    [Fact]
    public void Virtualized_columns_are_drawn_only_near_the_visible_part_of_the_axis()
    {
        // 14 days x 40 px = 560 px; 80 px are visible from 240: the window 160-400 px is drawn, that is days 4-9.
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") })
                .Add(x => x.Items, new[] { new Work("early", "r", Day(0), Day(0)), new Work("late", "r", Day(12), Day(12)) }),
            g => g.Add(x => x.Virtualize, true));
        Assert.Equal(14, cut.FindAll(".flare-gantt__scale-row:not(.flare-gantt__scale-row--major) .flare-gantt__tick").Count);

        cut.InvokeAsync(() => cut.Instance.Horizontal(240, 80));

        Assert.Equal(6, cut.FindAll(".flare-gantt__scale-row:not(.flare-gantt__scale-row--major) .flare-gantt__tick").Count);
        Assert.Empty(cut.FindAll(".flare-gantt__bar"));
        Assert.Equal("560px", cut.Find(".flare-gantt__scale").GetAttribute("style")!.Split(':')[1]); // the axis keeps its width
    }

    [Fact]
    public void An_endless_axis_asks_for_more_time_near_its_edges()
    {
        GanttSpan? asked = null;
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") }),
            g => g.Add(x => x.Endless, true).Add(x => x.RangeChanged, (GanttSpan range) => asked = range));

        // At the left edge with 200 px on screen: three screens (15 days) are added on the left; the right edge is far.
        cut.InvokeAsync(() => cut.Instance.Horizontal(0, 200));
        Assert.Equal(new GanttSpan(Day(-15).ToDateTime(TimeOnly.MinValue), Day(14).ToDateTime(TimeOnly.MinValue)), asked);

        // The consumer takes it: the axis moves the scroll by the 600 px added on the left.
        cut.Render(p => p.Add(x => x.Range, asked!.Value));
        Assert.Equal("1160px", cut.Find(".flare-gantt__scale").GetAttribute("style")!.Split(':')[1]);
        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "shift" && (double)i.Arguments[0]! == 600);
    }

    [Fact]
    public void Without_virtualize_every_row_is_drawn()
    {
        var rows = Enumerable.Range(0, 200).Select(i => new Node($"r{i}")).ToArray();
        var cut = Chart(p => p.Add(x => x.Rows, rows));

        Assert.Equal(200, cut.FindAll(".flare-gantt__row").Count);
    }

    [Fact]
    public void Dragged_side_width_is_kept_and_reported()
    {
        string? reported = null;
        var cut = Chart(p => p.Add(x => x.Rows, OneRow), g => g
            .Add(x => x.ResizableSide, true)
            .Add(x => x.SideWidth, "12rem")
            .Add(x => x.SideWidthChanged, (string w) => reported = w));

        Assert.NotNull(cut.Find(".flare-gantt__side-splitter[role=separator]"));
        // What the shared separator reports once a drag on it has ended.
        cut.InvokeAsync(() => cut.FindComponent<FlareSplitter>().Instance.OnSplitChanged("288px"));

        Assert.Equal("288px", reported);
        Assert.Contains("--_gantt-side:288px", cut.Find(".flare-gantt").GetAttribute("style"));
    }

    [Fact]
    public void No_handle_unless_asked()
    {
        var cut = Chart(p => p.Add(x => x.Rows, OneRow));

        Assert.Empty(cut.FindAll(".flare-gantt__side-splitter"));
    }

    [Fact]
    public void Resizing_stops_at_the_span_the_item_must_cover()
    {
        var seen = new List<GanttChangeEventArgs>();
        var cover = GanttSpan.FromDays(Day(1), Day(4)); // the children start before the parent and end a day earlier
        var cut = Chart(p => p.Add(x => x.Rows, OneRow).Add(x => x.Items, Parent).Add(x => x.Editable, true)
            .Add(x => x.ItemCover, (Func<Work, GanttSpan?>)(_ => cover))
            .Add(x => x.OnChange, (GanttChangeEventArgs e) => seen.Add(e)));

        var bar = cut.Find(".flare-gantt__bar[data-key='p']");
        bar.KeyDown(new KeyboardEventArgs { Key = "ArrowLeft", CtrlKey = true, ShiftKey = true }); // end 5 -> 4: allowed
        Drag(cut, "p", GanttEditAction.ResizeStart, 3);

        Assert.Equal(GanttSpan.FromDays(Day(2), Day(4)), seen[0].Changes[0].Span);
        // The start 2 is already later than the children's start (1): it cannot be pulled right, the bar snaps to the cover's start.
        Assert.Equal(GanttSpan.FromDays(Day(1), Day(5)), seen[1].Changes[0].Span);
    }
}
