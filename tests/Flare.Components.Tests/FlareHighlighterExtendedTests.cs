// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareHighlighter tests  (10 tests)
// Note: FlareCodeEditor does not exist in the codebase; the actual component
// is FlareHighlighter at src/Flare.Components/Highlighter/FlareHighlighter.razor.
// Tests are written against the real component and its actual parameters.
// -----------------------------------------------------------------------------

public class FlareHighlighterExtendedTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareHighlighterElement()
    {
        var cut = Render<FlareHighlighter>();

        Assert.NotEmpty(cut.FindAll(".flare-highlighter"));
    }

    [Fact]
    public void Text_WithoutHighlight_RendersFullText()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "hello world"));

        Assert.Contains("hello world", cut.Find(".flare-highlighter").TextContent);
    }

    [Fact]
    public void Text_MatchingHighlight_RendersMarkElement()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "hello world")
            .Add(x => x.Highlight, "world"));

        Assert.NotEmpty(cut.FindAll("mark"));
    }

    [Fact]
    public void Highlight_MatchedText_HasFlareHighlighterMarkClass()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Find the keyword here")
            .Add(x => x.Highlight, "keyword"));

        Assert.Contains("flare-highlighter__mark", cut.Find("mark").ClassName ?? "");
    }

    [Fact]
    public void CaseSensitive_False_Default_MatchesRegardlessOfCase()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello World")
            .Add(x => x.Highlight, "hello")
            .Add(x => x.CaseSensitive, false));

        Assert.NotEmpty(cut.FindAll("mark"));
    }

    [Fact]
    public void CaseSensitive_True_DoesNotMatchWhenCaseDiffers()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello World")
            .Add(x => x.Highlight, "hello")
            .Add(x => x.CaseSensitive, true));

        Assert.Empty(cut.FindAll("mark"));
    }

    [Fact]
    public void TextNull_RendersWithoutCrash()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, (string?)null));

        Assert.NotEmpty(cut.FindAll(".flare-highlighter"));
    }

    [Fact]
    public void HighlightNull_RendersFullTextWithoutMarkElements()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "some text")
            .Add(x => x.Highlight, (string?)null));

        Assert.Empty(cut.FindAll("mark"));
        Assert.Contains("some text", cut.Find(".flare-highlighter").TextContent);
    }

    [Fact]
    public void MultipleOccurrences_AllMatches_RenderedAsMarks()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "cat and cat")
            .Add(x => x.Highlight, "cat"));

        Assert.Equal(2, cut.FindAll("mark").Count);
    }

    [Fact]
    public void AdditionalAttributes_PassThroughToRootSpan()
    {
        var cut = Render<FlareHighlighter>(p => p
            .AddUnmatched("data-testid", "highlighter-root"));

        Assert.Equal("highlighter-root", cut.Find(".flare-highlighter").GetAttribute("data-testid"));
    }
}
