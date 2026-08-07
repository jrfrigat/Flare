using System.Text;

namespace Flare.Components;

/// <summary>
/// Pure-C# QR code encoder covering the whole ISO/IEC 18004 symbol range.
/// Encodes in byte mode at error correction levels L/M/Q/H, versions 1-40
/// (21x21 to 177x177 modules, up to 2953 bytes of payload).
/// </summary>
internal static class QrCodeGenerator
{
    /// <summary>Lowest symbol version defined by the standard.</summary>
    public const int MinVersion = 1;

    /// <summary>Highest symbol version defined by the standard.</summary>
    public const int MaxVersion = 40;

    // -- Reed-Solomon GF(256) arithmetic (primitive polynomial x^8+x^4+x^3+x^2+1 = 0x11D) --

    private static readonly byte[] _exp = new byte[512];
    private static readonly byte[] _log = new byte[256];

    static QrCodeGenerator()
    {
        int x = 1;
        for (int i = 0; i < 255; i++)
        {
            _exp[i] = (byte)x;
            _log[x] = (byte)i;
            x <<= 1;
            if ((x & 0x100) != 0) x ^= 0x11D;
        }
        for (int i = 255; i < 512; i++) _exp[i] = _exp[i - 255];
    }

    private static byte GfMul(byte a, byte b)
    {
        if (a == 0 || b == 0) return 0;
        return _exp[(_log[a] + _log[b]) % 255];
    }

    // -- Reed-Solomon error correction codewords --

    private static byte[] RsGenerator(int degree)
    {
        byte[] g = [1];
        for (int i = 0; i < degree; i++)
        {
            byte alpha = _exp[i];
            byte[] ng = new byte[g.Length + 1];
            for (int j = 0; j < g.Length; j++)
            {
                ng[j] ^= g[j];
                ng[j + 1] ^= GfMul(g[j], alpha);
            }
            g = ng;
        }
        return g;
    }

    private static byte[] RsEncode(ReadOnlySpan<byte> data, int ecCount, byte[] gen)
    {
        byte[] rem = new byte[ecCount];
        foreach (byte b in data)
        {
            byte factor = (byte)(b ^ rem[0]);
            Array.Copy(rem, 1, rem, 0, ecCount - 1);
            rem[ecCount - 1] = 0;
            // gen has ecCount+1 coefficients [1, g1, ..., g_ec]; the systematic
            // division feedback multiplies by g1..g_ec (skip the leading 1).
            for (int i = 0; i < ecCount; i++)
                rem[i] ^= GfMul(gen[i + 1], factor);
        }
        return rem;
    }

    // -- Block structure, ISO/IEC 18004 table 13-22, indexed by [levelIdx][version] --
    //
    // Only these two tables are transcribed; everything else about the symbol
    // (total codewords, data codewords, how the blocks are sized and split into
    // two groups) is derived from them plus the module geometry, so a symbol's
    // capacity cannot silently disagree with its layout. Level index: L=0, M=1,
    // Q=2, H=3. Index 0 is unused padding so the version number indexes directly.

    private static readonly byte[][] _ecPerBlock =
    [
        //   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30  31  32  33  34  35  36  37  38  39  40
        [0,  7, 10, 15, 20, 26, 18, 20, 24, 30, 18, 20, 24, 26, 30, 22, 24, 28, 30, 28, 28, 28, 28, 30, 30, 26, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30], // L
        [0, 10, 16, 26, 18, 24, 16, 18, 22, 22, 26, 30, 22, 22, 24, 24, 28, 28, 26, 26, 26, 26, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28], // M
        [0, 13, 22, 18, 26, 18, 24, 18, 22, 20, 24, 28, 26, 24, 20, 30, 24, 28, 28, 26, 30, 28, 30, 30, 30, 30, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30], // Q
        [0, 17, 28, 22, 16, 22, 28, 26, 26, 24, 28, 24, 28, 22, 24, 24, 30, 28, 28, 26, 28, 30, 24, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30], // H
    ];

    private static readonly byte[][] _blockCount =
    [
        //   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26  27  28  29  30  31  32  33  34  35  36  37  38  39  40
        [0,  1,  1,  1,  1,  1,  2,  2,  2,  2,  4,  4,  4,  4,  4,  6,  6,  6,  6,  7,  8,  8,  9,  9, 10, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 24, 25], // L
        [0,  1,  1,  1,  2,  2,  4,  4,  4,  5,  5,  5,  8,  9,  9, 10, 10, 11, 13, 14, 16, 17, 17, 18, 20, 21, 23, 25, 26, 28, 29, 31, 33, 35, 37, 38, 40, 43, 45, 47, 49], // M
        [0,  1,  1,  2,  2,  4,  4,  6,  6,  8,  8,  8, 10, 12, 16, 12, 17, 16, 18, 21, 20, 23, 23, 25, 27, 29, 34, 34, 35, 38, 40, 43, 45, 48, 51, 53, 56, 59, 62, 65, 68], // Q
        [0,  1,  1,  2,  4,  4,  4,  5,  6,  8,  8, 11, 11, 16, 16, 18, 16, 19, 21, 25, 25, 25, 34, 30, 32, 35, 37, 40, 42, 45, 48, 51, 54, 57, 60, 63, 66, 70, 74, 77, 81], // H
    ];

    // Pre-computed format information 15-bit words for each level (rows) and mask (cols).
    // Source: ISO/IEC 18004 Annex C. EC bits: L=01, M=00, Q=11, H=10.
    private static readonly int[][] _formatInfo =
    [
        // L
        [0x77C4, 0x72F3, 0x7DAA, 0x789D, 0x662F, 0x6318, 0x6C41, 0x6976],
        // M
        [0x5412, 0x5125, 0x5E7C, 0x5B4B, 0x45F9, 0x40CE, 0x4F97, 0x4AA0],
        // Q
        [0x355F, 0x3068, 0x3F31, 0x3A06, 0x24B4, 0x2183, 0x2EDA, 0x2BED],
        // H
        [0x1689, 0x13BE, 0x1CE7, 0x19D0, 0x0762, 0x0255, 0x0D0C, 0x083B],
    ];

    /// <summary>Module count along one edge of the given version's symbol.</summary>
    public static int SizeOf(int version) => version * 4 + 17;

    /// <summary>
    /// Counts the modules a version leaves free for data and error correction,
    /// by subtracting every function pattern from the symbol area. The whole
    /// capacity model is anchored on this geometry rather than a transcribed
    /// capacity table, so the two cannot drift apart.
    /// </summary>
    public static int RawDataModules(int version)
    {
        int size = SizeOf(version);
        int result = size * size;
        result -= 8 * 8 * 3;          // three finder patterns with their separators
        result -= 15 * 2 + 1;         // two format information strips + the dark module
        result -= (size - 16) * 2;    // the timing patterns outside the finder corners
        if (version >= 2)
        {
            int numAlign = version / 7 + 2;
            result -= (numAlign - 1) * (numAlign - 1) * 25;  // alignment patterns clear of the timing lines
            result -= (numAlign - 2) * 2 * 20;               // those straddling a timing line share 5 modules
            if (version >= 7)
                result -= 6 * 3 * 2;  // two version information blocks
        }
        return result;
    }

    /// <summary>Total codewords (data plus error correction) a version holds.</summary>
    public static int TotalCodewords(int version) => RawDataModules(version) / 8;

    /// <summary>Codewords a version and level leave for the data bitstream.</summary>
    public static int DataCodewords(int version, QrErrorCorrectionLevel level) =>
        TotalCodewords(version) - _ecPerBlock[(int)level][version] * _blockCount[(int)level][version];

    /// <summary>Bits the character count indicator occupies in byte mode at the given version.</summary>
    private static int CountBits(int version) => version < 10 ? 8 : 16;

    /// <summary>
    /// How a version and level divide their codewords into interleaved blocks.
    /// The data codewords rarely divide evenly, so the standard uses two groups:
    /// the first <c>shortBlocks</c> blocks carry <c>shortDataLen</c> data
    /// codewords and the rest carry one more.
    /// </summary>
    public static (int blocks, int ecPerBlock, int shortDataLen, int shortBlocks) BlockStructure(
        int version, QrErrorCorrectionLevel level)
    {
        int blocks = _blockCount[(int)level][version];
        int ecPerBlock = _ecPerBlock[(int)level][version];
        int total = TotalCodewords(version);
        return (blocks, ecPerBlock, DataCodewords(version, level) / blocks, blocks - total % blocks);
    }

    /// <summary>Longest byte-mode payload a version and level can carry.</summary>
    public static int MaxPayloadBytes(int version, QrErrorCorrectionLevel level) =>
        (DataCodewords(version, level) * 8 - 4 - CountBits(version)) / 8;

    /// <summary>
    /// Centre coordinates of the alignment patterns for a version. The first is
    /// always 6; the rest are spread evenly from the far edge inwards, with the
    /// spacing rounded up to an even number (version 32 is the one case where
    /// the general rule and the standard's table disagree).
    /// </summary>
    public static int[] AlignmentCenters(int version)
    {
        if (version == 1) return [];
        int numAlign = version / 7 + 2;
        int step = version == 32 ? 26 : (version * 4 + numAlign * 2 + 1) / (numAlign * 2 - 2) * 2;
        var result = new int[numAlign];
        result[0] = 6;
        for (int i = numAlign - 1, pos = SizeOf(version) - 7; i >= 1; i--, pos -= step)
            result[i] = pos;
        return result;
    }

    /// <summary>
    /// The 18-bit version information word: the version number followed by a
    /// (18,6) Golay remainder, computed rather than tabulated.
    /// </summary>
    public static int VersionInfoWord(int version)
    {
        int rem = version;
        for (int i = 0; i < 12; i++)
            rem = (rem << 1) ^ ((rem >> 11) * 0x1F25);
        return version << 12 | rem;
    }

    /// <summary>
    /// Generates a QR code matrix, picking the smallest version that fits.
    /// Returns null when the text exceeds the 2953-byte version-40 capacity at
    /// the requested level.
    /// </summary>
    public static bool[,]? Generate(string text, QrErrorCorrectionLevel ecLevel = QrErrorCorrectionLevel.M)
    {
        if (string.IsNullOrEmpty(text)) text = " ";

        byte[] bytes = Encoding.Latin1.GetBytes(text);
        int len = bytes.Length;
        int levelIdx = (int)ecLevel;

        int version = 0;
        for (int v = MinVersion; v <= MaxVersion; v++)
        {
            if (len <= MaxPayloadBytes(v, ecLevel)) { version = v; break; }
        }
        if (version == 0) return null;

        int size = SizeOf(version);
        int totalCodewords = TotalCodewords(version);
        int dataCodewords = DataCodewords(version, ecLevel);
        var (numBlocks, ecPerBlock, shortLen, numShortBlocks) = BlockStructure(version, ecLevel);

        // -- Build the data bitstream --
        var bits = new BitWriter(dataCodewords);

        bits.Write(0b0100, 4);              // byte mode
        bits.Write(len, CountBits(version));
        foreach (byte b in bytes) bits.Write(b, 8);

        int totalDataBits = dataCodewords * 8;
        bits.Write(0, Math.Min(4, totalDataBits - bits.Length));  // terminator
        bits.Write(0, (8 - bits.Length % 8) % 8);                 // pad to a codeword boundary

        byte[] padBytes = [0xEC, 0x11];
        for (int padIdx = 0; bits.Length < totalDataBits; padIdx++)
            bits.Write(padBytes[padIdx % 2], 8);

        byte[] allData = bits.ToByteArray();

        // -- Split into blocks and compute error correction --
        byte[] gen = RsGenerator(ecPerBlock);
        var dataBlocks = new byte[numBlocks][];
        var ecBlocks = new byte[numBlocks][];
        for (int b = 0, offset = 0; b < numBlocks; b++)
        {
            int blockLen = shortLen + (b < numShortBlocks ? 0 : 1);
            dataBlocks[b] = allData[offset..(offset + blockLen)];
            ecBlocks[b] = RsEncode(dataBlocks[b], ecPerBlock, gen);
            offset += blockLen;
        }

        // -- Interleave codewords --
        // Column-major across the blocks; the short blocks simply have nothing
        // to contribute on the final data pass.
        var finalCW = new byte[totalCodewords];
        int cw = 0;
        for (int i = 0; i <= shortLen; i++)
            for (int b = 0; b < numBlocks; b++)
                if (i < dataBlocks[b].Length) finalCW[cw++] = dataBlocks[b][i];
        for (int i = 0; i < ecPerBlock; i++)
            for (int b = 0; b < numBlocks; b++)
                finalCW[cw++] = ecBlocks[b][i];

        // -- Build the module matrix --
        var matrix = new byte[size, size];
        var isFunction = new bool[size, size];

        PlaceFinder(matrix, isFunction, 0, 0);
        PlaceFinder(matrix, isFunction, 0, size - 7);
        PlaceFinder(matrix, isFunction, size - 7, 0);

        for (int i = 8; i < size - 8; i++)
        {
            byte v = (byte)(i % 2 == 0 ? 1 : 0);
            matrix[6, i] = matrix[i, 6] = v;
            isFunction[6, i] = isFunction[i, 6] = true;
        }

        // Alignment patterns sit at every pairing of the centres except the
        // three corners already occupied by the finder patterns.
        int[] centers = AlignmentCenters(version);
        for (int i = 0; i < centers.Length; i++)
        {
            for (int j = 0; j < centers.Length; j++)
            {
                bool finderCorner = (i == 0 && j == 0)
                    || (i == 0 && j == centers.Length - 1)
                    || (i == centers.Length - 1 && j == 0);
                if (!finderCorner) PlaceAlignment(matrix, isFunction, centers[i], centers[j]);
            }
        }

        matrix[4 * version + 9, 8] = 1;
        isFunction[4 * version + 9, 8] = true;

        if (version >= 7) PlaceVersionInfo(matrix, isFunction, size, version);

        ReserveFormat(isFunction, size);

        // -- Place data bits --
        int cwIndex = 0;
        int bitIndex = 7;
        bool upward = true;
        int col = size - 1;
        while (col > 0)
        {
            if (col == 6) col--;

            for (int rowStep = 0; rowStep < size; rowStep++)
            {
                int row = upward ? (size - 1 - rowStep) : rowStep;
                for (int dc = 0; dc < 2; dc++)
                {
                    int c = col - dc;
                    if (isFunction[row, c]) continue;
                    if (cwIndex >= finalCW.Length) { matrix[row, c] = 0; continue; }
                    byte bit = (byte)((finalCW[cwIndex] >> bitIndex) & 1);
                    matrix[row, c] = bit;
                    bitIndex--;
                    if (bitIndex < 0) { bitIndex = 7; cwIndex++; }
                }
            }
            col -= 2;
            upward = !upward;
        }

        // -- Choose best mask --
        int bestPenalty = int.MaxValue;
        bool[,]? bestMatrix = null;

        for (int mask = 0; mask < 8; mask++)
        {
            bool[,] candidate = ApplyMask(matrix, isFunction, size, mask);
            ApplyFormat(candidate, size, mask, levelIdx);
            int penalty = ComputePenalty(candidate, size);
            if (penalty < bestPenalty)
            {
                bestPenalty = penalty;
                bestMatrix = candidate;
            }
        }

        return bestMatrix;
    }

    private static void PlaceFinder(byte[,] m, bool[,] fn, int row, int col)
    {
        for (int r = -1; r <= 7; r++)
        {
            for (int c = -1; c <= 7; c++)
            {
                int pr = row + r, pc = col + c;
                if (pr < 0 || pc < 0 || pr >= m.GetLength(0) || pc >= m.GetLength(1)) continue;
                fn[pr, pc] = true;
                if (r == -1 || r == 7 || c == -1 || c == 7) { m[pr, pc] = 0; continue; }
                bool dark = (r == 0 || r == 6 || c == 0 || c == 6) ||
                            (r >= 2 && r <= 4 && c >= 2 && c <= 4);
                m[pr, pc] = dark ? (byte)1 : (byte)0;
            }
        }
    }

    private static void PlaceAlignment(byte[,] m, bool[,] fn, int row, int col)
    {
        for (int r = -2; r <= 2; r++)
        {
            for (int c = -2; c <= 2; c++)
            {
                int pr = row + r, pc = col + c;
                // Where an alignment pattern straddles a timing line the modules
                // already carry the identical value, so leaving them is correct.
                if (fn[pr, pc]) continue;
                fn[pr, pc] = true;
                bool dark = r == -2 || r == 2 || c == -2 || c == 2 || (r == 0 && c == 0);
                m[pr, pc] = dark ? (byte)1 : (byte)0;
            }
        }
    }

    private static void PlaceVersionInfo(byte[,] m, bool[,] fn, int size, int version)
    {
        int word = VersionInfoWord(version);
        for (int i = 0; i < 18; i++)
        {
            byte bit = (byte)((word >> i) & 1);
            int a = size - 11 + i % 3;
            int b = i / 3;
            m[a, b] = bit;                 // block below the bottom-left finder
            fn[a, b] = true;
            m[b, a] = bit;                 // block left of the top-right finder
            fn[b, a] = true;
        }
    }

    private static void ReserveFormat(bool[,] fn, int size)
    {
        for (int i = 0; i <= 8; i++) if (!fn[8, i]) fn[8, i] = true;
        for (int i = 0; i <= 7; i++) if (!fn[i, 8]) fn[i, 8] = true;
        for (int i = size - 8; i < size; i++) fn[8, i] = true;
        for (int i = size - 7; i < size; i++) fn[i, 8] = true;
    }

    private static bool[,] ApplyMask(byte[,] m, bool[,] fn, int size, int mask)
    {
        var result = new bool[size, size];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
            {
                bool dark = m[r, c] == 1;
                if (!fn[r, c] && MaskCondition(mask, r, c)) dark = !dark;
                result[r, c] = dark;
            }
        return result;
    }

    private static bool MaskCondition(int mask, int r, int c) => mask switch
    {
        0 => (r + c) % 2 == 0,
        1 => r % 2 == 0,
        2 => c % 3 == 0,
        3 => (r + c) % 3 == 0,
        4 => (r / 2 + c / 3) % 2 == 0,
        5 => (r * c) % 2 + (r * c) % 3 == 0,
        6 => ((r * c) % 2 + (r * c) % 3) % 2 == 0,
        _ => ((r + c) % 2 + (r * c) % 3) % 2 == 0,
    };

    private static void ApplyFormat(bool[,] m, int size, int mask, int levelIdx)
    {
        int fmt = _formatInfo[levelIdx][mask];

        int[] s1HCols = [0, 1, 2, 3, 4, 5, 7, 8];
        for (int i = 0; i < 8; i++)
            m[8, s1HCols[i]] = ((fmt >> (14 - i)) & 1) == 1;

        int[] s1VRows = [7, 5, 4, 3, 2, 1, 0];
        for (int i = 0; i < 7; i++)
            m[s1VRows[i], 8] = ((fmt >> (6 - i)) & 1) == 1;

        for (int i = 0; i < 8; i++)
            m[8, size - 8 + i] = ((fmt >> i) & 1) == 1;

        for (int i = 0; i < 7; i++)
            m[size - 7 + i, 8] = ((fmt >> (14 - i)) & 1) == 1;
    }

    private static int ComputePenalty(bool[,] m, int size)
    {
        int penalty = 0;

        for (int r = 0; r < size; r++)
        {
            int run = 1;
            for (int c = 1; c < size; c++)
            {
                if (m[r, c] == m[r, c - 1]) { run++; if (run == 5) penalty += 3; else if (run > 5) penalty++; }
                else run = 1;
            }
        }
        for (int c = 0; c < size; c++)
        {
            int run = 1;
            for (int r = 1; r < size; r++)
            {
                if (m[r, c] == m[r - 1, c]) { run++; if (run == 5) penalty += 3; else if (run > 5) penalty++; }
                else run = 1;
            }
        }

        for (int r = 0; r < size - 1; r++)
            for (int c = 0; c < size - 1; c++)
                if (m[r, c] == m[r, c + 1] && m[r, c] == m[r + 1, c] && m[r, c] == m[r + 1, c + 1])
                    penalty += 3;

        bool[] pat1 = [true, false, true, true, true, false, true, false, false, false, false];
        bool[] pat2 = [false, false, false, false, true, false, true, true, true, false, true];
        for (int r = 0; r < size; r++)
            for (int c = 0; c <= size - 11; c++)
            {
                if (MatchRow(m, r, c, pat1)) penalty += 40;
                if (MatchRow(m, r, c, pat2)) penalty += 40;
            }
        for (int c = 0; c < size; c++)
            for (int r = 0; r <= size - 11; r++)
            {
                if (MatchCol(m, r, c, pat1)) penalty += 40;
                if (MatchCol(m, r, c, pat2)) penalty += 40;
            }

        int dark = 0;
        for (int r = 0; r < size; r++) for (int c = 0; c < size; c++) if (m[r, c]) dark++;
        int total = size * size;
        int pct = dark * 100 / total;
        int low = pct / 5 * 5, high = low + 5;
        penalty += Math.Min(Math.Abs(low - 50), Math.Abs(high - 50)) * 10;

        return penalty;
    }

    private static bool MatchRow(bool[,] m, int r, int c, bool[] pat)
    {
        for (int i = 0; i < pat.Length; i++) if (m[r, c + i] != pat[i]) return false;
        return true;
    }

    private static bool MatchCol(bool[,] m, int r, int c, bool[] pat)
    {
        for (int i = 0; i < pat.Length; i++) if (m[r + i, c] != pat[i]) return false;
        return true;
    }

    private sealed class BitWriter(int dataCodewords)
    {
        private readonly byte[] _bytes = new byte[dataCodewords];
        public int Length { get; private set; }

        public void Write(int value, int count)
        {
            for (int i = count - 1; i >= 0; i--, Length++)
                if (((value >> i) & 1) == 1) _bytes[Length / 8] |= (byte)(0x80 >> (Length % 8));
        }

        public byte[] ToByteArray() => _bytes;
    }
}
