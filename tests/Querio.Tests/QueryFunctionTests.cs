using System.Text.Json;
using System.Text.Json.Serialization;

namespace Querio.Tests;

/// <summary>
/// Covers functions the consumer declares. Querio assumes no built-ins of its own: a function exists
/// only because the schema says so, which is what keeps the model neutral about what any store
/// provides while still letting a query call into it.
/// </summary>
public sealed class QueryFunctionTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    private static void AssertReports(QuerySpec spec, QueryErrorCode expected)
    {
        var errors = spec.Validate(Schema).Errors;
        Assert.True(
            errors.Any(error => error.Code == expected),
            $"Expected {expected}. Got: " +
            (errors.Count == 0
                ? "no errors."
                : string.Join("; ", errors.Select(error => $"{error.Code} at {error.Path}"))));
    }

    [Fact]
    public void SelectsTheResultOfAValueFunction()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(QueryFunctionCall.OfFields("calcTax", "t", "amount"), "tax")
            .Build();

        Assert.True(spec.Validate(Schema).IsValid);
        Assert.Equal("calcTax", spec.Select[0].Call!.Function);
    }

    [Fact]
    public void AggregatesOverAFunctionResult()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .AggregateCall(QueryAggregate.Sum, QueryFunctionCall.OfFields("calcTax", "t", "amount"), "totalTax")
            .Build();

        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void GroupsAndOrdersByAFunctionResult()
    {
        var upper = QueryFunctionCall.OfFields("upper", "u", "name");
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(upper, "name")
            .CountRows("total")
            .GroupByCall(upper)
            .OrderByCall(upper)
            .Build();

        var result = spec.Validate(Schema);
        Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => $"{e.Code} at {e.Path}")));
    }

    [Fact]
    public void FiltersOnAFunctionResult()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .Select("u", "name")
            .Where(f => f.EqualCall(QueryFunctionCall.OfFields("upper", "u", "name"), "ADMIN"))
            .Build();

        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void NestsOneCallInsideAnother()
    {
        var nested = QueryFunctionCall.Of(
            "calcTax",
            QueryOperand.Function(QueryFunctionCall.OfFields("calcTax", "t", "amount")));

        var spec = QueryBuilder.From(Schema, "transfers", "t").SelectCall(nested, "twice").Build();

        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void DrawsRowsFromATableFunction()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(30, QueryTimeUnit.Day)), "a")
            .Select("a", "name")
            .Build();

        var result = spec.Validate(Schema);
        Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(e => $"{e.Code} at {e.Path}")));
        Assert.Equal(QuerySourceKind.Function, spec.From.Kind);
    }

    [Fact]
    public void JoinsATableFunctionOnExplicitConditions()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .JoinFunction(
                QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(7, QueryTimeUnit.Day)),
                "a",
                [new QueryJoinCondition(new QueryFieldRef("t", "fromUserId"), new QueryFieldRef("a", "id"))])
            .Select("a", "name")
            .Build();

        Assert.True(spec.Validate(Schema).IsValid);
    }

    [Fact]
    public void RejectsAFunctionTheSchemaDoesNotDeclare()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.Of("nope"), "x")
            .Build();

        AssertReports(spec, QueryErrorCode.UnknownFunction);
    }

    [Fact]
    public void RejectsATableFunctionUsedWhereAValueBelongs()
    {
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(1, QueryTimeUnit.Day)), "x")
            .Build();

        AssertReports(spec, QueryErrorCode.FunctionKindMismatch);
    }

    [Fact]
    public void RejectsAValueFunctionUsedWhereRowsBelong()
    {
        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.OfFields("upper", "x", "name"), "u")
            .Build();

        AssertReports(spec, QueryErrorCode.FunctionKindMismatch);
    }

    [Fact]
    public void RejectsTooFewArguments()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(QueryFunctionCall.Of("calcTax"), "tax")
            .Build();

        AssertReports(spec, QueryErrorCode.FunctionArgumentCount);
    }

    [Fact]
    public void RejectsTooManyArguments()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(
                QueryFunctionCall.Of("upper", QueryOperand.Literal("a"), QueryOperand.Literal("b")),
                "x")
            .Build();

        AssertReports(spec, QueryErrorCode.FunctionArgumentCount);
    }

    [Fact]
    public void AcceptsAnOmittedOptionalArgument()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(QueryFunctionCall.OfFields("calcTax", "t", "amount"), "tax")
            .Build();

        Assert.DoesNotContain(spec.Validate(Schema).Errors, e => e.Code == QueryErrorCode.FunctionArgumentCount);
    }

    [Fact]
    public void RejectsAnArgumentThatDoesNotReadAsItsParameterType()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(QueryFunctionCall.Of("calcTax", QueryOperand.Literal("not a number")), "tax")
            .Build();

        AssertReports(spec, QueryErrorCode.FunctionArgumentInvalid);
    }

    [Fact]
    public void RejectsAnArgumentNamingAnUnknownField()
    {
        var spec = QueryBuilder.From(Schema, "transfers", "t")
            .SelectCall(QueryFunctionCall.OfFields("calcTax", "t", "nope"), "tax")
            .Build();

        AssertReports(spec, QueryErrorCode.UnknownField);
    }

    [Fact]
    public void RejectsARelationOntoATableFunction()
    {
        // A function declares no relations, so there is no path the schema could describe.
        var spec = new QuerySpec(new QuerySource("transfers", "t"))
        {
            Joins =
            [
                new QueryJoin(null, "a")
                {
                    Call = QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(1, QueryTimeUnit.Day)),
                    Relation = "transfer_sender",
                },
            ],
        };

        AssertReports(spec, QueryErrorCode.MissingJoinCondition);
    }

    [Fact]
    public void RejectsAParticipantNamingBothAnEntityAndAFunction()
    {
        var spec = new QuerySpec(new QuerySource("users", "u")
        {
            Call = QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(1, QueryTimeUnit.Day)),
        });

        AssertReports(spec, QueryErrorCode.AmbiguousValueSource);
    }

    [Fact]
    public void RoundTripsCallsThroughJson()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(30, QueryTimeUnit.Day)), "a")
            .SelectCall(QueryFunctionCall.OfFields("upper", "a", "name"), "name")
            .Where(f => f.EqualCall(QueryFunctionCall.OfFields("upper", "a", "name"), "ADMIN"))
            .Build();

        var json = JsonSerializer.Serialize(spec, options);
        var restored = JsonSerializer.Deserialize<QuerySpec>(json, options)!;

        Assert.Equal(json, JsonSerializer.Serialize(restored, options));
        Assert.Equal("activeUsers", restored.From.Call!.Function);
        Assert.True(restored.Validate(Schema).IsValid);
    }
}
