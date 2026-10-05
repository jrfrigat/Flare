using System.Text.Json;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-191: the JSON export writes its cells without reflecting over their types, so it keeps working in a trimmed
/// app - and writes the same JSON as System.Text.Json for every value type a grid cell commonly holds.
/// </summary>
public class JsonGridExporterTests
{
    private enum Tier { Low, High = 7 }
    private enum ByteEnum : byte { Max = byte.MaxValue }
    private enum SByteEnum : sbyte { Min = sbyte.MinValue }
    private enum ShortEnum : short { Min = short.MinValue }
    private enum UShortEnum : ushort { Max = ushort.MaxValue }
    private enum IntEnum : int { Min = int.MinValue }
    private enum UIntEnum : uint { Max = uint.MaxValue }
    private enum LongEnum : long { Min = long.MinValue }
    private enum ULongEnum : ulong { Zero, AboveSigned = (ulong)long.MaxValue + 1, Max = ulong.MaxValue }

    [Fact]
    public async Task Export_PreservesEveryEnumUnderlyingTypeAndUnsignedBoundary()
    {
        object?[] row = [ByteEnum.Max, SByteEnum.Min, ShortEnum.Min, UShortEnum.Max,
            IntEnum.Min, UIntEnum.Max, LongEnum.Min, ULongEnum.Zero,
            (ULongEnum)long.MaxValue, ULongEnum.AboveSigned, ULongEnum.Max];
        var columns = row.Select((_, i) => new FlareExportColumn<object?[]>($"c{i}", r => r[i])).ToList();
        var download = new Capture();

        await new JsonGridExporter<object?[]>().ExportAsync(
            new DataGridExportData<object?[]> { Columns = columns, Rows = [row], FileName = "enums" }, download);

        Assert.Equal(JsonSerializer.Serialize(new[] { columns.ToDictionary(c => c.Title, c => c.Value(row)) }),
            download.Content);
    }

    private sealed class Capture : IFlareDownload
    {
        public string? Content;
        public ValueTask DownloadAsync(string filename, string content, string? mimeType = null, bool withBom = false)
        {
            Content = content;
            return ValueTask.CompletedTask;
        }
        public ValueTask DownloadCsvAsync(string filename, string csv) => ValueTask.CompletedTask;
        public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string? mimeType = null) => ValueTask.CompletedTask;
    }

    [Fact]
    public async Task Export_MatchesSystemTextJson()
    {
        object?[] row =
        [
            "Ann \"A\" <b>", null, true, 42, -7L, (short)3, 1.5d, 2.25f, 19.99m, Tier.High,
            new DateTime(2026, 10, 4, 13, 5, 0, DateTimeKind.Utc), new DateTimeOffset(2026, 10, 4, 13, 5, 0, TimeSpan.FromHours(3)),
            new DateOnly(2026, 10, 4), new TimeOnly(13, 5, 30), TimeSpan.FromMinutes(95),
            Guid.Parse("4b1f5f43-9b8c-4c55-9a0b-3a2d1f7c9e11"), "привет",
        ];
        var columns = row.Select((_, i) => new FlareExportColumn<object?[]>($"c{i}", r => r[i])).ToList();
        var download = new Capture();

        await new JsonGridExporter<object?[]>().ExportAsync(
            new DataGridExportData<object?[]> { Columns = columns, Rows = [row, row], FileName = "people" }, download);

        var expected = JsonSerializer.Serialize(Enumerable.Repeat(0, 2)
            .Select(_ => columns.ToDictionary(c => c.Title, c => c.Value(row))).ToList());
        Assert.Equal(expected, download.Content);
    }
}
