using Flare.Components;

namespace Flare.Core.Tests;

/// <summary>
/// Tests for DataGrid core pipeline and state management.
/// </summary>
public class DataGridTests
{
    private readonly List<TestPerson> _people = new()
    {
        new() { Name = "Alice", Age = 30, City = "Moscow" },
        new() { Name = "Bob", Age = 25, City = "London" },
        new() { Name = "Charlie", Age = 35, City = "Moscow" },
        new() { Name = "Diana", Age = 28, City = "Paris" },
        new() { Name = "Eve", Age = 32, City = "London" },
    };

    [Fact]
    public void Pipeline_Should_Sort_Ascending()
    {
        var sorts = new List<DataGridSort> { new("Name", SortDirection.Ascending) };
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, sorts, [], null, null, null, null, 0, 10);

        Assert.Equal(5, result.TotalCount);
        Assert.Equal("Alice", result.Items.First().Name);
        Assert.Equal("Eve", result.Items.Last().Name);
    }

    [Fact]
    public void Pipeline_Should_Sort_Descending()
    {
        var sorts = new List<DataGridSort> { new("Name", SortDirection.Descending) };
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, sorts, [], null, null, null, null, 0, 10);

        Assert.Equal("Eve", result.Items.First().Name);
        Assert.Equal("Alice", result.Items.Last().Name);
    }

    [Fact]
    public void Pipeline_Should_Filter_Contains()
    {
        var filters = new List<DataGridFilter> { new("City", FilterOperator.Contains, "Mos") };
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, [], filters, null, null, null, null, 0, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Contains("Mos", p.City));
    }

    [Fact]
    public void Pipeline_Should_Filter_Equals()
    {
        var filters = new List<DataGridFilter> { new("City", FilterOperator.Equals, "London") };
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, [], filters, null, null, null, null, 0, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Equal("London", p.City));
    }

    [Fact]
    public void Pipeline_Should_Page()
    {
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, [], [], null, null, null, null, 0, 2);

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public void Pipeline_Should_Page_SecondPage()
    {
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, [], [], null, null, null, null, 1, 2);

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Charlie", result.Items.First().Name);
    }

    [Fact]
    public void Pipeline_Should_Combined_Sort_Filter_Page()
    {
        var sorts = new List<DataGridSort> { new("Name", SortDirection.Ascending) };
        var filters = new List<DataGridFilter> { new("City", FilterOperator.Equals, "London") };
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, sorts, filters, null, null, null, null, 0, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Bob", result.Items.First().Name);
        Assert.Equal("Eve", result.Items.Last().Name);
    }

    [Fact]
    public void Pipeline_Should_Handle_Empty_Source()
    {
        var result = DataGridPipeline<TestPerson>.Execute(
            [], [], [], null, null, null, null, 0, 10);

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Pipeline_Should_Handle_QuickFilter()
    {
        Func<TestPerson, bool> quickFilter = p => p.Age > 30;
        var result = DataGridPipeline<TestPerson>.Execute(
            _people, [], [], null, quickFilter, null, null, 0, 10);

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Age > 30));
    }

    private sealed class TestPerson
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string City { get; set; } = "";
    }
}
