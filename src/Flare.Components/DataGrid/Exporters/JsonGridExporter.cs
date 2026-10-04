using System.Text;
using System.Text.Json;

namespace Flare.Components;

/// <summary>Standard JSON exporter (array of objects keyed by column title).</summary>
public sealed class JsonGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Unique exporter id.</summary>
    public string Id => "JSON";
    /// <summary>Display label for the export action.</summary>
    public string Label => "JSON";
    /// <summary>Material Symbols icon name for the export action.</summary>
    public FlareIcon? Icon => FlareIcons.DataObject;

    /// <summary>Exports the grid rows to a JSON file and triggers its download.</summary>
    public async Task ExportAsync(DataGridExportData<TItem> data, IFlareDownload download)
    {
        using var buffer = new MemoryStream();
        using (var json = new Utf8JsonWriter(buffer))
        {
            json.WriteStartArray();
            foreach (var row in data.Rows)
            {
                json.WriteStartObject();
                foreach (var c in data.Columns)
                {
                    json.WritePropertyName(c.Title);
                    WriteValue(json, c.Value(row));
                }
                json.WriteEndObject();
            }
            json.WriteEndArray();
        }
        var file = data.FileName + ".json";
        await download.DownloadAsync(file, Encoding.UTF8.GetString(buffer.ToArray()), "application/json");
    }

    // Written by the value's own type, as System.Text.Json writes them, without reflecting over it: a trimmed app
    // keeps no metadata for the cell types. A value of any other type is written as its text.
    private static void WriteValue(Utf8JsonWriter json, object? value)
    {
        switch (value)
        {
            case null: json.WriteNullValue(); break;
            case string s: json.WriteStringValue(s); break;
            case bool b: json.WriteBooleanValue(b); break;
            case Enum e: json.WriteNumberValue(Convert.ToInt64(e, System.Globalization.CultureInfo.InvariantCulture)); break;
            case byte or sbyte or short or ushort or int or uint or long:
                json.WriteNumberValue(Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture)); break;
            case ulong u: json.WriteNumberValue(u); break;
            case float f: json.WriteNumberValue(f); break;
            case double d: json.WriteNumberValue(d); break;
            case decimal m: json.WriteNumberValue(m); break;
            case DateTime dt: json.WriteStringValue(dt); break;
            case DateTimeOffset dto: json.WriteStringValue(dto); break;
            case DateOnly date: json.WriteStringValue(date.ToString("O", System.Globalization.CultureInfo.InvariantCulture)); break;
            case TimeOnly time: json.WriteStringValue(time.ToString("HH':'mm':'ss.FFFFFFF", System.Globalization.CultureInfo.InvariantCulture)); break;
            case TimeSpan span: json.WriteStringValue(span.ToString("c", System.Globalization.CultureInfo.InvariantCulture)); break;
            case Guid g: json.WriteStringValue(g); break;
            default: json.WriteStringValue(value.ToString()); break;
        }
    }
}
