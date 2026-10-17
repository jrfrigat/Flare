using System.Globalization;
using Bunit;
using Flare.Components.Gantt;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Flare.Components.Gantt.Tests;

// The same primitives, three domains that have nothing to do with tasks or people. If any of these needed a change
// in the package, the package would not be universal.

public sealed record Machine(string Id, string Name);

public sealed record Order(string Id, string MachineId, DateOnly From, DateOnly To);

public sealed record Room(string Id);

public sealed record Booking(string Id, string RoomId, DateTime From, DateTime To);

public sealed record Initiative(string Id, params Initiative[] Epics);

public sealed record Delivery(string EpicId, string Team, DateOnly From, DateOnly To);

public sealed class UniversalityTests : GanttTestBase
{
    [Fact]
    public void Production_plan_machines_orders_stages_and_maintenance()
    {
        var machines = new[] { new Machine("m1", "\u041F\u0440\u0435\u0441\u0441"), new Machine("m2", "\u0424\u0440\u0435\u0437\u0430") };
        var orders = new[]
        {
            new Order("o1", "m1", Day(0), Day(3)), new Order("o2", "m2", Day(4), Day(6)), new Order("o3", "m2", Day(0), Day(1))
        };
        IEnumerable<GanttSegment> Stages(Order o) =>
        [
            new GanttSegment("setup", GanttSpan.Day(o.From)),
            new GanttSegment("run", GanttSpan.FromDays(o.From.AddDays(1), o.To.AddDays(-1))),
            new GanttSegment("qa", GanttSpan.Day(o.To))
        ];

        var cut = Render<FlareGantt>(p => p
            .Add(x => x.Range, GanttSpan.FromDays(Monday, Day(9)))
            .Add(x => x.Culture, Ru)
            .AddChildContent<FlareGanttPanel<Machine, Order>>(pp => pp
                .Add(x => x.Rows, machines)
                .Add(x => x.RowKey, (Func<Machine, string>)(m => m.Id))
                .Add(x => x.RowHeader, (RenderFragment<Machine>)(m => b => b.AddContent(0, m.Name)))
                .Add(x => x.Items, orders)
                .Add(x => x.ItemKey, (Func<Order, string>)(o => o.Id))
                .Add(x => x.ItemRow, (Func<Order, string>)(o => o.MachineId))
                .Add(x => x.ItemSpan, (Func<Order, GanttSpan?>)(o => GanttSpan.FromDays(o.From, o.To)))
                .Add(x => x.ItemSegments, (Func<Order, IEnumerable<GanttSegment>?>)Stages)
                .Add(x => x.Regions, new[] { new GanttRegion("maintenance", GanttSpan.Day(Day(2)), "m2", "\u0422\u041E") })
                .Add(x => x.Links, new[] { new GanttLink("o1", "o2") })
                .Add(x => x.ShowCriticalPath, true)));

        Assert.Equal(["\u041F\u0440\u0435\u0441\u0441", "\u0424\u0440\u0435\u0437\u0430"], cut.FindAll("[role=rowheader]").Select(h => h.TextContent.Trim()));
        Assert.Equal(["setup", "run", "qa"], cut.FindAll(".flare-gantt__bar[data-key='o1'] .flare-gantt__segment").Select(s => s.GetAttribute("data-kind")));
        Assert.Equal("m2", cut.Find(".flare-gantt__region[data-kind='maintenance']").Closest(".flare-gantt__row")!.GetAttribute("data-row"));
        Assert.Single(cut.FindAll("path.flare-gantt__link"));
        Assert.Equal(["o1", "o2"], cut.FindAll(".flare-gantt__bar--critical").Select(b => b.GetAttribute("data-key")).Order());
    }

    [Fact]
    public void Meeting_rooms_on_an_hour_scale_with_occupancy_cells()
    {
        var day = new DateTime(2026, 10, 12);
        var rooms = new[] { new Room("\u0421\u0438\u043D\u044F\u044F"), new Room("\u0417\u0435\u043B\u0435\u043D\u0430\u044F") };
        var bookings = new[]
        {
            new Booking("b1", "\u0421\u0438\u043D\u044F\u044F", day.AddHours(9), day.AddHours(10.5)),
            new Booking("b2", "\u0421\u0438\u043D\u044F\u044F", day.AddHours(10), day.AddHours(11)),
            new Booking("b3", "\u0417\u0435\u043B\u0435\u043D\u0430\u044F", day.AddHours(12), day.AddHours(13))
        };
        var cells = Enumerable.Range(9, 4).Select(h => new GanttCell("\u0421\u0438\u043D\u044F\u044F", new GanttSpan(day.AddHours(h), day.AddHours(h + 1)),
            bookings.Any(b => b.RoomId == "\u0421\u0438\u043D\u044F\u044F" && b.From < day.AddHours(h + 1) && day.AddHours(h) < b.To) ? "busy" : "free")).ToArray();
        var changes = new List<GanttChangeEventArgs>();

        var cut = Render<FlareGantt>(p => p
            .Add(x => x.Range, new GanttSpan(day.AddHours(9), day.AddHours(13)))
            .Add(x => x.Scale, GanttScale.Hour)
            .Add(x => x.UnitWidth, 60d)
            .Add(x => x.RowHeight, 30d)
            .Add(x => x.Culture, Ru)
            .AddChildContent<FlareGanttPanel<Room, Booking>>(pp => pp
                .Add(x => x.Rows, rooms)
                .Add(x => x.RowKey, (Func<Room, string>)(r => r.Id))
                .Add(x => x.Items, bookings)
                .Add(x => x.ItemKey, (Func<Booking, string>)(b => b.Id))
                .Add(x => x.ItemRow, (Func<Booking, string>)(b => b.RoomId))
                .Add(x => x.ItemSpan, (Func<Booking, GanttSpan?>)(b => new GanttSpan(b.From, b.To)))
                .Add(x => x.Cells, cells)
                .Add(x => x.CellTemplate, (RenderFragment<GanttCell>)(c => b => b.AddContent(0, c.Kind == "busy" ? "\u0437\u0430\u043D\u044F\u0442\u043E" : "")))
                .Add(x => x.Editable, true)
                .Add(x => x.ScheduleMode, GanttScheduleMode.None)
                .Add(x => x.OnChange, (GanttChangeEventArgs e) => changes.Add(e))));

        Assert.Equal(["09:00", "10:00", "11:00", "12:00"], cut.FindAll(".flare-gantt__scale-row:not(.flare-gantt__scale-row--major) .flare-gantt__tick")
            .Select(t => t.TextContent.Trim()));
        Assert.Equal("height:90px", cut.Find(".flare-gantt__row[data-row='\u0421\u0438\u043D\u044F\u044F']").GetAttribute("style")); // cells + two lanes
        Assert.Equal(["busy", "busy", "free", "free"], cut.FindAll(".flare-gantt__cell").Select(c => c.GetAttribute("data-kind")));
        Assert.Equal("\u0437\u0430\u043D\u044F\u0442\u043E", cut.Find(".flare-gantt__cell").TextContent.Trim());
        Assert.Contains("left:0px;top:36px;width:90px", cut.Find(".flare-gantt__bar[data-key='b1']").ParentElement!.GetAttribute("style"));

        cut.Find(".flare-gantt__bar[data-key='b2']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown", CtrlKey = true });

        Assert.Equal("\u0417\u0435\u043B\u0435\u043D\u0430\u044F", Assert.Single(changes).Changes[0].RowKey);
    }

    [Fact]
    public void Roadmap_of_initiatives_and_teams_on_one_quarter_axis()
    {
        var initiatives = new[] { new Initiative("\u041F\u043B\u0430\u0442\u0444\u043E\u0440\u043C\u0430", new Initiative("e1"), new Initiative("e2")) };
        var deliveries = new[]
        {
            new Delivery("e1", "Core", new DateOnly(2026, 1, 1), new DateOnly(2026, 6, 30)),
            new Delivery("e2", "Web", new DateOnly(2026, 7, 1), new DateOnly(2026, 9, 30))
        };
        var teams = new[] { new Initiative("Core"), new Initiative("Web") };

        void Epics(RenderTreeBuilder b) => Panel(b, 0, initiatives, d => d.EpicId, rollup: true);
        void Teams(RenderTreeBuilder b) => Panel(b, 20, teams, d => d.Team, rollup: false);
        void Panel(RenderTreeBuilder b, int seq, Initiative[] rows, Func<Delivery, string> row, bool rollup)
        {
            b.OpenComponent<FlareGanttPanel<Initiative, Delivery>>(seq);
            b.AddAttribute(seq + 1, "Rows", rows);
            b.AddAttribute(seq + 2, "RowKey", (Func<Initiative, string>)(i => i.Id));
            b.AddAttribute(seq + 3, "RowChildren", (Func<Initiative, IEnumerable<Initiative>?>)(i => i.Epics));
            b.AddAttribute(seq + 4, "Items", deliveries);
            b.AddAttribute(seq + 5, "ItemKey", (Func<Delivery, string>)(d => d.EpicId));
            b.AddAttribute(seq + 6, "ItemRow", row);
            b.AddAttribute(seq + 7, "ItemSpan", (Func<Delivery, GanttSpan?>)(d => GanttSpan.FromDays(d.From, d.To)));
            b.AddAttribute(seq + 8, "RollupParents", rollup);
            b.CloseComponent();
        }

        var cut = Render<FlareGantt>(p => p
            .Add(x => x.Range, GanttSpan.FromDays(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31)))
            .Add(x => x.Scale, GanttScale.Quarter)
            .Add(x => x.UnitWidth, 100d)
            .Add(x => x.Culture, Ru)
            .Add(x => x.ChildContent, (RenderFragment)(b => { Epics(b); Teams(b); })));

        Assert.Single(cut.FindAll(".flare-gantt__header"));
        Assert.Equal(["Q1", "Q2", "Q3", "Q4"], cut.FindAll(".flare-gantt__scale-row:not(.flare-gantt__scale-row--major) .flare-gantt__tick").Select(t => t.TextContent.Trim()));
        var summary = cut.Find(".flare-gantt__bar--summary").ParentElement!.GetAttribute("style")!;
        Assert.Contains("left:0px", summary);
        Assert.Contains("width:300px", summary); // Q1 through Q3

        cut.FindAll(".flare-gantt__bar[data-key='e1']")[0].Click();
        Assert.Equal(2, cut.FindAll(".flare-gantt__bar--selected[data-key='e1']").Count);
    }

    [Fact]
    public void The_package_does_not_reference_its_incubator()
    {
        var assembly = typeof(FlareGantt).Assembly;
        Assert.DoesNotContain(assembly.GetReferencedAssemblies(), a => a.Name!.StartsWith("Alva", StringComparison.Ordinal));
        Assert.Equal(CultureInfo.InvariantCulture.Name, assembly.GetName().CultureName ?? "");
    }
}
