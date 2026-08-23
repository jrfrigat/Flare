namespace Flare.Components;

/// <summary>
/// Named advanced filters offered as a single choice, so a reader picks "Overdue" instead of rebuilding
/// its condition tree. Choosing an entry applies that filter group to the <c>FlareDataGrid</c>; the
/// leading empty entry clears it.
/// </summary>
/// <typeparam name="TItem">Row type of the grid being filtered.</typeparam>
public partial class FlareDataGridFilterPresets<TItem>;
