using Flare.Components.IDE;

namespace Flare.Components.Tests;

/// <summary>
/// The IDE tab strips bound a bool straight to aria-selected, which renders "" for the active tab and no
/// attribute at all for the others. Every tab has to say "true" or "false".
/// </summary>
public class TabSelectionStateTests : FlareTestContext
{
    public TabSelectionStateTests() => Services.AddFlareIde();

    [Fact]
    public void SheetTabs_AnnounceSelectionAsText()
    {
        var cut = Render<FlareSheetTabs>(p => p.Add(x => x.Sheets, ["Sheet1", "Sheet2"]).Add(x => x.ActiveIndex, 1));

        Assert.Equal(["false", "true"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));
    }

    [Fact]
    public void DocumentTabs_AnnounceSelectionAsText()
    {
        var cut = Render<FlareDocumentTabs>(p => p
            .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Title, "a.md"))
            .AddChildContent<FlareDocumentTab>(t => t.Add(x => x.Title, "b.md")));

        Assert.Equal(["true", "false"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));
    }

    [Fact]
    public void Ribbon_AnnouncesSelectionAsText()
    {
        var cut = Render<FlareRibbon>(p => p
            .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "Home"))
            .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "Insert")));

        Assert.Equal(["true", "false"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));
    }
}
