using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

// Cascade notifications reach subscribed descendants directly. Keep an unchanged fragment from
// separately walking all descendants, including plain text with no localization dependency.
internal sealed class FlareLocalizationContent : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public long Revision { get; set; }
    private long _renderedRevision = -1;
    protected override bool ShouldRender() => Revision != _renderedRevision;
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        _renderedRevision = Revision;
        builder.AddContent(0, ChildContent);
    }
}
