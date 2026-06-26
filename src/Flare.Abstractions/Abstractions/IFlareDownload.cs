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
