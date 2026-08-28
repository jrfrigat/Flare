using Microsoft.AspNetCore.Components;

namespace Flare.Components;

/// <summary>
/// Base class for a component that drives a <see cref="FlareDataGrid{TItem}"/> from outside it: a pager,
/// a column picker, a filter bar, a query editor, a status line - anything, including components Flare
/// does not ship. It resolves which grid it belongs to, subscribes to that grid's changes, re-renders
/// itself when one it cares about arrives, and unsubscribes on dispose.
/// </summary>
/// <remarks>
/// <para>
/// Binding, in precedence order: an explicit <see cref="Context"/>, an explicit <see cref="Grid"/>, a
/// cascaded <see cref="DataGridContext{TItem}"/>, then the cascaded grid. So the same control works
/// inside the grid's <c>ToolbarContent</c> with no attributes at all, and anywhere else on the page with
/// <c>Context="@_ctx"</c> or <c>Grid="@_grid"</c>.
/// </para>
/// <para>
/// Narrow <see cref="Observes"/> to the change kinds the control actually displays. A pager that
/// re-renders on every keystroke of somebody else's filter box is the cost this exists to avoid.
/// </para>
/// </remarks>
/// <typeparam name="TItem">Row type of the grid this control drives.</typeparam>
public abstract class FlareDataGridControl<TItem> : ComponentBase, IDisposable
{
    private DataGridContext<TItem>? _subscribed;

    [CascadingParameter] private DataGridContext<TItem>? CascadedContext { get; set; }

    [CascadingParameter] private FlareDataGrid<TItem>? CascadedGrid { get; set; }

    /// <summary>The grid context this control drives. Set it to place the control anywhere on the page;
    /// omit it inside a grid, where the cascade supplies one.</summary>
    [Parameter] public DataGridContext<TItem>? Context { get; set; }

    /// <summary>The grid this control drives, as an alternative to <see cref="Context"/> when the page
    /// already holds an <c>@ref</c> to the grid itself.</summary>
    [Parameter] public FlareDataGrid<TItem>? Grid { get; set; }

    /// <summary>The resolved context, or null when neither a context nor a grid could be found. Null is
    /// the normal state for one render when the control is declared above its grid.</summary>
    protected DataGridContext<TItem>? Owner =>
        Context ?? Grid?.ActiveContext ?? CascadedContext ?? CascadedGrid?.ActiveContext;

    /// <summary>True when a grid is resolved and attached, i.e. when reads return real values. A control
    /// that renders chrome should render nothing while this is false.</summary>
    protected bool HasGrid => Owner is { IsAttached: true };

    /// <summary>The change kinds this control re-renders for. Defaults to every kind; override it with
    /// the narrower set the control actually shows.</summary>
    protected virtual DataGridChange Observes => DataGridChange.All;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var owner = Owner;
        if (ReferenceEquals(_subscribed, owner)) return;
        if (_subscribed is not null) _subscribed.Changed -= OnGridChanged;
        _subscribed = owner;
        if (_subscribed is not null) _subscribed.Changed += OnGridChanged;
    }

    /// <summary>Called after a change this control observes. The default re-renders; override to react
    /// without rendering, and call the base to keep the re-render.</summary>
    /// <param name="change">The change kinds that occurred, already masked by <see cref="Observes"/>.</param>
    protected virtual void OnGridStateChanged(DataGridChange change) => InvokeAsync(StateHasChanged);

    private void OnGridChanged(DataGridChange change)
    {
        var observed = change & Observes;
        if (observed != DataGridChange.None) OnGridStateChanged(observed);
    }

    /// <summary>Unsubscribes from the bound context.</summary>
    public virtual void Dispose()
    {
        if (_subscribed is not null) _subscribed.Changed -= OnGridChanged;
        _subscribed = null;
        GC.SuppressFinalize(this);
    }
}
