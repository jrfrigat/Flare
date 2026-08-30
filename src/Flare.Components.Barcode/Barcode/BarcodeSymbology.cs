namespace Flare.Components;

/// <summary>
/// The 1D symbologies <see cref="FlareBarcode"/> can encode, ordered by how often they are actually
/// asked for rather than alphabetically.
/// </summary>
public enum BarcodeSymbology
{
    /// <summary>
    /// Code 128 - the general-purpose symbology. Encodes the full ASCII range and switches between its
    /// A/B/C subsets automatically, so a run of digits costs half the width it would elsewhere.
    /// </summary>
    Code128,

    /// <summary>EAN-13 retail article number: 12 digits plus a check digit this computes.</summary>
    Ean13,

    /// <summary>EAN-8, the short form for a small package: 7 digits plus a check digit.</summary>
    Ean8,

    /// <summary>UPC-A, the North American retail code: 11 digits plus a check digit.</summary>
    UpcA,

    /// <summary>
    /// Code 39 - digits, upper-case letters and a handful of symbols. Widely readable by old hardware,
    /// and about three times wider than Code 128 for the same text.
    /// </summary>
    Code39,

    /// <summary>ITF-14 shipping-container code: 13 digits plus a check digit, interleaved two-of-five.</summary>
    Itf14,

    /// <summary>Codabar - digits and six symbols, with a letter A-D at each end. Libraries and blood banks.</summary>
    Codabar,
}
