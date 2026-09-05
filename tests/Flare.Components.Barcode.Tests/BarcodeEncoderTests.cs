namespace Flare.Components.Tests;

/// <summary>
/// The seven symbologies, checked against their published vectors rather than against themselves.
/// </summary>
/// <remarks>
/// A barcode encoder is the kind of code that looks right and scans wrong, and the only test that can
/// tell the difference is one whose expected values came from the specification. So the assertions here
/// are check digits from the standards, module counts the geometry fixes, and the guard patterns each
/// symbology mandates - not a round-trip through the encoder's own tables.
/// </remarks>
public class BarcodeEncoderTests
{
    private static BarcodeEncoder.Symbol Encode(string value, BarcodeSymbology symbology)
    {
        var sym = BarcodeEncoder.TryEncode(value, symbology, out var error);
        Assert.True(sym is not null, $"expected {symbology} to encode '{value}', got: {error}");
        return sym!.Value;
    }

    private static string? Reject(string value, BarcodeSymbology symbology)
    {
        var sym = BarcodeEncoder.TryEncode(value, symbology, out var error);
        Assert.Null(sym);
        return error;
    }

    // ================================ Check digits =============================

    // The modulo-10 digit EAN, UPC and ITF-14 share. These four are the worked examples from the GS1
    // general specification, so a wrong weighting shows up here rather than at a till.
    [Theory]
    [InlineData("400638133393", '1')]   // EAN-13 body
    [InlineData("978030640615", '7')]   // ISBN-13 as EAN-13
    [InlineData("03600029145", '2')]    // UPC-A body
    [InlineData("9638507", '4')]        // EAN-8 body
    public void CheckDigitMatchesTheGs1Example(string body, char expected)
    {
        Assert.Equal(expected, BarcodeEncoder.CheckDigitMod10(body));
    }

    [Fact]
    public void CheckDigitIsAppendedToTheReadableLine()
    {
        Assert.Equal("4006381333931", Encode("400638133393", BarcodeSymbology.Ean13).Text);
    }

    // A label often carries the full number including the check digit. Both spellings must produce the
    // same symbol, or the same product scans as two different products.
    [Fact]
    public void SupplyingTheCheckDigitEncodesTheSameSymbol()
    {
        var without = Encode("400638133393", BarcodeSymbology.Ean13);
        var with = Encode("4006381333931", BarcodeSymbology.Ean13);

        Assert.Equal(without.Text, with.Text);
        Assert.Equal(without.Modules, with.Modules);
    }

    // ================================= Geometry ================================

    // Module counts are fixed by each specification, so they catch a pattern table that is one entry
    // wide in the wrong place - the failure that still renders a plausible-looking barcode.
    [Theory]
    [InlineData(BarcodeSymbology.Ean13, "4006381333931", 95)]
    [InlineData(BarcodeSymbology.Ean8, "96385074", 67)]
    [InlineData(BarcodeSymbology.UpcA, "036000291452", 95)]
    public void SymbolIsTheWidthTheSpecificationFixes(BarcodeSymbology symbology, string value, int modules)
    {
        Assert.Equal(modules, Encode(value, symbology).Modules.Sum());
    }

    // Every EAN symbol opens and closes with a 1-1-1 guard and carries a 1-1-1-1-1 guard in the middle;
    // that is what tells a scanner which way up it is reading.
    [Fact]
    public void EanCarriesItsGuardPatterns()
    {
        var m = Encode("4006381333931", BarcodeSymbology.Ean13).Modules;

        Assert.Equal([1, 1, 1], m.Take(3));
        Assert.Equal([1, 1, 1], m.TakeLast(3));
        // 3 lead-in + six 4-element digits = 27 elements before the centre guard.
        Assert.Equal([1, 1, 1, 1, 1], m.Skip(27).Take(5));
    }

    // UPC-A is EAN-13 with a leading zero, so the two must agree module for module. If they ever stop
    // agreeing, one of them is wrong and the other is silently the same bug.
    [Fact]
    public void UpcAIsEan13WithALeadingZero()
    {
        Assert.Equal(
            Encode("0036000291452", BarcodeSymbology.Ean13).Modules,
            Encode("036000291452", BarcodeSymbology.UpcA).Modules);
    }

    // ================================= Code 128 ================================

    // Every Code 128 symbol is six elements of 11 modules, except the stop bar, which is seven elements
    // of 13. So a valid symbol's total is 11n + 2, and a mistyped pattern breaks the arithmetic.
    [Theory]
    [InlineData("A")]
    [InlineData("Hello")]
    [InlineData("12345678")]
    [InlineData("ABC-123/456")]
    public void Code128SymbolWidthIsElevenPerCodeWordPlusTheStopBar(string value)
    {
        var total = Encode(value, BarcodeSymbology.Code128).Modules.Sum();

        Assert.Equal(2, (total - 2) % 11 == 0 ? 2 : -1);
    }

    // The whole reason subset C exists: two digits per symbol instead of one. A long digit run must come
    // out narrower than the same length of letters, or the switching logic is not working.
    [Fact]
    public void ALongDigitRunUsesSubsetCAndComesOutNarrower()
    {
        var digits = Encode("1234567890123456", BarcodeSymbology.Code128).Modules.Sum();
        var letters = Encode("ABCDEFGHIJKLMNOP", BarcodeSymbology.Code128).Modules.Sum();

        Assert.True(digits < letters, $"digits {digits} should be narrower than letters {letters}");
    }

    // A run too short to pay for the switch symbol must stay where it is; switching for two digits costs
    // more than it saves.
    [Fact]
    public void AShortDigitRunDoesNotPayForASubsetSwitch()
    {
        var mixed = Encode("AB12CD", BarcodeSymbology.Code128).Modules.Sum();

        // Start + 6 data + checksum = 8 code words, plus the stop bar.
        Assert.Equal(8 * 11 + 13, mixed);
    }

    [Fact]
    public void Code128RejectsNonAscii()
    {
        Assert.Contains("ASCII", Reject("Привет", BarcodeSymbology.Code128));
    }

    // ================================== Code 39 ================================

    // Code 39 delimits with '*' at both ends and separates characters with a narrow space: 9 elements per
    // character, plus one space between each pair.
    [Fact]
    public void Code39WrapsTheDataInItsDelimiter()
    {
        var m = Encode("ABC", BarcodeSymbology.Code39).Modules;

        Assert.Equal(5 * 9 + 4, m.Count);
    }

    [Fact]
    public void Code39UpperCasesRatherThanRejecting()
    {
        Assert.Equal("ABC", Encode("abc", BarcodeSymbology.Code39).Text);
    }

    [Fact]
    public void Code39RejectsACharacterOutsideItsAlphabet()
    {
        Assert.Contains("cannot encode", Reject("A&B", BarcodeSymbology.Code39));
    }

    // ================================== ITF-14 =================================

    // Interleaved two-of-five puts two digits in one group of ten elements, so 14 digits are 7 groups.
    [Fact]
    public void Itf14InterleavesTwoDigitsPerGroup()
    {
        var m = Encode("1540141253422", BarcodeSymbology.Itf14).Modules;

        // 4 start + 7 groups of 10 + 3 stop.
        Assert.Equal(4 + 70 + 3, m.Count);
    }

    // ITF-14 shares the mod-10 digit checked against GS1's examples above, so this asserts that it is
    // APPENDED, not that the arithmetic is right - the two are different claims and only one of them can
    // honestly be made here.
    [Fact]
    public void Itf14AppendsTheSharedCheckDigit()
    {
        const string body = "1540141253422";
        var expected = body + BarcodeEncoder.CheckDigitMod10(body);

        Assert.Equal(expected, Encode(body, BarcodeSymbology.Itf14).Text);
        Assert.Equal(14, expected.Length);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012345")]
    public void Itf14RejectsTheWrongLength(string value)
    {
        Assert.Contains("Expected 13 digits", Reject(value, BarcodeSymbology.Itf14));
    }

    // ================================== Codabar ================================

    [Fact]
    public void CodabarNeedsItsStartAndStopLetters()
    {
        Assert.Contains("start and stop", Reject("12345", BarcodeSymbology.Codabar));
        Assert.NotNull(BarcodeEncoder.TryEncode("A12345B", BarcodeSymbology.Codabar, out _));
    }

    [Fact]
    public void CodabarEncodesItsSymbolSet()
    {
        var sym = Encode("A123-456$789B", BarcodeSymbology.Codabar);
        Assert.Equal("A123-456$789B", sym.Text);
    }

    // ================================== Contract ===============================

    // The value usually arrives from a field the user is still typing into, so nothing here may throw -
    // an EAN is unencodable for the first twelve keystrokes of thirteen.
    [Theory]
    [InlineData(BarcodeSymbology.Code128)]
    [InlineData(BarcodeSymbology.Ean13)]
    [InlineData(BarcodeSymbology.Ean8)]
    [InlineData(BarcodeSymbology.UpcA)]
    [InlineData(BarcodeSymbology.Code39)]
    [InlineData(BarcodeSymbology.Itf14)]
    [InlineData(BarcodeSymbology.Codabar)]
    public void NoInputThrows(BarcodeSymbology symbology)
    {
        foreach (var value in new[] { null, "", " ", "4", "abc", "!!!", new string('9', 200) })
        {
            var ex = Record.Exception(() => BarcodeEncoder.TryEncode(value, symbology, out _));
            Assert.Null(ex);
        }
    }

    [Fact]
    public void AFailureAlwaysCarriesAReason()
    {
        Assert.False(string.IsNullOrWhiteSpace(Reject("", BarcodeSymbology.Code128)));
        Assert.False(string.IsNullOrWhiteSpace(Reject("12", BarcodeSymbology.Ean13)));
    }

    // Elements alternate bar, space, bar, space starting and ending with a bar - so every symbol has an
    // odd element count. An even one means a space is being drawn where a bar belongs.
    [Theory]
    [InlineData(BarcodeSymbology.Code128, "ABC123")]
    [InlineData(BarcodeSymbology.Ean13, "4006381333931")]
    [InlineData(BarcodeSymbology.Ean8, "96385074")]
    [InlineData(BarcodeSymbology.UpcA, "036000291452")]
    [InlineData(BarcodeSymbology.Code39, "ABC")]
    [InlineData(BarcodeSymbology.Itf14, "1540141253422")]
    [InlineData(BarcodeSymbology.Codabar, "A123B")]
    public void EverySymbolStartsAndEndsWithABar(BarcodeSymbology symbology, string value)
    {
        var m = Encode(value, symbology).Modules;

        Assert.True(m.Count % 2 == 1, $"{symbology} produced {m.Count} elements; a symbol must end on a bar");
        Assert.All(m, w => Assert.InRange(w, 1, 4));
    }
}
