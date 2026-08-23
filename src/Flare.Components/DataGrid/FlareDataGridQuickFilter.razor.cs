namespace Flare.Components;

/// <summary>
/// A debounced search box that narrows a <c>FlareDataGrid</c> to the rows where any visible column
/// contains the typed text, case-insensitively. Placed in the grid's toolbar it finds the grid through
/// the cascade; anywhere else it is pointed at one explicitly.
/// </summary>
/// <typeparam name="TItem">Row type of the grid being filtered.</typeparam>
public partial class FlareDataGridQuickFilter<TItem>;
