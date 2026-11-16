using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// A host that draws its own main tabs can suppress or replace the built-in Center strip. In that mode
/// the layout stops guessing a Center document: the host controls the selection, and no selection means
/// the main content is shown.
/// </summary>
public class IdeLayoutCenterStripTests : FlareTestContext
{
    public IdeLayoutCenterStripTests() => Services.AddFlareIde();

    private static Dictionary<IdeDocumentRegion, string?> Selection(string? center) =>
        new() { [IdeDocumentRegion.Center] = center };

    [Fact]
    public void DefaultLayout_DrawsAllThreeRegionStrips()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.Documents, []));

        Assert.Equal(["Left", "Center", "Right"],
            cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}").Select(strip => strip.GetAttribute("data-region")));
    }

    [Fact]
    public void SuppressedCenterStrip_ShowsMainContentEvenWithCenterDocuments()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "Main.cs")])
            .Add(x => x.ShowCenterDockStrip, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Center\"]"));
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}").Count);
        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.Main}").HasAttribute("inert"));
        Assert.True(cut.Find($".{Css.Classes.Ide.Layout.Document}").HasAttribute("hidden"));
    }

    [Fact]
    public void SuppressedCenterStrip_ControlledSelectionShowsTheDocument()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "Main.cs")])
            .Add(x => x.ShowCenterDockStrip, false)
            .Add(x => x.ActiveDocuments, Selection("a")));

        Assert.True(cut.Find($".{Css.Classes.Ide.Layout.Main}").HasAttribute("inert"));
        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.Document}").HasAttribute("hidden"));
    }

    [Fact]
    public void SuppressedCenterStrip_NullSelectionReturnsToMainContent()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "Main.cs")])
            .Add(x => x.ShowCenterDockStrip, false)
            .Add(x => x.ActiveDocuments, Selection("a")));

        cut.Render(p => p.Add(x => x.ActiveDocuments, Selection(null)));

        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.Main}").HasAttribute("inert"));
        Assert.True(cut.Find($".{Css.Classes.Ide.Layout.Document}").HasAttribute("hidden"));
    }

    [Fact]
    public void CustomCenterHeader_ReplacesTheStripAndKeepsControlledSelection()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, [new("a", "Main.cs")])
            .Add(x => x.ActiveDocuments, Selection("a"))
            .Add(x => x.CenterDockHeader, builder => builder.AddContent(0, "Host tabs")));

        var header = cut.Find($".{Css.Classes.Ide.Layout.CenterHeader}");
        Assert.Equal("Host tabs", header.TextContent);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Center\"]"));
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}").Count);
        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.Document}").HasAttribute("hidden"));
    }
}
