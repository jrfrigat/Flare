using System.Net;
using System.Text;
using Flare.Components;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components.Tests;

/// <summary>
/// FlareHttpUpload is the shipped upload delegate: whatever it puts on the wire is what an application
/// actually sends, so the test pins the multipart shape, the extra data, progress and cleanup.
/// </summary>
public class FlareHttpUploadTests
{
    private static readonly byte[] Bytes = Enumerable.Range(0, 150_000).Select(i => (byte)(i % 251)).ToArray();

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public byte[]? Body { get; private set; }
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public Exception? Throw { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = await request.Content!.ReadAsByteArrayAsync(cancellationToken);
            if (Throw is not null) throw Throw;
            return new HttpResponseMessage(Status);
        }
    }

    private sealed class FakeFile(string name, byte[] bytes, string contentType = "text/plain") : IBrowserFile
    {
        public TrackingStream Stream { get; } = new(bytes.Length, bytes);
        public string Name { get; } = name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public long Size { get; } = bytes.Length;
        public string ContentType { get; } = contentType;
        public long MaxAllowedSize { get; private set; }
        public CancellationToken Token { get; private set; }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            MaxAllowedSize = maxAllowedSize;
            Token = cancellationToken;
            return Stream;
        }
    }

    private sealed class TrackingStream(int size, byte[]? bytes = null, bool cancelOnRead = false) : MemoryStream(bytes ?? new byte[size])
    {
        public bool Disposed { get; private set; }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancelOnRead && cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);
            return base.ReadAsync(buffer, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class Recording : IProgress<long>
    {
        public List<long> Values { get; } = new();
        public void Report(long value) => Values.Add(value);
    }

    private static FlareUploadContext Context(FakeFile file, IProgress<long>? progress = null, CancellationToken token = default)
        => new(file, progress ?? new Recording(), token);

    [Fact]
    public async Task To_SendsTheFileAsMultipart()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("report.txt", Bytes);

        await upload.To("https://api.test/upload")(Context(file, token: Xunit.TestContext.Current.CancellationToken));

        var body = Encoding.UTF8.GetString(handler.Body!);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("https://api.test/upload", handler.Request.RequestUri!.ToString());
        Assert.StartsWith("multipart/form-data", handler.Request.Content!.Headers.ContentType!.MediaType);
        Assert.Matches("name=\"?file\"?", body);
        Assert.Matches("filename=\"?report.txt\"?", body);
        Assert.Contains("Content-Type: text/plain", body);
        Assert.True(handler.Body!.AsSpan().IndexOf(Bytes) >= 0);
    }

    [Fact]
    public async Task To_UsesTheGivenFieldNameAndFallsBackForAnEmptyContentType()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("blob.bin", Bytes, contentType: "");

        await upload.To("https://api.test/upload", "payload")(Context(file, token: Xunit.TestContext.Current.CancellationToken));

        var body = Encoding.UTF8.GetString(handler.Body!);
        Assert.Matches("name=\"?payload\"?", body);
        Assert.Contains("Content-Type: application/octet-stream", body);
    }

    [Fact]
    public async Task To_AddsFieldsAndHeaders()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("report.txt", Bytes);

        var transfer = upload.To("https://api.test/upload",
            headers: new Dictionary<string, string> { ["X-Request-Id"] = "abc" },
            fields: new Dictionary<string, string> { ["author"] = "Jane" });
        await transfer(Context(file, token: Xunit.TestContext.Current.CancellationToken));

        var body = Encoding.UTF8.GetString(handler.Body!);
        Assert.Matches("name=\"?author\"?", body);
        Assert.Contains("Jane", body);
        Assert.Equal("abc", handler.Request!.Headers.GetValues("X-Request-Id").Single());
    }

    [Fact]
    public async Task To_ReportsMonotonicCumulativeProgressEndingAtTheFileSize()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var progress = new Recording();
        var file = new FakeFile("report.txt", Bytes);

        await upload.To("https://api.test/upload")(Context(file, progress, Xunit.TestContext.Current.CancellationToken));

        Assert.True(progress.Values.Count > 1);
        for (var i = 1; i < progress.Values.Count; i++)
            Assert.True(progress.Values[i] > progress.Values[i - 1]);
        Assert.Equal(file.Size, progress.Values[^1]);
    }

    [Fact]
    public async Task To_ReportsNothingForAnEmptyFile()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var progress = new Recording();
        var file = new FakeFile("empty.txt", Array.Empty<byte>());

        await upload.To("https://api.test/upload")(Context(file, progress, Xunit.TestContext.Current.CancellationToken));

        Assert.Empty(progress.Values);
    }

    [Fact]
    public async Task To_PassesTheFileSizeAsTheStreamCeilingAndTheCallerToken()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("report.txt", Bytes);
        using var cts = new CancellationTokenSource();

        await upload.To("https://api.test/upload")(Context(file, token: cts.Token));

        Assert.Equal(file.Size, file.MaxAllowedSize);
        Assert.Equal(cts.Token, file.Token);
    }

    [Fact]
    public async Task To_DisposesTheFileStreamOnSuccess()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("report.txt", Bytes);

        await upload.To("https://api.test/upload")(Context(file, token: Xunit.TestContext.Current.CancellationToken));

        Assert.True(file.Stream.Disposed);
    }

    [Fact]
    public async Task To_SurfacesAnHttpErrorAndStillDisposesTheStream()
    {
        var handler = new CapturingHandler { Status = HttpStatusCode.BadRequest };
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("report.txt", Bytes);

        await Assert.ThrowsAsync<HttpRequestException>(() => upload.To("https://api.test/upload")(Context(file, token: Xunit.TestContext.Current.CancellationToken)));

        Assert.True(file.Stream.Disposed);
    }

    [Fact]
    public async Task To_HonorsCancellationAndStillDisposesTheStream()
    {
        var handler = new CapturingHandler();
        var upload = new FlareHttpUpload(new HttpClient(handler));
        var file = new FakeFile("report.txt", Bytes);
        var stream = new TrackingStream(Bytes.Length, Bytes, cancelOnRead: true);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => upload.To("https://api.test/upload")(new FlareUploadContext(new CancellingFile(file, stream), new Recording(), cts.Token)));

        Assert.True(stream.Disposed);
    }

    [Theory]
    [InlineData("", "file")]
    [InlineData("   ", "file")]
    [InlineData("https://api.test/upload", "")]
    [InlineData("https://api.test/upload", "  ")]
    public void To_RejectsAnEmptyUrlOrFieldName(string url, string fieldName)
    {
        var upload = new FlareHttpUpload(new HttpClient(new CapturingHandler()));

        Assert.Throws<ArgumentException>(() => upload.To(url, fieldName));
    }

    private sealed class CancellingFile(FakeFile inner, Stream stream) : IBrowserFile
    {
        public string Name => inner.Name;
        public DateTimeOffset LastModified => inner.LastModified;
        public long Size => inner.Size;
        public string ContentType => inner.ContentType;
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default) => stream;
    }
}
