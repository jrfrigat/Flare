using Flare.Components.IDE;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The divider between a docked IDE region and the document (Draftline TASK-58): a localized name, a value a
/// screen reader can announce, and a drag that starts from any pointer, a finger included.
/// </summary>
public class IdeLayoutResizerTests : FlareTestContext
{
    public IdeLayoutResizerTests() => Services.AddFlareIde();

    private static RenderFragment Panel(ToolPanelPosition position) => b =>
    {
        b.OpenComponent<FlareToolPanel>(0);
        b.AddAttribute(1, nameof(FlareToolPanel.Position), position);
        b.CloseComponent();
    };

    private IRenderedComponent<FlareIdeLayout> RenderLayout() => Render<FlareIdeLayout>(p => p
        .Add(x => x.LeftToolPanel, Panel(ToolPanelPosition.Left))
        .Add(x => x.RightToolPanel, Panel(ToolPanelPosition.Right))
        .Add(x => x.RightPanelWidth, "18rem")
        .Add(x => x.MinPanelSize, 150));

    [Fact]
    public void Divider_HasALocalizedNameAndItsSizeAsValue()
    {
        var left = RenderLayout().FindAll("[role=separator]")[0];

        Assert.Equal(FlareStrings.Splitter_Resize, left.GetAttribute("aria-label"));
        Assert.Equal("300", left.GetAttribute("aria-valuenow"));
        Assert.Equal("150", left.GetAttribute("aria-valuemin"));
        Assert.Equal("300 px", left.GetAttribute("aria-valuetext"));
    }

    [Fact]
    public void DividerOfARegionSizedInOtherUnits_AnnouncesNoValue()
    {
        var right = RenderLayout().FindAll("[role=separator]")[1];

        Assert.Null(right.GetAttribute("aria-valuenow"));
        Assert.Null(right.GetAttribute("aria-valuemin"));
    }

    [Fact]
    public void KeyboardResize_UpdatesTheValue()
    {
        var cut = RenderLayout();

        cut.FindAll("[role=separator]")[0].KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        Assert.Equal("308", cut.FindAll("[role=separator]")[0].GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void PointerDown_StartsTheDrag()
    {
        var cut = RenderLayout();

        cut.FindAll("[role=separator]")[0].PointerDown();

        Assert.Contains(JSInterop.Invocations, i => i.Identifier == "startPanelResize");
    }
}
