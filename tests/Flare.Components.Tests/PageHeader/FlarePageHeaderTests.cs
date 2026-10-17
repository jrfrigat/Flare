using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class FlarePageHeaderTests : FlareTestContext
{
    [Fact]
    public void TheTitleIsAnH1ByDefault()
    {
        var cut = Render<FlarePageHeader>(p => p.Add(x => x.Title, "Projects"));

        var title = cut.Find($".{Css.Classes.PageHeader.Title}");
        Assert.Equal("H1", title.TagName);
        Assert.Equal("Projects", title.TextContent);
        Assert.Equal("HEADER", cut.Find($".{Css.Classes.PageHeader.Root}").TagName);
    }

    [Theory]
    [InlineData(2, "H2")]
    [InlineData(6, "H6")]
    [InlineData(0, "H1")]
    [InlineData(9, "H6")]
    public void HeadingLevelPicksTheHeadingElement(int level, string tag)
    {
        var cut = Render<FlarePageHeader>(p => p.Add(x => x.Title, "Projects").Add(x => x.HeadingLevel, level));

        Assert.Equal(tag, cut.Find($".{Css.Classes.PageHeader.Title}").TagName);
    }

    [Fact]
    public void EyebrowDescriptionAndActionsAreRendered()
    {
        var cut = Render<FlarePageHeader>(p => p
            .Add(x => x.Eyebrow, "Workspace")
            .Add(x => x.Title, "Projects")
            .Add(x => x.Description, "Everything the team is working on.")
            .Add(x => x.ActionsContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button id=\"new\">New</button>"))));

        Assert.Equal("Workspace", cut.Find($".{Css.Classes.PageHeader.Eyebrow}").TextContent);
        Assert.Equal("Everything the team is working on.", cut.Find($".{Css.Classes.PageHeader.Description}").TextContent);
        Assert.NotNull(cut.Find($".{Css.Classes.PageHeader.Actions} #new"));
    }

    [Fact]
    public void ContentSlotsTakeThePlaceOfTheirStrings()
    {
        var cut = Render<FlarePageHeader>(p => p
            .Add(x => x.Eyebrow, "ignored")
            .Add(x => x.EyebrowContent, (RenderFragment)(b => b.AddMarkupContent(0, "<nav id=\"crumbs\">Home</nav>")))
            .Add(x => x.Description, "ignored")
            .Add(x => x.DescriptionContent, (RenderFragment)(b => b.AddMarkupContent(0, "<a id=\"more\">More</a>"))));

        Assert.NotNull(cut.Find($".{Css.Classes.PageHeader.Eyebrow} #crumbs"));
        Assert.NotNull(cut.Find($".{Css.Classes.PageHeader.Description} #more"));
        Assert.DoesNotContain("ignored", cut.Markup);
    }

    [Fact]
    public void UnsetPartsAreLeftOut()
    {
        var cut = Render<FlarePageHeader>(p => p.Add(x => x.Title, "Projects"));

        Assert.Empty(cut.FindAll($".{Css.Classes.PageHeader.Eyebrow}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.PageHeader.Description}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.PageHeader.Actions}"));
    }
}
