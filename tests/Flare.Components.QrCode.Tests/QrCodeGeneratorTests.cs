using System.Text;

namespace Flare.Components.Tests;

// ---------------------------------------------------------------------------
// Regression tests for QrCodeGenerator.
//
// These assert that generated codes are actually SCANNABLE, not merely that
// they look like QR codes. An independent decoder reads the finished matrix
// back: it un-masks via the recorded format bits, reads the codeword stream in
// the standard zig-zag, de-interleaves the two block groups, verifies
// Reed-Solomon syndromes are zero (a real scanner's decode succeeds iff they
// are), and parses the byte-mode payload. A code that round-trips with zero
// syndromes will scan on any conforming reader.
//
// Guards three historical bugs that shipped in untested code:
//   1. RS division off-by-one (gen[i] vs gen[i+1]) -> every code unscannable.
//   2. Wrong EC block structure for M-v3 and Q-v4.
//   3. Corrupt format-info constants for level H, masks 5/6/7.
//
// The transcribed capacity tables are checked separately in QrCodeTableTests -
// a round-trip cannot catch a table the encoder and decoder both read.
// ---------------------------------------------------------------------------
public class QrCodeGeneratorTests
{
    private static readonly QrErrorCorrectionLevel[] AllLevels =
        [QrErrorCorrectionLevel.L, QrErrorCorrectionLevel.M, QrErrorCorrectionLevel.Q, QrErrorCorrectionLevel.H];

    public static IEnumerable<object[]> EveryVersionAndLevel()
    {
        foreach (var level in AllLevels)
            for (int version = QrCodeGenerator.MinVersion; version <= QrCodeGenerator.MaxVersion; version++)
                yield return [level, version];
    }

    /// <summary>
    /// A payload of exactly the version's capacity: the boundary case, where
    /// the bitstream ends flush with the last codeword and no padding is added.
    /// It also pins version selection, since the previous version cannot hold it.
    /// </summary>
    [Theory]
    [MemberData(nameof(EveryVersionAndLevel))]
    public void PayloadAtExactCapacity_SelectsThatVersion_AndScans(QrErrorCorrectionLevel level, int version)
    {
        string payload = MakePayload(QrCodeGenerator.MaxPayloadBytes(version, level));

        var matrix = QrCodeGenerator.Generate(payload, level);

        Assert.NotNull(matrix);
        Assert.Equal(QrCodeGenerator.SizeOf(version), matrix!.GetLength(0));
        AssertScans(matrix, level, payload);
    }

    /// <summary>
    /// One byte past the previous version's capacity: the shortest payload that
    /// needs this version, so the bitstream is mostly pad codewords. Together
    /// with the boundary case this exercises both ends of every symbol size.
    /// </summary>
    [Theory]
    [MemberData(nameof(EveryVersionAndLevel))]
    public void ShortestPayloadForVersion_Scans(QrErrorCorrectionLevel level, int version)
    {
        int floor = version == QrCodeGenerator.MinVersion
            ? 0
            : QrCodeGenerator.MaxPayloadBytes(version - 1, level);
        string payload = MakePayload(floor + 1);

        var matrix = QrCodeGenerator.Generate(payload, level);

        Assert.NotNull(matrix);
        Assert.Equal(QrCodeGenerator.SizeOf(version), matrix!.GetLength(0));
        AssertScans(matrix, level, payload);
    }

    [Theory]
    [InlineData("https://example.com/path?q=42")]
    [InlineData("HELLO WORLD")]
    [InlineData("mixed CASE 123 -._~ /:")]
    // Long enough to have needed version 5+, which the encoder could not reach before.
    [InlineData("https://flare.example.org/components/data-grid?tab=api&anchor=parameters&v=2")]
    [InlineData("BEGIN:VCARD\nVERSION:3.0\nN:Doe;Jane\nTEL:+1-555-0100\nEMAIL:jane.doe@example.com\nEND:VCARD")]
    public void CommonPayloads_RoundTrip(string payload)
    {
        foreach (var level in AllLevels)
        {
            var matrix = QrCodeGenerator.Generate(payload, level);
            Assert.NotNull(matrix);
            AssertScans(matrix!, level, payload);
        }
    }

    [Fact]
    public void EmptyString_ProducesScannableCode()
    {
        // Generator substitutes a single space for empty input.
        var matrix = QrCodeGenerator.Generate("", QrErrorCorrectionLevel.M);
        Assert.NotNull(matrix);
        AssertScans(matrix!, QrErrorCorrectionLevel.M, " ");
    }

    [Fact]
    public void PayloadBeyondVersion40_ReturnsNull()
    {
        foreach (var level in AllLevels)
        {
            int max = QrCodeGenerator.MaxPayloadBytes(QrCodeGenerator.MaxVersion, level);
            Assert.NotNull(QrCodeGenerator.Generate(MakePayload(max), level));
            Assert.Null(QrCodeGenerator.Generate(MakePayload(max + 1), level));
        }
    }

    [Fact]
    public void VersionInformationBlock_IsPresentFromVersion7()
    {
        // Versions 7 and up carry an 18-bit version word twice; a reader uses it
        // to size the symbol, so a missing or misplaced block fails to scan even
        // though every data module is correct.
        const QrErrorCorrectionLevel level = QrErrorCorrectionLevel.L;
        var matrix = QrCodeGenerator.Generate(MakePayload(QrCodeGenerator.MaxPayloadBytes(7, level)), level);
        Assert.NotNull(matrix);

        int size = matrix!.GetLength(0);
        int expected = QrCodeGenerator.VersionInfoWord(7);
        int topRight = 0, bottomLeft = 0;
        for (int i = 0; i < 18; i++)
        {
            int a = size - 11 + i % 3, b = i / 3;
            if (matrix[b, a]) topRight |= 1 << i;
            if (matrix[a, b]) bottomLeft |= 1 << i;
        }

        Assert.Equal(expected, topRight);
        Assert.Equal(expected, bottomLeft);
    }

    private static void AssertScans(bool[,] matrix, QrErrorCorrectionLevel level, string payload)
    {
        var (decoded, syndromesZero) = QrTestDecoder.Decode(matrix, level);
        Assert.True(syndromesZero,
            $"Reed-Solomon syndromes non-zero (unscannable) for level {level}, {payload.Length} bytes");
        Assert.Equal(payload, decoded);
    }

    private static string MakePayload(int len)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789-._~ ABC/:";
        var sb = new StringBuilder(len);
        for (int i = 0; i < len; i++) sb.Append(alphabet[i % alphabet.Length]);
        if (len > 0) sb[0] = 'x'; // guarantee byte mode (lowercase)
        return sb.ToString();
    }
}
