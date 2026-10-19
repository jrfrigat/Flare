using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareMenuDisposalTests : FlareTestContext
{
    private readonly RecordingJsRuntime _runtime;

    public FlareMenuDisposalTests()
    {
        _runtime = new RecordingJsRuntime(JSInterop.JSRuntime);
        Services.AddScoped<IOverlayJsService>(_ => new OverlayJsService(_runtime));
    }

    private IEnumerable<object?> LayerIds(string id) => _runtime.Invocations
        .Where(i => i.Id == id && i.Args is { Length: > 0 })
        .Select(i => i.Args![0]);

    private static async Task OpenMenuAsync(IRenderedComponent<MenuHost> cut)
        => await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click(new MouseEventArgs { Detail = 1 }));

    private static void SetShown(IRenderedComponent<MenuHost> cut, bool shown)
        => cut.Render(p => p.Add(x => x.ShowMenu, shown));

    [Fact]
    public async Task UnmountingAnOpenMenu_RemovesTheAnchoredPanel()
    {
        var cut = Render<MenuHost>();
        await OpenMenuAsync(cut);
        cut.WaitForAssertion(() => Assert.Single(LayerIds("positionAnchoredPanel")));
        var layerId = LayerIds("positionAnchoredPanel").Single();

        SetShown(cut, false);

        cut.WaitForAssertion(() => Assert.Contains(layerId, LayerIds("removeAnchoredPanel")));
    }

    [Fact]
    public async Task RepeatedMountCycles_EachReleaseTheirOwnPanel()
    {
        var cut = Render<MenuHost>();
        var ids = new List<object?>();

        for (var i = 0; i < 3; i++)
        {
            await OpenMenuAsync(cut);
            cut.WaitForAssertion(() => Assert.Equal(i + 1, LayerIds("positionAnchoredPanel").Count()));
            ids.Add(LayerIds("positionAnchoredPanel").Last());
            SetShown(cut, false);
            cut.WaitForAssertion(() => Assert.Contains(ids[^1], LayerIds("removeAnchoredPanel")));
            SetShown(cut, true);
        }

        Assert.Equal(3, ids.Distinct().Count());
        Assert.All(ids, id => Assert.Contains(id, LayerIds("removeAnchoredPanel")));
    }

    [Fact]
    public async Task UnmountingAClosedMenu_DoesNotRemoveAPanelAgain()
    {
        var cut = Render<MenuHost>();
        await OpenMenuAsync(cut);
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Backdrop}").Click());
        cut.WaitForAssertion(() => Assert.Single(LayerIds("removeAnchoredPanel")));

        SetShown(cut, false);

        Assert.Single(LayerIds("removeAnchoredPanel"));
    }

    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private static RenderFragment OneItem() => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Save")));
        b.CloseComponent();
    };

    public sealed class MenuHost : ComponentBase
    {
        [Parameter] public bool ShowMenu { get; set; } = true;

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            if (!ShowMenu) return;
            b.OpenComponent<FlareMenu>(0);
            b.AddAttribute(1, nameof(FlareMenu.Activator), ActivatorButton());
            b.AddAttribute(2, nameof(FlareMenu.ChildContent), OneItem());
            b.CloseComponent();
        }
    }
}
