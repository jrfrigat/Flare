using System.Text.Json;
using System.Text.Json.Serialization;

namespace Querio.Tests;

/// <summary>
/// The spec is a data contract that consumers persist, so round-tripping it matters as much as
/// building it. The core references no serializer of its own - staying dependency-free is the point -
/// so these tests double as the proof that plain System.Text.Json handles the shape with nothing but
/// a string-enum converter.
/// </summary>
public sealed class QuerySerializationTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    // Exercises every corner of the shape at once: a join, plain and aggregate selects, each operand
    // kind, nested AND/OR, grouping, having and paging.
    private static QuerySpec Sample() => QueryBuilder.From(TestSchema.Build(), "requests", "r")
        .Join("apiKeys", "a")
        .Select("r", "route")
        .Select("a", "name", "keyName")
        .CountRows("total")
        .Percentile("r", "durationMs", 0.95, "p95")
        .Where(f => f
            .Since("r", "timestamp", 30, QueryTimeUnit.Day)
            .Equal("r", "error", true)
            .In("r", "status", new[] { 500, 503 })
            .CompareField("r", "id", QueryOperator.NotEquals, "a", "id")
            .AnyOf(any => any.Contains("r", "route", "/api").IsNull("a", "name")))
        .GroupBy("r", "route")
        .GroupBy("a", "name")
        .Having(h => h.SelectGreaterThan("total", 10))
        .OrderBySelectDescending("total")
        .Limit(50)
        .Offset(100)
        .Build();

    [Fact]
    public void RoundTripsThroughJsonUnchanged()
    {
        var json = JsonSerializer.Serialize(Sample(), Options);

        var restored = JsonSerializer.Deserialize<QuerySpec>(json, Options);

        Assert.NotNull(restored);
        Assert.Equal(json, JsonSerializer.Serialize(restored, Options));
    }

    [Fact]
    public void WritesEnumsByNameSoAStoredQueryStaysReadable()
    {
        // Numeric enum values would silently change meaning if a member were ever inserted.
        var json = JsonSerializer.Serialize(Sample(), Options);

        Assert.Contains("\"GreaterThanOrEqual\"", json, StringComparison.Ordinal);
        Assert.Contains("\"Count\"", json, StringComparison.Ordinal);
        Assert.Contains("\"Day\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void PreservesARelativeTimeWindowRatherThanFreezingIt()
    {
        var restored = RoundTrip(Sample());

        var operand = restored.Where!.Conditions[0].Value!;
        Assert.Equal(QueryOperandKind.Relative, operand.Kind);
        Assert.Equal(-30, operand.Relative!.Amount);
        Assert.Equal(QueryTimeUnit.Day, operand.Relative.Unit);
    }

    [Fact]
    public void PreservesEveryOperandKind()
    {
        var restored = RoundTrip(Sample());
        var conditions = restored.Where!.Conditions;

        Assert.Equal(QueryOperandKind.Relative, conditions[0].Value!.Kind);
        Assert.Equal(QueryOperandKind.Literal, conditions[1].Value!.Kind);

        Assert.Equal(QueryOperandKind.List, conditions[2].Value!.Kind);
        Assert.Equal(["500", "503"], conditions[2].Value!.Values!);

        Assert.Equal(QueryOperandKind.Field, conditions[3].Value!.Kind);
        Assert.Equal("a", conditions[3].Value!.Field!.Alias);
    }

    [Fact]
    public void PreservesNestedGroupsAndTheirConnector()
    {
        var restored = RoundTrip(Sample());

        var nested = Assert.Single(restored.Where!.Groups);
        Assert.True(nested.Or);
        Assert.Equal(2, nested.Conditions.Count);
    }

    [Fact]
    public void PreservesAggregatesGroupingAndPaging()
    {
        var restored = RoundTrip(Sample());

        Assert.Equal(4, restored.Select.Count);
        Assert.Equal(QueryAggregate.Percentile, restored.Select[3].Aggregate);
        Assert.Equal(0.95, restored.Select[3].Percentile);
        Assert.Equal(2, restored.GroupBy.Count);
        Assert.Equal("total", restored.Having!.Conditions[0].Select);
        Assert.Equal(50, restored.Limit);
        Assert.Equal(100, restored.Offset);
    }

    [Fact]
    public void RestoresAQueryThatStillValidates()
    {
        var schema = TestSchema.Build();
        var restored = RoundTrip(Sample());

        var result = restored.Validate(schema);

        Assert.True(result.IsValid,
            string.Join("; ", result.Errors.Select(error => $"{error.Code} at {error.Path}")));
    }

    private static QuerySpec RoundTrip(QuerySpec spec)
        => JsonSerializer.Deserialize<QuerySpec>(JsonSerializer.Serialize(spec, Options), Options)!;
}
