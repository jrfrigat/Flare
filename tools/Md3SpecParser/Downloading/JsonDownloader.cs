using System.Diagnostics;
using System.Text;

namespace Flare.Tools.Md3SpecParser.Downloading;

/// <summary>Byte-level download progress for one source.</summary>
/// <param name="Read">Bytes received so far.</param>
/// <param name="Total">Total bytes if known (Content-Length), else null.</param>
public readonly record struct DownloadProgress(long Read, long? Total);

/// <summary>
/// Fetches raw JSON for a source. Supports both <c>http(s)</c> urls and local file
/// paths, and reports byte-level progress while downloading.
/// </summary>
public sealed class JsonDownloader : IDisposable
{
    private readonly HttpClient _http;

    /// <summary>Creates a downloader with a sensible default timeout.</summary>
    public JsonDownloader()
    {
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("Md3SpecParser/1.0");
    }

    /// <summary>
    /// Returns the JSON content for <paramref name="url"/>, reporting download progress
    /// via <paramref name="progress"/>. Local paths are read directly.
    /// </summary>
    public async Task<string> GetAsync(
        string url, IProgress<DownloadProgress>? progress = null, CancellationToken ct = default)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            return await GetHttpAsync(uri, progress, ct).ConfigureAwait(false);

        if (File.Exists(url))
        {
            var text = await File.ReadAllTextAsync(url, ct).ConfigureAwait(false);
            var bytes = Encoding.UTF8.GetByteCount(text);
            progress?.Report(new DownloadProgress(bytes, bytes));
            return text;
        }

        throw new InvalidOperationException(
            $"Source is neither a reachable http(s) url nor an existing file: {url}");
    }

    private async Task<string> GetHttpAsync(Uri uri, IProgress<DownloadProgress>? progress, CancellationToken ct)
    {
        using var response = await _http
            .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var total = response.Content.Headers.ContentLength;
        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);

        var capacity = total is > 0 and < int.MaxValue ? (int)total.Value : 0;
        using var buffer = new MemoryStream(capacity);
        var chunk = new byte[81920];
        long read = 0;
        var clock = Stopwatch.StartNew();

        int n;
        while ((n = await stream.ReadAsync(chunk, ct).ConfigureAwait(false)) > 0)
        {
            buffer.Write(chunk, 0, n);
            read += n;
            if (progress is not null && clock.ElapsedMilliseconds >= 50)
            {
                progress.Report(new DownloadProgress(read, total));
                clock.Restart();
            }
        }

        progress?.Report(new DownloadProgress(read, total ?? read));
        return Encoding.UTF8.GetString(buffer.GetBuffer(), 0, (int)buffer.Length);
    }

    /// <inheritdoc />
    public void Dispose() => _http.Dispose();
}
