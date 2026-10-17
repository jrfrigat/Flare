namespace Flare.Components.Gantt;

/// <summary>
/// Half-open time interval [<see cref="Start"/>, <see cref="End"/>) on a zone-less clock. Whole-day work uses
/// <see cref="Day"/> and <see cref="FromDays"/>; instants use <see cref="FromOffsets"/>.
/// </summary>
/// <param name="Start">Inclusive start.</param>
/// <param name="End">Exclusive end; equal to <paramref name="Start"/> for a zero-length point.</param>
public readonly record struct GanttSpan(DateTime Start, DateTime End)
{
    /// <summary>Length of the interval.</summary>
    public TimeSpan Duration => End - Start;

    /// <summary>One whole day.</summary>
    public static GanttSpan Day(DateOnly day) => FromDays(day, day);

    /// <summary>Whole days from <paramref name="first"/> through <paramref name="lastInclusive"/>.</summary>
    public static GanttSpan FromDays(DateOnly first, DateOnly lastInclusive) =>
        new(first.ToDateTime(TimeOnly.MinValue), lastInclusive.AddDays(1).ToDateTime(TimeOnly.MinValue));

    /// <summary>Two instants shown on the clock of <paramref name="zone"/>.</summary>
    public static GanttSpan FromOffsets(DateTimeOffset start, DateTimeOffset end, TimeZoneInfo zone) =>
        new(TimeZoneInfo.ConvertTime(start, zone).DateTime, TimeZoneInfo.ConvertTime(end, zone).DateTime);

    /// <summary>True when the two intervals share time; touching ends do not overlap.</summary>
    public bool Overlaps(GanttSpan other) => Start < other.End && other.Start < End;

    /// <summary>The same interval moved by <paramref name="delta"/>.</summary>
    public GanttSpan Shift(TimeSpan delta) => new(Start + delta, End + delta);
}

/// <summary>Step of the time axis.</summary>
public enum GanttScale
{
    /// <summary>One hour per unit.</summary>
    Hour,
    /// <summary>One day per unit.</summary>
    Day,
    /// <summary>One week per unit, starting on the culture's first day of week.</summary>
    Week,
    /// <summary>One calendar month per unit.</summary>
    Month,
    /// <summary>One calendar quarter per unit.</summary>
    Quarter,
    /// <summary>One calendar year per unit.</summary>
    Year
}

/// <summary>Dependency type between two items.</summary>
public enum GanttLinkType
{
    /// <summary>The successor starts after the predecessor finishes.</summary>
    FinishToStart,
    /// <summary>The successor starts after the predecessor starts.</summary>
    StartToStart,
    /// <summary>The successor finishes after the predecessor finishes.</summary>
    FinishToFinish,
    /// <summary>The successor finishes after the predecessor starts.</summary>
    StartToFinish
}

/// <summary>How a change to one item moves the items that depend on it.</summary>
public enum GanttScheduleMode
{
    /// <summary>Only the changed item moves.</summary>
    None,
    /// <summary>Successors whose constraint is broken are pushed later, keeping their duration.</summary>
    Push
}

/// <summary>A part of a bar with its own look, such as work, waiting or overflow.</summary>
/// <param name="Kind">Consumer key rendered as a CSS modifier and <c>data-kind</c>.</param>
/// <param name="Span">Time covered by the part.</param>
/// <param name="Label">Optional text shown inside the part.</param>
public sealed record GanttSegment(string Kind, GanttSpan Span, string? Label = null);

/// <summary>A point in time: a milestone, a deadline, "today".</summary>
/// <param name="Kind">Consumer key rendered as a CSS modifier and <c>data-kind</c>.</param>
/// <param name="At">Moment of the marker.</param>
/// <param name="RowKey">Row the marker belongs to; null draws it across every row.</param>
/// <param name="Label">Optional caption.</param>
/// <param name="Key">Optional key, for example the item the marker belongs to.</param>
public sealed record GanttMarker(string Kind, DateTime At, string? RowKey = null, string? Label = null, string? Key = null);

/// <summary>A highlighted range of the axis: a sprint window, a holiday, an absence.</summary>
/// <param name="Kind">Consumer key rendered as a CSS modifier and <c>data-kind</c>.</param>
/// <param name="Span">Time covered by the region.</param>
/// <param name="RowKey">Row the region belongs to; null spans every row.</param>
/// <param name="Label">Optional caption.</param>
public sealed record GanttRegion(string Kind, GanttSpan Span, string? RowKey = null, string? Label = null);

/// <summary>A value of a row over one stretch of the axis, such as hours per day with a status.</summary>
/// <param name="RowKey">Row the cell belongs to.</param>
/// <param name="Span">Time covered by the cell, usually one axis unit.</param>
/// <param name="Kind">Consumer status key rendered as a CSS modifier and <c>data-kind</c>.</param>
/// <param name="Text">Text shown in the cell.</param>
/// <param name="Title">Optional tooltip text.</param>
public sealed record GanttCell(string RowKey, GanttSpan Span, string Kind, string? Text = null, string? Title = null);

/// <summary>What a legend sample is drawn as: the same element the chart draws, so it takes the same look.</summary>
public enum GanttLegendShape
{
    /// <summary>An item bar of the kind.</summary>
    Bar,
    /// <summary>A segment of the kind over a bar of <see cref="GanttLegendItem.BarKind"/>.</summary>
    Segment,
    /// <summary>A suggested (ghost) bar.</summary>
    Suggested,
    /// <summary>A region of the kind.</summary>
    Region,
    /// <summary>A value cell of the kind.</summary>
    Cell
}

/// <summary>One entry of <c>FlareGanttLegend</c>: a sample of a kind drawn as <paramref name="Shape"/>, a label and an optional hint.</summary>
public sealed record GanttLegendItem(string Kind, string Label, GanttLegendShape Shape = GanttLegendShape.Bar, string? Title = null, string? BarKind = null);

/// <summary>A dependency from one item to another.</summary>
/// <param name="FromKey">Predecessor item key.</param>
/// <param name="ToKey">Successor item key.</param>
/// <param name="Type">Dependency type.</param>
/// <param name="Lag">Extra time the constraint requires; negative for lead.</param>
/// <param name="Kind">Optional consumer key for the connector's look.</param>
public sealed record GanttLink(string FromKey, string ToKey, GanttLinkType Type = GanttLinkType.FinishToStart, TimeSpan Lag = default, string? Kind = null);

/// <summary>A new position of one item: its span and, when it moved between rows, its row.</summary>
/// <param name="ItemKey">Key of the item.</param>
/// <param name="Span">New span.</param>
/// <param name="RowKey">New row, or null when the row did not change.</param>
public sealed record GanttChange(string ItemKey, GanttSpan Span, string? RowKey = null);

/// <summary>Why a proposed link was refused.</summary>
public enum GanttValidationCode
{
    /// <summary>The link is allowed.</summary>
    Valid,
    /// <summary>The link points from an item to itself.</summary>
    SelfLink,
    /// <summary>The link closes a cycle.</summary>
    Cycle,
    /// <summary>The same link already exists.</summary>
    Duplicate
}

/// <summary>Outcome of checking a proposed link; never thrown.</summary>
/// <param name="Code">Result code.</param>
/// <param name="Path">For a cycle, the item keys that form it, ending where it started.</param>
public sealed record GanttValidation(GanttValidationCode Code, IReadOnlyList<string> Path)
{
    /// <summary>A link that may be created.</summary>
    public static GanttValidation Ok { get; } = new(GanttValidationCode.Valid, []);

    /// <summary>True when the link may be created.</summary>
    public bool IsValid => Code == GanttValidationCode.Valid;
}

/// <summary>Rectangle of a bar on the drawing surface, in pixels.</summary>
/// <param name="X">Left edge.</param>
/// <param name="Y">Top edge.</param>
/// <param name="Width">Width.</param>
/// <param name="Height">Height.</param>
public readonly record struct GanttBox(double X, double Y, double Width, double Height)
{
    /// <summary>Right edge.</summary>
    public double Right => X + Width;

    /// <summary>Vertical centre.</summary>
    public double MiddleY => Y + Height / 2;
}
