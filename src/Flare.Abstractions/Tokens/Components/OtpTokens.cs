using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for otp - component-specific geometry read by otp.css.</summary>
public sealed record OtpTokens
{
    /// <summary>Border Width.</summary>
    [CssVar(OtpField.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Cell Height.</summary>
    [CssVar(OtpField.CellHeight)] public required string CellHeight { get; init; }

    /// <summary>Cell Width.</summary>
    [CssVar(OtpField.CellWidth)] public required string CellWidth { get; init; }

    /// <summary>Focus Ring Width.</summary>
    [CssVar(OtpField.FocusRingWidth)] public required string FocusRingWidth { get; init; }

    /// <summary>Font Size.</summary>
    [CssVar(OtpField.FontSize)] public required string FontSize { get; init; }

    /// <summary>Font Weight.</summary>
    [CssVar(OtpField.FontWeight)] public required string FontWeight { get; init; }
}
