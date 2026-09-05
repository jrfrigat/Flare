namespace Flare.Components.Tests;

public class FlareHighlighterTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello world"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Highlighter.Root}"));
    }

    [Fact]
    public void FullTextRendered()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello world"));

        Assert.Contains("Hello world", cut.Find($".{Css.Classes.Highlighter.Root}").TextContent);
    }

    [Fact]
    public void HighlightsMatch_MarkElementPresent()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello world")
            .Add(x => x.Highlight, "world"));

        Assert.NotEmpty(cut.FindAll($"mark.{Css.Classes.Highlighter.Mark}"));
    }

    [Fact]
    public void HighlightedText_InsideMark()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "The quick brown fox")
            .Add(x => x.Highlight, "quick"));

        Assert.Contains("quick", cut.Find($"mark.{Css.Classes.Highlighter.Mark}").TextContent);
    }

    [Fact]
    public void CaseSensitive_DoesNotHighlightMismatch()
    {
        var cut = Render<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello World")
            .Add(x => x.Highlight, "hello")
            .Add(x => x.CaseSensitive, true));

        Assert.Empty(cut.FindAll($"mark.{Css.Classes.Highlighter.Mark}"));
    }
}

// ------------------------------------------------------------------------------
// FlareMarkdown component  (10 tests from Wave8)
// ------------------------------------------------------------------------------
