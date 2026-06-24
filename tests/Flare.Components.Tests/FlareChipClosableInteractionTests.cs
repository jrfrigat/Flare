// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareChip closable interaction  (5 tests)
// -----------------------------------------------------------------------------

public class FlareChipClosableInteractionTests : FlareTestContext
{
    [Fact]
    public void Closable_True_RendersCloseButton()
    {
        var cut = RenderComponent<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, true));

        Assert.NotEmpty(cut.FindAll("button.flare-chip__close"));
    }

    [Fact]
    public void Closeable_True_AlsoRendersCloseButton()
    {
        var cut = RenderComponent<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true));

        Assert.NotEmpty(cut.FindAll("button.flare-chip__close"));
    }

    [Fact]
    public void CloseButton_HasAriaLabel()
    {
        var cut = RenderComponent<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, true));

        var btn = cut.Find("button.flare-chip__close");
        Assert.NotNull(btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void OnClose_FiredWhenCloseButtonClicked()
    {
        var invoked = false;
        var cut = RenderComponent<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, true)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { invoked = true; })));

        cut.Find("button.flare-chip__close").Click();

        Assert.True(invoked);
    }

    [Fact]
    public void WithoutClosable_NoCloseButtonRendered()
    {
        var cut = RenderComponent<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closable, false)
            .Add(x => x.Closeable, false));

        Assert.Empty(cut.FindAll("button.flare-chip__close"));
    }
}
