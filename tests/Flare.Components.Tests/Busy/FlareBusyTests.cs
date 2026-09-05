using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The delay and the minimum hold are the reason this is a component rather than a div with a spinner in
/// it, so they are what these test. Both are asserted through real waits: the timings ARE the behaviour,
/// and a test that mocked the clock would be testing a different component.
/// </summary>
public class FlareBusyTests : FlareTestContext
{
    [Fact]
    public void QuickWorkNeverFlashesASpinner()
    {
        var cut = Render<FlareBusy>(p => p.Add(x => x.Busy, true).Add(x => x.Delay, 5000));

        // Still inside the delay, so nothing has appeared yet.
        Assert.Empty(cut.FindAll($".{Css.Classes.Busy.Veil}"));
        Assert.Null(cut.Find($".{Css.Classes.Busy.Root}").GetAttribute("aria-busy"));
    }

    [Fact]
    public async Task TheOverlayAppearsOnceTheWorkOutlastsTheDelay()
    {
        var cut = Render<FlareBusy>(p => p.Add(x => x.Busy, true).Add(x => x.Delay, 20));

        await Task.Delay(200, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll($".{Css.Classes.Busy.Veil}")));
        Assert.Equal("true", cut.Find($".{Css.Classes.Busy.Root}").GetAttribute("aria-busy"));
    }

    // Once the spinner is up it stays long enough to be read: without this, work that runs just past the
    // delay produces a flicker that reads as a glitch rather than as progress.
    [Fact]
    public async Task TheOverlayIsHeldForItsMinimumOnceShown()
    {
        var cut = Render<FlareBusy>(p => p
            .Add(x => x.Busy, true)
            .Add(x => x.Delay, 10)
            .Add(x => x.MinDuration, 3000));

        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll($".{Css.Classes.Busy.Veil}")));

        cut.Render(p => p.Add(x => x.Busy, false));
        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Busy.Veil}"));
    }

    // inert rather than a focus trap: the subtree keeps its layout and its scroll position, and the
    // browser takes it out of the tab order for us.
    [Fact]
    public async Task TheCoveredSubtreeIsTakenOutOfReach()
    {
        var cut = Render<FlareBusy>(p => p
            .Add(x => x.Busy, true)
            .Add(x => x.Delay, 10)
            .AddChildContent("<button>Save</button>"));

        await Task.Delay(150, Xunit.TestContext.Current.CancellationToken);
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find($".{Css.Classes.Busy.Content}").GetAttribute("inert")));
    }

    [Fact]
    public void NotBusyRendersItsContentUncovered()
    {
        var cut = Render<FlareBusy>(p => p
            .Add(x => x.Busy, false)
            .AddChildContent("<button>Save</button>"));

        Assert.Empty(cut.FindAll($".{Css.Classes.Busy.Veil}"));
        Assert.Null(cut.Find($".{Css.Classes.Busy.Content}").GetAttribute("inert"));
        Assert.NotNull(cut.Find("button"));
    }
}

// ------------------------------------------------------------------------------
// Password strength - the rule, which is the part with a right answer
// ------------------------------------------------------------------------------
