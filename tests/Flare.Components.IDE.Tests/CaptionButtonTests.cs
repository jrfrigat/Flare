using Flare.Components.IDE;
using Flare.Icons;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// The title-bar caption control: one native button carrying the kind marker, the optional danger
/// class, the host's label and glyph, and the two callbacks. The visual states live in the public
/// <c>--flare-caption-*</c> tokens and are read from the browser suite; these tests pin the markup
/// and the command contract.
/// </summary>
public class CaptionButtonTests : FlareTestContext
{
    public CaptionButtonTests() => Services.AddFlareIde();

    [Theory]
    [InlineData(CaptionButtonKind.Minimize, "minimize", false)]
    [InlineData(CaptionButtonKind.Maximize, "maximize", false)]
    [InlineData(CaptionButtonKind.Restore, "restore", false)]
    [InlineData(CaptionButtonKind.Close, "close", true)]
    public void Kind_RendersTheSemanticMarkerAndTheDangerClass(
        CaptionButtonKind kind, string marker, bool danger)
    {
        var cut = Render<FlareCaptionButton>(p => p
            .Add(x => x.Kind, kind)
            .Add(x => x.AriaLabel, "Window command")
            .Add(x => x.Tooltip, "Window command")
            .Add(x => x.Icon, FlareIcons.Close));

        var button = cut.Find("button");

        Assert.Equal(marker, button.GetAttribute("data-flare-caption"));
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.Equal("Window command", button.GetAttribute("aria-label"));
        Assert.Equal("Window command", button.GetAttribute("title"));
        Assert.Equal(danger, button.ClassList.Contains(Css.Classes.Ide.CaptionButton.Close));
        Assert.Contains(Css.Classes.Ide.CaptionButton.Root, button.ClassList);
    }

    [Fact]
    public void DefaultKind_IsMinimize()
    {
        var cut = Render<FlareCaptionButton>();

        Assert.Equal("minimize", cut.Find("button").GetAttribute("data-flare-caption"));
    }

    [Fact]
    public void Disabled_RendersTheNativeAttribute()
    {
        var cut = Render<FlareCaptionButton>(p => p.Add(x => x.Disabled, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void Enabled_DoesNotRenderTheDisabledAttribute()
    {
        var cut = Render<FlareCaptionButton>();

        Assert.False(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public async Task OnClick_RaisesTheCallback()
    {
        MouseEventArgs? received = null;
        var cut = Render<FlareCaptionButton>(p => p
            .Add(x => x.OnClick, (MouseEventArgs e) => received = e));

        await cut.Find("button").ClickAsync(new MouseEventArgs());

        Assert.NotNull(received);
    }

    [Fact]
    public async Task OnMouseDown_RaisesTheCallback()
    {
        MouseEventArgs? received = null;
        var cut = Render<FlareCaptionButton>(p => p
            .Add(x => x.OnMouseDown, (MouseEventArgs e) => received = e));

        await cut.Find("button").TriggerEventAsync("onmousedown", new MouseEventArgs());

        Assert.NotNull(received);
    }

    [Fact]
    public void Icon_IsDrawnInTheCaptionGlyphSlot()
    {
        var cut = Render<FlareCaptionButton>(p => p.Add(x => x.Icon, FlareIcons.Remove));

        var slot = cut.Find($".{Css.Classes.Ide.CaptionButton.Icon}");
        Assert.Single(slot.QuerySelectorAll($".{Css.Classes.Icon.Root}"));
    }

    [Fact]
    public void ChildContent_ReplacesTheIcon()
    {
        var cut = Render<FlareCaptionButton>(p => p
            .Add(x => x.Icon, FlareIcons.Remove)
            .Add(x => x.ChildContent, b => b.AddMarkupContent(0, "<span id=\"custom-glyph\">x</span>")));

        Assert.NotNull(cut.Find("#custom-glyph"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Icon.Root}"));
    }

    [Fact]
    public void ClassAndStyle_AreForwardedToTheButton()
    {
        var cut = Render<FlareCaptionButton>(p => p
            .Add(x => x.Class, "host-caption")
            .Add(x => x.Style, "margin-inline-start: 0"));

        var button = cut.Find("button");

        Assert.Contains("host-caption", button.ClassList);
        Assert.Equal("margin-inline-start: 0", button.GetAttribute("style"));
    }
}
