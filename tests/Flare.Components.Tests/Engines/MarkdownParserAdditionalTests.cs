namespace Flare.Components.Tests;

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

    // A pipe is the cell delimiter, so a cell needing a literal one escapes it - and in a table that is
    // the only way, since a code span does not protect it. Splitting naively turned one cell listing
    // enum values into several, which pushed every later cell into a column of its own.
    [Fact]
    public void Table_EscapedPipeStaysInsideItsCell()
    {
        var md = "| a | b |\n|---|---|\n| `X.One\\|Two\\|Three` | tail |";
        var html = MarkdownParser.ToHtml(md, true);

        var body = html[html.IndexOf("<tbody>", StringComparison.Ordinal)..];
        Assert.Equal(2, body.Split("<td>").Length - 1);
        Assert.Contains("X.One|Two|Three", html);
        Assert.DoesNotContain("\\|", html);
    }

    [Fact]
    public void Table_RowEndingInAnEscapedPipeKeepsIt()
    {
        var md = "| a |\n|---|\n| ends with \\| |";
        var html = MarkdownParser.ToHtml(md, true);

        var body = html[html.IndexOf("<tbody>", StringComparison.Ordinal)..];
        Assert.Equal(1, body.Split("<td>").Length - 1);
        Assert.Contains("ends with |", html);
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
