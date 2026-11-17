using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareMenuHoverSubscriptionTests : FlareTestContext
{
    private static int HoverHandlers(string markup) =>
        markup.Split("blazor:onmouseenter", StringSplitOptions.None).Length - 1;

    private static RenderFragment Menu(string label) => builder =>
    {
        builder.OpenComponent<FlareMenu>(0);
        builder.AddAttribute(1, nameof(FlareMenu.ChildContent), (RenderFragment)(content =>
        {
            content.OpenComponent<FlareMenuItem>(0);
            content.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(text =>
                text.AddContent(0, label)));
            content.CloseComponent();
        }));
        builder.CloseComponent();
    };

    [Fact]
    public void OrdinaryMenuDoesNotSubscribeToHover()
    {
        var cut = Render<FlareMenu>();

        Assert.Equal(0, HoverHandlers(cut.Markup));
    }

    [Fact]
    public void MenuInABarSubscribesToHover()
    {
        var bar = new FlareMenuBarContext();
        var cut = Render<FlareMenu>(p => p.AddCascadingValue(bar));

        Assert.Equal(1, HoverHandlers(cut.Markup));
    }

    [Fact]
    public void RightClickMenuInABarDoesNotSubscribeToHover()
    {
        var bar = new FlareMenuBarContext();
        var cut = Render<FlareMenu>(p => p
            .AddCascadingValue(bar)
            .Add(x => x.Activation, MenuActivation.RightClick));

        Assert.Equal(0, HoverHandlers(cut.Markup));
    }

    [Fact]
    public async Task HoverSwitchesTheOpenMenuInABar()
    {
        var bar = new FlareMenuBarContext();
        var cut = Render<CascadingValue<FlareMenuBarContext>>(p => p
            .Add(x => x.Value, bar)
            .AddChildContent(b =>
            {
                b.AddContent(0, Menu("First"));
                b.AddContent(1, Menu("Second"));
            }));
        var activators = cut.FindAll($".{Css.Classes.Menu.Activator}");

        await activators[0].ClickAsync(new MouseEventArgs());
        cut.WaitForAssertion(() => Assert.Contains("First", cut.Markup));

        await activators[1].TriggerEventAsync("onmouseenter", new MouseEventArgs());

        cut.WaitForAssertion(() => Assert.Contains("Second", cut.Markup));
        Assert.DoesNotContain("First", cut.Markup);
    }
}
