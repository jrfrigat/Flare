using System.Text;

namespace Flare.Components;

/// <summary>
/// Pure-C# minimal QR code encoder.
/// Supports byte mode, error correction levels L/M/Q/H, versions 1-4.
/// </summary>
internal static class QrCodeGenerator
{
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

    private static byte[] RsEncode(byte[] data, int ecCount)
    {
        byte[] gen = RsGenerator(ecCount);
        byte[] rem = new byte[ecCount];
        foreach (byte b in data)
        {
            byte factor = (byte)(b ^ rem[0]);
            Array.Copy(rem, 1, rem, 0, ecCount - 1);
            rem[ecCount - 1] = 0;
            for (int i = 0; i < ecCount; i++)
                rem[i] ^= GfMul(gen[i], factor);
        }
        return rem;
    }

    // -- Capacity tables (byte mode) indexed by [levelIdx][versionIdx] --
    // (moduleSize, dataCodewordsPerBlock, ecCodewordsPerBlock, numBlocks)
    // Level index: L=0, M=1, Q=2, H=3
    private static readonly (int size, int dataPerBlock, int ecPerBlock, int numBlocks)[][] _versionInfoByLevel =
    [
        // L
        [(21, 19, 7, 1), (25, 34, 10, 1), (29, 55, 15, 1), (33, 80, 20, 1)],
        // M
        [(21, 16, 10, 1), (25, 28, 16, 1), (29, 22, 13, 2), (33, 32, 18, 2)],
        // Q
        [(21, 13, 13, 1), (25, 22, 22, 1), (29, 17, 18, 2), (33, 12, 13, 4)],
        // H
        [(21, 9, 17, 1),  (25, 16, 28, 1), (29, 13, 22, 2), (33, 9, 16, 4)],
    ];

    // Conservative max byte capacity per level and version
    private static readonly int[][] _maxBytesByLevel =
    [
        [16, 31, 51, 76], // L
        [13, 24, 40, 60], // M
        [10, 18, 30, 44], // Q
        [6,  13, 22, 32], // H
    ];

    // Alignment pattern centers (version >=2; version 1 has none)
    private static readonly int[][] _alignCenters =
    [
        [],       // version 1
        [6, 18],  // version 2
        [6, 22],  // version 3
        [6, 26],  // version 4
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
        [0x1689, 0x13BE, 0x1CE7, 0x19D0, 0x0762, 0x0455, 0x0B0C, 0x0E3B],
    ];

    /// <summary>Generates a QR code matrix. Returns null if text exceeds version 4 capacity.</summary>
    public static bool[,]? Generate(string text, QrErrorCorrectionLevel ecLevel = QrErrorCorrectionLevel.M)
    {
        if (string.IsNullOrEmpty(text)) text = " ";

        byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);
        int len = bytes.Length;

        int levelIdx = (int)ecLevel;
        int[] maxBytes = _maxBytesByLevel[levelIdx];
        var versionInfo = _versionInfoByLevel[levelIdx];

        int versionIdx = -1;
        for (int i = 0; i < maxBytes.Length; i++)
        {
            if (len <= maxBytes[i]) { versionIdx = i; break; }
        }
        if (versionIdx < 0) return null;

        int version = versionIdx + 1;
        var (size, dataPerBlock, ecPerBlock, numBlocks) = versionInfo[versionIdx];

        // -- Build data bitstream --
        var bits = new BitWriter();

        bits.Write(0b0100, 4);        // byte mode
        bits.Write(len, 8);           // character count (versions 1-9)
        foreach (byte b in bytes) bits.Write(b, 8);

        int totalDataBits = dataPerBlock * numBlocks * 8;
        int terminatorLen = Math.Min(4, totalDataBits - bits.Length);
        if (terminatorLen > 0) bits.Write(0, terminatorLen);
        while (bits.Length % 8 != 0) bits.Write(0, 1);

        byte[] padBytes = [0xEC, 0x11];
        int padIdx = 0;
        while (bits.Length < totalDataBits)
        {
            bits.Write(padBytes[padIdx % 2], 8);
            padIdx++;
        }

        byte[] allData = bits.ToByteArray();

        // -- Split into blocks, compute EC --
        var dataBlocks = new List<byte[]>();
        var ecBlocks = new List<byte[]>();
        int offset = 0;
        for (int b = 0; b < numBlocks; b++)
        {
            byte[] block = allData[offset..(offset + dataPerBlock)];
            dataBlocks.Add(block);
            ecBlocks.Add(RsEncode(block, ecPerBlock));
            offset += dataPerBlock;
        }

        // -- Interleave codewords --
        var finalCW = new List<byte>();
        for (int i = 0; i < dataPerBlock; i++)
            foreach (var blk in dataBlocks) finalCW.Add(blk[i]);
        for (int i = 0; i < ecPerBlock; i++)
            foreach (var blk in ecBlocks) finalCW.Add(blk[i]);

        // -- Build module matrix --
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

        int[] centers = _alignCenters[versionIdx];
        if (centers.Length > 0)
            PlaceAlignment(matrix, isFunction, centers[1], centers[1]);

        matrix[4 * version + 9, 8] = 1;
        isFunction[4 * version + 9, 8] = true;

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
                    if (cwIndex >= finalCW.Count) { matrix[row, c] = 0; continue; }
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
                if (fn[pr, pc]) continue;
                fn[pr, pc] = true;
                bool dark = r == -2 || r == 2 || c == -2 || c == 2 || (r == 0 && c == 0);
                m[pr, pc] = dark ? (byte)1 : (byte)0;
            }
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

    private sealed class BitWriter
    {
        private readonly List<bool> _bits = [];
        public int Length => _bits.Count;
        public void Write(int value, int count)
        {
            for (int i = count - 1; i >= 0; i--) _bits.Add(((value >> i) & 1) == 1);
        }
        public byte[] ToByteArray()
        {
            int n = (_bits.Count + 7) / 8;
            byte[] result = new byte[n];
            for (int i = 0; i < _bits.Count; i++)
                if (_bits[i]) result[i / 8] |= (byte)(0x80 >> (i % 8));
            return result;
        }
    }
}
