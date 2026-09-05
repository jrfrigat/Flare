using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class FlareDataGridColumnPickerTests : FlareTestContext
{
    [Fact]
    public void Default_RendersDatagridElement()
    {
        var cut = Render<FlareDataGrid<string>>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Root}"));
    }

    [Fact]
    public void Items_RendersDataRows()
    {
        var items = new[] { "Alpha", "Beta", "Gamma" };
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, items));

        Assert.Equal(3, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
    }

    [Fact]
    public void PageSize_5_LimitsDisplayedRows()
    {
        var items = Enumerable.Range(1, 20).Select(i => $"Item {i}").ToArray();
        var cut = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.PageSize, 5));

        Assert.Equal(5, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
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

        Assert.NotEmpty(cut.FindAll($"tr.{Css.Classes.DataGrid.GroupHeader}"));
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
        Assert.Equal(5, cut.FindAll($"tr.{Css.Classes.DataGrid.GroupHeader}").Count);
        // All data rows still render under their leaf groups.
        Assert.Equal(4, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.GroupAggregate}"));
    }

    [Fact]
    public void Groups_ClickHeader_CollapsesChildRows()
    {
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.Grouping, GroupingFor<GroupedPerson>(("Role", g => g.Role))));

        Assert.Equal(4, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);

        // Collapse the first group (Eng, 3 rows) -> only the QA group's 1 row remains.
        cut.FindAll($"button.{Css.Classes.DataGrid.GroupToggle}")[0].Click();

        Assert.Single(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}"));
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
        var controls = cut.FindAll($"{scope} .{Css.Classes.Select.Control}");
        (last ? controls[controls.Count - 1] : controls[0]).Click();
        cut.FindAll($"{scope} .{Css.Classes.Select.Option}")
            .First(o => o.TextContent.Trim() == optionText)
            .Click();
    }

    [Fact]
    public void SelectFilter_RendersMultiSelectWithDistinctValues()
    {
        var cut = Render(FilterableGrid());

        // The Select filter reuses FlareMultiSelect.
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.FilterTh} .{Css.Classes.Multiselect.Root}"));
        // Open the dropdown: 2 distinct roles (Eng, QA).
        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Multiselect.Option}").Count);
    }

    [Fact]
    public void SelectFilter_FiltersRowsOnSelection()
    {
        var cut = Render(FilterableGrid());

        Assert.Equal(4, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);

        cut.Find($".{Css.Classes.Multiselect.Control}").Click();
        cut.FindAll($".{Css.Classes.Multiselect.Option}").First(o => o.TextContent.Contains("QA")).Click();

        Assert.Single(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}"));
    }

    [Fact]
    public void FilterBuilder_ToggleButton_OpensPanelWithOneCondition()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));

        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderPanel}"));

        cut.Find($".{Css.Classes.DataGrid.FilterBuilder} > button").Click();

        Assert.Single(cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderPanel}"));
        Assert.Single(cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderRow}"));
    }

    [Fact]
    public void FilterBuilder_ApplyGreaterThan_FiltersRows()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find($".{Css.Classes.DataGrid.FilterBuilder} > button").Click();

        // Configure: Score greater-than 75 -> of {90,80,70,60} only 90 and 80 qualify.
        // Column + operator reuse FlareSelect (custom popover); value reuses FlareField.
        PickSelect(cut, $".{Css.Classes.DataGrid.FilterBuilderField}", "Score");
        PickSelect(cut, $".{Css.Classes.DataGrid.FilterBuilderOp}", FlareStrings.DataGrid_OpGreaterThan);
        cut.Find($".{Css.Classes.DataGrid.FilterBuilderValue} .{Css.Classes.Input.Control}").Input("75");
        // Actions order: Clear, Apply -> Apply is the last button.
        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderActions} button").Last().Click();

        Assert.Equal(2, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
    }

    [Fact]
    public void FilterBuilder_DefaultField_FiltersWithoutReselecting()
    {
        // The first condition defaults its field to the first column (Role); the user only sets
        // the value and applies, without touching the field dropdown.
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find($".{Css.Classes.DataGrid.FilterBuilder} > button").Click();

        cut.Find($".{Css.Classes.DataGrid.FilterBuilderValue} .{Css.Classes.Input.Control}").Input("QA");
        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderActions} button").Last().Click(); // Apply

        Assert.Single(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}"));
    }

    [Fact]
    public void FilterBuilder_Connector_TogglesAndOr()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find($".{Css.Classes.DataGrid.FilterBuilder} > button").Click();

        var connector = cut.Find($".{Css.Classes.DataGrid.FilterBuilderConnector}");
        Assert.Equal(FlareStrings.DataGrid_And, connector.TextContent.Trim());

        connector.Click();
        Assert.Equal(FlareStrings.DataGrid_Or, cut.Find($".{Css.Classes.DataGrid.FilterBuilderConnector}").TextContent.Trim());
    }

    [Fact]
    public void FilterBuilder_AddGroup_AddsNestedGroup()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find($".{Css.Classes.DataGrid.FilterBuilder} > button").Click();

        Assert.Single(cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderGroup}"));

        // Group head buttons: [0] connector, [1] add condition, [2] add group.
        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderGroupHead} button")[2].Click();

        Assert.Equal(2, cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderGroup}").Count);
    }

    [Fact]
    public void FilterBuilder_NestedOrGroup_FiltersRows()
    {
        var cut = Render(FilterableGrid(showFilterBuilder: true));
        cut.Find($".{Css.Classes.DataGrid.FilterBuilder} > button").Click();

        // Root condition: Score >= 90 (matches the single 90 row).
        PickSelect(cut, $".{Css.Classes.DataGrid.FilterBuilderField}", "Score");
        PickSelect(cut, $".{Css.Classes.DataGrid.FilterBuilderOp}", FlareStrings.DataGrid_OpGreaterOrEqual);
        cut.Find($".{Css.Classes.DataGrid.FilterBuilderValue} .{Css.Classes.Input.Control}").Input("90");

        // Add a nested group, set it to OR, add a condition Score <= 60 (matches the single 60 row).
        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderGroupHead} button")[2].Click(); // add group
        // The nested group is the 2nd group; toggle its connector to Or.
        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderConnector}")[1].Click();
        // Configure the nested condition (last field/op/value belong to the new group's seeded row).
        PickSelect(cut, $".{Css.Classes.DataGrid.FilterBuilderField}", "Score", last: true);
        PickSelect(cut, $".{Css.Classes.DataGrid.FilterBuilderOp}", FlareStrings.DataGrid_OpLessOrEqual, last: true);
        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderValue} .{Css.Classes.Input.Control}").Last().Input("60");

        cut.FindAll($".{Css.Classes.DataGrid.FilterBuilderActions} button").Last().Click(); // Apply

        // Root is AND of [Score>=90] and [OR group: Score<=60]. No row is both >=90 and <=60.
        Assert.Empty(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}"));
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

        Assert.NotEmpty(cut.FindAll($"tr.{Css.Classes.DataGrid.PlaceholderRow}"));
        // Skeleton mode shows no spinner/text overlay.
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.Loading}"));
    }

    [Fact]
    public void Loading_SpinnerMode_ShowsRingOverlay()
    {
        var tcs = new TaskCompletionSource<DataGridResult<GroupedPerson>>();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, _ => tcs.Task)
            .Add(x => x.LoadingIndicator, DataGridLoadingIndicator.Spinner)
            .Add(x => x.Columns, GroupedCols));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Loading}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Circular}"));
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

        var root = cut.Find($".{Css.Classes.DataGrid.Root}");
        Assert.Contains(Css.Classes.DataGrid.Striped, root.ClassName);
        Assert.Contains(Css.Classes.DataGrid.Hoverable, root.ClassName);
        Assert.Contains(Css.Classes.DataGrid.Dense, root.ClassName);
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.ResizeHandle}"));
    }

    [Fact]
    public void EditableColumn_AutoEnablesEditing_AndBuffersTypedValue()
    {
        var people = new[] { new GroupedPerson("Eng", "Berlin", 90) };
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, people.AsEnumerable())
            .Add(x => x.Columns, EditableCols));

        // An Editable column alone surfaces the edit (pencil) action - no InlineEdit needed.
        cut.Find($".{Css.Classes.DataGrid.TdEditActions} button").Click();

        // The editable cell now renders a FlareField; typing updates the edit buffer.
        var input = cut.Find($".{Css.Classes.Input.Control}");
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
        Assert.Equal(10, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);

        // Simulate the bottom sentinel becoming visible -> next page appends.
        await cut.InvokeAsync(() => cut.Instance.TriggerLoad());
        Assert.Equal(20, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
    }

    // Infinite scroll uses PageSize as its CHUNK size, which is a different question from "how many
    // rows are on a page" - and the two stopped agreeing when PageSize started defaulting to 0, meaning
    // "no paging". A chunk of zero asks the provider for nothing and never reaches the short page that
    // ends the accumulation, so the grid loads forever and shows nothing.
    [Fact]
    public async Task InfiniteScroll_LoadsWithoutAnExplicitPageSize()
    {
        var data = Enumerable.Range(1, 500).Select(i => new GroupedPerson($"R{i}", "C", i)).ToList();
        var asked = new List<int>();
        Task<DataGridResult<GroupedPerson>> Provider(DataGridRequest r)
        {
            asked.Add(r.PageSize);
            return Task.FromResult(new DataGridResult<GroupedPerson>(
                data.Skip(r.Page * r.PageSize).Take(r.PageSize), data.Count));
        }

        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, Provider)
            .Add(x => x.InfiniteScroll, true)
            .Add(x => x.Virtual, false)
            .Add(x => x.Columns, GroupedCols));

        Assert.All(asked, size => Assert.True(size > 0, "A chunk of zero rows asks the provider for nothing."));
        var first = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count;
        Assert.True(first > 0, "The first chunk has to arrive without the page telling the grid how big it is.");

        await cut.InvokeAsync(() => cut.Instance.TriggerLoad());
        Assert.True(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count > first,
            "Reaching the bottom appends the next chunk.");
    }

    [Fact]
    public void Loading_ProgressLineMode_ShowsThinLineWithoutOverlay()
    {
        var tcs = new TaskCompletionSource<DataGridResult<GroupedPerson>>();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.ItemsProvider, _ => tcs.Task)
            .Add(x => x.LoadingIndicator, DataGridLoadingIndicator.ProgressLine)
            .Add(x => x.Columns, GroupedCols));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.ProgressLine}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Linear}"));
        // ProgressLine keeps the table visible: no spinner/text overlay, no dim class.
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.Loading}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.DataGrid.TableLoading}"));
    }

    [Fact]
    public void RowKey_UsedAsStableRowIdentity()
    {
        // Two distinct items that compare equal by reference would collide; RowKey disambiguates.
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, _grouped.AsEnumerable())
            .Add(x => x.RowKey, g => g.Score)
            .Add(x => x.Columns, GroupedCols));

        Assert.Equal(4, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
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

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Root}"));
    }

    // The virtual grid must be handed the row set it is RESPONSIBLE for, whole. It renders its own
    // window from a scroll position, so being handed less than that set caps it silently - which is the
    // 0.26.3 bug: the client path fed Virtualize a paged list while paging was switched off, so the grid
    // stopped at ten rows with no pager and no way to reach row eleven.
    //
    // Which set that is depends on the page size, and both answers are asserted here: with no paging the
    // set is everything, and with a page size it is the page - reachable past its end through the pager,
    // which is the part the bug did not have.
    // bUnit has no JS measurement, so Virtualize here renders every item it is given: that is exactly
    // what makes the count a usable assertion about the SOURCE.
    [Theory]
    [InlineData(null, 100)]
    [InlineData(0, 100)]
    [InlineData(25, 25)]
    public void Virtual_IsHandedTheWholeSetItRenders(int? pageSize, int expectedRows)
    {
        var data = Enumerable.Range(1, 100).Select(i => new GroupedPerson($"R{i}", "C", i)).ToList();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p =>
        {
            p.Add(x => x.Items, data.AsEnumerable())
             .Add(x => x.Virtual, true)
             .Add(x => x.Height, "300px")
             .Add(x => x.Columns, GroupedCols);
            if (pageSize is not null) p.Add(x => x.PageSize, pageSize.Value);
        });

        Assert.Equal(expectedRows, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
        // Every row past the window is reachable: by scrolling when there is one page, by the pager when
        // there are several.
        if (expectedRows < 100)
            Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.Pagination}"));
    }

    private sealed record Reading(int? Value);
    private sealed record Sample(Reading? Latest);

    // An Auto-typed column infers its type by RUNNING the caller's Field lambda, and it scans Items -
    // the whole set - while the grid renders only the current page. So a selector that is safe for
    // every row on screen can still throw on one the user cannot even see, and that used to take out
    // the whole render batch with no hint of which column caused it.
    //
    // `s => s.Latest!.Value` against an optional parent is the ordinary way to write this binding, and
    // the null-forgiving operator compiles happily. Row 0 yields null (a present parent with no value),
    // which is what makes the sampler walk on to row 1, where the parent is missing and the selector
    // throws. PageSize 1 keeps row 1 off the page, so nothing renders it.
    [Fact]
    public void AutoColumnType_SurvivesASelectorThatThrowsOnARowOffThePage()
    {
        var data = new List<Sample> { new(new Reading(null)), new(null), new(new Reading(5)) };
        var cut = Render<FlareDataGrid<Sample>>(p => p
            .Add(x => x.Items, data.AsEnumerable())
            .Add(x => x.PageSize, 1)
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<Sample>>(0);
                inner.AddAttribute(1, "Title", "Latest");
                inner.AddAttribute(2, "Field", (Func<Sample, object?>)(s => s.Latest!.Value));
                inner.CloseComponent();
            })));

        Assert.Single(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}"));
    }

    // Sorting a virtual grid must reorder the whole set and keep showing all of it. With a paged source
    // the grid sorted the hundred rows correctly and then displayed the first ten of the result, so the
    // ordering looked right and the data was still missing.
    [Fact]
    public void Virtual_SortsAcrossTheWholeSet()
    {
        var data = Enumerable.Range(1, 100).Select(i => new GroupedPerson($"R{i:D3}", "C", i)).ToList();
        var cut = Render<FlareDataGrid<GroupedPerson>>(p => p
            .Add(x => x.Items, data.AsEnumerable())
            .Add(x => x.Virtual, true)
            .Add(x => x.Height, "300px")
            .Add(x => x.Columns, (RenderFragment)(inner =>
            {
                inner.OpenComponent<FlareColumn<GroupedPerson>>(0);
                inner.AddAttribute(1, "Title", "Role");
                inner.AddAttribute(2, "Field", (Func<GroupedPerson, object?>)(p => p.Role));
                inner.AddAttribute(3, "Sortable", true);
                inner.CloseComponent();
            })));

        cut.Find($"th.{Css.Classes.DataGrid.Th}").Click();   // ascending
        cut.Find($"th.{Css.Classes.DataGrid.Th}").Click();   // descending

        var rows = cut.FindAll($"tr.{Css.Classes.DataGrid.Row}");
        Assert.Equal(100, rows.Count);
        Assert.Contains("R100", rows[0].TextContent);      // the last item is now first
    }
}

// ------------------------------------------------------------------------------
// FlareDataGrid header rendering / column declaration order
// ------------------------------------------------------------------------------
