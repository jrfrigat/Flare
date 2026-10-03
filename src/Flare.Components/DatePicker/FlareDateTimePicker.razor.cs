using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Resources;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>
/// Date and time field: a masked text input with a popup holding a calendar and a time pane, either as tabs
/// or side by side. The value is a <see cref="DateTimeOffset"/>; edits change its wall date and time and keep its offset.
/// </summary>
public partial class FlareDateTimePicker
{
    /// <summary>Currently selected date and time (supports <c>@bind-Value</c>).</summary>
    [Parameter] public DateTimeOffset? Value { get; set; }
    /// <summary>Raised when the value changes (the <c>@bind-Value</c> callback).</summary>
    [Parameter] public EventCallback<DateTimeOffset?> ValueChanged { get; set; }
    /// <summary>Earliest value that may be committed; earlier days are disabled in the calendar.</summary>
    [Parameter] public DateTimeOffset? Min { get; set; }
    /// <summary>Latest value that may be committed; later days are disabled in the calendar.</summary>
    [Parameter] public DateTimeOffset? Max { get; set; }
    /// <summary>Display/parse format. Null (default) uses the culture short date + time pattern. Typed text
    /// changes the wall date and time; the value keeps its offset and the seconds or fractions the format does
    /// not show, and a new value takes the local zone's offset. A format with an offset specifier (<c>z</c>,
    /// <c>K</c>) lets the user type the offset instead.</summary>
    [Parameter] public string? DateTimeFormat { get; set; }
    // Label, Placeholder, HelperText, ErrorText, Disabled, ReadOnly and Required are inherited from
    // FlareFieldBase (IFlareField). ReadOnly blocks typing (with AllowInput) and opening the popup.
    /// <summary>Expression used to bind and validate the field inside an <c>EditForm</c>.</summary>
    [Parameter] public Expression<Func<DateTimeOffset?>>? For { get; set; }
    /// <summary>Allows typing the value directly into the field (with auto-separators). Default true.</summary>
    [Parameter] public bool AllowInput { get; set; } = true;
    /// <summary>Allows opening the picker popup (shows the icon button). Default true.</summary>
    [Parameter] public bool AllowPicker { get; set; } = true;
    /// <summary>Time tab style: <see cref="TimePickerVariant.Dial"/> (clock) or <see cref="TimePickerVariant.Dropdown"/> (number fields, default); <see cref="TimePickerVariant.List"/> shows the number fields too.</summary>
    [Parameter] public TimePickerVariant TimeVariant { get; set; } = TimePickerVariant.Dropdown;
    /// <summary>Popup layout: <see cref="DateTimeVariant.Auto"/> (default, responsive), <see cref="DateTimeVariant.Tabs"/> or <see cref="DateTimeVariant.Panels"/> (side by side).</summary>
    [Parameter] public DateTimeVariant Mode { get; set; } = DateTimeVariant.Auto;
    /// <summary>Forces 12/24-hour clock on the dial. Null auto-detects from culture.</summary>
    [Parameter] public bool? Use24Hour { get; set; }
    /// <summary>Adds seconds to the field (the default format uses the culture's long time pattern) and a seconds
    /// box to the number-field time pane. The clock dial picks hours and minutes only and keeps the seconds,
    /// as in <c>FlareTimePicker</c>.</summary>
    [Parameter] public bool ShowSeconds { get; set; }
    /// <summary>Hour increment of the number-field time pane (default 1): a typed hour drops to the step below
    /// it. The dial and the text field are not stepped, as in <c>FlareTimePicker</c>.</summary>
    [Parameter] public int HourStep { get; set; } = 1;
    /// <summary>Minute increment of the number-field time pane (default 1): a typed minute drops to the step
    /// below it. The dial and the text field are not stepped, as in <c>FlareTimePicker</c>.</summary>
    [Parameter] public int MinuteStep { get; set; } = 1;
    /// <summary>Returns true for a day that cannot be picked (holidays, weekends): it is disabled in the
    /// calendar, skipped by the arrow keys, and a typed or confirmed value on it is not committed.</summary>
    [Parameter] public Func<DateOnly, bool>? IsDateDisabled { get; set; }
    /// <summary>Culture for the calendar and parsing. Default = CurrentUICulture.
    /// The calendar is Gregorian: a culture whose calendar has other months (Persian, Hijri, Hebrew) is shown on its Gregorian calendar, while Thai Buddhist and Japanese years are kept.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>Shows a leading week-of-year number column in the calendar.</summary>
    [Parameter] public bool ShowWeekNumbers { get; set; }
    /// <summary>Overrides the culture's first day of week (null = use the culture's).</summary>
    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }
    /// <summary>Returns extra CSS class(es) for a given day cell (e.g. to mark holidays).</summary>
    [Parameter] public Func<DateOnly, string>? DayClassFunc { get; set; }
    /// <summary>Content of a day cell (e.g. the number with an event dot). The picker keeps the cell itself - the
    /// button, its full-date label, focus, disabled state and selection - so a template cannot break them.
    /// Null shows the day number.</summary>
    [Parameter] public RenderFragment<DateOnly>? DayTemplate { get; set; }
    /// <summary>Reads typed text instead of the built-in parser: return the value - offset included - or null when
    /// the text is not one (the value is kept and the field shows it again). With a parser the field takes free
    /// text - no digit mask - and judges it on change (blur or Enter); Min/Max and IsDateDisabled still apply.</summary>
    [Parameter] public Func<string, DateTimeOffset?>? ParseInput { get; set; }
    /// <summary>Requests focus on the input after the first render (best-effort). Only one field per page
    /// should set this.</summary>
    [Parameter] public bool Autofocus { get; set; }
    /// <summary>Raised when the picker popup opens.</summary>
    [Parameter] public EventCallback Opened { get; set; }
    /// <summary>Raised when the picker popup closes.</summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.DateTimePicker.Root;

    private ElementReference _inputEl;

    /// <summary>Opens the picker popup.</summary>
    public Task OpenAsync() => _open ? Task.CompletedTask : TogglePanel();
    /// <summary>Closes the picker popup.</summary>
    public async Task CloseAsync()
    {
        if (!_open) return;
        _open = false;
        await Closed.InvokeAsync();
        StateHasChanged();
    }
    /// <summary>Toggles the picker popup.</summary>
    public Task ToggleAsync() => TogglePanel();

    // Escape anywhere in the popup - calendar, navigation, tabs, time pane - drops the draft and puts focus
    // back in the field once the panel is gone (TASK-134).
    private Task CancelAsync()
    {
        _popup.ReturnToField();
        return CloseAsync();
    }

    private Task HandlePanelKeyDown(KeyboardEventArgs e) => e.Key == "Escape" ? CancelAsync() : Task.CompletedTask;
    /// <summary>Clears the selected value.</summary>
    public Task ClearAsync() => Clear();
    /// <summary>Sets keyboard focus to the input.</summary>
    public ValueTask FocusAsync() => _inputEl.FocusAsync();

    private string ComposeDayClass(DateOnly d)
    {
        var selected = d == _selectedDate ? Css.Classes.Picker.DaySelected : string.Empty;
        var extra = DayClassFunc?.Invoke(d);
        if (string.IsNullOrEmpty(extra)) return selected;
        return string.IsNullOrEmpty(selected) ? extra : $"{selected} {extra}";
    }

    // Invalid drives the frame's error chrome (a resolved validation message).
    private bool _invalid => !string.IsNullOrEmpty(DisplayedErrorText);

    private bool _open;
    private bool _autofocused;
    private int _activeTab;
    private DateOnly? _selectedDate;
    // The keyboard cursor when it is not the picked day: the grid reports the day the user sees focused
    // after a month change or a click (TASK-133).
    private DateOnly? _focusedDate;
    private DateOnly FocusedCursor => _focusedDate ?? _selectedDate ?? _viewDate;
    private int _hour;
    private int _minute;
    private int _second;
    private DateOnly _viewDate;
    private ElementReference _fieldRef;
    private ElementReference _panelEl;
    private PickerPopup? _popupState;
    private PickerPopup _popup => _popupState ??= new PickerPopup(Overlay, $"flare-datetimepicker-{Guid.NewGuid():N}");
    private ElementReference _toggleEl;
    private FlareMonthGrid? _grid;
    private bool _is24Hour;
    private Breakpoint _bp = Breakpoint.Xs;
    private IAsyncDisposable? _bpSubscription;
    private string _text = string.Empty;
    private DateTimeOffset? _syncedValue;
    private string? _syncedText;
    private bool _focused;
    // Bumped by every input, change, blur, external re-sync and dispose; an input handler that resumes from
    // its awaited selection read in an older generation is stale and drops its result (TASK-138).
    private int _editGeneration;
    // Caret position to restore after the next render (an auto-separator rewrite moved it to the end);
    // -1 when there is nothing to restore.
    private int _pendingCaret = -1;

    // A calendar whose months are not the Gregorian grid's is swapped for Gregorian (TASK-150).
    private CultureInfo _culture => CalendarMath.PickerCulture(Culture ?? CultureInfo.CurrentUICulture);
    private string _format => string.IsNullOrEmpty(DateTimeFormat)
        ? $"{_culture.DateTimeFormat.ShortDatePattern} {(ShowSeconds ? _culture.DateTimeFormat.LongTimePattern : _culture.DateTimeFormat.ShortTimePattern)}"
        : DateTimeFormat;
    private string _dateSep => Nz(_culture.DateTimeFormat.DateSeparator, ".");
    // Date segments in the culture's own order, so year-first cultures mask and parse too (TASK-110).
    private string _numericPattern => MaskedInput.NumericDatePattern(_culture, _dateSep) + (ShowSeconds ? " HH:mm:ss" : " HH:mm");
    // A format that shows the offset (z, K) edits it too, after the time: "15.10.2026 14:30 +05:00" (TASK-165).
    private bool _editsOffset => _format.Contains('z') || _format.Contains('K');
    private string _editPattern => _editsOffset ? _numericPattern + " zzz" : _numericPattern;
    // Length of a complete edit: "zzz" renders as the 6 characters "+05:00".
    private int _editLength => _numericPattern.Length + (_editsOffset ? 7 : 0);

    private static string Nz(string? s, string fallback) => string.IsNullOrEmpty(s) ? fallback : s;

    // Layout: Auto resolves to Panels on >= md (960px) viewports, Tabs otherwise.
    private DateTimeVariant _effectiveVariant => Mode switch
    {
        DateTimeVariant.Panels => DateTimeVariant.Panels,
        DateTimeVariant.Auto => _bp.IsAtLeast(Breakpoint.Md) ? DateTimeVariant.Panels : DateTimeVariant.Tabs,
        _ => DateTimeVariant.Tabs,
    };
    private bool _panels => _effectiveVariant == DateTimeVariant.Panels;


    private DateOnly Today => DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime);

    // Day headers + the 6x7 grid are rendered by the shared FlareMonthGrid (CalendarMath).

    private string FormattedValue => Value.HasValue ? Value.Value.ToString(_format, _culture) : string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _viewDate = Today;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        UpdateFieldIdentifier(For);
        _is24Hour = Use24Hour ?? !_culture.DateTimeFormat.ShortTimePattern.Contains('h');
        // The popup draft (day, hour, minute) is loaded from Value when the popup opens, not here: a parent
        // re-render must not wipe a pick in progress (TASK-107).
        // A new value always re-syncs the text and cancels any input still in flight - a parent's value wins over
        // keystrokes typed against the old one (TASK-163); a new display (DateTimeFormat, Culture) only outside
        // editing (TASK-113). A focused field takes the value in its editing form.
        var display = FormattedValue;
        if (!Equals(Value, _syncedValue) || (!_focused && display != _syncedText))
        {
            _syncedValue = Value;
            _syncedText = display;
            _text = _focused && Value.HasValue && ParseInput is null ? Value.Value.ToString(_editPattern, _culture) : display;
            _editGeneration++;
        }
    }

    private void HandleFocus()
    {
        _focused = true;
        // Edit in numeric form; a custom parser edits the text as it is shown.
        if (Value.HasValue && ParseInput is null) _text = Value.Value.ToString(_editPattern, _culture);
    }

    private async Task HandleInput(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var generation = ++_editGeneration;
        // A custom parser reads free text: no mask, and the text is judged on change (blur or Enter).
        if (ParseInput is not null) { _text = raw; return; }
        var caretDigits = await MaskedCaret.DigitsBeforeAsync(ElementJs, _inputEl, raw);
        if (generation != _editGeneration) return;
        _text = MaskDateTime(raw);
        if (caretDigits >= 0) _pendingCaret = MaskedInput.CaretAfterDigit(_text, caretDigits);
        if (_text.Length == _editLength && TryParse(_text, out var dt) && IsAllowed(dt))
            await CommitValue(dt);
    }

    private async Task HandleTextChange(ChangeEventArgs e)
    {
        _editGeneration++;
        var raw = e.Value?.ToString()?.Trim() ?? string.Empty;
        _text = raw;
        if (string.IsNullOrEmpty(raw)) { await CommitValue(null); return; }
        if (TryParse(raw, out var dt) && IsAllowed(dt))
            await CommitValue(dt);
    }

    private void HandleBlur()
    {
        _focused = false;
        _editGeneration++;
        _text = FormattedValue;
    }

    private bool InRange(DateTimeOffset dt) =>
        (!Min.HasValue || dt >= Min.Value) && (!Max.HasValue || dt <= Max.Value);

    // A value may be committed - typed or confirmed - only inside [Min; Max] and on a day IsDateDisabled allows.
    private bool IsAllowed(DateTimeOffset dt) =>
        InRange(dt) && !(IsDateDisabled?.Invoke(DateOnly.FromDateTime(dt.DateTime)) ?? false);

    // A day is unselectable only when every moment of it lies outside [Min; Max]; a boundary day stays
    // selectable so the time part can decide (TASK-101). The day is taken in the offset the picked value will
    // have, not in the bound's own one, and compared in UTC ticks so the 0001/9999 edges cannot throw (TASK-137).
    private bool IsDayDisabled(DateOnly d)
    {
        if (IsDateDisabled?.Invoke(d) ?? false) return true;
        if (!Min.HasValue && !Max.HasValue) return false;
        var startUtc = d.DayNumber * TimeSpan.TicksPerDay - OffsetFor(d.ToDateTime(TimeOnly.MinValue)).Ticks;
        var endUtc = startUtc + TimeSpan.TicksPerDay - 1;
        return (Min.HasValue && endUtc < Min.Value.UtcTicks) || (Max.HasValue && startUtc > Max.Value.UtcTicks);
    }

    private async Task CommitValue(DateTimeOffset? dt)
    {
        if (dt.HasValue)
        {
            _selectedDate = DateOnly.FromDateTime(dt.Value.DateTime);
            _hour = dt.Value.Hour;
            _minute = dt.Value.Minute;
            _second = dt.Value.Second;
            _viewDate = _selectedDate.Value;
        }
        else
        {
            _selectedDate = null;
        }
        _syncedValue = dt;
        await ValueChanged.InvokeAsync(dt);
        NotifyFieldChanged();
    }

    private async Task TogglePanel()
    {
        if (Disabled || ReadOnly) return;
        if (_open) { await CloseAsync(); return; }
        LoadDraft();
        _open = true;
        // Placement is the shared engine's job now (see OnAfterRenderAsync): it flips sides on its own
        // measurement rather than on a guessed panel height, follows scroll, and puts the panel in the
        // top layer. The hand-rolled version that used to live here assumed 380px of panel and left the
        // popup a plain fixed box, so any ancestor with a transform dragged it off the screen.
        await Opened.InvokeAsync();
        // Called from OpenAsync/ToggleAsync as well as the toggle's click; only the click re-renders on its own.
        StateHasChanged();
    }

    // Every opening starts from the current Value: a pick dismissed by the scrim or Escape is dropped, and
    // a value cleared from outside opens with nothing selected (TASK-107).
    private void LoadDraft()
    {
        _activeTab = 0;
        _focusedDate = null;
        if (Value is { } v)
        {
            _selectedDate = DateOnly.FromDateTime(v.DateTime);
            _hour = v.Hour;
            _minute = v.Minute;
            _second = v.Second;
            _viewDate = _selectedDate.Value;
        }
        else
        {
            _selectedDate = null;
            _hour = 0;
            _minute = 0;
            _second = 0;
            _viewDate = Today;
        }
    }

    // Anchor on the first of the month so AddMonths cannot overflow at 0001-01 or 9999-12 (TASK-105).
    private void PrevMonth()
    {
        var firstOfMonth = new DateOnly(_viewDate.Year, _viewDate.Month, 1);
        if (firstOfMonth != DateOnly.MinValue) _viewDate = firstOfMonth.AddMonths(-1);
    }

    private void NextMonth()
    {
        var firstOfMonth = new DateOnly(_viewDate.Year, _viewDate.Month, 1);
        if (firstOfMonth.Year != 9999 || firstOfMonth.Month != 12) _viewDate = firstOfMonth.AddMonths(1);
    }

    private void SelectDate(DateOnly d)
    {
        if (IsDayDisabled(d)) return;
        _selectedDate = d;
        _activeTab = 1;
    }

    // Keyboard navigation inside the date pane: movement is relative to the selected/visible day and skips
    // unavailable days; Escape closes the popup (TASK-106). Enter/Space are left to the native click of the
    // focused day button, which the grid keeps on the cursor (TASK-124).
    private async Task HandleGridKeyDown(KeyboardEventArgs e)
    {
        if (Disabled || ReadOnly) return;

        if (e.Key == "Escape") { await CancelAsync(); return; }

        if (CalendarMath.KeyTarget(FocusedCursor, e.Key, e.ShiftKey,
                FirstDayOfWeek ?? _culture.DateTimeFormat.FirstDayOfWeek, IsDayDisabled) is not { } day) return;

        _selectedDate = day;
        _focusedDate = null;
        if (day.Year != _viewDate.Year || day.Month != _viewDate.Month)
            _viewDate = day;
    }

    private int _hourStep => HourStep < 1 ? 1 : HourStep;
    private int _minuteStep => MinuteStep < 1 ? 1 : MinuteStep;

    // A typed hour or minute drops to the step below it, as only step values are offered by the time columns.
    private void SetHour(string? v) =>
        _hour = int.TryParse(v, out var h) ? Snap(Math.Clamp(h, 0, 23), _hourStep) : _hour;

    private void SetMinute(string? v) =>
        _minute = int.TryParse(v, out var m) ? Snap(Math.Clamp(m, 0, 59), _minuteStep) : _minute;

    private void SetSecond(string? v) =>
        _second = int.TryParse(v, out var s) ? Math.Clamp(s, 0, 59) : _second;

    private static int Snap(int value, int step) => value - value % step;

    // The value the popup would commit, or null when no day is picked. Keeps the value's offset and every tick
    // the popup cannot edit - below the minute, or below the second with ShowSeconds: re-confirming without
    // editing must not move the instant (TASK-103, TASK-136). Returns false when that wall time with its offset is
    // not a representable instant - the first or last day of the DateTimeOffset range at an offset that pushes UTC
    // past it (TASK-160).
    private bool TryComposeDraft(out DateTimeOffset? value)
    {
        value = null;
        if (_selectedDate is not { } day) return true;
        var ticks = Value?.Ticks ?? 0;
        var (wall, hidden) = ShowSeconds
            ? (day.ToDateTime(new TimeOnly(_hour, _minute, _second)), ticks % TimeSpan.TicksPerSecond)
            : (day.ToDateTime(new TimeOnly(_hour, _minute)), ticks % TimeSpan.TicksPerMinute);
        if (!TryCompose(wall, hidden, out var composed)) return false;
        value = composed;
        return true;
    }

    // OK is offered only for a representable value inside [Min; Max], as for the text input (TASK-101, TASK-125).
    private bool CanConfirm => TryComposeDraft(out var dt) && (dt is not { } v || IsAllowed(v));

    private async Task Confirm()
    {
        if (!CanConfirm || !TryComposeDraft(out var dt)) return;
        if (dt is { } v) await CommitValue(v);
        await CloseAsync();
    }

    private async Task Clear()
    {
        await CommitValue(null);
        await CloseAsync();
    }


    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Autofocus && !_autofocused && !Disabled)
        {
            _autofocused = true;
            try { await FocusAsync(); } catch { /* input may not be in the DOM yet - best-effort */ }
        }

        if (firstRender && Mode == DateTimeVariant.Auto)
        {
            try
            {
                // Auto layout tracks the viewport tier; the service reports it immediately and on change.
                _bpSubscription = await Viewport.SubscribeBreakpointAsync(OnBreakpointAsync);
            }
            catch { /* breakpoint sync is best-effort */ }
        }

        if (_pendingCaret >= 0)
        {
            var caret = _pendingCaret;
            _pendingCaret = -1;
            // Only restore while the field still holds focus: the selection read is awaited, so the user may
            // have left the field meanwhile, and pulling focus back onto it would be wrong (TASK-120).
            if (_focused) await MaskedCaret.RestoreAsync(ElementJs, _inputEl, _text, caret);
        }

        // The popup is a modal dialog: the cursor day takes focus on open (TASK-134).
        await _popup.SyncAsync(_open, _fieldRef, _panelEl, new AnchoredPanelOptions { MatchWidth = true },
            _inputEl, AllowPicker ? _toggleEl : null, () => _grid?.FocusCursorAsync() ?? Task.CompletedTask);
    }

    private Task OnBreakpointAsync(Breakpoint breakpoint)
    {
        _bp = breakpoint;
        return InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _editGeneration++;
        if (_bpSubscription is not null)
        {
            try { await _bpSubscription.DisposeAsync(); } catch { }
        }
        if (_popupState is not null) await _popupState.DisposeAsync();
        await base.DisposeAsync();
    }
}
