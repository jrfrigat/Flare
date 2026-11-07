using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Flare.Components;

internal static class NumericFieldConversion<TValue>
{
    private static readonly Type _targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
    private static readonly TypeCode _targetCode = Type.GetTypeCode(_targetType);
    internal static readonly bool IsIntegral = !_targetType.IsEnum && _targetCode is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or
        TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;

    // A percent format writes 0.25 as "25%"; pasted text carrying the symbol must come back as the
    // fraction. .NET has no percent parse style, so the symbol is removed here and the number scaled.
    internal static bool IsDecimalBacked => _targetType == typeof(decimal) || IsIntegral;

    // A standard P/p format (with optional precision digits), or a custom format with an unescaped and
    // unquoted percent placeholder.
    internal static bool IsPercentFormat(string format)
    {
        var trimmed = format.Trim();
        if (trimmed.Length > 0 && (trimmed[0] is 'P' or 'p') && trimmed.Skip(1).All(char.IsDigit)) return true;
        for (var i = 0; i < trimmed.Length; i++)
        {
            var ch = trimmed[i];
            if (ch == '\\') { i++; continue; }
            if (ch is '"' or '\'')
            {
                var quote = ch;
                for (i++; i < trimmed.Length && trimmed[i] != quote; i++)
                {
                    if (trimmed[i] == '\\') i++;
                }
                continue;
            }
            if (ch == '%') return true;
        }
        return false;
    }

    // Removing a prefix or suffix can use a slice. A symbol between digits needs a single string.
    private static ReadOnlySpan<char> StripPercentSymbol(string raw, string? symbol, out bool percent)
    {
        percent = false;
        if (string.IsNullOrEmpty(symbol)) return raw;
        var index = raw.IndexOf(symbol, StringComparison.Ordinal);
        if (index < 0) return raw;
        percent = true;
        var end = index + symbol.Length;
        if (index == 0) return raw.AsSpan(end).Trim();
        if (end == raw.Length) return raw.AsSpan(0, index).Trim();
        return string.Concat(raw.AsSpan(0, index), raw.AsSpan(end)).AsSpan().Trim();
    }

    internal static bool TryParseNumber(string raw, CultureInfo culture, bool percentFormat, out double value)
    {
        var text = StripPercentSymbol(raw, percentFormat ? culture.NumberFormat.PercentSymbol : null, out var percent);
        if (!double.TryParse(text, NumberStyles.Any, culture, out value))
        {
            Span<char> buffer = text.Length <= 128 ? stackalloc char[text.Length] : new char[text.Length];
            var length = NormalizeNumber(text, culture.NumberFormat.NumberDecimalSeparator, buffer);
            if (!double.TryParse(buffer[..length], NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return false;
        }
        if (percent) value /= 100d;
        return true;
    }

    internal static bool TryParseDecimalNumber(string raw, CultureInfo culture, bool percentFormat, out decimal value)
    {
        var text = StripPercentSymbol(raw, percentFormat ? culture.NumberFormat.PercentSymbol : null, out var percent);
        if (!decimal.TryParse(text, NumberStyles.Any, culture, out value))
        {
            // Do not strip an exponent from a valid number that overflowed the decimal parser.
            if (double.TryParse(text, NumberStyles.Any, culture, out _)) return false;

            Span<char> buffer = text.Length <= 128 ? stackalloc char[text.Length] : new char[text.Length];
            var length = NormalizeNumber(text, culture.NumberFormat.NumberDecimalSeparator, buffer);
            if (!decimal.TryParse(buffer[..length], NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return false;
        }
        if (percent) value /= 100m;
        return true;
    }

    private static int NormalizeNumber(ReadOnlySpan<char> text, string separator, Span<char> buffer)
    {
        var length = 0;
        foreach (var character in text)
        {
            if (char.IsDigit(character) || character is '-' or '+') buffer[length++] = character;
            else if (separator.Length == 1 && character == separator[0]) buffer[length++] = '.';
        }
        return length;
    }

    internal static bool TryConvertDecimal(decimal value, out TValue? converted)
    {
        converted = default;
        if (_targetCode == TypeCode.Decimal)
        {
            converted = AsValue(value);
            return true;
        }
        if (value != decimal.Truncate(value)) return false;

        try
        {
            converted = _targetCode switch
            {
                TypeCode.Byte => AsValue(decimal.ToByte(value)),
                TypeCode.SByte => AsValue(decimal.ToSByte(value)),
                TypeCode.Int16 => AsValue(decimal.ToInt16(value)),
                TypeCode.UInt16 => AsValue(decimal.ToUInt16(value)),
                TypeCode.Int32 => AsValue(decimal.ToInt32(value)),
                TypeCode.UInt32 => AsValue(decimal.ToUInt32(value)),
                TypeCode.Int64 => AsValue(decimal.ToInt64(value)),
                TypeCode.UInt64 => AsValue(decimal.ToUInt64(value)),
                _ => default,
            };
            return true;
        }
        catch (OverflowException) { return false; }
    }

    internal static TValue? ConvertFloating(double number, string? original = null)
    {
        try
        {
            var converter = InvariantConverter.For(_targetType);
            if (original is null && converter.GetType() == typeof(DoubleConverter) && _targetType == typeof(double))
                return AsValue(number);

            if (converter.CanConvertFrom(typeof(string)))
                return (TValue?)converter.ConvertFromInvariantString(original ?? number.ToString(CultureInfo.InvariantCulture));
        }
        catch { }
        return default;
    }

    // Callers dispatch by the exact underlying type. Reinterpret the matching value or its
    // nullable representation without routing it through an object.
    private static TValue AsValue<TNumber>(TNumber value) where TNumber : struct
    {
        if (typeof(TValue) == typeof(TNumber)) return Unsafe.As<TNumber, TValue>(ref value);
        Debug.Assert(typeof(TValue) == typeof(TNumber?));
        TNumber? nullable = value;
        return Unsafe.As<TNumber?, TValue>(ref nullable);
    }
}
