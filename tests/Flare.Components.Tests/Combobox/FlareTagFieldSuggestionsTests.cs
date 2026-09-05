using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlareTagFieldSuggestionsTests : FlareTestContext
{
    [Fact]
    public void StaticSuggestions_FilterOnInput()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, new[] { "apple", "apricot", "banana" })
            .Add(x => x.MinChars, 1));

        cut.Find($".{Css.Classes.TagInput.Input}").Input("ap");
        var options = cut.FindAll($".{Css.Classes.Listbox.Option}");
        Assert.Equal(2, options.Count);
        Assert.Contains(options, o => o.TextContent.Trim() == "apple");
        Assert.Contains(options, o => o.TextContent.Trim() == "apricot");
    }

    [Fact]
    public void SelectingSuggestion_AddsTag()
    {
        IReadOnlyList<string> tags = [];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Suggestions, new[] { "apple", "apricot" })
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        cut.Find($".{Css.Classes.TagInput.Input}").Input("ap");
        cut.FindAll($".{Css.Classes.Listbox.Option}")[0].Click();
        Assert.Contains("apple", tags);
    }

    [Fact]
    public void AlreadyAddedTag_ExcludedFromSuggestions()
    {
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, new[] { "apple" })
            .Add(x => x.Suggestions, new[] { "apple", "apricot" }));

        cut.Find($".{Css.Classes.TagInput.Input}").Input("ap");
        var options = cut.FindAll($".{Css.Classes.Listbox.Option}");
        Assert.Single(options);
        Assert.Equal("apricot", options[0].TextContent.Trim());
    }
}
