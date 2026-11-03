using Microsoft.AspNetCore.Components;

namespace Flare.Components.Scheduler;

/// <summary>Shared context boundary for custom scheduler projections. It contains no layout or gesture engine.</summary>
/// <typeparam name="TItem">Application event type.</typeparam>
public abstract class FlareSchedulerViewBase<TItem> : FlareComponentBase
{
    /// <inheritdoc />
    protected override string ComponentCssClass => string.Empty;
    /// <summary>Snapshot supplied by the scheduler or by its ViewTemplate.</summary>
    [Parameter, EditorRequired] public SchedulerViewContext<TItem> Context { get; set; } = default!;
}
