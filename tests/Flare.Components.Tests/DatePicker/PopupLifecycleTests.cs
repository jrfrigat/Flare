using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class PopupLifecycleTests : FlareTestContext
{
    public PopupLifecycleTests() => Services.AddScoped<IOverlayJsService>(_ =>
        new OverlayJsService(new RemoteOverlayRuntime(JSInterop.JSRuntime)));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RerenderDuringInterop_RegistersOnce(bool delayDismiss)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid(delayDismiss ? "registerDismiss" : "positionAnchoredPanel", _ => true);
        var cut = Render<ObservedPopup>(p => p.Add(x => x.Open, true));
        var firstSync = cut.Instance.LastSync;
        cut.Render(p => p.Add(x => x.Class, "changed"));
        var pendingCalls = module.Invocations[delayDismiss ? "registerDismiss" : "positionAnchoredPanel"].Count;
        pending.SetVoidResult();
        await Task.WhenAll(firstSync, cut.Instance.LastSync).WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(1, pendingCalls);
        Assert.Single(module.Invocations["positionAnchoredPanel"]);
        Assert.Single(module.Invocations["registerDismiss"]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CloseDuringInterop_ReleasesTheOldPanel(bool delayDismiss)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid(delayDismiss ? "registerDismiss" : "positionAnchoredPanel", _ => true);
        var cut = Render<ObservedPopup>(p => p.Add(x => x.Open, true));
        var firstSync = cut.Instance.LastSync;
        cut.Render(p => p.Add(x => x.Open, false));
        pending.SetVoidResult();
        await Task.WhenAll(firstSync, cut.Instance.LastSync).WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Empty(cut.FindAll("div"));
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Equal(delayDismiss ? 1 : 0, module.Invocations["registerDismiss"].Count);
        Assert.Equal(delayDismiss ? 1 : 0, module.Invocations["removeDismiss"].Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReopenDuringInterop_PlacesTheNewElement(bool delayDismiss)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid(delayDismiss ? "registerDismiss" : "positionAnchoredPanel", _ => true);
        var cut = Render<ObservedPopup>(p => p.Add(x => x.Open, true));
        var firstSync = cut.Instance.LastSync;
        cut.Render(p => p.Add(x => x.Open, false));
        cut.Render(p => p.Add(x => x.Open, true));
        pending.SetVoidResult();
        await Task.WhenAll(firstSync, cut.Instance.LastSync).WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        var placements = module.Invocations["positionAnchoredPanel"];
        Assert.Equal(2, placements.Count);
        Assert.NotEqual(placements[0].Arguments[2], placements[1].Arguments[2]);
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Equal(delayDismiss ? 2 : 1, module.Invocations["registerDismiss"].Count);
        Assert.Equal(delayDismiss ? 1 : 0, module.Invocations["removeDismiss"].Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DisposeDuringInterop_WaitsAndReleasesResources(bool delayDismiss)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid(delayDismiss ? "registerDismiss" : "positionAnchoredPanel", _ => true);
        var cut = Render<ObservedPopup>(p => p.Add(x => x.Open, true));
        var dispose = cut.Instance.DisposeAsync().AsTask();
        var finishedBeforeInterop = dispose.IsCompleted;
        pending.SetVoidResult();
        await dispose.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.False(finishedBeforeInterop);
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Equal(delayDismiss ? 1 : 0, module.Invocations["registerDismiss"].Count);
        Assert.Equal(delayDismiss ? 1 : 0, module.Invocations["removeDismiss"].Count);
    }

    public class ObservedPopup : FlarePopup
    {
        public Task LastSync { get; private set; } = Task.CompletedTask;
        protected override Task OnAfterRenderAsync(bool firstRender) => LastSync = base.OnAfterRenderAsync(firstRender);
    }
}
