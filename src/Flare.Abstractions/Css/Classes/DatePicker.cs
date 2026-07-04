namespace Flare.Css.Classes;

/// <summary>CSS classes for date picker.</summary>
public static class DatePicker
{
    /// <summary>The <c>flare-datepicker</c> CSS class.</summary>
    public const string Root = "flare-datepicker";
    // Error + disabled state use the shared Css.Classes.Input.Error / Input.Disabled
    // (flare-input--error / --disabled); the field/control carry the shared flare-input__* classes.
    // The trigger field's click-to-open cursor uses the shared Css.Classes.Picker.Field (flare-picker__field);
    // the input itself is the pure shared Css.Classes.Input.Control (no datepicker-specific control class).
    // The calendar toggle button uses the shared Css.Classes.Input.Toggle (flare-input__toggle).
    /// <summary>The <c>flare-datepicker__panel</c> CSS class.</summary>
    public const string Panel = "flare-datepicker__panel";
    /// <summary>The <c>flare-datepicker__header</c> CSS class.</summary>
    public const string Header = "flare-datepicker__header";
    /// <summary>The <c>flare-datepicker__footer</c> CSS class.</summary>
    public const string Footer = "flare-datepicker__footer";
    /// <summary>The <c>flare-datepicker__month-label</c> CSS class.</summary>
    public const string MonthLabel = "flare-datepicker__month-label";
    /// <summary>The <c>flare-datepicker__month-label--clickable</c> CSS class.</summary>
    public const string MonthLabelClickable = "flare-datepicker__month-label--clickable";
    /// <summary>The <c>flare-datepicker__month-grid</c> CSS class.</summary>
    public const string MonthGrid = "flare-datepicker__month-grid";
    /// <summary>The <c>flare-datepicker__month-btn</c> CSS class.</summary>
    public const string MonthBtn = "flare-datepicker__month-btn";
    /// <summary>The <c>flare-datepicker__month-btn--selected</c> CSS class.</summary>
    public const string MonthSelected = "flare-datepicker__month-btn--selected";
    /// <summary>The <c>flare-datepicker__year-grid</c> CSS class.</summary>
    public const string YearGrid = "flare-datepicker__year-grid";
    /// <summary>The <c>flare-datepicker__year-btn</c> CSS class.</summary>
    public const string YearBtn = "flare-datepicker__year-btn";
    /// <summary>The <c>flare-datepicker__year-btn--selected</c> CSS class.</summary>
    public const string YearSelected = "flare-datepicker__year-btn--selected";
}

/// <summary>CSS classes for time picker.</summary>
public static class TimePicker
{
    /// <summary>The <c>flare-timepicker</c> CSS class.</summary>
    public const string Root = "flare-timepicker";
    // Error + disabled state use the shared Css.Classes.Input.Error / Input.Disabled. The field well is
    // the shared flare-input__field and the input is the pure shared Css.Classes.Input.Control (no
    // timepicker-specific field/control delta remains).
    // The clock toggle button uses the shared Css.Classes.Input.Toggle (flare-input__toggle).
    /// <summary>The <c>flare-timepicker__panel</c> CSS class.</summary>
    public const string Panel = "flare-timepicker__panel";
    /// <summary>The <c>flare-timepicker__panel-headline</c> CSS class.</summary>
    public const string PanelHeadline = "flare-timepicker__panel-headline";
    /// <summary>The <c>flare-timepicker__display</c> CSS class.</summary>
    public const string Display = "flare-timepicker__display";
    /// <summary>The <c>flare-timepicker__display-part</c> CSS class.</summary>
    public const string DisplayPart = "flare-timepicker__display-part";
    /// <summary>The <c>flare-timepicker__display-part--active</c> CSS class.</summary>
    public const string DisplayPartActive = "flare-timepicker__display-part--active";
    /// <summary>The <c>flare-timepicker__display-sep</c> CSS class.</summary>
    public const string DisplaySep = "flare-timepicker__display-sep";
    /// <summary>The <c>flare-timepicker__columns</c> CSS class.</summary>
    public const string Columns = "flare-timepicker__columns";
    /// <summary>The <c>flare-timepicker__columns-sep</c> CSS class.</summary>
    public const string ColumnsSep = "flare-timepicker__columns-sep";
    /// <summary>The <c>flare-timepicker__col</c> CSS class.</summary>
    public const string Col = "flare-timepicker__col";
    /// <summary>The <c>flare-timepicker__cell</c> CSS class.</summary>
    public const string Cell = "flare-timepicker__cell";
    /// <summary>The <c>flare-timepicker__cell--selected</c> CSS class.</summary>
    public const string CellSelected = "flare-timepicker__cell--selected";
    /// <summary>The <c>flare-timepicker__actions</c> CSS class.</summary>
    public const string Actions = "flare-timepicker__actions";
    /// <summary>The <c>flare-timepicker__panel--dial</c> CSS class.</summary>
    public const string PanelDial = "flare-timepicker__panel--dial";
}

/// <summary>CSS classes for clock dial.</summary>
public static class ClockDial
{
    /// <summary>The <c>flare-clock-dial</c> CSS class.</summary>
    public const string Root = "flare-clock-dial";
    /// <summary>The <c>flare-timepicker__dial-top</c> CSS class.</summary>
    public const string DialTop = "flare-timepicker__dial-top";
    /// <summary>The <c>flare-timepicker__time-fields</c> CSS class.</summary>
    public const string TimeFields = "flare-timepicker__time-fields";
    /// <summary>The <c>flare-timepicker__time-fields-sep</c> CSS class.</summary>
    public const string TimeFieldsSep = "flare-timepicker__time-fields-sep";
    /// <summary>The <c>flare-timepicker__time-field</c> CSS class.</summary>
    public const string TimeField = "flare-timepicker__time-field";
    /// <summary>The <c>flare-timepicker__time-field--active</c> CSS class.</summary>
    public const string TimeFieldActive = "flare-timepicker__time-field--active";
    /// <summary>The <c>flare-timepicker__period</c> CSS class.</summary>
    public const string Period = "flare-timepicker__period";
    /// <summary>The <c>flare-timepicker__period-btn</c> CSS class.</summary>
    public const string PeriodBtn = "flare-timepicker__period-btn";
    /// <summary>The <c>flare-timepicker__period-btn--selected</c> CSS class.</summary>
    public const string PeriodBtnSelected = "flare-timepicker__period-btn--selected";
    /// <summary>The <c>flare-timepicker__dial</c> CSS class.</summary>
    public const string Dial = "flare-timepicker__dial";
    /// <summary>The <c>flare-timepicker__dial-hand</c> CSS class.</summary>
    public const string Hand = "flare-timepicker__dial-hand";
    /// <summary>The <c>flare-timepicker__dial-center</c> CSS class.</summary>
    public const string Center = "flare-timepicker__dial-center";
    /// <summary>The <c>flare-timepicker__dial-num</c> CSS class.</summary>
    public const string Num = "flare-timepicker__dial-num";
    /// <summary>The <c>flare-timepicker__dial-num--selected</c> CSS class.</summary>
    public const string NumSelected = "flare-timepicker__dial-num--selected";
}

/// <summary>CSS classes for date time picker.</summary>
public static class DateTimePicker
{
    /// <summary>The <c>flare-datetimepicker</c> CSS class.</summary>
    public const string Root = "flare-datetimepicker";
    // The field well is the shared flare-input__field + flare-picker__field (click-to-open cursor) and
    // the input is the pure shared Css.Classes.Input.Control - identical to DatePicker, no delta class.
    // The picker toggle button uses the shared Css.Classes.Input.Toggle (flare-input__toggle);
    // label/helper/error text use the shared Css.Classes.Input.Label/Helper/HelperError.
    /// <summary>The <c>flare-datetimepicker__panel</c> CSS class.</summary>
    public const string Panel = "flare-datetimepicker__panel";
    /// <summary>The <c>flare-datetimepicker__panel--split</c> CSS class.</summary>
    public const string PanelSplit = "flare-datetimepicker__panel--split";
    /// <summary>The <c>flare-datetimepicker__panes</c> CSS class.</summary>
    public const string Panes = "flare-datetimepicker__panes";
    /// <summary>The <c>flare-datetimepicker__pane</c> CSS class.</summary>
    public const string Pane = "flare-datetimepicker__pane";
    /// <summary>The <c>flare-datetimepicker__tabs</c> CSS class.</summary>
    public const string Tabs = "flare-datetimepicker__tabs";
    /// <summary>The <c>flare-datetimepicker__tab</c> CSS class.</summary>
    public const string Tab = "flare-datetimepicker__tab";
    /// <summary>The <c>flare-datetimepicker__tab--active</c> CSS class.</summary>
    public const string TabActive = "flare-datetimepicker__tab--active";
    /// <summary>The <c>flare-datetimepicker__nav</c> CSS class.</summary>
    public const string Nav = "flare-datetimepicker__nav";
    /// <summary>The <c>flare-datetimepicker__nav-label</c> CSS class.</summary>
    public const string NavLabel = "flare-datetimepicker__nav-label";
    /// <summary>The <c>flare-datetimepicker__time</c> CSS class.</summary>
    public const string Time = "flare-datetimepicker__time";
    /// <summary>The <c>flare-datetimepicker__time-input</c> CSS class.</summary>
    public const string TimeInput = "flare-datetimepicker__time-input";
    /// <summary>The <c>flare-datetimepicker__footer</c> CSS class.</summary>
    public const string Footer = "flare-datetimepicker__footer";
    // Error + disabled state use the shared Css.Classes.Input.Error / Input.Disabled.
}
