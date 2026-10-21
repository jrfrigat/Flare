using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// A section is structure: it renders the semantic element, and its built-in heading keeps the theme's look
// while carrying the level the page structure asks for.
public class FlareSectionTests : FlareTestContext
{
    [Fact]
    public void RendersSectionWithRootClass()
    {
        var cut = Render<FlareSection>();

        var section = cut.Find("section");
        Assert.Contains(Css.Classes.Section.Root, section.ClassName);
        Assert.Null(section.GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void Title_RendersHeadingAtTheRequestedLevel_AndNamesTheSection()
    {
        var cut = Render<FlareSection>(p => p
            .Add(x => x.Title, "Installation")
            .Add(x => x.Level, 3)
            .Add(x => x.Id, "installation"));

        var heading = cut.Find("h3");
        Assert.Equal("installation", heading.GetAttribute("id"));
        Assert.Equal("Installation", heading.TextContent);
        Assert.Equal("installation", cut.Find("section").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void TitleWithoutId_StillNamesTheSection()
    {
        var cut = Render<FlareSection>(p => p.Add(x => x.Title, "Usage"));

        var id = cut.Find("h2").GetAttribute("id");
        Assert.False(string.IsNullOrEmpty(id));
        Assert.Equal(id, cut.Find("section").GetAttribute("aria-labelledby"));
    }

    [Theory]
    [InlineData(1, "h2")]
    [InlineData(4, "h4")]
    [InlineData(9, "h6")]
    public void Level_IsClampedToHeadingRange(int level, string expected)
    {
        var cut = Render<FlareSection>(p => p
            .Add(x => x.Title, "Section")
            .Add(x => x.Level, level));

        Assert.NotNull(cut.Find(expected));
    }

    [Fact]
    public void HeadingContent_ReplacesTheBuiltInHeadingAndLeavesTheSectionUnnamed()
    {
        var cut = Render<FlareSection>(p => p
            .Add(x => x.Title, "Ignored")
            .Add(x => x.HeadingContent, (RenderFragment)(b => b.AddMarkupContent(0, "<h2 id=\"own\">Own heading</h2>"))));

        Assert.Equal("Own heading", cut.Find("h2").TextContent);
        Assert.Null(cut.Find("section").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void BodyAndAdditionalAttributes_ReachTheSection()
    {
        var cut = Render<FlareSection>(p => p
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<p id=\"body\">Text</p>")))
            .AddUnmatched("data-test", "value"));

        var section = cut.Find("section");
        Assert.Equal("value", section.GetAttribute("data-test"));
        Assert.NotNull(cut.Find("#body"));
    }
}
