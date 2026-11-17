using AngleSharp.Dom;
using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A side dock strip exists only while the region has documents, so an empty side reserves no strip
/// row. While a side document covers the tool group the strip starts with a compact control that
/// returns to the tools; its localized region name is the accessible name. The Center strip keeps its
/// own return-to-main-content control.
/// </summary>
public class IdeLayoutDockCaptionTests : FlareTestContext
{
    public IdeLayoutDockCaptionTests() => Services.AddFlareIde();

    private IRenderedComponent<FlareIdeLayout> RenderLayout(
        IReadOnlyList<IdeDocument> documents,
        Action<IdeDocumentSelection>? selection = null,
        RenderFragment? centerHeader = null,
        IReadOnlyDictionary<IdeDocumentRegion, string?>? active = null) =>
        Render<FlareIdeLayout>(p =>
        {
            p.Add(x => x.Documents, documents);
            if (active is not null) p.Add(x => x.ActiveDocuments, active);
            if (selection is not null)
                p.Add(x => x.OnActiveDocumentChanged,
                    EventCallback.Factory.Create<IdeDocumentSelection>(this, selection));
            if (centerHeader is not null) p.Add(x => x.CenterDockHeader, centerHeader);
        });

    private static string StripSelector(IdeDocumentRegion region) =>
        $".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"{region}\"]";

    private static string CapSelector(IdeDocumentRegion region) =>
        $"{StripSelector(region)} > button";

    private static IElement CaptionOf(IRenderedComponent<FlareIdeLayout> cut, IdeDocumentRegion region) =>
        cut.Find(CapSelector(region));

    [Fact]
    public void CenterStrip_NeverPaintsTheRegionCaptionText()
    {
        var cut = RenderLayout([new("a", "Main.cs")]);
        var strip = cut.Find(StripSelector(IdeDocumentRegion.Center));
        var caption = CaptionOf(cut, IdeDocumentRegion.Center);

        Assert.Equal("Documents", strip.GetAttribute("aria-label"));
        Assert.DoesNotContain("Documents", strip.TextContent);
        Assert.Equal(string.Empty, caption.TextContent.Trim());
        Assert.Equal("Documents", caption.GetAttribute("aria-label"));
        Assert.Single(caption.QuerySelectorAll($".{Css.Classes.Button.Icon}"));
        Assert.Empty(caption.QuerySelectorAll($".{Css.Classes.Button.Label}"));
    }

    [Fact]
    public void EmptySideRegions_DrawNoStripAndNoCaptionRow()
    {
        var cut = RenderLayout([]);

        Assert.Single(cut.FindAll(StripSelector(IdeDocumentRegion.Center)));
        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Left)));
        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Right)));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Left\"] button"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"Right\"] button"));
    }

    [Fact]
    public void EmptySideRegion_PanelFillsTheStripRow()
    {
        var cut = RenderLayout([]);

        Assert.Contains(Css.Classes.Ide.Layout.PanelNoDockStrip,
            cut.Find($".{Css.Classes.Ide.Layout.PanelLeft}").ClassList);
        Assert.Contains(Css.Classes.Ide.Layout.PanelNoDockStrip,
            cut.Find($".{Css.Classes.Ide.Layout.PanelRight}").ClassList);
    }

    [Fact]
    public void SideRegionWithADocument_KeepsItsStripAndDropsTheCaption()
    {
        var cut = RenderLayout([new("a", "A.cs", IdeDocumentRegion.Left)]);

        var strip = cut.Find(StripSelector(IdeDocumentRegion.Left));
        Assert.Equal("Left tools", strip.GetAttribute("aria-label"));
        Assert.Equal("A.cs", cut.Find($"{StripSelector(IdeDocumentRegion.Left)} .{Css.Classes.Ide.Layout.DockSelect}")
            .TextContent.Trim());
        Assert.DoesNotContain(Css.Classes.Ide.Layout.PanelNoDockStrip,
            cut.Find($".{Css.Classes.Ide.Layout.PanelLeft}").ClassList);
        Assert.Contains(Css.Classes.Ide.Layout.PanelNoDockStrip,
            cut.Find($".{Css.Classes.Ide.Layout.PanelRight}").ClassList);
        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Right)));
    }

    [Fact]
    public void SideStripWithoutACoveringDocument_DrawsNoReturnControl()
    {
        var cut = RenderLayout(
            [new("a", "A.cs", IdeDocumentRegion.Left)],
            active: new Dictionary<IdeDocumentRegion, string?> { [IdeDocumentRegion.Left] = null });

        var strip = cut.Find(StripSelector(IdeDocumentRegion.Left));
        Assert.Empty(cut.FindAll(CapSelector(IdeDocumentRegion.Left)));
        Assert.Single(strip.QuerySelectorAll($".{Css.Classes.Ide.Layout.DockSelect}"));
        Assert.DoesNotContain("Left tools", strip.TextContent);
    }

    [Fact]
    public async Task CoveringSideDocument_ReturnsToTheToolsFromTheCompactControl()
    {
        IdeDocumentSelection? reported = null;
        var cut = RenderLayout(
            [new("a", "A.cs", IdeDocumentRegion.Left)],
            selection => reported = selection);

        var panelSelector = $".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.PanelBody}";
        Assert.True(cut.Find(panelSelector).HasAttribute("inert"));

        var control = CaptionOf(cut, IdeDocumentRegion.Left);
        Assert.Equal("Left tools", control.GetAttribute("aria-label"));
        Assert.Null(control.GetAttribute("aria-pressed"));

        await cut.InvokeAsync(() => control.ClickAsync(new MouseEventArgs()));

        Assert.Equal(new IdeDocumentSelection(IdeDocumentRegion.Left, null), reported);
        Assert.False(cut.Find(panelSelector).HasAttribute("inert"));
        Assert.Empty(cut.FindAll(CapSelector(IdeDocumentRegion.Left)));
        Assert.Single(cut.FindAll(StripSelector(IdeDocumentRegion.Left)));
    }

    [Fact]
    public async Task CenterCaption_ReturnsToTheMainContent()
    {
        var cut = RenderLayout([new("a", "Main.cs")]);
        var mainSelector = $".{Css.Classes.Ide.Layout.Main}";

        Assert.True(cut.Find(mainSelector).HasAttribute("inert"));
        Assert.Equal("false", CaptionOf(cut, IdeDocumentRegion.Center).GetAttribute("aria-pressed"));

        await cut.InvokeAsync(() => CaptionOf(cut, IdeDocumentRegion.Center).ClickAsync(new MouseEventArgs()));

        Assert.False(cut.Find(mainSelector).HasAttribute("inert"));
        Assert.True(cut.Find($".{Css.Classes.Ide.Layout.Document}").HasAttribute("hidden"));
        Assert.Equal("true", CaptionOf(cut, IdeDocumentRegion.Center).GetAttribute("aria-pressed"));
    }

    [Fact]
    public void CustomCenterHeader_ReplacesTheCenterCaptionControl()
    {
        var cut = RenderLayout(
            [new("a", "Main.cs"), new("b", "Left.cs", IdeDocumentRegion.Left)],
            centerHeader: builder => builder.AddContent(0, "Host tabs"));

        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Center)));
        Assert.Equal("Host tabs", cut.Find($".{Css.Classes.Ide.Layout.CenterHeader}").TextContent);
        Assert.Equal("Left tools", CaptionOf(cut, IdeDocumentRegion.Left).GetAttribute("aria-label"));
        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Right)));
    }
}
