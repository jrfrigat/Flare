using Flare.Components.IDE;
using Flare.Icons;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// A document tab can carry a second, smaller icon between its document icon and the title. Hosts use
/// it for a status the document belongs to, such as the connection of an editor tab.
/// </summary>
public class IdeDocumentTabStatusTests : FlareTestContext
{
    public IdeDocumentTabStatusTests() => Services.AddFlareIde();

    private static RenderFragment OneTab(bool withStatus) => builder =>
    {
        builder.OpenComponent<FlareDocumentTab>(0);
        builder.AddAttribute(1, nameof(FlareDocumentTab.Key), "a");
        builder.AddAttribute(2, nameof(FlareDocumentTab.Title), "A.sql");
        if (withStatus)
        {
            builder.AddAttribute(3, nameof(FlareDocumentTab.StatusIcon), FlareIcons.Link);
        }

        builder.CloseComponent();
    };

    [Fact]
    public void StatusIcon_RendersBetweenTheDocumentIconAndTheTitle()
    {
        var cut = Render<FlareDocumentTabs>(p => p.AddChildContent(OneTab(withStatus: true)));

        var tab = cut.Find("[role=tab]");
        var classes = tab.Children.Select(child => child.ClassName).ToArray();
        var status = Array.FindIndex(
            classes,
            value => value?.Contains(Css.Classes.Ide.DocTabs.Status, StringComparison.Ordinal) == true);
        var label = Array.FindIndex(
            classes,
            value => value?.Contains(Css.Classes.Ide.DocTabs.Label, StringComparison.Ordinal) == true);

        Assert.InRange(status, 1, label - 1);
    }

    [Fact]
    public void WithoutAStatusIcon_TheHeaderKeepsTheDocumentIconOnly()
    {
        var cut = Render<FlareDocumentTabs>(p => p.AddChildContent(OneTab(withStatus: false)));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.DocTabs.Status}"));
    }
}
