using System.Globalization;

namespace Flare.Components;

/// <summary>
/// Stateless, culture-aware formatting for DataGrid cell values. Maps a CLR runtime type to a
/// <see cref="ColumnDataType"/> (auto-detection) and renders a value to display text using the
/// column's type and optional .NET format string. Kept free of component/render concerns so it can
/// be reused by cell rendering, exporters and tests.
/// </summary>
public static class DataGridValueFormatter
{
    private static readonly HashSet<Type> NumericTypes =
    [
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
        typeof(int), typeof(uint), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(decimal),
    ];

    /// <summary>Maps a CLR type to the matching <see cref="ColumnDataType"/>. Nullable types are
    /// unwrapped. Unknown types fall back to <see cref="ColumnDataType.Text"/>.</summary>
    public static ColumnDataType Infer(Type? clrType)
    {
        if (clrType is null) return ColumnDataType.Text;
        var t = Nullable.GetUnderlyingType(clrType) ?? clrType;

        if (t == typeof(bool)) return ColumnDataType.Boolean;
        if (t == typeof(DateOnly)) return ColumnDataType.Date;
        if (t == typeof(TimeOnly) || t == typeof(TimeSpan)) return ColumnDataType.Time;
        if (t == typeof(DateTime) || t == typeof(DateTimeOffset)) return ColumnDataType.DateTime;
        if (t.IsEnum) return ColumnDataType.Enum;
        if (NumericTypes.Contains(t)) return ColumnDataType.Number;
        return ColumnDataType.Text;
    }

    /// <summary>Resolves a column's effective data type: the explicit <paramref name="declared"/>
    /// type, or - when it is <see cref="ColumnDataType.Auto"/> - the type inferred from a sample
    /// runtime value.</summary>
    public static ColumnDataType Resolve(ColumnDataType declared, object? sample)
        => declared == ColumnDataType.Auto ? Infer(sample?.GetType()) : declared;

    /// <summary>Renders a value to display text for the given resolved type, honoring an optional
    /// .NET format string and the supplied culture. <paramref name="value"/> null yields
    /// <paramref name="nullText"/>. Boolean is returned as lowercase "true"/"false" for text contexts
    /// (the grid renders it as an icon instead).</summary>
    public static string FormatText(object? value, ColumnDataType type, string? format, string? nullText, CultureInfo culture)
    {
        if (value is null) return nullText ?? string.Empty;

        // An explicit format always wins for any IFormattable value.
        if (!string.IsNullOrEmpty(format) && value is IFormattable formattable)
            return formattable.ToString(format, culture);

        switch (type)
        {
            case ColumnDataType.Boolean:
                return value is bool b ? (b ? "true" : "false") : Convert.ToString(value, culture) ?? string.Empty;
            case ColumnDataType.Date:
                return value switch
                {
                    DateOnly d => d.ToString("d", culture),
                    DateTime dt => dt.ToString("d", culture),
                    DateTimeOffset dto => dto.ToString("d", culture),
                    _ => Convert.ToString(value, culture) ?? string.Empty,
                };
            case ColumnDataType.DateTime:
                return value switch
                {
                    DateTime dt => dt.ToString("g", culture),
                    DateTimeOffset dto => dto.ToString("g", culture),
                    DateOnly d => d.ToString("d", culture),
                    _ => Convert.ToString(value, culture) ?? string.Empty,
                };
            case ColumnDataType.Time:
                return value switch
                {
                    TimeOnly to => to.ToString("t", culture),
                    TimeSpan ts => ts.ToString(),
                    DateTime dt => dt.ToString("t", culture),
                    _ => Convert.ToString(value, culture) ?? string.Empty,
                };
            default:
                return Convert.ToString(value, culture) ?? string.Empty;
        }
    }

    /// <summary>Maps a resolved data type to the default inline filter editor used when a column does
    /// not set <see cref="ColumnFilterType"/> explicitly.</summary>
    public static ColumnFilterType ToFilterType(ColumnDataType type) => type switch
    {
        ColumnDataType.Number => ColumnFilterType.Number,
        ColumnDataType.Date or ColumnDataType.DateTime or ColumnDataType.Time => ColumnFilterType.Date,
        ColumnDataType.Boolean or ColumnDataType.Enum => ColumnFilterType.Select,
        _ => ColumnFilterType.Text,
    };
}
