using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// Relevance scoring used by the Fuzzy search option on Autocomplete / MultiSelect.
public class FlareSearchTests
{
    [Fact]
    public void Exact_BeatsPrefix()
        => Assert.True(FlareSearch.Score("lo", "lo") > FlareSearch.Score("london", "lo"));

    [Fact]
    public void Prefix_BeatsContains()
        => Assert.True(FlareSearch.Score("london", "lon") > FlareSearch.Score("clone", "lon"));

    [Fact]
    public void ShorterPrefix_RanksHigher()
        => Assert.True(FlareSearch.Score("London", "lo") > FlareSearch.Score("Los Angeles", "lo"));

    [Fact]
    public void Subsequence_Matches_ButRanksBelowSubstring()
    {
        Assert.True(FlareSearch.Score("a-b-c", "abc") > 0);               // subsequence (not contiguous)
        Assert.True(FlareSearch.Score("xabcy", "abc") > FlareSearch.Score("a-b-c", "abc")); // substring wins
    }

    [Fact]
    public void NoMatch_IsZero() => Assert.Equal(0, FlareSearch.Score("apple", "xyz"));

    [Fact]
    public void EmptyQuery_MatchesAll() => Assert.Equal(1, FlareSearch.Score("anything", ""));

    [Fact]
    public void Rank_FiltersOutNonMatches_AndOrdersBestFirst()
    {
        var ranked = FlareSearch.Rank(
            new[] { "Los Angeles", "London", "Paris" },
            s => FlareSearch.Score(s, "lo")).ToList();
        Assert.Equal(new[] { "London", "Los Angeles" }, ranked);          // Paris dropped, London first
    }
}
