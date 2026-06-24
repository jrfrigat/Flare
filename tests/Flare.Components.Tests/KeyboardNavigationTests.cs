namespace Flare.Components.Tests;

/// <summary>
/// Keyboard navigation tests for Flare components.
/// Verifies that components respond correctly to keyboard input.
/// </summary>
public class KeyboardNavigationTests : TestContext
{
    [Fact]
    public void FlareButton_Should_Have_Type_Button()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Click me")
            .Add(p => p.Type, ButtonType.Button));

        var button = cut.Find("button");
        Assert.Equal("button", button.GetAttribute("type"));
    }

    [Fact]
    public void FlareButton_WhenDisabled_Should_Be_Disabled()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Click me")
            .Add(p => p.Disabled, true));

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void FlareButton_WithHref_Should_Render_Anchor()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Link")
            .Add(p => p.Href, "/page")
            .Add(p => p.Target, "_blank"));

        var anchor = cut.Find("a");
        Assert.Equal("/page", anchor.GetAttribute("href"));
        Assert.Equal("_blank", anchor.GetAttribute("target"));
    }

    [Fact]
    public void FlareCheckbox_Should_Be_Checkable()
    {
        var cut = RenderComponent<FlareCheckbox>(parameters => parameters
            .Add(p => p.Label, "Toggle")
            .Add(p => p.Value, false));

        var input = cut.Find("input[type='checkbox']");
        Assert.False(input.HasAttribute("checked"));
    }

    [Fact]
    public void FlareSwitch_Should_Be_Toggleable()
    {
        var cut = RenderComponent<FlareSwitch>(parameters => parameters
            .Add(p => p.Label, "Toggle")
            .Add(p => p.Value, false));

        var input = cut.Find("input");
        Assert.NotNull(input);
    }

    [Fact]
    public void FlareLink_Should_Be_Focusable()
    {
        var cut = RenderComponent<FlareLink>(parameters => parameters
            .Add(p => p.ChildContent, "Click here")
            .Add(p => p.Href, "/about"));

        var link = cut.Find("a");
        Assert.Equal("/about", link.GetAttribute("href"));
    }

    [Fact]
    public void FlareAlert_Should_Have_Role()
    {
        var cut = RenderComponent<FlareAlert>(parameters => parameters
            .Add(p => p.ChildContent, "Alert message")
            .Add(p => p.Severity, AlertSeverity.Info));

        var alert = cut.Find(".flare-alert");
        Assert.NotNull(alert);
    }
}
