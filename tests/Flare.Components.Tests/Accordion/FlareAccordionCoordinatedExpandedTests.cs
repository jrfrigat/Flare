using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// A panel opened by its parent after the first render has to occupy the accordion's single-expand
/// slot, or the next sibling toggle leaves two open. The slot also has to survive removal and an
/// AllowMultiple switch, and a veto still has the last word.
/// </summary>
public class FlareAccordionCoordinatedExpandedTests : FlareTestContext
{
    private bool _p0;
    private bool _p1;
    private bool _showP1 = true;
    private bool _allowMultiple;
    private Func<bool, Task<bool>>? _guard;

    private void AddPanel(RenderTreeBuilder builder, int seq, string header, bool expanded,
        Func<bool, Task<bool>>? guard = null)
    {
        builder.OpenComponent<FlareAccordionPanel>(seq);
        builder.AddAttribute(seq + 1, nameof(FlareAccordionPanel.Header), header);
        builder.AddAttribute(seq + 2, nameof(FlareAccordionPanel.Expanded), expanded);
        if (guard is not null)
            builder.AddAttribute(seq + 3, nameof(FlareAccordionPanel.OnBeforeToggle), guard);
        builder.CloseComponent();
    }

    private void AddParameters(ComponentParameterCollectionBuilder<FlareAccordion> p)
    {
        p.Add(x => x.AllowMultiple, _allowMultiple);
        p.AddChildContent(builder =>
        {
            AddPanel(builder, 0, "P0", _p0, _guard);
            if (_showP1) AddPanel(builder, 10, "P1", _p1);
        });
    }

    private IRenderedComponent<FlareAccordion> RenderPanels(Func<bool, Task<bool>>? guard = null)
    {
        _guard = guard;
        return Render<FlareAccordion>(AddParameters);
    }

    private static void AssertExpanded(IRenderedComponent<FlareAccordion> cut, bool p0, bool p1)
    {
        var buttons = cut.FindAll("button[aria-expanded]");
        Assert.Equal(p0.ToString().ToLowerInvariant(), buttons[0].GetAttribute("aria-expanded"));
        Assert.Equal(p1.ToString().ToLowerInvariant(), buttons[1].GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task APanelOpenedExternally_IsClosedByTheNextSibling()
    {
        var cut = RenderPanels();

        _p1 = true;
        cut.Render();
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click());

        AssertExpanded(cut, p0: true, p1: false);
    }

    [Fact]
    public async Task AnExternalOpen_StillHonoursThePreviousPanelsVeto()
    {
        _p0 = false;
        var cut = RenderPanels(next => Task.FromResult(next));

        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click());
        _p1 = true;
        cut.Render();

        // The veto keeps P0 open; the parent's choice still claims the slot.
        AssertExpanded(cut, p0: true, p1: true);
    }

    [Fact]
    public async Task RemovingTheOpenPanel_LeavesTheNextToggleWorking()
    {
        var cut = RenderPanels();
        _p1 = true;
        cut.Render();
        _showP1 = false;
        cut.Render();

        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click());

        var remaining = cut.FindAll("button[aria-expanded]");
        Assert.Single(remaining);
        Assert.Equal("true", remaining[0].GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task SwitchingToSingleExpand_KeepsTheLastOpenedPanel()
    {
        _allowMultiple = true;
        var cut = RenderPanels();
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click());
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[1].Click());
        AssertExpanded(cut, p0: true, p1: true);

        _allowMultiple = false;
        cut.Render(AddParameters);

        cut.WaitForAssertion(() => AssertExpanded(cut, p0: false, p1: true));
    }

    [Fact]
    public async Task SwitchingToSingleExpand_LetsTheSurvivorsSiblingVeto()
    {
        _allowMultiple = true;
        var cut = RenderPanels(next => Task.FromResult(next)); // refuses to collapse
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click());
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[1].Click());

        _allowMultiple = false;
        cut.Render(AddParameters);

        cut.WaitForAssertion(() => AssertExpanded(cut, p0: true, p1: true));
    }
}
