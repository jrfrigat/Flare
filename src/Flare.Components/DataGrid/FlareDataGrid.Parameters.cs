using Microsoft.AspNetCore.Components;

namespace Flare.Components;

// Public [Parameter] surface of FlareDataGrid (data source, paging, selection, editing, theming,
// server-side change events). Split out of FlareDataGrid.razor for readability.
public partial class FlareDataGrid<TItem>
{
    /// <summary>In-memory data source bound to the grid.</summary>
    [Parameter] public IEnumerable<TItem>? Items { get; set; }
    /// <summary>Server-side data provider for paging and sorting.</summary>
    [Parameter] public Func<DataGridRequest, Task<DataGridResult<TItem>>>? ItemsProvider { get; set; }
    /// <summary>A queryable data source (e.g. an EF Core <c>DbSet</c>). When set, the grid translates
    /// its sort/filter/paging into a LINQ expression tree via <see cref="DataGridQuery"/> and runs it
    /// against this query - so the database does the work, with no hand-written <see cref="ItemsProvider"/>.
    /// An explicit <see cref="ItemsProvider"/> takes precedence. Executes synchronously; for async EF
    /// execution supply an <see cref="ItemsProvider"/> instead.</summary>
    [Parameter] public IQueryable<TItem>? Queryable { get; set; }
    /// <summary>FlareColumn descriptors defining the grid columns.</summary>
    [Parameter] public RenderFragment? Columns { get; set; }
    /// <summary>Number of rows displayed per page.</summary>
    [Parameter] public int PageSize { get; set; } = 10;
    /// <summary>Custom content displayed when no data rows are available. Defaults to FlareEmptyState.</summary>
    [Parameter] public RenderFragment? EmptyStateContent { get; set; }
    /// <summary>Overrides the loading indicator text.</summary>
    [Parameter] public string? LoadingText { get; set; }
    /// <summary>Overrides the placeholder text shown in filter inputs.</summary>
    [Parameter] public string? FilterPlaceholder { get; set; }
    /// <summary>Overrides the rows-per-page label shown next to the page size selector.</summary>
    [Parameter] public string? RowsLabel { get; set; }
    /// <summary>Enables virtual scrolling instead of pagination (consistent with FlareList.Virtual).
    /// Combine with an <see cref="ItemsProvider"/> to load rows on demand as the user scrolls.</summary>
    [Parameter] public bool Virtual { get; set; }
    /// <summary>Infinite scrolling: with an <see cref="ItemsProvider"/>, rows accumulate page by page
    /// as the user scrolls to the bottom (instead of pagination or a known-total virtual window).
    /// The provider is called with successive <c>Page</c> values; loading stops when a page returns
    /// fewer than <see cref="PageSize"/> rows or the reported total is reached.</summary>
    [Parameter] public bool InfiniteScroll { get; set; }
    /// <summary>CSS height of the table container. Works with or without virtualization.</summary>
    [Parameter] public string Height { get; set; } = "400px";
    /// <summary>How the loading state is shown: a circular ring (default), skeleton rows, or text only.</summary>
    [Parameter] public DataGridLoadingIndicator LoadingIndicator { get; set; } = DataGridLoadingIndicator.Spinner;
    /// <summary>
    /// Estimated row height in pixels for the virtual scroller.
    /// <list type="bullet">
    ///   <item><description><c>null</c> (default): auto-detect via ResizeObserver - works for variable-height rows.</description></item>
    ///   <item><description><c>&gt; 0</c>: fixed height - faster rendering, use when all rows have the same height (e.g. 44px for compact tables).</description></item>
    /// </list>
    /// </summary>
    [Parameter] public float? VirtualItemSize { get; set; }
    /// <summary>
    /// Number of extra rows to render above/below the visible area (also fetched for <see cref="ItemsProvider"/>).
    /// <list type="bullet">
    ///   <item><description><c>null</c> (default): use 20 - pre-loads 20 rows in each direction for smooth scrolling.</description></item>
    ///   <item><description><c>&gt; 0</c>: custom overscan count for fine-tuning.</description></item>
    /// </list>
    /// </summary>
    [Parameter] public int? OverscanCount { get; set; }
    /// <summary>Enables drag-and-drop reordering of columns by dragging their headers.</summary>
    [Parameter] public bool ReorderableColumns { get; set; }
    /// <summary>Controlled column display order by column key - <c>Id</c>, else <c>SortKey</c>, else
    /// <c>Title</c> (left to right). Columns not listed keep their declared order at the end. When set,
    /// treat the grid as controlled and update this value from <see cref="OnColumnOrderChanged"/>.</summary>
    [Parameter] public IReadOnlyList<string>? ColumnOrder { get; set; }
    /// <summary>Raised after the user reorders columns, with the new left-to-right order (column keys).</summary>
    [Parameter] public EventCallback<IReadOnlyList<string>> OnColumnOrderChanged { get; set; }
    /// <summary>Enables drag-and-drop reordering of rows. Reordering raises <see cref="OnRowReordered"/>;
    /// the owner updates the backing data (the grid does not mutate <see cref="Items"/>).</summary>
    [Parameter] public bool RowReorderable { get; set; }
    /// <summary>Raised after the user drags a row onto another row, with the dragged item, the drop
    /// target, and their indices within the current page.</summary>
    [Parameter] public EventCallback<DataGridRowReorder<TItem>> OnRowReordered { get; set; }
    /// <summary>Grouping levels, declared as nested <see cref="DataGridGroup{TItem}"/> child components
    /// (outermost first). When present, rows are grouped into nested, collapsible sections with optional
    /// per-level or grid-level aggregates. The level keys also flow to a server provider via
    /// <c>DataGridRequest.GroupKeys</c>.
    /// <example><code>
    /// &lt;Grouping&gt;
    ///   &lt;DataGridGroup Key="Role" Selector="@(p =&gt; p.Role)" /&gt;
    ///   &lt;DataGridGroup Key="City" Selector="@(p =&gt; p.City)" /&gt;
    /// &lt;/Grouping&gt;
    /// </code></example></summary>
    [Parameter] public RenderFragment? Grouping { get; set; }

    /// <summary>Tree-grid configuration for hierarchical data. When set, enables expand/collapse UI
    /// and recursive row rendering based on the ChildrenSelector.</summary>
    [Parameter] public DataGridTreeConfig<TItem>? Tree { get; set; }

    /// <summary>Row selection mode - none, single, or multiple.</summary>
    [Parameter] public SelectionMode SelectionMode { get; set; } = SelectionMode.None;
    /// <summary>Enables Excel-like cell-range selection. With the grid focused, arrow keys move the
    /// active cell, Shift+Arrow (and Shift+Home/End) extend a rectangular selection, click-drag and
    /// Shift+Click select with the mouse, Ctrl+C copies the selection to the clipboard as tab-separated
    /// text and (when <see cref="OnPaste"/> is set) Ctrl+V pastes tab-separated text into the cells.</summary>
    [Parameter] public bool CellSelection { get; set; }
    /// <summary>Rows pinned to the top of the grid - always shown above the scrolling data, outside
    /// sorting, filtering and paging (e.g. a totals or highlighted row). Sticky in scrolling grids.</summary>
    [Parameter] public IReadOnlyList<TItem>? PinnedTopRows { get; set; }
    /// <summary>Rows pinned to the bottom of the grid - always shown below the scrolling data, outside
    /// sorting, filtering and paging. Sticky in scrolling grids (above any aggregate row).</summary>
    [Parameter] public IReadOnlyList<TItem>? PinnedBottomRows { get; set; }
    /// <summary>Raised on Ctrl+V when <see cref="CellSelection"/> is enabled: the clipboard's
    /// tab-separated text is parsed and mapped onto the cells starting at the top-left of the current
    /// selection. The handler applies each cell's value to its row item (the grid cannot write through
    /// the read-only column accessors itself).</summary>
    [Parameter] public EventCallback<DataGridPaste<TItem>> OnPaste { get; set; }
    /// <summary>Currently selected items for two-way binding.</summary>
    [Parameter] public HashSet<TItem>? SelectedItems { get; set; }
    /// <summary>Callback fired when the selected items set changes.</summary>
    [Parameter] public EventCallback<HashSet<TItem>> SelectedItemsChanged { get; set; }
    /// <summary>Callback fired when a row is clicked.</summary>
    [Parameter] public EventCallback<TItem> RowClick { get; set; }
    /// <summary>Template rendered as an expandable detail row below each data row.</summary>
    [Parameter] public RenderFragment<TItem>? RowDetailTemplate { get; set; }
    /// <summary>Custom content rendered in the toolbar area above the table. Child components placed
    /// here (quick filter, presets, export, filter builder, column picker, pager) resolve the grid
    /// automatically via the cascade - no <c>Grid</c> binding needed.</summary>
    [Parameter] public RenderFragment? ToolbarContent { get; set; }

    /// <summary>Custom content rendered in the footer area below the table (the toolbar's counterpart
    /// at the bottom). Like <see cref="ToolbarContent"/>, child components placed here resolve the grid
    /// automatically via the cascade. Use it to position grid controls (e.g. a
    /// <see cref="FlareDataGridPager{TItem}"/>) below the data.</summary>
    [Parameter] public RenderFragment? FooterContent { get; set; }

    /// <summary>Whether the grid renders its own built-in pager below the table (default true). Set to
    /// false to suppress it and place a <see cref="FlareDataGridPager{TItem}"/> yourself (e.g. in
    /// <see cref="FooterContent"/> or <see cref="ToolbarContent"/>).</summary>
    [Parameter] public bool ShowPager { get; set; } = true;

    /// <summary>Edit mode: None (default), Inline (row), or Cell (click individual cells).</summary>
    [Parameter] public DataGridEditMode EditMode { get; set; } = DataGridEditMode.None;
    /// <summary>Callback fired when a single row is saved (inline edit).</summary>
    [Parameter] public EventCallback<TItem> OnRowSaved { get; set; }
    /// <summary>Callback fired when a single row edit is cancelled.</summary>
    [Parameter] public EventCallback<TItem> OnRowCancelled { get; set; }
    /// <summary>Callback raised when batch save is requested. The parameter contains all edited items with their edit values.</summary>
    [Parameter] public EventCallback<IReadOnlyDictionary<TItem, IReadOnlyDictionary<string, string>>> OnBatchSave { get; set; }
    /// <summary>Callback raised when batch edit is cancelled.</summary>
    [Parameter] public EventCallback OnBatchCancel { get; set; }
    /// <summary>Optional predicate applied to every row after column filters; rows returning false are hidden.</summary>
    [Parameter] public Func<TItem, bool>? QuickFilter { get; set; }
    /// <summary>Returns extra CSS class string(s) for each data row, given the row item. Use for
    /// row-level conditional formatting (e.g. highlight rows by status).</summary>
    [Parameter] public Func<TItem, string>? RowClassFunc { get; set; }
    /// <summary>Returns an inline style string for each data row, given the row item.</summary>
    [Parameter] public Func<TItem, string>? RowStyleFunc { get; set; }
    /// <summary>Available page-size choices rendered in the pagination row-count selector. When empty the selector is hidden.</summary>
    [Parameter] public IReadOnlyList<int> RowsPerPageOptions { get; set; } = [];
    /// <summary>Aggregate definitions rendered as a footer row (Sum/Count/Average/Min/Max per column).</summary>
    [Parameter] public IReadOnlyList<AggregateDefinition<TItem>>? Aggregates { get; set; }

    // -- Appearance -----------------------------------------------------------

    /// <summary>Alternating row background ("zebra" striping).</summary>
    [Parameter] public bool Striped { get; set; }
    /// <summary>Highlights the row under the pointer on hover.</summary>
    [Parameter] public bool Hoverable { get; set; }
    /// <summary>Compact row height and cell padding for dense tables.</summary>
    [Parameter] public bool Dense { get; set; }
    /// <summary>Draws vertical borders between columns.</summary>
    [Parameter] public bool Bordered { get; set; }
    /// <summary>Stable identity selector for each row, used as the render key (improves diffing,
    /// selection and virtualization). Defaults to the item reference when not set.</summary>
    [Parameter] public Func<TItem, object>? RowKey { get; set; }

    /// <summary>Controls how column filters are displayed: Simple (text inputs), Menu (dropdowns), or Both.</summary>
    [Parameter] public DataGridFilterMode FilterMode { get; set; } = DataGridFilterMode.Simple;

    // -- Column visibility ----------------------------------------------------

    /// <summary>Shows a built-in "Columns" button in the toolbar that opens a checklist to show/hide
    /// individual columns. Hidden columns are tracked by their key (<see cref="FlareColumn{TItem}.Id"/> /
    /// SortKey / Title).</summary>
    [Parameter] public bool ShowColumnPicker { get; set; }
    /// <summary>Overrides the label on the column-picker button (defaults to a localized "Columns").</summary>
    [Parameter] public string? ColumnsButtonLabel { get; set; }
    /// <summary>Controlled set of hidden column keys. When set, the grid reflects this value and reports
    /// changes via <see cref="HiddenColumnsChanged"/>; otherwise the grid manages hiding internally.</summary>
    [Parameter] public IReadOnlyCollection<string>? HiddenColumns { get; set; }
    /// <summary>Raised with the full set of hidden column keys whenever a column is shown or hidden.</summary>
    [Parameter] public EventCallback<IReadOnlyCollection<string>> HiddenColumnsChanged { get; set; }

    /// <summary>When set, the grid persists sort/filter/column state to localStorage under this key.
    /// State is restored on component init and saved on every user change.</summary>
    [Parameter] public string? PersistStateKey { get; set; }

    // -- Server-side change events (Phase A) ----------------------------------
    // Fire on every user change so a host can rebuild an API query or persist layout. When an
    // ItemsProvider is set the grid also re-requests automatically; these are an additional hook.

    /// <summary>Raised when the sort changes, with all active sorts in apply order.</summary>
    [Parameter] public EventCallback<IReadOnlyList<DataGridSort>> OnSortChanged { get; set; }
    /// <summary>Raised when a column filter changes, with the full structured filter model.</summary>
    [Parameter] public EventCallback<IReadOnlyList<DataGridFilter>> OnFilterChanged { get; set; }
    /// <summary>Raised when the group-by keys change (outermost first).</summary>
    [Parameter] public EventCallback<IReadOnlyList<string>> OnGroupChanged { get; set; }
    /// <summary>Raised when the current page index changes.</summary>
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    /// <summary>Raised after any sort/filter/group/page/page-size change, with the complete grid state.</summary>
    [Parameter] public EventCallback<DataGridState> OnStateChanged { get; set; }

}
