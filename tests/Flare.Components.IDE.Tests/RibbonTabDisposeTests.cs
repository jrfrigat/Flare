using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Flare.Components.Tests;

/// <summary>
/// A removed ribbon tab has to unregister itself. Its cleanup lives in <see cref="IDisposable.Dispose"/>, and the
/// component inherits <see cref="FlareComponentBase"/>, which owns the asynchronous disposal contract - Blazor used
/// to choose the asynchronous one and never call Dispose, so the ribbon kept the removed tab in its list and no tab
/// was left active.
/// </summary>
public sealed class RibbonTabDisposeTests : FlareTestContext
{
    public RibbonTabDisposeTests() => Services.AddFlareIde();

    private const string UiModule = "./_content/Flare.Components/js/flare-ui.js";

    private sealed class RibbonHost : ComponentBase
    {
        [Parameter] public bool ShowSecond { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FlareRibbon>(0);
            builder.AddAttribute(1, "ChildContent", (RenderFragment)(content =>
            {
                content.OpenComponent<FlareRibbonTab>(0);
                content.AddAttribute(1, "Title", "First");
                content.CloseComponent();

                if (ShowSecond)
                {
                    content.OpenComponent<FlareRibbonTab>(2);
                    content.AddAttribute(3, "Title", "Second");
                    content.CloseComponent();
                }
            }));
            builder.CloseComponent();
        }
    }

    [Fact]
    public void RemovingTheActiveTab_ActivatesARemainingTab()
    {
        var cut = Render<RibbonHost>(p => p.Add(x => x.ShowSecond, true));
        cut.FindAll("[role=tab]")[1].Click();
        Assert.Equal("Second", Active(cut));

        cut.Render(p => p.Add(x => x.ShowSecond, false));

        Assert.Equal("First", Active(cut));
        Assert.NotEmpty(cut.FindAll("[role=tabpanel]"));
    }

    // The circuit is already gone when the page unloads, so the keyboard unregistration the ribbon does
    // on the way out can never succeed; it must not fault the disposal.
    [Fact]
    public async Task DisposingAfterTheCircuitDisconnected_DoesNotThrow()
    {
        var module = JSInterop.SetupModule(UiModule);
        module.SetupVoid("registerTabKeyboard", _ => true).SetVoidResult();
        module.SetupVoid("removeTabKeyboard", _ => true)
            .SetException(new JSDisconnectedException("circuit gone"));

        var cut = Render<RibbonHost>();
        cut.WaitForAssertion(() => Assert.Contains(module.Invocations,
            invocation => invocation.Identifier == "registerTabKeyboard"));

        await cut.FindComponent<FlareRibbon>().Instance.DisposeAsync();

        Assert.Contains(module.Invocations, invocation => invocation.Identifier == "removeTabKeyboard");
        Assert.NotEmpty(cut.FindAll("[role=tab]"));
    }

    private static string Active(IRenderedComponent<RibbonHost> cut) =>
        cut.Find("[role=tab][aria-selected=true]").TextContent.Trim();
}
