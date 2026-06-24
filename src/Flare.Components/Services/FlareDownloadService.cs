using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Triggers a client-side file download from in-memory content. Inject instead of using IJSRuntime.</summary>
public interface IFlareDownload
{
    /// <summary>Downloads <paramref name="content"/> as <paramref name="filename"/>.</summary>
    ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false);
    /// <summary>Downloads CSV content (text/csv with a UTF-8 BOM for spreadsheet compatibility).</summary>
    ValueTask DownloadCsvAsync(string filename, string csv);
    /// <summary>Downloads raw bytes (e.g. a generated .xlsx) as <paramref name="filename"/>.</summary>
    ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null);
}

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
