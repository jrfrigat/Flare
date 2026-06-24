// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// MarkdownParser additional unit tests  (8 tests)
// Pure C# - no Blazor component rendering.
// -----------------------------------------------------------------------------

public class MarkdownParserAdditionalTests
{
    [Fact]
    public void OrderedList_RendersOlAndLi()
    {
        var html = MarkdownParser.ToHtml("1. item", true);
        Assert.Contains("<ol>", html);
        Assert.Contains("<li>", html);
    }

    [Fact]
    public void Table_RendersTableElement()
    {
        var md = "| h1 | h2 |\n|---|---|\n| a | b |";
        var html = MarkdownParser.ToHtml(md, true);
        Assert.Contains("<table>", html);
    }

    [Fact]
    public void Blockquote_RendersBlockquoteTag()
    {
        var html = MarkdownParser.ToHtml("> blockquote", true);
        Assert.Contains("<blockquote>", html);
    }

    [Fact]
    public void Image_RendersImgTag()
    {
        var html = MarkdownParser.ToHtml("![alt](src.png)", true);
        Assert.Contains("<img", html);
    }

    [Fact]
    public void HeadingH2_RendersH2Tag()
    {
        var html = MarkdownParser.ToHtml("## H2", true);
        Assert.Contains("<h2>", html);
    }

    [Fact]
    public void HeadingH3_RendersH3Tag()
    {
        var html = MarkdownParser.ToHtml("### H3", true);
        Assert.Contains("<h3>", html);
    }

    [Fact]
    public void EmptyString_ReturnsEmptyWithoutCrash()
    {
        var html = MarkdownParser.ToHtml("", true);
        Assert.True(string.IsNullOrWhiteSpace(html));
    }

    [Fact]
    public void NullInput_ReturnsEmptyWithoutCrash()
    {
        var html = MarkdownParser.ToHtml(null, true);
        Assert.Equal(string.Empty, html);
    }
}
