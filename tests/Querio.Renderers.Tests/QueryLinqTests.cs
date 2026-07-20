using System.Linq.Expressions;
using Querio.Linq;
using Querio.Sql;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers running a query as .NET code. This is the target that proves the model is not secretly
/// SQL: there is no query language here to hide an assumption in, so anything the model expresses
/// has to mean something as an expression tree and as an actual answer over objects.
/// </summary>
public sealed class QueryLinqTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    private static readonly Guid KeyAlpha = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid KeyBeta = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid KeyMissing = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid Ann = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid Bob = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private static readonly DateTime Now = new(2026, 7, 20, 12, 0, 0, DateTimeKind.Utc);

    private sealed class RequestRow
    {
        public Guid Id { get; init; }
        public string Route { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
        public int DurationMs { get; init; }
        public bool CacheHit { get; init; }
        public bool Error { get; init; }
        public int Status { get; init; }
        public Guid ApiKeyId { get; init; }
    }

    private sealed class ApiKeyRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
    }

    private sealed class UserRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid? ManagerId { get; init; }
        public string Secret { get; init; } = string.Empty;
    }

    private static readonly RequestRow[] Requests =
    [
        new() { Route = "/a", Timestamp = Now.AddDays(-1), DurationMs = 120, Error = true, Status = 500, ApiKeyId = KeyAlpha },
        new() { Route = "/a", Timestamp = Now.AddDays(-2), DurationMs = 80, Error = true, Status = 500, ApiKeyId = KeyAlpha },
        new() { Route = "/b", Timestamp = Now.AddDays(-3), DurationMs = 300, Error = false, Status = 200, ApiKeyId = KeyBeta },
        new() { Route = "/b", Timestamp = Now.AddDays(-40), DurationMs = 500, Error = true, Status = 500, ApiKeyId = KeyBeta },
        new() { Route = "/b", Timestamp = Now.AddDays(-5), DurationMs = 90, Error = false, Status = 200, ApiKeyId = KeyBeta },
        new() { Route = "/c", Timestamp = Now.AddHours(-1), DurationMs = 50, Error = false, Status = 200, ApiKeyId = KeyMissing },
    ];

    private static readonly ApiKeyRow[] ApiKeys =
    [
        new() { Id = KeyAlpha, Name = "alpha", OwnerId = Ann },
        new() { Id = KeyBeta, Name = "beta", OwnerId = Bob },
    ];

    private static readonly UserRow[] Users =
    [
        new() { Id = Ann, Name = "Ann", ManagerId = null },
        new() { Id = Bob, Name = "Bob", ManagerId = Ann },
    ];

    private static QuerySources Sources() => new QuerySources()
        .Add("requests", Requests)
        .Add("apiKeys", ApiKeys)
        .Add("users", Users);

    private static QueryResult Run(QuerySpec spec, QueryFunctionLibrary? functions = null)
        => QueryExecutor.Execute(spec, Schema, Sources(), functions, Now);

    [Fact]
    public void FiltersObjectsTheWayItWouldFilterAStore()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .Where(f => f.Equal("r", "error", true).GreaterThan("r", "durationMs", 100))
            .Build();

        Assert.Equal(["/a", "/b"], Run(spec).Column<string>("route"));
    }

    [Fact]
    public void BuildsRealOperatorsRatherThanCallsIntoAnInterpreter()
    {
        // The point of this target is that it produces a tree a provider can read. If the conditions
        // came out as calls into a helper of ours, nothing downstream could translate them.
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f.Equal("r", "error", true).GreaterThan("r", "durationMs", 100))
            .Build();

        var predicate = QueryPredicate.For<RequestRow>(spec, Schema);

        Assert.Equal(ExpressionType.AndAlso, predicate.Body.NodeType);
        var both = (BinaryExpression)predicate.Body;
        var error = Assert.IsAssignableFrom<BinaryExpression>(both.Left);
        var duration = Assert.IsAssignableFrom<BinaryExpression>(both.Right);
        Assert.Equal(ExpressionType.Equal, error.NodeType);
        Assert.Equal(ExpressionType.GreaterThan, duration.NodeType);
        Assert.Equal("Error", Assert.IsAssignableFrom<MemberExpression>(error.Left).Member.Name);

        // A fixed value is narrowed while the query is built, so the tree carries no conversion.
        Assert.Equal(typeof(int), Assert.IsAssignableFrom<ConstantExpression>(duration.Right).Type);
    }

    [Fact]
    public void FeedsAnIQueryableTheSameWayEntityFrameworkWouldBeFed()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Where(f => f.Contains("r", "route", "/b"))
            .Build();

        var predicate = QueryPredicate.For<RequestRow>(spec, Schema);
        var matched = Requests.AsQueryable().Where(predicate).ToList();

        Assert.Equal(3, matched.Count);
        Assert.All(matched, request => Assert.Equal("/b", request.Route));
    }

    [Fact]
    public void RefusesToAnswerForAParticipantOneTypeCannotSpeakFor()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k")
            .Where(f => f.Equal("k", "name", "alpha"))
            .Build();

        var error = Assert.Throws<QueryRenderException>(() => QueryPredicate.For<RequestRow>(spec, Schema));
        Assert.Contains("'k'", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GroupsAndCountsLikeTheQueryItWouldOtherwiseBecome()
    {
        var result = Run(Aggregate());

        Assert.Equal(["/b", "/a", "/c"], result.Column<string>("route"));
        Assert.Equal([3L, 2L, 1L], result.Column<long>("total"));
    }

    [Fact]
    public void AnswersTheSameQuestionTheSqlRendererWouldHandToADatabase()
    {
        // One query, two targets: text for a store to run, and an answer computed here. Nothing in
        // the query knows which of the two it was built for.
        var spec = Aggregate();

        var sql = SqlRenderer.Render(spec, Schema, PostgreSqlDialect.Instance);
        var executed = Run(spec);

        Assert.Contains("GROUP BY", sql.Sql, StringComparison.Ordinal);
        Assert.Equal(3, executed.Rows.Count);
        Assert.Equal(Requests.Length, executed.Column<long>("total").Sum());
    }

    [Fact]
    public void KeepsOnlyTheGroupsThatPassTheGroupingFilter()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "route", "route")
            .CountRows("total")
            .GroupBy("r", "route")
            .Having(f => f.SelectGreaterThan("total", 1))
            .OrderBy("r", "route")
            .Build();

        Assert.Equal(["/a", "/b"], Run(spec).Column<string>("route"));
    }

    [Fact]
    public void JoinsThroughARelationTheSchemaDeclares()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k")
            .Select("k", "name", "key")
            .CountRows("total")
            .GroupBy("k", "name")
            .OrderBy("k", "name")
            .Build();

        var result = Run(spec);

        Assert.Equal(["alpha", "beta"], result.Column<string>("key"));
        Assert.Equal([2L, 3L], result.Column<long>("total"));
    }

    [Fact]
    public void KeepsUnmatchedRowsWhenTheJoinIsAnOuterOne()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .LeftJoin("apiKeys", "k")
            .Select("r", "route", "route")
            .Select("k", "name", "key")
            .Build();

        var result = Run(spec);

        Assert.Equal(Requests.Length, result.Rows.Count);
        // The one request whose key is not in the schema's objects keeps its row, with nothing beside it.
        Assert.Single(result.Rows, row => row[result.IndexOf("key")] is null);
    }

    [Fact]
    public void RefusesAJoinItHasNoShapeFor()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Join("apiKeys", "k", kind: QueryJoinKind.Right)
            .Select("r", "route", "route")
            .Build();

        var error = Assert.Throws<QueryRenderException>(() => Run(spec));
        Assert.Equal(QueryFeature.RightJoin, error.Feature);
    }

    [Fact]
    public void ReadsARelativeWindowAgainstTheMomentItWasGiven()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .CountRows("total")
            .Where(f => f.Since("r", "timestamp", 30, QueryTimeUnit.Day))
            .Build();

        // Five of the six requests fall inside the window; the fortnight-and-a-half old one does not.
        Assert.Equal(5L, Run(spec).Column<long>("total")[0]);
    }

    [Fact]
    public void ComputesAPercentileTheWayADatabaseInterpolatesIt()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Percentile("r", "durationMs", 0.95, "p95")
            .Build();

        // Sorted: 50, 80, 90, 120, 300, 500. The 95th percentile lands between the last two.
        Assert.Equal(450d, Run(spec).Column<double>("p95")[0]);
    }

    [Fact]
    public void RunsAFunctionTheCallerSuppliedTheCodeFor()
    {
        var functions = new QueryFunctionLibrary()
            .Register<string, string>("upper", text => text.ToUpperInvariant());

        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "name")
            .OrderBy("u", "name")
            .Build();

        Assert.Equal(["ANN", "BOB"], Run(spec, functions).Column<string>("name"));
    }

    [Fact]
    public void RefusesAFunctionNobodyImplemented()
    {
        // A schema says a function exists; it never says what it does. Guessing would be worse than
        // failing, so an unimplemented call stops the query.
        var spec = QueryBuilder.From(Schema, "users", "u")
            .SelectCall(QueryFunctionCall.OfFields("upper", "u", "name"), "name")
            .Build();

        var error = Assert.Throws<QueryRenderException>(() => Run(spec));
        Assert.Equal(QueryFeature.ValueFunctions, error.Feature);
    }

    [Fact]
    public void DrawsRowsFromATableFunctionAsThoughItWereAnEntity()
    {
        var functions = new QueryFunctionLibrary()
            .RegisterTable("activeUsers", _ => Users.Where(user => user.ManagerId is not null));

        var spec = QueryBuilder
            .FromFunction(Schema, QueryFunctionCall.Of("activeUsers", QueryOperand.Ago(7, QueryTimeUnit.Day)), "a")
            .Select("a", "name", "name")
            .Build();

        Assert.Equal(["Bob"], Run(spec, functions).Column<string>("name"));
    }

    [Fact]
    public void SkipsAndTakesTheWayPagingDoes()
    {
        var spec = QueryBuilder.From(Schema, "requests", "r")
            .Select("r", "durationMs", "duration")
            .OrderBy("r", "durationMs")
            .Offset(2)
            .Limit(2)
            .Build();

        Assert.Equal([90L, 120L], Run(spec).Column<long>("duration"));
    }

    private static QuerySpec Aggregate() => QueryBuilder.From(Schema, "requests", "r")
        .Select("r", "route", "route")
        .CountRows("total")
        .GroupBy("r", "route")
        .OrderBySelectDescending("total")
        .Build();
}
