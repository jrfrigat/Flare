namespace Flare.Components.Tests;

public class TagFieldSuggestionTests : FlareTestContext
{
    private static readonly string[] Ten = Enumerable.Range(1, 10).Select(i => $"item{i}").ToArray();
    private static string OptionSelector => $".{Css.Classes.Listbox.Option}";

    [Fact]
    public void MatchBeyondTheSuggestionLimit_IsFound()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, Ten)
            .Add(x => x.MaxSuggestions, 8));

        cut.Find("input").Focus();
        cut.Find("input").Input("item9");

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(OptionSelector)));
        Assert.Equal("item9", cut.Find(OptionSelector).TextContent.Trim());
    }

    [Fact]
    public void MaxSuggestions_LimitsTheMatches()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, Ten)
            .Add(x => x.MaxSuggestions, 8));

        cut.Find("input").Focus();
        cut.Find("input").Input("item");

        cut.WaitForAssertion(() => Assert.Equal(8, cut.FindAll(OptionSelector).Count));
    }

    [Fact]
    public void LocalSuggestions_FollowMinChars()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, Ten)
            .Add(x => x.MinChars, 3));

        cut.Find("input").Focus();
        Assert.Empty(cut.FindAll(OptionSelector));

        cut.Find("input").Input("it");
        Assert.Empty(cut.FindAll(OptionSelector));

        cut.Find("input").Input("ite");
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll(OptionSelector)));
    }

    [Fact]
    public void AsyncSuggestions_FollowMinChars()
    {
        var calls = 0;
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.MinChars, 3)
            .Add(x => x.DebounceMs, 0)
            .Add(x => x.SearchFunc, (string _, CancellationToken _) =>
            {
                calls++;
                return Task.FromResult<IEnumerable<string>>(["alpha"]);
            }));

        cut.Find("input").Focus();
        cut.Find("input").Input("al");
        Assert.Empty(cut.FindAll(OptionSelector));

        cut.Find("input").Input("alp");
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(OptionSelector)));
        Assert.Equal(1, calls);
    }

    [Fact]
    public void SelectedTags_AreExcludedFromSuggestions()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, new[] { "alpha", "beta" })
            .Add(x => x.Values, new[] { "alpha" }));

        cut.Find("input").Focus();
        cut.Find("input").Input("a");

        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(OptionSelector)));
        Assert.Equal("beta", cut.Find(OptionSelector).TextContent.Trim());
    }
}
