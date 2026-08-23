namespace Flare.Components;

/// <summary>
/// Shared heading node of a <c>FlareDataGrid</c>: the geometry, identity and pinning every kind of
/// header carries, whether it ends up drawing cells (a column) or only grouping others (a band or a
/// composite row). Declaration order is captured at construction, so the grid can rebuild the header
/// tree in the order the markup states it.
/// </summary>
public partial class FlareColumnBase;
