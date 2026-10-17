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
    [CssVar(FileUploadField.BorderWidth)] public string BorderWidth { get; init; } = "var(--flare-border-width-emphasis)";

    /// <summary>Background of the drop zone on hover.</summary>
    [CssVar(FileUploadField.HoverBg)] public string HoverBg { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 5%, var(--flare-color-surface))";

    /// <summary>Background of the drop zone while a file is dragged over it.</summary>
    [CssVar(FileUploadField.DraggingBg)] public string DraggingBg { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 10%, var(--flare-color-surface))";

    /// <summary>Width of the inset ring drawn while a file is dragged over the drop zone.</summary>
    [CssVar(FileUploadField.DraggingRingWidth)] public required string DraggingRingWidth { get; init; }

    /// <summary>Size of the drop zone's large upload glyph.</summary>
    [CssVar(FileUploadField.IconSize)] public required string IconSize { get; init; }

    /// <summary>Resting height of the drop zone.</summary>
    [CssVar(FileUploadField.ZoneMinHeight)] public required string ZoneMinHeight { get; init; }

    /// <summary>Corner radius of the drop zone.</summary>
    [CssVar(FileUploadField.ZoneRadius)] public string ZoneRadius { get; init; } = "var(--flare-card-radius)";

    /// <summary>Size of the per-file glyph in the selected-files list.</summary>
    [CssVar(FileUploadField.FileIconSize)] public required string FileIconSize { get; init; }

    /// <summary>Gap between the file rows.</summary>
    [CssVar(FileUploadField.RowGap)] public string RowGap { get; init; } = "var(--flare-spacing-2)";

    /// <summary>Corner radius of one file row.</summary>
    [CssVar(FileUploadField.RowRadius)] public string RowRadius { get; init; } = "var(--flare-shape-extra-small)";

    /// <summary>Background of a row whose transfer is running.</summary>
    [CssVar(FileUploadField.RowActiveBg)] public string RowActiveBg { get; init; } = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)";

    /// <summary>Background of a completed row.</summary>
    [CssVar(FileUploadField.RowSuccessBg)] public string RowSuccessBg { get; init; } = "color-mix(in srgb, var(--flare-color-tertiary) 8%, transparent)";

    /// <summary>Background of a failed or cancelled row.</summary>
    [CssVar(FileUploadField.RowErrorBg)] public string RowErrorBg { get; init; } = "color-mix(in srgb, var(--flare-color-error) 8%, transparent)";

    /// <summary>Foreground of the message on a failed row - the one place a component would otherwise
    /// hardcode red.</summary>
    [CssVar(FileUploadField.RowErrorColor)] public string RowErrorColor { get; init; } = "var(--flare-color-error)";
}
