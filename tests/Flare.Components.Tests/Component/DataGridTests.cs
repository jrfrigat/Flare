using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareDataGrid filter/detail  (6 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlareDataGridAdvancedTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people =
    [
        new("Alice", "Engineering"),
        new("Bob", "Marketing"),
        new("Carol", "Engineering"),
    ];

    private static RenderFragment DataGridWith(
        bool filterable = false,
        bool frozen = false,
        RenderFragment? toolbarContent = null,
        RenderFragment<Person>? rowDetailTemplate = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        if (toolbarContent != null)
            b.AddAttribute(3, "ToolbarContent", toolbarContent);
        if (rowDetailTemplate != null)
            b.AddAttribute(4, "RowDetailTemplate", rowDetailTemplate);
        b.AddAttribute(5, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.Name));
            if (filterable)
            {
                inner.AddAttribute(13, "Filterable", true);
                // Apply the filter synchronously in tests (no debounce timer).
                inner.AddAttribute(15, "FilterDebounceMs", 0);
            }
            if (frozen) inner.AddAttribute(14, "Frozen", true);
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Person>>(20);
            inner.AddAttribute(21, "Title", "Dept");
            inner.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Department));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void FilterableColumns_RenderFilterRow()
    {
        // In the default Simple filter mode the filter row appears automatically
        // when at least one column is Filterable.
        var cut = Render(DataGridWith(filterable: true));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__filter-row"));
    }

    [Fact]
    public void FilterRow_FiltersData()
    {
        var cut = Render(DataGridWith(filterable: true));

        // The text filter reuses FlareField (Immediate) in the filter row.
        cut.Find(".flare-datagrid__filter-row .flare-input__control").Input("Alice");

        var rows = cut.FindAll(".flare-datagrid__row");
        Assert.Single(rows);
        Assert.Contains("Alice", rows[0].TextContent);
    }

    [Fact]
    public void ToolbarContent_Renders()
    {
        var toolbar = (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"custom-toolbar\">Toolbar</span>"));
        var cut = Render(DataGridWith(toolbarContent: toolbar));

        Assert.NotNull(cut.Find(".custom-toolbar"));
    }

    [Fact]
    public void RowDetailTemplate_RendersToggleButton()
    {
        var detail = (RenderFragment<Person>)(p => b =>
            b.AddMarkupContent(0, $"<span class=\"detail-text\">{p.Name} details</span>"));

        var cut = Render(DataGridWith(rowDetailTemplate: detail));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__detail-btn"));
    }

    [Fact]
    public void FrozenColumn_AppliesFrozenClassToHeader()
    {
        var cut = Render(DataGridWith(frozen: true));

        var frozenHeaders = cut.FindAll(".flare-datagrid__th--frozen");
        Assert.NotEmpty(frozenHeaders);
    }

    [Fact]
    public void FilterableColumn_HasInputInFilterRow()
    {
        var cut = Render(DataGridWith(filterable: true));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__filter-row .flare-input__control"));
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid column picker / extended  (7 tests from Wave9)
// ------------------------------------------------------------------------------

public class C_FlareDataGridColumnPickerTests : FlareTestContext
{
    [Fact]
    public void Default_RendersDatagridElement()
    {
        var cut = Render<FlareDataGrid<string>>();

        Assert.NotEmpty(cut.FindAll(".flare-datagrid"));
    }

    [Fact]
    public void Items_RendersDataRows()
    {
        var items = new[] { "Alpha", "Beta", "Gamma" };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, items));

        Assert.Equal(3, cut.FindAll("tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void PageSize_5_LimitsDisplayedRows()
    {
        var items = Enumerable.Range(1, 20).Select(i => $"Item {i}").ToArray();
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.PageSize, 5));

        Assert.Equal(5, cut.FindAll("tr.flare-datagrid__row").Count);
    }

    // Builds a <Grouping> fragment of <DataGridGroup> children (the post-refactor grouping API).
    private static RenderFragment GroupingFor<T>(params (string Key, Func<T, object?> Selector)[] levels) => b =>
    {
        var seq = 0;
        foreach (var (key, selector) in levels)
        {
            b.OpenComponent<DataGridGroup<T>>(seq++);
            b.AddAttribute(seq++, "Key", key);
            b.AddAttribute(seq++, "Selector", selector);
            b.CloseComponent();
        }
    };

    [Fact]
    public void Groups_SingleLevel_RendersGroupHeaderRows()
    {
        var items = new[] { "Apple", "Avocado", "Banana" };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Grouping, GroupingFor<string>(("Letter", s => s.StartsWith("A") ? "A" : "B"))));

        Assert.NotEmpty(cut.FindAll("tr.flare-datagrid__group-header"));
    }

    private record GroupedPerson(string Role, string City, int Score);

    private static readonly GroupedPerson[] _grouped =
    [
        new("Eng", "Berlin", 90),
        new("Eng", "Berlin", 80),
        new("Eng", "Paris", 70),
        new("QA", "Paris", 60),
    ];

    [Fact]
    public void Groups_NestedLevels_RenderHeadersForEachLevel()
    {
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.Grouping, GroupingFor<GroupedPerson>(
                ("Role", g => g.Role),
                ("City", g => g.City))));

        // 2 roles (Eng, QA) + 3 cities (Berlin, Paris under Eng; Paris under QA) = 5 group headers.
        Assert.Equal(5, cut.FindAll("tr.flare-datagrid__group-header").Count);
        // All data rows still render under their leaf groups.
        Assert.Equal(4, cut.FindAll("tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void Groups_WithAggregates_RenderAggregateChips()
    {
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.Grouping, GroupingFor<GroupedPerson>(("Role", g => g.Role)))
            .Add(x => x.Aggregates, new[]
            {
                new AggregateDefinition<GroupedPerson> { ColumnTitle = "Max", Type = AggregateType.Max, ValueSelector = g => g.Score, Format = "N0" },
            }));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__group-aggregate"));
    }

    [Fact]
    public void Groups_ClickHeader_CollapsesChildRows()
    {
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.Grouping, GroupingFor<GroupedPerson>(("Role", g => g.Role))));

        Assert.Equal(4, cut.FindAll("tr.flare-datagrid__row").Count);

        // Collapse the first group (Eng, 3 rows) -> only the QA group's 1 row remains.
        cut.FindAll("button.flare-datagrid__group-toggle")[0].Click();

        Assert.Single(cut.FindAll("tr.flare-datagrid__row"));
    }

    // The Select filter applies on selection; the text/number filter row is debounced, so the
    // columns set FilterDebounceMs=0 to keep the tests synchronous.
    private RenderFragment FilterableGrid(bool showFilterBuilder = false) => b =>
    {
        b.OpenComponent<FlareDataGrid<GroupedPerson>>(0);
        b.AddAttribute(1, "Items", _grouped.AsEnumerable());
        if (showFilterBuilder)
        {
            // The builder resolves the grid from the cascade (ToolbarContent) - no explicit wiring.
            b.AddAttribute(2, "ToolbarContent", (RenderFragment)(tb =>
            {
                tb.OpenComponent<DataGridFilterBuilder<GroupedPerson>>(0);
                tb.CloseComponent();
            }));
        }
        b.AddAttribute(3, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<GroupedPerson>>(10);
            inner.AddAttribute(11, "Title", "Role");
            inner.AddAttribute(12, "Field", (Func<GroupedPerson, object?>)(p => p.Role));
            inner.AddAttribute(13, "Filterable", true);
            inner.AddAttribute(14, "FilterType", ColumnFilterType.Select);
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<GroupedPerson>>(20);
            inner.AddAttribute(21, "Title", "Score");
            inner.AddAttribute(22, "Field", (Func<GroupedPerson, object?>)(p => p.Score));
            inner.AddAttribute(23, "Filterable", true);
            inner.AddAttribute(24, "FilterType", ColumnFilterType.Number);
            inner.AddAttribute(25, "FilterDebounceMs", 0);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    // FlareSelect is a custom popover (not a native <select>): open the control, then click the
    // option by its visible text. Operator labels use the same FlareStrings the component renders.
    private static void PickSelect(IRenderedComponent<IComponent> cut, string scope, string optionText, bool last = false)
    {
        var controls = cut.FindAll($"{scope} .flare-select__control");
        (last ? controls[controls.Count - 1] : controls[0]).Click();
        cut.FindAll($"{scope} .flare-select__option")
            .First(o => o.TextContent.Trim() == optionText)
            .Click();
    }

    [Fact]
    public void SelectFilter_RendersMultiSelectWithDistinctValues()
    {
        var cut = Render(FilterableGrid());

        // The Select filter reuses FlareMultiSelect.
        Assert.NotEmpty(cut.FindAll(".flare-datagrid__filter-th .flare-multiselect"));
        // Open the dropdown: 2 distinct roles (Eng, QA).
        cut.Find(".flare-multiselect__control").Click();
        Assert.Equal(2, cut.FindAll(".flare-multiselect__option").Count);
    }

    [Fact]
    public void SelectFilter_FiltersRowsOnSelection()
    {
        var cut = Render(FilterableGrid());

        Assert.Equal(4, cut.FindAll("tr.flare-datagrid__row").Count);

        cut.Find(".flare-multiselect__control").Click();
        cut.FindAll(".flare-multiselect__option").First(o => o.TextContent.Contains("QA")).Click();

        Assert.Single(cut.FindAll("tr.flare-datagrid__row"));
    }

    [Fact]
    public void FilterBuilder_ToggleButton_OpensPanelWithOneCondition()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));

        Assert.Empty(cut.FindAll(".flare-datagrid__filter-builder-panel"));

        cut.Find(".flare-datagrid__filter-builder > button").Click();

        Assert.Single(cut.FindAll(".flare-datagrid__filter-builder-panel"));
        Assert.Single(cut.FindAll(".flare-datagrid__filter-builder-row"));
    }

    [Fact]
    public void FilterBuilder_ApplyGreaterThan_FiltersRows()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find(".flare-datagrid__filter-builder > button").Click();

        // Configure: Score greater-than 75 -> of {90,80,70,60} only 90 and 80 qualify.
        // Column + operator reuse FlareSelect (custom popover); value reuses FlareField.
        PickSelect(cut, ".flare-datagrid__filter-builder-field", "Score");
        PickSelect(cut, ".flare-datagrid__filter-builder-op", FlareStrings.DataGrid_OpGreaterThan);
        cut.Find(".flare-datagrid__filter-builder-value .flare-input__control").Input("75");
        // Actions order: Clear, Apply -> Apply is the last button.
        cut.FindAll(".flare-datagrid__filter-builder-actions button").Last().Click();

        Assert.Equal(2, cut.FindAll("tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void FilterBuilder_DefaultField_FiltersWithoutReselecting()
    {
        // The first condition defaults its field to the first column (Role); the user only sets
        // the value and applies, without touching the field dropdown.
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find(".flare-datagrid__filter-builder > button").Click();

        cut.Find(".flare-datagrid__filter-builder-value .flare-input__control").Input("QA");
        cut.FindAll(".flare-datagrid__filter-builder-actions button").Last().Click(); // Apply

        Assert.Single(cut.FindAll("tr.flare-datagrid__row"));
    }

    [Fact]
    public void FilterBuilder_Connector_TogglesAndOr()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find(".flare-datagrid__filter-builder > button").Click();

        var connector = cut.Find(".flare-datagrid__filter-builder-connector");
        Assert.Equal(FlareStrings.DataGrid_And, connector.TextContent.Trim());

        connector.Click();
        Assert.Equal(FlareStrings.DataGrid_Or, cut.Find(".flare-datagrid__filter-builder-connector").TextContent.Trim());
    }

    [Fact]
    public void FilterBuilder_AddGroup_AddsNestedGroup()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find(".flare-datagrid__filter-builder > button").Click();

        Assert.Single(cut.FindAll(".flare-datagrid__filter-builder-group"));

        // Group head buttons: [0] connector, [1] add condition, [2] add group.
        cut.FindAll(".flare-datagrid__filter-builder-group-head button")[2].Click();

        Assert.Equal(2, cut.FindAll(".flare-datagrid__filter-builder-group").Count);
    }

    [Fact]
    public void FilterBuilder_NestedOrGroup_FiltersRows()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find(".flare-datagrid__filter-builder > button").Click();

        // Root condition: Score >= 90 (matches the single 90 row).
        PickSelect(cut, ".flare-datagrid__filter-builder-field", "Score");
        PickSelect(cut, ".flare-datagrid__filter-builder-op", FlareStrings.DataGrid_OpGreaterOrEqual);
        cut.Find(".flare-datagrid__filter-builder-value .flare-input__control").Input("90");

        // Add a nested group, set it to OR, add a condition Score <= 60 (matches the single 60 row).
        cut.FindAll(".flare-datagrid__filter-builder-group-head button")[2].Click(); // add group
        // The nested group is the 2nd group; toggle its connector to Or.
        cut.FindAll(".flare-datagrid__filter-builder-connector")[1].Click();
        // Configure the nested condition (last field/op/value belong to the new group's seeded row).
        PickSelect(cut, ".flare-datagrid__filter-builder-field", "Score", last: true);
        PickSelect(cut, ".flare-datagrid__filter-builder-op", FlareStrings.DataGrid_OpLessOrEqual, last: true);
        cut.FindAll(".flare-datagrid__filter-builder-value .flare-input__control").Last().Input("60");

        cut.FindAll(".flare-datagrid__filter-builder-actions button").Last().Click(); // Apply

        // Root is AND of [Score>=90] and [OR group: Score<=60]. No row is both >=90 and <=60.
        Assert.Empty(cut.FindAll("tr.flare-datagrid__row"));
    }

    private static RenderFragment GroupedCols => inner =>
    {
        inner.OpenComponent<FlareColumn<GroupedPerson>>(0);
        inner.AddAttribute(1, "Title", "Role");
        inner.AddAttribute(2, "Field", (Func<GroupedPerson, object?>)(p => p.Role));
        inner.CloseComponent();
    };

    [Fact]
    public void Loading_SkeletonMode_ShowsSkeletonRows()
    {
        // Provider task left pending so the grid stays in its loading state.
        var tcs = new TaskCompletionSource<DataGridResult<GroupedPerson>>();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, _ => tcs.Task)
            .Add(x => x.LoadingIndicator, DataGridLoadingIndicator.Skeleton)
            .Add(x => x.Columns, GroupedCols));

        Assert.NotEmpty(cut.FindAll("tr.flare-datagrid__placeholder-row"));
        // Skeleton mode shows no spinner/text overlay.
        Assert.Empty(cut.FindAll(".flare-datagrid__loading"));
    }

    [Fact]
    public void Loading_SpinnerMode_ShowsRingOverlay()
    {
        var tcs = new TaskCompletionSource<DataGridResult<GroupedPerson>>();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, _ => tcs.Task)
            .Add(x => x.LoadingIndicator, DataGridLoadingIndicator.Spinner)
            .Add(x => x.Columns, GroupedCols));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__loading"));
        Assert.NotEmpty(cut.FindAll(".flare-progress--circular"));
    }

    private static RenderFragment EditableCols => inner =>
    {
        inner.OpenComponent<FlareColumn<GroupedPerson>>(0);
        inner.AddAttribute(1, "Title", "Role");
        inner.AddAttribute(2, "Field", (Func<GroupedPerson, object?>)(p => p.Role));
        inner.AddAttribute(3, "Editable", true);
        inner.CloseComponent();
        inner.OpenComponent<FlareColumn<GroupedPerson>>(10);
        inner.AddAttribute(11, "Title", "City");
        inner.AddAttribute(12, "Field", (Func<GroupedPerson, object?>)(p => p.City));
        inner.CloseComponent();
    };

    [Fact]
    public void Appearance_StripedHoverableDense_ApplyModifierClasses()
    {
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.Striped, true)
            .Add(x => x.Hoverable, true)
            .Add(x => x.Dense, true)
            .Add(x => x.Columns, GroupedCols));

        var root = cut.Find(".flare-datagrid");
        Assert.Contains("flare-datagrid--striped", root.ClassName);
        Assert.Contains("flare-datagrid--hoverable", root.ClassName);
        Assert.Contains("flare-datagrid--dense", root.ClassName);
    }

    [Fact]
    public void ResizableColumn_RendersResizeHandle()
    {
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.Columns, inner =>
            {
                inner.OpenComponent<FlareColumn<GroupedPerson>>(0);
                inner.AddAttribute(1, "Title", "Role");
                inner.AddAttribute(2, "Field", (Func<GroupedPerson, object?>)(p => p.Role));
                inner.AddAttribute(3, "Resizable", true);
                inner.CloseComponent();
            }));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__resize-handle"));
    }

    [Fact]
    public void EditableColumn_AutoEnablesEditing_AndBuffersTypedValue()
    {
        var people = new[] { new GroupedPerson("Eng", "Berlin", 90) };
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, people.AsEnumerable())
            .Add(x => x.Columns, EditableCols));

        // An Editable column alone surfaces the edit (pencil) action - no InlineEdit needed.
        cut.Find(".flare-datagrid__td--edit-actions button").Click();

        // The editable cell now renders a FlareField; typing updates the edit buffer.
        var input = cut.Find(".flare-input__control");
        input.Input("Manager");
        Assert.Equal("Manager", cut.Instance.GetEditValues()["Role"]);
    }

    [Fact]
    public async Task InfiniteScroll_LoadsAndAppendsPagesOnTrigger()
    {
        var data = Enumerable.Range(1, 50)
            .Select(i => new GroupedPerson($"R{i}", "C", i))
            .ToList();
        Task<DataGridResult<GroupedPerson>> Provider(DataGridRequest r) =>
            Task.FromResult(new DataGridResult<GroupedPerson>(
                data.Skip(r.Page * r.PageSize).Take(r.PageSize), data.Count));

        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, Provider)
            .Add(x => x.InfiniteScroll, true)
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, GroupedCols));

        // Initial load brings the first page.
        Assert.Equal(10, cut.FindAll("tr.flare-datagrid__row").Count);

        // Simulate the bottom sentinel becoming visible -> next page appends.
        await cut.InvokeAsync(() => cut.Instance.TriggerLoad());
        Assert.Equal(20, cut.FindAll("tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void Loading_ProgressLineMode_ShowsThinLineWithoutOverlay()
    {
        var tcs = new TaskCompletionSource<DataGridResult<GroupedPerson>>();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, _ => tcs.Task)
            .Add(x => x.LoadingIndicator, DataGridLoadingIndicator.ProgressLine)
            .Add(x => x.Columns, GroupedCols));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__progress-line"));
        Assert.NotEmpty(cut.FindAll(".flare-progress--linear"));
        // ProgressLine keeps the table visible: no spinner/text overlay, no dim class.
        Assert.Empty(cut.FindAll(".flare-datagrid__loading"));
        Assert.Empty(cut.FindAll(".flare-datagrid__table--loading"));
    }

    [Fact]
    public void RowKey_UsedAsStableRowIdentity()
    {
        // Two distinct items that compare equal by reference would collide; RowKey disambiguates.
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.RowKey, g => g.Score)
            .Add(x => x.Columns, GroupedCols));

        Assert.Equal(4, cut.FindAll("tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void Virtual_WithoutItemSize_RendersWithoutThrowing()
    {
        // Virtualize throws on a non-positive ItemSize; the grid must supply a default row height
        // when VirtualItemSize is left unset.
        var data = Enumerable.Range(1, 100).Select(i => new GroupedPerson($"R{i}", "C", i)).ToList();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, data.AsEnumerable())
            .Add(x => x.Virtual, true)
            .Add(x => x.Height, "300px")
            .Add(x => x.Columns, GroupedCols));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid"));
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid header rendering / column declaration order
// ------------------------------------------------------------------------------

public class C_FlareDataGridHeaderTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people = [new("Alice", "Eng"), new("Bob", "Mkt")];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(5, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.Name));
            inner.CloseComponent();
            inner.OpenComponent<FlareColumn<Person>>(20);
            inner.AddAttribute(21, "Title", "Dept");
            inner.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Department));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    private static RenderFragment ReorderGrid(
        bool reorderableColumns = false,
        IReadOnlyList<string>? columnOrder = null,
        Action<IReadOnlyList<string>>? onColumnOrderChanged = null,
        bool rowReorderable = false,
        Action<DataGridRowReorder<Person>>? onRowReordered = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "ReorderableColumns", reorderableColumns);
        b.AddAttribute(3, "RowReorderable", rowReorderable);
        if (columnOrder is not null) b.AddAttribute(4, "ColumnOrder", columnOrder);
        if (onColumnOrderChanged is not null)
            b.AddAttribute(5, "OnColumnOrderChanged",
                EventCallback.Factory.Create(onColumnOrderChanged.Target!, onColumnOrderChanged));
        if (onRowReordered is not null)
            b.AddAttribute(6, "OnRowReordered",
                EventCallback.Factory.Create(onRowReordered.Target!, onRowReordered));
        b.AddAttribute(7, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.Name));
            inner.CloseComponent();
            inner.OpenComponent<FlareColumn<Person>>(20);
            inner.AddAttribute(21, "Title", "Dept");
            inner.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Department));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Default_HeaderOrder_MatchesDeclaration()
    {
        var cut = Render(Grid());
        var titles = cut.FindAll("th[role=columnheader]").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(["Name", "Dept"], titles);
    }

    [Fact]
    public void RowAndColumnFuncs_ApplyInDefaultRenderPath()
    {
        // Regression: the default (non-virtualized, non-grouped) path must apply both the row-level
        // RowClassFunc/RowStyleFunc and the column-level ClassFunc/StyleFunc.
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(x => x.Items, _people.AsEnumerable())
            .Add(x => x.RowClassFunc, item => item.Name == "Alice" ? "row-hi" : "")
            .Add(x => x.RowStyleFunc, item => item.Name == "Alice" ? "background:red" : "")
            .Add(x => x.Columns, inner =>
            {
                inner.OpenComponent<FlareColumn<Person>>(0);
                inner.AddAttribute(1, "Title", "Name");
                inner.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
                inner.AddAttribute(3, "ClassFunc", (Func<Person, string>)(p => p.Name == "Alice" ? "cell-hi" : ""));
                inner.AddAttribute(4, "StyleFunc", (Func<Person, string>)(p => p.Name == "Alice" ? "font-weight:bold" : ""));
                inner.CloseComponent();
            }));

        var aliceRow = cut.FindAll("tr.flare-datagrid__row").First(r => r.TextContent.Contains("Alice"));
        Assert.Contains("row-hi", aliceRow.ClassName);
        Assert.Contains("background:red", aliceRow.GetAttribute("style") ?? "");

        var aliceCell = aliceRow.QuerySelector("td.flare-datagrid__td")!;
        Assert.Contains("cell-hi", aliceCell.ClassName);
        Assert.Contains("font-weight:bold", aliceCell.GetAttribute("style") ?? "");
    }

    [Fact]
    public void ReorderableColumns_AddsDraggableMarkup()
    {
        var cut = Render(ReorderGrid(reorderableColumns: true));
        var th = cut.FindAll("th[role=columnheader]")[0];
        Assert.Equal("true", th.GetAttribute("draggable"));
        Assert.Contains("flare-datagrid__th--draggable", th.ClassName);
    }

    [Fact]
    public void Columns_NotReorderable_HaveNoDraggableAttribute()
    {
        var cut = Render(ReorderGrid(reorderableColumns: false));
        var th = cut.FindAll("th[role=columnheader]")[0];
        Assert.Null(th.GetAttribute("draggable"));
    }

    [Fact]
    public void ColumnOrder_Param_ReordersHeaders()
    {
        var cut = Render(ReorderGrid(columnOrder: ["Dept", "Name"]));
        var titles = cut.FindAll("th[role=columnheader]").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(["Dept", "Name"], titles);
    }

    [Fact]
    public void DragDropColumn_ReordersAndRaisesCallback()
    {
        IReadOnlyList<string>? reported = null;
        var cut = Render(ReorderGrid(reorderableColumns: true, onColumnOrderChanged: o => reported = o));

        // Drag "Dept" (index 1) onto "Name" (index 0) -> Dept first.
        cut.FindAll("th[role=columnheader]")[1].TriggerEvent("ondragstart", new Microsoft.AspNetCore.Components.Web.DragEventArgs());
        cut.FindAll("th[role=columnheader]")[0].TriggerEvent("ondrop", new Microsoft.AspNetCore.Components.Web.DragEventArgs());

        var titles = cut.FindAll("th[role=columnheader]").Select(t => t.TextContent.Trim()).ToList();
        Assert.Equal(["Dept", "Name"], titles);
        Assert.NotNull(reported);
        Assert.Equal(["Dept", "Name"], reported!);
    }

    [Fact]
    public void RowReorderable_AddsDraggableMarkup()
    {
        var cut = Render(ReorderGrid(rowReorderable: true));
        var row = cut.FindAll("tr.flare-datagrid__row")[0];
        Assert.Equal("true", row.GetAttribute("draggable"));
        Assert.Contains("flare-datagrid__row--draggable", row.ClassName);
    }

    [Fact]
    public void DragDropRow_RaisesOnRowReorderedWithIndices()
    {
        DataGridRowReorder<Person>? reported = null;
        var cut = Render(ReorderGrid(rowReorderable: true, onRowReordered: e => reported = e));

        // Drag row 1 (Bob) onto row 0 (Alice).
        cut.FindAll("tr.flare-datagrid__row")[1].TriggerEvent("ondragstart", new Microsoft.AspNetCore.Components.Web.DragEventArgs());
        cut.FindAll("tr.flare-datagrid__row")[0].TriggerEvent("ondrop", new Microsoft.AspNetCore.Components.Web.DragEventArgs());

        Assert.NotNull(reported);
        Assert.Equal(1, reported!.OldIndex);
        Assert.Equal(0, reported.NewIndex);
        Assert.Equal("Bob", reported.Item.Name);
        Assert.Equal("Alice", reported.Target.Name);
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid column bands (grouped headers via <FlareColumnBand>)
// ------------------------------------------------------------------------------

public class C_FlareDataGridBandTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static void AddColumn(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string title, Func<Person, object?> field)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        b.CloseComponent();
    }

    // Header tree: Name | [Details: Role, [Location: City]] | Score
    private static RenderFragment BandedGrid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            AddColumn(cols, 0, "Name", p => p.Name);
            cols.OpenComponent<FlareColumnBand>(10);
            cols.AddAttribute(11, "Title", "Details");
            cols.AddAttribute(12, "ChildContent", (RenderFragment)(d =>
            {
                AddColumn(d, 0, "Role", p => p.Role);
                d.OpenComponent<FlareColumnBand>(10);
                d.AddAttribute(11, "Title", "Location");
                d.AddAttribute(12, "ChildContent", (RenderFragment)(l => AddColumn(l, 0, "City", p => p.City)));
                d.CloseComponent();
            }));
            cols.CloseComponent();
            AddColumn(cols, 20, "Score", p => p.Score);
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Bands_RenderTitleCellsWithColspan()
    {
        var cut = Render(BandedGrid());

        var bandCells = cut.FindAll("th.flare-datagrid__th--band");
        Assert.Contains(bandCells, th => th.TextContent.Trim() == "Details");
        Assert.Contains(bandCells, th => th.TextContent.Trim() == "Location");

        // Details spans the two leaves under it (Role + City).
        var details = bandCells.First(th => th.TextContent.Trim() == "Details");
        Assert.Equal("2", details.GetAttribute("colspan"));
    }

    [Fact]
    public void Bands_ProduceThreeHeaderRows()
    {
        var cut = Render(BandedGrid());
        // Nesting depth 2 -> 2 band rows + 1 leaf row = 3 header rows.
        Assert.Equal(3, cut.FindAll("thead tr").Count);
    }

    [Fact]
    public void FreeColumn_SpansFullHeaderHeight()
    {
        var cut = Render(BandedGrid());
        var nameTh = cut.FindAll("th[role=columnheader]").First(t => t.TextContent.Contains("Name"));
        Assert.Equal("3", nameTh.GetAttribute("rowspan"));
    }

    [Fact]
    public void BandedColumns_StillRenderAlignedDataCells()
    {
        var cut = Render(BandedGrid());
        // Four leaf columns -> four data cells per row, in declaration order.
        var firstRowCells = cut.FindAll("tr.flare-datagrid__row")[0].QuerySelectorAll("td.flare-datagrid__td");
        Assert.Equal(4, firstRowCells.Length);
        Assert.Equal("Alice", firstRowCells[0].TextContent.Trim());
        Assert.Equal("Eng", firstRowCells[1].TextContent.Trim());
        Assert.Equal("Berlin", firstRowCells[2].TextContent.Trim());
        Assert.Equal("90", firstRowCells[3].TextContent.Trim());
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid composite columns (stacked fields in one cell)
// ------------------------------------------------------------------------------

public class C_FlareDataGridCompositeTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static void Field(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string title, Func<Person, object?> field, int colSpan = 1)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        if (colSpan != 1) b.AddAttribute(seq + 3, "ColSpan", colSpan);
        b.CloseComponent();
    }

    private static void Row(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, RenderFragment content)
    {
        b.OpenComponent<FlareColumnRow>(seq);
        b.AddAttribute(seq + 1, "ChildContent", content);
        b.CloseComponent();
    }

    // Columns: [Employee composite: (Name, City) / (Role span2)] | Score
    private static RenderFragment CompositeGrid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            cols.OpenComponent<FlareColumn<Person>>(0);
            cols.AddAttribute(1, "Title", "Employee");
            cols.AddAttribute(3, "CompositeMode", CompositeMode.Card);
            cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
            {
                Row(comp, 0, r =>
                {
                    Field(r, 0, "Name", p => p.Name);
                    Field(r, 10, "City", p => p.City);
                });
                Row(comp, 10, r => Field(r, 0, "Role", p => p.Role, colSpan: 2));
            }));
            cols.CloseComponent();
            cols.OpenComponent<FlareColumn<Person>>(20);
            cols.AddAttribute(21, "Title", "Score");
            cols.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Score));
            cols.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Composite_RendersOneCellGridPerRow()
    {
        var cut = Render(CompositeGrid());
        // Two data rows -> two composite containers.
        Assert.Equal(2, cut.FindAll(".flare-datagrid__composite").Count);
        // The composite host plus the normal Score column = exactly two td cells per data row.
        var cells = cut.FindAll("tr.flare-datagrid__row")[0].QuerySelectorAll("td.flare-datagrid__td");
        Assert.Equal(2, cells.Length);
    }

    [Fact]
    public void Composite_RendersAllFieldValuesAndLabels()
    {
        var cut = Render(CompositeGrid());
        var firstComposite = cut.FindAll(".flare-datagrid__composite")[0];

        var values = firstComposite.QuerySelectorAll(".flare-datagrid__composite-value").Select(v => v.TextContent.Trim()).ToList();
        Assert.Contains("Alice", values);
        Assert.Contains("Berlin", values);
        Assert.Contains("Eng", values);

        var labels = firstComposite.QuerySelectorAll(".flare-datagrid__composite-label").Select(l => l.TextContent.Trim()).ToList();
        Assert.Contains("Name", labels);
        Assert.Contains("Role", labels);
    }

    [Fact]
    public void Composite_FieldColSpan_AppliesGridColumnSpan()
    {
        var cut = Render(CompositeGrid());
        var roleField = cut.FindAll(".flare-datagrid__composite-field")
            .First(f => f.TextContent.Contains("Role"));
        Assert.Contains("span 2", roleField.GetAttribute("style") ?? "");
    }

    [Fact]
    public void CompositeFields_DoNotBecomeGridColumns()
    {
        var cut = Render(CompositeGrid());
        // Only the two top-level columns (Employee, Score) are real column headers.
        Assert.Equal(2, cut.FindAll("th[role=columnheader]").Count);
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid banded composite (records span several rows, DevExpress-style)
// ------------------------------------------------------------------------------

public class C_FlareDataGridBandedCompositeTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static void Field(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, string title, Func<Person, object?> field, int colSpan = 1, bool sortable = false)
    {
        b.OpenComponent<FlareColumn<Person>>(seq);
        b.AddAttribute(seq + 1, "Title", title);
        b.AddAttribute(seq + 2, "Field", field);
        if (colSpan != 1) b.AddAttribute(seq + 3, "ColSpan", colSpan);
        if (sortable) b.AddAttribute(seq + 4, "Sortable", true);
        b.CloseComponent();
    }

    private static void Row(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b, int seq, RenderFragment content)
    {
        b.OpenComponent<FlareColumnRow>(seq);
        b.AddAttribute(seq + 1, "ChildContent", content);
        b.CloseComponent();
    }

    // Columns: [Employee banded: (Name, City sortable) / (Role span2)] | Score (plain)
    private static RenderFragment BandedGrid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            cols.OpenComponent<FlareColumn<Person>>(0);
            cols.AddAttribute(1, "Title", "Employee");
            cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
            {
                Row(comp, 0, r => { Field(r, 0, "Name", p => p.Name); Field(r, 10, "City", p => p.City, sortable: true); });
                Row(comp, 10, r => Field(r, 0, "Role", p => p.Role, colSpan: 2));
            }));
            cols.CloseComponent();
            cols.OpenComponent<FlareColumn<Person>>(20);
            cols.AddAttribute(21, "Title", "Score");
            cols.AddAttribute(22, "Field", (Func<Person, object?>)(p => p.Score));
            cols.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Banded_HeaderHasOneRowPerRecordRow()
    {
        var cut = Render(BandedGrid());
        // Composite has 2 rows -> 2 header rows.
        Assert.Equal(2, cut.FindAll("thead tr").Count);
    }

    [Fact]
    public void Banded_PlainColumnSpansAllRecordRows()
    {
        var cut = Render(BandedGrid());
        var scoreTh = cut.FindAll("th[role=columnheader]").First(t => t.TextContent.Contains("Score"));
        Assert.Equal("2", scoreTh.GetAttribute("rowspan"));
    }

    [Fact]
    public void Banded_EachRecordSpansTwoBodyRows()
    {
        var cut = Render(BandedGrid());
        // 2 records x 2 rows = 4 record rows.
        Assert.Equal(4, cut.FindAll("tr.flare-datagrid__row--record").Count);
    }

    [Fact]
    public void Banded_RendersFieldValuesAndColSpan()
    {
        var cut = Render(BandedGrid());
        var firstRow = cut.FindAll("tr.flare-datagrid__row--record")[0];
        var composites = firstRow.QuerySelectorAll("td.flare-datagrid__td--composite");
        var texts = composites.Select(c => c.TextContent.Trim()).ToList();
        Assert.Contains("Alice", texts);
        Assert.Contains("Berlin", texts);

        // The Role cell on the second record-row spans 2 columns.
        var secondRow = cut.FindAll("tr.flare-datagrid__row--record")[1];
        var roleCell = secondRow.QuerySelectorAll("td.flare-datagrid__td--composite")[0];
        Assert.Equal("Eng", roleCell.TextContent.Trim());
        Assert.Equal("2", roleCell.GetAttribute("colspan"));
    }
}

public class C_FlareDataGridBandedSortTests : FlareTestContext
{
    private record Person(string Name, string Role, string City, int Score);

    private static readonly Person[] _people =
    [
        new("Alice", "Eng", "Berlin", 90),
        new("Bob", "QA", "Paris", 60),
    ];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(cols =>
        {
            cols.OpenComponent<FlareColumn<Person>>(0);
            cols.AddAttribute(1, "Title", "Employee");
            cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
            {
                comp.OpenComponent<FlareColumnRow>(0);
                comp.AddAttribute(1, "ChildContent", (RenderFragment)(r =>
                {
                    r.OpenComponent<FlareColumn<Person>>(0);
                    r.AddAttribute(1, "Title", "Name");
                    r.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
                    r.CloseComponent();
                    r.OpenComponent<FlareColumn<Person>>(10);
                    r.AddAttribute(11, "Title", "City");
                    r.AddAttribute(12, "Field", (Func<Person, object?>)(p => p.City));
                    r.AddAttribute(13, "Sortable", true);
                    r.AddAttribute(14, "Filterable", true);
                    r.AddAttribute(15, "FilterDebounceMs", 0); // instant for the test
                    r.CloseComponent();
                }));
                comp.CloseComponent();
            }));
            cols.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void SortableCompositeField_SortsRecordsAndShowsIndicator()
    {
        var cut = Render(Grid());
        var cityTh = cut.FindAll("th.flare-datagrid__th--composite").First(t => t.TextContent.Contains("City"));
        Assert.Contains("flare-datagrid__th--sortable", cityTh.ClassName);

        cityTh.Click(); // ascending by City: Berlin (Alice), Paris (Bob)
        var firstAsc = cut.FindAll("tr.flare-datagrid__row--record")[0]
            .QuerySelectorAll("td.flare-datagrid__td--composite")[0].TextContent.Trim();
        Assert.Equal("Alice", firstAsc);

        cut.FindAll("th.flare-datagrid__th--composite").First(t => t.TextContent.Contains("City")).Click(); // descending
        var firstDesc = cut.FindAll("tr.flare-datagrid__row--record")[0]
            .QuerySelectorAll("td.flare-datagrid__td--composite")[0].TextContent.Trim();
        Assert.Equal("Bob", firstDesc);

        Assert.NotEmpty(cut.FindAll("th.flare-datagrid__th--composite .flare-datagrid__sort-icon"));
    }

    [Fact]
    public void FilterableCompositeField_FiltersRecordsByFieldValue()
    {
        var cut = Render(Grid());
        // Both records visible initially.
        Assert.Equal(2, cut.FindAll("tr.flare-datagrid__row--record-first").Count);

        // The City sub-header hosts an inline filter input.
        var cityTh = cut.FindAll("th.flare-datagrid__th--composite").First(t => t.TextContent.Contains("City"));
        var input = cityTh.QuerySelector(".flare-datagrid__composite-filter input");
        Assert.NotNull(input);

        input!.Input("Berlin"); // keep only Alice (Berlin)
        var records = cut.FindAll("tr.flare-datagrid__row--record-first");
        Assert.Single(records);
        Assert.Contains("Alice", records[0].TextContent);
        Assert.DoesNotContain("Bob", cut.Markup);
    }

    [Fact]
    public void Virtual_BandedComposite_RendersWithoutThrowing()
    {
        // Each record spans several <tr>; Virtualize must take a positive per-record ItemSize.
        var cut = Render(b =>
        {
            b.OpenComponent<FlareDataGrid<Person>>(0);
            b.AddAttribute(1, "Items", _people.AsEnumerable());
            b.AddAttribute(2, "Virtual", true);
            b.AddAttribute(3, "Height", "300px");
            b.AddAttribute(4, "Columns", (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Person>>(0);
                cols.AddAttribute(1, "Title", "Employee");
                cols.AddAttribute(2, "Composite", (RenderFragment)(comp =>
                {
                    comp.OpenComponent<FlareColumnRow>(0);
                    comp.AddAttribute(1, "ChildContent", (RenderFragment)(r =>
                    {
                        r.OpenComponent<FlareColumn<Person>>(0);
                        r.AddAttribute(1, "Title", "Name");
                        r.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
                        r.CloseComponent();
                    }));
                    comp.CloseComponent();
                    comp.OpenComponent<FlareColumnRow>(10);
                    comp.AddAttribute(11, "ChildContent", (RenderFragment)(r =>
                    {
                        r.OpenComponent<FlareColumn<Person>>(0);
                        r.AddAttribute(1, "Title", "City");
                        r.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.City));
                        r.CloseComponent();
                    }));
                    comp.CloseComponent();
                }));
                cols.CloseComponent();
            }));
            b.CloseComponent();
        });

        // Uses the fixed-height virtual scroll container (no pagination) and renders the banded header.
        Assert.NotEmpty(cut.FindAll(".flare-datagrid__wrapper--virtual"));
        Assert.NotEmpty(cut.FindAll("th.flare-datagrid__th--composite"));
    }
}

// ------------------------------------------------------------------------------
// FlareColumn stable identity (Key = Id ?? SortKey ?? Title)
// ------------------------------------------------------------------------------

public class C_FlareDataGridColumnIdentityTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people = [new("Alice", "Eng"), new("Bob", "QA")];

    [Fact]
    public void ColumnState_IsKeyedByKey_NotTitle()
    {
        // A column with a SortKey has Key != Title; the grid's column state must use the Key.
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.Columns, (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Person>>(0);
                cols.AddAttribute(1, "Title", "Name");
                cols.AddAttribute(2, "Field", (Func<Person, object?>)(x => x.Name));
                cols.CloseComponent();
                cols.OpenComponent<FlareColumn<Person>>(10);
                cols.AddAttribute(11, "Title", "Department");
                cols.AddAttribute(12, "Field", (Func<Person, object?>)(x => x.Department));
                cols.AddAttribute(13, "SortKey", "dept");
                cols.CloseComponent();
            })));

        var order = cut.Instance.CurrentState.ColumnOrder;
        Assert.Contains("dept", order);            // the key, not the display title
        Assert.DoesNotContain("Department", order);
    }

    [Fact]
    public void DuplicateTitles_DisambiguatedById_SortIndependently()
    {
        // Two columns share a title; Id keeps their sort state distinct.
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.Columns, (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Person>>(0);
                cols.AddAttribute(1, "Title", "Value");
                cols.AddAttribute(2, "Id", "name");
                cols.AddAttribute(3, "Field", (Func<Person, object?>)(x => x.Name));
                cols.AddAttribute(4, "Sortable", true);
                cols.CloseComponent();
                cols.OpenComponent<FlareColumn<Person>>(10);
                cols.AddAttribute(11, "Title", "Value");
                cols.AddAttribute(12, "Id", "dept");
                cols.AddAttribute(13, "Field", (Func<Person, object?>)(x => x.Department));
                cols.AddAttribute(14, "Sortable", true);
                cols.CloseComponent();
            })));

        // Click the first "Value" header; only it gets a sort indicator (state keyed by Id "name").
        cut.FindAll("th.flare-datagrid__th--sortable")[0].Click();
        var sortIcons = cut.FindAll("th .flare-datagrid__sort-icon");
        Assert.Single(sortIcons);
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid interactive column visibility (ShowColumnPicker)
// ------------------------------------------------------------------------------

public class C_FlareDataGridColumnVisibilityTests : FlareTestContext
{
    private record Person(string Name, string Department);

    private static readonly Person[] _people = [new("Alice", "Eng"), new("Bob", "QA")];

    private static RenderFragment Cols() => cols =>
    {
        cols.OpenComponent<FlareColumn<Person>>(0);
        cols.AddAttribute(1, "Title", "Name");
        cols.AddAttribute(2, "Field", (Func<Person, object?>)(x => x.Name));
        cols.CloseComponent();
        cols.OpenComponent<FlareColumn<Person>>(10);
        cols.AddAttribute(11, "Title", "Department");
        cols.AddAttribute(12, "Field", (Func<Person, object?>)(x => x.Department));
        cols.AddAttribute(13, "SortKey", "dept");
        cols.CloseComponent();
    };

    [Fact]
    public void ShowColumnPicker_RendersToolbarButton()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.ShowColumnPicker, true)
            .Add(g => g.Columns, Cols()));

        Assert.NotEmpty(cut.FindAll(".flare-datagrid__column-picker-wrap"));
    }

    [Fact]
    public void ColumnPicker_TogglesColumnVisibilityByKey()
    {
        HashSet<string>? reported = null;
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(g => g.Items, _people.AsEnumerable())
            .Add(g => g.ShowColumnPicker, true)
            .Add(g => g.HiddenColumnsChanged, (IReadOnlyCollection<string> h) => reported = [.. h])
            .Add(g => g.Columns, Cols()));

        Assert.Equal(2, cut.FindAll("thead th[role='columnheader']").Count);

        cut.Find(".flare-datagrid__column-picker-wrap button").Click(); // open the picker
        cut.FindAll(".flare-datagrid__column-picker input[type=checkbox]")[1].Change(false); // hide Department

        Assert.Single(cut.FindAll("thead th[role='columnheader']"));
        Assert.NotNull(reported);
        Assert.Contains("dept", reported!); // tracked by key, not title
    }
}

// ------------------------------------------------------------------------------
// FlareColumn per-column strategies: custom SortComparison + FilterFunc
// ------------------------------------------------------------------------------

public class C_FlareDataGridColumnStrategyTests : FlareTestContext
{
    private record Ticket(string Name, string Priority);

    private static readonly Ticket[] _tickets =
    [
        new("A", "Low"),
        new("B", "High"),
        new("C", "Medium"),
    ];

    private static int Rank(string p) => p switch { "High" => 0, "Medium" => 1, "Low" => 2, _ => 3 };

    private static RenderFragment Grid(Comparison<Ticket>? sortComparison = null, Func<Ticket, string, bool>? filterFunc = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Ticket>>(0);
        b.AddAttribute(1, "Items", _tickets.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Ticket>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Ticket, object?>)(t => t.Name));
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Ticket>>(20);
            inner.AddAttribute(21, "Title", "Priority");
            inner.AddAttribute(22, "Field", (Func<Ticket, object?>)(t => t.Priority));
            if (sortComparison is not null)
            {
                inner.AddAttribute(23, "Sortable", true);
                inner.AddAttribute(24, "SortComparison", sortComparison);
            }
            if (filterFunc is not null)
            {
                inner.AddAttribute(25, "Filterable", true);
                inner.AddAttribute(26, "FilterDebounceMs", 0);
                inner.AddAttribute(27, "FilterFunc", filterFunc);
            }
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void SortComparison_OrdersByDomainRule()
    {
        // Lexical sort would give High, Low, Medium; the custom comparison sorts by priority rank.
        var cut = Render(Grid(sortComparison: (a, b) => Rank(a.Priority).CompareTo(Rank(b.Priority))));
        cut.FindAll("th.flare-datagrid__th--sortable").First(t => t.TextContent.Contains("Priority")).Click();

        var rows = cut.FindAll("tr.flare-datagrid__row");
        Assert.Equal("B", rows[0].QuerySelectorAll("td")[0].TextContent.Trim()); // High
        Assert.Equal("C", rows[1].QuerySelectorAll("td")[0].TextContent.Trim()); // Medium
        Assert.Equal("A", rows[2].QuerySelectorAll("td")[0].TextContent.Trim()); // Low
    }

    [Fact]
    public void FilterFunc_AppliesCustomPredicate()
    {
        // Custom predicate: keep tickets whose priority rank is <= the typed number.
        var cut = Render(Grid(filterFunc: (t, text) => int.TryParse(text, out var max) && Rank(t.Priority) <= max));
        cut.Find(".flare-datagrid__filter-row .flare-input__control").Input("0"); // only High

        var rows = cut.FindAll("tr.flare-datagrid__row");
        Assert.Single(rows);
        Assert.Contains("B", rows[0].TextContent);
    }
}

// ------------------------------------------------------------------------------
// Type-aware cell rendering + auto-detection (ColumnDataType / DataGridValueFormatter)
// ------------------------------------------------------------------------------

public class C_DataGridValueFormatterTests
{
    private static readonly System.Globalization.CultureInfo Inv = System.Globalization.CultureInfo.InvariantCulture;

    [Theory]
    [InlineData(typeof(bool), ColumnDataType.Boolean)]
    [InlineData(typeof(bool?), ColumnDataType.Boolean)]
    [InlineData(typeof(DateOnly), ColumnDataType.Date)]
    [InlineData(typeof(DateTime), ColumnDataType.DateTime)]
    [InlineData(typeof(DateTimeOffset), ColumnDataType.DateTime)]
    [InlineData(typeof(TimeOnly), ColumnDataType.Time)]
    [InlineData(typeof(TimeSpan), ColumnDataType.Time)]
    [InlineData(typeof(int), ColumnDataType.Number)]
    [InlineData(typeof(decimal?), ColumnDataType.Number)]
    [InlineData(typeof(double), ColumnDataType.Number)]
    [InlineData(typeof(DayOfWeek), ColumnDataType.Enum)]
    [InlineData(typeof(string), ColumnDataType.Text)]
    [InlineData(null, ColumnDataType.Text)]
    public void Infer_MapsClrTypeToDataType(Type? clr, ColumnDataType expected)
        => Assert.Equal(expected, DataGridValueFormatter.Infer(clr));

    [Fact]
    public void Resolve_Auto_UsesSampleRuntimeType()
        => Assert.Equal(ColumnDataType.Boolean, DataGridValueFormatter.Resolve(ColumnDataType.Auto, true));

    [Fact]
    public void Resolve_Explicit_WinsOverSample()
        => Assert.Equal(ColumnDataType.Text, DataGridValueFormatter.Resolve(ColumnDataType.Text, 42));

    [Fact]
    public void FormatText_Null_UsesNullText()
        => Assert.Equal("n/a", DataGridValueFormatter.FormatText(null, ColumnDataType.Text, null, "n/a", Inv));

    [Fact]
    public void FormatText_ExplicitFormat_WinsForFormattable()
        => Assert.Equal("12.50", DataGridValueFormatter.FormatText(12.5m, ColumnDataType.Number, "0.00", null, Inv));

    [Fact]
    public void FormatText_Date_DropsTime()
        => Assert.Equal("06/21/2026",
            DataGridValueFormatter.FormatText(new DateTime(2026, 6, 21, 13, 45, 0), ColumnDataType.Date, null, null, Inv));

    [Theory]
    [InlineData(ColumnDataType.Number, ColumnFilterType.Number)]
    [InlineData(ColumnDataType.Date, ColumnFilterType.Date)]
    [InlineData(ColumnDataType.DateTime, ColumnFilterType.Date)]
    [InlineData(ColumnDataType.Boolean, ColumnFilterType.Select)]
    [InlineData(ColumnDataType.Enum, ColumnFilterType.Select)]
    [InlineData(ColumnDataType.Text, ColumnFilterType.Text)]
    public void ToFilterType_MapsDataTypeToEditor(ColumnDataType type, ColumnFilterType expected)
        => Assert.Equal(expected, DataGridValueFormatter.ToFilterType(type));
}

public class C_FlareDataGridTypedCellTests : FlareTestContext
{
    private record Row(string Name, bool Active, DateTime Created, decimal Amount);

    private static readonly Row[] _rows =
    [
        new("Alice", true, new DateTime(2026, 6, 21, 9, 30, 0), 1234.5m),
        new("Bob", false, new DateTime(2026, 1, 2, 0, 0, 0), 7.25m),
    ];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Row>>(10);
            inner.AddAttribute(11, "Title", "Active");
            inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Active));
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void BoolColumn_AutoDetected_RendersCheckboxIcon()
    {
        var cut = Render(Grid());
        var icons = cut.FindAll(".flare-datagrid__bool");
        Assert.Equal(2, icons.Count);
        Assert.Contains(icons, i => i.TextContent.Trim() == "check_box");            // Alice = true
        Assert.Contains(icons, i => i.TextContent.Trim() == "check_box_outline_blank"); // Bob = false
    }

    [Fact]
    public void BoolColumn_True_GetsOnStateClass()
    {
        var cut = Render(Grid());
        Assert.Contains(cut.FindAll(".flare-datagrid__bool--on"), i => i.TextContent.Trim() == "check_box");
        Assert.Contains(cut.FindAll(".flare-datagrid__bool--off"), i => i.TextContent.Trim() == "check_box_outline_blank");
    }

    [Fact]
    public void DateColumn_WithType_DropsTimeComponent()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10);
                inner.AddAttribute(11, "Title", "Created");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Created));
                inner.AddAttribute(13, "Type", ColumnDataType.Date);
                inner.CloseComponent();
            })));

        var firstCell = cut.FindAll("tr.flare-datagrid__row td")[0].TextContent.Trim();
        Assert.Equal("06/21/2026", firstCell);
        Assert.DoesNotContain(":", firstCell); // no time
    }

    [Fact]
    public void NumberColumn_WithFormat_AppliesFormatString()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10);
                inner.AddAttribute(11, "Title", "Amount");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Amount));
                inner.AddAttribute(13, "Format", "N2");
                inner.CloseComponent();
            })));

        Assert.Equal("1,234.50", cut.FindAll("tr.flare-datagrid__row td")[0].TextContent.Trim());
    }
}

public class C_FlareDataGridAlignmentTests : FlareTestContext
{
    private record Row(string Name, bool Active, decimal Amount);

    private static readonly Row[] _rows = [new("Alice", true, 10m), new("Bob", false, 20m)];

    // Renders Name (text), Active (bool), Amount (number); optionally overrides Amount's Align.
    private static RenderFragment Grid(ColumnAlign? amountAlign = null) => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Row>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name));
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Row>>(20);
            inner.AddAttribute(21, "Title", "Active");
            inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Active));
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Row>>(30);
            inner.AddAttribute(31, "Title", "Amount");
            inner.AddAttribute(32, "Field", (Func<Row, object?>)(r => r.Amount));
            if (amountAlign is not null) inner.AddAttribute(33, "Align", amountAlign.Value);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void NumberColumn_AutoAlignsEnd()
    {
        var cut = Render(Grid());
        var cells = cut.FindAll("tr.flare-datagrid__row")[0].QuerySelectorAll("td");
        Assert.Contains("flare-datagrid__cell--end", cells[2].ClassName);   // Amount
        Assert.DoesNotContain("flare-datagrid__cell--", cells[0].ClassName); // Name (text) = leading, no class
    }

    [Fact]
    public void BoolColumn_AutoAlignsCenter()
    {
        var cut = Render(Grid());
        var cells = cut.FindAll("tr.flare-datagrid__row")[0].QuerySelectorAll("td");
        Assert.Contains("flare-datagrid__cell--center", cells[1].ClassName); // Active
    }

    [Fact]
    public void Header_MatchesCellAlignment()
    {
        var cut = Render(Grid());
        var ths = cut.FindAll("th.flare-datagrid__th");
        Assert.Contains("flare-datagrid__cell--center", ths[1].ClassName); // Active header
        Assert.Contains("flare-datagrid__cell--end", ths[2].ClassName);    // Amount header
    }

    [Fact]
    public void ExplicitAlign_OverridesTypeDefault()
    {
        // A number column forced to Start must not get the auto end-alignment.
        var cut = Render(Grid(amountAlign: ColumnAlign.Start));
        var amountCell = cut.FindAll("tr.flare-datagrid__row")[0].QuerySelectorAll("td")[2];
        Assert.DoesNotContain("flare-datagrid__cell--end", amountCell.ClassName);
    }
}

public class C_FlareDataGridComputedColumnTests : FlareTestContext
{
    private record Person(string First, string Last, int Age);

    // Deliberately out of full-name order so a working sort visibly reorders them.
    private static readonly Person[] _people =
        [new("Carol", "Smith", 40), new("Alice", "Adams", 30), new("Bob", "Smith", 25)];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Person>>(0);
        b.AddAttribute(1, "Items", _people.AsEnumerable());
        b.AddAttribute(2, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Person>>(10);
            // Title is not a property name and the value is computed: the old reflection-based
            // pipeline could not resolve it for sort/filter; the compiled Field selector can.
            inner.AddAttribute(11, "Title", "Full name");
            inner.AddAttribute(12, "Field", (Func<Person, object?>)(p => $"{p.First} {p.Last}"));
            inner.AddAttribute(13, "Sortable", true);
            inner.AddAttribute(14, "Filterable", true);
            inner.AddAttribute(15, "FilterDebounceMs", 0); // apply instantly (no debounce timer in the test)
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Filter_ByComputedColumn_Works()
    {
        var cut = Render(Grid());
        cut.Find(".flare-datagrid__filter-row .flare-input__control").Input("Smith");

        var rows = cut.FindAll("tr.flare-datagrid__row");
        Assert.Equal(2, rows.Count);
        Assert.All(rows, r => Assert.Contains("Smith", r.TextContent));
    }

    [Fact]
    public void Sort_ByComputedColumn_OrdersByAccessor()
    {
        var cut = Render(Grid());
        cut.Find("th.flare-datagrid__th--sortable").Click(); // ascending by full name

        var rows = cut.FindAll("tr.flare-datagrid__row");
        Assert.Equal("Alice Adams", rows[0].QuerySelector("td")!.TextContent.Trim());
        Assert.Equal("Bob Smith", rows[1].QuerySelector("td")!.TextContent.Trim());
        Assert.Equal("Carol Smith", rows[2].QuerySelector("td")!.TextContent.Trim());
    }
}

public class C_FlareDataGridMenuFilterTests : FlareTestContext
{
    private record Fruit(string Name);

    private static readonly Fruit[] _items = [new("Apple"), new("Banana"), new("Cherry")];

    private static RenderFragment Grid() => b =>
    {
        b.OpenComponent<FlareDataGrid<Fruit>>(0);
        b.AddAttribute(1, "Items", _items.AsEnumerable());
        b.AddAttribute(2, "FilterMode", DataGridFilterMode.Menu);
        b.AddAttribute(3, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Fruit>>(10);
            inner.AddAttribute(11, "Title", "Name"); // key "Name" resolves the property for the pipeline
            inner.AddAttribute(12, "Field", (Func<Fruit, object?>)(f => f.Name));
            inner.AddAttribute(13, "Sortable", true);
            inner.AddAttribute(14, "Filterable", true);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void MenuMode_RendersTriggerButton_NotInlineFilterRow()
    {
        var cut = Render(Grid());
        Assert.Single(cut.FindAll("button.flare-datagrid__filter-trigger"));
        // Menu mode must not also render the always-on inline filter row.
        Assert.Empty(cut.FindAll(".flare-datagrid__filter-row"));
    }

    [Fact]
    public void Trigger_TogglesPanel()
    {
        var cut = Render(Grid());
        Assert.Empty(cut.FindAll(".flare-datagrid__filter-menu"));
        cut.Find("button.flare-datagrid__filter-trigger").Click();
        Assert.Single(cut.FindAll(".flare-datagrid__filter-menu"));
        cut.Find("button.flare-datagrid__filter-trigger").Click();
        Assert.Empty(cut.FindAll(".flare-datagrid__filter-menu"));
    }

    [Fact]
    public void ApplyFilter_FiltersRows_AndMarksTriggerActive()
    {
        var cut = Render(Grid());
        cut.Find("button.flare-datagrid__filter-trigger").Click(); // open

        // Fields in order: [0] search box, [1] operator select, [2] condition value input.
        var valueField = cut.FindAll(".flare-datagrid__filter-menu-field")[2];
        valueField.QuerySelector("input")!.Input("Ban"); // default operator = contains; all values stay checked

        // Apply is the filled button in the actions row.
        cut.FindAll(".flare-datagrid__filter-menu-actions button")[^1].Click();

        var rows = cut.FindAll("tr.flare-datagrid__row");
        Assert.Single(rows);
        Assert.Contains("Banana", rows[0].TextContent);
        Assert.Single(cut.FindAll("button.flare-datagrid__filter-trigger--active"));
    }
}

public class C_DataGridPipelineTypedFilterTests
{
    private record Row(int Score, DateTime Date);

    private static readonly Row[] _rows =
    [
        new(9,   new DateTime(2026, 1, 1)),
        new(85,  new DateTime(2026, 6, 1)),
        new(100, new DateTime(2026, 12, 1)),
    ];

    private static DataGridColumnStrategies<Row> Strategies() => new()
    {
        FilterSelectors = new Dictionary<string, Func<Row, object?>>
        {
            ["Score"] = r => r.Score,
            ["Date"] = r => r.Date,
        },
        ColumnTypes = new Dictionary<string, ColumnDataType>
        {
            ["Score"] = ColumnDataType.Number,
            ["Date"] = ColumnDataType.DateTime,
        },
    };

    private static List<int> RunScores(DataGridFilter f) =>
        DataGridPipeline<Row>.Execute(_rows, [], [f], null, null, null, null, 0, 100, Strategies())
            .Items.Select(r => r.Score).ToList();

    [Fact]
    public void Number_GreaterThan_ComparesNumerically_NotLexically()
    {
        // Lexically "9" > "85" and "100" < "85"; numerically only 100 > 85.
        Assert.Equal([100], RunScores(new DataGridFilter("Score", FilterOperator.GreaterThan, "85")));
    }

    [Fact]
    public void Number_Equals_IgnoresNumericFormatting()
    {
        // "85" != "85.0" as strings; equal as numbers.
        Assert.Equal([85], RunScores(new DataGridFilter("Score", FilterOperator.Equals, "85.0")));
    }

    [Fact]
    public void Number_Between_IsInclusive()
    {
        Assert.Equal([85], RunScores(new DataGridFilter("Score", FilterOperator.Between, "10", "90")));
    }

    [Fact]
    public void Date_GreaterThan_ComparesChronologically_FromIsoInput()
    {
        // ISO date input vs DateTime cells; June and December are after 2026-05-01.
        var n = DataGridPipeline<Row>.Execute(_rows, [], [new DataGridFilter("Date", FilterOperator.GreaterThan, "2026-05-01")],
            null, null, null, null, 0, 100, Strategies()).Items.Count();
        Assert.Equal(2, n);
    }

    [Fact]
    public void Date_Between_SelectsRange()
    {
        var n = DataGridPipeline<Row>.Execute(_rows, [], [new DataGridFilter("Date", FilterOperator.Between, "2026-03-01", "2026-09-01")],
            null, null, null, null, 0, 100, Strategies()).Items.Count();
        Assert.Equal(1, n); // only June
    }
}

public class C_FlareDataGridExportTests : FlareTestContext
{
    private record Row(string Name, decimal Amount, DateTime Date, bool Active);

    private static readonly Row[] _rows =
    [
        new("Alice", 1234.5m, new DateTime(2026, 6, 21, 9, 0, 0), true),
        new("Bob",   50m,     new DateTime(2026, 1, 2, 0, 0, 0),  false),
    ];

    private sealed class CapturingDownload : IFlareDownload
    {
        public string? Content;
        public string? FileName;
        public ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false)
        { FileName = filename; Content = content; return default; }
        public ValueTask DownloadCsvAsync(string filename, string csv)
        { FileName = filename; Content = csv; return default; }
        public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null)
        { FileName = filename; Content = System.Text.Encoding.UTF8.GetString(bytes); return default; }
    }

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Name");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Amount");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Amount)); inner.AddAttribute(s++, "Format", "N2"); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Date");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Date)); inner.AddAttribute(s++, "Type", ColumnDataType.Date); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Active");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Active)); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        return Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.Columns, Cols()));
    }

    [Fact]
    public void GetExportData_AppliesTypeAwareText()
    {
        var data = RenderGrid().Instance.GetExportData("people");

        Assert.Equal("people", data.FileName);
        Assert.Equal(4, data.Columns.Count);
        Assert.Equal(2, data.Rows.Count);
        Assert.Equal("1,234.50", data.Columns.First(c => c.Title == "Amount").TextOf(_rows[0])); // N2
        Assert.Equal("06/21/2026", data.Columns.First(c => c.Title == "Date").TextOf(_rows[0])); // date only
        Assert.Equal("true", data.Columns.First(c => c.Title == "Active").TextOf(_rows[0]));      // bool -> text
    }

    [Fact]
    public async Task CsvExport_WritesFormattedValues()
    {
        var data = RenderGrid().Instance.GetExportData("people");
        var dl = new CapturingDownload();
        await new CsvGridExporter<Row>().ExportAsync(data, dl);

        Assert.Equal("people.csv", dl.FileName);
        Assert.Contains("1,234.50", dl.Content);   // formatted, not "1234.5"
        Assert.Contains("06/21/2026", dl.Content);  // date only, no time
        Assert.Contains("true", dl.Content);
    }

    [Fact]
    public void Export_InToolbarContent_ResolvesGridFromCascade()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var dl = new CapturingDownload();
        Services.AddScoped<IFlareDownload>(_ => dl);

        // No Grid is passed: the export resolves the enclosing grid via the toolbar cascade.
        RenderFragment toolbar = tb =>
        {
            tb.OpenComponent<DataGridExport<Row>>(0);
            tb.AddAttribute(1, "FileName", "people");
            tb.AddAttribute(2, "Exporters",
                (IReadOnlyList<IDataGridExporter<Row>>)[DataGridExporters.Csv<Row>()]);
            tb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.ToolbarContent, toolbar)
            .Add(x => x.Columns, Cols()));

        cut.Find(".flare-datagrid__toolbar button").Click();

        Assert.Equal("people.csv", dl.FileName);
        Assert.Contains("Alice", dl.Content);
        Assert.Contains("1,234.50", dl.Content); // grid's N2 format applied via the resolved grid
    }

    [Fact]
    public void Export_Split_RendersSplitButton_PrimaryExportsFirstExporter()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var dl = new CapturingDownload();
        Services.AddScoped<IFlareDownload>(_ => dl);

        RenderFragment toolbar = tb =>
        {
            tb.OpenComponent<DataGridExport<Row>>(0);
            tb.AddAttribute(1, "FileName", "people");
            tb.AddAttribute(2, "Split", true);
            tb.AddAttribute(3, "Exporters",
                (IReadOnlyList<IDataGridExporter<Row>>)[DataGridExporters.Csv<Row>(), DataGridExporters.Json<Row>()]);
            tb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.ToolbarContent, toolbar)
            .Add(x => x.Columns, Cols()));

        // Split layout: one split-button host with a primary action, not a button per exporter.
        Assert.Single(cut.FindAll(".flare-split-btn"));
        var primary = cut.Find(".flare-split-btn__main");
        Assert.Contains("CSV", primary.TextContent); // first exporter is the primary action

        primary.Click();

        Assert.Equal("people.csv", dl.FileName); // primary runs the first (CSV) exporter
        Assert.Contains("Alice", dl.Content);
    }
}

public class C_FlareDataGridCellSelectionTests : FlareTestContext
{
    private sealed class CapturingClipboard : IFlareClipboard
    {
        public string? Text;
        public string ReadText = string.Empty;
        public ValueTask CopyAsync(string text) { Text = text; return default; }
        public ValueTask<string> ReadAsync() => new(ReadText);
    }

    private record Row(string Name, int A, int B);
    private static readonly Row[] _rows = [new("r1", 1, 2), new("r2", 3, 4), new("r3", 5, 6)];

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        foreach (var (title, get) in new (string, Func<Row, object?>)[]
            { ("Name", r => r.Name), ("A", r => r.A), ("B", r => r.B) })
        {
            inner.OpenComponent<FlareColumn<Row>>(s++);
            inner.AddAttribute(s++, "Title", title);
            inner.AddAttribute(s++, "Field", get);
            inner.CloseComponent();
        }
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid(CapturingClipboard? clip = null)
    {
        if (clip is not null) Services.AddScoped<IFlareClipboard>(_ => clip);
        return Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.CellSelection, true)
            .Add(x => x.Columns, Cols()));
    }

    private static void Key(IRenderedComponent<FlareDataGrid<Row>> cut, string key, bool shift = false, bool ctrl = false)
        => cut.Find(".flare-datagrid__table").KeyDown(new KeyboardEventArgs { Key = key, ShiftKey = shift, CtrlKey = ctrl });

    [Fact]
    public void ShiftArrow_ExtendsRange_HighlightsCells()
    {
        var cut = RenderGrid();
        Key(cut, "ArrowDown");                 // init active cell (0,0)
        Key(cut, "ArrowRight", shift: true);   // extend to (0,1)
        Key(cut, "ArrowDown", shift: true);    // extend to (1,1) -> 2x2 block

        // 4 body cells in the 2x2 block are highlighted.
        Assert.Equal(4, cut.FindAll("td.flare-datagrid__cell--range").Count);
    }

    [Fact]
    public void CtrlC_CopiesRangeAsTsv()
    {
        var clip = new CapturingClipboard();
        var cut = RenderGrid(clip);
        Key(cut, "ArrowDown");                 // (0,0)
        Key(cut, "ArrowRight", shift: true);   // -> (0,1): columns Name, A
        Key(cut, "ArrowDown", shift: true);    // -> (1,1): rows r1, r2
        Key(cut, "c", ctrl: true);

        Assert.Equal("r1\t1\nr2\t3", clip.Text);
    }

    [Fact]
    public void PlainArrow_CollapsesRange()
    {
        var cut = RenderGrid();
        Key(cut, "ArrowDown");                 // (0,0)
        Key(cut, "ArrowRight", shift: true);   // range (0,0)-(0,1)
        Key(cut, "ArrowDown");                 // plain move collapses to single cell (1,1)

        Assert.Single(cut.FindAll("td.flare-datagrid__cell--range"));
    }

    [Fact]
    public void MouseDrag_SelectsRectangularRange()
    {
        var cut = RenderGrid();
        cut.FindAll("tr.flare-datagrid__row")[0].QuerySelectorAll("td")[0]
            .TriggerEvent("onmousedown", new MouseEventArgs()); // start at (0,0)
        // Re-query after the re-render so the handler IDs are current.
        cut.FindAll("tr.flare-datagrid__row")[1].QuerySelectorAll("td")[1]
            .TriggerEvent("onmouseenter", new MouseEventArgs()); // drag to (1,1)

        Assert.Equal(4, cut.FindAll("td.flare-datagrid__cell--range").Count); // 2x2 block
    }

    [Fact]
    public async Task CtrlV_PastesTsv_AndRaisesOnPaste()
    {
        var clip = new CapturingClipboard { ReadText = "x\ty\nz\tw" };
        Services.AddScoped<IFlareClipboard>(_ => clip);
        DataGridPaste<Row>? received = null;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.CellSelection, true)
            .Add(x => x.OnPaste, EventCallback.Factory.Create<DataGridPaste<Row>>(this, dp => received = dp))
            .Add(x => x.Columns, Cols()));

        cut.Find(".flare-datagrid__table").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" }); // active cell (0,0)
        await cut.Find(".flare-datagrid__table").KeyDownAsync(new KeyboardEventArgs { Key = "v", CtrlKey = true });

        Assert.NotNull(received);
        Assert.Equal(4, received!.Cells.Count);
        // Pasted block maps onto Name (col 0) and A (col 1) of rows r1, r2.
        Assert.Contains(received.Cells, c => c.ColumnKey == "Name" && c.Value == "x");
        Assert.Contains(received.Cells, c => c.ColumnKey == "A" && c.Value == "y");
        Assert.Contains(received.Cells, c => c.ColumnKey == "A" && c.Value == "w");
    }
}

public class C_FlareDataGridTypedEditorTests : FlareTestContext
{
    private enum Status { Open, Closed }
    private record Row(string Name, bool Active, int Score, Status State);

    private static readonly Row[] _rows =
    [
        new("Alice", true, 10, Status.Open),
        new("Bob", false, 20, Status.Closed),
    ];

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        void Col(string title, Func<Row, object?> get)
        {
            inner.OpenComponent<FlareColumn<Row>>(s++);
            inner.AddAttribute(s++, "Title", title);
            inner.AddAttribute(s++, "Field", get);
            inner.AddAttribute(s++, "Editable", true);
            inner.CloseComponent();
        }
        Col("Name", r => r.Name);
        Col("Active", r => r.Active);
        Col("Score", r => r.Score);
        Col("State", r => r.State);
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderEditing()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.EditMode, DataGridEditMode.Inline)
            .Add(x => x.Columns, Cols()));
        // Begin editing the first row via its edit action button.
        cut.FindAll("td.flare-datagrid__td--edit-actions button")[0].Click();
        return cut;
    }

    [Fact]
    public void BoolColumn_EditsWithCheckbox()
    {
        var cut = RenderEditing();
        var editRow = cut.FindAll("tr.flare-datagrid__row")[0];
        Assert.NotEmpty(editRow.QuerySelectorAll(".flare-checkbox"));
    }

    [Fact]
    public void EnumColumn_EditsWithSelect()
    {
        var cut = RenderEditing();
        var editRow = cut.FindAll("tr.flare-datagrid__row")[0];
        Assert.NotEmpty(editRow.QuerySelectorAll(".flare-select"));
    }

    [Fact]
    public void NumberColumn_EditsWithNumberInput()
    {
        var cut = RenderEditing();
        var editRow = cut.FindAll("tr.flare-datagrid__row")[0];
        var numberInputs = editRow.QuerySelectorAll("input[type=number]");
        Assert.NotEmpty(numberInputs);
    }

    [Fact]
    public void NumberSeed_IsInvariant()
    {
        var cut = RenderEditing();
        var values = cut.Instance.GetEditValues();
        Assert.Equal("10", values["Score"]);   // invariant
        Assert.Equal("True", values["Active"]); // bool round-trips
    }
}

public class C_FlareDataGridExcelFilterTests : FlareTestContext
{
    private record Fruit(int Id, string Name);
    private static readonly Fruit[] _items =
        [new(1, "Apple"), new(2, "Banana"), new(3, "Cherry"), new(4, "Banana")];

    private IRenderedComponent<FlareDataGrid<Fruit>> RenderGrid() =>
        Render<FlareDataGrid<Fruit>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.FilterMode, DataGridFilterMode.Menu)
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Fruit>>(10);
                inner.AddAttribute(11, "Title", "Name");
                inner.AddAttribute(12, "Field", (Func<Fruit, object?>)(f => f.Name));
                inner.AddAttribute(13, "Filterable", true);
                inner.CloseComponent();
            })));

    [Fact]
    public void Menu_ShowsDistinctValueChecklist()
    {
        var cut = RenderGrid();
        cut.Find("button.flare-datagrid__filter-trigger").Click();

        var checks = cut.FindAll(".flare-datagrid__filter-menu-list .flare-checkbox");
        // 3 distinct values (Apple/Banana/Cherry) + the "(Select all)" row.
        Assert.Equal(4, checks.Count);
        Assert.Contains(checks, c => c.TextContent.Contains("Apple"));
        Assert.Contains(checks, c => c.TextContent.Contains("Banana"));
        Assert.Contains(checks, c => c.TextContent.Contains("Cherry"));
    }

    [Fact]
    public void Uncheck_Value_AppliesInFilter_ExcludingIt()
    {
        var cut = RenderGrid();
        cut.Find("button.flare-datagrid__filter-trigger").Click();

        var banana = cut.FindAll(".flare-datagrid__filter-menu-list .flare-checkbox")
            .First(c => c.TextContent.Contains("Banana"));
        banana.QuerySelector("input")!.Change(false); // uncheck Banana

        cut.FindAll(".flare-datagrid__filter-menu-actions button")[^1].Click(); // Apply

        var rows = cut.FindAll("tr.flare-datagrid__row");
        Assert.Equal(2, rows.Count); // Apple, Cherry (both Bananas excluded)
        Assert.DoesNotContain(rows, r => r.TextContent.Contains("Banana"));
        Assert.Single(cut.FindAll("button.flare-datagrid__filter-trigger--active"));
    }

    [Fact]
    public void Search_FiltersTheChecklist()
    {
        var cut = RenderGrid();
        cut.Find("button.flare-datagrid__filter-trigger").Click();
        cut.Find(".flare-datagrid__filter-menu .flare-input__control").Input("ban");

        var values = cut.FindAll(".flare-datagrid__filter-menu-list .flare-checkbox")
            .Where(c => !c.TextContent.Contains("Select all")).ToList();
        Assert.Single(values);
        Assert.Contains("Banana", values[0].TextContent);
    }
}

public class C_FlareDataGridPinnedRowsTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly Row[] _data =
        Enumerable.Range(1, 20).Select(i => new Row($"Item {i}", i)).ToArray();
    private static readonly Row[] _top = [new("TOPROW", 0)];
    private static readonly Row[] _bottom = [new("BOTTOMROW", 999)];

    private static RenderFragment Cols() => inner =>
    {
        var s = 0;
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Name");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(s++); inner.AddAttribute(s++, "Title", "Value");
        inner.AddAttribute(s++, "Field", (Func<Row, object?>)(r => r.Value)); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid() =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.PinnedTopRows, _top)
            .Add(x => x.PinnedBottomRows, _bottom)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public void PinnedTop_RendersInThead()
    {
        var cut = RenderGrid();
        var pinned = cut.FindAll("thead tr.flare-datagrid__row--pinned");
        Assert.Single(pinned);
        Assert.Contains("TOPROW", pinned[0].TextContent);
    }

    [Fact]
    public void PinnedBottom_RendersInTfoot()
    {
        var cut = RenderGrid();
        var pinned = cut.FindAll("tfoot tr.flare-datagrid__row--pinned");
        Assert.Single(pinned);
        Assert.Contains("BOTTOMROW", pinned[0].TextContent);
    }

    [Fact]
    public void PinnedRows_AreOutsidePaging()
    {
        var cut = RenderGrid();
        var bodyRows = cut.FindAll("tbody tr.flare-datagrid__row");
        Assert.Equal(5, bodyRows.Count); // page size unaffected by the pinned rows
        Assert.DoesNotContain(bodyRows, r => r.TextContent.Contains("TOPROW") || r.TextContent.Contains("BOTTOMROW"));
    }
}

public class C_FlareDataGridFrozenRightTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly Row[] _data = [new("A", 1), new("B", 2)];

    [Fact]
    public void FrozenRightColumn_GetsFrozenRightClass_OnHeaderAndCells()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
                inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
                inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "Value");
                inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Value));
                inner.AddAttribute(23, "FrozenRight", true); inner.CloseComponent();
            })));

        Assert.Single(cut.FindAll("th.flare-datagrid__th--frozen-right"));
        Assert.Equal(2, cut.FindAll("td.flare-datagrid__td--frozen-right").Count); // one per data row
        // The table opts into horizontal-scroll layout when any column is frozen (left or right).
        Assert.NotEmpty(cut.FindAll("table.flare-datagrid__table--scroll-x"));
    }
}

public class C_PdfWriterTests
{
    private static string Pdf(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows, string? title = null)
        => System.Text.Encoding.Latin1.GetString(PdfWriter.Write(headers, rows, title));

    [Fact]
    public void Write_ProducesValidPdf_WithHeaderAndCellText()
    {
        var text = Pdf(["Name", "Score"],
            [["Alice", "92"], ["Bob", "78"]], title: "people");

        Assert.StartsWith("%PDF-1.4", text);
        Assert.Contains("%%EOF", text);
        Assert.Contains("/BaseFont /Helvetica", text);
        Assert.Contains("(Name)", text);   // header in the content stream
        Assert.Contains("(Alice)", text);  // data cell
        Assert.Contains("(people)", text); // title
    }

    [Fact]
    public void Write_EscapesParensAndDropsNonLatin()
    {
        var bs = ((char)92).ToString();              // a single backslash
        var cyrillic = ((char)0x0429).ToString();    // a Cyrillic letter (outside Latin-1)
        var text = Pdf(["A"], [["x(y) " + cyrillic]]);
        Assert.Contains("x" + bs + "(y" + bs + ") ?", text); // parens escaped, non-Latin -> '?'
    }

    [Fact]
    public void Write_PaginatesManyRows()
    {
        var rows = Enumerable.Range(0, 200)
            .Select(i => (IReadOnlyList<string?>)new[] { $"Row {i}", "v" }).ToList();
        var text = Pdf(["A", "B"], rows);

        var pages = System.Text.RegularExpressions.Regex.Matches(text, "/MediaBox").Count;
        Assert.True(pages > 1, $"expected multiple pages, got {pages}");
    }
}

public class C_DataGridQueryTests
{
    private record Person(string Name, int Age, DateTime Hired, bool Active);

    private static readonly Person[] _people =
    [
        new("Alice", 30, new DateTime(2021, 3, 1), true),
        new("bob",   25, new DateTime(2019, 11, 2), false),
        new("Carol", 40, new DateTime(2022, 6, 30), true),
        new("Dave",  35, new DateTime(2020, 1, 20), false),
    ];

    private static IQueryable<Person> Q => _people.AsQueryable();

    private static DataGridRequest Req(IReadOnlyList<DataGridFilter>? filters = null,
        IReadOnlyList<DataGridSort>? sorts = null, int page = 0, int size = 100)
        => new(page, size, null, SortDirection.Ascending) { FilterModel = filters ?? [], Sorts = sorts ?? [] };

    private static DataGridResult<Person> Run(IReadOnlyList<DataGridFilter>? f = null,
        IReadOnlyList<DataGridSort>? s = null, int page = 0, int size = 100)
        => DataGridQuery.Execute(Q, Req(f, s, page, size));

    [Fact]
    public void Contains_IsCaseInsensitive()
        => Assert.Equal(3, Run([new("Name", FilterOperator.Contains, "A")]).TotalCount); // Alice, Carol, Dave

    [Fact]
    public void Equals_String_IsCaseInsensitive()
    {
        var res = Run([new("Name", FilterOperator.Equals, "BOB")]);
        Assert.Equal("bob", Assert.Single(res.Items).Name);
    }

    [Fact]
    public void GreaterThan_ComparesNumbers()
        => Assert.Equal(2, Run([new("Age", FilterOperator.GreaterThan, "30")]).TotalCount); // 40, 35

    [Fact]
    public void Between_IsInclusiveNumeric()
        => Assert.Equal(2, Run([new("Age", FilterOperator.Between, "26", "36")]).TotalCount); // 30, 35

    [Fact]
    public void Date_GreaterThan_FromIso()
        => Assert.Equal(2, Run([new("Hired", FilterOperator.GreaterThan, "2021-01-01")]).TotalCount); // 2021, 2022

    [Fact]
    public void Bool_Equals()
        => Assert.Equal(2, Run([new("Active", FilterOperator.Equals, "true")]).TotalCount);

    [Fact]
    public void In_MatchesAnyValue()
        => Assert.Equal(2, Run([new("Name", FilterOperator.In, Values: ["Alice", "Carol"])]).TotalCount);

    [Fact]
    public void Sort_Descending_WithPaging()
    {
        var res = Run(s: [new("Age", SortDirection.Descending)], page: 0, size: 2);
        Assert.Equal(4, res.TotalCount);            // total before paging
        Assert.Equal(2, res.Items.Count());         // page size
        Assert.Equal(40, res.Items.First().Age);    // highest first
    }

    [Fact]
    public void MultiSort_AppliesThenBy()
    {
        // Active asc (false first), then Age desc.
        var res = Run(s: [new("Active", SortDirection.Ascending), new("Age", SortDirection.Descending)]);
        Assert.Equal("Dave", res.Items.First().Name); // Active=false, Age=35 (highest among inactive)
    }

    [Fact]
    public void UnknownColumn_IsSkipped()
        => Assert.Equal(4, Run([new("Nope", FilterOperator.Equals, "x")]).TotalCount);
}

public class C_FlareDataGridQueryableWiringTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly List<Row> _data =
        Enumerable.Range(1, 12).Select(i => new Row($"Item{i:00}", i)).ToList();

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Value");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Value));
        inner.AddAttribute(13, "Sortable", true); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "Name");
        inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid() =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Queryable, _data.AsQueryable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public void Queryable_PagesServerSide()
        => Assert.Equal(5, RenderGrid().FindAll("tbody tr.flare-datagrid__row").Count);

    [Fact]
    public void Queryable_SortsViaTranslation()
    {
        var cut = RenderGrid();
        // Re-query before each click - a sort reloads via the queryable and re-renders the tree.
        string FirstValue() => cut.FindAll("tbody tr.flare-datagrid__row")[0].QuerySelectorAll("td")[0].TextContent.Trim();

        cut.FindAll("th.flare-datagrid__th--sortable").First(t => t.TextContent.Contains("Value")).Click();
        Assert.Equal("1", FirstValue()); // ascending
        cut.FindAll("th.flare-datagrid__th--sortable").First(t => t.TextContent.Contains("Value")).Click();
        Assert.Equal("12", FirstValue()); // descending
    }
}

public class C_FlareDataGridA11yTests : FlareTestContext
{
    private record Row(string Name, int Value);
    private static readonly Row[] _data =
        Enumerable.Range(1, 12).Select(i => new Row($"N{i:00}", i)).ToArray();

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name));
        inner.AddAttribute(13, "Sortable", true); inner.AddAttribute(14, "Filterable", true);
        inner.AddAttribute(15, "FilterDebounceMs", 0); inner.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid() =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public void LiveRegion_IsPresent()
    {
        var status = RenderGrid().Find("div[role=status]");
        Assert.Equal("polite", status.GetAttribute("aria-live"));
    }

    [Fact]
    public void Sort_AnnouncesColumnAndDirection()
    {
        var cut = RenderGrid();
        cut.FindAll("th.flare-datagrid__th--sortable").First(t => t.TextContent.Contains("Name")).Click();
        var status = cut.Find("div[role=status]").TextContent;
        Assert.Contains("Sorted by Name", status);
        Assert.Contains("ascending", status);
    }

    [Fact]
    public void Filter_AnnouncesResultCount()
    {
        var cut = RenderGrid();
        cut.Find(".flare-datagrid__filter-row input").Input("N1"); // N10, N11, N12
        Assert.Contains("3 results", cut.Find("div[role=status]").TextContent);
    }

    [Fact]
    public void DataCells_HaveGridcellRole()
        => Assert.NotEmpty(RenderGrid().FindAll("td[role=gridcell]"));
}

public class C_FlareDataGridQuickFilterTests : FlareTestContext
{
    private record Row(string Name, string City);
    private static readonly Row[] _data = [new("Alice", "Berlin"), new("Bob", "London"), new("Carol", "Berlin")];

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "City");
        inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.City)); inner.CloseComponent();
    };

    [Fact]
    public async Task ApplyQuickFilter_MatchesAnyColumn_AndClears()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.Columns, Cols()));

        await cut.InvokeAsync(() => cut.Instance.ApplyQuickFilter("berlin")); // City match, case-insensitive
        Assert.Equal(2, cut.FindAll("tbody tr.flare-datagrid__row").Count);

        await cut.InvokeAsync(() => cut.Instance.ApplyQuickFilter(null)); // clear
        Assert.Equal(3, cut.FindAll("tbody tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void QuickFilterComponent_RendersSearchInput()
    {
        var cut = Render<FlareDataGridQuickFilter<Row>>(p => p.Add(x => x.DebounceMs, 0));
        Assert.NotEmpty(cut.FindAll("input"));
    }
}

public class C_FlareDataGridFilterPresetsTests : FlareTestContext
{
    private record Row(string Name, string City);
    private static readonly Row[] _data = [new("Alice", "Berlin"), new("Bob", "London"), new("Carol", "Berlin")];

    private static readonly DataGridFilterPreset[] _presets =
    [
        new("Berliners", new DataGridFilterGroup(false, [new DataGridFilter("City", FilterOperator.Equals, "Berlin")], [])),
        new("Londoners", new DataGridFilterGroup(false, [new DataGridFilter("City", FilterOperator.Equals, "London")], [])),
    ];

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<Row>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name)); inner.CloseComponent();
        inner.OpenComponent<FlareColumn<Row>>(20); inner.AddAttribute(21, "Title", "City");
        inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.City)); inner.CloseComponent();
    };

    [Fact]
    public void PresetsComponent_ListsNoFilterPlusPresets()
    {
        var cut = Render<FlareDataGridFilterPresets<Row>>(p => p.Add(x => x.Presets, _presets));
        cut.Find(".flare-select__control").Click();
        var options = cut.FindAll(".flare-select__option").Select(o => o.TextContent.Trim()).ToList();
        Assert.Contains(options, o => o.Contains("(No filter)"));
        Assert.Contains(options, o => o.Contains("Berliners"));
        Assert.Contains(options, o => o.Contains("Londoners"));
    }

    [Fact]
    public async Task ApplyingPresetFilter_FiltersTheGrid()
    {
        // The preset component applies its DataGridFilterGroup via Grid.ApplyAdvancedFilter.
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _data.AsEnumerable())
            .Add(x => x.Columns, Cols()));

        await cut.InvokeAsync(() => cut.Instance.ApplyAdvancedFilter(_presets[0].Filter)); // Berliners
        var rows = cut.FindAll("tbody tr.flare-datagrid__row");
        Assert.Equal(2, rows.Count);
        Assert.All(rows, r => Assert.Contains("Berlin", r.TextContent));
    }
}

public class C_FlareDataGridPagerTests : FlareTestContext
{
    // 20 items, 5 per page -> 4 pages.
    private static readonly string[] _items =
        Enumerable.Range(1, 20).Select(i => $"Item {i:00}").ToArray();

    private static RenderFragment Cols() => inner =>
    {
        inner.OpenComponent<FlareColumn<string>>(10); inner.AddAttribute(11, "Title", "Name");
        inner.AddAttribute(12, "Field", (Func<string, object?>)(s => s)); inner.CloseComponent();
    };

    [Fact]
    public void BuiltInPager_RendersByDefault()
    {
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

        Assert.Single(cut.FindAll(".flare-datagrid__pagination"));
        Assert.Equal(4, cut.Instance.PageCount); // 20 items / 5 per page
    }

    [Fact]
    public async Task GoToPage_ShowsThatPagesRows()
    {
        // Regression: client-side paging must slice the full row set, not the current page slice.
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.Columns, Cols()));

        Assert.Contains("Item 01", cut.Find("tbody").TextContent);

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(1)); // second page
        Assert.Contains("Item 06", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 01", cut.Find("tbody").TextContent);

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(3)); // last page (16..20)
        Assert.Contains("Item 20", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 15", cut.Find("tbody").TextContent);
    }

    [Fact]
    public void ShowPagerFalse_SuppressesBuiltInPager()
    {
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.ShowPager, false)
            .Add(x => x.Columns, Cols()));

        Assert.Empty(cut.FindAll(".flare-datagrid__pagination"));
    }

    [Fact]
    public void FooterContent_RendersFooter_AndPagerResolvesGridAndPaginates()
    {
        RenderFragment footer = fb =>
        {
            // No Grid passed: the pager resolves the enclosing grid via the footer cascade.
            fb.OpenComponent<FlareDataGridPager<string>>(0);
            fb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.ShowPager, false)
            .Add(x => x.FooterContent, footer)
            .Add(x => x.Columns, Cols()));

        // The footer area renders, the built-in pager does not, and the pager lives in the footer.
        Assert.Single(cut.FindAll(".flare-datagrid__footer"));
        Assert.Empty(cut.FindAll(".flare-datagrid__pagination"));
        Assert.NotEmpty(cut.FindAll(".flare-datagrid__footer .flare-pagination"));

        // Page 1 shows the first five items.
        Assert.Contains("Item 01", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 06", cut.Find("tbody").TextContent);

        // Click the page-2 button inside the footer pager -> grid advances a page.
        var pageTwo = cut.FindAll(".flare-datagrid__footer .flare-pagination button")
            .First(b => b.TextContent.Trim() == "2");
        pageTwo.Click();

        Assert.Contains("Item 06", cut.Find("tbody").TextContent);
        Assert.DoesNotContain("Item 01", cut.Find("tbody").TextContent);
    }

    [Fact]
    public void StandalonePager_WithoutGrid_RendersNothing()
    {
        var cut = Render<FlareDataGridPager<string>>();
        Assert.Empty(cut.FindAll(".flare-pagination"));
    }

    [Fact]
    public void Pager_OwnRowsPerPageOptions_OverrideGrid()
    {
        // The pager owns its presentation: its RowsPerPageOptions are used, not the grid's.
        RenderFragment footer = fb =>
        {
            fb.OpenComponent<FlareDataGridPager<string>>(0);
            fb.AddAttribute(1, "RowsPerPageOptions", (IReadOnlyList<int>)[5, 10, 25]);
            fb.CloseComponent();
        };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, _items.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.ShowPager, false)
            .Add(x => x.RowsPerPageOptions, new[] { 5, 50 }) // grid default - should be ignored by the pager
            .Add(x => x.FooterContent, footer)
            .Add(x => x.Columns, Cols()));

        var opts = cut.FindAll(".flare-datagrid__footer .flare-pagination option")
            .Select(o => o.TextContent.Trim()).ToList();
        Assert.Equal(["5", "10", "25"], opts);
    }
}

// ------------------------------------------------------------------------------
// DataGridPersistence - round-trips grid state through browser localStorage using
// the built-in localStorage.* interop (not a custom JS module export, which is the
// bug this guards against: the old code imported flare-theme.js and called exports
// that never existed, so persistence silently no-op'd / threw JSException).
// ------------------------------------------------------------------------------

public class C_DataGridPersistenceTests
{
    private record Person(string Name);

    // Minimal IJSRuntime that implements an in-memory localStorage, so SaveAsync/LoadAsync/ClearAsync
    // exercise the real interop identifiers ("localStorage.setItem" etc.) end to end.
    private sealed class FakeLocalStorageJsRuntime : IJSRuntime
    {
        public readonly Dictionary<string, string> Store = new(StringComparer.Ordinal);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            switch (identifier)
            {
                case "localStorage.setItem":
                    Store[(string)args![0]!] = (string)args![1]!;
                    return new ValueTask<TValue>(default(TValue)!);
                case "localStorage.removeItem":
                    Store.Remove((string)args![0]!);
                    return new ValueTask<TValue>(default(TValue)!);
                case "localStorage.getItem":
                    var value = Store.TryGetValue((string)args![0]!, out var v) ? v : null;
                    return new ValueTask<TValue>((TValue)(object?)value!);
                default:
                    return new ValueTask<TValue>(default(TValue)!);
            }
        }
    }

    [Fact]
    public async Task SaveThenLoad_RoundTripsState_ThroughLocalStorage()
    {
        var js = new FakeLocalStorageJsRuntime();
        var persistence = new DataGridPersistence<Person>(new Flare.Infrastructure.BrowserStorage(js),"grid-key");

        var state = new DataGridPersistedState
        {
            Sorts = [new PersistedSort { Key = "Name", Direction = "Descending" }],
            Filters = new Dictionary<string, string> { ["Dept"] = "Eng" },
            ColumnOrder = ["Dept", "Name"],
            HiddenColumns = ["Score"],
            Page = 2,
            PageSize = 25,
        };

        await persistence.SaveAsync(state);

        // Proves the fix: state is actually written to localStorage under the key
        // (the old module-export code never reached real localStorage).
        Assert.True(js.Store.ContainsKey("grid-key"));

        var loaded = await persistence.LoadAsync();

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Page);
        Assert.Equal(25, loaded.PageSize);
        var sort = Assert.Single(loaded.Sorts!);
        Assert.Equal("Name", sort.Key);
        Assert.Equal("Descending", sort.Direction);
        Assert.Equal("Eng", loaded.Filters!["Dept"]);
        Assert.Equal(["Dept", "Name"], loaded.ColumnOrder);
        Assert.Equal(["Score"], loaded.HiddenColumns);
    }

    [Fact]
    public async Task Load_ReturnsNull_WhenNothingStored()
    {
        var js = new FakeLocalStorageJsRuntime();
        var persistence = new DataGridPersistence<Person>(new Flare.Infrastructure.BrowserStorage(js),"absent");

        Assert.Null(await persistence.LoadAsync());
    }

    [Fact]
    public async Task Clear_RemovesStoredState()
    {
        var js = new FakeLocalStorageJsRuntime();
        var persistence = new DataGridPersistence<Person>(new Flare.Infrastructure.BrowserStorage(js),"grid-key");
        await persistence.SaveAsync(new DataGridPersistedState { PageSize = 10 });
        Assert.True(js.Store.ContainsKey("grid-key"));

        await persistence.ClearAsync();

        Assert.False(js.Store.ContainsKey("grid-key"));
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid persistence wiring - a PersistStateKey grid must save the user's
// FIRST change even when storage starts empty. Regression guard: _persistenceLoaded
// was only set when prior saved state existed, so a brand-new grid silently dropped
// every change until something had already been stored.
// ------------------------------------------------------------------------------

public class C_DataGridPersistenceWiringTests : FlareTestContext
{
    private record Row(string Name, int Score);

    private static readonly Row[] _rows =
    [
        new("Bob", 2),
        new("Alice", 1),
        new("Carol", 3),
    ];

    private static RenderFragment Columns() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();
    };

    [Fact]
    public void FreshGrid_PersistsFirstUserChange_EvenWithEmptyStorage()
    {
        // Loose JS interop returns null for localStorage.getItem -> storage starts empty.
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.PersistStateKey, "wiring-key")
            .Add(x => x.Columns, Columns()));

        // Sorting is a user change and must be persisted even with nothing previously stored.
        cut.FindAll("thead th").First(th => th.TextContent.Contains("Name")).Click();

        cut.WaitForAssertion(() => Assert.Contains(
            JSInterop.Invocations["localStorage.setItem"],
            i => i.Arguments.Count > 0 && i.Arguments[0] as string == "wiring-key"));
    }

    [Fact]
    public void Restore_AppliesSavedPageSize_NotClobberedByDefault()
    {
        // LoadAsync reads this saved state (page size 8) back from storage on init.
        const string json = "{\"sorts\":[],\"filters\":{},\"columnOrder\":[],\"hiddenColumns\":[],\"page\":0,\"pageSize\":8}";
        JSInterop.Setup<string?>("localStorage.getItem", "restore-key").SetResult(json);

        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows.AsEnumerable())
            .Add(x => x.PageSize, 5)
            .Add(x => x.PersistStateKey, "restore-key")
            .Add(x => x.Columns, Columns()));

        // Regression: OnParametersSet runs after the restore and used to reset _currentPageSize to
        // PageSize (5), discarding the persisted size. The restored 8 must win.
        Assert.Equal(8, cut.Instance.EffectivePageSize);
    }
}
