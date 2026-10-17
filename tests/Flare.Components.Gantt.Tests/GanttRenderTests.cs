using System.Globalization;
using Bunit;
using Flare.Components.Gantt;
using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flare.Components.Gantt.Tests;

public sealed record Node(string Key, params Node[] Children);

public sealed record Work(string Key, string Row, DateOnly Start, DateOnly End, string? Kind = null, double? Done = null);

public abstract class GanttTestBase : FlareTestContext
{
    protected static readonly DateOnly Monday = new(2026, 10, 12);
    protected static readonly CultureInfo Ru = new("ru-RU");

    protected GanttTestBase() => Services.AddFlareGantt();

    protected static DateOnly Day(int offset) => Monday.AddDays(offset);

    // What the browser reports when a pointer gesture on a bar ends (gantt.js): the panel, the bar, what was done and
    // by how many whole axis units, and the row a move ended over.
    protected static void Drag(IRenderedComponent<FlareGantt> cut, string key, GanttEditAction action, int units, string? row = null)
    {
        var panel = cut.Find(".flare-gantt__body").GetAttribute("data-panel")!;
        cut.InvokeAsync(() => cut.Instance.BarDragged(panel, key, action.ToString(), units, row)).GetAwaiter().GetResult();
    }

    protected IRenderedComponent<FlareGantt> Chart(Action<ComponentParameterCollectionBuilder<FlareGanttPanel<Node, Work>>> panel,
        Action<ComponentParameterCollectionBuilder<FlareGantt>>? gantt = null) =>
        Render<FlareGantt>(p =>
        {
            p.Add(x => x.Range, GanttSpan.FromDays(Monday, Day(13)))
             .Add(x => x.Scale, GanttScale.Day)
             .Add(x => x.UnitWidth, 40d)
             .Add(x => x.RowHeight, 30d)
             .Add(x => x.BarInset, 5d)
             .Add(x => x.Culture, Ru);
            gantt?.Invoke(p);
            p.AddChildContent<FlareGanttPanel<Node, Work>>(pp =>
            {
                pp.Add(x => x.RowKey, (Func<Node, string>)(n => n.Key))
                  .Add(x => x.RowChildren, (Func<Node, IEnumerable<Node>?>)(n => n.Children))
                  .Add(x => x.ItemKey, (Func<Work, string>)(w => w.Key))
                  .Add(x => x.ItemRow, (Func<Work, string>)(w => w.Row))
                  .Add(x => x.ItemSpan, (Func<Work, GanttSpan?>)(w => GanttSpan.FromDays(w.Start, w.End)))
                  .Add(x => x.ItemKind, (Func<Work, string?>)(w => w.Kind))
                  .Add(x => x.ItemProgress, (Func<Work, double?>)(w => w.Done));
                panel(pp);
            });
        });
}

public sealed class GanttRenderTests : GanttTestBase
{
    [Fact]
    public void Bars_are_placed_by_the_axis()
    {
        var cut = Chart(p => p
            .Add(x => x.Rows, new[] { new Node("r1") })
            .Add(x => x.Items, new[] { new Work("w1", "r1", Day(2), Day(4), "work") }));

        var slot = cut.Find(".flare-gantt__bar-slot");
        Assert.Equal("left:80px;top:5px;width:120px;height:20px", slot.GetAttribute("style"));
        var bar = cut.Find(".flare-gantt__bar");
        Assert.Contains("flare-gantt--kind-work", bar.ClassList);
        Assert.Equal("work", bar.GetAttribute("data-kind"));
        Assert.Equal("w1, 14.10.2026 \u2013 16.10.2026", bar.GetAttribute("aria-label"));
    }

    [Fact]
    public void Header_shows_days_with_weekdays_and_the_month_above()
    {
        var cut = Chart(p => p.Add(x => x.Rows, Array.Empty<Node>()));

        var ticks = cut.FindAll(".flare-gantt__scale-row:not(.flare-gantt__scale-row--major) .flare-gantt__tick");
        Assert.Equal(14, ticks.Count);
        Assert.Contains("12", ticks[0].TextContent);
        Assert.Contains("\u043F\u043D", ticks[0].TextContent);
        Assert.Contains("\u041E\u043A\u0442\u044F\u0431\u0440\u044C 2026", cut.Find(".flare-gantt__scale-row--major").TextContent);
    }

    [Fact]
    public void Overlapping_items_get_lanes_and_the_row_grows()
    {
        var cut = Chart(p => p
            .Add(x => x.Rows, new[] { new Node("r1") })
            .Add(x => x.Items, new[] { new Work("a", "r1", Day(0), Day(3)), new Work("b", "r1", Day(1), Day(2)) }));

        Assert.Equal("height:60px", cut.Find(".flare-gantt__row").GetAttribute("style"));
        var slots = cut.FindAll(".flare-gantt__bar-slot");
        Assert.Contains("top:35px", slots[1].GetAttribute("style"));
    }

    [Fact]
    public void Tree_rows_collapse_and_parents_roll_up_their_children()
    {
        var cut = Chart(p => p
            .Add(x => x.Rows, new[] { new Node("stage", new Node("t1"), new Node("t2")) })
            .Add(x => x.RollupParents, true)
            .Add(x => x.Items, new[] { new Work("w1", "t1", Day(1), Day(2), Done: 1), new Work("w2", "t2", Day(3), Day(6), Done: 0) }));

        Assert.Equal("treegrid", cut.Find(".flare-gantt__body").GetAttribute("role"));
        var rows = cut.FindAll(".flare-gantt__row");
        Assert.Equal(["stage", "t1", "t2"], rows.Select(r => r.GetAttribute("data-row")));
        Assert.Equal(["1", "2", "2"], rows.Select(r => r.GetAttribute("aria-level")));
        Assert.Equal("true", rows[0].GetAttribute("aria-expanded"));

        var summary = cut.Find(".flare-gantt__bar--summary");
        Assert.Contains("left:40px", summary.ParentElement!.GetAttribute("style"));     // from day 1
        Assert.Contains("width:240px", summary.ParentElement!.GetAttribute("style"));   // through day 6
        Assert.Equal("width:33.33%", summary.QuerySelector(".flare-gantt__progress")!.GetAttribute("style")); // 2 of 6 days done

        cut.Find(".flare-gantt__toggle").Click();
        Assert.Single(cut.FindAll(".flare-gantt__row"));
        Assert.Equal("false", cut.Find(".flare-gantt__row").GetAttribute("aria-expanded"));
        Assert.Single(cut.FindAll(".flare-gantt__bar--summary")); // the rollup still covers the hidden children
    }

    [Fact]
    public void Segments_cells_regions_and_markers_carry_their_kinds()
    {
        var cut = Chart(p => p
                .Add(x => x.Rows, new[] { new Node("anna") })
                .Add(x => x.Items, new[] { new Work("w", "anna", Day(0), Day(3)) })
                .Add(x => x.ItemSegments, (Func<Work, IEnumerable<GanttSegment>?>)(_ => [new GanttSegment("waiting", GanttSpan.FromDays(Day(2), Day(3)), "Wait")]))
                .Add(x => x.Cells, new[] { new GanttCell("anna", GanttSpan.Day(Day(0)), "overload", "11.5") })
                .Add(x => x.Regions, new[] { new GanttRegion("vacation", GanttSpan.FromDays(Day(7), Day(9)), "anna", "\u041E\u0442\u043F\u0443\u0441\u043A") })
                .Add(x => x.Markers, new[] { new GanttMarker("deadline", Day(4).ToDateTime(TimeOnly.MinValue), "anna", "\u0421\u0440\u043E\u043A") }),
            g => g.Add(x => x.Regions, new[] { new GanttRegion("sprint", GanttSpan.FromDays(Day(0), Day(9)), Label: "\u0421\u043F\u0440\u0438\u043D\u0442") })
                  .Add(x => x.Markers, new[] { new GanttMarker("today", Day(1).ToDateTime(TimeOnly.MinValue)) }));

        var segment = cut.Find(".flare-gantt__segment");
        Assert.Equal("left:80px;width:80px", segment.GetAttribute("style"));
        Assert.Equal("waiting", segment.GetAttribute("data-kind"));

        var cell = cut.Find(".flare-gantt__cell");
        Assert.Equal("11.5", cell.TextContent.Trim());
        Assert.Contains("flare-gantt--kind-overload", cell.ClassList);
        Assert.Equal("height:60px", cut.Find(".flare-gantt__row").GetAttribute("style")); // cell strip + one lane

        Assert.Equal(["sprint", "vacation"], cut.FindAll(".flare-gantt__region").Select(r => r.GetAttribute("data-kind")));
        Assert.Equal(["deadline", "today"], cut.FindAll(".flare-gantt__marker").Select(m => m.GetAttribute("data-kind")));
    }

    [Fact]
    public void Legend_samples_are_the_chart_elements_of_each_kind()
    {
        var cut = Render<FlareGanttLegend>(p => p.Add(x => x.Items, new GanttLegendItem[]
        {
            new("risk", "\u0420\u0438\u0441\u043A"),
            new("waiting", "\u041E\u0436\u0438\u0434\u0430\u043D\u0438\u0435", GanttLegendShape.Segment, BarKind: "work"),
            new("sprint", "\u0421\u043F\u0440\u0438\u043D\u0442", GanttLegendShape.Region),
            new("overload", "\u041F\u0435\u0440\u0435\u0433\u0440\u0443\u0437", GanttLegendShape.Cell, "\u0411\u043E\u043B\u044C\u0448\u0435, \u0447\u0435\u043C \u0434\u043E\u0441\u0442\u0443\u043F\u043D\u043E")
        }));

        var samples = cut.FindAll(".flare-gantt-legend__sample");
        Assert.Equal(4, samples.Count);
        Assert.Contains("flare-gantt__bar", samples[0].ClassList);
        Assert.Contains("flare-gantt--kind-risk", samples[0].ClassList);
        Assert.Contains("flare-gantt--kind-work", samples[1].ClassList);
        Assert.Contains("flare-gantt--kind-waiting", samples[1].QuerySelector(".flare-gantt__segment")!.ClassList);
        Assert.Contains("flare-gantt__region", samples[2].ClassList);
        Assert.Contains("flare-gantt__cell", samples[3].ClassList);
        Assert.Equal("\u0411\u043E\u043B\u044C\u0448\u0435, \u0447\u0435\u043C \u0434\u043E\u0441\u0442\u0443\u043F\u043D\u043E", cut.FindAll(".flare-gantt-legend__item")[3].GetAttribute("title"));
    }

    [Fact]
    public void Item_title_replaces_the_label_in_the_bar_tooltip()
    {
        var cut = Chart(p => p
            .Add(x => x.Rows, new[] { new Node("anna") })
            .Add(x => x.Items, new[] { new Work("w", "anna", Day(0), Day(3)) })
            .Add(x => x.ItemTitle, (Func<Work, string?>)(w => $"{w.Key}\n\u0420\u0438\u0441\u043A: \u043D\u0435\u0442 \u0438\u0441\u043F\u043E\u043B\u043D\u0438\u0442\u0435\u043B\u044F")));

        Assert.Equal("w\n\u0420\u0438\u0441\u043A: \u043D\u0435\u0442 \u0438\u0441\u043F\u043E\u043B\u043D\u0438\u0442\u0435\u043B\u044F", cut.Find(".flare-gantt__bar").GetAttribute("title"));
    }

    [Fact]
    public void Non_working_days_are_shaded_on_the_day_scale()
    {
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") }),
            g => g.Add(x => x.NonWorking, (Func<DateOnly, bool>)(d => d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)));

        var shaded = cut.FindAll(".flare-gantt__nonworking");
        Assert.Equal(2, shaded.Count);                                        // two weekends, each merged
        Assert.Equal("left:200px;width:80px", shaded[0].GetAttribute("style")); // Sat 17 + Sun 18
        Assert.Equal(4, cut.FindAll(".flare-gantt__tick--nonworking").Count);
    }

    [Fact]
    public void Links_follow_the_routing_and_can_be_filtered()
    {
        var items = new[] { new Work("a", "r1", Day(0), Day(1)), new Work("b", "r2", Day(3), Day(4)), new Work("c", "r2", Day(6), Day(6)) };
        var links = new[] { new GanttLink("a", "b"), new GanttLink("b", "c", GanttLinkType.StartToStart, Kind: "soft") };
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r1"), new Node("r2") }).Add(x => x.Items, items).Add(x => x.Links, links));

        var paths = cut.FindAll("path.flare-gantt__link");
        Assert.Equal(2, paths.Count);
        var expected = GanttRouting.Path(new GanttBox(0, 5, 80, 20), new GanttBox(120, 35, 80, 20), GanttLinkType.FinishToStart, 8);
        Assert.Equal(expected, paths[0].GetAttribute("d"));
        Assert.Contains("flare-gantt__link--ss", paths[1].ClassList);
        Assert.Contains("flare-gantt--kind-soft", paths[1].ClassList);

        var filtered = Chart(p => p.Add(x => x.Rows, new[] { new Node("r1"), new Node("r2") }).Add(x => x.Items, items).Add(x => x.Links, links)
            .Add(x => x.LinkTypes, new[] { GanttLinkType.FinishToStart }));
        Assert.Single(filtered.FindAll("path.flare-gantt__link"));
    }

    [Fact]
    public void Critical_path_marks_bars_and_links()
    {
        var items = new[] { new Work("a", "r", Day(0), Day(1)), new Work("b", "r", Day(2), Day(3)), new Work("x", "r2", Day(0), Day(0)) };
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r"), new Node("r2") }).Add(x => x.Items, items)
            .Add(x => x.Links, new[] { new GanttLink("a", "b") }).Add(x => x.ShowCriticalPath, true));

        Assert.Equal(["a", "b"], cut.FindAll(".flare-gantt__bar--critical").Select(b => b.GetAttribute("data-key")));
        Assert.Single(cut.FindAll(".flare-gantt__link--critical"));
    }

    [Fact]
    public void Suggested_changes_are_ghosts_on_their_target_row()
    {
        var cut = Chart(p => p
            .Add(x => x.Rows, new[] { new Node("anna"), new Node("olga") })
            .Add(x => x.Items, new[] { new Work("w", "anna", Day(0), Day(1)) })
            .Add(x => x.Suggested, new[] { new GanttChange("w", GanttSpan.FromDays(Day(2), Day(3)), "olga") }));

        var ghost = cut.Find(".flare-gantt__bar--suggested");
        Assert.Equal("olga", ghost.Closest(".flare-gantt__row")!.GetAttribute("data-row"));
        Assert.Null(ghost.GetAttribute("tabindex"));
    }

    [Fact]
    public void Compact_mode_has_no_row_headers()
    {
        var cut = Chart(p => p.Add(x => x.Rows, new[] { new Node("r") }).Add(x => x.Items, new[] { new Work("w", "r", Day(0), Day(1)) }),
            g => g.Add(x => x.Compact, true));

        Assert.Empty(cut.FindAll("[role=rowheader]"));
        Assert.Empty(cut.FindAll(".flare-gantt__corner"));
        Assert.Contains("--_gantt-side:0px", cut.Find(".flare-gantt").GetAttribute("style"));
        Assert.Contains("flare-gantt--compact", cut.Find(".flare-gantt").ClassList);
    }

    [Fact]
    public void Two_panels_share_the_axis_and_the_selection()
    {
        string? selected = null;
        var cut = Render<FlareGantt>(p => p
            .Add(x => x.Range, GanttSpan.FromDays(Monday, Day(6)))
            .Add(x => x.Culture, Ru)
            .Add(x => x.SelectedKeyChanged, (string? key) => selected = key)
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                Panel(b, 0, "tasks", [new Work("w", "tasks", Day(0), Day(1))]);
                Panel(b, 10, "people", [new Work("w", "people", Day(0), Day(1))]);
            })));

        cut.FindAll(".flare-gantt__bar")[0].Click();

        Assert.Equal("w", selected);
        Assert.Equal(2, cut.FindAll(".flare-gantt__bar--selected").Count);
        Assert.Single(cut.FindAll(".flare-gantt__header"));
    }

    private static void Panel(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string row, Work[] items)
    {
        b.OpenComponent<FlareGanttPanel<Node, Work>>(seq);
        b.AddAttribute(seq + 1, nameof(FlareGanttPanel<Node, Work>.Rows), new[] { new Node(row) });
        b.AddAttribute(seq + 2, nameof(FlareGanttPanel<Node, Work>.RowKey), (Func<Node, string>)(n => n.Key));
        b.AddAttribute(seq + 3, nameof(FlareGanttPanel<Node, Work>.Items), items);
        b.AddAttribute(seq + 4, nameof(FlareGanttPanel<Node, Work>.ItemKey), (Func<Work, string>)(w => w.Key));
        b.AddAttribute(seq + 5, nameof(FlareGanttPanel<Node, Work>.ItemRow), (Func<Work, string>)(w => w.Row));
        b.AddAttribute(seq + 6, nameof(FlareGanttPanel<Node, Work>.ItemSpan), (Func<Work, GanttSpan?>)(w => GanttSpan.FromDays(w.Start, w.End)));
        b.CloseComponent();
    }

    [Fact]
    public void Panel_outside_a_gantt_is_refused()
    {
        Assert.Throws<InvalidOperationException>(() => Render<FlareGanttPanel<Node, Work>>(p => p.Add(x => x.RowKey, (Func<Node, string>)(n => n.Key))));
    }
}
