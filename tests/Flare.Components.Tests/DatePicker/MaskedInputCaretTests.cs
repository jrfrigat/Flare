namespace Flare.Components.Tests;

/// <summary>
/// TASK-111: the masked date/time fields rebuild their text on every keystroke, so the caret has to be
/// found again by counting digits. These cover the caret arithmetic and the two browser repros from the
/// audit (editing the middle of a date, and the second digit of an hour).
/// </summary>
public class MaskedInputCaretTests
{
    [Theory]
    [InlineData("15.10.2026", 0, 0)]
    [InlineData("15.10.2026", 2, 2)]
    [InlineData("15.10.2026", 3, 2)]   // the '.' contributes no digit
    [InlineData("11.02.026", 9, 7)]
    [InlineData("1:33", 1, 1)]
    [InlineData("", 5, 0)]
    public void DigitsBefore_CountsOnlyDigits(string text, int index, int expected)
        => Assert.Equal(expected, MaskedInput.DigitsBefore(text, index));

    [Theory]
    [InlineData("11.02.026", 0, 0)]
    [InlineData("11.02.026", 1, 1)]
    [InlineData("11.02.026", 2, 2)]
    [InlineData("11.02.026", 3, 4)]    // caret lands after the 3rd digit, past the separator
    [InlineData("11.02.026", 7, 9)]    // all digits -> end of the text
    [InlineData("11.02.026", 9, 9)]    // more digits than present -> end
    public void CaretAfterDigit_FindsTheSpotAfterThatDigit(string masked, int digits, int expected)
        => Assert.Equal(expected, MaskedInput.CaretAfterDigit(masked, digits));

    [Fact]
    public void DateRepro_BackspaceWithoutMovingTheCaret()
    {
        // 15.10.2026, caret after "15" (position 2); Backspace removes '5' -> "1.10.2026", caret 1.
        const string raw = "1.10.2026";
        const int domCaret = 1;
        var masked = MaskedInput.MaskByPattern(raw, "dd.MM.yyyy");
        Assert.Equal("11.02.026", masked);

        var digits = MaskedInput.DigitsBefore(raw, domCaret);
        var caret = MaskedInput.CaretAfterDigit(masked, digits);

        Assert.Equal(1, digits);
        Assert.Equal(1, caret); // stays on the first segment instead of jumping to the end
    }

    [Fact]
    public void TimeRepro_DeletingSecondHourDigit()
    {
        // 12:33, caret after "12" (position 2); Backspace removes '2' -> "1:33", caret 1.
        const string raw = "1:33";
        const int domCaret = 1;
        var masked = MaskedInput.MaskTime(raw, showSeconds: false);
        Assert.Equal("13:3", masked);   // the digits are laid on HH:mm, they do not keep the typed shape

        var digits = MaskedInput.DigitsBefore(raw, domCaret);
        var caret = MaskedInput.CaretAfterDigit(masked, digits);

        Assert.Equal(1, digits);
        Assert.Equal(1, caret);
    }

    [Fact]
    public void CaretAfterDigit_ZeroDigits_ReturnsStart()
        => Assert.Equal(0, MaskedInput.CaretAfterDigit("--:--", 0));

    [Fact]
    public void CaretAfterDigit_WithNoDigits_ReturnsEnd()
        => Assert.Equal(5, MaskedInput.CaretAfterDigit("--:--", 1));
}
