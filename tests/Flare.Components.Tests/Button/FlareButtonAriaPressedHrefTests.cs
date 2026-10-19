namespace Flare.Components.Tests;

/// <summary>
/// The link branch of a button is still a button (role="button"), so it has to report the same toggle
/// state the native branch does; it used to omit aria-pressed entirely.
/// </summary>
public class FlareButtonAriaPressedHrefTests : FlareTestContext
{
    private IRenderedComponent<FlareButton> RenderLink(bool? pressed, bool disabled = false) =>
        Render<FlareButton>(p => p
            .Add(x => x.Href, "#target")
            .Add(x => x.AriaPressed, pressed)
            .Add(x => x.Disabled, disabled)
            .AddChildContent("Toggle"));

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void AHrefButton_CarriesTheToggleState(bool pressed, string expected)
    {
        var link = RenderLink(pressed).Find("a");

        Assert.Equal("button", link.GetAttribute("role"));
        Assert.Equal(expected, link.GetAttribute("aria-pressed"));
    }

    [Fact]
    public void AHrefButton_WithoutAState_HasNoAriaPressed()
    {
        Assert.Null(RenderLink(null).Find("a").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void ADisabledHrefButton_KeepsTheStateAndLosesTheHref()
    {
        var link = RenderLink(true, disabled: true).Find("a");

        Assert.Null(link.GetAttribute("href"));
        Assert.Equal("true", link.GetAttribute("aria-disabled"));
        Assert.Equal("true", link.GetAttribute("aria-pressed"));
    }
}
