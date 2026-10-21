using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// The tab keyboard lives in flare-ui.js and is registered once. A stale PWA can serve that module as
/// a failure; the registration is retried on the next render so a transient failure does not leave the
/// bar without arrows, Home, End or Delete for the life of the page.
/// </summary>
public class FlareTabsKeyboardFallbackTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-ui.js";

    private static RenderFragment TabsContent() => inner =>
    {
        for (var i = 0; i < 3; i++)
        {
            inner.OpenComponent<FlareTab>(i * 10 + 2);
            inner.AddAttribute(i * 10 + 3, "Label", $"Tab {i + 1}");
            inner.CloseComponent();
        }
    };

    private IRenderedComponent<FlareTabs> RenderTabs() =>
        Render<FlareTabs>(p => p.Add(x => x.ChildContent, TabsContent()));

    [Fact]
    public void FailedRegistration_IsRetriedOnTheNextRender()
    {
        var keyboard = JSInterop.SetupModule(Module)
            .SetupVoid("registerTabKeyboard", _ => true)
            .SetException(new JSException("module not loaded"));

        RenderTabs();

        Assert.Equal(2, keyboard.Invocations.Count);
    }

    [Fact]
    public void SuccessfulRegistration_IsNotRetried()
    {
        var keyboard = JSInterop.SetupModule(Module)
            .SetupVoid("registerTabKeyboard", _ => true);

        RenderTabs();

        Assert.Single(keyboard.Invocations);
    }
}
