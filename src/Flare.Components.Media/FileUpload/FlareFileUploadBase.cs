using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Flare.Components;

/// <summary>
/// What <see cref="FlareFileUploadZone"/> and <see cref="FlareFileUploadButton"/> share: the hidden file
/// input, the accept/multiple/limit rules, the selected-file list and the change callback. Only the trigger
/// - a drop region or a button in a row - differs, and each subclass owns just that.
/// </summary>
public abstract class FlareFileUploadBase : FlareComponentBase
{
    /// <summary>Callback invoked with the list of selected files.</summary>
    [Parameter] public EventCallback<IReadOnlyList<IBrowserFile>> OnFilesChanged { get; set; }
    /// <summary>Accepted file types as a MIME or extension filter string.</summary>
    [Parameter] public string? Accept { get; set; }
    /// <summary>Allows selecting multiple files when true.</summary>
    [Parameter] public bool Multiple { get; set; }
    /// <summary>Disables the control - the trigger stops opening the file picker.</summary>
    [Parameter] public bool Disabled { get; set; }
    /// <summary>Maximum number of files that can be selected.</summary>
    [Parameter] public int MaxFiles { get; set; } = 10;
    /// <summary>Shows the list of selected files under the trigger. Default true.</summary>
    [Parameter] public bool ShowFileList { get; set; } = true;

    /// <summary>The files the user has selected, in selection order.</summary>
    protected readonly List<IBrowserFile> Files = [];

    /// <summary>Id linking the trigger's <c>&lt;label for&gt;</c> to the hidden input.</summary>
    protected readonly string InputId = $"flare-fu-{Guid.NewGuid():N}";

    /// <summary>Reads the picked files, caps them at <see cref="MaxFiles"/> and raises
    /// <see cref="OnFilesChanged"/>.</summary>
    protected async Task HandleChangeAsync(InputFileChangeEventArgs e)
    {
        Files.Clear();
        Files.AddRange(e.GetMultipleFiles(MaxFiles));
        await OnFilesChanged.InvokeAsync(Files.AsReadOnly());
    }

    /// <summary>Renders a byte count as a short human-readable size (B / KB / MB).</summary>
    protected static string FormatSize(long bytes) => bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _ => $"{bytes / (1024.0 * 1024):F1} MB",
    };
}
