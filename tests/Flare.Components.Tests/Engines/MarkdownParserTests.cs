namespace Flare.Components.Tests;

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

    // Emphasis used to encode its body flat instead of parsing it, so anything nested inside came out
    // as literal markers. Code inside bold is the case that matters: it is how technical prose - the
    // project's own CHANGELOG included - is written, and it was rendering raw backticks on /changelog.
    [Fact]
    public void InlineCode_InsideBold_IsParsed()
    {
        var html = MarkdownParser.ToHtml("**`FlareSelect` was shorter**", true);
        Assert.Contains("<strong><code>FlareSelect</code> was shorter</strong>", html);
        Assert.DoesNotContain("`", html);
    }

    [Fact]
    public void InlineCode_InsideItalicAndBoldItalic_IsParsed()
    {
        Assert.Contains("<em><code>x</code></em>", MarkdownParser.ToHtml("*`x`*", true));
        Assert.Contains("<strong><em><code>x</code></em></strong>", MarkdownParser.ToHtml("***`x`***", true));
    }

    [Fact]
    public void Italic_InsideBold_IsParsed()
    {
        var html = MarkdownParser.ToHtml("**bold with *stress* inside**", true);
        Assert.Contains("<strong>bold with <em>stress</em> inside</strong>", html);
    }

    // A code span binds tighter than emphasis, so markers inside it stay literal.
    [Fact]
    public void EmphasisMarkers_InsideCode_StayLiteral()
    {
        var html = MarkdownParser.ToHtml("`a ** b`", true);
        Assert.Contains("<code>a ** b</code>", html);
        Assert.DoesNotContain("<strong>", html);
    }

    // Nesting must not become a hole in the encoder: the recursion still routes literal text through
    // HtmlEncode, so markup inside an emphasis body is escaped exactly as it is outside one.
    [Fact]
    public void MarkupInsideBold_IsStillEncoded()
    {
        var html = MarkdownParser.ToHtml("**<img src=x onerror=alert(1)>**", true);
        Assert.DoesNotContain("<img", html);
        Assert.Contains("&lt;img", html);
    }
}
