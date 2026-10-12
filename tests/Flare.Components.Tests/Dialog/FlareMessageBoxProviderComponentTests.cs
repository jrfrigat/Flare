using Flare.Abstractions;
using Flare.Components.Services;
using Flare.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareMessageBoxProviderComponentTests : FlareTestContext
{
    public FlareMessageBoxProviderComponentTests()
    {
        Services.AddSingleton<IMessageBoxService, MessageBoxService>();
        Services.AddScoped<IOverlayJsService>(_ =>
            new OverlayJsService(new RemoteOverlayRuntime(JSInterop.JSRuntime)));
    }

    [Fact]
    public async Task TypingAndRerendering_PreserveInputAndDoNotRecaptureFocus()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        var pending = service.PromptAsync("Name", defaultValue: "Ann");
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["trapFocus"]));

        cut.Find("input").Input("Bob");
        cut.Render();
        Assert.Equal("Bob", cut.Find("input").GetAttribute("value"));
        Assert.Single(module.Invocations["trapFocus"]);
        Assert.Empty(module.Invocations["focusFirstInDialog"]);
        cut.Find($".{Css.Classes.Msgbox.BtnConfirm}").Click();

        Assert.Equal("Bob", await pending);
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["releaseFocusTrap"]));
    }

    [Fact]
    public async Task Queue_KeepsOriginalTrapUntilTheLastRequestCloses()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        var first = service.PromptAsync("First");
        var second = service.PromptAsync("Second", defaultValue: "Next");
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["trapFocus"]));
        cut.Find($".{Css.Classes.Msgbox.BtnCancel}").Click();

        Assert.Null(await first);
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["focusFirstInDialog"]));
        Assert.Equal("Next", cut.Find("input").GetAttribute("value"));
        Assert.Empty(module.Invocations["releaseFocusTrap"]);
        Assert.Single(module.Invocations["trapFocus"]);
        cut.Find($".{Css.Classes.Msgbox.BtnCancel}").Click();

        Assert.Null(await second);
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["releaseFocusTrap"]));
    }

    [Fact]
    public async Task RerenderDuringTrap_PreservesTypedInput()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var trap = module.SetupVoid("trapFocus", _ => true);
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        var pending = service.PromptAsync("Name", defaultValue: "Ann");
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["trapFocus"]));
        cut.Find("input").Input("Bob");
        cut.Render();
        Assert.Equal("Bob", cut.Find("input").GetAttribute("value"));
        trap.SetVoidResult();
        cut.Find($".{Css.Classes.Msgbox.BtnConfirm}").Click();
        Assert.Equal("Bob", await pending);
    }

    [Fact]
    public async Task DisposeDuringTrap_WaitsForRegistrationThenReleasesIt()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        var trap = module.SetupVoid("trapFocus", _ => true);
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        _ = service.PromptAsync("Name");
        cut.WaitForAssertion(() => Assert.Single(module.Invocations["trapFocus"]));
        var dispose = ((IAsyncDisposable)cut.Instance).DisposeAsync().AsTask();
        Assert.False(dispose.IsCompleted);
        trap.SetVoidResult();

        await dispose.WaitAsync(TimeSpan.FromSeconds(5), Xunit.TestContext.Current.CancellationToken);
        Assert.Single(module.Invocations["releaseFocusTrap"]);
    }
}
