using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>Month field bound to a <see cref="DateOnly"/>: the first day of the chosen month on the calendar the picker
/// shows (1 Mehr 1405 for fa-IR). The month is typed on a mask in the culture's order or picked from a month and
/// year view in a popup or inline.</summary>
public partial class FlareMonthPicker
{
    /// <summary>The first day of the selected month (supports <c>@bind-Value</c>). A value on another day of a month
    /// selects that month.</summary>
    [Parameter] public DateOnly? Value { get; set; }
    /// <summary>Raised with the first day of the month the user commits (typed, picked or cleared).</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
    /// <summary>Model field bound inside an <c>EditForm</c>: changes reach the edit context and its validation message is shown.</summary>
    [Parameter] public Expression<Func<DateOnly?>>? For { get; set; }
    /// <summary>Earliest day that can be picked: a month wholly before it is disabled and not committed when typed.</summary>
    [Parameter] public DateOnly? Min { get; set; }
    /// <summary>Latest day that can be picked: a month wholly after it is disabled and not committed when typed.</summary>
    [Parameter] public DateOnly? Max { get; set; }
    /// <summary>Culture for the month names, the month mask and the calendar. Default = CurrentUICulture. The months
    /// follow the culture's calendar (Persian for fa-IR, Um al-Qura for ar-SA).</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>The calendar to number months on instead of the culture's own, for example a
    /// <see cref="GregorianCalendar"/> for fa-IR or a <see cref="HebrewCalendar"/> for he-IL (13 months in a leap year,
    /// written in letters - the field then takes free text). Only a calendar the culture offers among its optional
    /// calendars is used; any other is ignored. Null (the default) uses the culture's calendar.</summary>
    [Parameter] public Calendar? Calendar { get; set; }
    /// <summary>Display and parse format. Null (default) uses the culture's year-month pattern, e.g. "MMMM yyyy".</summary>
    [Parameter] public string? MonthFormat { get; set; }
    /// <summary>The view the picker opens to: <see cref="PickerOpenTo.Year"/> opens on the years, anything else on the
    /// months.</summary>
    [Parameter] public PickerOpenTo OpenTo { get; set; } = PickerOpenTo.Month;
    /// <summary>Allows typing the month directly into the field (with auto-separator). Default true.</summary>
    [Parameter] public bool AllowInput { get; set; } = true;
    /// <summary>Allows opening the month popup (shows the calendar icon button). Default true.</summary>
    [Parameter] public bool AllowPicker { get; set; } = true;
    /// <summary>Closes the popup when a month is picked. Default true. Ignored when <see cref="Inline"/>.</summary>
    [Parameter] public bool AutoClose { get; set; } = true;
    /// <summary>Renders the month view inline (always visible under the field) rather than in a popup.</summary>
    [Parameter] public bool Inline { get; set; }
    /// <summary>Shows the Clear button in the popup footer. Default true.</summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;
    /// <summary>Shows the This month button, which picks the current month. Default true.</summary>
    [Parameter] public bool ShowThisMonthButton { get; set; } = true;
    /// <summary>Override text for the Clear button. When null, falls back to the localizer key Picker_Clear.</summary>
    [Parameter] public string? ClearText { get; set; }
    /// <summary>Override text for the This month button. When null, falls back to the localizer key Picker_ThisMonth.</summary>
    [Parameter] public string? ThisMonthText { get; set; }
    /// <summary>Reads typed text instead of the built-in parser: return any day of the month, or null when the text
    /// is not one. With a parser the field takes free text - no digit mask - and judges it on change.</summary>
    [Parameter] public Func<string, DateOnly?>? ParseInput { get; set; }
    /// <summary>Forces the error visual state without an error message (e.g. driven by external validation).</summary>
    [Parameter] public bool HasError { get; set; }
    /// <summary>Requests focus on the input after the first render (best-effort).</summary>
    [Parameter] public bool Autofocus { get; set; }
    /// <summary>Raised when the popup opens.</summary>
    [Parameter] public EventCallback Opened { get; set; }
    /// <summary>Raised when the popup closes.</summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.MonthPicker.Root;

    /// <summary>Opens the month popup.</summary>
    public Task OpenAsync() => SetOpenAsync(true);
    /// <summary>Closes the month popup.</summary>
    public Task CloseAsync() => SetOpenAsync(false);
    /// <summary>Clears the selected month.</summary>
    public Task ClearAsync() => ClearValue();
    /// <summary>Sets keyboard focus to the month input.</summary>
    public ValueTask FocusAsync() => _inputEl.FocusAsync();

    private ElementReference _inputEl;
    private ElementReference _fieldEl;
    private ElementReference _panelEl;
    private ElementReference _toggleEl;
    private FlareMonthYearGrid? _grid;
    private PickerPopup? _popupState;
    private PickerPopup _popup => _popupState ??= new PickerPopup(Overlay, $"flare-monthpicker-{Guid.NewGuid():N}");
    private bool _open;
    private bool _autofocused;
    private MonthYearGridView _view = MonthYearGridView.Months;
    private int _viewYear;
    private string _text = string.Empty;
    private DateOnly? _syncedValue;
    private string? _syncedText;
    private bool _focused;
    private int _editGeneration;
    private int _pendingCaret = -1;

    private CultureInfo _culture => CalendarMath.PickerCulture(Culture ?? CultureInfo.CurrentUICulture, Calendar);
    private System.Globalization.Calendar _gridCalendar => CalendarMath.GridCalendar(_culture);
    private (int First, int Last) _years => CalendarMonth.Years(_gridCalendar);
    private DateOnly Today => DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime);
    // The selected month on the grid calendar.
    private CalendarMonth? _selected => Value is { } v ? CalendarMonth.Of(v, _gridCalendar) : null;
    private string _format => string.IsNullOrEmpty(MonthFormat) ? _culture.DateTimeFormat.YearMonthPattern : MonthFormat;
    private string _separator => string.IsNullOrEmpty(_culture.DateTimeFormat.DateSeparator) ? "." : _culture.DateTimeFormat.DateSeparator;
    // MM.yyyy, or yyyy-MM where the culture writes the year first (ja-JP, sv-SE).
    private string _numericPattern => MaskedInput.NumericMonthPattern(_culture, _separator);
    private bool _digitEditing => ParseInput is null && CalendarMath.WritesDigits(_culture);
    private bool _invalid => HasError || !string.IsNullOrEmpty(DisplayedErrorText);
    private string DisplayValue => _selected is { } m ? CalendarMath.FormatSafe(m.Start, _format, _culture) : string.Empty;

    private string HeaderLabel => _view == MonthYearGridView.Months
        ? CalendarMath.YearLabel(_viewYear, _culture)
        : $"{CalendarMath.YearLabel(Math.Max(_viewYear / 12 * 12, _years.First), _culture)}-" +
          $"{CalendarMath.YearLabel(Math.Min(_viewYear / 12 * 12 + 11, _years.Last), _culture)}";

    private string ViewSwitchHint => _view == MonthYearGridView.Months ? FlareStrings.Picker_ChooseYear : FlareStrings.Picker_ChooseMonth;

    private bool CanPrev => _view == MonthYearGridView.Months ? _viewYear > _years.First : _viewYear / 12 * 12 > _years.First;
    private bool CanNext => _view == MonthYearGridView.Months ? _viewYear < _years.Last : _viewYear / 12 * 12 + 12 <= _years.Last;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ShowYearOf(Value ?? Today);
        _view = OpenTo == PickerOpenTo.Year ? MonthYearGridView.Years : MonthYearGridView.Months;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        UpdateFieldIdentifier(For);
        var display = DisplayValue;
        // A value set from outside shows its year; a new display alone keeps the year the user browsed to.
        if (!Equals(Value, _syncedValue) && Value is { } shown) ShowYearOf(shown);
        if (!Equals(Value, _syncedValue) || (!_focused && display != _syncedText))
        {
            _syncedValue = Value;
            _syncedText = display;
            _text = _focused && _selected is { } m && _digitEditing ? CalendarMath.FormatSafe(m.Start, _numericPattern, _culture) : display;
            _editGeneration++;
        }
    }

    private void ShowYearOf(DateOnly day) => _viewYear = CalendarMonth.Of(day, _gridCalendar).Year;

    private async Task SetOpenAsync(bool open)
    {
        if (Inline || _open == open || (open && (Disabled || ReadOnly))) return;
        _open = open;
        if (open)
        {
            ShowYearOf(Value ?? Today);
            _view = OpenTo == PickerOpenTo.Year ? MonthYearGridView.Years : MonthYearGridView.Months;
            await Opened.InvokeAsync();
        }
        else await Closed.InvokeAsync();
        StateHasChanged();
    }

    private Task Toggle() => SetOpenAsync(!_open);
    private Task Close() => SetOpenAsync(false);

    private void SwitchView() =>
        _view = _view == MonthYearGridView.Months ? MonthYearGridView.Years : MonthYearGridView.Months;

    private void PrevPage()
    {
        if (!CanPrev) return;
        _viewYear = Math.Max(_years.First, _viewYear - (_view == MonthYearGridView.Months ? 1 : 12));
    }

    private void NextPage()
    {
        if (!CanNext) return;
        _viewYear = Math.Min(_years.Last, _viewYear + (_view == MonthYearGridView.Months ? 1 : 12));
    }

    // A year opens its months; a month is the value. A locked field browses but commits nothing (TASK-173).
    private async Task Pick(int value)
    {
        if (_view == MonthYearGridView.Years)
        {
            _viewYear = value;
            _view = MonthYearGridView.Months;
            return;
        }
        if (Disabled || ReadOnly) return;
        await CommitAsync(CalendarMonth.Create(_gridCalendar, _viewYear, value).Start);
        if (AutoClose) await SetOpenAsync(false);
    }

    private async Task GoToThisMonth()
    {
        if (Disabled) return;
        ShowYearOf(Today);
        _view = MonthYearGridView.Months;
        if (ReadOnly) return;
        var month = CalendarMonth.Of(Today, _gridCalendar);
        if (CalendarMonth.MonthUnavailable(_gridCalendar, month.Year, month.Month, Min, Max)) return;
        await CommitAsync(month.Start);
        if (AutoClose) await SetOpenAsync(false);
    }

    private async Task ClearValue()
    {
        if (Disabled || ReadOnly) return;
        await CommitAsync(null);
        await SetOpenAsync(false);
    }

    private async Task CommitAsync(DateOnly? first)
    {
        if (first is { } d) ShowYearOf(d);
        _syncedValue = first;
        await ValueChanged.InvokeAsync(first);
        NotifyFieldChanged();
    }

    private void HandleGridKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "PageUp") PrevPage();
        else if (e.Key == "PageDown") NextPage();
    }

    // Escape anywhere in the popup closes it and puts focus back on the field.
    private async Task HandlePanelKeyDown(KeyboardEventArgs e)
    {
        if (Inline || e.Key != "Escape" || !_open) return;
        _popup.ReturnToField();
        await SetOpenAsync(false);
    }

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
            if (_focused) await MaskedCaret.RestoreAsync(ElementJs, _inputEl, _text, caret);
        }
        if (Inline) return;
        await _popup.SyncAsync(_open, _fieldEl, _panelEl, null, _inputEl, AllowPicker ? _toggleEl : null,
            () => _grid?.FocusAsync() ?? Task.CompletedTask);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _editGeneration++;
        await _popup.DisposeAsync();
        await base.DisposeAsync();
    }

    private void HandleFocus()
    {
        _focused = true;
        if (_selected is { } m && _digitEditing) _text = CalendarMath.FormatSafe(m.Start, _numericPattern, _culture);
    }

    private async Task HandleInput(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var generation = ++_editGeneration;
        if (!_digitEditing) { _text = raw; return; }
        var caretDigits = await MaskedCaret.DigitsBeforeAsync(ElementJs, _inputEl, raw);
        if (generation != _editGeneration) return;
        _text = MaskedInput.MaskByPattern(raw, _numericPattern);
        if (caretDigits >= 0) _pendingCaret = MaskedInput.CaretAfterDigit(_text, caretDigits);
        if (_text.Length == _numericPattern.Length && TryParseMonth(_text, out var first)) await CommitAsync(first);
    }

    private async Task HandleTextChange(ChangeEventArgs e)
    {
        _editGeneration++;
        var raw = e.Value?.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(raw)) { _text = raw; await CommitAsync(null); return; }
        if (TryParseMonth(raw, out var first)) { _text = raw; await CommitAsync(first); return; }
        _text = DisplayValue;
    }

    private void HandleBlur()
    {
        _focused = false;
        _editGeneration++;
        _text = DisplayValue;
    }

    // The first day of the month the text names, when it is a month that can be picked.
    private bool TryParseMonth(string s, out DateOnly first)
    {
        first = default;
        DateOnly day;
        if (ParseInput is { } parse)
        {
            if (parse(s) is not { } parsed) return false;
            day = parsed;
        }
        else if (!DateOnly.TryParseExact(s, _numericPattern, _culture, DateTimeStyles.None, out day)
                 && !DateOnly.TryParseExact(s, _format, _culture, DateTimeStyles.None, out day)
                 && !(!CalendarMath.WritesDigits(_culture) && DateOnly.TryParse(s, _culture, DateTimeStyles.None, out day)))
            return false;
        var month = CalendarMonth.Of(day, _gridCalendar);
        if (CalendarMonth.MonthUnavailable(_gridCalendar, month.Year, month.Month, Min, Max)) return false;
        first = month.Start;
        return true;
    }
}
