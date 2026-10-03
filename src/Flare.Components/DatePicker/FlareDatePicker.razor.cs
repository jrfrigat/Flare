using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Resources;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>Date field with a masked text entry and a calendar popup (or an inline calendar), bound to a <see cref="DateOnly"/>.</summary>
public partial class FlareDatePicker
{
    /// <summary>Currently selected date (supports <c>@bind-Value</c>).</summary>
    [Parameter] public DateOnly? Value { get; set; }
    /// <summary>Raised when the user commits a date (typed, picked or cleared).</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
    // Label, Placeholder, HelperText, ErrorText, Disabled, ReadOnly and Required are inherited from
    // FlareFieldBase (IFlareField). ReadOnly blocks typing (with AllowInput) and the calendar popup.
    /// <summary>Display/parse format. Null (default) uses the culture short date pattern; can be a long format like "dd MMMM yyyy".</summary>
    [Parameter] public string? DateFormat { get; set; }
    /// <summary>Allows typing the date directly into the field (with auto-separator). Default true.</summary>
    [Parameter] public bool AllowInput { get; set; } = true;
    /// <summary>Allows opening the calendar popup (shows the calendar icon button). Default true.</summary>
    [Parameter] public bool AllowPicker { get; set; } = true;
    /// <summary>Earliest date that can be picked or typed (inclusive); earlier days are disabled.</summary>
    [Parameter] public DateOnly? Min { get; set; }
    /// <summary>Latest date that can be picked or typed (inclusive); later days are disabled.</summary>
    [Parameter] public DateOnly? Max { get; set; }
    /// <summary>Forces the error visual state without an error message (e.g. driven by external validation).</summary>
    [Parameter] public bool HasError { get; set; }
    /// <summary>Model field bound inside an <c>EditForm</c>: changes reach the edit context and its validation message is shown.</summary>
    [Parameter] public Expression<Func<DateOnly?>>? For { get; set; }
    /// <summary>Override text for the Clear button. When null, falls back to the localizer key Picker_Clear.</summary>
    [Parameter] public string? ClearText { get; set; }
    /// <summary>Override text for the Today button. When null, falls back to the localizer key DatePicker_Today.</summary>
    [Parameter] public string? TodayText { get; set; }
    /// <summary>Shows the Clear button in the calendar footer. Default true.</summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;
    /// <summary>Shows the Today button in the calendar footer. Default true.</summary>
    [Parameter] public bool ShowTodayButton { get; set; } = true;
    /// <summary>Predicate that disables specific dates (return true to disable). Applied on top of Min/Max.</summary>
    [Parameter] public Func<DateOnly, bool>? IsDateDisabled { get; set; }
    /// <summary>Culture used for the calendar (day headers, month names, first day of week) and for writing and
    /// reading the date. Default = CurrentUICulture. The calendar follows the culture's own: Persian months and years
    /// for fa-IR, Um al-Qura for ar-SA; Thai Buddhist and Japanese cultures keep the Gregorian months and write
    /// their own years.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>The calendar to show and write dates on instead of the culture's own, for example a
    /// <see cref="GregorianCalendar"/> for fa-IR or a <see cref="HebrewCalendar"/> for he-IL (13 months in a leap
    /// year, dates written in letters - the field then takes free text instead of a digit mask). Only a calendar the
    /// culture offers among its optional calendars is used; any other is ignored. Null (the default) uses the
    /// culture's calendar. The value stays a <see cref="DateOnly"/>.</summary>
    [Parameter] public Calendar? Calendar { get; set; }
    /// <summary>The calendar view the picker opens to (Day/Month/Year). <see cref="PickerOpenTo.Year"/> is
    /// handy for far-back dates like a date of birth.</summary>
    [Parameter] public PickerOpenTo OpenTo { get; set; } = PickerOpenTo.Day;
    /// <summary>Closes the picker automatically when a day is selected. Default true. Ignored when <see cref="Inline"/>.</summary>
    [Parameter] public bool AutoClose { get; set; } = true;
    /// <summary>Renders the calendar inline (always visible under the field) rather than in a popup.</summary>
    [Parameter] public bool Inline { get; set; }
    /// <summary>Requests focus on the date input after the first render (best-effort). Only one field per
    /// page should set this.</summary>
    [Parameter] public bool Autofocus { get; set; }
    /// <summary>Shows a leading week-of-year number column in the calendar.</summary>
    [Parameter] public bool ShowWeekNumbers { get; set; }
    /// <summary>Overrides the culture's first day of week when set (null = use the culture's, e.g. for a
    /// Monday-first calendar regardless of locale).</summary>
    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }
    /// <summary>Returns extra CSS class(es) for a given day cell (e.g. to mark holidays or highlight days).</summary>
    [Parameter] public Func<DateOnly, string>? DayClassFunc { get; set; }
    /// <summary>Content of a day cell (e.g. the number with an event dot). The picker keeps the cell itself - the
    /// button, its full-date label, focus, disabled state and selection - so a template cannot break them.
    /// Null shows the day number.</summary>
    [Parameter] public RenderFragment<DateOnly>? DayTemplate { get; set; }
    /// <summary>Reads typed text instead of the built-in parser: return the date, or null when the text is not
    /// one (the value is kept and the field shows it again). With a parser the field takes free text - no digit
    /// mask - and judges it on change (blur or Enter).</summary>
    [Parameter] public Func<string, DateOnly?>? ParseInput { get; set; }
    /// <summary>Raised when the calendar popup opens.</summary>
    [Parameter] public EventCallback Opened { get; set; }
    /// <summary>Raised when the calendar popup closes.</summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.DatePicker.Root;

    private ElementReference _inputEl;
    private PickerView _initialView => OpenTo switch
    {
        PickerOpenTo.Month => PickerView.Month,
        PickerOpenTo.Year => PickerView.Year,
        _ => PickerView.Day,
    };

    /// <summary>Opens the calendar popup.</summary>
    public Task OpenAsync() => SetOpenAsync(true);
    /// <summary>Closes the calendar popup.</summary>
    public Task CloseAsync() => SetOpenAsync(false);
    /// <summary>Toggles the calendar popup open/closed.</summary>
    public Task ToggleAsync() => SetOpenAsync(!_open);
    /// <summary>Clears the selected date.</summary>
    public Task ClearAsync() => ClearValue();
    /// <summary>Sets keyboard focus to the date input.</summary>
    public ValueTask FocusAsync() => _inputEl.FocusAsync();

    // Single entry point for open-state changes so Opened/Closed fire consistently. Inline pickers are
    // always shown, so open/close is a no-op there. A locked field cannot be opened, but a popup opened before
    // the lock still closes - by the API, the scrim or Escape (TASK-162).
    private async Task SetOpenAsync(bool open)
    {
        if (Inline || _open == open || (open && (Disabled || ReadOnly))) return;
        _open = open;
        if (open)
        {
            ShowMonthOf(Value ?? Today);
            _calView = _initialView;
            await Opened.InvokeAsync();
        }
        else
        {
            await Closed.InvokeAsync();
        }
        StateHasChanged();
    }

    private string ComposeDayClass(DateOnly d)
    {
        var selected = d == Value ? Css.Classes.Picker.DaySelected : string.Empty;
        var extra = DayClassFunc?.Invoke(d);
        if (string.IsNullOrEmpty(extra)) return selected;
        return string.IsNullOrEmpty(selected) ? extra : $"{selected} {extra}";
    }

    private bool _open;

    private ElementReference _fieldEl;
    private ElementReference _panelEl;
    private PickerPopup? _popupState;
    private PickerPopup _popup => _popupState ??= new PickerPopup(Overlay, $"flare-datepicker-{Guid.NewGuid():N}");

    // The culture on the calendar the picker shows and writes dates on (TASK-182).
    private CultureInfo _culture => CalendarMath.PickerCulture(Culture ?? CultureInfo.CurrentUICulture, Calendar);

    private DayOfWeek _firstDayOfWeek => _culture.DateTimeFormat.FirstDayOfWeek;

    // Day headers + the 6x7 grid are rendered by the shared FlareMonthGrid (CalendarMath).

    // Effective display/parse format (culture short pattern when not set).
    private string _format => string.IsNullOrEmpty(DateFormat) ? _culture.DateTimeFormat.ShortDatePattern : DateFormat;
    private string _separator => Nz(_culture.DateTimeFormat.DateSeparator, ".");
    // Normalised numeric mask used for typing + placeholder, with the segments in the culture's own order so
    // year-first cultures (ja-JP, sv-SE, ...) mask and parse too (TASK-110).
    private string _numericPattern => MaskedInput.NumericDatePattern(_culture, _separator);
    // The field edits digits on a mask unless a custom parser reads the text or the calendar writes letters
    // (Hebrew): then it takes free text and judges it on change.
    private bool _digitEditing => ParseInput is null && CalendarMath.WritesDigits(_culture);

    private static string Nz(string? s, string fallback) => string.IsNullOrEmpty(s) ? fallback : s;

    private string DisplayValue => Value.HasValue ? CalendarMath.FormatSafe(Value.Value, _format, _culture) : string.Empty;

    // Strip to digits and lay them on the numeric pattern. "01062026" -> "01.06.2026" (ru), "2026/10/15" (ja).
    private string MaskDate(string? raw) => MaskedInput.MaskByPattern(raw, _numericPattern);
    private DateOnly Today => DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime);

    // Invalid drives the frame's error chrome: explicit HasError or a resolved validation message.
    private bool _invalid => HasError || !string.IsNullOrEmpty(DisplayedErrorText);

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ShowMonthOf(Value ?? Today);
        _calView = _initialView;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        UpdateFieldIdentifier(For);

        // A new value always re-syncs the text and cancels any input still in flight - a parent's value wins over
        // keystrokes typed against the old one (TASK-163); a new display (DateFormat, Culture) only outside
        // editing (TASK-113). A focused field takes the value in its editing form.
        var display = DisplayValue;
        // A new value set from outside shows its month: an inline calendar is never reopened to re-anchor it, and a
        // new display alone (Culture, DateFormat) keeps the month the user browsed to (TASK-176).
        if (!Equals(Value, _syncedValue) && Value is { } shown) ShowMonthOf(shown);
        if (!Equals(Value, _syncedValue) || (!_focused && display != _syncedText))
        {
            _syncedValue = Value;
            _syncedText = display;
            _text = _focused && Value.HasValue && _digitEditing ? CalendarMath.FormatSafe(Value.Value, _numericPattern, _culture) : display;
            _editGeneration++;
        }
    }

    private string _text = string.Empty;
    private DateOnly? _syncedValue;
    private string? _syncedText;
    private bool _focused;
    private FlareMonthGrid? _grid;
    private ElementReference _toggleEl;
    // Bumped by every input, change, blur, external re-sync and dispose; an input handler that resumes from
    // its awaited selection read in an older generation is stale and drops its result (TASK-138).
    private int _editGeneration;
    private bool _autofocused;
    // Caret position to restore after the next render (an auto-separator rewrite moved it to the end);
    // -1 when there is nothing to restore.
    private int _pendingCaret = -1;
    // The cell the keyboard cursor sits on (roving focus); starts from the value (or today) and moves
    // with the arrow keys, independently of Value (TASK-106).
    private DateOnly? _focusedDate;
    private DateOnly FocusedCursor => _focusedDate ?? Value ?? Today;

    // Fixed-position the calendar under the field on open (and clean up on close) so it escapes a
    // Card's overflow:hidden. The scrim handles dismissal, so no document listener is needed here.
    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Autofocus && !_autofocused && !Disabled)
        {
            _autofocused = true;
            try { await FocusAsync(); } catch { /* input may not be in the DOM yet - best-effort */ }
        }
        if (_pendingCaret >= 0)
        {
            var caret = _pendingCaret;
            _pendingCaret = -1;
            // Only restore while the field still holds focus: the selection read is awaited, so the user may
            // have left the field meanwhile, and pulling focus back onto it would be wrong (TASK-120).
            if (_focused) await MaskedCaret.RestoreAsync(ElementJs, _inputEl, _text, caret);
        }
        if (Inline) return; // inline panel sits in normal flow - no anchored positioning
        // The popup is a modal dialog under the field: Tab stays inside and the cursor day takes focus;
        // closing returns focus to the toggle, or to the field after Escape (TASK-134).
        await _popup.SyncAsync(_open, _fieldEl, _panelEl, null, _inputEl, AllowPicker ? _toggleEl : null,
            () => _grid?.FocusCursorAsync() ?? Task.CompletedTask);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _editGeneration++;
        await _popup.DisposeAsync();
        await base.DisposeAsync();
    }


    private bool IsDisabled(DateOnly d) =>
        (Min.HasValue && d < Min.Value) || (Max.HasValue && d > Max.Value) ||
        (IsDateDisabled?.Invoke(d) ?? false);

    // A disabled or read-only field makes every day unselectable, inline calendar included (TASK-104).
    private bool IsDayUnavailable(DateOnly d) => Disabled || ReadOnly || IsDisabled(d);

    private Task Toggle() => SetOpenAsync(!_open);

    private Task Close() => SetOpenAsync(false);

    private async Task SelectDay(DateOnly d)
    {
        if (Disabled || ReadOnly || IsDisabled(d)) return;
        await ValueChanged.InvokeAsync(d);
        NotifyFieldChanged();
        if (AutoClose) await SetOpenAsync(false);
    }

    /// <summary>Parses a date typed directly into the field (keyboard entry).</summary>
    private void HandleFocus()
    {
        _focused = true;
        // Edit in numeric form; a custom parser edits the text as it is shown.
        if (Value.HasValue && _digitEditing) _text = CalendarMath.FormatSafe(Value.Value, _numericPattern, _culture);
    }

    private async Task HandleInput(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var generation = ++_editGeneration;
        // A custom parser or a calendar written in letters reads free text: no mask, and the text is judged on
        // change (blur or Enter).
        if (!_digitEditing) { _text = raw; return; }
        var caretDigits = await MaskedCaret.DigitsBeforeAsync(ElementJs, _inputEl, raw);
        if (generation != _editGeneration) return;
        _text = MaskDate(raw);
        if (caretDigits >= 0) _pendingCaret = MaskedInput.CaretAfterDigit(_text, caretDigits);
        if (_text.Length == _numericPattern.Length && TryParseDate(_text, out var d) && !IsDisabled(d))
            await CommitDate(d);
    }

    private async Task HandleTextChange(ChangeEventArgs e)
    {
        _editGeneration++;
        var raw = e.Value?.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(raw)) { _text = raw; await CommitDate(null); return; }
        if (TryParseDate(raw, out var d) && !IsDisabled(d))
        {
            _text = raw;
            await CommitDate(d);
            return;
        }
        // Incomplete or unparsable text is not committed; put the field back to what it currently shows so a
        // stale half-edit is not left behind (TASK-119).
        _text = DisplayValue;
    }

    private void HandleBlur()
    {
        _focused = false;
        _editGeneration++;
        _text = DisplayValue; // reformat to the display format on blur
    }

    private bool TryParseDate(string s, out DateOnly d)
    {
        if (ParseInput is { } parse)
        {
            var parsed = parse(s);
            d = parsed ?? default;
            return parsed.HasValue;
        }
        if (DateOnly.TryParseExact(s, _numericPattern, _culture, DateTimeStyles.None, out d)) return true;
        if (DateOnly.TryParseExact(s, _format, _culture, DateTimeStyles.None, out d)) return true;
        // A half-edited mask ("11.02.026") must never fall through to the lenient parser and be committed
        // as a different month/year (TASK-119): only a complete date (8 digits) may use it. A calendar written in
        // letters has no digits to count, and its free text goes to the culture's parser as typed.
        if ((!CalendarMath.WritesDigits(_culture) || s.Count(char.IsDigit) == 8)
            && DateOnly.TryParse(s, _culture, DateTimeStyles.None, out d)) return true;
        d = default;
        return false;
    }

    private async Task CommitDate(DateOnly? d)
    {
        if (d.HasValue) ShowMonthOf(d.Value);
        _syncedValue = d;
        await ValueChanged.InvokeAsync(d);
        NotifyFieldChanged();
    }

    private async Task ClearValue()
    {
        if (Disabled || ReadOnly) return;
        await ValueChanged.InvokeAsync(null);
        NotifyFieldChanged();
        await SetOpenAsync(false);
    }

    private async Task HandleGridKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape") { await ClosePopupAsync(); return; }
        if (Disabled || ReadOnly) return;

        // Movement is relative to the focused cell, not the value (TASK-106), which the grid has just synced
        // to the day the user sees focused (TASK-133). Disabled dates (Min/Max/IsDateDisabled) are skipped.
        // Enter/Space are left to the native click of the focused day button (TASK-124).
        var next = CalendarMath.KeyTarget(FocusedCursor, e.Key, e.ShiftKey,
            FirstDayOfWeek ?? _culture.DateTimeFormat.FirstDayOfWeek, IsDisabled, _gridCalendar);
        if (next is null) return;

        _focusedDate = next.Value;

        // Navigate the visible month if the cursor crossed a boundary.
        if (!_view.Contains(next.Value)) ShowMonthOf(next.Value);
    }

    // Escape closes the popup and puts focus back on the trigger field once the panel is gone (TASK-106).
    private async Task ClosePopupAsync()
    {
        if (!_open) return;
        _popup.ReturnToField();
        await SetOpenAsync(false);
    }

    // Escape anywhere in the popup - header, month or year view - closes it, not only in the day grid
    // (TASK-134).
    private Task HandlePanelKeyDown(KeyboardEventArgs e) =>
        !Inline && e.Key == "Escape" ? ClosePopupAsync() : Task.CompletedTask;
}
