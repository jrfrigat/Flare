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
    public void DockedRegionsUseSharedSplitters()
    {
        Assert.Equal(2, RenderLayout().FindComponents<FlareSplitter>().Count);
    }

    [Fact]
    public void Divider_HasALocalizedNameAndItsSizeAsValue()
    {
        var left = RenderLayout().FindAll("[role=separator]")[0];

        Assert.Equal(FlareStrings.Splitter_Resize, left.GetAttribute("aria-label"));
        Assert.Equal("300", left.GetAttribute("aria-valuenow"));
        Assert.Equal("150", left.GetAttribute("aria-valuemin"));
    }

    [Fact]
    public void DividerOfARegionSizedInOtherUnits_AnnouncesNoValue()
    {
        var right = RenderLayout().FindAll("[role=separator]")[1];

        Assert.Null(right.GetAttribute("aria-valuenow"));
        Assert.Null(right.GetAttribute("aria-valuemin"));
    }

    [Fact]
    public async Task SharedSplitterCallback_UpdatesTheValue()
    {
        var cut = RenderLayout();

        await cut.InvokeAsync(() => cut.FindComponents<FlareSplitter>()[0].Instance.OnSplitChanged("308px"));

        Assert.Equal("308", cut.FindAll("[role=separator]")[0].GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void SharedSplitters_PreserveMeasuredLengthsAndDirections()
    {
        var cut = RenderLayout();

        var handles = cut.FindComponents<FlareSplitter>();
        Assert.False(handles[0].Instance.Reverse);
        Assert.True(handles[1].Instance.Reverse);
        Assert.NotNull(handles[1].Instance.ValueTarget);
        Assert.Equal(8, handles[0].Instance.KeyboardStep);
        Assert.Equal(24, handles[0].Instance.KeyboardLargeStep);
    }
}
