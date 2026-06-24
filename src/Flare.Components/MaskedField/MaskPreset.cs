namespace Flare.Components;

/// <summary>
/// Ready-made mask patterns for <c>FlareMaskedField</c>. When set (and no explicit <c>Mask</c> is given),
/// the corresponding pattern is used. Pattern tokens: <c>#</c> digit, <c>A</c> letter, <c>*</c> alphanumeric.
/// </summary>
public enum MaskPreset
{
    /// <summary>No preset - use the explicit <c>Mask</c>.</summary>
    None,
    /// <summary>US phone: <c>(###) ###-####</c>.</summary>
    Phone,
    /// <summary>Date: <c>##/##/####</c>.</summary>
    Date,
    /// <summary>Time: <c>##:##</c>.</summary>
    Time,
    /// <summary>IPv4 address: <c>###.###.###.###</c>.</summary>
    IpAddress,
    /// <summary>Credit card: <c>#### #### #### ####</c>.</summary>
    CreditCard,
    /// <summary>US SSN: <c>###-##-####</c>.</summary>
    Ssn,
}
