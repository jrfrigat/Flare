using System.Globalization;

namespace Flare.Components.Gantt;

/// <summary>What the user did to an item.</summary>
public enum GanttEditAction
{
    /// <summary>Moved in time, between rows, or both.</summary>
    Move,
    /// <summary>Changed the start.</summary>
    ResizeStart,
    /// <summary>Changed the end.</summary>
    ResizeEnd
}

/// <summary>
/// A proposed change. The component never changes the consumer's data: it reports the new positions here, and
/// the consumer applies them (or not). <see cref="Cancel"/> only tells the component not to announce the change.
/// </summary>
/// <param name="ItemKey">The item the user acted on.</param>
/// <param name="Action">What the user did.</param>
/// <param name="Changes">The item's new position first, then every successor the schedule mode pushed.</param>
public sealed class GanttChangeEventArgs(string ItemKey, GanttEditAction Action, IReadOnlyList<GanttChange> Changes)
{
    /// <summary>The item the user acted on.</summary>
    public string ItemKey { get; } = ItemKey;

    /// <summary>What the user did.</summary>
    public GanttEditAction Action { get; } = Action;

    /// <summary>The item's new position first, then every successor the schedule mode pushed.</summary>
    public IReadOnlyList<GanttChange> Changes { get; } = Changes;

    /// <summary>Set by the handler to refuse the change.</summary>
    public bool Cancel { get; set; }
}

/// <summary>A link the user asked to create; it already passed the cycle check.</summary>
/// <param name="Link">The new link.</param>
public sealed class GanttLinkEventArgs(GanttLink Link)
{
    /// <summary>The new link.</summary>
    public GanttLink Link { get; } = Link;

    /// <summary>Set by the handler to refuse the link.</summary>
    public bool Cancel { get; set; }
}

/// <summary>A link that was refused, with the reason in words.</summary>
/// <param name="Link">The refused link.</param>
/// <param name="Validation">Why it was refused.</param>
/// <param name="Message">The localized message, also announced to assistive technology.</param>
public sealed record GanttLinkRejectedEventArgs(GanttLink Link, GanttValidation Validation, string Message);

/// <summary>
/// A row and one axis unit a payload was dropped on - what <c>FlareGantt.ResolveDrop</c> returns for a row zone. A consumer
/// wraps its page in its own <c>FlareDragContext&lt;TPayload&gt;</c> and resolves <c>TargetId</c> in its drop handler.
/// </summary>
/// <param name="RowKey">Row the payload was dropped on.</param>
/// <param name="Span">Axis unit the payload was dropped on.</param>
public sealed record GanttDropTarget(string RowKey, GanttSpan Span)
{
    private const char Separator = '\u001F';

    /// <summary>The zone target for a row and a span.</summary>
    public static string Format(string rowKey, GanttSpan span) =>
        string.Join(Separator, rowKey, span.Start.ToString("o", CultureInfo.InvariantCulture), span.End.ToString("o", CultureInfo.InvariantCulture));

    /// <summary>Reads a target written by <see cref="Format"/>; false for any other string.</summary>
    public static bool TryParse(string? targetId, out GanttDropTarget? target)
    {
        target = null;
        var parts = targetId?.Split(Separator);
        if (parts is not { Length: 3 }
            || !DateTime.TryParse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var start)
            || !DateTime.TryParse(parts[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var end))
            return false;
        target = new GanttDropTarget(parts[0], new GanttSpan(start, end));
        return true;
    }
}

/// <summary>
/// Target of the drop zone of a whole row, which <c>DropGroup</c> renders: one zone per row keeps a chart of hundreds
/// of rows cheap. The axis unit is not in the target - <c>FlareGantt.ResolveDrop</c> finds it from where the pointer
/// was released.
/// </summary>
/// <param name="RowKey">Row the payload was dropped on.</param>
public sealed record GanttRowDropTarget(string RowKey)
{
    private const char Marker = '\u001D';

    /// <summary>The zone target for a row.</summary>
    public static string Format(string rowKey) => Marker + rowKey;

    /// <summary>Reads a target written by <see cref="Format"/>; false for any other string.</summary>
    public static bool TryParse(string? targetId, out GanttRowDropTarget? target)
    {
        target = targetId is { Length: > 1 } && targetId[0] == Marker ? new GanttRowDropTarget(targetId[1..]) : null;
        return target is not null;
    }
}

/// <summary>
/// Target of a drop zone on one item's bar, encoded in the zone's string <c>Target</c> when the panel's
/// <c>ItemDropGroup</c> is set: dropping something onto the item itself, such as a person onto a task.
/// </summary>
/// <param name="ItemKey">Item the payload was dropped on.</param>
public sealed record GanttItemDropTarget(string ItemKey)
{
    private const char Marker = '\u001E';

    /// <summary>The zone target for an item.</summary>
    public static string Format(string itemKey) => Marker + itemKey;

    /// <summary>Reads a target written by <see cref="Format"/>; false for any other string.</summary>
    public static bool TryParse(string? targetId, out GanttItemDropTarget? target)
    {
        target = targetId is { Length: > 1 } && targetId[0] == Marker ? new GanttItemDropTarget(targetId[1..]) : null;
        return target is not null;
    }
}
