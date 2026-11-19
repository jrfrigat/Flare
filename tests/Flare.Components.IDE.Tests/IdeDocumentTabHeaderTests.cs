using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

public class IdeDocumentTabHeaderTests : FlareTestContext
{
    public IdeDocumentTabHeaderTests() => Services.AddFlareIde();

    private static RenderFragment OneTab(RenderFragment? header = null) => builder =>
    {
        builder.OpenComponent<FlareDocumentTab>(0);
        builder.SetKey("a");
        builder.AddComponentParameter(1, nameof(FlareDocumentTab.Key), "a");
        builder.AddComponentParameter(2, nameof(FlareDocumentTab.Title), "A.sql");
        builder.AddComponentParameter(3, nameof(FlareDocumentTab.HeaderContent), header);
        builder.CloseComponent();
    };

    private static RenderFragment Header(string text) => builder =>
    {
        builder.OpenElement(0, "strong");
        builder.AddContent(1, text);
        builder.CloseElement();
    };

    [Fact]
    public void CustomHeader_ReplacesVisibleTitleAndKeepsTheIconAndAccessibleName()
    {
        var cut = Render<FlareDocumentTabs>(p => p.AddChildContent(OneTab(Header("Primary database"))));

        var tab = cut.Find("[role=tab]");
        Assert.Equal("A.sql", tab.GetAttribute("aria-label"));
        Assert.NotNull(tab.QuerySelector($".{Css.Classes.Ide.DocTabs.Icon}"));
        var label = cut.Find($".{Css.Classes.Ide.DocTabs.Label}");
        Assert.Equal("Primary database", label.QuerySelector("strong")?.TextContent);
        Assert.Single(tab.QuerySelectorAll($".{Css.Classes.Ide.DocTabs.Close}"));
    }

    [Fact]
    public void WithoutCustomHeader_TabUsesTheTitle()
    {
        var cut = Render<FlareDocumentTabs>(p => p.AddChildContent(OneTab()));

        Assert.Equal("A.sql", cut.Find($".{Css.Classes.Ide.DocTabs.Label}").TextContent.Trim());
    }

    [Fact]
    public void CustomHeader_UpdatesOnTheSameTabAndCanReturnToTheTitle()
    {
        var cut = Render<FlareDocumentTabs>(p => p.AddChildContent(OneTab(Header("First header"))));
        var tabId = cut.Find("[role=tab]").Id;

        cut.Render(p => p.AddChildContent(OneTab(Header("Updated header"))));

        Assert.Equal("Updated header", cut.Find("[role=tab] strong").TextContent);
        Assert.Equal(tabId, cut.Find("[role=tab]").Id);

        cut.Render(p => p.AddChildContent(OneTab()));

        Assert.Empty(cut.FindAll("[role=tab] strong"));
        Assert.Equal("A.sql", cut.Find($".{Css.Classes.Ide.DocTabs.Label}").TextContent.Trim());
        Assert.Equal(tabId, cut.Find("[role=tab]").Id);
    }
}