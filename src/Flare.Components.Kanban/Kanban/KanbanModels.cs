namespace Flare.Components;

public sealed record KanbanColumn(string Id, string Title, string? Color = null);

public sealed record KanbanCard(
    string Id,
    string ColumnId,
    string Title,
    string? Description = null,
    string? Tag = null,
    string? TagColor = null);
