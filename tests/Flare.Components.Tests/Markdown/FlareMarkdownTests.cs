namespace Flare.Components.Tests;

public class FlareMarkdownTests : FlareTestContext
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
