using System.Net;
using System.Net.Http.Headers;

namespace Flare.Components.Services;

/// <inheritdoc cref="IFlareUpload" />
/// <remarks>
/// Lives in <c>Flare.Infrastructure</c> because it needs an <see cref="HttpClient"/>, and
/// <c>Flare.Components</c> takes no service implementations. Registered by <c>AddFlare</c> only when the
/// host has an <c>IHttpClientFactory</c> or an <see cref="HttpClient"/> of its own - a component
/// library must not invent an HTTP client for an application that did not ask for one.
/// </remarks>
public sealed class FlareHttpUpload : IFlareUpload
{
    private readonly HttpClient _http;

    /// <param name="http">The client to send on, supplied by the host application.</param>
    public FlareHttpUpload(HttpClient http) => _http = http;

    /// <inheritdoc />
    public Func<FlareUploadContext, Task> To(
        string url,
        string fieldName = "file",
        IReadOnlyDictionary<string, string>? headers = null,
        IReadOnlyDictionary<string, string>? fields = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);

        return async ctx =>
        {
            using var content = new MultipartFormDataContent();

            // OpenReadStream needs a ceiling; the component has already filtered on MaxFileSize, so the
            // file's own size is the honest one to pass rather than a number invented here.
            var stream = ctx.File.OpenReadStream(ctx.File.Size, ctx.CancellationToken);
            var body = new ProgressStreamContent(stream, ctx.Progress, ctx.CancellationToken);
            body.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrEmpty(ctx.File.ContentType) ? "application/octet-stream" : ctx.File.ContentType);
            content.Add(body, fieldName, ctx.File.Name);

            if (fields is not null)
                foreach (var (k, v) in fields) content.Add(new StringContent(v), k);

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            if (headers is not null)
                foreach (var (k, v) in headers) request.Headers.TryAddWithoutValidation(k, v);

            using var response = await _http.SendAsync(request, ctx.CancellationToken);
            response.EnsureSuccessStatusCode();
        };
    }

    /// <summary>Streams the file and reports the running byte count as it goes.</summary>
    /// <remarks>
    /// <c>StreamContent</c> would copy the whole body with no way to observe it, which is why an upload
    /// built on it shows a bar that jumps from 0 to 100. Copying in chunks is the only way to know.
    /// </remarks>
    private sealed class ProgressStreamContent : HttpContent
    {
        private const int ChunkSize = 64 * 1024;

        private readonly Stream _source;
        private readonly IProgress<long> _progress;
        private readonly CancellationToken _token;

        public ProgressStreamContent(Stream source, IProgress<long> progress, CancellationToken token)
        {
            _source = source;
            _progress = progress;
            _token = token;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[ChunkSize];
            long sent = 0;
            int read;
            while ((read = await _source.ReadAsync(buffer, _token)) > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, read), _token);
                sent += read;
                _progress.Report(sent);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _source.CanSeek ? _source.Length : 0;
            return _source.CanSeek;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _source.Dispose();
            base.Dispose(disposing);
        }
    }
}
