using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components;

// Slot visuals and local focus determine the standard markup independently of the event list.
internal sealed class SchedulerSlotRegion : ComponentBase
{
    [Parameter] public object? Visuals { get; set; }
    [Parameter] public int FocusedSlot { get; set; }
    [Parameter] public bool Editable { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private object? _renderedVisuals;
    private int _renderedFocus;
    private bool _renderedEditable;

    protected override bool ShouldRender() => Visuals is null ||
        !ReferenceEquals(Visuals, _renderedVisuals) || FocusedSlot != _renderedFocus || Editable != _renderedEditable;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        _renderedVisuals = Visuals;
        _renderedFocus = FocusedSlot;
        _renderedEditable = Editable;
        builder.AddContent(0, ChildContent);
    }
}
