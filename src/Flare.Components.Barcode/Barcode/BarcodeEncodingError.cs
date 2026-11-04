using System.Globalization;

namespace Flare.Components;

internal enum BarcodeFailure
{
    Empty, AsciiOnly, DigitsOnly, DigitCount, Code39Character, ItfDigitsOnly,
    CodabarDelimiters, CodabarCharacter, Unexpected
}

// Machine-readable failure for the UI. The legacy encoder callback retains its neutral diagnostic.
internal sealed record BarcodeEncodingError(BarcodeFailure Code, params object[] Arguments)
{
    public string InvariantMessage => Code == BarcodeFailure.Unexpected ? (string)Arguments[0]
        : string.Format(CultureInfo.InvariantCulture, Code switch
        {
            BarcodeFailure.Empty => "The value is empty.",
            BarcodeFailure.AsciiOnly => "Code 128 encodes ASCII only; '{0}' is outside it.",
            BarcodeFailure.DigitsOnly => "Only digits can be encoded in this symbology.",
            BarcodeFailure.DigitCount => "Expected {0} digits (or {1} with the check digit), got {2}.",
            BarcodeFailure.Code39Character => "Code 39 cannot encode '{0}'.",
            BarcodeFailure.ItfDigitsOnly => "ITF-14 encodes digits only.",
            BarcodeFailure.CodabarDelimiters => "Codabar needs a start and stop letter A-D around the data.",
            BarcodeFailure.CodabarCharacter => "Codabar cannot encode '{0}'.",
            _ => "Cannot encode this value"
        }, Arguments);
}
