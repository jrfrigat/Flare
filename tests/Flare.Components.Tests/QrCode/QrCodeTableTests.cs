namespace Flare.Components.Tests.QrCode;

// ---------------------------------------------------------------------------
// Checks the encoder's two transcribed tables (error correction codewords per
// block, and block count) against sources that are independent of them.
//
// This matters because QrTestDecoder deliberately reads the block structure
// from the generator: a round-trip proves the pipeline agrees with itself, but
// it cannot catch a mistyped table. These tests can. Three angles:
//
//   1. Geometry. The codewords a symbol holds follow from its module count once
//      every function pattern is subtracted - no capacity table involved. If a
//      block count were wrong the blocks would no longer tile the symbol.
//   2. Published capacities. The byte-mode payload limits from ISO/IEC 18004
//      table 7, which depend on both tables at once.
//   3. Published layout constants: alignment centres and version information
//      words, tabulated in the standard but computed here from a formula.
// ---------------------------------------------------------------------------
public class QrCodeTableTests
{
    private static readonly QrErrorCorrectionLevel[] AllLevels =
        [QrErrorCorrectionLevel.L, QrErrorCorrectionLevel.M, QrErrorCorrectionLevel.Q, QrErrorCorrectionLevel.H];

    [Fact]
    public void EveryBlockLayout_ExactlyTilesTheSymbol()
    {
        for (int version = QrCodeGenerator.MinVersion; version <= QrCodeGenerator.MaxVersion; version++)
        {
            foreach (var level in AllLevels)
            {
                var (blocks, ecPerBlock, shortLen, shortBlocks) = QrCodeGenerator.BlockStructure(version, level);
                int total = QrCodeGenerator.TotalCodewords(version);

                Assert.InRange(shortBlocks, 0, blocks);

                // Data codewords laid out across the two groups must add up to
                // exactly what is left after error correction, and the whole
                // set must fill the symbol's codeword capacity.
                int laidOut = shortBlocks * shortLen + (blocks - shortBlocks) * (shortLen + 1);
                Assert.Equal(QrCodeGenerator.DataCodewords(version, level), laidOut);
                Assert.Equal(total, laidOut + ecPerBlock * blocks);

                // Reed-Solomon needs at least as many data codewords as it has
                // parity, or the level would not deliver its stated recovery.
                Assert.True(shortLen > 0, $"v{version} {level}: empty data block");
                Assert.True(ecPerBlock is >= 7 and <= 30, $"v{version} {level}: ec/block {ecPerBlock} out of range");
            }
        }
    }

    [Theory]
    // ISO/IEC 18004 table 1: total codewords per version.
    [InlineData(1, 26)]
    [InlineData(2, 44)]
    [InlineData(3, 70)]
    [InlineData(4, 100)]
    [InlineData(5, 134)]
    [InlineData(6, 172)]
    [InlineData(7, 196)]
    [InlineData(8, 242)]
    [InlineData(9, 292)]
    [InlineData(10, 346)]
    [InlineData(25, 1588)]
    [InlineData(40, 3706)]
    public void TotalCodewords_MatchTheStandard(int version, int expected) =>
        Assert.Equal(expected, QrCodeGenerator.TotalCodewords(version));

    [Theory]
    // ISO/IEC 18004 table 7: byte-mode character capacity, in L/M/Q/H order.
    [InlineData(1, 17, 14, 11, 7)]
    [InlineData(2, 32, 26, 20, 14)]
    [InlineData(3, 53, 42, 32, 24)]
    [InlineData(4, 78, 62, 46, 34)]
    [InlineData(5, 106, 84, 60, 44)]
    [InlineData(6, 134, 106, 74, 58)]
    [InlineData(7, 154, 122, 86, 64)]
    [InlineData(8, 192, 152, 108, 84)]
    [InlineData(9, 230, 180, 130, 98)]
    [InlineData(10, 271, 213, 151, 119)]
    [InlineData(40, 2953, 2331, 1663, 1273)]
    public void BytePayloadCapacity_MatchesTheStandard(int version, int l, int m, int q, int h)
    {
        Assert.Equal(l, QrCodeGenerator.MaxPayloadBytes(version, QrErrorCorrectionLevel.L));
        Assert.Equal(m, QrCodeGenerator.MaxPayloadBytes(version, QrErrorCorrectionLevel.M));
        Assert.Equal(q, QrCodeGenerator.MaxPayloadBytes(version, QrErrorCorrectionLevel.Q));
        Assert.Equal(h, QrCodeGenerator.MaxPayloadBytes(version, QrErrorCorrectionLevel.H));
    }

    [Fact]
    public void PayloadCapacity_RisesWithEveryVersion()
    {
        foreach (var level in AllLevels)
            for (int version = QrCodeGenerator.MinVersion + 1; version <= QrCodeGenerator.MaxVersion; version++)
                Assert.True(
                    QrCodeGenerator.MaxPayloadBytes(version, level) > QrCodeGenerator.MaxPayloadBytes(version - 1, level),
                    $"v{version} {level} does not hold more than v{version - 1}, so version selection could skip it");
    }

    [Theory]
    // ISO/IEC 18004 table E.1. Version 32 is the row the general spacing rule misses.
    [InlineData(1, new int[0])]
    [InlineData(2, new[] { 6, 18 })]
    [InlineData(3, new[] { 6, 22 })]
    [InlineData(4, new[] { 6, 26 })]
    [InlineData(7, new[] { 6, 22, 38 })]
    [InlineData(14, new[] { 6, 26, 46, 66 })]
    [InlineData(17, new[] { 6, 30, 54, 78 })]
    [InlineData(21, new[] { 6, 28, 50, 72, 94 })]
    [InlineData(28, new[] { 6, 26, 50, 74, 98, 122 })]
    [InlineData(32, new[] { 6, 34, 60, 86, 112, 138 })]
    [InlineData(36, new[] { 6, 24, 50, 76, 102, 128, 154 })]
    [InlineData(40, new[] { 6, 30, 58, 86, 114, 142, 170 })]
    public void AlignmentCentres_MatchTheStandard(int version, int[] expected) =>
        Assert.Equal(expected, QrCodeGenerator.AlignmentCenters(version));

    [Theory]
    // ISO/IEC 18004 table D.1: the 18-bit version information words.
    [InlineData(7, 0x07C94)]
    [InlineData(8, 0x085BC)]
    [InlineData(9, 0x09A99)]
    [InlineData(10, 0x0A4D3)]
    [InlineData(40, 0x28C69)]
    public void VersionInformationWords_MatchTheStandard(int version, int expected) =>
        Assert.Equal(expected, QrCodeGenerator.VersionInfoWord(version));
}
