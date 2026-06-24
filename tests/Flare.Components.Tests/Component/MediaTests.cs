namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareHighlighter  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareHighlighterTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = RenderComponent<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello world"));

        Assert.NotEmpty(cut.FindAll(".flare-highlighter"));
    }

    [Fact]
    public void FullTextRendered()
    {
        var cut = RenderComponent<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello world"));

        Assert.Contains("Hello world", cut.Find(".flare-highlighter").TextContent);
    }

    [Fact]
    public void HighlightsMatch_MarkElementPresent()
    {
        var cut = RenderComponent<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello world")
            .Add(x => x.Highlight, "world"));

        Assert.NotEmpty(cut.FindAll("mark.flare-highlighter__mark"));
    }

    [Fact]
    public void HighlightedText_InsideMark()
    {
        var cut = RenderComponent<FlareHighlighter>(p => p
            .Add(x => x.Text, "The quick brown fox")
            .Add(x => x.Highlight, "quick"));

        Assert.Contains("quick", cut.Find("mark.flare-highlighter__mark").TextContent);
    }

    [Fact]
    public void CaseSensitive_DoesNotHighlightMismatch()
    {
        var cut = RenderComponent<FlareHighlighter>(p => p
            .Add(x => x.Text, "Hello World")
            .Add(x => x.Highlight, "hello")
            .Add(x => x.CaseSensitive, true));

        Assert.Empty(cut.FindAll("mark.flare-highlighter__mark"));
    }
}

// ------------------------------------------------------------------------------
// FlareMarkdown component  (10 tests from Wave8)
// ------------------------------------------------------------------------------

public class C_FlareMarkdownTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareMarkdownElement()
    {
        var cut = RenderComponent<FlareMarkdown>();

        Assert.NotEmpty(cut.FindAll(".flare-markdown"));
    }

    [Fact]
    public void ValueNull_RendersEmptyWithoutCrash()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, (string?)null));

        Assert.NotEmpty(cut.FindAll(".flare-markdown"));
        Assert.Empty(cut.Find(".flare-markdown").InnerHtml.Trim());
    }

    [Fact]
    public void ValueH1_RendersH1InOutput()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "# Hello"));

        Assert.NotEmpty(cut.FindAll("h1"));
    }

    [Fact]
    public void ValueBold_RendersStrongInOutput()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "**bold**"));

        Assert.NotEmpty(cut.FindAll("strong"));
    }

    [Fact]
    public void ValueItalic_RendersEmInOutput()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "*italic*"));

        Assert.NotEmpty(cut.FindAll("em"));
    }

    [Fact]
    public void ValueInlineCode_RendersCodeInOutput()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "`code`"));

        Assert.NotEmpty(cut.FindAll("code"));
    }

    [Fact]
    public void ValueUnorderedList_RendersUlAndLi()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "- item"));

        Assert.NotEmpty(cut.FindAll("ul"));
        Assert.NotEmpty(cut.FindAll("li"));
    }

    [Fact]
    public void ValueLink_RendersAnchorWithHref()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "[text](https://example.com)"));

        var anchor = cut.Find("a");
        Assert.NotNull(anchor.GetAttribute("href"));
    }

    [Fact]
    public void ValueMultipleParagraphs_RendersPTags()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.Value, "First paragraph.\n\nSecond paragraph."));

        Assert.True(cut.FindAll("p").Count >= 2);
    }

    [Fact]
    public void SanitizeHtmlTrue_ScriptTagNotRendered()
    {
        var cut = RenderComponent<FlareMarkdown>(p => p
            .Add(x => x.SanitizeHtml, true)
            .Add(x => x.Value, "<script>alert(1)</script>"));

        Assert.DoesNotContain("<script>", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }
}

// ------------------------------------------------------------------------------
// FlareTreeView  (4 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareTreeViewTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = RenderComponent<FlareTreeView>();

        Assert.NotEmpty(cut.FindAll(".flare-tree-view"));
    }

    [Fact]
    public void HasRoleTree()
    {
        var cut = RenderComponent<FlareTreeView>();

        Assert.Equal("tree", cut.Find(".flare-tree-view").GetAttribute("role"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = RenderComponent<FlareTreeView>(p => p
            .AddChildContent("<li id=\"tree-child\">Node</li>"));

        Assert.NotEmpty(cut.FindAll("#tree-child"));
    }

    [Fact]
    public void AriaLabel_AppliedToElement()
    {
        var cut = RenderComponent<FlareTreeView>(p => p
            .Add(x => x.AriaLabel, "File tree"));

        Assert.Equal("File tree", cut.Find(".flare-tree-view").GetAttribute("aria-label"));
    }
}

// ------------------------------------------------------------------------------
// FlareDataTree extended  (7 tests from Wave9)
// ------------------------------------------------------------------------------

public class C_FlareDataTreeTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareVtreeElement()
    {
        var items = Array.Empty<string>();
        var cut = RenderComponent<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void HasChildren_Param_Exists_RendersWithoutError()
    {
        var items = new[] { "Root" };
        var cut = RenderComponent<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void ChildrenProvider_Param_Exists_RendersWithoutError()
    {
        var items = new[] { "Root" };
        var cut = RenderComponent<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.ChildrenProvider,
                (Func<string, Task<IEnumerable<string>>>)(_ =>
                    Task.FromResult(Enumerable.Empty<string>()))));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void EmptyItems_RendersWithoutError()
    {
        var cut = RenderComponent<FlareDataTree<string>>(p => p
            .Add(x => x.Items, Array.Empty<string>())
            .Add(x => x.KeySelector, (Func<string, object>)(s => s)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }

    [Fact]
    public void HasChildren_False_NodeRendersLeafSpacer()
    {
        var items = new[] { "Leaf" };
        var cut = RenderComponent<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree__leaf-spacer"));
    }

    [Fact]
    public void Items_WithContent_RendersNodes()
    {
        var items = new[] { "Folder A", "Folder B" };
        var cut = RenderComponent<FlareDataTree<string>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<string, object>)(s => s))
            .Add(x => x.HasChildren, (Func<string, bool>)(_ => false)));

        Assert.Equal(2, cut.FindAll(".flare-vtree__node").Count);
    }

    [Fact]
    public void ComponentIsGeneric_AcceptsIntType()
    {
        var items = new[] { 1, 2, 3 };
        var cut = RenderComponent<FlareDataTree<int>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.KeySelector, (Func<int, object>)(i => i)));

        Assert.NotEmpty(cut.FindAll(".flare-vtree"));
    }
}

// ------------------------------------------------------------------------------
// MarkdownParser unit tests  (8 tests from Wave8)
// ------------------------------------------------------------------------------

public class C_MarkdownParserTests
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
