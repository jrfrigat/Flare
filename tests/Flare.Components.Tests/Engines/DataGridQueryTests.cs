using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

public class DataGridQueryTests
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
