using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// The mobile keyboard. Every mask preset is digits, and a text keyboard for a credit-card number is a
// phone-sized mistake that never shows up on a desktop.
public class MaskedFieldKeyboardTests : FlareTestContext
{
    private static string? Mode(IRenderedComponent<FlareMaskedField> cut) =>
        cut.Find("input").GetAttribute("inputmode");

    [Theory]
    [InlineData(MaskPreset.CreditCard, "numeric")]
    [InlineData(MaskPreset.Ssn, "numeric")]
    [InlineData(MaskPreset.Date, "numeric")]
    [InlineData(MaskPreset.Time, "numeric")]
    [InlineData(MaskPreset.IpAddress, "numeric")]
    public void ADigitOnlyPresetAsksForTheNumericKeyboard(MaskPreset preset, string expected)
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Preset, preset));

        Assert.Equal(expected, Mode(cut));
    }

    // A phone number is tel, not numeric: the tel keypad carries +, * and # as well as the digits.
    [Fact]
    public void APhoneAsksForTheTelKeypad()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Preset, MaskPreset.Phone));

        Assert.Equal("tel", Mode(cut));
    }

    [Fact]
    public void ACustomDigitMaskAlsoGetsTheNumericKeyboard()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Mask, "###-###"));

        Assert.Equal("numeric", Mode(cut));
    }

    // A mask that accepts letters needs the full keyboard, so it must NOT be narrowed.
    [Fact]
    public void AMaskWithLettersKeepsTheFullKeyboard()
    {
        var cut = Render<FlareMaskedField>(p => p.Add(x => x.Mask, "AA-####"));

        Assert.Null(Mode(cut));
    }

    [Fact]
    public void AnExplicitInputModeWins()
    {
        var cut = Render<FlareMaskedField>(p => p
            .Add(x => x.Preset, MaskPreset.CreditCard)
            .Add(x => x.InputMode, "decimal"));

        Assert.Equal("decimal", Mode(cut));
    }
}
