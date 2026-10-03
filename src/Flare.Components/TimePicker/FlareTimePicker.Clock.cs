using System.Globalization;

namespace Flare.Components;

// The clock the field writes and reads (TASK-172): 24-hour "HH:mm[:ss]", or on a 12-hour clock "hh:mm[:ss] AM" with the
// culture's designators. The separators are literal colons, as the digit mask writes them.
public partial class FlareTimePicker
{
    private CultureInfo _culture => Culture ?? CultureInfo.CurrentUICulture;
    private string _am => Nz(_culture.DateTimeFormat.AMDesignator, "AM");
    private string _pm => Nz(_culture.DateTimeFormat.PMDesignator, "PM");

    private string _timeFormat => (_is24Hour, ShowSeconds) switch
    {
        (true, false) => "HH':'mm",
        (true, true) => "HH':'mm':'ss",
        (false, false) => "hh':'mm tt",
        (false, true) => "hh':'mm':'ss tt",
    };

    private string _timePlaceholder => (_is24Hour ? "HH:mm" : "hh:mm") + (ShowSeconds ? ":ss" : "") + (_is24Hour ? "" : " " + _am);

    private string FormatTime(TimeOnly t) => _is24Hour ? t.ToString(_timeFormat, CultureInfo.InvariantCulture) : Format12(t);

    // A culture without designators (24-hour cultures) still gets readable AM/PM on a forced 12-hour clock.
    private string Format12(TimeOnly t) =>
        $"{t.ToString(ShowSeconds ? "hh':'mm':'ss" : "hh':'mm", CultureInfo.InvariantCulture)} {(t.Hour >= 12 ? _pm : _am)}";

    private bool TryParseTime(string text, out TimeOnly shown)
    {
        if (_is24Hour) return TimeOnly.TryParseExact(text, _timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out shown);
        shown = default;
        var time = MaskedInput.MaskTime(text, ShowSeconds);
        var period = text.Length > time.Length ? text[time.Length..].Trim() : string.Empty;
        var isPm = string.Equals(period, _pm, StringComparison.OrdinalIgnoreCase);
        if (!isPm && !string.Equals(period, _am, StringComparison.OrdinalIgnoreCase)) return false;
        if (!TimeOnly.TryParseExact(time, ShowSeconds ? "hh':'mm':'ss" : "hh':'mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var twelve)) return false;
        shown = twelve.Hour == 12 ? (isPm ? twelve : twelve.AddHours(-12)) : (isPm ? twelve.AddHours(12) : twelve);
        return true;
    }

    private string MaskTime(string? raw) => _is24Hour
        ? MaskedInput.MaskTime(raw, ShowSeconds)
        : MaskedInput.MaskTime12(raw, ShowSeconds, _am, _pm, Value is { Hour: >= 12 });

    private static string Nz(string? s, string fallback) => string.IsNullOrEmpty(s) ? fallback : s;
}
