// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// MarkdownParser unit tests  (8 tests)
// Pure C# - no Blazor component needed.
// -----------------------------------------------------------------------------

public class MarkdownParserTests
{
    [Fact]
    public void HeadingH1_RendersH1Tag()
    {
        var html = MarkdownParser.ToHtml("# Hello", true);
        Assert.Contains("<h1>", html);
    }

    [Fact]
    public void Bold_RendersStrongTag()
    {
        var html = MarkdownParser.ToHtml("**bold**", true);
        Assert.Contains("<strong>", html);
    }

    [Fact]
    public void Italic_RendersEmTag()
    {
        var html = MarkdownParser.ToHtml("*italic*", true);
        Assert.Contains("<em>", html);
    }

    [Fact]
    public void UnorderedList_RendersUlAndLi()
    {
        var html = MarkdownParser.ToHtml("- item", true);
        Assert.Contains("<ul>", html);
        Assert.Contains("<li>", html);
    }

    [Fact]
    public void Link_RendersAnchorWithHref()
    {
        var html = MarkdownParser.ToHtml("[link](https://example.com)", true);
        Assert.Contains("<a href", html);
    }

    [Fact]
    public void HorizontalRule_RendersHr()
    {
        var html = MarkdownParser.ToHtml("---", true);
        Assert.Contains("<hr", html);
    }

    [Fact]
    public void Blockquote_RendersBlockquoteTag()
    {
        var html = MarkdownParser.ToHtml("> quote", true);
        Assert.Contains("<blockquote>", html);
    }

    [Fact]
    public void ScriptTag_WithSanitize_DoesNotContainScript()
    {
        var html = MarkdownParser.ToHtml("<script>x</script>", true);
        Assert.DoesNotContain("<script>", html, StringComparison.OrdinalIgnoreCase);
    }
}
