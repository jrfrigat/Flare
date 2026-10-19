using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// A menu item that is a link must activate like one from the keyboard too: the real anchor is clicked,
/// so <c>Target</c>, <c>rel</c> and <c>download</c> reach the browser instead of being dropped by a
/// router navigation that has no target.
/// </summary>
public class FlareMenuItemTargetTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;

    public FlareMenuItemTargetTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IElementJsService>(_ => new ElementJsService(_runtime));
    }

    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private IRenderedComponent<FlareMenu> OpenMenu(RenderFragment content) => Render<FlareMenu>(p => p
        .Add(m => m.Activator, ActivatorButton())
        .Add(m => m.ChildContent, content));

    private static async Task ActivateFirstItemAsync(IRenderedComponent<FlareMenu> cut)
    {
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(new MouseEventArgs { Detail = 0 }));
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Panel}").KeyDown(new KeyboardEventArgs { Key = "Enter" }));
    }

    private RenderFragment LinkItem(string href, string target, Action onClick) => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.Href), href);
        b.AddAttribute(2, nameof(FlareMenuItem.Target), target);
        b.AddAttribute(3, nameof(FlareMenuItem.OnClick), EventCallback.Factory.Create(this, onClick));
        b.AddAttribute(4, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Report")));
        b.CloseComponent();
    };

    [Fact]
    public async Task KeyboardActivationOfALink_ClicksTheNativeAnchor()
    {
        var cut = OpenMenu(LinkItem("https://example.com/report", "_blank", () => { }));

        await ActivateFirstItemAsync(cut);

        cut.WaitForAssertion(() => Assert.Contains("flareDom.click", _runtime.Calls));
        // The router was not used: it would have replaced the current URI and dropped Target.
        Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
    }

    [Fact]
    public async Task KeyboardActivationOfAnAction_StaysOnTheManagedPath()
    {
        var clicked = 0;
        var cut = OpenMenu(b =>
        {
            b.OpenComponent<FlareMenuItem>(0);
            b.AddAttribute(1, nameof(FlareMenuItem.OnClick), EventCallback.Factory.Create(this, () => clicked++));
            b.AddAttribute(2, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Run")));
            b.CloseComponent();
        });

        await ActivateFirstItemAsync(cut);

        Assert.Equal(1, clicked);
        Assert.DoesNotContain("flareDom.click", _runtime.Calls);
    }
}
