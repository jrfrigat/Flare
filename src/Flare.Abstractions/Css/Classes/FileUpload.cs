namespace Flare.Css.Classes;

/// <summary>CSS classes for file upload.</summary>
public static class FileUpload
{
    /// <summary>The <c>flare-file-upload</c> CSS class.</summary>
    public const string Root = "flare-file-upload";
    /// <summary>The <c>flare-file-upload--button</c> CSS class.</summary>
    public const string Button = "flare-file-upload--button";
    /// <summary>The <c>flare-file-upload--dragging</c> CSS class.</summary>
    public const string Dragging = "flare-file-upload--dragging";
    /// <summary>The <c>flare-file-upload--disabled</c> CSS class.</summary>
    public const string Disabled = "flare-file-upload--disabled";
    /// <summary>The <c>flare-file-upload__drop-wrap</c> CSS class.</summary>
    public const string DropWrap = "flare-file-upload__drop-wrap";
    /// <summary>The <c>flare-file-upload__zone</c> CSS class.</summary>
    public const string Zone = "flare-file-upload__zone";
    /// <summary>The <c>flare-file-upload__button</c> CSS class.</summary>
    public const string ButtonTrigger = "flare-file-upload__button";
    /// <summary>The <c>flare-file-upload__icon</c> CSS class.</summary>
    public const string Icon = "flare-file-upload__icon";
    /// <summary>The <c>flare-file-upload__primary</c> CSS class.</summary>
    public const string Primary = "flare-file-upload__primary";
    /// <summary>The <c>flare-file-upload__hint</c> CSS class.</summary>
    public const string Hint = "flare-file-upload__hint";
    /// <summary>The <c>flare-file-upload__input</c> CSS class.</summary>
    public const string Input = "flare-file-upload__input";
    /// <summary>The <c>flare-file-upload__list</c> CSS class.</summary>
    public const string List = "flare-file-upload__list";
    /// <summary>The <c>flare-file-upload__file</c> CSS class.</summary>
    public const string File = "flare-file-upload__file";
    /// <summary>The <c>flare-file-upload__file-icon</c> CSS class.</summary>
    public const string FileIcon = "flare-file-upload__file-icon";
    /// <summary>The <c>flare-file-upload__file-name</c> CSS class.</summary>
    public const string FileName = "flare-file-upload__file-name";
    /// <summary>The <c>flare-file-upload__file-size</c> CSS class.</summary>
    public const string FileSize = "flare-file-upload__file-size";
    /// <summary>The <c>flare-file-upload__file-error</c> CSS class - the message on a failed row.</summary>
    public const string FileError = "flare-file-upload__file-error";
    /// <summary>The <c>flare-file-upload__file-actions</c> CSS class - the cancel/retry affordances.</summary>
    public const string FileActions = "flare-file-upload__file-actions";
    /// <summary>The <c>flare-file-upload__progress</c> CSS class - the per-row transfer bar.</summary>
    public const string Progress = "flare-file-upload__progress";

    // A modifier per queue state that HAS a distinct look. Queued has none - the indeterminate bar already
    // says the row is waiting - so it gets no class rather than a class with no rule behind it.
    /// <summary>The <c>flare-file-upload__file--uploading</c> CSS class.</summary>
    public const string FileUploading = "flare-file-upload__file--uploading";
    /// <summary>The <c>flare-file-upload__file--completed</c> CSS class.</summary>
    public const string FileCompleted = "flare-file-upload__file--completed";
    /// <summary>The <c>flare-file-upload__file--failed</c> CSS class.</summary>
    public const string FileFailed = "flare-file-upload__file--failed";
    /// <summary>The <c>flare-file-upload__file--cancelled</c> CSS class.</summary>
    public const string FileCancelled = "flare-file-upload__file--cancelled";
}
