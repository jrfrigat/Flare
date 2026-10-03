using System.Globalization;
using System.Linq.Expressions;
using Flare.Components.Resources;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components;

/// <summary>Time field with a masked text entry and a clock-dial or column popup, bound to a <see cref="TimeOnly"/>.</summary>
public partial class FlareTimePicker
{
    /// <summary>Currently selected time.</summary>
    [Parameter] public TimeOnly? Value { get; set; }
    /// <summary>Callback invoked when the selected time changes.</summary>
    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }
    // Label, Placeholder, HelperText, ErrorText, Disabled, ReadOnly and Required are inherited from
    // FlareFieldBase (IFlareField). ReadOnly keeps the trigger focusable but blocks typing and the popup.
    /// <summary>Popup style: an analog clock <see cref="TimePickerVariant.Dial"/> (default) or <see cref="TimePickerVariant.Dropdown"/>
    /// columns. Both variants are offered by every theme; where a design system specifies something else (a keyboard-entry
    /// mode inside the dial dialog, a single combobox list of times), the variant's description says so.</summary>
    [Parameter] public TimePickerVariant PopupVariant { get; set; } = TimePickerVariant.Dial;
    /// <summary>Forces a 12-hour (AM/PM) or 24-hour clock, for the field as well as the popup: on a 12-hour clock the field
    /// shows and takes "hh:mm AM". Null (default) follows the <see cref="Culture"/> short time pattern.</summary>
    [Parameter] public bool? Use24Hour { get; set; }
    /// <summary>Culture for the 12/24-hour default and the AM/PM designators. Default = CurrentUICulture.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }
    /// <summary>Minute increment shown in the Dropdown column. Default 1.</summary>
    [Parameter] public int MinuteStep { get; set; } = 1;
    /// <summary>Headline shown at the top of the picker popup. When null, falls back to the localized default.</summary>
    [Parameter] public string? Headline { get; set; }
    /// <summary>Confirm button text. When null, falls back to the localized "OK".</summary>
    [Parameter] public string? OkText { get; set; }
    /// <summary>Cancel button text. When null, falls back to the localized "Cancel".</summary>
    [Parameter] public string? CancelText { get; set; }
    /// <summary>Expression used to bind and validate the field.</summary>
    [Parameter] public Expression<Func<TimeOnly?>>? For { get; set; }
    /// <summary>Adds a seconds column (Dropdown variant) and the seconds to the field: HH:mm:ss, or hh:mm:ss AM on a
    /// 12-hour clock. Default false.</summary>
    [Parameter] public bool ShowSeconds { get; set; }
    /// <summary>Hour increment shown in the Dropdown column. Default 1.</summary>
    [Parameter] public int HourStep { get; set; } = 1;
    /// <summary>Earliest selectable time (inclusive); out-of-range cells are disabled.</summary>
    [Parameter] public TimeOnly? Min { get; set; }
    /// <summary>Latest selectable time (inclusive); out-of-range cells are disabled.</summary>
    [Parameter] public TimeOnly? Max { get; set; }
    /// <summary>Confirms and closes as soon as the last time unit is selected (no OK press): the minute on
    /// the dial (released or typed), the minute or second column in the dropdown. A time outside
    /// Min/Max is not confirmed and the popup stays open. Default false.</summary>
    [Parameter] public bool AutoClose { get; set; }
    /// <summary>Shows the button in the dial popup that switches between the clock dial and keyboard entry (an hour
    /// and a minute text field). Default true. Each opening starts on the dial; the dropdown popup has no switch,
    /// as its columns already take typed digits.</summary>
    [Parameter] public bool ShowKeyboardToggle { get; set; } = true;
    /// <summary>Requests focus on the time input after the first render (best-effort). Only one field per
    /// page should set this.</summary>
    [Parameter] public bool Autofocus { get; set; }
    /// <summary>Raised when the picker popup opens.</summary>
    [Parameter] public EventCallback Opened { get; set; }
    /// <summary>Raised when the picker popup closes.</summary>
    [Parameter] public EventCallback Closed { get; set; }

    /// <inheritdoc />
    protected override string ComponentCssClass => Css.Classes.TimePicker.Root;

    private bool _open;
    private int _tempHour;
    private int _tempMinute;
    private int _tempSecond;
    private string _text = string.Empty;
    private TimeOnly? _syncedValue;
    private string? _syncedText;
    private bool _is24Hour;
    // Caret position to restore after the next render (an auto-separator rewrite moved it to the end);
    // -1 when there is nothing to restore.
    private int _pendingCaret = -1;
    private bool _focused;
    // Bumped by every input, change, blur, external re-sync and dispose; an input handler that resumes from
    // its awaited selection read in an older generation is stale and drops its result (TASK-138).
    private int _editGeneration;

    private void HandleBlur()
    {
        _focused = false;
        _editGeneration++;
    }

    private ElementReference _inputEl;
    private ElementReference _fieldEl;

    private int _hourStep => HourStep < 1 ? 1 : HourStep;

    /// <summary>Opens the picker popup.</summary>
    public Task OpenAsync() { if (!_open) return Toggle(); return Task.CompletedTask; }
    /// <summary>Closes the picker popup.</summary>
    public Task CloseAsync() => Close();
    /// <summary>Toggles the picker popup.</summary>
    public Task ToggleAsync() => Toggle();
    /// <summary>Clears the selected time and closes the popup. Does nothing while the field is disabled or read-only.</summary>
    public async Task ClearAsync() { if (_locked) return; await Commit(null); await Close(); }
    /// <summary>Sets keyboard focus to the time input.</summary>
    public ValueTask FocusAsync() => _inputEl.FocusAsync();

    private bool TimeInRange(TimeOnly t) => (!Min.HasValue || t >= Min.Value) && (!Max.HasValue || t <= Max.Value);
    private bool HourDisabled(int h) => (Min.HasValue && new TimeOnly(h, 59, 59) < Min.Value) || (Max.HasValue && new TimeOnly(h, 0, 0) > Max.Value);
    private bool MinuteDisabled(int m) => (Min.HasValue && new TimeOnly(_tempHour, m, 59) < Min.Value) || (Max.HasValue && new TimeOnly(_tempHour, m, 0) > Max.Value);
    private bool SecondDisabled(int s) => !TimeInRange(new TimeOnly(_tempHour, _tempMinute, s));

    private async Task SelectMinute(int m)
    {
        _tempMinute = m;
        if (AutoClose && !ShowSeconds) await Confirm();
    }

    private async Task SelectSecond(int s)
    {
        _tempSecond = s;
        if (AutoClose) await Confirm();
    }

    // The dial edits hours and minutes only, so the settled minute is its last unit (TASK-112).
    private async Task OnDialMinuteSelected(int m)
    {
        _tempMinute = m;
        if (AutoClose) await Confirm();
    }
    private ElementReference _panelEl;
    private ElementReference _toggleEl;
    private PickerPopup? _popupState;
    private PickerPopup _popup => _popupState ??= new PickerPopup(Overlay, $"flare-timepicker-{Guid.NewGuid():N}");

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (MinuteStep < 1) MinuteStep = 1;
        _is24Hour = Use24Hour ?? !_culture.DateTimeFormat.ShortTimePattern.Contains('h');
        UpdateFieldIdentifier(For);

        // A new value always re-syncs the text; a new display (ShowSeconds) only outside editing (TASK-113).
        var display = Value is { } v ? FormatTime(v) : string.Empty;
        if (!Equals(Value, _syncedValue) || (!_focused && display != _syncedText))
        {
            _syncedValue = Value;
            _syncedText = display;
            _text = display;
            _editGeneration++;
        }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        _editGeneration++;
        await _popup.DisposeAsync();
        await base.DisposeAsync();
    }

    private string _headline => Headline ?? (_keyboardEntry ? FlareStrings.TimePicker_EnterTime : FlareStrings.TimePicker_Headline);
    private bool _showKeyboardToggle => ShowKeyboardToggle && PopupVariant == TimePickerVariant.Dial;

    // The view that mounts takes focus itself: the dial its root, the entry its hour field.
    private void ToggleKeyboardEntry()
    {
        _keyboardEntry = !_keyboardEntry;
        _entryInvalid = false;
    }

    // Invalid drives the frame's error chrome (a resolved validation message).
    private bool _invalid => !string.IsNullOrEmpty(DisplayedErrorText);

    private async Task HandleInput(ChangeEventArgs e)
    {
        var raw = e.Value?.ToString() ?? string.Empty;
        var generation = ++_editGeneration;
        var caretDigits = await MaskedCaret.DigitsBeforeAsync(ElementJs, _inputEl, raw);
        if (generation != _editGeneration) return;
        _text = MaskTime(raw);
        if (caretDigits >= 0) _pendingCaret = MaskedInput.CaretAfterDigit(_text, caretDigits);
        if (_text.Count(char.IsAsciiDigit) == (ShowSeconds ? 6 : 4) && TryParseTime(_text, out var shown)
            && Compose(shown) is var t && TimeInRange(t))
            await Commit(t);
    }

    private async Task HandleCommitText(ChangeEventArgs e)
    {
        _editGeneration++;
        _text = MaskTime(e.Value?.ToString());
        if (string.IsNullOrEmpty(_text)) { await Commit(null); return; }
        // The text is already on the HH:mm[:ss] skeleton, so a complete time parses exactly; a lenient parse
        // would only ever accept a half-edit ("12:3" as 12:03, TASK-126). Incomplete, unparsable or
        // out-of-range text is not committed and the field snaps back to the current value (TASK-102).
        if (TryParseTime(_text, out var shown)
            && Compose(shown) is var t && TimeInRange(t))
        {
            _text = FormatTime(t);
            await Commit(t);
        }
        else
        {
            _text = Value is { } v ? FormatTime(v) : string.Empty;
        }
    }

    // The mask lives in the shared, unit-tested MaskedInput helper (TASK-118): it lays the digits out on
    // the HH:mm[:ss] skeleton without clamping, so an incomplete edit never rewrites a neighbour segment.
    // Out-of-range input simply fails to parse and is not committed.

    private ElementReference _dropRef;
    private int _dropActive;   // 0 = hour, 1 = minute (Dropdown keyboard)
    private string _dropBuf = string.Empty;
    private bool _focusDrop;
    // The dial popup shows FlareTimeEntry instead of the dial (TASK-130).
    private bool _keyboardEntry;
    // A keyboard-entry field holds a number out of range: the draft is the last valid time, so OK waits.
    private bool _entryInvalid;
    private bool _autofocused;

    private async Task Toggle()
    {
        if (Disabled || ReadOnly) return;
        if (_isList) { await ToggleListAsync(); return; }
        if (!_open)
        {
            _tempHour = Value?.Hour ?? 0;
            _tempMinute = Value?.Minute ?? 0;
            _tempSecond = Value?.Second ?? 0;
            ClampTemp();
            _dropActive = 0;
            _dropBuf = string.Empty;
            _focusDrop = PopupVariant == TimePickerVariant.Dropdown;
            _keyboardEntry = false;
            _entryInvalid = false;
            _open = true;
            await Opened.InvokeAsync();
        }
        else
        {
            _open = false;
            await Closed.InvokeAsync();
        }
        StateHasChanged();
    }

    private void ClampTemp()
    {
        var t = new TimeOnly(_tempHour, _tempMinute, _tempSecond);
        var clamp = Min.HasValue && t < Min.Value ? Min.Value
                  : Max.HasValue && t > Max.Value ? Max.Value
                  : (TimeOnly?)null;
        if (clamp is { } c) { _tempHour = c.Hour; _tempMinute = c.Minute; _tempSecond = c.Second; }
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
            // Only restore while the field still holds focus: the selection read is awaited, so the user may
            // have left the field meanwhile, and pulling focus back onto it would be wrong (TASK-120).
            if (_focused) await MaskedCaret.RestoreAsync(ElementJs, _inputEl, _text, caret);
        }

        // The popup sits under the field in the top layer (escaping a Card's overflow:hidden) and holds Tab
        // while open; the scrim handles dismissal (TASK-134).
        if (_isList) await SyncListAsync();
        else await _popup.SyncAsync(_open, _fieldEl, _panelEl, null, _inputEl, _toggleEl);

        if (_focusDrop)
        {
            _focusDrop = false;
            try { await _dropRef.FocusAsync(); } catch { /* best-effort */ }
        }
        await ScrollDropActiveAsync();
    }

    // The columns announce the selected cell of the active column through aria-activedescendant (TASK-131);
    // a value off the step grid has no cell, so nothing is announced until a cell is picked.
    private readonly string _columnsId = $"flare-timepicker-cols-{Guid.NewGuid():N}";
    private string CellId(char unit, int n) => $"{_columnsId}-{unit}-{n}";
    private string CellClass(bool selected, int column) =>
        Css.Classes.TimePicker.Cell
        + (selected ? " " + Css.Classes.TimePicker.CellSelected : "")
        + (selected && column == _dropActive ? " " + Css.Classes.TimePicker.CellActive : "");
    private string? _columnsActiveId => _dropActive switch
    {
        0 when _tempHour % _hourStep == 0 => CellId('h', _tempHour),
        1 when MinuteStep > 0 && _tempMinute % MinuteStep == 0 => CellId('m', _tempMinute),
        2 => CellId('s', _tempSecond),
        _ => null,
    };

    private async Task Close()
    {
        if (!_open) return;
        _open = false;
        await Closed.InvokeAsync();
        StateHasChanged();
    }

    // Escape anywhere in the popup drops the pick and puts focus back in the field (TASK-134).
    private async Task HandlePanelKeyDown(KeyboardEventArgs e)
    {
        if (e.Key != "Escape") return;
        _popup.ReturnToField();
        await Close();
    }

    private TimeOnly TempTime => Compose(new(_tempHour, _tempMinute, ShowSeconds ? _tempSecond : 0));

    // The value's ticks below the smallest unit the field and popup show. Editing what is shown keeps them, so
    // confirming without a change does not move the value (TASK-159, as TASK-136 for the date-time picker).
    private long HiddenTicks => Value is { } v ? v.Ticks % (ShowSeconds ? TimeSpan.TicksPerSecond : TimeSpan.TicksPerMinute) : 0;

    // The shown time plus the hidden ticks - unless those alone push it past Min/Max, so typing exactly Max
    // stays possible when the value carries unseen seconds.
    private TimeOnly Compose(TimeOnly shown)
    {
        var withHidden = shown.Add(TimeSpan.FromTicks(HiddenTicks));
        return TimeInRange(withHidden) || !TimeInRange(shown) ? withHidden : shown;
    }

    // Min/Max bound the popup too, not only the disabled cells (TASK-102): OK is offered only for a time
    // inside the range, and a refused confirm (AutoClose included) keeps the popup open (TASK-125).
    // A field locked after the popup opened takes no value from it; the popup still closes (TASK-173).
    private bool _locked => Disabled || ReadOnly;
    private bool CanConfirm => !_locked && TimeInRange(TempTime) && !(_keyboardEntry && _entryInvalid);

    private async Task Confirm()
    {
        if (!CanConfirm) return;
        var t = TempTime;
        _open = false;
        await Closed.InvokeAsync();
        await Commit(t);
    }

    private async Task Commit(TimeOnly? value)
    {
        _syncedValue = value;
        _text = value is { } v ? FormatTime(v) : string.Empty;
        await ValueChanged.InvokeAsync(value);
        NotifyFieldChanged();
    }
}
