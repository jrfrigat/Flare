using System.Text;

namespace Flare.Components;

/// <summary>
/// Turns a string into the bar pattern of a 1D symbology. Every symbology here is a pure function from
/// text to modules, so each one is testable against its published check vectors with no browser in the
/// loop - which is the whole reason the encoders are written rather than wrapped.
/// </summary>
/// <remarks>
/// The output is a run-length list, not a bitmap: entry <c>i</c> is the width in modules of the
/// <c>i</c>-th element, and elements alternate bar, space, bar, space starting with a bar. That is the
/// form every 1D symbology is actually specified in, and it renders as one SVG rect per bar rather than
/// one per module.
/// </remarks>
public static class BarcodeEncoder
{
    /// <summary>The bars of an encoded symbol, plus the text a reader would see printed under it.</summary>
    /// <param name="Modules">Element widths in modules, alternating bar, space, bar, space.</param>
    /// <param name="Text">The human-readable line, including any check digit the encoder computed.</param>
    public readonly record struct Symbol(IReadOnlyList<int> Modules, string Text);

    /// <summary>
    /// Encodes <paramref name="value"/> in <paramref name="symbology"/>.
    /// </summary>
    /// <param name="value">The text to encode.</param>
    /// <param name="symbology">Which symbology to encode it in.</param>
    /// <param name="error">Why the value could not be encoded, or null on success.</param>
    /// <returns>The symbol, or null when <paramref name="value"/> is not encodable.</returns>
    /// <remarks>
    /// Never throws for bad input. The value usually arrives from a field the user is still typing into,
    /// and a component that threw during render would take the page down between the third and fourth
    /// digit of an EAN.
    /// </remarks>
    public static Symbol? TryEncode(string? value, BarcodeSymbology symbology, out string? error)
    {
        error = null;
        if (string.IsNullOrEmpty(value)) { error = "The value is empty."; return null; }

        try
        {
            return symbology switch
            {
                BarcodeSymbology.Code128 => Code128(value, out error),
                BarcodeSymbology.Ean13 => Ean(value, 13, out error),
                BarcodeSymbology.Ean8 => Ean(value, 8, out error),
                BarcodeSymbology.UpcA => Ean(value, 12, out error),
                BarcodeSymbology.Code39 => Code39(value, out error),
                BarcodeSymbology.Itf14 => Itf14(value, out error),
                BarcodeSymbology.Codabar => Codabar(value, out error),
                _ => null,
            };
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return null;
        }
    }

    // ================================ Code 128 =================================

    // The 107 symbol patterns, each six element widths summing to 11 modules; entry 106 is the stop bar,
    // which is seven elements and 13 modules. Transcribed from ISO/IEC 15417 table 1.
    private static readonly string[] Code128Patterns =
    [
        "212222", "222122", "222221", "121223", "121322", "131222", "122213", "122312", "132212", "221213",
        "221312", "231212", "112232", "122132", "122231", "113222", "123122", "123221", "223211", "221132",
        "221231", "213212", "223112", "312131", "311222", "321122", "321221", "312212", "322112", "322211",
        "212123", "212321", "232121", "111323", "131123", "131321", "112313", "132113", "132311", "211313",
        "231113", "231311", "112133", "112331", "132131", "113123", "113321", "133121", "313121", "211331",
        "231131", "213113", "213311", "213131", "311123", "311321", "331121", "312113", "312311", "332111",
        "314111", "221411", "431111", "111224", "111422", "121124", "121421", "141122", "141221", "112214",
        "112412", "122114", "122411", "142112", "142211", "241211", "221114", "413111", "241112", "134111",
        "111242", "121142", "121241", "114212", "124112", "124211", "411212", "421112", "421211", "212141",
        "214121", "412121", "111143", "111341", "131141", "114113", "114311", "411113", "411311", "113141",
        "114131", "311141", "411131", "211412", "211214", "211232", "2331112",
    ];

    private const int Code128StartA = 103;
    private const int Code128StartB = 104;
    private const int Code128StartC = 105;
    private const int Code128Stop = 106;
    private const int Code128CodeA = 101;
    private const int Code128CodeB = 100;
    private const int Code128CodeC = 99;

    private static Symbol? Code128(string value, out string? error)
    {
        error = null;
        foreach (var c in value)
        {
            if (c > 127) { error = $"Code 128 encodes ASCII only; '{c}' is outside it."; return null; }
        }

        var codes = new List<int>();
        var subset = ChooseInitialSubset(value);
        codes.Add(subset switch { 'C' => Code128StartC, 'A' => Code128StartA, _ => Code128StartB });

        var i = 0;
        while (i < value.Length)
        {
            if (subset == 'C')
            {
                // Subset C packs two digits into one symbol, which is what makes it worth switching for.
                if (i + 1 < value.Length && char.IsAsciiDigit(value[i]) && char.IsAsciiDigit(value[i + 1]))
                {
                    codes.Add((value[i] - '0') * 10 + (value[i + 1] - '0'));
                    i += 2;
                    continue;
                }

                subset = char.IsControl(value[i]) ? 'A' : 'B';
                codes.Add(subset == 'A' ? Code128CodeA : Code128CodeB);
                continue;
            }

            // Worth moving into C for a long enough digit run: six mid-string, or four at either end,
            // because a switch symbol costs one and each pair saves one.
            var run = DigitRun(value, i);
            if (run >= 6 || (run >= 4 && (i == 0 || i + run == value.Length)))
            {
                if (run % 2 == 1) { codes.Add(Encode128Char(value[i], subset)); i++; }
                subset = 'C';
                codes.Add(Code128CodeC);
                continue;
            }

            var ch = value[i];
            if (subset == 'B' && char.IsControl(ch)) { subset = 'A'; codes.Add(Code128CodeA); continue; }
            if (subset == 'A' && ch >= 96) { subset = 'B'; codes.Add(Code128CodeB); continue; }

            codes.Add(Encode128Char(ch, subset));
            i++;
        }

        // Checksum: the start code plus each data symbol weighted by its position, modulo 103.
        var sum = codes[0];
        for (var k = 1; k < codes.Count; k++) sum += codes[k] * k;
        codes.Add(sum % 103);
        codes.Add(Code128Stop);

        return new Symbol(PatternsToModules(codes.Select(c => Code128Patterns[c])), value);
    }

    private static char ChooseInitialSubset(string value)
    {
        var run = DigitRun(value, 0);
        if (run >= 4 && (run == value.Length || run >= 6)) return 'C';
        return value.Any(char.IsControl) ? 'A' : 'B';
    }

    private static int DigitRun(string value, int from)
    {
        var n = 0;
        while (from + n < value.Length && char.IsAsciiDigit(value[from + n])) n++;
        return n;
    }

    // Subset A carries the control characters and drops the lower case; subset B is the other way round.
    private static int Encode128Char(char c, char subset) => subset switch
    {
        'A' => c < 32 ? c + 64 : c - 32,
        _ => c - 32,
    };

    // ============================ EAN / UPC family ==============================

    // Left-hand odd (L), left-hand even (G) and right-hand (R) digit patterns, four elements each.
    private static readonly string[] EanL = ["3211", "2221", "2122", "1411", "1132", "1231", "1114", "1312", "1213", "3112"];
    private static readonly string[] EanG = ["1123", "1222", "2212", "1141", "2311", "1321", "4111", "2131", "3121", "2113"];

    // Which of the first six digits use the G pattern, indexed by the leading digit. This is where
    // EAN-13's thirteenth digit lives: it is not drawn, it is the parity of the left half.
    private static readonly string[] Ean13Parity =
    [
        "LLLLLL", "LLGLGG", "LLGGLG", "LLGGGL", "LGLLGG",
        "LGGLLG", "LGGGLL", "LGLGLG", "LGLGGL", "LGGLGL",
    ];

    // EAN-8's left half is all-L; only the right half differs, so it needs no parity table.
    private static Symbol? Ean(string value, int length, out string? error)
    {
        error = null;
        var digits = value.Where(char.IsAsciiDigit).ToArray();
        if (digits.Length != value.Length)
        {
            error = "Only digits can be encoded in this symbology.";
            return null;
        }

        // A caller may supply the check digit or leave it off; either is common on a label.
        var body = new string(digits);
        if (body.Length == length) body = body[..^1];
        if (body.Length != length - 1)
        {
            error = $"Expected {length - 1} digits (or {length} with the check digit), got {digits.Length}.";
            return null;
        }

        var full = body + CheckDigitMod10(body);
        var elements = new List<int>();

        void Guard(bool centre = false) => elements.AddRange(centre ? [1, 1, 1, 1, 1] : [1, 1, 1]);

        if (length == 8)
        {
            Guard();
            for (var i = 0; i < 4; i++) AddPattern(elements, EanL[full[i] - '0']);
            Guard(centre: true);
            for (var i = 4; i < 8; i++) AddPattern(elements, EanL[full[i] - '0']);
            Guard();
        }
        else
        {
            // UPC-A is EAN-13 with a leading zero, so it goes through the same path and only the
            // human-readable line differs.
            var thirteen = length == 12 ? "0" + full : full;
            var parity = Ean13Parity[thirteen[0] - '0'];

            Guard();
            for (var i = 1; i <= 6; i++)
            {
                var d = thirteen[i] - '0';
                AddPattern(elements, parity[i - 1] == 'L' ? EanL[d] : EanG[d]);
            }
            Guard(centre: true);
            for (var i = 7; i < 13; i++) AddPattern(elements, EanL[thirteen[i] - '0']);
            Guard();
        }

        return new Symbol(elements, full);
    }

    /// <summary>
    /// The modulo-10 check digit EAN, UPC and ITF-14 all share: weight the digits 3 and 1 from the right,
    /// then take what is left to reach the next multiple of ten.
    /// </summary>
    /// <param name="body">The digits before the check digit.</param>
    /// <returns>The check digit as a character.</returns>
    public static char CheckDigitMod10(string body)
    {
        var sum = 0;
        for (var i = 0; i < body.Length; i++)
        {
            var d = body[^(i + 1)] - '0';
            sum += i % 2 == 0 ? d * 3 : d;
        }

        return (char)('0' + (10 - sum % 10) % 10);
    }

    // ================================ Code 39 ===================================

    private const string Code39Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

    // Nine elements per character, five bars and four spaces, exactly three of them wide.
    private static readonly string[] Code39Patterns =
    [
        "111221211", "211211112", "112211112", "212211111", "111221112", "211221111", "112221111", "111211212",
        "211211211", "112211211", "211112112", "112112112", "212112111", "111122112", "211122111", "112122111",
        "111112212", "211112211", "112112211", "111122211", "211111122", "112111122", "212111121", "111121122",
        "211121121", "112121121", "111111222", "211111221", "112111221", "111121221", "221111112", "122111112",
        "222111111", "121121112", "221121111", "122121111", "121111212", "221111211", "122111211", "121121211",
        "121212111", "121211121", "121112121", "111212121", "121121112",
    ];

    private static Symbol? Code39(string value, out string? error)
    {
        error = null;
        var text = value.ToUpperInvariant();
        foreach (var c in text)
        {
            if (Code39Alphabet.IndexOf(c) < 0 || c == '*')
            {
                error = $"Code 39 cannot encode '{c}'.";
                return null;
            }
        }

        var elements = new List<int>();
        var first = true;
        // The delimiter is the same '*' symbol at both ends, and it is not part of the data.
        foreach (var c in "*" + text + "*")
        {
            if (!first) elements.Add(1);       // one narrow space between characters
            AddPattern(elements, Code39Patterns[Code39Alphabet.IndexOf(c)]);
            first = false;
        }

        return new Symbol(elements, text);
    }

    // ================================= ITF-14 ===================================

    // Interleaved two-of-five: five elements per digit, two of them wide. Odd-position digits become the
    // bars and even-position digits the spaces of the same group, which is where "interleaved" comes from.
    private static readonly string[] ItfPatterns =
    [
        "11221", "21112", "12112", "22111", "11212", "21211", "12211", "11122", "21121", "12121",
    ];

    private static Symbol? Itf14(string value, out string? error)
    {
        error = null;
        var digits = value.Where(char.IsAsciiDigit).ToArray();
        if (digits.Length != value.Length) { error = "ITF-14 encodes digits only."; return null; }

        var body = new string(digits);
        if (body.Length == 14) body = body[..^1];
        if (body.Length != 13)
        {
            error = $"Expected 13 digits (or 14 with the check digit), got {digits.Length}.";
            return null;
        }

        var full = body + CheckDigitMod10(body);
        var elements = new List<int> { 1, 1, 1, 1 };   // start: narrow bar, space, bar, space

        for (var i = 0; i < full.Length; i += 2)
        {
            var bars = ItfPatterns[full[i] - '0'];
            var spaces = ItfPatterns[full[i + 1] - '0'];
            for (var k = 0; k < 5; k++)
            {
                elements.Add(bars[k] - '0');
                elements.Add(spaces[k] - '0');
            }
        }

        elements.AddRange([3, 1, 1]);                  // stop: wide bar, narrow space, narrow bar
        return new Symbol(elements, full);
    }

    // ================================= Codabar ==================================

    private const string CodabarAlphabet = "0123456789-$:/.+ABCD";

    private static readonly string[] CodabarPatterns =
    [
        "1111221", "1111212", "1112112", "2211111", "1121121", "2111121", "1211112", "1211211",
        "1221111", "2112111", "1112211", "1122111", "2111212", "2121112", "2121211", "1121212",
        "1122121", "1212112", "1112122", "1112221",
    ];

    private static Symbol? Codabar(string value, out string? error)
    {
        error = null;
        var text = value.ToUpperInvariant();
        if (text.Length < 3 || !char.IsAsciiLetter(text[0]) || !char.IsAsciiLetter(text[^1]))
        {
            error = "Codabar needs a start and stop letter A-D around the data.";
            return null;
        }

        foreach (var c in text)
        {
            if (CodabarAlphabet.IndexOf(c) < 0) { error = $"Codabar cannot encode '{c}'."; return null; }
        }

        var elements = new List<int>();
        var first = true;
        foreach (var c in text)
        {
            if (!first) elements.Add(1);
            AddPattern(elements, CodabarPatterns[CodabarAlphabet.IndexOf(c)]);
            first = false;
        }

        return new Symbol(elements, text);
    }

    // ================================= Shared ===================================

    private static void AddPattern(List<int> into, string pattern)
    {
        foreach (var c in pattern) into.Add(c - '0');
    }

    private static List<int> PatternsToModules(IEnumerable<string> patterns)
    {
        var elements = new List<int>();
        foreach (var p in patterns) AddPattern(elements, p);
        return elements;
    }
}
