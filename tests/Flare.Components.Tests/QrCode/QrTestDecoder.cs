using System.Text;

namespace Flare.Components.Tests.QrCode;

/// <summary>
/// Minimal, independent QR decoder used only by the test suite to prove that
/// <see cref="QrCodeGenerator"/> output is genuinely scannable. It shares no
/// code with the generator: it reconstructs the codeword stream from the
/// finished matrix and validates it with the standard block structure and
/// Reed-Solomon syndromes, so a passing test reflects a real reader's success.
/// Supports byte mode, versions 1-4 (the range the generator supports).
/// </summary>
internal static class QrTestDecoder
{
    // GF(256), primitive polynomial 0x11D (same field QR uses).
    private static readonly byte[] Exp = new byte[512];
    private static readonly byte[] Log = new byte[256];

    static QrTestDecoder()
    {
        int x = 1;
        for (int i = 0; i < 255; i++) { Exp[i] = (byte)x; Log[x] = (byte)i; x <<= 1; if ((x & 0x100) != 0) x ^= 0x11D; }
        for (int i = 255; i < 512; i++) Exp[i] = Exp[i - 255];
    }

    private static byte Mul(byte a, byte b) => (a == 0 || b == 0) ? (byte)0 : Exp[(Log[a] + Log[b]) % 255];

    // Standard block structure for versions 1-4: [levelIdx][versionIdx] = (dataPerBlock, ecPerBlock, numBlocks).
    // All version 1-4 combinations use a single group of equal blocks.
    private static readonly (int dataPerBlock, int ecPerBlock, int numBlocks)[][] Std =
    {
        new[] { (19, 7, 1), (34, 10, 1), (55, 15, 1), (80, 20, 1) }, // L
        new[] { (16, 10, 1), (28, 16, 1), (44, 26, 1), (32, 18, 2) }, // M
        new[] { (13, 13, 1), (22, 22, 1), (17, 18, 2), (24, 26, 2) }, // Q
        new[] { (9, 17, 1), (16, 28, 1), (13, 22, 2), (9, 16, 4) },   // H
    };

    /// <summary>Decodes a finished module matrix (dark = true, no quiet zone).</summary>
    public static (string decoded, bool syndromesZero) Decode(bool[,] m, QrErrorCorrectionLevel level)
    {
        int n = m.GetLength(0);
        int version = (n - 21) / 4 + 1;
        int levelIdx = (int)level;
        var (dataPerBlock, ecPerBlock, numBlocks) = Std[levelIdx][version - 1];

        int mask = ReadMask(m);
        var fn = FunctionMap(n, version);

        // Un-mask (mask applies to non-function modules only).
        var v = new bool[n, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
            {
                bool d = m[r, c];
                if (!fn[r, c] && MaskCondition(mask, r, c)) d = !d;
                v[r, c] = d;
            }

        // Read the codeword stream in the standard zig-zag order.
        var stream = new List<byte>();
        int cur = 0, bits = 0;
        bool up = true;
        for (int col = n - 1; col > 0; col -= 2)
        {
            if (col == 6) col--;
            for (int rowStep = 0; rowStep < n; rowStep++)
            {
                int row = up ? (n - 1 - rowStep) : rowStep;
                for (int dc = 0; dc < 2; dc++)
                {
                    int c = col - dc;
                    if (fn[row, c]) continue;
                    cur = (cur << 1) | (v[row, c] ? 1 : 0);
                    if (++bits == 8) { stream.Add((byte)cur); cur = 0; bits = 0; }
                }
            }
            up = !up;
        }

        // De-interleave into equal blocks.
        var dataBlocks = new byte[numBlocks][];
        var ecBlocks = new byte[numBlocks][];
        for (int b = 0; b < numBlocks; b++) { dataBlocks[b] = new byte[dataPerBlock]; ecBlocks[b] = new byte[ecPerBlock]; }
        int idx = 0;
        for (int i = 0; i < dataPerBlock; i++) for (int b = 0; b < numBlocks; b++) dataBlocks[b][i] = stream[idx++];
        for (int i = 0; i < ecPerBlock; i++) for (int b = 0; b < numBlocks; b++) ecBlocks[b][i] = stream[idx++];

        // Reed-Solomon syndrome check: valid iff C(alpha^j) == 0 for j in [0, ecPerBlock).
        bool syndromesZero = true;
        for (int b = 0; b < numBlocks && syndromesZero; b++)
        {
            var full = new byte[dataPerBlock + ecPerBlock];
            Array.Copy(dataBlocks[b], 0, full, 0, dataPerBlock);
            Array.Copy(ecBlocks[b], 0, full, dataPerBlock, ecPerBlock);
            for (int j = 0; j < ecPerBlock; j++)
            {
                byte s = 0;
                for (int i = 0; i < full.Length; i++)
                    s = (byte)(s ^ Mul(full[i], Exp[((full.Length - 1 - i) * j) % 255]));
                if (s != 0) { syndromesZero = false; break; }
            }
        }

        // Parse the byte-mode payload from the concatenated data codewords.
        var data = new List<byte>();
        for (int b = 0; b < numBlocks; b++) data.AddRange(dataBlocks[b]);

        int bp = 0;
        int Read(int count) { int val = 0; for (int k = 0; k < count; k++) { int bi = bp / 8, bit = 7 - (bp % 8); int by = bi < data.Count ? data[bi] : 0; val = (val << 1) | ((by >> bit) & 1); bp++; } return val; }

        int mode = Read(4);
        if (mode != 0b0100) return ($"<non-byte mode {mode}>", syndromesZero);
        int len = Read(8);
        var bytes = new byte[len];
        for (int i = 0; i < len; i++) bytes[i] = (byte)Read(8);
        return (Encoding.GetEncoding("ISO-8859-1").GetString(bytes), syndromesZero);
    }

    private static int ReadMask(bool[,] m)
    {
        int[] hCols = { 0, 1, 2, 3, 4, 5, 7, 8 };
        int[] vRows = { 7, 5, 4, 3, 2, 1, 0 };
        int raw = 0;
        for (int i = 0; i < 8; i++) if (m[8, hCols[i]]) raw |= 1 << (14 - i);
        for (int i = 0; i < 7; i++) if (m[vRows[i], 8]) raw |= 1 << (6 - i);
        return ((raw ^ 0x5412) >> 10) & 0x7;
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

    private static bool[,] FunctionMap(int n, int version)
    {
        var fn = new bool[n, n];
        void Fill(int r0, int c0, int r1, int c1)
        {
            for (int r = r0; r <= r1; r++)
                for (int c = c0; c <= c1; c++)
                    if (r >= 0 && c >= 0 && r < n && c < n) fn[r, c] = true;
        }

        Fill(0, 0, 7, 7); Fill(0, n - 8, 7, n - 1); Fill(n - 8, 0, n - 1, 7); // finders + separators
        for (int i = 8; i < n - 8; i++) { fn[6, i] = true; fn[i, 6] = true; } // timing

        int[][] centers = { Array.Empty<int>(), new[] { 6, 18 }, new[] { 6, 22 }, new[] { 6, 26 } };
        var ce = centers[version - 1];
        if (ce.Length > 0)
        {
            int cc = ce[1];
            for (int r = cc - 2; r <= cc + 2; r++)
                for (int c = cc - 2; c <= cc + 2; c++) fn[r, c] = true; // single alignment pattern
        }

        fn[4 * version + 9, 8] = true; // dark module
        for (int i = 0; i <= 8; i++) { fn[8, i] = true; fn[i, 8] = true; } // format reserve
        for (int i = n - 8; i < n; i++) fn[8, i] = true;
        for (int i = n - 7; i < n; i++) fn[i, 8] = true;
        return fn;
    }
}
