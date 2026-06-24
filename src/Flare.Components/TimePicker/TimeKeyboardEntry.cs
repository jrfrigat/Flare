namespace Flare.Components;

/// <summary>
/// Shared digit-by-digit time entry used by the clock dial and the dropdown columns.
/// Builds a two-digit value as the user types and reports when the field is complete
/// (so the caller can advance hour -> minute).
/// </summary>
internal static class TimeKeyboardEntry
{
    /// <summary>
    /// Feeds one digit into a field. Returns the new value, the carried buffer
    /// ("" when the field is complete) and whether the field is complete.
    /// </summary>
    public static (int Value, string Buffer, bool Complete) Feed(string buffer, char digit, int max)
    {
        var d = digit - '0';
        if (buffer.Length == 0)
        {
            var complete = d * 10 > max;          // a second digit cannot fit -> done
            return (d, complete ? string.Empty : d.ToString(), complete);
        }

        var combined = int.Parse(buffer) * 10 + d;
        if (combined <= max)
            return (combined, string.Empty, true); // two digits -> complete

        // Overflow: restart with this digit as the new first digit.
        var restartComplete = d * 10 > max;
        return (d, restartComplete ? string.Empty : d.ToString(), restartComplete);
    }
}
