using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>Field for several dates, bound to a sorted list of <see cref="DateOnly"/> without repeats: days are toggled
/// in a calendar popup (or an inline calendar) on the culture's calendar, or typed as a list with <c>;</c> between
/// the dates.</summary>
public partial class FlareMultiDatePicker : IFlareMultiField<DateOnly>
{
    /// <summary>The selected dates, sorted and without repeats (supports <c>@bind-Values</c>, like the other
    /// multi-value fields). The component never changes the list it is given; a change comes back as a new list.</summary>
    [Parameter] public IReadOnlyList<DateOnly> Values { get; set; } = [];
    /// <summary>Raised with the new list when the user toggles, types or clears dates.</summary>
    [Parameter] public EventCallback<IReadOnlyList<DateOnly>> ValuesChanged { get; set; }
    /// <summary>Model field bound inside an <c>EditForm</c>: changes reach the edit context and its validation message is shown.</summary>
    [Parameter] public Expression<Func<IReadOnlyList<DateOnly>>>? For { get; set; }
    /// <summary>The most dates that can be selected; once reached, the other days are disabled until one is removed.
    /// Null (the default) sets no limit.</summary>
    [Parameter] public int? MaxCount { get; set; }
    /// <summary>The most dates the field lists before it shows their count instead. Default 3.</summary>
    [Parameter] public int MaxShownDates { get; set; } = 3;
    /// <summary>Earliest date that can be selected (inclusive); earlier days are disabled.</summary>
    [Parameter] public DateOnly? Min { get; set; }
    /// <summary>Latest date that can be selected (inclusive); later days are disabled.</summary>
    [Parameter] public DateOnly? Max { get; set; }
    /// <summary>Predicate that disables specific dates (return true to disable). Applied on top of Min/Max.</summary>
    [Parameter] public Func<DateOnly, bool>? IsDateDisabled { get; set; }
    /// <summary>Culture for the calendar and for writing and reading the dates. Default = CurrentUICulture. The
    /// calendar follows the culture's own (Persian for fa-IR, Um al-Qura for ar-SA).</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>The calendar to show and write dates on instead of the culture's own, for example a
    /// <see cref="GregorianCalendar"/> for fa-IR or a <see cref="HebrewCalendar"/> for he-IL. Only a calendar the
    /// culture offers among its optional calendars is used; any other is ignored. Null uses the culture's calendar.</summary>
    [Parameter] public Calendar? Calendar { get; set; }
    /// <summary>Format of each date in the field. Null (default) uses the culture's short date pattern.</summary>
    [Parameter] public string? DateFormat { get; set; }
    /// <summary>The calendar view the picker opens to (Day/Month/Year).</summary>
    [Parameter] public PickerOpenTo OpenTo { get; set; } = PickerOpenTo.Day;
    /// <summary>Allows typing the dates directly into the field, separated by <c>;</c>. Default true.</summary>
    [Parameter] public bool AllowInput { get; set; } = true;
    /// <summary>Allows opening the calendar popup (shows the calendar icon button). Default true.</summary>
    [Parameter] public bool AllowPicker { get; set; } = true;
    /// <summary>Renders the calendar inline (always visible under the field) rather than in a popup.</summary>
    [Parameter] public bool Inline { get; set; }
    /// <summary>Shows a leading week-of-year number column in the calendar.</summary>
    [Parameter] public bool ShowWeekNumbers { get; set; }
    /// <summary>Overrides the culture's first day of week when set (null = use the culture's).</summary>
    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }
    /// <summary>Returns extra CSS class(es) for a given day cell (e.g. to mark holidays).</summary>
    [Parameter] public Func<DateOnly, string>? DayClassFunc { get; set; }
    /// <summary>Content of a day cell; the picker keeps the cell itself - the button, its label, focus, disabled state
    /// and selection. Null shows the day number.</summary>
    [Parameter] public RenderFragment<DateOnly>? DayTemplate { get; set; }
    /// <summary>Shows the Clear button in the calendar footer. Default true.</summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;
    /// <summary>Override text for the Clear button. When null, falls back to the localizer key Picker_Clear.</summary>
    [Parameter] public string? ClearText { get; set; }
    /// <summary>Override text for the Done button that closes the popup. When null, falls back to Picker_Done.</summary>
    [Parameter] public string? DoneText { get; set; }
    /// <summary>Forces the error visual state without an error message (e.g. driven by external validation).</summary>
    [Parameter] public bool HasError { get; set; }
    /// <summary>Requests focus on the input after the first render (best-effort).</summary>
    [Parameter] public bool Autofocus { get; set; }
    /// <summary>Raised when the calendar popup opens.</summary>
    [Parameter] public EventCallback Opened { get; set; }
    /// <summary>Raised when the calendar popup closes.</summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.MultiDatePicker.Root;

    /// <summary>Opens the calendar popup.</summary>
    public Task OpenAsync() => SetOpenAsync(true);
    /// <summary>Closes the calendar popup.</summary>
    public Task CloseAsync() => SetOpenAsync(false);
    /// <summary>Clears every selected date.</summary>
    public Task ClearAsync() => ClearValue();
    /// <summary>Sets keyboard focus to the input.</summary>
    public ValueTask FocusAsync() => _inputEl.FocusAsync();

    private ElementReference _inputEl;
    private ElementReference _fieldEl;
    private ElementReference _panelEl;
    private ElementReference _toggleEl;
    private FlareMonthGrid? _grid;
    private PickerPopup? _popupState;
    private PickerPopup _popup => _popupState ??= new PickerPopup(Overlay, $"flare-multidatepicker-{Guid.NewGuid():N}");
    private PickerViews? _viewState;
    private PickerViews _views => _viewState ??= new PickerViews(() => _culture, () => Min, () => Max, () => Disabled);
    private bool _open;

    // A closed picker shows only its field: a parent re-render that passes the same values (and new callback
    // lambdas) leaves it as it is. Day predicates, templates and parsers are read while the calendar is shown.
    /// <inheritdoc />
    protected override bool SkipsUnchangedParameters => true;
    /// <inheritdoc />
    protected override bool DelegatesAffectRender => _open || Inline;
    private bool _autofocused;
    private bool _focused;
    private string _text = string.Empty;
    private IReadOnlyList<DateOnly>? _syncedValue;
    private string? _syncedText;
    private DateOnly? _focusedDate;

    private CultureInfo _culture => CalendarMath.PickerCulture(Culture ?? CultureInfo.CurrentUICulture, Calendar);
    private string _format => string.IsNullOrEmpty(DateFormat) ? _culture.DateTimeFormat.ShortDatePattern : DateFormat;
    private DateOnly Today => DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime);
    private IReadOnlyList<DateOnly> _dates => Values ?? [];
    private DateOnly FocusedCursor => _focusedDate ?? (_dates.Count > 0 ? _dates[0] : Today);
    private bool _invalid => HasError || !string.IsNullOrEmpty(DisplayedErrorText);
    private bool _full => MaxCount is { } max && _dates.Count >= max;

    // Up to MaxShownDates dates written out, more as their count.
    private string DisplayValue => _dates.Count == 0 ? string.Empty
        : _dates.Count > MaxShownDates ? string.Format(_culture, FlareStrings.MultiDatePicker_Count, _dates.Count)
        : EditText;

    // Every date written out, as the field edits them.
    private string EditText => string.Join("; ", _dates.Select(d => CalendarMath.FormatSafe(d, _format, _culture)));

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _views.ShowMonthOf(_dates.Count > 0 ? _dates[0] : Today);
        _views.View = InitialView;
    }

    private PickerView InitialView => OpenTo switch
    {
        PickerOpenTo.Month => PickerView.Month,
        PickerOpenTo.Year => PickerView.Year,
        _ => PickerView.Day,
    };

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        UpdateFieldIdentifier(For);
        var changed = !SameDates(Values, _syncedValue);
        // A list set from outside shows the month of its first new date; a re-render keeps the month browsed to.
        if (changed && _dates.FirstOrDefault(d => _syncedValue?.Contains(d) != true) is { } first && first != default)
            _views.ShowMonthOf(first);
        var display = DisplayValue;
        if (changed || (!_focused && display != _syncedText))
        {
            _syncedValue = Values;
            _syncedText = display;
            _text = _focused ? EditText : display;
        }
    }

    private static bool SameDates(IReadOnlyList<DateOnly>? a, IReadOnlyList<DateOnly>? b) =>
        ReferenceEquals(a, b) || (a ?? []).SequenceEqual(b ?? []);

    private bool IsSelected(DateOnly d) => _dates.Contains(d);

    private bool IsDisabled(DateOnly d) =>
        (Min is { } min && d < min) || (Max is { } max && d > max) || (IsDateDisabled?.Invoke(d) ?? false);

    // A locked field makes every day unselectable; a full one only the days not selected, so they can be removed.
    private bool IsDayUnavailable(DateOnly d) => Disabled || ReadOnly || IsDisabled(d) || (_full && !IsSelected(d));

    private string ComposeDayClass(DateOnly d)
    {
        var selected = IsSelected(d) ? Css.Classes.Picker.DaySelected : string.Empty;
        var extra = DayClassFunc?.Invoke(d);
        if (string.IsNullOrEmpty(extra)) return selected;
        return string.IsNullOrEmpty(selected) ? extra : $"{selected} {extra}";
    }

    private async Task SetOpenAsync(bool open)
    {
        if (Inline || _open == open || (open && (Disabled || ReadOnly))) return;
        _open = open;
        if (open)
        {
            _views.ShowMonthOf(_dates.Count > 0 ? _dates[0] : Today);
            _views.View = InitialView;
            await Opened.InvokeAsync();
        }
        else await Closed.InvokeAsync();
        StateHasChanged();
    }

    private Task Toggle() => SetOpenAsync(!_open);
    private Task Close() => SetOpenAsync(false);

    private async Task ToggleDay(DateOnly day)
    {
        if (Disabled || ReadOnly || IsDisabled(day)) return;
        if (IsSelected(day)) await CommitAsync(_dates.Where(d => d != day));
        else if (!_full) await CommitAsync(_dates.Append(day));
    }

    private async Task ClearValue()
    {
        if (Disabled || ReadOnly) return;
        await CommitAsync([]);
    }

    private async Task CommitAsync(IEnumerable<DateOnly> dates)
    {
        IReadOnlyList<DateOnly> next = dates.Distinct().Order().ToArray();
        _syncedValue = next;
        await ValuesChanged.InvokeAsync(next);
        NotifyFieldChanged();
    }

    private async Task HandleGridKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape") { await ClosePopupAsync(); return; }
        if (Disabled || ReadOnly) return;
        // Enter/Space are left to the native click of the focused day button (TASK-124).
        var next = CalendarMath.KeyTarget(FocusedCursor, e.Key, e.ShiftKey,
            FirstDayOfWeek ?? _culture.DateTimeFormat.FirstDayOfWeek, IsDisabled, _views.Grid);
        if (next is not { } day) return;
        _focusedDate = day;
        if (!_views.Shown.Contains(day)) _views.ShowMonthOf(day);
    }

    private async Task ClosePopupAsync()
    {
        if (!_open) return;
        _popup.ReturnToField();
        await SetOpenAsync(false);
    }

    private Task HandlePanelKeyDown(KeyboardEventArgs e) =>
        !Inline && e.Key == "Escape" ? ClosePopupAsync() : Task.CompletedTask;

    private void HandleFocus()
    {
        _focused = true;
        _text = EditText;
    }

    private void HandleInput(ChangeEventArgs e) => _text = e.Value?.ToString() ?? string.Empty;

    // The typed list is committed only whole: every date must parse and be available, within MaxCount.
    private async Task HandleTextChange(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString()?.Trim() ?? string.Empty;
        var parts = raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var dates = new List<DateOnly>();
        foreach (var part in parts)
        {
            if (!DateOnly.TryParseExact(part, _format, _culture, DateTimeStyles.None, out var d)
                && !DateOnly.TryParse(part, _culture, DateTimeStyles.None, out d)) { _text = DisplayValue; return; }
            if (IsDisabled(d)) { _text = DisplayValue; return; }
            dates.Add(d);
        }
        if (MaxCount is { } max && dates.Distinct().Count() > max) { _text = DisplayValue; return; }
        _text = raw;
        await CommitAsync(dates);
    }

    private void HandleBlur()
    {
        _focused = false;
        _text = DisplayValue;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Autofocus && !_autofocused && !Disabled)
        {
            _autofocused = true;
            try { await FocusAsync(); } catch { /* input may not be in the DOM yet - best-effort */ }
        }
        if (Inline) return;
        await _popup.SyncAsync(_open, _fieldEl, _panelEl, null, _inputEl, AllowPicker ? _toggleEl : null,
            () => _grid?.FocusCursorAsync() ?? Task.CompletedTask);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await _popup.DisposeAsync();
        await base.DisposeAsync();
    }
}
