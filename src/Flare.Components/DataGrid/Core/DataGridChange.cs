namespace Flare.Components;

/// <summary>
/// What changed in a <see cref="DataGridContext{TItem}"/>, as a bit mask. Raised by
/// <see cref="DataGridContext{TItem}.Changed"/> and matched against
/// <c>FlareDataGridControl&lt;TItem&gt;.Observes</c> so a control re-renders only for the changes it
/// actually shows. Reading the changed values is a separate, explicit step: the notification carries
/// flags rather than a state snapshot so a burst of keystrokes in a filter box costs no allocation.
/// </summary>
[Flags]
public enum DataGridChange
{
    /// <summary>Nothing changed.</summary>
    None = 0,
    /// <summary>The sort stack changed (a column was sorted, re-sorted or unsorted).</summary>
    Sort = 1,
    /// <summary>A column filter, the quick filter or the advanced filter tree changed.</summary>
    Filter = 2,
    /// <summary>The current page or the page size changed.</summary>
    Page = 4,
    /// <summary>The column set changed: registration, visibility, display order or width.</summary>
    Columns = 8,
    /// <summary>The selected rows changed.</summary>
    Selection = 16,
    /// <summary>The group-by keys changed.</summary>
    Grouping = 32,
    /// <summary>The rows or the total row count changed (a provider load, a refresh, new Items).</summary>
    Data = 64,
    /// <summary>A row or cell entered, left or committed edit mode.</summary>
    Editing = 128,
    /// <summary>Every change kind. The default for a control that does not narrow its interest.</summary>
    All = Sort | Filter | Page | Columns | Selection | Grouping | Data | Editing,
}
