using Flare.Css;
using Flare.Css.Tokens;
namespace Flare.Abstractions.Tokens.Components;

/// <summary>
/// Design tokens for file upload - the drop zone's geometry and drag feedback, read by fileupload.css.
/// The button form takes no tokens of its own: it wears the button family's, which is the point of it.
/// </summary>
public sealed record FileUploadTokens
{
    /// <summary>Width of the drop zone's dashed border.</summary>
    [CssVar(FileUploadField.BorderWidth)] public required string BorderWidth { get; init; }

    /// <summary>Background of the drop zone on hover.</summary>
    [CssVar(FileUploadField.HoverBg)] public required string HoverBg { get; init; }

    /// <summary>Background of the drop zone while a file is dragged over it.</summary>
    [CssVar(FileUploadField.DraggingBg)] public required string DraggingBg { get; init; }

    /// <summary>Width of the inset ring drawn while a file is dragged over the drop zone.</summary>
    [CssVar(FileUploadField.DraggingRingWidth)] public required string DraggingRingWidth { get; init; }

    /// <summary>Size of the drop zone's large upload glyph.</summary>
    [CssVar(FileUploadField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Resting height of the drop zone.</summary>
    [CssVar(FileUploadField.ZoneMinHeight)] public required string ZoneMinHeight { get; init; }

    /// <summary>Corner radius of the drop zone.</summary>
    [CssVar(FileUploadField.ZoneRadius)] public required string ZoneRadius { get; init; }

    /// <summary>Size of the per-file glyph in the selected-files list.</summary>
    [CssVar(FileUploadField.FileIconSize)] public required string FileIconSize { get; init; }
}
