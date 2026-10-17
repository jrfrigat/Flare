using Flare.Components.Gantt;
using Flare.Gallery.Resources;

namespace Flare.Gallery.Pages.Components.Gantt.Examples;

/// <summary>A row of the work breakdown: a release, a stream of work or a task.</summary>
public sealed record GanttNode(string Key, string Title, params GanttNode[] Children);

/// <summary>A person whose lane shows the work assigned to them.</summary>
public sealed record GanttPerson(string Key, string Name);

/// <summary>One piece of work: where it sits in the breakdown, who does it and when.</summary>
public sealed record GanttWork(string Key, string Title, string Node, string Person, GanttSpan Span, double Progress, GanttSpan? Waiting = null);

/// <summary>
/// A small release plan the Gantt demos share: a work breakdown, three people, dependencies and the sprint, around
/// the current week so "today" is on the axis. The chart never changes the data; <see cref="Apply"/> is what the page
/// does with a change it accepts.
/// </summary>
public sealed class GanttSample
{
    /// <summary>Hours a person works on a working day.</summary>
    public const int DayHours = 8;

    public GanttSample()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        Start = today.AddDays(-(((int)today.DayOfWeek + 6) % 7) - 7);
        Range = GanttSpan.FromDays(Start, Start.AddDays(27));

        Tree =
        [
            new GanttNode("release", GalleryStrings.Gantt_Release,
                new GanttNode("backend", GalleryStrings.Gantt_Backend,
                    new GanttNode("design", GalleryStrings.Gantt_TaskDesign),
                    new GanttNode("api", GalleryStrings.Gantt_TaskApi)),
                new GanttNode("frontend", GalleryStrings.Gantt_Frontend,
                    new GanttNode("ui", GalleryStrings.Gantt_TaskUi),
                    new GanttNode("tests", GalleryStrings.Gantt_TaskTests)),
                new GanttNode("ship", GalleryStrings.Gantt_TaskShip)),
        ];
        People =
        [
            new GanttPerson("anna", GalleryStrings.Gantt_PersonAnna),
            new GanttPerson("boris", GalleryStrings.Gantt_PersonBoris),
            new GanttPerson("vera", GalleryStrings.Gantt_PersonVera),
        ];
        Work =
        [
            new("design", GalleryStrings.Gantt_TaskDesign, "design", "anna", Days(0, 3), 1),
            new("api", GalleryStrings.Gantt_TaskApi, "api", "anna", Days(7, 11), 0.5, Days(9, 9)),
            new("ui", GalleryStrings.Gantt_TaskUi, "ui", "boris", Days(7, 15), 0.3),
            new("tests", GalleryStrings.Gantt_TaskTests, "tests", "vera", Days(16, 19), 0),
            new("ship", GalleryStrings.Gantt_TaskShip, "ship", "vera", Days(21, 21), 0),
        ];
        Links =
        [
            new GanttLink("design", "api"),
            new GanttLink("api", "tests"),
            new GanttLink("ui", "tests"),
            new GanttLink("tests", "ship"),
        ];
    }

    /// <summary>Monday of the week before this one: the first day on the axis.</summary>
    public DateOnly Start { get; }

    /// <summary>Four weeks of time.</summary>
    public GanttSpan Range { get; }

    public GanttNode[] Tree { get; }

    public GanttPerson[] People { get; }

    public List<GanttWork> Work { get; private set; }

    public List<GanttLink> Links { get; private set; }

    /// <summary>The sprint the release is planned in, across every panel.</summary>
    public GanttRegion[] Sprint => [new("sprint", Days(7, 18), Label: GalleryStrings.Gantt_Sprint)];

    /// <summary>"Today", across every panel.</summary>
    public GanttMarker[] Today => [new("today", DateTime.Today, Label: GalleryStrings.Gantt_Today)];

    public static bool IsWeekend(DateOnly day) => day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>The waiting part of a piece of work, drawn over its bar.</summary>
    public static IEnumerable<GanttSegment>? Segments(GanttWork work) =>
        work.Waiting is { } waiting ? [new GanttSegment("waiting", waiting, GalleryStrings.Gantt_Waiting)] : null;

    /// <summary>Hours assigned to each person on each working day, marked when more than a day's work.</summary>
    public IEnumerable<GanttCell> Load()
    {
        for (var day = Start; day.ToDateTime(TimeOnly.MinValue) < Range.End; day = day.AddDays(1))
        {
            if (IsWeekend(day)) continue;
            var span = GanttSpan.Day(day);
            foreach (var person in People)
            {
                var hours = Work.Count(w => w.Person == person.Key && w.Span.Overlaps(span)) * DayHours;
                if (hours == 0) continue;
                yield return new GanttCell(person.Key, span, hours > DayHours ? "overload" : "ok", hours.ToString(),
                    string.Format(GalleryStrings.Gantt_HoursTitle, hours, DayHours));
            }
        }
    }

    /// <summary>Applies the positions the chart proposed: new dates, and a new person for a bar moved between lanes.</summary>
    public void Apply(GanttChangeEventArgs change) =>
        Work = Work.Select(w => change.Changes.FirstOrDefault(c => c.ItemKey == w.Key) is { } c
            ? w with { Span = c.Span, Person = c.RowKey ?? w.Person }
            : w).ToList();

    /// <summary>Adds a link the user drew.</summary>
    public void Add(GanttLink link) => Links = [.. Links, link];

    private GanttSpan Days(int first, int last) => GanttSpan.FromDays(Start.AddDays(first), Start.AddDays(last));
}
