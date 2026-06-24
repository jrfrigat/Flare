using Flare.Tools.Md3SpecParser.Generation;

namespace Flare.Tools.Md3SpecParser.Cli;

/// <summary>
/// Renders per-type generation progress as an in-place console bar showing how many
/// sources have been downloaded and parsed. Falls back to plain lines when output is
/// redirected (e.g. CI logs).
/// </summary>
public sealed class ProgressReporter : IProgress<GenProgress>
{
    private const int BarWidth = 22;

    private readonly bool _plain = Console.IsOutputRedirected;
    private string? _currentType;

    /// <inheritdoc />
    public void Report(GenProgress p)
    {
        if (_plain)
        {
            ReportPlain(p);
            return;
        }

        // Finish the previous type's line before starting a new one.
        if (_currentType is not null && _currentType != p.Type)
            Console.WriteLine();
        _currentType = p.Type;

        var done = p.Phase is GenPhase.Written or GenPhase.Skipped or GenPhase.Failed;
        Console.Write('\r' + Line(p).PadRight(SafeWidth()));
        if (done)
        {
            Console.WriteLine();
            _currentType = null;
        }
    }

    private void ReportPlain(GenProgress p)
    {
        switch (p.Phase)
        {
            case GenPhase.Written:
                Console.WriteLine($"  {p.Type}: downloaded {p.Downloaded}/{p.TotalSources}, parsed {p.Parsed}/{p.TotalSources} - written");
                break;
            case GenPhase.Skipped:
                Console.WriteLine($"  {p.Type}: skipped (exists)");
                break;
            case GenPhase.Failed:
                Console.WriteLine($"  {p.Type}: failed");
                break;
        }
    }

    private static string Line(GenProgress p)
    {
        var total = Math.Max(p.TotalSources, 1);

        // The bar is split into one slot per file; the in-flight file fills its slot by
        // bytes received, so the whole bar advances at download speed.
        var currentFraction = p.Phase == GenPhase.Download && p.CurrentTotalBytes is > 0
            ? Math.Clamp(p.CurrentBytes / (double)p.CurrentTotalBytes.Value, 0, 1)
            : 0;

        var fraction = p.Phase switch
        {
            GenPhase.Written => 1.0,
            GenPhase.Skipped or GenPhase.Failed => 0.0,
            _ => Math.Clamp((p.Downloaded + currentFraction) / total, 0, 1),
        };

        var filled = (int)Math.Round(fraction * BarWidth);
        var bar = new string('█', filled) + new string('░', BarWidth - filled);

        var status = p.Phase switch
        {
            GenPhase.Download => $"↓ {Short(p.Current)} {Size(p.CurrentBytes, p.CurrentTotalBytes)}",
            GenPhase.Parse => $"⚙ {Short(p.Current)}",
            GenPhase.Written => "done",
            GenPhase.Skipped => "skipped",
            GenPhase.Failed => "failed",
            _ => string.Empty,
        };

        return $"  {Truncate(p.Type, 16),-16} [{bar}] {p.Downloaded}/{total}  {status}";
    }

    private static string Size(long read, long? total)
    {
        static string Mb(long b) => (b / 1_048_576.0).ToString("0.0") + " MB";
        return total is > 0 ? $"{Mb(read)}/{Mb(total.Value)}" : Mb(read);
    }

    private static string Short(string? source)
    {
        if (string.IsNullOrEmpty(source)) return string.Empty;
        var slash = source.LastIndexOfAny(new[] { '/', '\\' });
        return Truncate(slash >= 0 ? source[(slash + 1)..] : source, 32);
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..(max - 1)] + "...";

    private static int SafeWidth()
    {
        try { return Math.Max(Console.WindowWidth - 1, 40); }
        catch { return 80; }
    }
}
