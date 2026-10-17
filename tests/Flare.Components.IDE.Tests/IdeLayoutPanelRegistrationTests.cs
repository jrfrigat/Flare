using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// A region of <see cref="FlareIdeLayout"/> can host more than one <see cref="FlareToolPanel"/>. Each panel
/// reports its title and mode to the layout while it renders; when the layout kept a single record per
/// region, two panels with different titles overwrote it in turn and re-rendered the layout forever.
/// </summary>
public class IdeLayoutPanelRegistrationTests : FlareTestContext
{
    public IdeLayoutPanelRegistrationTests() => Services.AddFlareIde();

    private static RenderFragment TwoPanels(IdePanelMode first = IdePanelMode.Docked, IdePanelMode second = IdePanelMode.Docked) => b =>
    {
        b.OpenComponent<FlareToolPanel>(0);
        b.AddAttribute(1, nameof(FlareToolPanel.Position), ToolPanelPosition.Right);
        b.AddAttribute(2, nameof(FlareToolPanel.Title), "Properties");
        b.AddAttribute(3, nameof(FlareToolPanel.Mode), first);
        b.CloseComponent();
        b.OpenComponent<FlareToolPanel>(4);
        b.AddAttribute(5, nameof(FlareToolPanel.Position), ToolPanelPosition.Right);
        b.AddAttribute(6, nameof(FlareToolPanel.Title), "Screens");
        b.AddAttribute(7, nameof(FlareToolPanel.Mode), second);
        b.CloseComponent();
    };

    // On the regression the first render never returns, so this class hangs the run instead of passing.
    private IRenderedComponent<FlareIdeLayout> RenderLayout(RenderFragment rightPanels) =>
        Render<FlareIdeLayout>(p => p.Add(x => x.RightToolPanel, rightPanels));

    [Fact]
    public void TwoPanelsInOneRegion_RenderOnceAndBothAppear()
    {
        var cut = RenderLayout(TwoPanels());

        Assert.Equal(["Properties", "Screens"],
            cut.FindAll($".{Css.Classes.Ide.ToolPanel.Title}").Select(t => t.TextContent));
    }

    [Fact]
    public async Task CollapsedRegion_RailNamesEveryPanel()
    {
        var cut = RenderLayout(TwoPanels());

        await cut.InvokeAsync(() => cut.FindAll($".{Css.Classes.Ide.ToolPanel.Header} button")[0].Click());

        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.ToolPanel.Root}"));
        Assert.Equal(["Properties", "Screens"],
            cut.FindAll($".{Css.Classes.Ide.Layout.RailTitle}").Select(t => t.TextContent));
    }

    [Fact]
    public void Region_FloatsOnlyWhenEveryPanelAutoHides()
    {
        var mixed = RenderLayout(TwoPanels(IdePanelMode.AutoHide, IdePanelMode.Docked));
        Assert.Empty(mixed.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}"));

        var floating = RenderLayout(TwoPanels(IdePanelMode.AutoHide, IdePanelMode.AutoHide));
        Assert.NotEmpty(floating.FindAll($".{Css.Classes.Ide.Layout.PanelFloating}"));
    }

    [Fact]
    public async Task SinglePanel_StillLabelsTheRail()
    {
        var cut = RenderLayout(b =>
        {
            b.OpenComponent<FlareToolPanel>(0);
            b.AddAttribute(1, nameof(FlareToolPanel.Position), ToolPanelPosition.Right);
            b.AddAttribute(2, nameof(FlareToolPanel.Title), "Properties");
            b.CloseComponent();
        });

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Ide.ToolPanel.Header} button").Click());

        Assert.Equal("Properties", cut.Find($".{Css.Classes.Ide.Layout.RailTitle}").TextContent);
    }
}
