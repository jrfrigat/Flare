namespace Flare.Components.Tests.Accessibility;

// The region is clipped from the page but not from the accessibility tree; role and live mode are what
// make it announce, so both have to reach the element.
public class FlareVisuallyHiddenTests : FlareTestContext
{
    [Fact]
    public void RendersTheHiddenClassAndContent()
    {
        var cut = Render<FlareVisuallyHidden>(p => p.AddChildContent("Saved"));

        var span = cut.Find($"span.{Css.Classes.Visually.Hidden}");
        Assert.Equal("Saved", span.TextContent);
        Assert.Null(span.GetAttribute("role"));
        Assert.Null(span.GetAttribute("aria-live"));
    }

    [Fact]
    public void RoleAndLiveMode_ReachTheRegion()
    {
        var cut = Render<FlareVisuallyHidden>(p => p
            .Add(x => x.Role, "status")
            .Add(x => x.AriaLive, "polite")
            .AddChildContent("Three unread"));

        var span = cut.Find($"span.{Css.Classes.Visually.Hidden}");
        Assert.Equal("status", span.GetAttribute("role"));
        Assert.Equal("polite", span.GetAttribute("aria-live"));
    }

    [Fact]
    public void AdditionalAttributes_ReachTheRegion()
    {
        var cut = Render<FlareVisuallyHidden>(p => p
            .AddUnmatched("data-test", "value")
            .AddChildContent("x"));

        Assert.Equal("value", cut.Find($"span.{Css.Classes.Visually.Hidden}").GetAttribute("data-test"));
    }
}
