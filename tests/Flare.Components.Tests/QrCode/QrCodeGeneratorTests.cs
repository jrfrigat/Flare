using System.Text;

namespace Flare.Components.Tests.QrCode;

// ---------------------------------------------------------------------------
// Regression tests for QrCodeGenerator.
//
// These assert that generated codes are actually SCANNABLE, not merely that
// they look like QR codes. An INDEPENDENT decoder reads the finished matrix
// back: it un-masks via the recorded format bits, reads the codeword stream in
// the standard zig-zag, de-interleaves with the ISO/IEC 18004 block structure,
// verifies Reed-Solomon syndromes are zero (a real scanner's decode succeeds
// iff they are), and parses the byte-mode payload. A code that round-trips with
// zero syndromes will scan on any conforming reader.
//
// Guards three historical bugs that shipped in untested code:
//   1. RS division off-by-one (gen[i] vs gen[i+1]) -> every code unscannable.
//   2. Wrong EC block structure for M-v3 and Q-v4.
//   3. Corrupt format-info constants for level H, masks 5/6/7.
// ---------------------------------------------------------------------------
public class QrCodeGeneratorTests
{
    public static IEnumerable<object[]> RoundTripCases()
    {
        // Payload lengths chosen to land on each version (1-4) at each level,
        // including the boundaries that the block-structure/format bugs broke.
        int[] lengths = { 1, 2, 6, 10, 13, 16, 18, 22, 24, 28, 30, 32, 40, 44, 55, 60 };
        foreach (var level in new[]
        {
            QrErrorCorrectionLevel.L, QrErrorCorrectionLevel.M,
            QrErrorCorrectionLevel.Q, QrErrorCorrectionLevel.H,
        })
        {
            foreach (var len in lengths)
                yield return new object[] { level, MakePayload(len) };
        }
    }

    [Theory]
    [MemberData(nameof(RoundTripCases))]
    public void GeneratedCode_IsScannable(QrErrorCorrectionLevel level, string payload)
    {
        var matrix = QrCodeGenerator.Generate(payload, level);
        if (matrix is null)
            return; // payload exceeds the supported version-4 capacity at this level

        var (decoded, syndromesZero) = QrTestDecoder.Decode(matrix, level);

        Assert.True(syndromesZero, $"Reed-Solomon syndromes non-zero (unscannable) for level {level}, len {payload.Length}");
        Assert.Equal(payload, decoded);
    }

    [Theory]
    [InlineData("https://example.com/path?q=42")]
    [InlineData("HELLO WORLD")]
    [InlineData("mixed CASE 123 -._~ /:")]
    public void CommonPayloads_RoundTrip(string payload)
    {
        foreach (var level in new[]
        {
            QrErrorCorrectionLevel.L, QrErrorCorrectionLevel.M,
            QrErrorCorrectionLevel.Q, QrErrorCorrectionLevel.H,
        })
        {
            var matrix = QrCodeGenerator.Generate(payload, level);
            Assert.NotNull(matrix);
            var (decoded, syndromesZero) = QrTestDecoder.Decode(matrix!, level);
            Assert.True(syndromesZero, $"unscannable for level {level}");
            Assert.Equal(payload, decoded);
        }
    }

    [Fact]
    public void EmptyString_ProducesScannableCode()
    {
        // Generator substitutes a single space for empty input.
        var matrix = QrCodeGenerator.Generate("", QrErrorCorrectionLevel.M);
        Assert.NotNull(matrix);
        var (decoded, syndromesZero) = QrTestDecoder.Decode(matrix!, QrErrorCorrectionLevel.M);
        Assert.True(syndromesZero);
        Assert.Equal(" ", decoded);
    }

    [Fact]
    public void OverlongPayload_ReturnsNull()
    {
        // Beyond version-4 byte capacity at level H.
        Assert.Null(QrCodeGenerator.Generate(MakePayload(200), QrErrorCorrectionLevel.H));
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
