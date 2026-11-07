using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

// The virtual scale can move without asking the consumer to render the panel composition again.
internal sealed class GanttScaleRegion : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    internal void Refresh() => StateHasChanged();

    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
}
