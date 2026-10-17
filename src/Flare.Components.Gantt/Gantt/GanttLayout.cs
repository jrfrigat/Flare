namespace Flare.Components.Gantt;

/// <summary>A bar placed on the drawing surface of a panel.</summary>
internal sealed class GanttBarLayout<TItem>
{
    public required string Key { get; init; }
    public required string RowKey { get; init; }
    public TItem? Item { get; init; }
    public required GanttSpan Span { get; init; }
    public string? Kind { get; init; }
    public string? Label { get; init; }
    public IReadOnlyList<GanttSegment> Segments { get; init; } = [];
    public double? Progress { get; init; }
    public bool IsSummary { get; init; }
    public bool IsSuggested { get; init; }
    public bool IsPreview { get; init; }
    public int Lane { get; set; }

    /// <summary>Box relative to the panel body's time area.</summary>
    public GanttBox Box { get; set; }

    /// <summary>Box relative to the row track.</summary>
    public GanttBox Local { get; set; }
}

/// <summary>A visible row of a panel with everything drawn on it.</summary>
internal sealed class GanttRowLayout<TRow, TItem>
{
    public required string Key { get; init; }
    public required TRow Row { get; init; }
    public int Depth { get; init; }
    public bool HasChildren { get; init; }
    public bool Expanded { get; init; }
    public double Top { get; set; }
    public double Height { get; set; }
    public double LanesTop { get; set; }
    public List<GanttBarLayout<TItem>> Bars { get; } = [];
    public List<GanttCell> Cells { get; } = [];
    public List<GanttRegion> Regions { get; } = [];
    public List<GanttMarker> Markers { get; } = [];
}

/// <summary>A connector between two placed bars.</summary>
internal sealed record GanttLinkLayout(GanttLink Link, string Path, bool Critical);
