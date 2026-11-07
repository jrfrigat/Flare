using Microsoft.AspNetCore.Components;

namespace Flare.Components.Scheduler;

/// <summary>Shared context boundary for custom scheduler projections. It contains no layout or gesture engine.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public abstract class FlareSchedulerViewBase<TItem> : FlareLocalizedComponentBase
{
    /// <inheritdoc />
    protected override string ComponentCssClass => string.Empty;
    /// <summary>Loaded snapshot supplied by any scheduling scope or custom composition.</summary>
    [Parameter, EditorRequired] public SchedulerViewContext<TItem> Context { get; set; } = default!;
    /// <summary>Replaces event visuals inside the retained geometry and gesture boundary. Own child activation and keyboard actions; interactive children are not nested in a button.</summary>
    [Parameter] public RenderFragment<SchedulerEventContext<TItem>>? EventTemplate { get; set; }
}
