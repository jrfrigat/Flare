namespace Flare.Components;

public sealed record KanbanColumn(string Id, string Title, string? Color = null);

public sealed record KanbanCard(
    string Id,
    string ColumnId,
    string Title,
    string? Description = null,
    string? Tag = null,
    string? TagColor = null);

/// <summary>Supplies a column and its current cards to a custom Kanban column header.</summary>
/// <param name="Column">The column whose header is being rendered.</param>
/// <param name="Cards">The cards currently displayed in the column.</param>
public sealed record KanbanColumnTemplateContext(
    KanbanColumn Column,
    IReadOnlyList<KanbanCard> Cards);
