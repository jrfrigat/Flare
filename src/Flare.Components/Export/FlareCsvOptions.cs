using System.Globalization;

namespace Flare.Components;

/// <summary>
/// How a CSV file is written: separator, line ending, formula guard, number/date culture and whether
/// the bytes carry a UTF-8 byte order mark. Two presets cover the split that makes CSV awkward - a
/// file read by a program wants RFC 4180, a file opened by a spreadsheet wants the local list
/// separator and the marker that tells the spreadsheet the text is UTF-8.
/// </summary>
public sealed record FlareCsvOptions
{
    /// <summary>Field separator. RFC 4180 says comma; spreadsheets follow the locale instead.</summary>
    public char Delimiter { get; init; } = ',';

    /// <summary>Record separator. RFC 4180 says CRLF, which every reader accepts.</summary>
    public string NewLine { get; init; } = "\r\n";

    /// <summary>
    /// Prefixes a value that starts with <c>= + - @</c>, tab or CR with an apostrophe, so a spreadsheet
    /// reads it as text rather than as a formula. On by default: a cell whose contents came from user
    /// input is the exact case CSV injection exploits. Turn it off only for a file a program parses.
    /// </summary>
    public bool GuardFormulas { get; init; } = true;

    /// <summary>
    /// Culture used to format raw values that carry no explicit formatter. Invariant by default, so a
    /// file written on one machine parses the same on another.
    /// </summary>
    public IFormatProvider FormatProvider { get; init; } = CultureInfo.InvariantCulture;

    /// <summary>
    /// Whether the file's bytes begin with a UTF-8 byte order mark. Without it a spreadsheet reads a
    /// UTF-8 file in the local ANSI codepage and non-ASCII text arrives as mojibake. This is a property
    /// of the BYTES: <see cref="FlareCsv.Build{TRow}(IEnumerable{TRow}, IReadOnlyList{FlareExportColumn{TRow}}, FlareCsvOptions?)"/>
    /// returns text and never prefixes the marker - it is applied where the text becomes a file.
    /// </summary>
    public bool ByteOrderMark { get; init; }

    /// <summary>RFC 4180: comma-separated, CRLF, invariant culture, no byte order mark. The choice for
    /// a file another program reads.</summary>
    public static FlareCsvOptions Rfc4180 => new();

    /// <summary>
    /// The dialect the local spreadsheet opens without an import wizard: the current culture's list
    /// separator (a semicolon wherever the decimal mark is a comma), current-culture numbers and dates,
    /// and a UTF-8 byte order mark. Read from <see cref="CultureInfo.CurrentCulture"/> at each access,
    /// so it follows the app's culture rather than freezing the one that started it.
    /// </summary>
    public static FlareCsvOptions Spreadsheet
    {
        get
        {
            var culture = CultureInfo.CurrentCulture;
            var separator = culture.TextInfo.ListSeparator;
            return new FlareCsvOptions
            {
                Delimiter = separator.Length > 0 ? separator[0] : ',',
                FormatProvider = culture,
                ByteOrderMark = true,
            };
        }
    }
}
