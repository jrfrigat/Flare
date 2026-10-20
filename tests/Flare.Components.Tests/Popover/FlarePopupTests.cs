using Flare.Abstractions;
using Flare.Components;
using Flare.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

// FlarePopup is the select family's panel: the consumer's classes, style and attributes belong on the
// rendered panel, and a placement option that changes while the panel is open reaches the engine
// without a close and a reopen.
public sealed class FlarePopupTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;

    public FlarePopupTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    [Fact]
    public void StyleClassAndAttributesLandOnThePanel()
    {
        var cut = Render<FlarePopup>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Class, "custom-popup")
            .Add(x => x.Style, "color:red")
            .AddUnmatched("data-testid", "popup")
            .AddChildContent("<span id=\"inside\">option</span>"));

        var panel = cut.Find(".custom-popup");
        Assert.Equal("color:red", panel.GetAttribute("style"));
        Assert.Equal("popup", panel.GetAttribute("data-testid"));
        Assert.NotNull(cut.Find("#inside"));
    }

    [Fact]
    public void ChangingMatchWidthWhileOpen_RePlacesWithoutClosing()
    {
        var cut = Render<FlarePopup>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.MatchWidth, false)
            .AddChildContent("<span>option</span>"));
        cut.WaitForAssertion(() =>
            Assert.Contains(_runtime.Invocations, i => i.Id == "positionAnchoredPanel"));

        cut.Render(p => p
            .Add(x => x.Open, true)
            .Add(x => x.MatchWidth, true)
            .AddChildContent("<span>option</span>"));

        cut.WaitForAssertion(() =>
            Assert.Equal(2, _runtime.Invocations.Count(i => i.Id == "positionAnchoredPanel")));
        Assert.DoesNotContain("removeAnchoredPanel", _runtime.Calls);
    }
}
