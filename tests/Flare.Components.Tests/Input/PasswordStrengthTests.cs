using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The default rule scores on length first and variety second, which is the order that matters: a long
/// passphrase of one character class beats a short one of four, and a rule that says otherwise teaches
/// people to write <c>Passw0rd!</c>.
/// </summary>
public class PasswordStrengthTests
{
    private static int Score(string? password) => FlarePasswordRules.Evaluate(password).Score;

    [Fact]
    public void AnEmptyPasswordScoresNothingAndSaysWhy()
    {
        var result = FlarePasswordRules.Evaluate("");

        Assert.Equal(0, result.Score);
        Assert.NotEmpty(result.Suggestions);
    }

    [Fact]
    public void LengthCarriesMoreThanVariety()
    {
        // Four character classes in eight characters, against one class in twenty-four.
        var short4 = Score("Pa55w!rX");
        var longPhrase = Score("correcthorsebatterystaple");

        Assert.True(longPhrase > short4, $"passphrase {longPhrase} should beat {short4}");
    }

    [Fact]
    public void ASingleCharacterClassIsCappedHoweverLong()
    {
        Assert.True(Score("aaaaaaaaaaaaaaaaaaaaaaaa") <= 1);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Password1")]
    [InlineData("qwerty")]
    public void AnObviousPasswordScoresZeroWhateverItsShape(string value)
    {
        var result = FlarePasswordRules.Evaluate(value);

        Assert.Equal(0, result.Score);
        Assert.Contains("Common", result.Suggestions);
    }

    [Fact]
    public void ScoreNeverLeavesItsScale()
    {
        foreach (var value in new[] { "", "a", "Aa1!", new string('x', 500), "Tr0ub4dor&3xtra-long-passphrase" })
        {
            var score = Score(value);
            Assert.InRange(score, 0, FlarePasswordStrength.MaxScore);
        }
    }

    // The rule returns keys, not sentences, so the default rule stays free of display text and the field
    // is what turns them into localized strings.
    [Fact]
    public void SuggestionsAreKeysUntilACallerLocalizesThem()
    {
        var raw = FlarePasswordRules.Evaluate("abc");
        var localized = FlarePasswordRules.Evaluate("abc", k => $"[{k}]");

        Assert.Contains("Longer", raw.Suggestions);
        Assert.Contains("[Longer]", localized.Suggestions);
    }

    [Fact]
    public void TheMeterIsOffByDefault()
    {
        var context = new FlareTestContext();
        var cut = context.Render<FlarePasswordField>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Input.Strength}"));
    }
}

// ------------------------------------------------------------------------------
// FlareTimeSpanPicker - a duration, not a clock
// ------------------------------------------------------------------------------
