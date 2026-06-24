namespace Flare.Components.Tests;

/// <summary>
/// Basic accessibility tests for key Flare components.
/// Verifies ARIA attributes, roles, and keyboard support.
/// </summary>
public class AccessibilityTests : TestContext
{
    [Fact]
    public void FlareButton_Should_RenderButtonElement()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Click me"));

        var button = cut.Find("button");
        Assert.NotNull(button);
        Assert.Equal("Click me", button.TextContent);
    }

    [Fact]
    public void FlareButton_WithAriaLabel_Should_HaveAriaLabel()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Click me")
            .Add(p => p.AriaLabel, "Primary action"));

        var button = cut.Find("button");
        Assert.Equal("Primary action", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void FlareButton_WhenDisabled_Should_HaveDisabledAttribute()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Click me")
            .Add(p => p.Disabled, true));

        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void FlareButton_WhenLoading_Should_HaveAriaBusy()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Click me")
            .Add(p => p.Loading, true));

        var button = cut.Find("button");
        Assert.Equal("true", button.GetAttribute("aria-busy"));
    }

    [Fact]
    public void FlareButton_WithHref_Should_RenderAsAnchor()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "Link")
            .Add(p => p.Href, "/page"));

        var anchor = cut.Find("a");
        Assert.Equal("/page", anchor.GetAttribute("href"));
    }

    [Fact]
    public void FlareCheckbox_Should_HaveAriaChecked()
    {
        var cut = RenderComponent<FlareCheckbox>(parameters => parameters
            .Add(p => p.Label, "Accept terms")
            .Add(p => p.Value, true));

        var input = cut.Find("input[type='checkbox']");
        Assert.Equal("true", input.GetAttribute("aria-checked"));
    }

    [Fact]
    public void FlareAlert_Should_Render()
    {
        var cut = RenderComponent<FlareAlert>(parameters => parameters
            .Add(p => p.ChildContent, "Warning message")
            .Add(p => p.Severity, AlertSeverity.Warning));

        var alert = cut.Find(".flare-alert");
        Assert.NotNull(alert);
    }

    [Fact]
    public void FlareSwitch_Should_Render()
    {
        var cut = RenderComponent<FlareSwitch>(parameters => parameters
            .Add(p => p.Label, "Enable feature")
            .Add(p => p.Value, false));

        var input = cut.Find("input");
        Assert.NotNull(input);
    }

    [Fact]
    public void FlareLink_Should_Render()
    {
        var cut = RenderComponent<FlareLink>(parameters => parameters
            .Add(p => p.ChildContent, "Click here")
            .Add(p => p.Href, "/about"));

        var link = cut.Find("a");
        Assert.Equal("/about", link.GetAttribute("href"));
    }
}
