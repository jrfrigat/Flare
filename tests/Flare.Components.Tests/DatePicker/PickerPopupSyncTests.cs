namespace Flare.Components.Tests;

/// <summary>Picker placement is not repeated while a previous JS call is pending.</summary>
public class PickerPopupSyncTests : FlareTestContext
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DateParameterChangeWhilePlacing_DoesNotPlaceTwice(bool fromField)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var placement = module.SetupVoid("positionAnchoredPanel", _ => true);
        var cut = Render<ObservedDatePicker>();
        var before = cut.Instance.Rendered;
        cut.Find(fromField ? "input" : $".{Css.Classes.Input.Toggle}").Click();
        Assert.Single(cut.FindAll("[role='dialog']"));
        Assert.Equal(before + 1, cut.Instance.Rendered);
        cut.Render(p => p.Add(x => x.Label, "Changed while placing"));
        Assert.Single(module.Invocations["positionAnchoredPanel"]);
        placement.SetVoidResult();
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Single(module.Invocations[fromField ? "registerDismiss" : "trapFocus"]);
        Assert.Single(module.Invocations["positionAnchoredPanel"]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TimeParameterChangeWhilePlacing_DoesNotPlaceTwice(bool fromField)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var placement = module.SetupVoid("positionAnchoredPanel", _ => true);
        var cut = Render<ObservedTimePicker>();
        var before = cut.Instance.Rendered;
        cut.Find(fromField ? "input" : $".{Css.Classes.Input.Toggle}").Click();
        Assert.Single(cut.FindAll("[role='dialog']"));
        Assert.Equal(before + 1, cut.Instance.Rendered);
        cut.Render(p => p.Add(x => x.Label, "Changed while placing"));
        Assert.Single(module.Invocations["positionAnchoredPanel"]);
        placement.SetVoidResult();
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Single(module.Invocations[fromField ? "registerDismiss" : "trapFocus"]);
        Assert.Single(module.Invocations["positionAnchoredPanel"]);
    }

    [Fact]
    public async Task DateProgrammaticOpenAndClose_StillRender()
    {
        var cut = Render<ObservedDatePicker>();
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        Assert.Single(cut.FindAll("[role='dialog']"));
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Empty(cut.FindAll("[role='dialog']"));
        await cut.InvokeAsync(cut.Instance.ToggleAsync);
        Assert.Single(cut.FindAll("[role='dialog']"));
    }

    [Fact]
    public async Task TimeProgrammaticOpenAndClose_StillRender()
    {
        var cut = Render<FlareTimePicker>();
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        Assert.Single(cut.FindAll("[role='dialog']"));
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Empty(cut.FindAll("[role='dialog']"));
        await cut.InvokeAsync(cut.Instance.ToggleAsync);
        Assert.Single(cut.FindAll("[role='dialog']"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DateCloseWhilePlacing_RemovesLayerWithoutTakingFocus(bool fromField)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var placement = module.SetupVoid("positionAnchoredPanel", _ => true);
        var cut = Render<ObservedDatePicker>();
        cut.Find(fromField ? "input" : $".{Css.Classes.Input.Toggle}").Click();
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        Assert.Empty(cut.FindAll("[role='dialog']"));
        placement.SetVoidResult();
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Empty(module.Invocations["trapFocus"]);
        Assert.Empty(module.Invocations["registerDismiss"]);
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Single(module.Invocations["trapFocus"]);
        Assert.Equal(2, module.Invocations["positionAnchoredPanel"].Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TimeDisposeWhilePlacing_ReleasesLayerWithoutInstallingHandlers(bool fromField)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var placement = module.SetupVoid("positionAnchoredPanel", _ => true);
        var cut = Render<FlareTimePicker>();
        cut.Find(fromField ? "input" : $".{Css.Classes.Input.Toggle}").Click();
        var dispose = cut.Instance.DisposeAsync().AsTask();
        Assert.False(dispose.IsCompleted);
        placement.SetVoidResult();
        await dispose;
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Empty(module.Invocations["trapFocus"]);
        Assert.Empty(module.Invocations["registerDismiss"]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DateReopenWhileSyncing_PlacesTheNewPanel(bool delayFocusTrap)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid(delayFocusTrap ? "trapFocus" : "positionAnchoredPanel", _ => true);
        var cut = Render<ObservedDatePicker>();
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        pending.SetVoidResult();
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(2, module.Invocations["positionAnchoredPanel"].Count);
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Equal(delayFocusTrap ? 2 : 1, module.Invocations["trapFocus"].Count);
        var placements = module.Invocations["positionAnchoredPanel"];
        Assert.NotEqual(placements[0].Arguments[2], placements[1].Arguments[2]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TimeReopenWhileSyncing_PlacesTheNewPanel(bool delayFocusTrap)
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var pending = module.SetupVoid(delayFocusTrap ? "trapFocus" : "positionAnchoredPanel", _ => true);
        var cut = Render<ObservedTimePicker>();
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        await cut.InvokeAsync(cut.Instance.CloseAsync);
        await cut.InvokeAsync(cut.Instance.OpenAsync);
        pending.SetVoidResult();
        await cut.Instance.LastSync.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(2, module.Invocations["positionAnchoredPanel"].Count);
        Assert.Single(module.Invocations["removeAnchoredPanel"]);
        Assert.Equal(delayFocusTrap ? 2 : 1, module.Invocations["trapFocus"].Count);
        var placements = module.Invocations["positionAnchoredPanel"];
        Assert.NotEqual(placements[0].Arguments[2], placements[1].Arguments[2]);
    }

    // bUnit's fragment RenderCount includes descendant batches. Count the owner's lifecycle instead.
    public class ObservedDatePicker : FlareDatePicker
    {
        public int Rendered { get; private set; }
        public Task LastSync { get; private set; } = Task.CompletedTask;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            Rendered++;
            return LastSync = base.OnAfterRenderAsync(firstRender);
        }
    }

    public class ObservedTimePicker : FlareTimePicker
    {
        public int Rendered { get; private set; }
        public Task LastSync { get; private set; } = Task.CompletedTask;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            Rendered++;
            return LastSync = base.OnAfterRenderAsync(firstRender);
        }
    }
}
