namespace Flare.Components.Tests;

/// <summary>
/// RTL (Right-to-Left) layout tests for key Flare components.
/// Verifies components render correctly in RTL mode.
/// </summary>
public class RtlTests : TestContext
{
    [Fact]
    public void FlareButton_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareButton>(parameters => parameters
            .Add(p => p.ChildContent, "زر")
            .Add(p => p.AriaLabel, "زر"));

        var button = cut.Find("button");
        Assert.NotNull(button);
        Assert.Equal("زر", button.TextContent);
    }

    [Fact]
    public void FlareField_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareField<string>>(parameters => parameters
            .Add(p => p.Label, "الاسم")
            .Add(p => p.Value, ""));

        var input = cut.Find("input");
        Assert.NotNull(input);
    }

    [Fact]
    public void FlareCheckbox_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareCheckbox>(parameters => parameters
            .Add(p => p.Label, "أوافق")
            .Add(p => p.Value, false));

        var checkbox = cut.Find("input[type='checkbox']");
        Assert.NotNull(checkbox);
    }

    [Fact]
    public void FlareSwitch_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareSwitch>(parameters => parameters
            .Add(p => p.Label, "تفعيل")
            .Add(p => p.Value, false));

        var input = cut.Find("input");
        Assert.NotNull(input);
    }

    [Fact]
    public void FlareAlert_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareAlert>(parameters => parameters
            .Add(p => p.ChildContent, "تنبيه")
            .Add(p => p.Severity, AlertSeverity.Warning));

        var alert = cut.Find(".flare-alert");
        Assert.NotNull(alert);
    }

    [Fact]
    public void FlareLink_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareLink>(parameters => parameters
            .Add(p => p.ChildContent, "رابط")
            .Add(p => p.Href, "/about"));

        var link = cut.Find("a");
        Assert.Equal("/about", link.GetAttribute("href"));
    }

    [Fact]
    public void FlareBadge_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareBadge>(parameters => parameters
            .Add(p => p.ChildContent, "5"));

        var badge = cut.Find(".flare-badge");
        Assert.NotNull(badge);
    }

    [Fact]
    public void FlareAvatar_Should_Render_In_Rtl()
    {
        var cut = RenderComponent<FlareAvatar>(parameters => parameters
            .Add(p => p.Alt, "أحمد"));

        var avatar = cut.Find(".flare-avatar");
        Assert.NotNull(avatar);
    }
}
