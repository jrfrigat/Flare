using Flare.Components.Scheduler;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Flare.Components;

public partial class FlareScheduler<TItem>
{
    private Microsoft.Extensions.Localization.IStringLocalizer<SchedulerStrings> SchedulerText => Localizer<SchedulerStrings>();
    [Inject] private TimeProvider Clock { get; set; } = default!;
    [Inject] private Services.IUiJsService Ui { get; set; } = default!;
    private ElementReference _root;
    private ElementReference _viewport;
    private DateOnly _date, _dateParameter;
    private SchedulerView _view, _viewParameter;
    private bool _initialized, _loading, _disposed;
    private Exception? _error;
    private int _generation, _version;
    private CancellationTokenSource? _load;
    private SchedulerWindow _window = default!;
    private IReadOnlyList<SchedulerOccurrence<TItem>> _occurrences = [];
    private string? _announcementKey;
    private string _announcement => _announcementKey is null ? "" : SchedulerText[_announcementKey].Value;
    private object? _loadKey;
    protected override string ComponentCssClass => Css.Classes.Scheduler.Root;

    private string Caption => _view == SchedulerView.Year ? _date.Year.ToString(EffectiveCulture)
        : _view == SchedulerView.Day ? _date.ToString("D", EffectiveCulture)
        : _date.ToString("Y", EffectiveCulture);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) await Ui.RegisterTreeKeyboardAsync(_root);
    }

    protected override async Task OnParametersSetAsync()
    {
        ArgumentNullException.ThrowIfNull(Mapping);
        ArgumentNullException.ThrowIfNull(TimeZone);
        if (SlotDuration < TimeSpan.FromMinutes(5) || SlotDuration > TimeSpan.FromDays(1)
            || TimeSpan.TicksPerDay % SlotDuration.Ticks != 0)
            throw new ArgumentOutOfRangeException(nameof(SlotDuration));
        if (WorkDays.Count == 0 || WorkDays.Any(day => !Enum.IsDefined(day)))
            throw new ArgumentException("WorkDays must contain valid weekdays.", nameof(WorkDays));
        if (!Enum.IsDefined(View)) throw new ArgumentOutOfRangeException(nameof(View));
        if (!_initialized || DateChanged.HasDelegate || Date != _dateParameter)
            _date = Date == default ? DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(Clock.GetUtcNow(), TimeZone).DateTime) : Date;
        if (!_initialized || ViewChanged.HasDelegate || View != _viewParameter) _view = View;
        _dateParameter = Date;
        _viewParameter = View;
        _initialized = true;
        var window = SchedulerClock.Window(SchedulerWindows.Dates(_date, _view, EffectiveCulture), TimeZone);
        var key = (window, Events, EventsProvider, Mapping, RecurrenceExpander, MaximumOccurrences, MaximumEvaluationSteps);
        if (!Equals(key, _loadKey)) { _loadKey = key; await LoadAsync(window); }
    }

    /// <summary>Reloads the current window, for example after application persistence completes.</summary>
    public Task ReloadAsync() => LoadAsync(SchedulerClock.Window(SchedulerWindows.Dates(_date, _view, EffectiveCulture), TimeZone));

    private async Task LoadAsync(SchedulerWindow window)
    {
        if (_disposed) return;
        _load?.Cancel();
        _load?.Dispose();
        var cancellation = _load = new CancellationTokenSource();
        var generation = ++_generation;
        _window = window;
        _loadKey = (window, Events, EventsProvider, Mapping, RecurrenceExpander, MaximumOccurrences, MaximumEvaluationSteps);
        _occurrences = [];
        _loading = true;
        _error = null;
        try
        {
            var pending = EventsProvider?.Invoke(new(window, cancellation.Token));
            if (pending is { IsCompleted: false }) StateHasChanged();
            var events = pending is not null ? await pending : Events ?? [];
            if (cancellation.IsCancellationRequested || generation != _generation || _disposed) return;
            _occurrences = SchedulerOccurrences.Resolve(events, Mapping, window, MaximumOccurrences,
                cancellation.Token, MaximumEvaluationSteps, RecurrenceExpander);
            if (SelectedEvent is { } selected) SelectedEvent = _occurrences.FirstOrDefault(o => o.Identity == selected.Identity);
            _version++;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
        catch (Exception error)
        {
            if (generation != _generation || _disposed) return;
            _error = error;
            await OnLoadError.InvokeAsync(error);
        }
        finally
        {
            if (generation == _generation && !_disposed) { _loading = false; StateHasChanged(); }
        }
    }

    /// <summary>Navigates by one projection window in either direction.</summary>
    /// <param name="direction">Negative for previous, positive for next; zero keeps the date.</param>
    public Task NavigateAsync(int direction) => GoToAsync(SchedulerWindows.Advance(_date, _view, Math.Sign(direction)));
    /// <summary>Navigates to today's date in the display zone.</summary>
    public Task TodayAsync() => GoToAsync(DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(Clock.GetUtcNow(), TimeZone).DateTime));
    /// <summary>Requests a date and reloads the accepted navigation window.</summary>
    /// <param name="date">Requested navigation date.</param>
    public async Task GoToAsync(DateOnly date)
    {
        _date = date;
        await DateChanged.InvokeAsync(date);
        if (DateChanged.HasDelegate) _date = Date;
        await ReloadAsync();
    }
    /// <summary>Requests a projection and reloads its window.</summary>
    /// <param name="view">Requested built-in or templated projection.</param>
    public async Task ShowViewAsync(SchedulerView view)
    {
        if (!Enum.IsDefined(view)) throw new ArgumentOutOfRangeException(nameof(view));
        _view = view;
        await ViewChanged.InvokeAsync(view);
        if (ViewChanged.HasDelegate) _view = View;
        await ReloadAsync();
    }
    /// <summary>Selects an event through the shared callback boundary.</summary>
    /// <param name="occurrence">Visible occurrence.</param>
    public async Task SelectEventAsync(SchedulerOccurrence<TItem> occurrence)
    {
        SelectedEvent = occurrence;
        await OnEventSelected.InvokeAsync(occurrence);
    }
    /// <summary>Selects a timed or all-day slot through the shared callback boundary.</summary>
    /// <param name="slot">Selected half-open span.</param>
    public async Task SelectSlotAsync(SchedulerSlot slot)
    {
        SelectedEvent = null;
        await OnSlotSelected.InvokeAsync(slot);
        if (Editable) await CreateEventAsync(slot);
    }
    /// <summary>Cancels pending provider work before disposing component services.</summary>
    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _generation++;
        _load?.Cancel();
        _load?.Dispose();
        if (_editorReference is { } editor) await editor.CancelAsync();
        try { await Ui.RemoveTreeKeyboardAsync(_root); }
        catch (JSDisconnectedException) { }
        catch (JSException) { }
        await base.DisposeAsync();
    }
}
