using Flare.Components.Tests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

// ------------------------------------------------------------------------------
// Button-family audit follow-ups: FocusAsync across the family, ButtonEdge,
// ToggleButton Toggle()/SetToggledAsync(), ToggleGroup Mandatory + CheckMark,
// and the FAB OnClick signature alignment.
// ------------------------------------------------------------------------------

public class C_FlareButtonFamilyAuditTests : FlareTestContext
{
    // ---- FlareIconButton ----

    [Fact]
    public void IconButton_EdgeStart_AddsEdgeClassToInnerButton()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.Add).Add(x => x.Edge, ButtonEdge.Start).Add(x => x.AriaLabel, "add"));

        Assert.Contains("flare-edge-start", cut.Find("button").ClassName);
    }

    [Fact]
    public void IconButton_BlankTargetLink_DefaultsRelNoopener()
    {
        var cut = Render<FlareIconButton>(p => p
            .Add(x => x.Icon, FlareIcons.OpenInNew).Add(x => x.Href, "https://example.com")
            .Add(x => x.Target, "_blank").Add(x => x.AriaLabel, "open"));

        Assert.Equal("noopener noreferrer", cut.Find("a").GetAttribute("rel"));
    }

    [Fact]
    public async Task IconButton_FocusAsync_DoesNotThrow()
    {
        var cut = Render<FlareIconButton>(p => p.Add(x => x.Icon, FlareIcons.Add).Add(x => x.AriaLabel, "add"));
        await cut.Instance.FocusAsync();
    }

    // ---- FlareToggleButton ----

    [Fact]
    public void ToggleButton_EdgeEnd_AddsEdgeClass()
    {
        var cut = Render<FlareToggleButton>(p => p.Add(x => x.Edge, ButtonEdge.End).AddChildContent("B"));
        Assert.Contains("flare-edge-end", cut.Find("button").ClassName);
    }

    [Fact]
    public async Task ToggleButton_Toggle_FlipsPressed()
    {
        var cut = Render<FlareToggleButton>(p => p.AddChildContent("B"));

        await cut.InvokeAsync(() => cut.Instance.Toggle());
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));

        await cut.InvokeAsync(() => cut.Instance.Toggle());
        Assert.Equal("false", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public async Task ToggleButton_SetToggledAsync_SetsPressed()
    {
        var cut = Render<FlareToggleButton>(p => p.AddChildContent("B"));

        await cut.InvokeAsync(() => cut.Instance.SetToggledAsync(true));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
    }

    // ---- FlareToggleGroup ----

    [Fact]
    public void ToggleGroup_Mandatory_KeepsSelectionOnReclick()
    {
        string? val = "a";
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Value, val)
            .Add(x => x.Mandatory, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => val = v))
            .AddChildContent<FlareToggleButton>(cp => cp.Add(b => b.Value, "a").AddChildContent("A")));

        cut.Find("button").Click();

        Assert.Equal("a", val); // clicking the active item does not deselect it
    }

    [Fact]
    public void ToggleGroup_SingleSelect_DeselectsOnReclick()
    {
        string? val = "a";
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Value, val)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => val = v))
            .AddChildContent<FlareToggleButton>(cp => cp.Add(b => b.Value, "a").AddChildContent("A")));

        cut.Find("button").Click();

        Assert.Null(val); // default single-select is deselectable
    }

    [Fact]
    public void ToggleGroup_CheckMark_RendersCheckOnSelected()
    {
        var cut = Render<FlareToggleGroup<string>>(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.CheckMark, true)
            .AddChildContent<FlareToggleButton>(cp => cp.Add(b => b.Value, "a").AddChildContent("A")));

        Assert.Contains("flare-toggle-btn__check", cut.Markup);
    }

    // ---- FlareSplitButton ----

    [Fact]
    public async Task SplitButton_FocusAsync_DoesNotThrow()
    {
        var cut = Render<FlareSplitButton>(p => p.AddChildContent("Save"));
        await cut.InvokeAsync(() => cut.Instance.FocusAsync());
    }

    // ---- FlareFloatingActionButton ----

    [Fact]
    public void Fab_OnClick_ReceivesMouseEvent()
    {
        var clicked = false;
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "add")
            .Add(x => x.Position, FabPosition.Static)
            .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true)));

        cut.Find("button").Click();

        Assert.True(clicked);
    }

    [Fact]
    public async Task Fab_FocusAsync_DoesNotThrow()
    {
        var cut = Render<FlareFloatingActionButton>(p => p
            .Add(x => x.AriaLabel, "add").Add(x => x.Position, FabPosition.Static));
        await cut.InvokeAsync(() => cut.Instance.FocusAsync());
    }
}
