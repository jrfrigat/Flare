# Date/Time-family audit - follow-ups

**Source:** cross-framework audit of Flare's date/time picker family vs the locally-installed
MudBlazor / Blazorise / Fluent UI Blazor (2026-07-11). Components: `FlareDatePicker`,
`FlareDateRangePicker`, `FlareDateTimePicker`, `FlareTimePicker` (+ shared `FlareMonthGrid`,
`CalendarMath`). `FlareCalendar` is a separate event/scheduling calendar, out of scope.

**Where Flare already leads:** built-in range presets (`DateRangePreset` / `DefaultPresets`) - none of
the three ship these; a dedicated `FlareDateRangePicker` (Fluent has none); `Min`/`Max` (Fluent has
none); modern `DateOnly`/`TimeOnly`/`DateTimeOffset` types; `IsDateDisabled` predicate. The audit
closed the one systemic gap - no public imperative API - plus the calendar/time breadth items.

## Shipped on `main` (items 1-8)

1. **Public imperative API** on `FlareDatePicker` / `FlareTimePicker` / `FlareDateTimePicker`:
   `OpenAsync()`, `CloseAsync()`, `ToggleAsync()`, `ClearAsync()`, `FocusAsync()`.
2. **`OpenTo`** (`PickerOpenTo` Day/Month/Year) on `FlareDatePicker` - opens straight to the year grid
   for far-back dates (date of birth). The Day/Month/Year views already existed; this just picks the
   initial one.
3. **`AutoClose`** on `FlareDatePicker` (default true - closes on select) and `FlareTimePicker`
   (default false - confirm+close on the last unit without OK).
4. **`Opened` / `Closed`** events on `FlareDatePicker` / `FlareTimePicker` / `FlareDateTimePicker`.
5. **`ShowWeekNumbers`** (leading week-of-year column) and an explicit nullable **`FirstDayOfWeek`**
   override (null = culture's) on `FlareMonthGrid` and every date picker. `FirstDayOfWeek` earns its
   keep over `Culture` alone: a Monday-first calendar regardless of an en-US locale would otherwise
   require cloning/mutating a `CultureInfo`.
6. **`FlareTimePicker`**: **`ShowSeconds`** (seconds column in the Dropdown variant + HH:mm:ss entry),
   **`Min`/`Max`** time (out-of-range cells disabled + clamp), **`HourStep`**.
7. **`Inline`** on `FlareDatePicker` - renders the calendar in normal flow (no popup/scrim/anchoring).
8. **`DayClassFunc`** (`Func<DateOnly,string>`) on the date pickers - custom per-day CSS (holidays,
   highlights), composed with the built-in selection/range classes.

Propagation: `FlareDateTimePicker` gets the imperative API + `Opened`/`Closed` + calendar customization
(`ShowWeekNumbers`/`FirstDayOfWeek`/`DayClassFunc`). `FlareDateRangePicker` is inline-by-design (no popup,
so imperative/`Opened`/`Closed`/`AutoClose`/`OpenTo` are N/A) but gained the calendar customization on
both its Calendar-mode grid and its inner Fields-mode pickers.

## Deferred

### 9 - `Autofocus` on the pickers - DONE
Blazorise + Fluent auto-focus a picker on render. Flare pickers inherit `FlareFieldBase` (not the
editable `FlareEditableFieldBase`), so they never got the field-family `Autofocus`. **Shipped:** an
`Autofocus` bool on `FlareDatePicker`, `FlareTimePicker` and `FlareDateTimePicker` (the three input-backed
pickers) - it calls `FocusAsync()` once on first render (best-effort, guarded). The range picker
(`FlareComponentBase`, no single input) is out of scope. Per-picker, matching the imperative-API pattern.

### 10 - MudBlazor-only richness - LOW
Not shipped (Mud-only, marginal for Flare's model):
- **Multi-date selection** (`SelectMode.Multiple` / `SelectedDates`) - Blazorise + Fluent Calendar have it;
  Flare's pickers are single-value + a dedicated range picker. Niche.
- **`DisplayMonths` / `MaxMonthColumns`** (multi-month side-by-side view).
- **`PickerActions` / custom toolbar templates**, **`ShowToolbar`**.
- **`GoToDate()` / `ScrollToYearAsync()`** imperative navigation helpers.
- **`TimeEditMode`** (OnlyHours / OnlyMinutes).
- **`OpenTo` on `FlareDateTimePicker`** and **time seconds/min-max on the DateTimePicker's time pane** -
  its calendar is a simple prev/next month grid and its time pane is minute-precision; adding the
  Day/Month/Year view switching + seconds there is a larger change than the composite warrants now.
