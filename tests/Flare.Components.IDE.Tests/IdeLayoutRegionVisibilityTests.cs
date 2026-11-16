using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// A host can hide a whole docking region - the View command that closes a dock - without affecting
/// the other regions: a hidden region renders no panel, dock strip, document or resizer, and its
/// documents stay in the host list unmounted.
/// </summary>
public class IdeLayoutRegionVisibilityTests : FlareTestContext
{
    public IdeLayoutRegionVisibilityTests() => Services.AddFlareIde();

    private static readonly IdeDocument[] Documents =
    [
        new("main", "Main.cs"),
        new("left", "Left.cs", IdeDocumentRegion.Left),
        new("right", "Right.cs", IdeDocumentRegion.Right)
    ];

    private static Dictionary<IdeDocumentRegion, string?> Selection() => new()
    {
        [IdeDocumentRegion.Left] = "left",
        [IdeDocumentRegion.Right] = "right"
    };

    private IRenderedComponent<FlareIdeLayout> RenderLayout(
        bool? leftVisible = null, bool? rightVisible = null, bool docking = true, bool panels = true) =>
        Render<FlareIdeLayout>(p =>
        {
            if (docking)
            {
                p.Add(x => x.Documents, Documents);
                p.Add(x => x.ActiveDocuments, Selection());
                p.Add(x => x.DocumentContent, document => builder => builder.AddContent(0, $"Doc {document.Key}"));
            }
            if (panels)
            {
                p.Add(x => x.LeftToolPanel, builder => builder.AddContent(0, "Left tools"));
                p.Add(x => x.RightToolPanel, builder => builder.AddContent(0, "Right tools"));
            }
            if (leftVisible is { } left) p.Add(x => x.LeftRegionVisible, left);
            if (rightVisible is { } right) p.Add(x => x.RightRegionVisible, right);
        });

    private static string Region(string cssClass, string region) =>
        $".{cssClass}[data-region=\"{region}\"]";

    [Fact]
    public void DefaultLayout_DrawsBothSideRegions()
    {
        var cut = RenderLayout();

        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Left")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Right")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Resizer, "Left")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Resizer, "Right")));

        var body = cut.Find($".{Css.Classes.Ide.Layout.Body}");
        Assert.DoesNotContain(Css.Classes.Ide.Layout.BodyHideLeft, body.ClassList);
        Assert.DoesNotContain(Css.Classes.Ide.Layout.BodyHideRight, body.ClassList);
    }

    [Fact]
    public void HiddenLeftRegion_RemovesItsChromeAndKeepsTheRightRegion()
    {
        var cut = RenderLayout(leftVisible: false);

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelLeft}"));
        Assert.Empty(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Left")));
        Assert.Empty(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Left")));
        Assert.Empty(cut.FindAll(Region(Css.Classes.Ide.Layout.Resizer, "Left")));
        Assert.DoesNotContain("Left tools", cut.Markup);

        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelRight}"));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Right")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Right")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Resizer, "Right")));

        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Center")));
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.Main}"));

        var body = cut.Find($".{Css.Classes.Ide.Layout.Body}");
        Assert.Contains(Css.Classes.Ide.Layout.BodyHideLeft, body.ClassList);
        Assert.DoesNotContain(Css.Classes.Ide.Layout.BodyHideRight, body.ClassList);
    }

    [Fact]
    public void HiddenRightRegion_RemovesItsChromeAndKeepsTheLeftRegion()
    {
        var cut = RenderLayout(rightVisible: false);

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelRight}"));
        Assert.Empty(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Right")));
        Assert.Empty(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Right")));
        Assert.Empty(cut.FindAll(Region(Css.Classes.Ide.Layout.Resizer, "Right")));
        Assert.DoesNotContain("Right tools", cut.Markup);

        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelLeft}"));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Left")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Left")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Resizer, "Left")));

        var body = cut.Find($".{Css.Classes.Ide.Layout.Body}");
        Assert.Contains(Css.Classes.Ide.Layout.BodyHideRight, body.ClassList);
        Assert.DoesNotContain(Css.Classes.Ide.Layout.BodyHideLeft, body.ClassList);
    }

    [Fact]
    public void BothRegionsHidden_LeaveTheCenterColumnOnly()
    {
        var cut = RenderLayout(leftVisible: false, rightVisible: false);

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelLeft}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelRight}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.Resizer}"));
        var strip = Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.DockTabs}"));
        Assert.Equal("Center", strip.GetAttribute("data-region"));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Center")));

        var body = cut.Find($".{Css.Classes.Ide.Layout.Body}");
        Assert.Contains(Css.Classes.Ide.Layout.BodyHideLeft, body.ClassList);
        Assert.Contains(Css.Classes.Ide.Layout.BodyHideRight, body.ClassList);
    }

    [Fact]
    public void MakingTheRegionVisibleAgain_RestoresItsChromeAndDocuments()
    {
        var cut = RenderLayout(leftVisible: false);
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelLeft}"));

        cut.Render(p => p.Add(x => x.LeftRegionVisible, true));

        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelLeft}"));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.DockTabs, "Left")));
        Assert.Single(cut.FindAll(Region(Css.Classes.Ide.Layout.Document, "Left")));
        Assert.Contains("Left tools", cut.Markup);
        var body = cut.Find($".{Css.Classes.Ide.Layout.Body}");
        Assert.DoesNotContain(Css.Classes.Ide.Layout.BodyHideLeft, body.ClassList);
    }

    [Fact]
    public void HiddenRegion_HidesAPanelOutsideDockingToo()
    {
        var cut = RenderLayout(leftVisible: false, docking: false);

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.Layout.PanelLeft}"));
        Assert.DoesNotContain("Left tools", cut.Markup);
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.Layout.PanelRight}"));
        Assert.Contains("Right tools", cut.Markup);
    }

    [Fact]
    public void HiddenRegion_DoesNotMountItsDocumentContent()
    {
        var mounted = new List<string>();
        Render<FlareIdeLayout>(p => p
            .Add(x => x.Documents, Documents)
            .Add(x => x.ActiveDocuments, Selection())
            .Add(x => x.LeftRegionVisible, false)
            .Add(x => x.DocumentContent, document =>
            {
                mounted.Add(document.Key);
                return builder => builder.AddContent(0, $"Doc {document.Key}");
            }));

        Assert.DoesNotContain("left", mounted);
        Assert.Contains("main", mounted);
        Assert.Contains("right", mounted);
    }
}
