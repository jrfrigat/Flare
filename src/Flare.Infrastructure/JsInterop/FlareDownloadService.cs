using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Default <see cref="IFlareDownload"/> over a Blob + anchor click.</summary>
public sealed class FlareDownloadService(IJSRuntime js) : IFlareDownload
{
    /// <summary>Triggers a browser download of the given text content as a file.</summary>
    public ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false) =>
        js.InvokeVoidAsync("FlareDownload.download", filename, content, mimeType, withBom);

    /// <summary>Triggers a browser download of the given content as a CSV file.</summary>
    public ValueTask DownloadCsvAsync(string filename, string csv) =>
        DownloadAsync(filename, csv, "text/csv", withBom: true);

    /// <summary>Triggers a browser download of the given binary content as a file.</summary>
    public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null) =>
        js.InvokeVoidAsync("FlareDownload.downloadBase64", filename, Convert.ToBase64String(bytes),
            mimeType ?? "application/octet-stream");
}
