using AngleSharp.Dom;
using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The built-in IDE dock strips never paint a region caption. The control that returns from a covering
/// document to the tools or main content is an icon-only button whose localized region name stays its
/// accessible name.
/// </summary>
public class IdeLayoutDockCaptionTests : FlareTestContext
{
    public IdeLayoutDockCaptionTests() => Services.AddFlareIde();

    private IRenderedComponent<FlareIdeLayout> RenderLayout(
        IReadOnlyList<IdeDocument> documents,
        Action<IdeDocumentSelection>? selection = null,
        RenderFragment? centerHeader = null) =>
        Render<FlareIdeLayout>(p =>
        {
            p.Add(x => x.Documents, documents);
            if (selection is not null)
                p.Add(x => x.OnActiveDocumentChanged,
                    EventCallback.Factory.Create<IdeDocumentSelection>(this, selection));
            if (centerHeader is not null) p.Add(x => x.CenterDockHeader, centerHeader);
        });

    private static string StripSelector(IdeDocumentRegion region) =>
        $".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"{region}\"]";

    private static IElement CaptionOf(IRenderedComponent<FlareIdeLayout> cut, IdeDocumentRegion region) =>
        cut.Find($"{StripSelector(region)} > button");

    private static readonly (IdeDocumentRegion Region, string Label)[] Regions =
    [
        (IdeDocumentRegion.Left, "Left tools"),
        (IdeDocumentRegion.Center, "Documents"),
        (IdeDocumentRegion.Right, "Right tools")
    ];

    [Fact]
    public void DefaultStrips_NeverPaintRegionCaptionText()
    {
        var cut = RenderLayout([]);

        foreach (var (region, label) in Regions)
        {
            var strip = cut.Find(StripSelector(region));
            var caption = CaptionOf(cut, region);

            Assert.Equal(label, strip.GetAttribute("aria-label"));
            Assert.DoesNotContain(label, strip.TextContent);
            Assert.Equal(string.Empty, caption.TextContent.Trim());
            Assert.Equal(label, caption.GetAttribute("aria-label"));
            Assert.Single(caption.QuerySelectorAll($".{Css.Classes.Button.Icon}"));
            Assert.Empty(caption.QuerySelectorAll($".{Css.Classes.Button.Label}"));
        }
    }

    [Fact]
    public async Task SideCaption_ReturnsToTheToolContentAndReportsItsPressedState()
    {
        IdeDocumentSelection? reported = null;
        var cut = RenderLayout(
            [new("a", "Main.cs", IdeDocumentRegion.Left)],
            selection => reported = selection);

        var panelSelector = $".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.PanelBody}";
        Assert.True(cut.Find(panelSelector).HasAttribute("inert"));
        Assert.Equal("false", CaptionOf(cut, IdeDocumentRegion.Left).GetAttribute("aria-pressed"));

        await cut.InvokeAsync(() => CaptionOf(cut, IdeDocumentRegion.Left).ClickAsync(new MouseEventArgs()));

        Assert.Equal(new IdeDocumentSelection(IdeDocumentRegion.Left, null), reported);
        Assert.False(cut.Find(panelSelector).HasAttribute("inert"));
        Assert.Equal("true", CaptionOf(cut, IdeDocumentRegion.Left).GetAttribute("aria-pressed"));
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
            [new("a", "Main.cs")],
            centerHeader: builder => builder.AddContent(0, "Host tabs"));

        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Center)));
        Assert.Equal("Host tabs", cut.Find($".{Css.Classes.Ide.Layout.CenterHeader}").TextContent);
        Assert.Equal("Left tools", CaptionOf(cut, IdeDocumentRegion.Left).GetAttribute("aria-label"));
        Assert.Equal("Right tools", CaptionOf(cut, IdeDocumentRegion.Right).GetAttribute("aria-label"));
    }
}
