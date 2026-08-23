namespace Flare.Components;

/// <summary>
/// Page navigation for a <c>FlareDataGrid</c>, placed wherever the layout wants it rather than where the
/// grid would put it. The grid keeps owning the page and page size - the data concern - while this owns
/// how paging is presented. It renders nothing when there is nothing to navigate: a virtualized or
/// infinite-scroll grid, or a single page with no page-size choice.
/// </summary>
/// <typeparam name="TItem">Row type of the grid being paged.</typeparam>
public partial class FlareDataGridPager<TItem>;
