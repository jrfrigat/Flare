// Legacy wave file - tests have been migrated to Component/ directory.
// Kept for reference only. New tests should go in Component/*.cs files.
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// -----------------------------------------------------------------------------
// FlareAlert animation tests  (6 tests)
// -----------------------------------------------------------------------------

public class FlareAlertAnimationTests : FlareTestContext
{
    [Fact]
    public void Default_RendersAlertElement()
    {
        var cut = RenderComponent<FlareAlert>();

        Assert.NotEmpty(cut.FindAll(".flare-alert"));
    }

    [Fact]
    public void Severity_Success_AddsSuccessClass()
    {
        var cut = RenderComponent<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Success));

        Assert.Contains("flare-alert--success", cut.Find(".flare-alert").ClassName ?? "");
    }

    [Fact]
    public void ShowCloseButton_True_RendersCloseButton()
    {
        var cut = RenderComponent<FlareAlert>(p => p
            .Add(x => x.ShowCloseButton, true));

        Assert.NotEmpty(cut.FindAll("button.flare-alert__close"));
    }

    [Fact]
    public void AnimateDismiss_ParamExists_DefaultTrue_RendersWithoutError()
    {
        // AnimateDismiss defaults to true - component renders normally
        var cut = RenderComponent<FlareAlert>(p => p
            .Add(x => x.AnimateDismiss, true));

        Assert.NotEmpty(cut.FindAll(".flare-alert"));
    }

    [Fact]
    public void AnimateDismiss_False_WithShowCloseButton_OnClose_InvokedSynchronously()
    {
        // When AnimateDismiss=false there is no Task.Delay(300), so OnClose fires promptly.
        // bUnit's Click() is synchronous so we can still observe it.
        var invoked = false;
        var cut = RenderComponent<FlareAlert>(p => p
            .Add(x => x.ShowCloseButton, true)
            .Add(x => x.AnimateDismiss, false)
            .Add(x => x.OnClose, EventCallback.Factory.Create(this, () => { invoked = true; })));

        cut.Find("button.flare-alert__close").Click();

        Assert.True(invoked);
    }

    [Fact]
    public void ChildContent_RendersMessageText()
    {
        var cut = RenderComponent<FlareAlert>(p => p
            .AddChildContent("Something went wrong."));

        Assert.Contains("Something went wrong.", cut.Find(".flare-alert__body").TextContent);
    }
}
