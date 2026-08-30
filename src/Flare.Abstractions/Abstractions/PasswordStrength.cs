namespace Flare.Components;

/// <summary>How strong a password was judged to be, and why.</summary>
/// <param name="Score">
/// Strength from 0 (unusable) to 4 (strong). A bounded scale rather than a percentage because a meter
/// with five stops is read at a glance and a percentage invites false precision about something nobody
/// can actually measure.
/// </param>
/// <param name="Suggestions">
/// What would make it stronger, in the order worth acting on. Empty when nothing more is needed.
/// </param>
public readonly record struct FlarePasswordStrength(int Score, IReadOnlyList<string> Suggestions)
{
    /// <summary>The highest score the scale carries.</summary>
    public const int MaxScore = 4;
}

/// <summary>
/// The default strength rule, used when a caller turns the meter on without supplying its own.
/// </summary>
/// <remarks>
/// Deliberately a delegate on the component rather than a fixed rule inside it. Password policy is an
/// application's decision - a bank, an intranet and a game have different answers and all three are
/// right - and baking one into a UI library is the same class of mistake as baking in a colour. This is
/// a reasonable default, not a standard.
/// </remarks>
public static class FlarePasswordRules
{
    /// <summary>
    /// Scores on length first and variety second, which is the order that actually matters: a long
    /// passphrase of one character class beats a short one of four, and a rule that says otherwise
    /// teaches people to write <c>Passw0rd!</c>.
    /// </summary>
    /// <remarks>
    /// Variety is measured as DISTINCT CHARACTERS, not as character classes. Classes are the usual
    /// measure and they are the wrong one twice over: they call <c>Passw0rd!</c> varied and
    /// <c>correcthorsebatterystaple</c> monotonous, when the second is far harder to guess. Distinct
    /// characters separate a passphrase from a held-down key, which is the distinction length alone
    /// cannot make.
    /// </remarks>
    /// <param name="password">The password to judge.</param>
    /// <param name="suggestionSource">
    /// Supplies the localized suggestion text by key (<c>Longer</c>, <c>Mix</c>, <c>Common</c>). Passing
    /// null returns the keys themselves, which is what a caller doing its own localization wants.
    /// </param>
    /// <returns>The score and what would improve it.</returns>
    public static FlarePasswordStrength Evaluate(string? password, Func<string, string>? suggestionSource = null)
    {
        var text = suggestionSource ?? (k => k);
        var suggestions = new List<string>();

        if (string.IsNullOrEmpty(password))
            return new FlarePasswordStrength(0, [text("Longer")]);

        var length = password.Length;
        var classes = 0;
        if (password.Any(char.IsLower)) classes++;
        if (password.Any(char.IsUpper)) classes++;
        if (password.Any(char.IsDigit)) classes++;
        if (password.Any(c => !char.IsLetterOrDigit(c))) classes++;

        // Length carries most of the score, because it carries most of the entropy.
        var score = length switch
        {
            < 8 => 0,
            < 12 => 1,
            < 16 => 2,
            < 20 => 3,
            _ => 4,
        };

        // Variety adjusts; it does not decide.
        var distinct = password.Distinct().Count();

        // Too few distinct characters is the one thing that overrides length: a held-down key is not a
        // passphrase however long it runs, and nothing else in this rule can tell them apart.
        if (distinct < 5) score = Math.Min(score, 1);

        // One class costs a step only when the password is also short on distinct characters. A real
        // passphrase is one class by definition, and penalising it is how a rule ends up recommending
        // Passw0rd! over correcthorsebatterystaple.
        else if (classes == 1 && distinct < 8) score = Math.Max(0, score - 1);
        else if (classes >= 3 && length >= 8) score = Math.Min(FlarePasswordStrength.MaxScore, score + 1);

        if (IsObvious(password)) { score = 0; suggestions.Add(text("Common")); }
        if (length < 12) suggestions.Add(text("Longer"));
        if (classes < 3) suggestions.Add(text("Mix"));

        return new FlarePasswordStrength(Math.Clamp(score, 0, FlarePasswordStrength.MaxScore), suggestions);
    }

    // A deliberately tiny list. A real breach corpus belongs in the application, not in a UI package -
    // shipping a megabyte of leaked passwords inside a component library would be an odd thing to do.
    private static readonly string[] Obvious =
    [
        "password", "123456", "qwerty", "111111", "letmein", "welcome", "admin", "iloveyou",
        "abc123", "monkey", "dragon", "football", "master", "sunshine", "princess", "qwerty123",
    ];

    private static bool IsObvious(string password)
    {
        var lower = password.ToLowerInvariant();
        return Obvious.Any(o => lower == o || (lower.Length <= o.Length + 3 && lower.StartsWith(o, StringComparison.Ordinal)));
    }
}
