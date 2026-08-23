namespace Flare.Components;

/// <summary>
/// One data column of a <c>FlareDataGrid</c>: what to read from a row, how to display and edit it, and
/// which of sorting, filtering, grouping and aggregation it takes part in. Declaring it registers the
/// column with the enclosing grid, band or composite row - the markup is a description, not the cells
/// themselves, which the grid draws.
/// </summary>
/// <typeparam name="TItem">Row type of the grid this column belongs to.</typeparam>
public partial class FlareColumn<TItem>;
