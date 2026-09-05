using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlarePopover Trigger="Click" toggles open from the anchor without external wiring.
public class FlarePopoverTriggerTests : FlareTestContext
{
    [Fact]
    public async Task ClickTrigger_TogglesOpenFromAnchor()
    {
        var states = new List<bool>();
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Click)
            .Add(x => x.AnchorContent, "<button>open</button>")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));

        await cut.InvokeAsync(() => cut.Find("span[style*=cursor]").Click());
        Assert.Equal(new[] { true }, states);   // anchor click requested open
    }

    [Fact]
    public void ManualTrigger_DoesNotWrapAnchor()
    {
        var cut = Render<FlarePopover>(p => p.Add(x => x.AnchorContent, "<button>x</button>"));
        Assert.Empty(cut.FindAll("span[style*=cursor]"));   // default Manual: no built-in handler
    }

    // The bug this pins: moving the pointer from the anchor into the panel closed the popover under it.
    // `Offset` leaves a dead band between the two - the panel is absolutely positioned, so it adds nothing to
    // the root's box - and crossing it fires a real mouseleave. That starts the HideDelay wait. Re-entering
    // then had to cancel it, but HandleMouseEnter returned on `|| Open` BEFORE bumping the generation that is
    // the only cancellation there is, so the hide ran to completion. HideDelay's whole stated purpose is to
    // survive exactly this gap, so raising it only delayed the vanishing.
    [Fact]
    public async Task HoverReentry_CancelsThePendingHide()
    {
        var states = new List<bool>();
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Hover)
            .Add(x => x.Open, true)          // open, as it is while the pointer crosses the gap
            .Add(x => x.HideDelay, 60)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));

        var root = cut.Find($".{Css.Classes.Popover.Anchor}");
        await cut.InvokeAsync(() => root.MouseLeave());   // into the Offset gap: hide is now pending
        await cut.InvokeAsync(() => root.MouseEnter());   // into the panel: must cancel that hide

        await Task.Delay(200, Xunit.TestContext.Current.CancellationToken);   // outlast HideDelay

        Assert.DoesNotContain(false, states);
    }

    // The mirror of the same mistake, on the other handler: leaving DURING the open delay must cancel the
    // pending open. While that delay runs the popover is still closed, so a `!Open` guard placed before the
    // generation bump would return early and let the popover open onto a pointer that has already gone -
    // which is the one thing Delay exists to prevent.
    [Fact]
    public async Task HoverLeave_DuringTheOpenDelay_CancelsThePendingOpen()
    {
        var states = new List<bool>();
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Hover)
            .Add(x => x.Open, false)        // closed, as it is while the open delay runs
            .Add(x => x.Delay, 60)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));

        var root = cut.Find($".{Css.Classes.Popover.Anchor}");
        await cut.InvokeAsync(() => root.MouseEnter());   // open is now pending
        await cut.InvokeAsync(() => root.MouseLeave());   // passed straight through: cancel it

        await Task.Delay(200, Xunit.TestContext.Current.CancellationToken);   // outlast Delay

        Assert.DoesNotContain(true, states);
    }

    // The other half of the same generation counter: a leave with no re-entry must still close.
    [Fact]
    public async Task HoverLeave_WithoutReentry_StillCloses()
    {
        var states = new List<bool>();
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Hover)
            .Add(x => x.Open, true)
            .Add(x => x.HideDelay, 20)
            .Add(x => x.AnchorContent, "<button>anchor</button>")
            .Add(x => x.OpenChanged, EventCallback.Factory.Create<bool>(this, v => states.Add(v))));

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Popover.Anchor}").MouseLeave());
        await Task.Delay(200, Xunit.TestContext.Current.CancellationToken);

        Assert.Contains(false, states);
    }
}
