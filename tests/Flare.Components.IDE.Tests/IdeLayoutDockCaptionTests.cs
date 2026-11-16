using AngleSharp.Dom;
using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The side dock strips can draw their region caption as text, as an icon, or not at all, without
/// losing the localized accessible name or the control that returns from a covering document.
/// </summary>
public class IdeLayoutDockCaptionTests : FlareTestContext
{
    public IdeLayoutDockCaptionTests() => Services.AddFlareIde();

    private IRenderedComponent<FlareIdeLayout> RenderLayout(
        IdeDockCaptionMode mode,
        IReadOnlyList<IdeDocument> documents,
        Action<IdeDocumentSelection>? selection = null) =>
        Render<FlareIdeLayout>(p =>
        {
            p.Add(x => x.DockCaptionMode, mode);
            p.Add(x => x.Documents, documents);
            if (selection is not null)
                p.Add(x => x.OnActiveDocumentChanged,
                    EventCallback.Factory.Create<IdeDocumentSelection>(this, selection));
        });

    private static string StripSelector(IdeDocumentRegion region) =>
        $".{Css.Classes.Ide.Layout.DockTabs}[data-region=\"{region}\"]";

    private static IElement CaptionOf(IRenderedComponent<FlareIdeLayout> cut, IdeDocumentRegion region) =>
        cut.Find($"{StripSelector(region)} > button");

    [Fact]
    public void DefaultMode_KeepsTheVisibleCaptionText()
    {
        var cut = Render<FlareIdeLayout>(p => p.Add(x => x.Documents, []));

        var caption = CaptionOf(cut, IdeDocumentRegion.Left);

        Assert.Equal(IdeDockCaptionMode.Text, cut.Instance.DockCaptionMode);
        Assert.Equal("Left tools", caption.TextContent.Trim());
        Assert.Null(caption.GetAttribute("aria-label"));
        Assert.Equal("true", caption.GetAttribute("aria-pressed"));
        Assert.DoesNotContain(Css.Classes.Visually.Hidden, caption.ClassName);
    }

    [Fact]
    public void IconOnlyMode_KeepsTheAccessibleNameWithoutVisibleText()
    {
        var cut = RenderLayout(IdeDockCaptionMode.IconOnly, []);

        var left = CaptionOf(cut, IdeDocumentRegion.Left);
        var right = CaptionOf(cut, IdeDocumentRegion.Right);

        Assert.Equal("Left tools", left.GetAttribute("aria-label"));
        Assert.Equal("Right tools", right.GetAttribute("aria-label"));
        Assert.Equal(string.Empty, left.TextContent.Trim());
        Assert.Single(left.QuerySelectorAll($".{Css.Classes.Button.Icon}"));
        Assert.Empty(left.QuerySelectorAll($".{Css.Classes.Button.Label}"));
        Assert.Equal("Left tools", cut.Find(StripSelector(IdeDocumentRegion.Left)).GetAttribute("aria-label"));
        Assert.Equal("Documents", CaptionOf(cut, IdeDocumentRegion.Center).TextContent.Trim());
    }

    [Fact]
    public async Task HiddenMode_KeepsALabelledReturnControlWhileADocumentCoversTheTools()
    {
        IdeDocumentSelection? reported = null;
        var cut = RenderLayout(
            IdeDockCaptionMode.Hidden,
            [new("a", "Main.cs", IdeDocumentRegion.Left)],
            selection => reported = selection);

        var panel = cut.Find($".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.PanelBody}");
        Assert.True(panel.HasAttribute("inert"));

        var caption = CaptionOf(cut, IdeDocumentRegion.Left);
        Assert.Contains(Css.Classes.Visually.Hidden, caption.ClassName);
        Assert.Equal("Left tools", caption.GetAttribute("aria-label"));
        Assert.Equal("Left tools", caption.TextContent.Trim());
        Assert.Equal("Left tools", cut.Find(StripSelector(IdeDocumentRegion.Left)).GetAttribute("aria-label"));

        await cut.InvokeAsync(() => caption.ClickAsync(new MouseEventArgs()));

        Assert.Equal(new IdeDocumentSelection(IdeDocumentRegion.Left, null), reported);
        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.PanelBody}").HasAttribute("inert"));
        Assert.Empty(cut.FindAll($"{StripSelector(IdeDocumentRegion.Left)} > button"));
    }

    [Fact]
    public void HiddenMode_WithoutSideDocuments_DrawsNoSideStrip()
    {
        var cut = RenderLayout(IdeDockCaptionMode.Hidden, [new("a", "Main.cs")]);

        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Left)));
        Assert.Empty(cut.FindAll(StripSelector(IdeDocumentRegion.Right)));
        Assert.Single(cut.FindAll(StripSelector(IdeDocumentRegion.Center)));
        Assert.Equal("Documents", CaptionOf(cut, IdeDocumentRegion.Center).TextContent.Trim());
    }

    [Fact]
    public void HiddenMode_WithASideDocument_KeepsTheStripForItsDocumentTabs()
    {
        var cut = RenderLayout(
            IdeDockCaptionMode.Hidden,
            [new("a", "Main.cs"), new("b", "Guide.md", IdeDocumentRegion.Right)]);

        var strip = cut.Find(StripSelector(IdeDocumentRegion.Right));

        Assert.Single(strip.QuerySelectorAll($".{Css.Classes.Ide.Layout.DockTab}"));
        Assert.Contains("Guide.md", strip.TextContent);
        Assert.Contains(Css.Classes.Visually.Hidden, CaptionOf(cut, IdeDocumentRegion.Right).ClassName);
    }

    [Fact]
    public void HiddenMode_WithoutACoveringDocument_RendersNoReturnControl()
    {
        var cut = Render<FlareIdeLayout>(p => p
            .Add(x => x.DockCaptionMode, IdeDockCaptionMode.Hidden)
            .Add(x => x.Documents, [new("a", "Main.cs", IdeDocumentRegion.Left)])
            .Add(x => x.ActiveDocuments,
                new Dictionary<IdeDocumentRegion, string?> { [IdeDocumentRegion.Left] = null }));

        Assert.Single(cut.FindAll(StripSelector(IdeDocumentRegion.Left)));
        Assert.Empty(cut.FindAll($"{StripSelector(IdeDocumentRegion.Left)} > button"));
        Assert.False(cut.Find($".{Css.Classes.Ide.Layout.PanelLeft} .{Css.Classes.Ide.Layout.PanelBody}").HasAttribute("inert"));
    }
}
