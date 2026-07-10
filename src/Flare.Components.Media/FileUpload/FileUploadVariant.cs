namespace Flare.Components;

/// <summary>The visual form a <see cref="FlareFileUpload"/> presents.</summary>
public enum FileUploadVariant
{
    /// <summary>A large dashed drag-and-drop area that also opens the file dialog on click (default).</summary>
    DropZone,
    /// <summary>A single compact button that opens the OS file dialog - no drop area.</summary>
    Button,
}
