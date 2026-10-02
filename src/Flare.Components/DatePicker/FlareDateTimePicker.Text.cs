using System.Globalization;

namespace Flare.Components;

// Reading typed text into a value: the digit mask, the exact and lenient patterns and the offset rules.
public partial class FlareDateTimePicker
{
    // Strip to digits and lay them on the numeric pattern. "010620261233" -> "01.06.2026 12:33" (ru).
    private string MaskDateTime(string? raw) => MaskedInput.MaskByPattern(raw, _numericPattern);

    // Text edits the wall date and time only: the offset and the ticks the pattern cannot show come from the
    // current value, so changing a minute moves the instant by that minute (TASK-135).
    private bool TryParse(string s, out DateTimeOffset value)
    {
        if (ParseInput is { } parse)
        {
            var parsed = parse(s);
            value = parsed ?? default;
            return parsed.HasValue;
        }
        if (TryParseExact(s, _numericPattern, out value)) return true;
        if (TryParseExact(s, _format, out value)) return true;
        // A half-edited mask ("15.10.2026 12:3") must never fall through to the lenient parser and be
        // committed as a different time (TASK-126, as TASK-119 for the date): only a complete one may use it.
        if (s.Count(char.IsDigit) == _numericPattern.Count(char.IsLetter)
            && DateTime.TryParse(s, _culture, DateTimeStyles.None, out var wall)
            && TryCompose(wall, HiddenTicks(_numericPattern), out value)) return true;
        value = default;
        return false;
    }

    private bool TryParseExact(string s, string pattern, out DateTimeOffset value)
    {
        value = default;
        if (pattern.Contains('z') || pattern.Contains('K'))
        {
            // A format that shows the offset lets the user type it.
            if (!DateTimeOffset.TryParseExact(s, pattern, _culture, DateTimeStyles.None, out var typed)) return false;
            try { value = typed.AddTicks(HiddenTicks(pattern)); return true; }
            catch (ArgumentOutOfRangeException) { return false; }
        }
        return DateTime.TryParseExact(s, pattern, _culture, DateTimeStyles.None, out var wall)
            && TryCompose(wall, HiddenTicks(pattern), out value);
    }

    // Ticks of the value below the smallest unit the pattern shows; the text cannot edit them.
    private long HiddenTicks(string pattern)
    {
        if (Value is not { } v || pattern.Contains('f') || pattern.Contains('F')) return 0;
        return v.Ticks % (pattern.Contains('s') ? TimeSpan.TicksPerSecond : TimeSpan.TicksPerMinute);
    }

    // The value's own offset, or for a new value the local zone's offset at that wall time.
    private TimeSpan OffsetFor(DateTime wall) => Value?.Offset ?? TimeZoneInfo.Local.GetUtcOffset(wall);

    // False when the wall time with its offset falls outside the DateTimeOffset range (0001-01-01 at +05:00).
    private bool TryCompose(DateTime wall, long hiddenTicks, out DateTimeOffset value)
    {
        try
        {
            var w = DateTime.SpecifyKind(wall, DateTimeKind.Unspecified).AddTicks(hiddenTicks);
            value = new DateTimeOffset(w, OffsetFor(w));
            return true;
        }
        catch (ArgumentException)
        {
            value = default;
            return false;
        }
    }
}
