using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

// Geometry and the full accessible table change with chart inputs, independently of pointer details.
internal sealed class ChartRenderRegion : ComponentBase
{
    private long _renderedRevision = -1;

    [Parameter] public long Revision { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected override bool ShouldRender() => Revision != _renderedRevision;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        _renderedRevision = Revision;
        builder.AddContent(0, ChildContent);
    }
}
