namespace Flare.Components;

/// <summary>Standard JSON exporter (array of objects keyed by column title).</summary>
public sealed class JsonGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "JSON";
    /// <summary>Display label for the export action.</summary>
    public string Label => "JSON";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public string? Icon => "data_object";

    /// <summary>Exports the grid rows to a JSON file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        var rows = data.Rows.Select(row =>
        {
            var dict = new Dictionary<string, object?>();
            foreach (var c in data.Columns) dict[c.Title] = c.Value(row);
            return dict;
        }).ToList();
        var json = System.Text.Json.JsonSerializer.Serialize(rows);
        var file = data.FileName + ".json";
        await download.DownloadAsync(file, json, "application/json");
    }
}
