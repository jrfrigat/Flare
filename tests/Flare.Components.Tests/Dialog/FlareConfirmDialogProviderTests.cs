using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;
using Flare.Components.Services;

namespace Flare.Components.Tests;

public class FlareConfirmDialogProviderTests : FlareTestContext
{
    public FlareConfirmDialogProviderTests()
    {
        Services.AddScoped<IOverlayJsService>(_ =>
            new OverlayJsService(new RemoteOverlayRuntime(JSInterop.JSRuntime)));
    }

    [Fact]
    public async Task ModalResources_AreRegisteredOnceAndReleasedOnCancel()
    {
        var overlay = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        overlay.Mode = JSRuntimeMode.Loose;
        var scroll = JSInterop.SetupModule("./_content/Flare.Components/js/flare-scroll.js");
        scroll.Mode = JSRuntimeMode.Loose;
        var cut = Render<FlareConfirmDialogProvider>();
        var pending = cut.Instance.ConfirmAsync("Delete?", "Keep the draft?");
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["trapFocus"]));

        cut.Render();
        Assert.Single(overlay.Invocations["trapFocus"]);
        Assert.Single(overlay.Invocations["registerDialogEscHandler"]);
        Assert.Single(scroll.Invocations["lock"]);
        cut.Find($".{Css.Classes.Confirmdialog.BtnCancel}").Click();

        Assert.False(await pending);
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["releaseFocusTrap"]));
        Assert.Single(overlay.Invocations["removeDialogEscHandler"]);
        Assert.Single(scroll.Invocations["unlock"]);
    }

    [Fact]
    public async Task EscapeDismissal_ReturnsNullAndRemovesDialog()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var pending = cut.Instance.ConfirmAsync("Delete?", "Keep the draft?");
        cut.WaitForState(() => cut.FindAll("[role='alertdialog']").Count == 1);

        await cut.Instance.CloseFromEsc();

        Assert.Null(await pending);
        cut.WaitForState(() => cut.FindAll("[role='alertdialog']").Count == 0);
    }

    [Fact]
    public async Task DisposalDuringTrap_WaitsThenReleasesResourcesAndDismissesRequest()
    {
        var overlay = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        overlay.Mode = JSRuntimeMode.Loose;
        var scroll = JSInterop.SetupModule("./_content/Flare.Components/js/flare-scroll.js");
        scroll.Mode = JSRuntimeMode.Loose;
        var trap = overlay.SetupVoid("trapFocus", _ => true);
        var cut = Render<FlareConfirmDialogProvider>();
        var pending = cut.Instance.ConfirmAsync("Delete?", "Keep the draft?");
        var queued = cut.Instance.ConfirmAsync("Next?", "Another confirmation");
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["trapFocus"]));

        var dispose = cut.Instance.DisposeAsync().AsTask();
        Assert.False(dispose.IsCompleted);
        trap.SetVoidResult();
        await dispose.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);

        Assert.Null(await pending);
        Assert.Null(await queued);
        Assert.Single(overlay.Invocations["releaseFocusTrap"]);
        Assert.Single(scroll.Invocations["unlock"]);
        await cut.Instance.DisposeAsync();
        Assert.Single(scroll.Invocations["unlock"]);
    }

    [Fact]
    public async Task ConcurrentRequests_KeepCallOrderAndOwnAnswersWithOneModalLifetime()
    {
        var overlay = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        overlay.Mode = JSRuntimeMode.Loose;
        var scroll = JSInterop.SetupModule("./_content/Flare.Components/js/flare-scroll.js");
        scroll.Mode = JSRuntimeMode.Loose;
        var cut = Render<FlareConfirmDialogProvider>();
        var first = cut.Instance.ConfirmAsync("First", "First message", "Accept first", "Keep first");
        var second = cut.Instance.ConfirmAsync("Second", "Second message", "Accept second", "Keep second");
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["trapFocus"]));

        Assert.Equal("First", cut.Find($".{Css.Classes.Confirmdialog.Title}").TextContent);
        Assert.False(second.IsCompleted);
        cut.Find($".{Css.Classes.Confirmdialog.BtnConfirm}").Click();
        Assert.True(await first);
        cut.WaitForAssertion(() => Assert.Equal("Second", cut.Find($".{Css.Classes.Confirmdialog.Title}").TextContent));
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["focusFirstInDialog"]));
        Assert.Equal("Keep second", cut.Find($".{Css.Classes.Confirmdialog.BtnCancel}").TextContent);
        Assert.Single(overlay.Invocations["trapFocus"]);
        Assert.Single(scroll.Invocations["lock"]);
        Assert.Empty(scroll.Invocations["unlock"]);
        cut.Find($".{Css.Classes.Confirmdialog.BtnCancel}").Click();
        Assert.False(await second);
        cut.WaitForAssertion(() => Assert.Single(scroll.Invocations["unlock"]));
    }

    [Fact]
    public async Task EqualRequests_AreIndependentAndEscapeDoesNotAnswerTheNextOne()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var first = cut.Instance.ConfirmAsync("Same", "Same message");
        var second = cut.Instance.ConfirmAsync("Same", "Same message");
        cut.WaitForState(() => cut.FindAll("[role='alertdialog']").Count == 1);

        await cut.Instance.CloseFromEsc();
        Assert.Null(await first);
        Assert.False(second.IsCompleted);
        cut.Find($".{Css.Classes.Confirmdialog.BtnConfirm}").Click();
        Assert.True(await second);
        cut.WaitForState(() => cut.FindAll("[role='alertdialog']").Count == 0);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareConfirmDialogProvider>(p => p
            .AddChildContent("<span id=\"child-of-confirm\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#child-of-confirm"));
    }

    [Fact]
    public void DialogNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }

    [Fact]
    public void ConfirmButtonNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.BtnConfirm}"));
    }

    [Fact]
    public void CancelButtonNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.BtnCancel}"));
    }

    [Fact]
    public void AfterConfirmAsync_DialogVisible()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Delete?", "Are you sure?");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }

    [Fact]
    public void AfterConfirmAsync_TitleRendered()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Delete item", "Cannot be undone.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Title}").Count > 0);

        Assert.Contains("Delete item", cut.Find($".{Css.Classes.Confirmdialog.Title}").TextContent);
    }

    [Fact]
    public void AfterConfirmAsync_MessageRendered()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Title", "Please confirm this action.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Body}").Count > 0);

        Assert.Contains("Please confirm this action.", cut.Find($".{Css.Classes.Confirmdialog.Body}").TextContent);
    }

    [Fact]
    public void ClickConfirm_DialogDismisses()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Sure?", "Yes or No");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.BtnConfirm}").Count > 0);

        cut.Find($".{Css.Classes.Confirmdialog.BtnConfirm}").Click();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }
}
