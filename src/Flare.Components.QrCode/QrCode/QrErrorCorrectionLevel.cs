namespace Flare.Components;

/// <summary>
/// QR code error correction level.
/// Higher levels allow more data recovery at the cost of reduced capacity.
/// </summary>
public enum QrErrorCorrectionLevel
{
    /// <summary>~7% of data can be restored.</summary>
    L,
    /// <summary>~15% of data can be restored (default).</summary>
    M,
    /// <summary>~25% of data can be restored.</summary>
    Q,
    /// <summary>~30% of data can be restored.</summary>
    H,
}
