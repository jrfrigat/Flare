using Microsoft.JSInterop;

namespace Flare.Components;

/// <summary>Default <see cref="IFlareClipboard"/> over the browser clipboard.</summary>
public sealed class FlareClipboardService(IJSRuntime js) : IFlareClipboard
{
    /// <summary>Copies the given text to the system clipboard.</summary>
    public async ValueTask CopyAsync(string text)
    {
        try { await js.InvokeVoidAsync("navigator.clipboard.writeText", text); }
        catch { await js.InvokeVoidAsync("FlareClipboardFallback.copy", text); }
    }

    /// <summary>Reads the current text contents of the system clipboard.</summary>
    public async ValueTask<string> ReadAsync()
    {
        try { return await js.InvokeAsync<string>("flareOtp.getClipboardText"); }
        catch { return string.Empty; }
    }
}
