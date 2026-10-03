using System.Globalization;
using System.Text;

namespace Flare.Components;

/// <summary>
/// Caret math for the masked date/time fields (FlareDatePicker, FlareDateTimePicker, FlareTimePicker).
/// Those fields rebuild their text on every keystroke so the auto-separator appears, which would leave the
/// caret at the end of the line; these helpers find the caret position by counting digits instead of
/// characters, so the caret stays on the segment the user was editing.
/// </summary>
internal static class MaskedInput
{
    /// <summary>Number of digits in <paramref name="text"/> that sit before character <paramref name="index"/>.</summary>
    public static int DigitsBefore(string text, int index)
    {
        if (index <= 0) return 0;
        if (index > text.Length) index = text.Length;
        var count = 0;
        for (var i = 0; i < index; i++)
            if (char.IsDigit(text[i])) count++;
        return count;
    }

    /// <summary>Index just after the <paramref name="digits"/>-th digit in <paramref name="masked"/>, or the
    /// end of the string when it holds fewer digits.</summary>
    public static int CaretAfterDigit(string masked, int digits)
    {
        if (digits <= 0) return 0;
        var seen = 0;
        for (var i = 0; i < masked.Length; i++)
        {
            if (!char.IsDigit(masked[i])) continue;
            seen++;
            if (seen == digits) return i + 1;
        }
        return masked.Length;
    }

    /// <summary>
    /// The all-digit date pattern a masked date field edits in, with the year, month and day segments in
    /// the order the culture's short date pattern uses them (<c>dd.MM.yyyy</c>, <c>MM/dd/yyyy</c>,
    /// <c>yyyy/MM/dd</c>, ...), joined by <paramref name="separator"/>.
    /// </summary>
    public static string NumericDatePattern(CultureInfo culture, string separator)
    {
        var shortPattern = culture.DateTimeFormat.ShortDatePattern;
        static int At(string p, char c) { var i = p.IndexOf(c); return i < 0 ? int.MaxValue : i; }
        var segments = new[] { ('d', "dd"), ('M', "MM"), ('y', "yyyy") }
            .OrderBy(s => At(shortPattern, s.Item1))
            .Select(s => s.Item2);
        return string.Join(separator, segments);
    }

    /// <summary>
    /// Lays the digits typed into a field out on <paramref name="pattern"/>: each letter of the pattern
    /// takes the next digit, and the separators between letters are inserted only once a digit follows
    /// them. Extra digits are dropped; nothing is clamped.
    /// </summary>
    public static string MaskByPattern(string? raw, string pattern)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;
        using var digits = raw.Where(char.IsDigit).GetEnumerator();
        var sb = new StringBuilder(pattern.Length);
        var separator = new StringBuilder();
        foreach (var c in pattern)
        {
            if (!char.IsLetter(c)) { separator.Append(c); continue; }
            if (!digits.MoveNext()) break;
            if (sb.Length > 0) sb.Append(separator);
            separator.Clear();
            sb.Append(digits.Current);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Lays a time typed into the field out on the <c>HH:mm</c> (or <c>HH:mm:ss</c>) skeleton: the digits
    /// fill the segments in order and the separators are inserted. The values are NOT clamped or corrected -
    /// an incomplete edit must never rewrite the neighbouring segment (TASK-118); validation happens when
    /// the field is committed.
    /// </summary>
    public static string MaskTime(string? raw, bool showSeconds)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;
        var digits = new string(raw.Where(char.IsDigit).Take(showSeconds ? 6 : 4).ToArray());
        if (digits.Length <= 2) return digits;
        if (digits.Length <= 4) return $"{digits[..2]}:{digits[2..]}";
        return $"{digits[..2]}:{digits[2..4]}:{digits[4..]}";
    }

    /// <summary>
    /// The 12-hour form of <see cref="MaskTime"/>: the digits fill <c>hh:mm</c> (or <c>hh:mm:ss</c>) and, once all are
    /// typed, the period follows. A letter typed beside the designator already shown - the one left once that
    /// designator is taken out - picks it when it starts only one of <paramref name="am"/> and <paramref name="pm"/>
    /// (a, p); otherwise the designator spelled out in the text does, and otherwise <paramref name="currentPm"/> keeps
    /// the period the value already has.
    /// </summary>
    public static string MaskTime12(string? raw, bool showSeconds, string am, string pm, bool currentPm)
    {
        var time = MaskTime(raw, showSeconds);
        if (string.IsNullOrEmpty(raw) || raw.Count(char.IsDigit) < (showSeconds ? 6 : 4)) return time;
        return $"{time} {(PicksPm(raw, am, pm) ?? currentPm ? pm : am)}";
    }

    private static bool? PicksPm(string raw, string am, string pm)
    {
        static bool Starts(string designator, char c) =>
            designator.Length > 0 && char.ToUpperInvariant(designator[0]) == char.ToUpperInvariant(c);
        static string Without(string s, string designator)
        {
            var at = designator.Length > 0 ? s.IndexOf(designator, StringComparison.OrdinalIgnoreCase) : -1;
            return at < 0 ? s : s.Remove(at, designator.Length);
        }
        // The caret sits before the designator the mask wrote, so a typed letter can land in front of it.
        var typed = Without(Without(raw, pm), am);
        for (var i = typed.Length - 1; i >= 0; i--)
        {
            var c = typed[i];
            if (!char.IsLetter(c)) continue;
            var isAm = Starts(am, c);
            var isPm = Starts(pm, c);
            if (isAm != isPm) return isPm;
        }
        var hasAm = am.Length > 0 && raw.Contains(am, StringComparison.OrdinalIgnoreCase);
        var hasPm = pm.Length > 0 && raw.Contains(pm, StringComparison.OrdinalIgnoreCase);
        return hasAm == hasPm ? null : hasPm;
    }
}
