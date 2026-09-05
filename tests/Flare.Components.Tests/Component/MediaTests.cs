namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareHighlighter  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareHighlighterTests : FlareTestContext
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

public class C_FlareMarkdownTests : FlareTestContext
{
    [Fact]
    public void RendersRootFlareMarkdownElement()
    {
        var cut = Render<FlareMarkdown>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Markdown.Root}"));
    }

    [Fact]
    public void ValueNull_RendersEmptyWithoutCrash()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, (string?)null));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Markdown.Root}"));
        Assert.Empty(cut.Find($".{Css.Classes.Markdown.Root}").InnerHtml.Trim());
    }

    [Fact]
    public void ValueH1_RendersH1InOutput()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "# Hello"));

        Assert.NotEmpty(cut.FindAll("h1"));
    }

    [Fact]
    public void ValueBold_RendersStrongInOutput()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "**bold**"));

        Assert.NotEmpty(cut.FindAll("strong"));
    }

    [Fact]
    public void ValueItalic_RendersEmInOutput()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "*italic*"));

        Assert.NotEmpty(cut.FindAll("em"));
    }

    [Fact]
    public void ValueInlineCode_RendersCodeInOutput()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "`code`"));

        Assert.NotEmpty(cut.FindAll("code"));
    }

    [Fact]
    public void ValueUnorderedList_RendersUlAndLi()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "- item"));

        Assert.NotEmpty(cut.FindAll("ul"));
        Assert.NotEmpty(cut.FindAll("li"));
    }

    [Fact]
    public void ValueLink_RendersAnchorWithHref()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "[text](https://example.com)"));

        var anchor = cut.Find("a");
        Assert.NotNull(anchor.GetAttribute("href"));
    }

    [Fact]
    public void ValueMultipleParagraphs_RendersPTags()
    {
        var cut = Render<FlareMarkdown>(p => p
            .Add(x => x.Value, "First paragraph.\n\nSecond paragraph."));

        Assert.True(cut.FindAll("p").Count >= 2);
    }

    [Fact]
    public void SanitizeHtmlTrue_ScriptTagNotRendered()
    {
        var cut = Render<FlareMarkdown>(p => p
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
        var cut = Render<FlareTreeView>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.TreeView.Root}"));
    }

    [Fact]
    public void HasRoleTree()
    {
        var cut = Render<FlareTreeView>();

        Assert.Equal("tree", cut.Find($".{Css.Classes.TreeView.Root}").GetAttribute("role"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTreeView>(p => p
            .AddChildContent("<li id=\"tree-child\">Node</li>"));

        Assert.NotEmpty(cut.FindAll("#tree-child"));
    }

    [Fact]
    public void AriaLabel_AppliedToElement()
    {
        var cut = Render<FlareTreeView>(p => p
            .Add(x => x.AriaLabel, "File tree"));

        Assert.Equal("File tree", cut.Find($".{Css.Classes.TreeView.Root}").GetAttribute("aria-label"));
    }
}
