using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// Blazor calls <see cref="IAsyncDisposable.DisposeAsync"/> alone when a component implements both disposal
/// contracts, and <see cref="FlareComponentBase"/> implements the asynchronous one. A derived component that kept
/// its cleanup in <see cref="IDisposable.Dispose"/> was therefore never cleaned up. The base forwards to the shorter
/// contract; these tests pin that contract.
/// </summary>
public sealed class FlareComponentBaseDisposeTests : FlareTestContext
{
    private sealed class DisposableComponent : FlareComponentBase, IDisposable
    {
        public bool Disposed { get; private set; }

        protected override string ComponentCssClass => "dispose-test";

        public void Dispose() => Disposed = true;

        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, "probe");
    }

    // Renders the probe while Show is true, so removing it from the tree is a real component disposal.
    private sealed class Host : ComponentBase
    {
        [Parameter] public bool Show { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!Show) return;
            builder.OpenComponent<DisposableComponent>(0);
            builder.CloseComponent();
        }
    }

    [Fact]
    public void RemovingADisposableComponent_CallsDispose()
    {
        var cut = Render<Host>(p => p.Add(x => x.Show, true));
        var component = cut.FindComponent<DisposableComponent>().Instance;

        cut.Render(p => p.Add(x => x.Show, false));

        Assert.True(component.Disposed,
            "Blazor disposes only the asynchronous contract; FlareComponentBase.DisposeAsync must forward to Dispose.");
    }

    [Fact]
    public async Task DisposeAsync_OnItsOwn_CallsDispose()
    {
        var component = new DisposableComponent();

        await component.DisposeAsync();

        Assert.True(component.Disposed);
    }
}
