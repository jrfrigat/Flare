// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareChip closable  (6 tests)
// Note: the parameter is "Closeable" (not "Closable") per the source.
// -----------------------------------------------------------------------------

public class FlareChipClosableTests : FlareTestContext
{
    [Fact]
    public void Closeable_False_Default_NoCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, false));

        Assert.Empty(cut.FindAll("button.flare-chip__close"));
    }

    [Fact]
    public void Closeable_True_RendersCloseButton()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true));

        Assert.NotEmpty(cut.FindAll("button.flare-chip__close"));
    }

    [Fact]
    public void Closeable_True_CloseButtonHasAriaLabel()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true));

        var btn = cut.Find("button.flare-chip__close");
        Assert.NotNull(btn.GetAttribute("aria-label"));
    }

    [Fact]
    public void Closeable_True_ClickClose_InvokesOnClose()
    {
        var invoked = false;
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { invoked = true; })));

        cut.Find("button.flare-chip__close").Click();

        Assert.True(invoked);
    }

    [Fact]
    public void Label_RendersLabelText()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "MyChip"));

        Assert.Contains("MyChip", cut.Find(".flare-chip__label").TextContent);
    }

    [Fact]
    public void Closeable_True_AndSelected_BothRenderCorrectly()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Tag")
            .Add(x => x.Closeable, true)
            .Add(x => x.Selected, true));

        Assert.NotEmpty(cut.FindAll("button.flare-chip__close"));
        Assert.Contains("flare-chip--selected", cut.Find(".flare-chip").ClassName ?? "");
    }
}
