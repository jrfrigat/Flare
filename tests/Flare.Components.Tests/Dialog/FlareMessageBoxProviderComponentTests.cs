using Flare.Abstractions;
using Flare.Components.Services;
using Flare.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Flare.Components.Tests;

public class FlareMessageBoxProviderComponentTests : FlareTestContext
{
    [Theory]
    [InlineData("confirm")]
    [InlineData("cancel")]
    [InlineData("enter")]
    [InlineData("escape")]
    [InlineData("input")]
    public async Task QueuedCallback_FromPreviousRequestCannotAnswerOrChangeTheNext(string action)
    {
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        var first = service.PromptAsync("First", defaultValue: "Original");
        var second = service.PromptAsync("Second", defaultValue: "Next");
        cut.WaitForState(() => cut.FindAll("input").Count == 1);
        var cssClass = action switch
        {
            "confirm" => Css.Classes.Msgbox.BtnConfirm,
            "cancel" => Css.Classes.Msgbox.BtnCancel,
            "escape" => Css.Classes.Msgbox.Root,
            _ => Css.Classes.Msgbox.Input,
        };
        var eventName = action is "confirm" or "cancel" ? "onclick"
            : action == "input" ? "oninput" : action == "enter" ? "onkeyup" : "onkeydown";
        var callback = CaptureHandler(cut.ComponentId, cssClass, eventName);
        cut.Find($".{Css.Classes.Msgbox.BtnCancel}").Click();
        Assert.Null(await first);
        cut.WaitForAssertion(() => Assert.Equal("Next", cut.Find("input").GetAttribute("value")));

        object args = action == "input" ? new ChangeEventArgs { Value = "Late input" }
            : action is "confirm" or "cancel" ? new MouseEventArgs()
            : new KeyboardEventArgs { Key = action == "enter" ? "Enter" : "Escape" };
        await cut.InvokeAsync(() => InvokeCapturedAsync(callback, args));
        await cut.InvokeAsync(() => InvokeCapturedAsync(callback, args));
        cut.Render();

        Assert.False(second.IsCompleted);
        Assert.Equal("Next", cut.Find("input").GetAttribute("value"));
        cut.Find($".{Css.Classes.Msgbox.BtnCancel}").Click();
        Assert.Null(await second);
    }

#pragma warning disable BL0006
    private object CaptureHandler(int componentId, string cssClass, string eventName)
    {
        var method = typeof(Microsoft.AspNetCore.Components.RenderTree.Renderer).GetMethod(
            "GetCurrentRenderTreeFrames", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        var frames = (ArrayRange<RenderTreeFrame>)method.Invoke(Renderer, [componentId])!;
        for (var i = 0; i < frames.Count; i++)
        {
            if (frames.Array[i].FrameType != RenderTreeFrameType.Element) continue;
            var matches = false;
            object? callback = null;
            for (var j = i + 1; j < frames.Count && frames.Array[j].FrameType == RenderTreeFrameType.Attribute; j++)
            {
                var attribute = frames.Array[j];
                if (attribute.AttributeName == "class")
                    matches = attribute.AttributeValue?.ToString()?.Split(' ').Contains(cssClass) == true;
                if (attribute.AttributeName == eventName) callback = attribute.AttributeValue;
            }
            if (matches && callback is not null) return callback;
        }
        throw new InvalidOperationException($"No {eventName} handler on {cssClass}.");
    }
#pragma warning restore BL0006

    private static async Task InvokeCapturedAsync(object callback, object args)
    {
        if (callback is EventCallback<MouseEventArgs> mouse) await mouse.InvokeAsync((MouseEventArgs)args);
        else if (callback is EventCallback<KeyboardEventArgs> keyboard) await keyboard.InvokeAsync((KeyboardEventArgs)args);
        else if (callback is EventCallback<ChangeEventArgs> change) await change.InvokeAsync((ChangeEventArgs)args);
        else if (callback is EventCallback untyped) await untyped.InvokeAsync(args);
        else
        {
            var handler = Assert.IsAssignableFrom<MulticastDelegate>(callback);
            var result = handler.DynamicInvoke(handler.Method.GetParameters().Length == 0 ? [] : [args]);
            if (result is Task task) await task;
        }
    }

    public FlareMessageBoxProviderComponentTests()
    {
        Services.AddSingleton<IMessageBoxService, MessageBoxService>();
        Services.AddScoped<IOverlayJsService>(_ =>
            new OverlayJsService(new RemoteOverlayRuntime(JSInterop.JSRuntime)));
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("", true)]
    [InlineData("  ", false)]
    [InlineData("  ", true)]
    public async Task Prompt_ReturnsExactInputIncludingEmptyAndWhitespace(string value, bool enter)
    {
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        var pending = service.PromptAsync("Name", defaultValue: "Original");
        cut.WaitForState(() => cut.FindAll("input").Count == 1);
        cut.Find("input").Input(value);

        if (enter)
        {
            cut.Find("input").KeyDown("Enter");
            Assert.False(pending.IsCompleted);
            cut.Find("input").KeyUp("Enter");
        }
        else cut.Find($".{Css.Classes.Msgbox.BtnConfirm}").Click();

        Assert.Equal(value, await pending);
    }

    [Fact]
    public async Task Queue_HoldsOneScrollLockUntilTheLastMessageCloses()
    {
        var overlay = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        overlay.Mode = JSRuntimeMode.Loose;
        var scroll = JSInterop.SetupModule("./_content/Flare.Components/js/flare-scroll.js");
        scroll.Mode = JSRuntimeMode.Loose;
        var service = Services.GetRequiredService<IMessageBoxService>();
        var cut = Render<FlareMessageBoxProvider>();
        var first = service.PromptAsync("First");
        var second = service.PromptAsync("Second");
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["trapFocus"]));

        cut.Find($".{Css.Classes.Msgbox.BtnCancel}").Click();
        Assert.Null(await first);
        cut.WaitForAssertion(() => Assert.Single(overlay.Invocations["focusFirstInDialog"]));
        Assert.Single(scroll.Invocations["lock"]);
        Assert.Empty(scroll.Invocations["unlock"]);
        cut.Find($".{Css.Classes.Msgbox.BtnCancel}").Click();
        Assert.Null(await second);
        cut.WaitForAssertion(() => Assert.Single(scroll.Invocations["unlock"]));
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
