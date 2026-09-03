using System.Globalization;
using Flare.Components;

namespace Flare.Core.Tests;

/// <summary>
/// The standalone CSV surface: the escaping the grid exporter used to keep to itself, plus the two
/// dialect knobs that decide whether a spreadsheet opens the file or an import wizard does.
/// </summary>
public class FlareCsvTests
{
    private sealed record Order(string Number, decimal Amount);

    private static readonly IReadOnlyList<FlareExportColumn<Order>> Columns =
    [
        new FlareExportColumn<Order>("Number", o => o.Number),
        new FlareExportColumn<Order>("Amount", o => o.Amount),
    ];

    [Fact]
    public void Build_WritesHeaderThenRows()
    {
        var csv = FlareCsv.Build([new Order("A-1", 12.5m)], Columns);

        Assert.Equal("Number,Amount\r\nA-1,12.5\r\n", csv);
    }

    [Fact]
    public void Build_QuotesTheDelimiterInUse_NotAlwaysTheComma()
    {
        // The cell holds a semicolon and no comma. Under RFC 4180 nothing needs quoting; under a
        // semicolon dialect the same cell would split the record in two if it were left bare.
        var rows = new[] { new[] { "one;two" } };

        Assert.Equal("one;two\r\n", FlareCsv.Build([], rows, FlareCsvOptions.Rfc4180));
        Assert.Equal("\"one;two\"\r\n", FlareCsv.Build([], rows, FlareCsvOptions.Rfc4180 with { Delimiter = ';' }));
    }

    [Fact]
    public void Build_QuotesEmbeddedQuotesAndNewlines()
    {
        var csv = FlareCsv.Build([], new[] { new[] { "say \"hi\"", "line\nbreak" } });

        Assert.Equal("\"say \"\"hi\"\"\",\"line\nbreak\"\r\n", csv);
    }

    [Fact]
    public void Build_NeutralisesAFormulaLead_AndCanBeToldNotTo()
    {
        var rows = new[] { new[] { "=1+1" } };

        Assert.Equal("'=1+1\r\n", FlareCsv.Build([], rows));
        Assert.Equal("=1+1\r\n", FlareCsv.Build([], rows, FlareCsvOptions.Rfc4180 with { GuardFormulas = false }));
    }

    [Fact]
    public void Build_FormatsRawValuesWithTheChosenCulture()
    {
        var ru = FlareCsvOptions.Rfc4180 with { FormatProvider = CultureInfo.GetCultureInfo("ru-RU"), Delimiter = ';' };

        var invariant = FlareCsv.Build([new Order("A-1", 12.5m)], Columns);
        var russian = FlareCsv.Build([new Order("A-1", 12.5m)], Columns, ru);

        Assert.Contains("12.5", invariant, StringComparison.Ordinal);
        Assert.Contains("12,5", russian, StringComparison.Ordinal);
    }

    [Fact]
    public void ExplicitColumnText_WinsOverTheCultureFormatting()
    {
        var columns = new[] { FlareExportColumn<Order>.Of("Amount", o => o.Amount.ToString("F2", CultureInfo.InvariantCulture)) };
        var ru = FlareCsvOptions.Rfc4180 with { FormatProvider = CultureInfo.GetCultureInfo("ru-RU") };

        Assert.Equal("Amount\r\n12.50\r\n", FlareCsv.Build([new Order("A-1", 12.5m)], columns, ru));
    }

    [Fact]
    public void Spreadsheet_TakesTheSeparatorAndTheCultureFromTheCurrentCulture()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            var options = FlareCsvOptions.Spreadsheet;

            // ru-RU writes 12,5 for the number, so its list separator has to be something else.
            Assert.Equal(';', options.Delimiter);
            Assert.True(options.ByteOrderMark);
            Assert.Equal("Number;Amount\r\nA-1;12,5\r\n", FlareCsv.Build([new Order("A-1", 12.5m)], Columns, options));

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Assert.Equal(',', FlareCsvOptions.Spreadsheet.Delimiter);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void ByteOrderMark_IsNotWrittenIntoTheText()
    {
        // It is a property of the bytes, applied where the text becomes a file. A marker inside the
        // string would be doubled by a download that adds its own.
        var csv = FlareCsv.Build([new Order("A-1", 1m)], Columns, FlareCsvOptions.Spreadsheet);

        Assert.DoesNotContain('\uFEFF', csv);
    }

    [Fact]
    public void Build_WithoutHeaders_WritesRowsOnly()
    {
        Assert.Equal("a,b\r\n", FlareCsv.Build([], new[] { new[] { "a", "b" } }));
    }
}
