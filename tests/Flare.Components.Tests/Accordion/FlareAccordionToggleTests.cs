using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareAccordion: auto-collapsing a sibling now notifies (two-way bind stays in sync), and a panel's
// OnBeforeToggle can veto a toggle.
public class FlareAccordionToggleTests : FlareTestContext
{
    [Fact]
    public async Task SingleExpand_AutoCollapsesSibling_AndNotifies()
    {
        var p0States = new List<bool>();
        var cut = Render<FlareAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareAccordionPanel>(0);
            b.AddAttribute(1, nameof(FlareAccordionPanel.Header), (object)"P0");
            b.AddAttribute(2, nameof(FlareAccordionPanel.ExpandedChanged),
                EventCallback.Factory.Create<bool>(this, v => p0States.Add(v)));
            b.CloseComponent();
            b.OpenComponent<FlareAccordionPanel>(3);
            b.AddAttribute(4, nameof(FlareAccordionPanel.Header), (object)"P1");
            b.CloseComponent();
        }));

        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click()); // expand P0
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[1].Click()); // expand P1 -> P0 collapses

        Assert.Contains(true, p0States);
        Assert.Contains(false, p0States);  // the auto-collapse fired ExpandedChanged(false) -- the bug fix
    }

    [Fact]
    public async Task OnBeforeToggle_ReturningFalse_BlocksExpand()
    {
        var cut = Render<FlareAccordionPanel>(p => p
            .Add(x => x.Header, "P")
            .Add(x => x.OnBeforeToggle, _ => Task.FromResult(false)));
        await cut.InvokeAsync(() => cut.Find("button[aria-expanded]").Click());
        Assert.Equal("false", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded"));
    }

    // The guard exists for "confirm before closing a panel with unsaved edits", and single-expand is the
    // case that fires it: opening a sibling is what closes you. Auto-collapse used to skip it entirely.
    [Fact]
    public async Task VetoedAutoCollapse_KeepsTheOldPanelOpen_AndStopsTheNewOneOpening()
    {
        var cut = Render<FlareAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareAccordionPanel>(0);
            b.AddAttribute(1, nameof(FlareAccordionPanel.Header), (object)"P0");
            b.AddAttribute(2, nameof(FlareAccordionPanel.OnBeforeToggle),
                (Func<bool, Task<bool>>)(next => Task.FromResult(next)));  // refuses to collapse, allows expand
            b.CloseComponent();
            b.OpenComponent<FlareAccordionPanel>(3);
            b.AddAttribute(4, nameof(FlareAccordionPanel.Header), (object)"P1");
            b.CloseComponent();
        }));

        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[0].Click());  // P0 opens
        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[1].Click());  // P1 blocked by P0's veto

        var buttons = cut.FindAll("button[aria-expanded]");
        Assert.Equal("true", buttons[0].GetAttribute("aria-expanded"));
        Assert.Equal("false", buttons[1].GetAttribute("aria-expanded"));
    }

    // A panel declared Expanded in markup never registered with the coordinator, so the first toggle of
    // another panel found nothing to close and single-expand quietly became multi-expand.
    [Fact]
    public async Task InitiallyExpandedPanel_IsCollapsedByTheFirstSiblingToOpen()
    {
        var cut = Render<FlareAccordion>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<FlareAccordionPanel>(0);
            b.AddAttribute(1, nameof(FlareAccordionPanel.Header), (object)"P0");
            b.AddAttribute(2, nameof(FlareAccordionPanel.Expanded), true);
            b.CloseComponent();
            b.OpenComponent<FlareAccordionPanel>(3);
            b.AddAttribute(4, nameof(FlareAccordionPanel.Header), (object)"P1");
            b.CloseComponent();
        }));

        await cut.InvokeAsync(() => cut.FindAll("button[aria-expanded]")[1].Click());

        var buttons = cut.FindAll("button[aria-expanded]");
        Assert.Equal("false", buttons[0].GetAttribute("aria-expanded"));
        Assert.Equal("true", buttons[1].GetAttribute("aria-expanded"));
    }
}
