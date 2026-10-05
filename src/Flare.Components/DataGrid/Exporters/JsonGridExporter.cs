using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Collections;

namespace Flare.Components;

/// <summary>Standard JSON exporter (array of objects keyed by column title).</summary>
public sealed class JsonGridExporter<TItem> : IDataGridExporter<TItem>
{
    /// <summary>Optional source-generated metadata for complex cell types and custom converters.
    /// Without it, JSON DOM values, primitive values, enumerables and string-key dictionaries are supported.
    /// Other types throw rather than being converted to text. Register every runtime type used by complex cells.</summary>
    public JsonSerializerContext? SerializerContext { get; init; }

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
        using (var json = new Utf8JsonWriter(buffer, new JsonWriterOptions
        {
            Encoder = SerializerContext?.Options.Encoder,
            Indented = SerializerContext?.Options.WriteIndented ?? false,
            MaxDepth = SerializerContext?.Options.MaxDepth is > 0 and var depth ? depth : 64,
        }))
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

    // Source-generated metadata takes precedence, including converters for otherwise built-in types.
    private void WriteValue(Utf8JsonWriter json, object? value)
    {
        if (value is not null && SerializerContext?.GetTypeInfo(value.GetType()) is { } metadata)
        {
            JsonSerializer.Serialize(json, value, metadata);
            return;
        }
        switch (value)
        {
            case null: json.WriteNullValue(); break;
            case string s: json.WriteStringValue(s); break;
            case char c: json.WriteStringValue(c.ToString()); break;
            case bool b: json.WriteBooleanValue(b); break;
            case Enum e when e.GetTypeCode() == TypeCode.UInt64:
                json.WriteNumberValue(Convert.ToUInt64(e, System.Globalization.CultureInfo.InvariantCulture)); break;
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
            case Uri uri: json.WriteStringValue(uri.OriginalString); break;
            case byte[] bytes: json.WriteBase64StringValue(bytes); break;
            case JsonElement element: element.WriteTo(json); break;
            case JsonDocument document: document.RootElement.WriteTo(json); break;
            case JsonNode node: node.WriteTo(json); break;
            case Array array when array.Rank > 1:
                throw new NotSupportedException("JSON export does not support multidimensional arrays.");
            case IDictionary dictionary:
                json.WriteStartObject();
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (entry.Key is not string key)
                        throw new NotSupportedException("JSON export requires SerializerContext metadata for dictionaries with non-string keys.");
                    json.WritePropertyName(key);
                    WriteValue(json, entry.Value);
                }
                json.WriteEndObject();
                break;
            case IEnumerable sequence:
                json.WriteStartArray();
                foreach (var item in sequence) WriteValue(json, item);
                json.WriteEndArray();
                break;
            default:
                throw new NotSupportedException($"JSON export requires source-generated SerializerContext metadata for '{value.GetType()}'.");
        }
    }
}
