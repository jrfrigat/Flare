using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>Design tokens for Dropzone - component-specific geometry read by Dropzone.css.</summary>
public sealed record DropzoneTokens
{
    /// <summary>Border Width.</summary>
    [CssVar(DropzoneField.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Hover Bg.</summary>
    [CssVar(DropzoneField.HoverBg)] public required string HoverBg { get; init; }

    /// <summary>Dragging Bg.</summary>
    [CssVar(DropzoneField.DraggingBg)] public required string DraggingBg { get; init; }

    /// <summary>Dragging Ring Width.</summary>
    [CssVar(DropzoneField.DraggingRingWidth)] public required string DraggingRingWidth { get; init; }

    /// <summary>Icon Size.</summary>
    [CssVar(DropzoneField.IconSize)] public required string IconSize { get; init; }
}
