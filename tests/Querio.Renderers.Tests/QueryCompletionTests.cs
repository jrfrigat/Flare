using Querio.Language;
using Querio.Tests;

namespace Querio.Renderers.Tests;

/// <summary>
/// Covers what can be written at a caret. Nearly every case here is text that does not parse,
/// because that is where a caret actually sits: completion is only useful while the query is still
/// being typed.
/// </summary>
public sealed class QueryCompletionTests
{
    private static readonly QuerySchema Schema = TestSchema.Build();

    /// <summary>Suggests at the caret marked with a pipe, which is not part of the text.</summary>
    private static IReadOnlyList<QueryCompletion> At(string textWithCaret)
    {
        var caret = textWithCaret.IndexOf('|');
        var text = textWithCaret.Replace("|", string.Empty);
        return QueryCompletionEngine.Suggest(text, caret < 0 ? text.Length : caret, Schema);
    }

    private static IReadOnlyList<string> Labels(string textWithCaret)
        => At(textWithCaret).Select(candidate => candidate.Label).ToList();

    [Fact]
    public void OffersTheFieldsOfWhatTheQueryDrawsFrom()
    {
        var labels = Labels("select [r].| from [requests] as [r]");

        Assert.Contains("route", labels);
        Assert.Contains("durationMs", labels);
        Assert.DoesNotContain("name", labels);
    }

    [Fact]
    public void OffersAForeignKeyAsSomethingToTravel()
    {
        var suggestions = At("select [r].| from [requests] as [r]");

        var key = Assert.Single(suggestions, candidate => candidate.Kind == QueryCompletionKind.Navigation);
        Assert.Equal("apiKeyId", key.Label);
        Assert.Equal("-> API keys", key.Detail);
    }

    [Fact]
    public void OffersWhatIsOnTheOtherSideOfATravelledKey()
    {
        // The caret is two hops in, and the text is nowhere near being a query yet.
        var labels = Labels("select [r].[apiKeyId].| from [requests] as [r]");

        Assert.Contains("name", labels);
        Assert.Contains("ownerId", labels);
        Assert.DoesNotContain("route", labels);
    }

    [Fact]
    public void KeepsOfferingAlongAChainOfAnyLength()
    {
        var labels = Labels("select [r].[apiKeyId].[ownerId].| from [requests] as [r]");

        Assert.Contains("managerId", labels);
        Assert.Contains("secret", labels);
    }

    [Fact]
    public void OffersACompositeKeyByItsRelationBecauseNoSingleFieldNamesIt()
    {
        var suggestions = At("select [o].| from [orders] as [o]");

        var key = Assert.Single(suggestions, candidate => candidate.Kind == QueryCompletionKind.Navigation);
        Assert.Equal("order_lines", key.Label);
    }

    [Fact]
    public void NarrowsToWhatHasBeenTypedAndSaysWhatToReplace()
    {
        var suggestions = At("select [r].[dur|] from [requests] as [r]");

        var match = Assert.Single(suggestions);
        Assert.Equal("durationMs", match.Label);
        // The whole bracketed word is replaced, brackets included, rather than appended to.
        Assert.Equal(11, match.ReplaceStart);
        Assert.Equal(5, match.ReplaceLength);
        Assert.Equal("[durationMs]", match.Text);
    }

    [Fact]
    public void PutsWhatTheWordStartsWithAboveWhatMerelyContainsIt()
    {
        // "na" starts "name" and sits inside "managerId"; the one it starts has to come first.
        var labels = Labels("select [u].[na|] from [users] as [u]");

        Assert.Equal("name", labels[0]);
        Assert.Contains("managerId", labels);
    }

    [Fact]
    public void OffersEntitiesWhereTheQueryDrawsFrom()
    {
        var suggestions = At("select [r].[route] from |");

        Assert.Contains(suggestions, candidate => candidate.Label == "requests" && candidate.Kind == QueryCompletionKind.Entity);
        Assert.Contains(suggestions, candidate => candidate.Label == "activeUsers" && candidate.Kind == QueryCompletionKind.Function);
    }

    [Fact]
    public void OffersRelationsWhereAJoinTravelsOne()
    {
        var suggestions = At("select [r].[route] from [requests] as [r] join |");

        Assert.Contains(suggestions, candidate => candidate.Kind == QueryCompletionKind.Relation);
        Assert.Contains(suggestions, candidate => candidate.Label == "request_apiKey");
    }

    [Fact]
    public void OffersEveryAliasOnceMoreThanOneSourceIsInPlay()
    {
        var suggestions = At("select | from [requests] as [r] join [apiKeys] as [k] through [request_apiKey]");
        var aliases = suggestions.Where(candidate => candidate.Kind == QueryCompletionKind.Alias).Select(c => c.Label);

        Assert.Equal(["k", "r"], aliases.OrderBy(alias => alias));
        // A bare field would be ambiguous with two sources, so none is offered unqualified.
        Assert.DoesNotContain(suggestions, candidate => candidate.Kind == QueryCompletionKind.Field);
    }

    [Fact]
    public void OffersBareFieldsWhileOnlyOneSourceCouldHaveMeantThem()
    {
        var suggestions = At("select | from [requests] as [r]");

        Assert.Contains(suggestions, candidate => candidate.Label == "route" && candidate.Kind == QueryCompletionKind.Field);
    }

    [Fact]
    public void OffersAggregatesAndDeclaredFunctionsWhereAValueGoes()
    {
        var suggestions = At("select | from [requests] as [r]");

        Assert.Contains(suggestions, candidate => candidate.Label == "count" && candidate.Kind == QueryCompletionKind.Aggregate);
        Assert.Contains(suggestions, candidate => candidate.Label == "trunc");
        Assert.Contains(suggestions, candidate => candidate.Label == "upper" && candidate.Kind == QueryCompletionKind.Function);
    }

    [Fact]
    public void OffersOutputNamesWhereOnlyTheyMakeSense()
    {
        const string query = "select [r].[route], count(*) as [total] from [requests] as [r] group by [r].[route] ";

        var having = At(query + "having |");
        var select = At("select | from [requests] as [r]");

        Assert.Contains(having, candidate => candidate.Label == "total" && candidate.Kind == QueryCompletionKind.Output);
        Assert.DoesNotContain(select, candidate => candidate.Kind == QueryCompletionKind.Output);
    }

    [Fact]
    public void OffersTheClausesThatCouldComeNext()
    {
        var labels = Labels("select [r].[route] from [requests] as [r] |");

        Assert.Contains("where", labels);
        Assert.Contains("order by", labels);
        Assert.Contains("limit", labels);
    }

    [Fact]
    public void AnswersEvenWhenTheTextIsBrokenSomewhereElse()
    {
        // The first item is nonsense, and the caret is on the second. Completion still works because
        // the reader hands back the half of the query it could make.
        var labels = Labels("select [r].[nope], [r].| from [requests] as [r]");

        Assert.Contains("route", labels);
    }
}
