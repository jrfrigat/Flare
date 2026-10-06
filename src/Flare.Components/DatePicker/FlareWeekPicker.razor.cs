using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>Week field bound to a <see cref="DateOnly"/>: the first day of the chosen week. A click or Enter on any day
/// picks its week; the field writes the week by the culture's week rule ("Week 41, 2026") or by ISO 8601
/// ("2026-W41") and takes a week or a date typed in.</summary>
public partial class FlareWeekPicker
{
    /// <summary>The first day of the selected week (supports <c>@bind-Value</c>). A value on another day selects its
    /// week.</summary>
    [Parameter] public DateOnly? Value { get; set; }
    /// <summary>Raised with the first day of the week the user commits (picked, typed or cleared).</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }
    /// <summary>Model field bound inside an <c>EditForm</c>: changes reach the edit context and its validation message is shown.</summary>
    [Parameter] public Expression<Func<DateOnly?>>? For { get; set; }
    /// <summary>Numbers weeks by ISO 8601 - weeks from Monday, week 1 holds the first Thursday, the ISO year - and
    /// writes them as <c>2026-W41</c>. Default false: the culture's week rule and first day of week.</summary>
    [Parameter] public bool IsoWeeks { get; set; }
    /// <summary>Earliest date that can be picked: a week with no day on or after it cannot be picked.</summary>
    [Parameter] public DateOnly? Min { get; set; }
    /// <summary>Latest date that can be picked: a week with no day on or before it cannot be picked.</summary>
    [Parameter] public DateOnly? Max { get; set; }
    /// <summary>Predicate that disables specific dates; a week whose every day is disabled cannot be picked.</summary>
    [Parameter] public Func<DateOnly, bool>? IsDateDisabled { get; set; }
    /// <summary>Culture for the calendar, the week rule and the field. Default = CurrentUICulture. The calendar follows
    /// the culture's own (Persian for fa-IR); ISO weeks are always Gregorian.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>The calendar to show dates on instead of the culture's own; only one the culture offers among its
    /// optional calendars is used. Null uses the culture's calendar.</summary>
    [Parameter] public Calendar? Calendar { get; set; }
    /// <summary>Overrides the culture's first day of week (null = use the culture's). Ignored with <see cref="IsoWeeks"/>,
    /// where weeks start on Monday.</summary>
    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }
    /// <summary>Shows the week-number column in the calendar. Default true.</summary>
    [Parameter] public bool ShowWeekNumbers { get; set; } = true;
    /// <summary>The calendar view the picker opens to (Day/Month/Year).</summary>
    [Parameter] public PickerOpenTo OpenTo { get; set; } = PickerOpenTo.Day;
    /// <summary>Allows typing a week or a date directly into the field. Default true.</summary>
    [Parameter] public bool AllowInput { get; set; } = true;
    /// <summary>Allows opening the calendar popup (shows the calendar icon button). Default true.</summary>
    [Parameter] public bool AllowPicker { get; set; } = true;
    /// <summary>Closes the popup when a week is picked. Default true. Ignored when <see cref="Inline"/>.</summary>
    [Parameter] public bool AutoClose { get; set; } = true;
    /// <summary>Renders the calendar inline (always visible under the field) rather than in a popup.</summary>
    [Parameter] public bool Inline { get; set; }
    /// <summary>Returns extra CSS class(es) for a given day cell (e.g. to mark holidays).</summary>
    [Parameter] public Func<DateOnly, string>? DayClassFunc { get; set; }
    /// <summary>Content of a day cell; the picker keeps the cell itself. Null shows the day number.</summary>
    [Parameter] public RenderFragment<DateOnly>? DayTemplate { get; set; }
    /// <summary>Shows the Clear button in the calendar footer. Default true.</summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;
    /// <summary>Shows the This week button, which picks the current week. Default true.</summary>
    [Parameter] public bool ShowThisWeekButton { get; set; } = true;
    /// <summary>Override text for the Clear button. When null, falls back to the localizer key Picker_Clear.</summary>
    [Parameter] public string? ClearText { get; set; }
    /// <summary>Override text for the This week button. When null, falls back to the localizer key Picker_ThisWeek.</summary>
    [Parameter] public string? ThisWeekText { get; set; }
    /// <summary>Forces the error visual state without an error message (e.g. driven by external validation).</summary>
    [Parameter] public bool HasError { get; set; }
    /// <summary>Requests focus on the input after the first render (best-effort).</summary>
    [Parameter] public bool Autofocus { get; set; }
    /// <summary>Raised when the calendar popup opens.</summary>
    [Parameter] public EventCallback Opened { get; set; }
    /// <summary>Raised when the calendar popup closes.</summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.WeekPicker.Root;

    /// <summary>Opens the calendar popup.</summary>
    public Task OpenAsync() => SetOpenAsync(true);
    /// <summary>Closes the calendar popup.</summary>
    public Task CloseAsync() => SetOpenAsync(false);
    /// <summary>Clears the selected week.</summary>
    public Task ClearAsync() => ClearValue();
    /// <summary>Sets keyboard focus to the input.</summary>
    public ValueTask FocusAsync() => _inputEl.FocusAsync();

    private static readonly Regex IsoText = new(@"^\s*(\d{4})-?[Ww](\d{1,2})\s*$", RegexOptions.CultureInvariant);

    private ElementReference _inputEl;
    private ElementReference _fieldEl;
    private ElementReference _panelEl;
    private ElementReference _toggleEl;
    private FlareMonthGrid? _grid;
    private FlareFieldChrome _chrome = default!;
    private PickerPopup? _popupState;
    private PickerPopup _popup => _popupState ??= new PickerPopup(Overlay, $"flare-weekpicker-{Guid.NewGuid():N}");
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
    private DateOnly? _syncedValue;
    private string? _syncedText;
    private DateOnly? _focusedDate;

    private CultureInfo _culture => CalendarMath.PickerCulture(Culture ?? CultureInfo.CurrentUICulture, Calendar);
    private DayOfWeek _firstDay => IsoWeeks ? DayOfWeek.Monday : FirstDayOfWeek ?? _culture.DateTimeFormat.FirstDayOfWeek;
    private DateOnly Today => DateOnly.FromDateTime(TimeProvider.GetLocalNow().DateTime);
    private DateOnly? _start => Value is { } v ? WeekStart(v) : null;
    private DateOnly FocusedCursor => _focusedDate ?? _start ?? Today;
    private bool _invalid => HasError || !string.IsNullOrEmpty(DisplayedErrorText);
    private string _format => IsoWeeks ? "yyyy-Www" : string.Format(_culture, FlareStrings.WeekPicker_Value, "N", _culture.DateTimeFormat.Calendar.GetYear(DateTime.Today));
    private string DisplayValue => _start is { } s ? WeekText(s) : string.Empty;

    private DateOnly WeekStart(DateOnly day) => day.AddDays(-(((int)day.DayOfWeek - (int)_firstDay + 7) % 7));

    // "Week 41, 2026" by the culture's rule on its calendar, or "2026-W41" by ISO 8601.
    private string WeekText(DateOnly start)
    {
        var at = start.ToDateTime(TimeOnly.MinValue);
        if (IsoWeeks) return $"{ISOWeek.GetYear(at):0000}-W{ISOWeek.GetWeekOfYear(at):00}";
        var calendar = _culture.DateTimeFormat.Calendar;
        var year = at >= calendar.MinSupportedDateTime && at <= calendar.MaxSupportedDateTime ? calendar.GetYear(at) : start.Year;
        return string.Format(_culture, FlareStrings.WeekPicker_Value, CalendarMath.WeekOfYear(start, _culture, _firstDay), year);
    }

    private bool IsDisabled(DateOnly d) =>
        (Min is { } min && d < min) || (Max is { } max && d > max) || (IsDateDisabled?.Invoke(d) ?? false);

    private bool WeekAvailable(DateOnly start) => Enumerable.Range(0, 7).Any(i => !IsDisabled(start.AddDays(i)));

    private bool IsDayUnavailable(DateOnly d) => Disabled || ReadOnly || IsDisabled(d);

    private bool IsSelected(DateOnly d) => _start is { } s && d >= s && d <= s.AddDays(6);

    // The chosen week painted whole with the range classes: its first day, its middle days and its last day.
    private string ComposeDayClass(DateOnly d)
    {
        var week = _start is { } s && d >= s && d <= s.AddDays(6)
            ? d == s ? Css.Classes.Daterangepicker.DayStart
              : d == s.AddDays(6) ? Css.Classes.Daterangepicker.DayEnd
              : Css.Classes.Daterangepicker.DayInRange
            : null;
        var extra = DayClassFunc?.Invoke(d);
        return string.Join(' ', new[] { week, extra }.Where(c => !string.IsNullOrEmpty(c)));
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // The value's own day anchors the view: its week may start in the month before.
        _views.ShowMonthOf(Value ?? Today);
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
        var display = DisplayValue;
        if (!Equals(Value, _syncedValue) && Value is { } shown) _views.ShowMonthOf(shown);
        if (!Equals(Value, _syncedValue) || (!_focused && display != _syncedText))
        {
            _syncedValue = Value;
            _syncedText = display;
            _text = display;
        }
    }

    private async Task SetOpenAsync(bool open)
    {
        if (Inline || _open == open || (open && (Disabled || ReadOnly))) return;
        _open = open;
        if (open)
        {
            _views.ShowMonthOf(Value ?? Today);
            _views.View = InitialView;
            await Opened.InvokeAsync();
        }
        else await Closed.InvokeAsync();
        StateHasChanged();
    }

    private Task Toggle() => SetOpenAsync(!_open);

    // A click in the field opens the popup as well; focus stays in the field (PickerPopup.FromField), so the user
    // can go on typing. Arrow Down moves into the popup; Escape, or Tab out of the field, closes it.
    private Task OpenFromField()
    {
        if (_open || Inline || !AllowPicker || Disabled || ReadOnly) return Task.CompletedTask;
        _popup.OpenFromField();
        return SetOpenAsync(true);
    }

    // Wired only while a field-opened popup is showing, so typing into a closed field raises no key events.
    private EventCallback<KeyboardEventArgs> FieldKeyDown => _open && _popupState?.FromField == true
        ? EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleFieldKeyDown)
        : default;

    private Task HandleFieldKeyDown(KeyboardEventArgs e) => e.Key switch
    {
        "ArrowDown" => _popup.EnterAsync(_panelEl, () => _grid?.FocusCursorAsync() ?? Task.CompletedTask),
        "Escape" or "Tab" => SetOpenAsync(false),
        _ => Task.CompletedTask,
    };
    private Task Close() => SetOpenAsync(false);

    private async Task PickWeek(DateOnly day)
    {
        if (Disabled || ReadOnly || IsDisabled(day)) return;
        await CommitAsync(WeekStart(day));
        if (AutoClose) await SetOpenAsync(false);
    }

    private async Task GoToThisWeek()
    {
        if (Disabled) return;
        _views.GoTo(Today);
        if (ReadOnly || !WeekAvailable(WeekStart(Today))) return;
        await CommitAsync(WeekStart(Today));
        if (AutoClose) await SetOpenAsync(false);
    }

    private async Task ClearValue()
    {
        if (Disabled || ReadOnly) return;
        await CommitAsync(null);
        await SetOpenAsync(false);
    }

    // The view stays where the user picked; a typed week moves it to the typed day before the commit.
    private async Task CommitAsync(DateOnly? start)
    {
        _syncedValue = start;
        await ValueChanged.InvokeAsync(start);
        NotifyFieldChanged();
    }

    private async Task HandleGridKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape") { await ClosePopupAsync(); return; }
        if (Disabled || ReadOnly) return;
        // Up/Down move by a week; Enter/Space are left to the native click of the focused day (TASK-124).
        var next = CalendarMath.KeyTarget(FocusedCursor, e.Key, e.ShiftKey, _firstDay, IsDisabled, _views.Grid);
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

    private void HandleFocus() => _focused = true;

    private void HandleInput(ChangeEventArgs e) => _text = e.Value?.ToString() ?? string.Empty;

    // An ISO week ("2026-W41", "2026w41") or any date the culture reads; its week must have a day that can be picked.
    private async Task HandleTextChange(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(raw)) { _text = raw; await CommitAsync(null); return; }
        DateOnly day;
        if (IsoText.Match(raw) is { Success: true } m && int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture) is var week
            && week >= 1 && week <= ISOWeek.GetWeeksInYear(int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture)))
            day = DateOnly.FromDateTime(ISOWeek.ToDateTime(int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), week, DayOfWeek.Monday));
        else if (!DateOnly.TryParse(raw, _culture, DateTimeStyles.None, out day)) { _text = DisplayValue; return; }
        var start = WeekStart(day);
        if (!WeekAvailable(start)) { _text = DisplayValue; return; }
        _views.ShowMonthOf(day);
        await CommitAsync(start);
        _text = DisplayValue;
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
            () => _grid?.FocusCursorAsync() ?? Task.CompletedTask, dismissRoot: _chrome.Root, dismiss: () => InvokeAsync(Close));
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await _popup.DisposeAsync();
        await base.DisposeAsync();
    }
}
