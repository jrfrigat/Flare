using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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

    [Fact]
    public void Ribbon_StartsOnTheFirstEnabledTab()
    {
        var cut = Render<FlareRibbon>(p => p
            .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "Home").Add(x => x.Disabled, true))
            .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "Insert")));

        Assert.Equal(["false", "true"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));
    }

    [Fact]
    public void Ribbon_MovesOffAnActiveTabThatBecomesDisabled()
    {
        var cut = Render<FlareRibbon>(p => p.Add(x => x.ChildContent, RibbonTabs(homeDisabled: false, insertDisabled: true)));
        Assert.Equal(["true", "false"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));

        cut.Render(p => p.Add(x => x.ChildContent, RibbonTabs(homeDisabled: true, insertDisabled: false)));

        Assert.Equal(["false", "true"], cut.FindAll("[role=tab]").Select(t => t.GetAttribute("aria-selected")));
    }

    private static RenderFragment RibbonTabs(bool homeDisabled, bool insertDisabled) => builder =>
    {
        builder.OpenComponent<FlareRibbonTab>(0);
        builder.AddAttribute(1, nameof(FlareRibbonTab.Title), "Home");
        builder.AddAttribute(2, nameof(FlareRibbonTab.Disabled), homeDisabled);
        builder.CloseComponent();
        builder.OpenComponent<FlareRibbonTab>(3);
        builder.AddAttribute(4, nameof(FlareRibbonTab.Title), "Insert");
        builder.AddAttribute(5, nameof(FlareRibbonTab.Disabled), insertDisabled);
        builder.CloseComponent();
    };
}
